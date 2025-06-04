using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecSys.Models
{
    public partial class RecRating
    {
        public int RatingId { get; set; }
        public int HistoryId { get; set; }
        public int? RecommendedLecturerId { get; set; }

        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        [Required]
        public int? SubmittedRating { get; set; }

        public DateTime? SubmittedAt { get; set; }

        public virtual RecHistory History { get; set; } = null!;
        public virtual Lecturer? RecommendedLecturer { get; set; }
    }
}
