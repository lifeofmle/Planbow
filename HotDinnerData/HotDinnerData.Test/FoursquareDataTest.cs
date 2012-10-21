using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotDinnerData.Test
{
    [TestClass]
    public class FoursquareDataTest
    {
        [TestMethod]
        public void FoursquareParseTest()
        {
            var json = File.ReadAllText("FourSquareVenue.json");
            var fsJObject = JObject.Parse(json);

            var fsResponse = fsJObject["response"];

            var fsVenue = fsResponse["venue"];            

            var fsData = fsVenue.ToObject<FoursquareVenue>();

            Assert.IsNotNull(fsData);


        }
    }
}
