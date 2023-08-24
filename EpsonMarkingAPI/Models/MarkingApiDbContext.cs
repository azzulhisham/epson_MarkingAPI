using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace EpsonMarkingAPI.Models
{
    [DbConfigurationType(typeof(DbContextConfiguration))]
    public class MarkingApiDbContext : DbContext
    {
        public MarkingApiDbContext()
            : base("name=MarkingRecordsConnection")
        {
        }

        public virtual DbSet<MarkingRecords> markingRecords { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}