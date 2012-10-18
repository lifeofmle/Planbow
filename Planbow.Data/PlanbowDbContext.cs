using Planbow.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace Planbow.Data
{
    public class PlanbowDbContext : DbContext 
    {
        public PlanbowDbContext()
            : base(nameOrConnectionString: "Planbow")
        { }

        public DbSet<Plan> Plans { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<SocialPlatform> SocialPlatforms { get; set; }
        public DbSet<Activity> Activities { get; set; }
        public DbSet<FoursquareData> FoursquareData { get; set; }
    }
}
