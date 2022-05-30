using System;
using System.Collections.Generic;

#nullable disable

namespace Back.Entity
{
    public partial class Admin
    {
        public Admin()
        {
            Applications = new HashSet<Application>();
            TrainingPlans = new HashSet<TrainingPlan>();
        }

        public decimal AdminId { get; set; }
        public string Name { get; set; }
        public string Department { get; set; }

        public virtual User AdminNavigation { get; set; }
        public virtual ICollection<Application> Applications { get; set; }
        public virtual ICollection<TrainingPlan> TrainingPlans { get; set; }
    }
}
