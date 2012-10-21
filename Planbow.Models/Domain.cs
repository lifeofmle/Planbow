using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Planbow.Models
{
    public class Domain
    {
        [JsonIgnore]
        public string CreatedBy { get; set; }

        [JsonIgnore]
        public DateTime CreatedDate { get; set; }

        [JsonIgnore]
        public string ModifiedBy { get; set; }

        [JsonIgnore]
        public DateTime ModifiedDate { get; set; }        
    }
}
