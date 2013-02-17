using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Planbow.Models
{
    public class Weather
    {
        public int Id { get; set; }

        public string City
        {
            get;
            set;
        }

        public string Area
        {
            get;
            set;
        }

        public string CurrentWeather
        {
            get;
            set;
        }

        public DateTime Updated
        {
            get;
            set;
        }
    }
}
