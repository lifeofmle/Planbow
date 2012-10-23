using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Planbow.Models
{
    public class SocialPlatform : Domain
    {
        [JsonIgnore]
        public string PlatformId { get; set; }

        [JsonIgnore]
        public string Source { get; set; }
        
        //public string Url { get; set; }
    }
}
