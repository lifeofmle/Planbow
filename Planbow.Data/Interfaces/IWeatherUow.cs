using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Planbow.Data.Interfaces
{
    public interface IWeatherUow
    {
        // Save pending changes to the data store.
        void Commit();

        // Repositories
        IWeatherRepository WeatherConditions { get; }
    }
}
