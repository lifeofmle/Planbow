using Newtonsoft.Json.Linq;
using NLog;
using Planbow.Data.Helpers;
using Planbow.Data.Interfaces;
using Planbow.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;

namespace Planbow.Data
{
    public class PlanRepository : EFRepository<Plan>, IPlanRepository
    {
        private static Logger Log = LogManager.GetCurrentClassLogger();

        private List<Venue> _hotDinnerVenues;

        private static object FileLock = new object();

        public PlanRepository(DbContext context) : base(context) 
        {
            List<string[]> hotDinnerData = null;
            _hotDinnerVenues = new List<Venue>();

            // Read in csv from resource
            lock (FileLock)
            {
                using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Planbow.Data.Files.mappedDinners_20121027.csv"))
                {
                    CsvReader reader = new CsvReader(stream);

                    foreach (var data in reader.RowEnumerator)
                    {
                        if (data is string[])
                        {
                            var restaurant = data as string[];

                            var venue = new Venue();

                            venue.Name = CsvUtility.Escape(restaurant[0]);                            

                            var foursquareId = restaurant[1];
                            venue.FoursquareData = new FoursquareVenue() { FoursquareId = foursquareId };

                            if (!string.IsNullOrEmpty(restaurant[2]))
                                venue.Longitude = double.Parse(restaurant[2]);

                            if (!string.IsNullOrEmpty(restaurant[3]))
                                venue.Latitude = double.Parse(restaurant[3]);                            

                            var hotDinner = new HotDinnerData();
                            hotDinner.IsOpen = restaurant[4] == "Open" ? true : false;
                            hotDinner.When = restaurant[5];
                            hotDinner.Where = restaurant[6];
                            hotDinner.ShortDescription = restaurant[7];
                            hotDinner.Description = restaurant[8];
                            hotDinner.Url = restaurant[9];                           

                            venue.HotDinnerData = hotDinner;

                            _hotDinnerVenues.Add(venue);
                        }
                    }
                }
            }           

        }

        //public IEnumerable<Plan> GetPlans()
        //{
        //    var plans = new List<Plan>();

        //    var activities = new List<Activity>();
        //    activities.Add(new Activity { ActivityId = Guid.NewGuid().ToString(), Name = "Village East", Description = "Bar/Restaurant", Latitude = 100.123, Longitude = 100.123, StartDate = DateTime.Now.AddHours(47) });
        //    activities.Add(new Activity { ActivityId = Guid.NewGuid().ToString(), Name = "Tower Bridge", Description = "Landmark", Latitude = 100.123, Longitude = 100.123, StartDate = DateTime.Now.AddHours(27) });
        //    activities.Add(new Activity { ActivityId = Guid.NewGuid().ToString(), Name = "Borough Market", Description = "Market", Latitude = 100.123, Longitude = 100.123, StartDate = DateTime.Now.AddHours(17) });

        //    plans.Add(new Plan { PlanId = Guid.NewGuid().ToString(), Name = "Dinner and movies", CreatedDate = DateTime.Now.AddDays(-3), Activities = activities });
        //    plans.Add(new Plan { PlanId = Guid.NewGuid().ToString(), Name = "Day at the markets", CreatedDate = DateTime.Now.AddDays(-10), Activities = activities });
        //    plans.Add(new Plan { PlanId = Guid.NewGuid().ToString(), Name = "Drinks and scenic walk in SE1s", CreatedDate = DateTime.Now.AddDays(-7), Activities = activities });

        //    return plans;
        //}

        public IEnumerable<Venue> HotDinnerVenues()
        {
            return _hotDinnerVenues;
        }

        public FoursquareVenue GetVenue(string foursquareId)
        {
            var clientId = "Q3PBA3ZZ3WV4T4SOKCKBTOYAAACRYXOGGC5EKMHW2GL4V4MD";
            var clientSecret = "S4QKDK2E0I3MLS02IY0YU14YBQ5RQ1XGZAR2UVC11IHDF5ZZ";
            var date = DateTime.Now.ToString("yyyyMMdd");

            var foursquareUrl = string.Format("https://api.foursquare.com/v2/venues/{0}?v={1}&client_id={2}&client_secret={3}",
                                        foursquareId,
                                        date,
                                        clientId,
                                        clientSecret);

            FoursquareVenue foursquareVenue = null;
            try
            {
                var request = WebRequest.Create(foursquareUrl);

                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    using (var stream = response.GetResponseStream())
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            var json = reader.ReadToEnd();

                            var fsJObject = JObject.Parse(json);

                            var fsResponse = fsJObject["response"];

                            var fsVenue = fsResponse["venue"];

                            foursquareVenue = fsVenue.ToObject<FoursquareVenue>();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Could not process Foursquare request", foursquareUrl, ex);
            }

            return foursquareVenue;
        }
    }
}
