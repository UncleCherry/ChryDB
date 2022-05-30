using System;
using System.Collections.Generic;

#nullable disable

namespace Back.Entity
{
    public partial class Record
    {
        public decimal RecordId { get; set; }
        public decimal? CourseId { get; set; }
        public DateTime? Time { get; set; }
        public string Url { get; set; }

        public virtual Course Course { get; set; }
    }
}
