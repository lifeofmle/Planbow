using Planbow.Data.Interfaces;
using Planbow.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Planbow.Web.Controllers
{
    public class FoursquareController : ApiControllerBase
    {
        public FoursquareController(IPlanbowUow uow)
        {
            Uow = uow;
        }

        public string Get(string id)
        {
            if (Uow == null)
                return null;

            return Uow.Plans.GetVenue(id);
        }
    }
}