using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RecSys.Models;
using RecSys.Services;
using System.Threading.Tasks;
using System;
using System.Linq;
using System.Web;

namespace RecSys.Pages
{
    public class LecturerDetailsModel : PageModel
    {
        private readonly ILecturerService _lecturerService;

        public LecturerDetailsModel(ILecturerService lecturerService)
        {
            _lecturerService = lecturerService;
        }

        [BindProperty]
        public LecturerViewModel Lecturer { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public int HistoryId { get; set; }

        [BindProperty]
        public int RatingValue { get; set; }

        [BindProperty(SupportsGet = true)]
        public int CurrentRatingCount { get; set; }

        [BindProperty(SupportsGet = true)]
        public int TotalLecturersToRate { get; set; }

        public async Task<IActionResult> OnGetAsync(
                int id,
                string? name = null,
                string? imagePath = null,
                double rating = 0,
                string? email = null,
                double cosineSimilarity = 0,
                string? expertise = null,
                string? taughtCourse = null,
                double predictedRating = 0,
                int historyId = 0,
                int currentRatingCount = 0,
                int totalLecturersToRate = 0)
        {
            HistoryId = historyId;
            CurrentRatingCount = currentRatingCount;

            // Get total from session if not provided
            TotalLecturersToRate = totalLecturersToRate > 0 ? totalLecturersToRate :
                HttpContext.Session.GetInt32("TotalLecturersToRate") ?? 0;

            // Ensure we have a valid total
            if (TotalLecturersToRate <= 0)
            {
                // Fallback to a reasonable default if needed
                TotalLecturersToRate = 999;
            }

            // Try to get from URL parameters first
            if (!string.IsNullOrEmpty(name))
            {
                Lecturer = new LecturerViewModel
                {
                    SvId = id,
                    SvName = name,
                    ImagePath = imagePath ?? "/images/default-profile.png",
                    Email = email ?? "No email provided",
                    CosineSimilarity = cosineSimilarity,
                    SvExpertise = expertise ?? "No expertise listed",
                    TaughtCourse = taughtCourse ?? "No courses listed",
                    PredictedRating = predictedRating
                };
                return Page();
            }

            // Fallback to database lookup
            var lecturerEntity = await _lecturerService.GetLecturerFromDbAsync(id);
            if (lecturerEntity == null)
                return NotFound();

            Lecturer = new LecturerViewModel
            {
                SvId = lecturerEntity.SvId,
                SvName = lecturerEntity.SvName,
                ImagePath = lecturerEntity.ImagePath ?? "/images/default-profile.png",
                Email = lecturerEntity.Email ?? "No email provided",
                SvExpertise = lecturerEntity.SvExpertise ?? "No expertise listed",
                PredictedRating = await _lecturerService.GetPredictedRatingFromApiAsync(lecturerEntity.SvId),
                CosineSimilarity = cosineSimilarity,
                TaughtCourse = taughtCourse ?? "No courses listed"
            };

            return Page();
        }
        public async Task<IActionResult> OnPostSubmitRatingAsync()
        {
            if (RatingValue < 1 || RatingValue > 5)
            {
                return new JsonResult(new { success = false, message = "Please select a valid rating between 1 and 5" });
            }

            try
            {
                var success = await _lecturerService.SubmitRatingAsync(Lecturer.SvId, RatingValue, HistoryId);
                var newRating = await _lecturerService.GetPredictedRatingFromApiAsync(Lecturer.SvId);

                // Calculate new rating count
                var newRatingCount = CurrentRatingCount + 1;

                // Ensure TotalLecturersToRate is valid
                if (TotalLecturersToRate <= 0)
                {
                    TotalLecturersToRate = HttpContext.Session.GetInt32("TotalLecturersToRate") ?? 8;
                }

                return new JsonResult(new
                {
                    success = success,
                    message = success ? "Appreciate your time! Your rating will help enhance the model’s accuracy" : "Unable to submit rating after two attempts",
                    newRating = newRating,
                    currentRatingCount = newRatingCount,
                    totalLecturersToRate = TotalLecturersToRate,
                    shouldGoBack = newRatingCount < 2 && newRatingCount < TotalLecturersToRate,
                    showFinishButton = newRatingCount >= 2,
                    shouldRedirectToIndex = newRatingCount >= TotalLecturersToRate
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    success = false,
                    message = "An error occurred while submitting your rating"
                });
            }
        }
    }
}