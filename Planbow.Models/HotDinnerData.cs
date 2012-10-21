using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Planbow.Models
{
    public class HotDinnerData : SocialPlatform
    {
        [JsonProperty("id")]
        public string HotDinnerId { get; set; }

        public string RestaurantName { get; set; }

        public string Where { get; set; }

        public string Description { get; set; }

        public string ShortDescription { get; set; }

        public string Url { get; set; }

        public string When { get; set; }

        public bool IsOpen { get; set; }
    }
}
