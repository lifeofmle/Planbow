using Planbow.Data;
using Planbow.Data.Interfaces;
using Planbow.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace PlanbowApp.Controllers
{
    public class PlanController : ApiController
    {
        private IPlanRepository _planRepository;

        public PlanController()
        {
            _planRepository = new PlanRepository();
        }

        //
        // GET: api/plan
        public IEnumerable<Plan> Get()
        {
            return _planRepository.GetPlans();
        }

    }
}
