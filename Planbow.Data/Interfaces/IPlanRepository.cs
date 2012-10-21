using Planbow.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Planbow.Data.Interfaces
{
    public interface IPlanRepository : IRepository<Plan>
    {
        IEnumerable<Venue> HotDinnerVenues();

        FoursquareVenue GetVenue(string venueId);
    }
}
