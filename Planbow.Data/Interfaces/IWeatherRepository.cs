using Planbow.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Planbow.Data.Interfaces
{
    public interface IWeatherRepository : IRepository<Weather>
    {
        Weather GetWeather(string city);
    }
}
