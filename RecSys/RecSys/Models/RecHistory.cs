using System;
using System.Collections.Generic;

namespace RecSys.Models;

public partial class RecHistory
{
    public int HistoryId { get; set; }

    public Guid AnonymousId { get; set; }

    public string? PreferredCourses { get; set; }

    public string? ProjectFocus { get; set; }

    public DateTime? SubmittedAt { get; set; }

    public virtual ICollection<RecRating> RecRatings { get; set; } = new List<RecRating>();
}
