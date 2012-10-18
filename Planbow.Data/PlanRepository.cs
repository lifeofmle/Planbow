using Planbow.Data.Interfaces;
using Planbow.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Planbow.Data
{
    public class PlanRepository : IPlanRepository
    {
        public IEnumerable<Plan> GetPlans()
        {
            var plans = new List<Plan>();
            
            var activities = new List<Activity>();
            activities.Add(new Activity { Id = Guid.NewGuid().ToString(), Name = "Village East", Description = "Bar/Restaurant", Latitude = 100.123, Longitude = 100.123, StartDate = DateTime.Now.AddHours(47) });
            activities.Add(new Activity { Id = Guid.NewGuid().ToString(), Name = "Tower Bridge", Description = "Landmark", Latitude = 100.123, Longitude = 100.123, StartDate = DateTime.Now.AddHours(27) });
            activities.Add(new Activity { Id = Guid.NewGuid().ToString(), Name = "Borough Market", Description = "Market", Latitude = 100.123, Longitude = 100.123, StartDate = DateTime.Now.AddHours(17) });

            plans.Add(new Plan { Id = Guid.NewGuid().ToString(), Name = "Dinner and movies", CreatedDate = DateTime.Now.AddDays(-3), Activities = activities });
            plans.Add(new Plan { Id = Guid.NewGuid().ToString(), Name = "Day at the markets", CreatedDate = DateTime.Now.AddDays(-10), Activities = activities });
            plans.Add(new Plan { Id = Guid.NewGuid().ToString(), Name = "Drinks and scenic walk in SE1s", CreatedDate = DateTime.Now.AddDays(-7), Activities = activities });

            return plans;
        }
    }
}
