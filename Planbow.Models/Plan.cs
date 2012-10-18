using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Planbow.Models
{
    public class Plan : Domain
    {
        public string PlanId { get; set; }
        public string Name { get; set; }

        public User Owner { get; set; }

        public int Likes { get; set; }
        public int Views { get; set; }

        public List<Activity> Activities { get; set; }
    }
}
