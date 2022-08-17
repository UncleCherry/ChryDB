using System;
using System.Collections.Generic;

#nullable disable

namespace Back.Entity
{
    public partial class TrainingPlan
    {
        public TrainingPlan()
        {
            Students = new HashSet<Student>();
        }

        public string Major { get; set; }
        public decimal Grade { get; set; }
        public string Info { get; set; }
        public decimal? AdminId { get; set; }

        public virtual Admin Admin { get; set; }
        public virtual ICollection<Student> Students { get; set; }
    }
}
