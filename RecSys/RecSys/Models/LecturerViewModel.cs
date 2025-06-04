namespace RecSys.Models;

public class LecturerViewModel
{
    public int SvId { get; set; }
    public string SvName { get; set; } = ""; // 确保非null
    public string SvExpertise { get; set; } = "";
    public string Email { get; set; } = "";
    public string ImagePath { get; set; } = "/images/default-profile.png";
    public double PredictedRating { get; set; }
    public double CosineSimilarity { get; set; }
    public string TaughtCourse { get; set; } = "";
    public double CompositeScore { get; set; }
}
