using Discord.Commands;
using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace GameBot.Modules
{
    [Group("anime")]
    [Name("Anime")]
    [Summary("Provides commands used to get information on this season's anime")]
    public class Anime : ModuleBase<SocketCommandContext>
    {
        private delegate string Query(HtmlNode item);
        private const string CR_PREMIUM = "filter=premium";
        private const string CR_FREE = "filter=free";
        private const string CR_TODAY = ".today";
        private const string CR_RELEASE = ".release";
        private const string CR_CALENDAR = "simulcastcalendar";
        private const string CR_SEASON_NAME = ".season-name";
        private const string CR_EPISODE_LINK = ".available-episode-link";
        private const string CR_LINEUP = "lineup";
        private const string CR_LINEUP_TITLE = ".lineup-series-title";
        private const string CR_LINEUP_CELL = ".lineup-portrait-grid > li";

        [Command, Name("anime")]
        [Summary("Provides a list of today's new anime with links to the new episode")]
        public async Task AnimeAsync()
        {
            //Get the list of anime
            var page = await GetCRPage(CR_CALENDAR, CR_PREMIUM);

            List<String> shows = GetCRAttributes(page, String.Join(" ", CR_TODAY, CR_RELEASE), GetCRCalendarName, GetCRCalendarLink);

            string msg = String.Join("\n", shows.ToArray());
            await ReplyAsync(msg);
        }

        [Command("free")]
        [Summary("Provides a list of today's new free anime with links to the new episode")]
        public async Task FreeAnimeAsync()
        {
            //Get the list of anime
            var page = await GetCRPage(CR_CALENDAR, CR_FREE);

            List<String> shows = GetCRAttributes(page, String.Join(" ", CR_TODAY, CR_RELEASE), GetCRCalendarName, GetCRCalendarLink);

            string msg = String.Join("\n", shows.ToArray());
            await ReplyAsync(msg);
        }

        [Command("all")]
        [Summary("Provides a list of this week's new anime")]
        public async Task AllAnimeAsync()
        {
            try
            {
                //Get the list of anime
                var page = await GetCRPage(CR_LINEUP);

                List<String> shows = GetCRAttributes(page, CR_LINEUP_CELL, GetCRLineupTitle);

                string msg = String.Join("\n", shows.ToArray());
                await ReplyAsync(msg);
            }
            catch (Exception e)
            {
                Console.Write(e.StackTrace);
                await ReplyAsync("Oops! There was a problem retrieving anime info.  Try reading a book instead!");
            }
        }

        //Parses the crunchyroll page, returning
        private List<String> GetCRAttributes(HtmlNode page, string selector, params Query[] queries)
        {
            var shows = new List<String>();

            foreach (var item in page.QuerySelectorAll(selector))
            {
                //get these filthy dubs out of my face
                if (item.InnerText.Contains("Dub")) continue;

                var showAttributes = new List<String>();
                    foreach (var query in queries)
                    {
                        showAttributes.Add(query(item));
                    }

                shows.Add(String.Join(" ", showAttributes));
            }

            shows = shows.Distinct().ToList();
            shows.Sort((x, y) => String.Compare(x, y));

            return shows;
        }
        
        private async Task<HtmlNode> GetCRPage(string page, params string[] urlParams)
        {
            var web = new HtmlWeb();
            var document = await web.LoadFromWebAsync("https://www.crunchyroll.com/" + page + "?" + String.Join("&", urlParams));
            return document.DocumentNode;
        }

        private string GetCRCalendarName(HtmlNode item)
        {
            return WebUtility.HtmlDecode(item.QuerySelector(CR_SEASON_NAME).InnerText.Trim());
        }

        private string GetCRCalendarLink(HtmlNode item)
        {
            return WebUtility.HtmlDecode(item.QuerySelector(CR_EPISODE_LINK).Attributes["href"].Value);
        }

        private string GetCRLineupTitle(HtmlNode item)
        {
            return WebUtility.HtmlDecode(item.QuerySelector(CR_LINEUP_TITLE).InnerText.Trim());
        }
    }
}
