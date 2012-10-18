using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Planbow.Models
{
    public class FoursquareData : SocialData
    {
        public string VenueId { get; set; }

        public int CheckIns { get; set; }

        public bool HasOffers { get; set; }
    }
}
