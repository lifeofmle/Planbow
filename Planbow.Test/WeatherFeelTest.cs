using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using Planbow.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Planbow.Test
{
    [TestClass]
    public class WeatherFeelTest
    {
        [TestMethod]
        public void ShouldParseForCurrentConditionsAndForecast()
        {
            var json = GetString("Planbow.Test.currentForecast.json");

            Assert.IsNotNull(json);

            var jObject = JObject.Parse(json);

            var currentObs = jObject["current_observation"];            

            var weatherJson = new JObject();
            weatherJson.Add("currentTemp", currentObs["temp_c"]);
            weatherJson.Add("currentFeelsLike", currentObs["feelslike_c"]);
            weatherJson.Add("currentCondition", currentObs["weather"]);
            weatherJson.Add("currentIcon", currentObs["icon"]);
            
            var forecast = jObject["forecast"];

            var dailyForecasts = forecast["txt_forecast"]["forecastday"];

            var forecastList = new JArray();
            foreach (var item in dailyForecasts)
            {
                if (item.Value<string>("period") == "0")
                {
                    weatherJson.Add("currentPop", item["pop"]);
                    continue;
                }

                forecastList.Add(item);
            }         
            
            weatherJson.Add("forecast", forecastList);

            Assert.AreEqual(10, weatherJson.GetValue("currentTemp"));
            Assert.AreEqual("10", weatherJson.GetValue("currentFeelsLike"));
            Assert.AreEqual("Partly Cloudy", weatherJson.GetValue("currentCondition"));
            Assert.AreEqual("partlycloudy", weatherJson.GetValue("currentIcon"));
            Assert.AreEqual("0", weatherJson.GetValue("currentPop"));
        }

        [TestMethod]
        public void ShouldParseForHourlyData()
        {
            var json = GetString("Planbow.Test.hourly.json");

            Assert.IsNotNull(json);
            var jObject = JObject.Parse(json);

            var weatherJson = new JObject();

            var hourlyForecast = jObject["hourly_forecast"];

            var hourlyData = new JArray();

            int i = 0;
            foreach (var item in hourlyForecast)
            {
                var hour = item["FCTTIME"]["hour"];
                var temp = item["temp"]["metric"];

                var hourValue = hour.Value<int>();
                var time = "am";
                if (hourValue > 12)
                {
                    hourValue = hourValue - 12;
                    time = "pm";
                }

                var hourObj = new JObject();
                hourObj.Add("hour", hourValue + time);
                hourObj.Add("temp", temp);

                hourlyData.Add(hourObj);

                i++;

                if (i >= 12)
                    break;
            }

            weatherJson.Add("hourly", hourlyData);

            Assert.IsNotNull(weatherJson.GetValue("hourly"));
        }

        [TestMethod]
        public void ShouldParseForYesterdayConditions()
        {
            var json = GetString("Planbow.Test.yesterday.json");

            Assert.IsNotNull(json);
            var jObject = JObject.Parse(json);

            var weatherJson = new JObject();            

            var yesterdaySummary = string.Empty;

            var dailySummary = jObject["history"]["dailysummary"][0];

            var tempMin = dailySummary["mintempm"].Value<string>();
            var tempMax = dailySummary["maxtempm"].Value<string>();

            yesterdaySummary = string.Format("Ranged from a low of {0}&#176;C to a high of {1}&#176;C", tempMin, tempMax);

            weatherJson.Add("yesterday", yesterdaySummary);

            Assert.IsNotNull(weatherJson.GetValue("yesterday"));
        }

        private string GetString(string path)
        {
            var stringText = string.Empty;

            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(path))
            {
                using (var textReader = new StreamReader(stream))
                {
                    stringText = textReader.ReadToEnd();
                }
            }

            return stringText;
        }
    }
}
