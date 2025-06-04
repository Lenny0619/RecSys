using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecSys.Models
{
    public partial class Lecturer
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SvId { get; set; }

        [Required(ErrorMessage = "Lecturer name is required")]
        [Display(Name = "Lecturer Name")]
        public string? SvName { get; set; }

        [Display(Name = "Expertise")]
        public string? SvExpertise { get; set; }

        [Display(Name = "Eligibility")]
        public int? Eligibility { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string? Email { get; set; }

        [Display(Name = "Profile Image")]
        public string? ImagePath { get; set; }

        public virtual ICollection<CourseLecturer> CourseLecturers { get; set; } = new List<CourseLecturer>();
    }
}