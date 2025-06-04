using Microsoft.EntityFrameworkCore;
using RecSys.Models;

namespace RecSys.Data
{
    public class SupervisorRecommendationService : ISupervisorRecommendationService
    {
        private readonly RecSysContext _context;

        public SupervisorRecommendationService(RecSysContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Lecturer>> GetRecommendedSupervisorsAsync(string course1, string course2, string course3, string projectFocus)
        {
            var courseCodes = new[] { course1, course2, course3 };

            var lecturers = await _context.CourseLecturers
                .Where(cl => courseCodes.Contains(cl.Course.CourseCode!))
                .Select(cl => cl.Sv)
                .Distinct()
                .Where(l => l.SvExpertise != null && l.SvExpertise.Contains(projectFocus))
                .ToListAsync();

            return lecturers;
        }
    }
}