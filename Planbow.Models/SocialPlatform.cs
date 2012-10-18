using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Planbow.Models
{
    public class SocialPlatform : Domain
    {
        [Key]
        public string PlatformId { get; set; }

        public string Source { get; set; }
        
        public string Url { get; set; }
    }
}
