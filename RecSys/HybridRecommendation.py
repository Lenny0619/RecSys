import os
import pandas as pd
from flask_babel import Babel
from sqlalchemy import create_engine
from sklearn.feature_extraction.text import CountVectorizer
from sklearn.metrics.pairwise import cosine_similarity
from surprise import SVD, Dataset, Reader

DATABASE_URL = "mssql+pyodbc://Lenny:Buvu%400619@recsysdb.database.windows.net:1433/RecSys_db?driver=ODBC+Driver+17+for+SQL+Server"
engine = create_engine(DATABASE_URL)
##engine = create_engine("mssql+pyodbc://localhost\\SQLEXPRESS/RecSys?driver=SQL+Server&trusted_connection=yes")
#engine = create\_engine("mssql+pyodbc://sa\:YourStrong!Passw0rd\@10.100.12.49:1433/RecSys?driver=ODBC+Driver+17+for+SQL+Server")
#engine = create\_engine("mssql+pyodbc://sa\:YourStrong!Passw0rd\@sqlserver:1433/RecSys?driver=ODBC+Driver+17+for+SQL+Server")

def load_data():
    """
    Load data from SQL Server with new lecturer identification.
    """
    try:
        conn = engine.connect()
        print("✅ Database connected!")
    except Exception as e:
        print("❌ Database not connected!", str(e))
        return None

    try:
        # Load all tables
        course = pd.read_sql("SELECT * FROM Course", conn)
        course_lecturer = pd.read_sql("SELECT * FROM CourseLecturer", conn)
        lecturers = pd.read_sql("SELECT * FROM Lecturers", conn)
        past_students_df = pd.read_sql("SELECT studentId, svId, rating FROM past_students_choices", conn)
        rec_ratings_df = pd.read_sql("SELECT HistoryId, RecommendedLecturerId, SubmittedRating FROM RecRatings", conn)
        
        # Combine ratings and identify which lecturers have ratings
        rec_ratings_df = rec_ratings_df.rename(columns={
            'HistoryId': 'studentId',
            'RecommendedLecturerId': 'svId',
            'SubmittedRating': 'rating'
        })
        all_ratings = pd.concat([past_students_df, rec_ratings_df], ignore_index=True)
        
        # Mark new lecturers (those with no ratings)
        rated_lecturers = all_ratings['svId'].unique()
        lecturers['is_new'] = ~lecturers['svId'].isin(rated_lecturers)

        conn.close()
        print("✅ Data loaded successfully!")
        return all_ratings, lecturers, course, course_lecturer

    except Exception as e:
        print("❌ Error loading data:", str(e))
        return None

def train_svd(ratings):
    """
    Train SVD model only on existing ratings.
    """
    print("🔄 Training SVD model...")
    reader = Reader(rating_scale=(3, 5))
    data = Dataset.load_from_df(ratings[['studentId', 'svId', 'rating']], reader)
    trainset = data.build_full_trainset()
    algo = SVD()
    algo.fit(trainset)
    print("✅ SVD model trained!")
    return algo

def calculate_cosine_similarity(project_focus, expertise_list):
    """
    Calculate cosine similarity between project focus and lecturers' expertise.
    """
    vectorizer = CountVectorizer().fit_transform([project_focus] + expertise_list)
    vectors = vectorizer.toarray()
    cosine_similarities = cosine_similarity(vectors[0:1], vectors[1:])
    print("✅ Cosine similarities calculated!")
    return cosine_similarities[0]

def find_course(course_list, input_name):
    """
    Find course based on user input.
    """
    input_name = input_name.lower().strip()
    for course in course_list:
        if input_name in course['abbreviations'].lower():
            return course
    return None

