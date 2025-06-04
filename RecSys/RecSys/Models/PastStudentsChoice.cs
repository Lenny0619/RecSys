using System;
using System.Collections.Generic;

namespace RecSys.Models;

public partial class PastStudentsChoice
{
    public int StudentId { get; set; }

    public int? SvId { get; set; }

    public string? ProjectFocus { get; set; }

    public double? Rating { get; set; }
}
