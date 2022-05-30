using System;
using System.Collections.Generic;

#nullable disable

namespace Back.Entity
{
    public partial class Application
    {
        public decimal ApplicationId { get; set; }
        public decimal? UserId { get; set; }
        public decimal? AdminId { get; set; }
        public string Reason { get; set; }
        public DateTime? Time { get; set; }
        public int? Type { get; set; }
        public int? State { get; set; }

        public virtual Admin Admin { get; set; }
        public virtual User User { get; set; }
    }
}
