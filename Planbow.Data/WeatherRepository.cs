using Newtonsoft.Json.Linq;
using NLog;
using Planbow.Data.Helpers;
using Planbow.Data.Interfaces;
using Planbow.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;

namespace Planbow.Data
{
    public class WeatherRepository : EFRepository<Weather>, IWeatherRepository
    {
        private static Logger Log = LogManager.GetCurrentClassLogger();

        private Dictionary<string, Weather> _weatherDictionary = new Dictionary<string,Weather>();

        private string _wundergroundApiKey = "98eae93fbe9acf44";

        private static object FileLock = new object();

        private static List<string> _states = new List<string> { "CA", "NY" };

        public WeatherRepository(DbContext context) : base(context) 
        {
            _weatherDictionary = new Dictionary<string,Weather>();

            Refresh();
        }

        public Weather GetWeather(string city)
        {
            Log.Info("Requesting weather for {0}", city);

            if (string.IsNullOrEmpty(city))
                return null;

            if (_weatherDictionary.ContainsKey(city))
                return _weatherDictionary[city];

            return null;
        }

        private void SaveWeather(Weather weather)
        {
            string sql = 
                @"UPDATE 
                    Weather
                  SET
                    CurrentWeather = {0},
                    Updated = {1}
                  WHERE
                    CityId = {2}";

            var weatherRepository = this.DbContext as WeatherDbContext;

            if (weatherRepository == null)
                return;

            weatherRepository.Database.ExecuteSqlCommand(sql, weather.CurrentWeather, DateTime.Now, weather.Id);
        }

        private Weather UpdateWeather(Weather weather)
        {
            if (weather == null)
                return weather;

            var retrievedWeather = GetWeatherData(weather.City, weather.Area);
            
            weather.CurrentWeather = retrievedWeather.CurrentWeather;
            weather.Updated = DateTime.Now;
            return weather;
        }

        private Weather GetWeatherData(string city, string area)
        {
            var weather = new Weather() { City = city, Area = area };

            var formattedCity = FormatCity(city);

            var isInternational = !_states.Contains(area);

            // Current Conditions and 3-Day Forecast
            var currentUrl = isInternational ?
                  "http://api.wunderground.com/api/{0}/geolookup/conditions/forecast/q/{1}/{2}.json"
                : "http://api.wunderground.com/api/{0}/forecast/geolookup/conditions/q/{1}/{2}.json";
                  // http://api.wunderground.com/api/{0}/geolookup/conditions/forecast/q/UK/London.json

            var wundergroundUrl = string.Format(currentUrl, _wundergroundApiKey, area, formattedCity);

            //var currentConditions = GetData(wundergroundUrl);

            //if (!string.IsNullOrEmpty(currentConditions))
            //{

            //}

            // Yesterday Info
            var wundergroundYesterdayUrl = string.Format("http://api.wunderground.com/api/{0}/yesterday/q/{2}/{1}.json",
                                        _wundergroundApiKey,
                                        formattedCity,
                                        area);

            //var yesterday = GetData(wundergroundYesterdayUrl);

            //if (!string.IsNullOrEmpty(yesterday))
            //{

            //}

            // 10 Hr Breakdown
            var wunderground10HourUrl = string.Format("http://api.wunderground.com/api/{0}/hourly10day/q/{2}/{1}.json",
                                        _wundergroundApiKey,
                                        formattedCity,
                                        area);

            //var hourly = GetData(wunderground10HourUrl);

            //if (!string.IsNullOrEmpty(hourly))
            //{

            //}

            return weather;
        }

        private string GetData(string url)
        {
            var responseValue = string.Empty;

            try
            {
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = WebRequestMethods.Http.Get;
                request.Accept = "application/json";

                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    using (var stream = response.GetResponseStream())
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            responseValue = reader.ReadToEnd();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Could not process request", url, ex);
            }

            return responseValue;
        }

        private IEnumerable<Weather> RetrieveWeather()
        {
            var weatherRepository = this.DbContext as WeatherDbContext;

            if (weatherRepository == null)
                return null;

            var weatherConditions = weatherRepository.WeatherConditions;

            if (weatherConditions == null)
                return null;

            var results = weatherConditions.SqlQuery(
            @"SELECT 
                c.Id, c.Name as City, c.Area, w.CurrentWeather, w.Updated 
             FROM
                City c
             INNER JOIN
                Weather w
             ON 
                c.Id = w.CityId").ToList();

            return results;
        }

        private void Refresh()
        {
            var weatherConditions = RetrieveWeather();

            if (weatherConditions == null)
                return;

            bool hasChanges = false;

            foreach (var condition in weatherConditions)
            {
                var weatherItem = condition;

                if (weatherItem.City == "London") // || DateTime.Now.Subtract(condition.Updated).Hours >= 12)
                {
                    weatherItem = UpdateWeather(condition);

                    SaveWeather(weatherItem);
                }

                _weatherDictionary.Add(FormatCity(weatherItem.City), weatherItem);
            }
        }

        private string FormatCity(string city)
        {
            if (string.IsNullOrEmpty(city))
                return null;

            return city.Replace(" ", "_");
        }
    }
}
