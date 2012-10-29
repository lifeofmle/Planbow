using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Planbow.Models
{
    public class FoursquareVenue : SocialPlatform
    {
        [JsonProperty("id")]
        public string FoursquareId { get; set; }

        public FoursquareLocation Location { get; set; }

        public FoursquareStatistic Stats { get; set; }

        [JsonProperty("reservations")]
        public FoursquareReservation Reservation { get; set; }

        public List<FoursquareCategory> Categories { get; set; }

        public string Phone { get; set; }

        public string Twitter { get; set; }

        public string Website { get; set; }

        public bool HasTips { get; set; }

        public int TipsCount { get; set; }

        public List<string> TipTexts { get; set; }

        public bool HasPhotos { get; set; }

        public int PhotosCount { get; set; }

        public List<string> PhotoUrls { get; set; }

        public void PopulateValues(JContainer json)
        {

        }
    }

    public class FoursquareLocation
    {
        public string Address { get; set; }

        [JsonProperty("lat")]
        public double Latitude { get; set; }

        [JsonProperty("lng")]
        public double Longitude { get; set; }

        public string PostalCode { get; set; }

        public string City { get; set; }

        public string Country { get; set; }

        public string Cc { get; set; }
    }

    public class FoursquareCategory
    {
        [JsonProperty("id")]
        public string CategoryId { get; set; }

        public string Name { get; set; }

        public string PluralName { get; set; }

        public string ShortName { get; set; }

        public FoursquareIcon Icon { get; set; }
    }

    public class FoursquareIcon
    {
        public string Prefix { get; set; }

        public List<int> Sizes { get; set; }

        public string Name { get; set; }
    }

    public class FoursquareStatistic
    {
        public int CheckinsCount { get; set; }
    }

    public class FoursquareReservation
    {
        public string Url { get; set; }
    }

}
