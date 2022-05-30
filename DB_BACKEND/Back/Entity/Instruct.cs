using System;
using System.Collections.Generic;

#nullable disable

namespace Back.Entity
{
    public partial class Instruct
    {
        public decimal CourseId { get; set; }
        public decimal InstructorId { get; set; }

        public virtual Course Course { get; set; }
        public virtual Instructor Instructor { get; set; }
    }
}
