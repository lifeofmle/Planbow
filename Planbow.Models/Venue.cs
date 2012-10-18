using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Planbow.Models
{
    public class Venue : Location
    {
        public string VenueId { get; set; }

        public string VenueType { get; set; }

        public List<SocialPlatform> SocialPlatforms { get; set; }

        public virtual List<User> PastVistors { get; set; }
        public virtual List<User> PotentialVisitors { get; set; }
    }
}
