using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Planbow.Models
{
    public class Venue : Location
    {
        public string VenueType { get; set; }

        public List<SocialData> SocialDataItems { get; set; }

        public List<User> PastVistors { get; set; }
        public List<User> PotentialVisitors { get; set; }
    }
}
