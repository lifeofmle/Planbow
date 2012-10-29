using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using System.IO;
using Newtonsoft.Json.Linq;
using Planbow.Models;

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

                }
            }
        }
    }
}
