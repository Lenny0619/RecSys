using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RecSys.Models;

namespace RecSys.Pages
{
    public class ResultPageModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public ResultPageModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Changed property name to match view (or update view to use "Lecturers")
        public List<LecturerViewModel> Lecturers { get; set; } = new List<LecturerViewModel>();

        [BindProperty(SupportsGet = true)]
        public int HistoryId { get; set; }

        [BindProperty(SupportsGet = true)]
        public int RatedCount { get; set; }

        public async Task<IActionResult> OnGetAsync(string courseNames, string projectFocus, int historyId, int ratedCount = 0)
        {
            try
            {
                // Store parameters for the view
                ViewData["CourseNames"] = courseNames;
                ViewData["ProjectFocus"] = projectFocus;
                RatedCount = ratedCount;
                HistoryId = historyId;

                var payload = new
                {
                    course_names = courseNames?.Split(','),
                    project_focus = projectFocus,
                    history_id = historyId
                };

                var response = await _httpClient.PostAsJsonAsync("http://localhost:5000/process", payload);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var apiResponse = JsonSerializer.Deserialize<ApiResponseWrapper>(json, options);

                Lecturers = apiResponse?.Recommendations?
                    .OrderByDescending(l => l.CompositeScore)
                    .Select(r => new LecturerViewModel
                    {
                        SvId = r.Id,
                        SvName = r.Name,
                        SvExpertise = r.Expertise,
                        Email = r.Email,
                        ImagePath = r.ImagePath,
                        PredictedRating = r.PredictedRating,
                        CosineSimilarity = r.CosineSimilarity,
                        TaughtCourse = r.TaughtCourse,
                        CompositeScore = r.CompositeScore
                    })
                    .ToList() ?? new List<LecturerViewModel>();

                // Store the total count in session
                HttpContext.Session.SetInt32("TotalLecturersToRate", Lecturers.Count);
                ViewData["TotalLecturersToRate"] = Lecturers.Count;

                return Page();
            }
            catch (Exception ex)
            {
                Lecturers = new List<LecturerViewModel>();
                return Page();
            }
        }

        private class ApiResponseWrapper
        {
            public List<LecturerApiResponse> Recommendations { get; set; }
        }

        private class LecturerApiResponse
        {
            public int Id { get; set; }  // Changed from LecturerId to match Python API
            public string Name { get; set; }
            public string Expertise { get; set; }
            public string Email { get; set; }
            public string ImagePath { get; set; }
            public double PredictedRating { get; set; }
            public double CosineSimilarity { get; set; }
            public string TaughtCourse { get; set; }
            public double CompositeScore { get; set; }
        }
    }
}