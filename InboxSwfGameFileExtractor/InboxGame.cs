using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace InboxSwfGameFileExtractor
{
    class InboxGame
    {
        public string Title { get; private set; }
        public string SwfFileUrl { get; private set; }

        public InboxGame(string title, string swfFileUrl)
        {
            Title = title;
            SwfFileUrl = swfFileUrl;
        }

        // Expected input: http://games.inbox.lv/mini/game/[THE GAME YOU WANT]/
        // Remove the []
        public static async Task<InboxGame> CreateInstanceFrom(string gameUrl)
        {
            HtmlDocument html = await GetHtmlFrom(gameUrl);
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

            return new InboxGame(title, swfFileUrl);
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

        private static async Task<HtmlDocument> GetHtmlFrom(string url)
        {
            string data = await Request(url);

            var document = new HtmlDocument();
            document.LoadHtml(data);

            return document;
        }

        private static async Task<string> Request(string url)
        {
            using var client = new HttpClient();
            using HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
    }
}
