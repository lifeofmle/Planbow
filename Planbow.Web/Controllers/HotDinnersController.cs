using Planbow.Data.Interfaces;
using Planbow.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Planbow.Web.Controllers
{
    public class HotDinnersController : ApiControllerBase
    {
        public HotDinnersController(IPlanbowUow uow)
        {
            Uow = uow;
        }

        public IEnumerable<Venue> Get()
        {
            if (Uow == null)
                return null;

            return Uow.Plans.HotDinnerVenues();
        }

        public Venue Get(string id)
        {
            if (Uow == null)
                return null;

            return Uow.Plans.GetHotDinner(id);
        }
    }
}
