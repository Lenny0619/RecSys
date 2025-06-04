using System;
using System.Collections.Generic;

namespace RecSys.Models;

public partial class CourseLecturer
{
    public int SvId { get; set; }

    public int CourseId { get; set; }

    public int Year { get; set; }

    public int Sem { get; set; }

    public virtual Course? Course { get; set; }
    public virtual Lecturer? Sv { get; set; }
}
