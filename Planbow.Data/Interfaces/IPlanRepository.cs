using Newtonsoft.Json.Linq;
using Planbow.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Planbow.Data.Interfaces
{
    public interface IPlanRepository : IRepository<Plan>
    {
        IEnumerable<Venue> Venues();

        string GetVenue(string venueId);
    }
}
