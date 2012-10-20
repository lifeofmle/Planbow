using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HotDinnerData.Test
{
    [TestClass]
    public class ParsingTest
    {
        public const string NUTSHELL = "In a nutshell:";
        public const string WHERE = "Where is it?";
        public const string WHY = "Why should you care?";

        [TestMethod]
        public void HotDinnersDetailsTest()
        {
            var text = "In a nutshell: B&amp;L goes East  Where is it? 38 St John Street, EC1  Why should you care? It's the second venue that B&amp;L bought from Bistro Du Vin serving the same lobster. lobster roll or burger and chips that's doing so well elsewhere.";

            var nutshellIndex = text.IndexOf(NUTSHELL) + NUTSHELL.Length;
            var whereIndex = text.IndexOf(WHERE);
            var whyIndex = text.IndexOf(WHY);

            var nutshell = text.Substring(nutshellIndex, whereIndex-nutshellIndex).Trim();
            Assert.AreEqual("B&amp;L goes East", nutshell);

            var whereIsIt = text.Substring(whereIndex + WHERE.Length, whyIndex - (whereIndex + WHERE.Length)).Trim();
            Assert.AreEqual("38 St John Street, EC1", whereIsIt);

            var whyShouldYouCare = text.Substring(whyIndex + WHY.Length).Trim();
            Assert.AreEqual("It's the second venue that B&amp;L bought from Bistro Du Vin serving the same lobster. lobster roll or burger and chips that's doing so well elsewhere.", whyShouldYouCare);
        }
    }
}
