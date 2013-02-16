using Planbow.Data.Interfaces;
using Planbow.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Planbow.Web.Controllers
{
    public class WeatherFeelController : ApiControllerBase
    {
        public WeatherFeelController(IWeatherUow uow)
        {
            WeatherUow = uow;
        }

        public IEnumerable<Weather> Get()
        {
            if (WeatherUow == null)
                return null;

            return WeatherUow.WeatherConditions.GetAll();
        }

        public Weather Get(string id)
        {
            if (WeatherUow == null)
                return null;

            return WeatherUow.WeatherConditions.GetWeather(id);
        }
    }
}
