import os
import pandas as pd
from flask_babel import Babel
from flask_cors import CORS
from flask import Flask, request, jsonify
from sqlalchemy import create_engine, text
from apscheduler.schedulers.background import BackgroundScheduler
from HybridRecommendation import recommend_supervisors, train_svd, load_data

app = Flask(__name__)
CORS(app)

DATABASE_URL = "mssql+pyodbc://Lenny:Buvu%400619@recsysdb.database.windows.net:1433/RecSys_db?driver=ODBC+Driver+17+for+SQL+Server"
engine = create_engine(DATABASE_URL)
##DATABASE_URL = "mssql+pyodbc://sqlserver\\SQLEXPRESS/RecSys?driver=SQL+Server"
#DATABASE_URL = "mssql+pyodbc://sa:YourStrong!Passw0rd@10.100.12.49:1433/RecSys?driver=ODBC+Driver+17+for+SQL+Server"
#DATABASE_URL = "mssql+pyodbc://sa:YourStrong!Passw0rd@localhost,1434/RecSys?driver=ODBC+Driver+17+for+SQL+Server"

svd_algo = None
ratings, lecturers, courses, course_lecturer = load_data()

def retrain_svd():
    global svd_algo, ratings
    if ratings is not None:
        print("🔄 Retraining SVD model...")
        svd_algo = train_svd(ratings)
        print("✅ SVD model retrained successfully!")
    else:
        print("❌ No ratings data available for retraining.")

retrain_svd()

scheduler = BackgroundScheduler()
scheduler.add_job(retrain_svd, 'interval', days=2)
scheduler.start()
    
@app.route('/test-db-connection')
def test_db_connection():
    result = load_data()
    if result:
        return jsonify({"status": "success", "message": "DB connected and data loaded"})
    else:
        return jsonify({"status": "fail", "message": "DB connection or data loading failed"}), 500

@app.route('/retrain', methods=['POST'])
def retrain_model():
    try:
        global ratings, lecturers, courses, course_lecturer
        ratings, lecturers, courses, course_lecturer = load_data()
        
        retrain_svd()
        
        return jsonify({
            "success": True,
            "message": "Model retrained successfully with latest ratings"
        })
    except Exception as e:
        print(f"Error retraining model: {str(e)}")
        return jsonify({
            "success": False,
            "message": f"Error retraining model: {str(e)}"
        }), 500

@app.route('/process', methods=['POST'])
def process_recommendation():
    try:
        data = request.get_json()
        course_names = data.get('course_names', [])
        project_focus = data.get('project_focus', '')
        
        recommendations = recommend_supervisors(','.join(course_names), project_focus)
        
        if not recommendations:
            return jsonify({"error": "No recommendations found"}), 200
            
        # Return ALL eligible lecturers already sorted by cosine similarity and rating
        return jsonify({
            "Recommendations": [
                {
                    "id": rec['id'],
                    "name": rec['name'],
                    "expertise": rec['expertise'],
                    "email": rec['email'],
                    "imagePath": rec.get('image_path', ''),
                    "predictedRating": rec['predicted_rating'],
                    "cosineSimilarity": rec['cosine_similarity'],
                    "taughtCourse": rec.get('taught_course', '')
                }
                for rec in recommendations
            ]
        }), 200
        
    except Exception as e:
        return jsonify({"error": str(e)}), 500

@app.route('/predict_rating', methods=['GET'])
def predict_rating():
    sv_id = request.args.get('svId', type=int)
    if sv_id is None:
        return jsonify({"error": "Missing svId"}), 400
    predicted_rating = svd_algo.predict(0, sv_id).est
    return jsonify({"predicted_rating": predicted_rating})

@app.route('/get_lecturer_details/<int:lecturer_id>', methods=['GET'])
def get_lecturer_details(lecturer_id):
    try:
        lecturer = pd.read_sql(
            f"SELECT svId as id, svName as name, svExpertise as expertise, email, ImagePath as image_path FROM Lecturers WHERE svId = {lecturer_id}",
            engine
        ).to_dict(orient='records')[0]
        return jsonify(lecturer)
    except Exception as e:
        return jsonify({"error": str(e)}), 404

if __name__ == '__main__':
    app.run(host='0.0.0.0', port=5000, debug=True)