using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Planbow.Models
{
    public class User : Domain
    {
        public string Id { get; set; }
        public string ImageUrl { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string TwitterId { get; set; }
        public string FacebookId { get; set; }
        public string FoursquareId { get; set; }
    }
}