def recommend_supervisors(course_names_input, project_focus):
    try:
        print("🔄 Starting recommendation process...")
        ratings, lecturers, courses, course_lecturer = load_data()
        if ratings is None or lecturers is None or courses is None or course_lecturer is None:
            print("❌ Error: Data not loaded properly.")
            return []

        # Train SVD only if we have ratings
        if not ratings.empty:
            svd_algo = train_svd(ratings)
        else:
            svd_algo = None
            print("⚠️ No ratings available - using content-based only")

        # Preprocess course data
        courses['abbreviations'] = courses['abbreviations'].fillna('').astype(str).str.strip().str.lower() + " " + \
                                 courses['courseName'].fillna('').astype(str).str.strip().str.lower()
        course_list = courses.to_dict(orient='records')

        # Match user input courses
        course_names = [name.strip().lower() for name in course_names_input.split(',')]
        course_ids = []
        for name in course_names:
            matched_course = find_course(course_list, name)
            if matched_course:
                course_ids.append(matched_course['courseId'])
            else:
                print(f"⚠️ No match found for '{name}'")
        if not course_ids:
            print("❌ No matching courses found.")
            return []

        print(f"✅ Matched courses: {course_ids}")

        # Find eligible lecturers
        lecturers_for_courses = course_lecturer[course_lecturer['courseId'].isin(course_ids)]['svId'].unique()
        eligible_lecturers = lecturers[lecturers['eligibility'] == 1].copy()
        
        # Calculate expertise similarity for all
        lecturer_expertise_list = eligible_lecturers['svExpertise'].tolist()
        cosine_similarities = calculate_cosine_similarity(project_focus, lecturer_expertise_list)
        
        # Initialize scores
        eligible_lecturers['cosine_similarity'] = cosine_similarities
        eligible_lecturers['taught_course'] = eligible_lecturers['svId'].isin(lecturers_for_courses).map({True: "Yes", False: "No"})
        eligible_lecturers['taught_course_flag'] = eligible_lecturers['svId'].isin(lecturers_for_courses).astype(int)
        
        # Calculate predicted ratings differently for new vs existing lecturers
        if svd_algo is not None:
            # For existing lecturers: get SVD prediction
            existing_mask = ~eligible_lecturers['is_new']
            eligible_lecturers.loc[existing_mask, 'predicted_rating'] = eligible_lecturers[existing_mask]['svId'].apply(
                lambda x: svd_algo.predict(0, x).est
            )
            
            # For new lecturers: use base rating of 3.0 (slightly below average)
            new_mask = eligible_lecturers['is_new']
            eligible_lecturers.loc[new_mask, 'predicted_rating'] = 3.0
        else:
            # If no ratings at all, use base rating for everyone
            eligible_lecturers['predicted_rating'] = 4.0

        # Normalize ratings to 0-1 scale
        eligible_lecturers['norm_rating'] = (eligible_lecturers['predicted_rating'] * 0.1)  # Normalize 4-5 to 0-1
        
        # Calculate composite score with different weights for new vs existing
        existing_mask = ~eligible_lecturers['is_new']
        new_mask = eligible_lecturers['is_new']
        
        # Existing lecturers: 50% CF, 50% CB
        eligible_lecturers.loc[existing_mask, 'composite_score'] = (
            0.5 * eligible_lecturers.loc[existing_mask, 'cosine_similarity'] + 
            0.5 * eligible_lecturers.loc[existing_mask, 'norm_rating'] +
            eligible_lecturers.loc[existing_mask, 'taught_course_flag'] * 0.3
        )
        
        # New lecturers: 80% CB, 20% base rating
        eligible_lecturers.loc[new_mask, 'composite_score'] = (
            0.8 * eligible_lecturers.loc[new_mask, 'cosine_similarity'] + 
            0.2 * eligible_lecturers.loc[new_mask, 'norm_rating'] +
            eligible_lecturers.loc[new_mask, 'taught_course_flag'] * 0.3
        )

        # Sort by composite score (highest first)
        eligible_lecturers['has_cosine'] = eligible_lecturers['cosine_similarity'] > 0
        sorted_lecturers = eligible_lecturers.sort_values(
            ['has_cosine', 'composite_score'], 
            ascending=[False, False]
        )

        # Prepare output for ALL eligible lecturers
        recommendations = []
        for _, lecturer in sorted_lecturers.iterrows():
            rec = {
                'id': int(lecturer['svId']),
                'name': str(lecturer['svName']),
                'expertise': str(lecturer['svExpertise']),
                'email': str(lecturer['email']),
                'image_path': str(lecturer['ImagePath']),
                'predicted_rating': float(lecturer['predicted_rating']),
                'cosine_similarity': float(lecturer['cosine_similarity']),
                'taught_course': str(lecturer['taught_course']),
                'composite_score': float(lecturer['composite_score']),
                'is_new_lecturer': bool(lecturer['is_new']),
                'score_explanation': "ContentBased (80%) + Base Rating (20%)" if lecturer['is_new'] else "ContentBased (50%) + Collaborative (50%)"
            }
            recommendations.append(rec)

        # Debug output
        print("\n" + "="*100)
        print("ALL ELIGIBLE LECTURERS WITH SCORES")
        print("="*100)
        print(f"{'Lecturer Name':<25} {'Status':<10} {'Taught':<6} {'Cosine':<8} {'PredRt':<6} {'NormRt':<8} {'CompositeSco':<10}{'Explanation'}")
        print("-"*100)
        for rec in recommendations:
            status = "NEW" if rec['is_new_lecturer'] else "EXISTING"
            print(f"{rec['name'][:24]:<25} {status:<10} {rec['taught_course']:<6} {rec['cosine_similarity']:.3f} "
                  f"{rec['predicted_rating']:.2f} {rec['predicted_rating']*0.1:<8.3f} {rec['composite_score']:.3f} {rec['score_explanation']}")
        print("-"*100)
        print("\n" + "="*100)

        return recommendations

    except Exception as e:
        print("❌ Error in recommendation process:", str(e))
        return []