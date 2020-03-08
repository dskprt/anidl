using anidl.Utils;
using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Windows;

namespace anidl.Provider.Providers {

    public class AnimePaheProvider : Provider {

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

        public override List<Anime.Episode> GetEpisodes(object id) {
            List<Anime.Episode> episodes = new List<Anime.Episode>();

            using(WebClient client = new WebClient()) {
                string str = client.DownloadString(string.Format("{0}?m=release&sort=episode_asc&id={1}", BASE_API_URL, id));
                dynamic json = JsonConvert.DeserializeObject(str);

                foreach(dynamic obj in json.data) {
                    episodes.Add(new Anime.Episode(new Anime((string)obj.anime_title, "https://animepahe.com/anime/" + (string)obj.anime_slug, (int)obj.anime_id),
                        (string)obj.title, (int)obj.episode, "https://animepahe.com/anime/" + (string)obj.anime_slug + "/" + (string)obj.id, (int)obj.id));
                }
            }

            return episodes;
        }

        public override void Download(string url) {
            using(WebClient client = new WebClient()) {
                string str = client.DownloadString(url);

                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(str);

                // TODO: server selection
                string host = doc.DocumentNode.SelectSingleNode("//button[@data-provider]").InnerText;

                // TODO: yeah this too
                if(host != "kwik") {
                    MessageBox.Show("Not supported");
                    return;
                }

                int id = int.Parse(url.Split('/')[url.Split('/').Length - 1]);

                str = client.DownloadString(string.Format("{0}?id={1}&m=embed&p={2}", BASE_API_URL, id, host));
                dynamic json = JsonConvert.DeserializeObject(str);

                // TODO: quality selector (for now defaults to 720p, unless it 404s)
                foreach(dynamic obj in json.data) {
                    foreach(dynamic ob in obj) {
                        foreach(dynamic o in ob) {
                            foreach(dynamic res in o) {
                                try {
                                    client.Headers.Add("Referrer", string.Format("{0}?id={1}&m=embed&p={2}", BASE_API_URL, id, host));
                                    str = client.DownloadString((string)res.url);
                                    doc.LoadHtml(str);

                                    foreach (var node in doc.DocumentNode.SelectNodes("//script")) {
                                        if (node.InnerText.Contains("eval(")) {
                                            string[] arr = node.InnerText.Split('\'');
                                            MessageBox.Show(arr[arr.Length - 4]);
                                        }
                                    }
                                } catch(WebException e) {
                                    continue;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
