using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SwfGameFileExtractor.Utilities
{
    static class HttpUtility
    {
        public static async Task<string> Request(string url)
        {
            using var client = new HttpClient();
            using HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public static async Task<HtmlDocument> GetHtmlFrom(string url)
        {
            string data = await HttpUtility.Request(url);

            var document = new HtmlDocument();
            document.LoadHtml(data);

            return document;
        }
    }
}
