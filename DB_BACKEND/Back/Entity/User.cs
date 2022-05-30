using System;
using System.Collections.Generic;

#nullable disable

namespace Back.Entity
{
    public partial class User
    {
        public User()
        {
            Applications = new HashSet<Application>();
        }

        public decimal UserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public int UserType { get; set; }

        public virtual Admin Admin { get; set; }
        public virtual Instructor Instructor { get; set; }
        public virtual Student Student { get; set; }
        public virtual ICollection<Application> Applications { get; set; }
    }
}
