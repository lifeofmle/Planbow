﻿using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HotDinnerData
{
    class Program
    {
        private static Logger Log = LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            // Past Restaurants
            // http://www.hot-dinners.com/Features/Articles/new-and-recently-opened-london-restaurants
            
            // Upcoming Restaurants
            // http://www.hot-dinners.com/Features/Articles/coming-soon-new-restaurants-opening-in-london

            // Map Hot Dinners restaurants on a map, so get longitude and latitude points

            // Cross reference against Foursquare, London Eating and Yelp

            // Track check-ins and discuss trends of food and which restaurant has been successful
            // Correlate against twitter to see if hype is sustained

            var hotRestaurants = new List<HotDinnerData>();

            //var httpRequest = (HttpWebRequest)WebRequest.Create(@"http://www.hot-dinners.com/Features/Articles/new-and-recently-opened-london-restaurants");
            //var httpResponse = httpRequest.GetResponse();

            //using (var stream = httpResponse.GetResponseStream())
            //{
            //var htmlDocument = new HtmlDocument();
            //htmlDocument.Load("hotDinnerOpened.html");
            //var html = htmlDocument.DocumentNode;

            //// Opened restaurants
            //var openedRestaurants = new List<HotDinnerData>();
            //var restaurants = html.SelectNodes("//div[@style='margin-left: 150px;']");

            //foreach (var restaurant in restaurants)
            //{
            //    try
            //    {
            //        var hotDinnerData = HotDinnerDataParser.Parse(restaurant, true);
            //        openedRestaurants.Add(hotDinnerData);
            //    }
            //    catch (Exception ex)
            //    {
            //        Log.Error("Could not parse restaurant data", ex, restaurant.OuterHtml);
            //    }
            //}

            ////}

            //hotRestaurants.AddRange(openedRestaurants);

            //htmlDocument = new HtmlDocument();
            //htmlDocument.Load("hotDinnerComingSoon.html");
            //html = htmlDocument.DocumentNode;

            //var comingSoon = new List<HotDinnerData>();

            //var restaurantNames = new List<string>();
            //var restaurantNameNodes = html.SelectNodes("//span[@class=\"articleheading\"]");
            //foreach (var nameNode in restaurantNameNodes)
            //{
            //    if (string.IsNullOrEmpty(nameNode.InnerText))
            //        continue;

            //    if (nameNode.InnerHtml.Contains("<br>"))
            //        restaurantNames.Add(nameNode.InnerText.Trim());
            //}

            //// Coming soon
            //var index = 0;
            //var restaurantDetails = html.SelectNodes("//p");
            //foreach (var restaurant in restaurantDetails)
            //{
            //    try
            //    {
            //        if (restaurant.InnerText.Contains("In a nutshell"))
            //        {
            //            var hotDinnerData = HotDinnerDataParser.Parse(restaurant, false);
            //            hotDinnerData.RestaurantName = restaurantNames[index];

            //            comingSoon.Add(hotDinnerData);
            //            index++;
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        Log.Error("Could not parse restaurant data", ex, restaurant.OuterHtml);
            //    }
            //}

            //hotRestaurants.AddRange(comingSoon);

            //using (var fileStream = new FileStream(@"hotDinner.csv", FileMode.OpenOrCreate, FileAccess.ReadWrite))
            //{
            //    using (var streamWriter = new StreamWriter(fileStream))
            //    {
            //        foreach (var data in hotRestaurants)
            //            streamWriter.WriteLine(CreateCsv(data));    
            //    }
            //}

            var hotDinnerData = CsvUtility.ParseCsv("hotDinner_version1.csv");

            var foursquareVenues = new List<string>();

            using (var fileStream = new FileStream(@"hotDinnerLocations.csv", FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                using (var streamWriter = new StreamWriter(fileStream))
                {
                    foreach (var restaurant in hotDinnerData)
                    {
                        var restaurantName = restaurant[0];
                        var foursquareId = restaurant[1];

                        var clientId = "Q3PBA3ZZ3WV4T4SOKCKBTOYAAACRYXOGGC5EKMHW2GL4V4MD";
                        var clientSecret = "S4QKDK2E0I3MLS02IY0YU14YBQ5RQ1XGZAR2UVC11IHDF5ZZ";
                        var date = DateTime.Now.ToString("yyyyMMdd");

                        if (string.IsNullOrEmpty(foursquareId))
                        {
                            var message = string.Format("{0},{1},{2},{3},{4}",
                                            CsvUtility.Escape(restaurantName),
                                            CsvUtility.Escape(string.Empty),
                                            CsvUtility.Escape(string.Empty),
                                            CsvUtility.Escape(string.Empty),
                                            CsvUtility.Escape(string.Empty));

                            streamWriter.WriteLine(message);
                        }
                        else
                        {
                            var foursquareUrl = string.Format("https://api.foursquare.com/v2/venues/{0}?v={1}&client_id={2}&client_secret={3}",
                                                foursquareId,
                                                date,
                                                clientId,
                                                clientSecret);

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

                                            var fsData = fsVenue.ToObject<FoursquareVenue>();

                                            var message = string.Format("{0},{1},{2},{3}",
                                                CsvUtility.Escape(restaurantName),
                                                CsvUtility.Escape(foursquareId),
                                                CsvUtility.Escape(fsData.Location.Latitude.ToString()),
                                                CsvUtility.Escape(fsData.Location.Longitude.ToString()));

                                            foursquareVenues.Add(fsVenue.ToString());

                                            streamWriter.WriteLine(message);
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Log.Error(ex);
                            }                            
                        }

                        Console.WriteLine("Processed {0}", restaurantName);
                    }
                }
            }

            using (var fileStream = new FileStream(@"foursquareData.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                using (var streamWriter = new StreamWriter(fileStream))
                {
                    streamWriter.WriteLine("[");
                    foreach (var item in foursquareVenues)
                    {
                        streamWriter.WriteLine(item);
                        streamWriter.WriteLine(",");
                    }
                    streamWriter.WriteLine("]");
                }
            }

            // https://api.foursquare.com/v2/venues/{VENUE_ID}?v=20120321
            // ie. https://api.foursquare.com/v2/venues/506bf241498e3642a595bd75?v=20120321

            // https://api.foursquare.com/v2/venues/4f0b659ae4b0cc066f384c66?v=20120321&client_id=Q3PBA3ZZ3WV4T4SOKCKBTOYAAACRYXOGGC5EKMHW2GL4V4MD&client_secret=S4QKDK2E0I3MLS02IY0YU14YBQ5RQ1XGZAR2UVC11IHDF5ZZ

            
                        

            Console.WriteLine("Press any key to continue");
            Console.ReadKey();
        }

        public static string CreateCsv(HotDinnerData hotDinnerData)
        {
            var message = string.Format("{0},{1},{2},{3},{4},{5},{6}",
                            CsvUtility.Escape(hotDinnerData.RestaurantName),    
                            CsvUtility.Escape(hotDinnerData.IsOpen ? "Open" : "Soon"),
                            CsvUtility.Escape(hotDinnerData.When),
                            CsvUtility.Escape(hotDinnerData.Where),
                            CsvUtility.Escape(hotDinnerData.ShortDescription),
                            CsvUtility.Escape(hotDinnerData.Description),
                            CsvUtility.Escape(hotDinnerData.Url));

            return message;
        }
    }
}