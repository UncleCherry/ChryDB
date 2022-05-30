using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Back
{
    public partial class DBContext : DbContext
    {
        public DBContext(DbContextOptions<DBContext> options)
           : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
           // modelBuilder.Entity<STUDENT>().HasNoKey();
        }
       // public virtual DbSet<STUDENT> STUDENT { get; set; }
    }
}

