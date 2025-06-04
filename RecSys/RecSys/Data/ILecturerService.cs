using RecSys.Models;
using System.Threading.Tasks;

namespace RecSys.Services
{
    public interface ILecturerService
    {
        Task<Lecturer?> GetLecturerFromDbAsync(int id);
        Task<double> GetPredictedRatingFromApiAsync(int svId);
        Task<bool> SubmitRatingAsync(int lecturerSvId, int rating, int historyId);
        Task<int> GetRecommendedLecturersCountAsync(int historyId);
        Task<int?> GetNextLecturerToRateAsync(int historyId, int currentLecturerId);
    }
}