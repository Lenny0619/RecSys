using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecSys.Models;

public partial class Course
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int CourseId { get; set; }

    public string? CourseCode { get; set; }

    public string? CourseName { get; set; }

    public string? Abbreviations { get; set; }

    public virtual ICollection<CourseLecturer> CourseLecturers { get; set; } = new List<CourseLecturer>();
}
