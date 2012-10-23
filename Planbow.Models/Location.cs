using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Planbow.Models
{
    public class Location : Domain
    {       
        public string Name { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        //public string City { get; set; }
        //public string PostCode { get; set; }
        //public string Country { get; set; }        
    }
}
