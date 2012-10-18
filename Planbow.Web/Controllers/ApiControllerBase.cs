using Planbow.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Planbow.Web.Controllers
{
    public abstract class ApiControllerBase : ApiController
    {
        protected IPlanbowUow Uow { get; set; }
    }
}