using System;
using System.Collections.Generic;

#nullable disable

namespace Back.Entity
{
    public partial class Instructor
    {
        public Instructor()
        {
            Instructs = new HashSet<Instruct>();
        }

        public decimal InstructorId { get; set; }
        public string Name { get; set; }
        public string Department { get; set; }

        public virtual User InstructorNavigation { get; set; }
        public virtual ICollection<Instruct> Instructs { get; set; }
    }
}
