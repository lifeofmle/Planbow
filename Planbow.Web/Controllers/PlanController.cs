using Planbow.Data;
using Planbow.Data.Interfaces;
using Planbow.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace Planbow.Web.Controllers
{
    public class PlanController : ApiControllerBase
    {
        public PlanController(IPlanbowUow uow)
        {
            Uow = uow;
        }

        //
        // GET: api/plan
        public IEnumerable<Plan> Get()
        {
            if (Uow == null)
                return null;

            return Uow.Plans.GetAll();
        }

    }
}
