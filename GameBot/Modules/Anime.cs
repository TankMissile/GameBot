using Discord.Commands;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Fizzler.Systems.HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace GameBot.Modules
{
    [Group("anime")]
    [Summary("Provides commands used to get information on this season's anime")]
    public class Anime : ModuleBase<SocketCommandContext>
    {
        [Command("all")]
        public async Task AllAnimeAsync()
        {
            //Get the list of anime
            var web = new HtmlWeb();
            var document = await web.LoadFromWebAsync("https://www.crunchyroll.com/simulcastcalendar?filter=premium");
            var page = document.DocumentNode;

            List<String> shows = new List<String>();
            foreach(var item in page.QuerySelectorAll(".release"))
            {
                var title = WebUtility.HtmlDecode(item.QuerySelector(".season-name").InnerText.Trim());
                shows.Add(title.Trim());
            }

            shows = shows.Distinct().ToList();
            shows.Sort((x, y) => String.Compare(x, y));

            string msg = String.Join("\n", shows.ToArray());
            Console.WriteLine(msg);
            await ReplyAsync(msg);
        }

        [Command]
        public async Task AnimeAsync()
        {
            //Get the list of anime
            var web = new HtmlWeb();
            var document = await web.LoadFromWebAsync("https://www.crunchyroll.com/simulcastcalendar?filter=premium");
            var page = document.DocumentNode;

            List<String> shows = new List<String>();
            foreach (var item in page.QuerySelector(".today").QuerySelectorAll(".release"))
            {
                var title = WebUtility.HtmlDecode(item.QuerySelector(".season-name").InnerText.Trim());
                var link = WebUtility.HtmlDecode(item.QuerySelector(".available-episode-link").Attributes["href"].Value);
                shows.Add(title + " " + link);
            }

            shows = shows.Distinct().ToList();
            shows.Sort((x, y) => String.Compare(x, y));

            string msg = String.Join("\n", shows.ToArray());
            Console.WriteLine(msg);
            await ReplyAsync(msg);
        }
    }
}
