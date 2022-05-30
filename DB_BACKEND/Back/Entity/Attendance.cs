using System;
using System.Collections.Generic;

#nullable disable

namespace Back.Entity
{
    public partial class Attendance
    {
        public decimal AttendanceId { get; set; }
        public decimal? CourseId { get; set; }
        public decimal? StudentId { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public bool? IsEffective { get; set; }

        public virtual Course Course { get; set; }
        public virtual Student Student { get; set; }
    }
}
