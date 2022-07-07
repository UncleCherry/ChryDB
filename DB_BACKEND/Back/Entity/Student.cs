using System;
using System.Collections.Generic;

#nullable disable

namespace Back.Entity
{
    public partial class Student
    {
        public Student()
        {
            Attendances = new HashSet<Attendance>();
            Grades = new HashSet<Grade>();
            Takes = new HashSet<Take>();
        }

        public decimal StudentId { get; set; }
        public string Name { get; set; }
        public decimal? Grade { get; set; }
        public string Major { get; set; }
        public decimal? Credit { get; set; }

        public virtual User StudentNavigation { get; set; }
        public virtual TrainingPlan TrainingPlan { get; set; }
        public virtual ICollection<Attendance> Attendances { get; set; }
        public virtual ICollection<Grade> Grades { get; set; }
        public virtual ICollection<Take> Takes { get; set; }
    }
}
