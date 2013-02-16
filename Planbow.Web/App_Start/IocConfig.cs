using Ninject;
using Planbow.Data;
using Planbow.Data.Helpers;
using Planbow.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Planbow.Web.App_Start
{
    public class IocConfig
    {
        public static void RegisterIoc(HttpConfiguration config)
        {
            var kernel = new StandardKernel(); // Ninject IoC

            // These registrations are "per instance request".
            // See http://blog.bobcravens.com/2010/03/ninject-life-cycle-management-or-scoping/

            kernel.Bind<RepositoryFactories>()
                .To<RepositoryFactories>()
                .InSingletonScope();

            kernel.Bind<IRepositoryProvider>().To<RepositoryProvider>();
            kernel.Bind<IPlanbowUow>().To<PlanbowUow>();
            kernel.Bind<IWeatherUow>().To<WeatherUow>();

            // Tell WebApi how to use our Ninject IoC
            config.DependencyResolver = new NinjectDependencyResolver(kernel);
        }
    }
}