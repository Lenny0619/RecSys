using RecSys.Models;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace RecSys.Services
{
    public class LecturerService : ILecturerService
    {
        private readonly RecSysContext _context;
        private readonly HttpClient _httpClient;

        public LecturerService(RecSysContext context, HttpClient httpClient)
        {
            _context = context;
            _httpClient = httpClient;
        }

        public async Task<Lecturer?> GetLecturerFromDbAsync(int id)
        {
            return await _context.Lecturers
                .FirstOrDefaultAsync(l => l.SvId == id);
        }

        public async Task<double> GetPredictedRatingFromApiAsync(int svId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/predict_rating?svId={svId}");
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<RatingPrediction>(json);

                return result?.predicted_rating ?? 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting predicted rating: {ex.Message}");
                return 0;
            }
        }

        public async Task<bool> SubmitRatingAsync(int lecturerSvId, int rating, int historyId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Check if the combination already exists
                var existingRating = await _context.RecRatings
                    .FirstOrDefaultAsync(r => r.HistoryId == historyId && r.RecommendedLecturerId == lecturerSvId);

                if (existingRating != null)
                {
                    Console.WriteLine("Rating already exists for this HistoryId and LecturerId.");
                    return false;
                }

                // Save new rating
                var ratingEntity = new RecRating
                {
                    HistoryId = historyId,
                    RecommendedLecturerId = lecturerSvId,
                    SubmittedRating = rating,
                    SubmittedAt = DateTime.Now
                };

                _context.RecRatings.Add(ratingEntity);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                Console.WriteLine($"Rating saved - HistoryId: {historyId}, LecturerId: {lecturerSvId}, Rating: {rating}");

                // Fire-and-forget the retrain call (don't let it affect the rating submission)
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await _httpClient.PostAsync("/retrain", null);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Retrain failed: {ex.Message}");
                    }
                });

                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"Error submitting rating: {ex.Message}\n{ex.StackTrace}");
                return false;
            }
        }
        public async Task<int?> GetNextLecturerToRateAsync(int historyId, int currentLecturerId)
        {
            // This is a placeholder implementation - you'll need to replace it with your actual logic
            // to get the next lecturer that needs to be rated for this historyId

            // Example implementation:
            // 1. Get all recommended lecturers for this historyId
            // 2. Filter out the current lecturer
            // 3. Return the first one that hasn't been rated yet

            // For now, we'll just return null to demonstrate the flow
            return null;
        }
        public async Task<int> GetRecommendedLecturersCountAsync(int historyId)
        {
            return await _context.RecRatings
                .Where(r => r.HistoryId == historyId)
                .CountAsync();
        }
    }

    // Class for API response deserialization
    public class RatingPrediction
    {
        public double predicted_rating { get; set; }
    }
}