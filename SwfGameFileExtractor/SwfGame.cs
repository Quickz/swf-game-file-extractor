using HtmlAgilityPack;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace SwfGameFileExtractor
{
    class SwfGame
    {
        public string Title { get; private set; }
        public string SwfFileUrl { get; private set; }

        public SwfGame(string title, string swfFileUrl)
        {
            Title = title;
            SwfFileUrl = swfFileUrl;
        }
    }
}
