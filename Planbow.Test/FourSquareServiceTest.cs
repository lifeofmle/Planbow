using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using System.IO;
using Newtonsoft.Json.Linq;
using Planbow.Models;
using System.Collections.Generic;

namespace Planbow.Test
{
    [TestClass]
    public class FourSquareServiceTest
    {
        [TestMethod]
        public void ParseVenueTest()
        {
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Planbow.Test.FoursquareVenue.json"))
            {
                using (var textReader = new StreamReader(stream))
                {
                    var foursquareJson = textReader.ReadToEnd();

                    var fsJObject = JObject.Parse(foursquareJson);

                    var fsResponse = fsJObject["response"];

                    var fsVenue = fsResponse["venue"];

                    var venue = fsVenue.ToObject<FoursquareVenue>();

                    var tips = fsVenue["tips"];
                    if (tips.HasValues)
                    {
                        venue.HasTips = true;
                        venue.TipsCount = tips["count"].Value<int>();

                        venue.TipTexts = new List<string>();
                        if (venue.TipsCount > 0)
                        {
                            var tipsList = new List<string>();
                            var groups = tips["groups"];

                            if (groups.HasValues)
                            {
                                var items = groups["items"];

                                if (items.HasValues)
                                {
                                    foreach (var item in items)
                                    {
                                        if (item["type"].Value<string>() == "others")
                                        {
                                            foreach (var data in item["items"])
                                            {
                                                tipsList.Add(data["text"].Value<string>());
                                            }
                                        }
                                    }
                                }
                            }                            

                            venue.TipTexts = tipsList;
                        }
                    }

                    if (fsVenue["photos"].HasValues)
                    {
                        venue.HasPhotos = true;

                        venue.PhotosCount = tips["count"].Value<int>();

                        venue.PhotoUrls = new List<string>();
                        if (venue.TipsCount > 0)
                        {
                            var photoUrls = new List<string>();
                            var groups = tips["groups"];

                            if (groups.HasValues)
                            {
                                var items = groups["items"];

                                if (items.HasValues)
                                {
                                    foreach (var item in items)
                                    {
                                        if (item["type"].Value<string>() == "venue")
                                        {
                                            foreach (var data in item["items"])
                                            {
                                                photoUrls.Add(data["url"].Value<string>());
                                            }
                                        }
                                    }
                                }
                            }    

                            venue.PhotoUrls = photoUrls;
                        }                  

                    }
                }
            }
        }
    }
}
