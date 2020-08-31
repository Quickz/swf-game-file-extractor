using HtmlAgilityPack;
using SwfGameFileExtractor.Utilities;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace SwfGameFileExtractor
{
    static class InboxSwfFactory
    {
        public static async Task<SwfGame> CreateInstanceFrom(string gameUrl)
        {
            HtmlDocument html = await HttpUtility.GetHtmlFrom(gameUrl);
            string id = GetSwfFileIdFrom(html);
            string title = GetSwfFileTitleFrom(html);

            if (id == null)
            {
                throw new Exception("Unable to acquire a valid ID!");
            }

            if (title == null)
            {
                throw new Exception("Unable to acquire a title!");
            }

            string swfFileUrl = $"http://games.inbox.lv/uploads/games/{ id }.swf";

            return new SwfGame(title, swfFileUrl);
        }

        private static string GetSwfFileTitleFrom(HtmlDocument document)
        {
            const string classToFind = "gameTitle";

            // Selecting any tag that matches the specified class
            var htmlClassNodes = document
                .DocumentNode
                .SelectNodes($"//*[@class='{classToFind}']");

            return htmlClassNodes?.FirstOrDefault()?.InnerText;
        }

        private static string GetSwfFileIdFrom(HtmlDocument document)
        {
            const string classToFind = "game-rate-id";

            // Selecting any tag that matches the specified class
            var htmlClassNodes = document
                .DocumentNode
                .SelectNodes($"//*[@class='{classToFind}']");

            return htmlClassNodes?.FirstOrDefault()?.Id;
        }
    }
}
