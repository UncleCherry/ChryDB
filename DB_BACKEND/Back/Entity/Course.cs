using System;
using System.Collections.Generic;

#nullable disable

namespace Back.Entity
{
    public partial class Course
    {
        public Course()
        {
            Attendances = new HashSet<Attendance>();
            Exams = new HashSet<Exam>();
            Instructs = new HashSet<Instruct>();
            Records = new HashSet<Record>();
            Takes = new HashSet<Take>();
        }

        public decimal CourseId { get; set; }
        public string CourseName { get; set; }
        public string TimeSlot { get; set; }
        public byte? Credit { get; set; }
        public bool? IsRequired { get; set; }
        public string Year { get; set; }
        public string Semester { get; set; }
        public int? MeetingId { get; set; }

        public virtual ICollection<Attendance> Attendances { get; set; }
        public virtual ICollection<Exam> Exams { get; set; }
        public virtual ICollection<Instruct> Instructs { get; set; }
        public virtual ICollection<Record> Records { get; set; }
        public virtual ICollection<Take> Takes { get; set; }
    }
}
