using anidl.Utils;
using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;

namespace anidl.Provider.Providers {

    public class AnimePaheProvider : Provider {

        [DllImport("wininet.dll", SetLastError = true)]
        private static extern bool InternetGetCookieEx(string pchURL, string pchCookieName,
               StringBuilder pchCookieData, ref System.UInt32 pcchCookieData,
               int dwFlags, IntPtr lpReserved);

        private const string BASE_API_URL = "https://animepahe.com/api";

        public override List<Anime> Search(string query) {
            List<Anime> animes = new List<Anime>();

            using(WebClient client = new WebClient()) {
                string str = client.DownloadString(string.Format("{0}?l=10&m=search&q={1}", BASE_API_URL, query.Replace(" ", "+")));
                dynamic json = JsonConvert.DeserializeObject(str);

                foreach(dynamic obj in json.data) {
                    animes.Add(new Anime((string)obj.title, "https://animepahe.com/anime/" + (string)obj.slug, (int)obj.id));
                }
            }

            return animes;
        }

        public override List<Anime.Episode> GetEpisodes(Anime anime) {
            List<Anime.Episode> episodes = new List<Anime.Episode>();

            using(WebClient client = new WebClient()) {
                string str = client.DownloadString(string.Format("{0}?m=release&sort=episode_asc&id={1}", BASE_API_URL, anime.id));
                dynamic json = JsonConvert.DeserializeObject(str);

                foreach(dynamic obj in json.data) {
                    episodes.Add(new Anime.Episode(anime, (string)obj.title, (int)obj.episode,
                        anime.url + "/" + (string)obj.session, (string)obj.session));
                }
            }

            return episodes;
        }

        public override void Download(Anime.Episode episode) {
            string url = episode.anime.url.Replace("/anime/", "/play/") + "/" + episode.id;
            Debug.WriteLine(url);

            HtmlDocument doc = new HtmlDocument();

            using (WebClient client = new WebClient()) {
                doc.LoadHtml(client.DownloadString(url));
            }

            string script = doc.DocumentNode.SelectSingleNode("//script[3]").InnerHtml;
            Regex regex = new Regex("https:\\/\\/([a-zA-Z]+.[a-zA-Z]+)\\/e\\/([a-zA-Z0-9]+)");

            url = regex.Match(script).Value;

            if(!url.StartsWith("https://kwik.cx/")) {
                MessageBox.Show("Not supported: " + url);
                return;
            }

            url = url.Replace("/e/", "/f/");

            using(WebClient client = new WebClient()) {
                StringBuilder cookie = new StringBuilder();
                StringBuilder _cookie = new StringBuilder();
                System.UInt32 size = 256;

                bool result = InternetGetCookieEx(url, "cf_clearance", cookie, ref size,
                    0x00002000, IntPtr.Zero);
                bool _result = InternetGetCookieEx(url, "__cfduid", _cookie, ref size,
                    0x00002000, IntPtr.Zero);

                /*
                 * Host: kwik.cx
User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:74.0) Gecko/20100101 Firefox/74.0
Accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,
                ; q = 0.8
Accept - Language: pl,en - US; q = 0.7,en; q = 0.3
Accept - Encoding: gzip, deflate, br
  Connection: keep - alive
Cookie: __cfduid = d3e1ba2f15a2bfcb39d46c7a89f2d61401583685480; cf_clearance = 6c5ec57a5f15e47b04420ec786b22615911f401d - 1584721798 - 0 - 150; kwik_session = eyJpdiI6IlFjc3k1dkJLWjZlR3JEU3gwVHMrdHc9PSIsInZhbHVlIjoiSEdjbUZJZnAzVlMwODJWSUNyOWhLZjRMZXpLM3dzT1pmV2pVQ3JkXC9zV3FwdEpNYm0ybEN0NmM0a2FENDdKRmwiLCJtYWMiOiI0YjYxZDNlZGRiNTcyNTMzZTUwZGE0ODU4NjhmOWMwNTg5ZmJhZTRhZmMyMGJhZDQ2NDRhZmQ4MDA5ZTI3OGUwIn0 % 3D
Upgrade - Insecure - Requests: 1
Cache - Control: max - age = 0
                   */

                client.Headers.Add(HttpRequestHeader.UserAgent, "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:64.0) Gecko/20100101 Firefox/64.0");
                client.Headers.Add("Origin", "https://kwik.cx");
                client.Headers.Add("Referer", url);
                client.Headers.Add("Upgrade-Insecure-Requests", "1");
                client.Headers.Add("Accept-Encoding", "gzip,deflate,br");
                client.Headers.Add("Accept-Language", "pl,en-US,q=0.7,en;q=0.3");
                client.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,;q=0.8");
                client.Headers.Add("Host", "kwik.cx");

                if(result && _result) {
                    Debug.WriteLine("yes result");
                    client.Headers.Add(HttpRequestHeader.Cookie, cookie.ToString() + ";" + _cookie.ToString());
                }

                foreach(var d in client.Headers.AllKeys) {
                    Debug.WriteLine(d);
                }

                string str = "";

                try {
                    str = client.DownloadString(url);
                } catch(Exception e) {
                    new WebBrowserWindow(url).ShowDialog();

                    client.Headers.Add(HttpRequestHeader.Cookie, Settings.cfCookies["kwik"]);
                    Debug.WriteLine(client.Headers["Cookie"]);
                    str = client.DownloadString(url);
                }

                Debug.WriteLine(str);
            }
        }
    }
}
