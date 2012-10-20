using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotDinnerData
{
    public static class HotDinnerDataParser
    {
        public static string NUTSHELL = "In a nutshell:";
        public static string WHERE = "Where is it?";
        public static string WHY = "Why should you care?";
        public static string WHY_ALT = "Why should you care";
        public static string WHEN = "When can you try it for yourself?";

        public static HotDinnerData Parse(HtmlNode htmlNode, bool isOpen)
        {
            var hotDinnerData = new HotDinnerData();
            hotDinnerData.IsOpen = isOpen;

            var restaurantNode = htmlNode.SelectSingleNode(".//span[@class='articleheading']");
            if (restaurantNode != null)
                hotDinnerData.RestaurantName = restaurantNode.InnerText;

            var detailsText = string.Empty;

            if (isOpen)
            {
                var detailsNode = htmlNode.SelectSingleNode(".//p[2]");
                if (detailsNode != null)
                    detailsText = detailsNode.InnerText;
            }
            else
                detailsText = htmlNode.InnerText;            

            var nutshellIndex = detailsText.IndexOf(NUTSHELL) + NUTSHELL.Length;
            var whereIndex = detailsText.IndexOf(WHERE);
            var whyIndex = detailsText.IndexOf(WHY);
            var whyLength = WHY.Length;
            if (whyIndex == -1)
            {
                whyIndex = detailsText.IndexOf(WHY_ALT);
                whyLength = WHY_ALT.Length;
            }
            var whenIndex = detailsText.IndexOf(WHEN);                

            hotDinnerData.ShortDescription = detailsText.Substring(nutshellIndex, whereIndex - nutshellIndex).Trim();

            hotDinnerData.Where = detailsText.Substring(whereIndex + WHERE.Length, whyIndex - (whereIndex + WHERE.Length)).Trim();

            if (whenIndex == -1)
                hotDinnerData.Description = detailsText.Substring(whyIndex + whyLength).Trim();
            else
            {
                hotDinnerData.Description = detailsText.Substring(whyIndex + whyLength, whenIndex - (whyIndex + whyLength)).Trim();
                hotDinnerData.When = detailsText.Substring(whenIndex + WHEN.Length).Trim();
            }                       

            var linkNode = htmlNode.SelectSingleNode(".//a");
            if (linkNode != null)
                hotDinnerData.Url = linkNode.GetAttributeValue("href", "");

            return hotDinnerData;
        }
    }
}
