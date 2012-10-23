using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Planbow.Models
{
    public class Venue : Location
    {
        public FoursquareVenue FoursquareData { get; set; }

        public HotDinnerData HotDinnerData { get; set; }

        [JsonIgnore]
        public string VenueType { get; set; }        

        [JsonIgnore]
        public virtual List<User> PastVistors { get; set; }

        [JsonIgnore]
        public virtual List<User> PotentialVisitors { get; set; }
    }
}
