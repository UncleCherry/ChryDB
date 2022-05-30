using System;
using System.Collections.Generic;

#nullable disable

namespace Back.Entity
{
    public partial class Take
    {
        public decimal CourseId { get; set; }
        public decimal StudentId { get; set; }

        public virtual Course Course { get; set; }
        public virtual Student Student { get; set; }
    }
}
