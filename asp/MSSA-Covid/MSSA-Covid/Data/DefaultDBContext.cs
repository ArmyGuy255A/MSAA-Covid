using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MSSA_Covid.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace MSSA_Covid.Data
{
    public class DefaultDBContext : DbContext
    {
        public DefaultDBContext(DbContextOptions<DefaultDBContext> options) : base(options)
        {
        }

        public DbSet<Location> Locations { get; set; }
        public DbSet<PageStatistic> PageStatistics { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Location>().HasData(
                new Location { Id = -10, City = "Pensacola", State = "Florida", County = "Escambia" },
                new Location { Id = -1, City = "Fort Walton Beach", State = "Florida", County = "Okaloosa" },
                new Location { Id = -2, City = "Crestview", State = "Florida", County = "Okaloosa" },
                new Location { Id = -4, City = "Niceville", State = "Florida", County = "Okaloosa" },
                new Location { Id = -5, City = "Destin", State = "Florida", County = "Okaloosa" },
                new Location { Id = -6, City = "Hephzibah", State = "Georgia", County = "Richmond" },
                new Location { Id = -7, City = "Augusta", State = "Georgia", County = "Richmond" },
                new Location { Id = -8, City = "Fort Gordon", State = "Georgia", County = "Richmond" },
                new Location { Id = -9, City = "Blythe", State = "Georgia", County = "Richmond" }
               );

        }
    }
}
