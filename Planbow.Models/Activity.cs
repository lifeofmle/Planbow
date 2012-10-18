using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Planbow.Models
{
    public class Activity : Venue
    {
        public string Description { get; set; }  
      
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public User Owner { get; set; }

        public List<User> Guests { get; set; }
    }
}
