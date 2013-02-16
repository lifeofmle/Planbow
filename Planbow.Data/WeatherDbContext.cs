using Planbow.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace Planbow.Data
{
    public class WeatherDbContext : DbContext
    {
        public WeatherDbContext() : base(nameOrConnectionString: "WeatherFeel")
        {
        }

        public DbSet<Weather> WeatherConditions { get; set; }
    
    }
}
