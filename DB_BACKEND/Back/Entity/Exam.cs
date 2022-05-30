using System;
using System.Collections.Generic;

#nullable disable

namespace Back.Entity
{
    public partial class Exam
    {
        public Exam()
        {
            Grades = new HashSet<Grade>();
        }

        public decimal ExamId { get; set; }
        public decimal? CourseId { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int? MeetingId { get; set; }

        public virtual Course Course { get; set; }
        public virtual ICollection<Grade> Grades { get; set; }
    }
}
