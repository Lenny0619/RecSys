using Microsoft.EntityFrameworkCore;
using RecSys.Models;

namespace RecSys.Data
{
    public interface ISupervisorRecommendationService
    {
        Task<IEnumerable<Lecturer>> GetRecommendedSupervisorsAsync(string course1, string course2, string course3, string projectFocus);
    }
}