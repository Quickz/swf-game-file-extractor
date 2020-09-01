using HtmlAgilityPack;
using SwfGameFileExtractor.Utilities;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace SwfGameFileExtractor
{
    static class Y8SwfFactory
    {
        // Example: https://www.y8.com/games/megaman_polarity
        public static async Task<SwfGame> CreateInstanceFrom(string gameUrl)
        {
            HtmlDocument html = await HttpUtility.GetHtmlFrom(gameUrl);

            string url = GetSwfFileUrlFrom(html);

            if (url == null)
            {
                throw new Exception("Unable to get a valid y8 URL!");
            }

            string title = GetTitleFrom(html);

            if (title == null)
            {
                throw new Exception("Unable to get a valid game title!");
            }

            return new SwfGame(title, url);
        }

        private static string GetSwfFileUrlFrom(HtmlDocument document)
        {
            const string idToFind = "gamefileEmbed";

            var htmlNodes = document
                .DocumentNode
                .SelectNodes($"//*[@id='{idToFind}']");

            return htmlNodes?.FirstOrDefault()?.Attributes.FirstOrDefault(x => x.Name == "src")?.Value;
        }

        private static string GetTitleFrom(HtmlDocument document)
        {
            const string idToFind = "details";
            const string subClassTofind = "left-part";

            var htmlNodes = document
                .DocumentNode
                .SelectNodes($"//*[@id='{idToFind}']");

            var titleNode = htmlNodes?
                .FirstOrDefault()?
                .ChildNodes
                .FirstOrDefault(x => x.HasClass(subClassTofind))?
                .ChildNodes
                .FirstOrDefault(x => x.Name == "h1");

            return titleNode.InnerText.Trim();
        }
    }
}
