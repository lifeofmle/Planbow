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

        private List<Venue> _venues;
        private Dictionary<string, string> _foursquareDictionary = new Dictionary<string,string>();
        private Dictionary<string, Venue> _venueDictionary;

        private string _foursquareJson;

        private static object FileLock = new object();

        public PlanRepository(DbContext context) : base(context) 
        {
            _venueDictionary = new Dictionary<string, Venue>();

            _venues = new List<Venue>(InitializeVenues());                  
        }

        public IEnumerable<Venue> Venues()
        {
            return _venues.Where(x => x.Visited);
        }    

        public string GetVenue(string foursquareId)
        {
            Log.Info("Requesting FoursquareId {0}", foursquareId);

            var venueInfo = string.Empty;

            if (_foursquareDictionary.ContainsKey(foursquareId))
            {
                venueInfo = _foursquareDictionary[foursquareId];
            }
            else
            {
                venueInfo = RequestFoursquareData(foursquareId);
                _foursquareDictionary.Add(foursquareId, venueInfo);
            }

            return venueInfo;
        }

        private string RequestFoursquareData(string foursquareId)
        {
            var clientId = "Q3PBA3ZZ3WV4T4SOKCKBTOYAAACRYXOGGC5EKMHW2GL4V4MD";
            var clientSecret = "S4QKDK2E0I3MLS02IY0YU14YBQ5RQ1XGZAR2UVC11IHDF5ZZ";
            var date = "20121029";

            var foursquareUrl = string.Format("https://api.foursquare.com/v2/venues/{0}?v={1}&client_id={2}&client_secret={3}",
                                        foursquareId,
                                        date,
                                        clientId,
                                        clientSecret);

            var responseValue = string.Empty;

            try
            {
                var request = WebRequest.Create(foursquareUrl);

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
                Log.Error("Could not process Foursquare request", foursquareUrl, ex);
            }

            return responseValue;
        }

        private IEnumerable<Venue> InitializeVenues()
        {
            var venues = new List<Venue>();

            // Read in csv from resource
            lock (FileLock)
            {
                using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Planbow.Data.Files.mappedDinners_20121118.csv"))
                {
                    CsvReader reader = new CsvReader(stream);

                    var count = 0;
                    foreach (var data in reader.RowEnumerator)
                    {
                        if (data is string[])
                        {
                            var restaurant = data as string[];

                            var venue = new Venue();
                            venue.Id = (++count).ToString();

                            venue.Name = CsvUtility.Escape(restaurant[0]);                            

                            if (!string.IsNullOrEmpty(restaurant[1]))                            
                                venue.Visited = bool.Parse(restaurant[1]);

                            var foursquareId = restaurant[2];
                            if (string.IsNullOrEmpty(foursquareId))
                                foursquareId = Guid.NewGuid().ToString();

                            venue.FoursquareData = new FoursquareVenue() { FoursquareId = foursquareId };

                            if (!string.IsNullOrEmpty(restaurant[3]))
                                venue.Longitude = double.Parse(restaurant[3]);

                            if (!string.IsNullOrEmpty(restaurant[4]))
                                venue.Latitude = double.Parse(restaurant[4]);

                            //var hotDinner = new HotDinnerData();
                            //hotDinner.IsOpen = restaurant[4] == "Open" ? true : false;
                            //hotDinner.When = restaurant[5];
                            //hotDinner.Where = restaurant[6];
                            //hotDinner.ShortDescription = restaurant[7];
                            //hotDinner.Description = restaurant[8];
                            //hotDinner.Url = restaurant[9];
                            //venue.HotDinnerData = hotDinner;
                            //_hotDinnerVenues.Add(venue);

                            venues.Add(venue);

                            if (!_venueDictionary.ContainsKey(venue.Id))
                                _venueDictionary.Add(venue.Id, venue);
                        }
                    }
                }
            } 

            return venues;
        }
    }
}
