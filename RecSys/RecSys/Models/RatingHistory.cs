using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecSys.Models
{
    public class RatingHistory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int HistoryId { get; set; }

        [Required]
        [StringLength(50)]
        public string AnonymousId { get; set; }

        [Required]
        [StringLength(200)]
        public string PreferredCourses { get; set; }

        [Required]
        [StringLength(200)]
        public string ProjectFocus { get; set; }

        [Required]
        public int RecommendedLecturers { get; set; }

        [Required]
        [Range(1, 5)]
        public int SubmittedRatings { get; set; }

        [Required]
        public DateTime SubmittedAt { get; set; }
    }
}
