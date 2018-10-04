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
        private delegate string Query(HtmlNode item);
        private string premiumCR = "filter=premium";
        private string freeCR = "filter=free";

        [Command("all")]
        [Summary("Provides a list of this week's new anime")]
        public async Task AllAnimeAsync()
        {
            //Get the list of anime
            var page = await getCRPage(premiumCR);

            List<String> shows = getCRAttributes(page, ".release", getCRName);

            string msg = String.Join("\n", shows.ToArray());
            await ReplyAsync(msg);
        }

        [Command("all free")]
        [Alias("free all")]
        [Summary("Provides a list of this week's new anime")]
        public async Task AllFreeAnimeAsync()
        {
            //Get the list of anime
            var page = await getCRPage(freeCR);

            List<String> shows = getCRAttributes(page, ".release", getCRName);

            string msg = String.Join("\n", shows.ToArray());
            await ReplyAsync(msg);
        }

        [Command]
        [Summary("Provides a list of today's new anime with links to the new episode")]
        public async Task AnimeAsync()
        {
            //Get the list of anime
            var page = await getCRPage(premiumCR);

            List<String> shows = getCRAttributes(page, ".today .release", getCRName, getCRLink);

            string msg = String.Join("\n", shows.ToArray());
            await ReplyAsync(msg);
        }

        [Command("free")]
        [Summary("Provides a list of today's new free anime with links to the new episode")]
        public async Task FreeAnimeAsync()
        {
            //Get the list of anime
            var page = await getCRPage(freeCR);

            List<String> shows = getCRAttributes(page, ".today .release", getCRName, getCRLink);

            string msg = String.Join("\n", shows.ToArray());
            await ReplyAsync(msg);
        }

        //Parses the crunchyroll page, returning
        private List<String> getCRAttributes(HtmlNode page, string selector, params Query[] queries)
        {
            var shows = new List<String>();

            foreach (var item in page.QuerySelectorAll(selector))
            {
                var showAttributes = new List<String>();
                foreach(var query in queries)
                {
                    showAttributes.Add(query(item));
                }

                shows.Add(String.Join(" ", showAttributes));
            }

            shows = shows.Distinct().ToList();
            shows.Sort((x, y) => String.Compare(x, y));

            return shows;
        }

        //Gets the calendar page from crunchyroll, as html
        private async Task<HtmlNode> getCRPage(params string[] urlParams)
        {
            var web = new HtmlWeb();
            var document = await web.LoadFromWebAsync("https://www.crunchyroll.com/simulcastcalendar?" + String.Join("&", urlParams));
            return document.DocumentNode;
        }

        private string getCRName(HtmlNode item)
        {
            return WebUtility.HtmlDecode(item.QuerySelector(".season-name").InnerText.Trim());
        }

        private string getCRLink(HtmlNode item)
        {
            return WebUtility.HtmlDecode(item.QuerySelector(".available-episode-link").Attributes["href"].Value);
        }
    }
}
