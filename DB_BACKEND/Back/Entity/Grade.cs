using System;
using System.Collections.Generic;

#nullable disable

namespace Back.Entity
{
    public partial class Grade
    {
        public decimal ExamId { get; set; }
        public decimal StudentId { get; set; }
        public byte? Grade_ { get; set; }

        public virtual Exam Exam { get; set; }
        public virtual Student Student { get; set; }
    }
}
