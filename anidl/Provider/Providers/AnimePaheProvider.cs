using anidl.Utils;
using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
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
                                    client.Headers.Add("Referer", "https://animepahe.com/");
                                    client.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:64.0) Gecko/20100101 Firefox/64.0");

                                    str = client.DownloadString((string)res.url);
                                    doc.LoadHtml(str);

                                    foreach (var node in doc.DocumentNode.SelectNodes("//script")) {
                                        if (node.InnerText.Contains("eval(")) {
                                            string[] arr = node.InnerText.Split('\'');
                                            string src = GetKwikSource(arr[arr.Length - 4]);
                                        }
                                    }

                                    break;
                                } catch(WebException e) {
                                    if(((HttpWebResponse)e.Response).StatusCode == HttpStatusCode.NotFound) {
                                        continue;
                                    } else {
                                        MessageBox.Show(e.Message);
                                    }
                                }
                            }

                            break;
                        }

                        break;
                    }

                    break;
                }
            }
        }

        private string GetKwikSource(string data) {
            string p = "g m='1r://11-10.Z.Y/X/W/V/U.T?S=R&Q=N';g b=O.C('b');g 0=r M(b,{'L':{'K':s},'J':'16:9','I':5,'H':{'G':'F'},E:['8-D','8','12','P-d','14','13','1m','1n','1o','1p','t'],'t':{'1q':s}});4(!w.1t()){b.1u=m}p{f q={1v:1s};g c=r w(q);c.1i(m);c.1g(b);i.c=c}0.2('1d',7=>{i.1c.1b.1a('19')});0.18=1;j x(a,l,n){4(a.o){a.o(l,n,17)}p 4(a.u){a.u('2'+l,n)}}f 6=j(k){i.B.15(k,'*')};x(i,'k',j(e){f 3=e.3;4(3==='8')0.8();4(3==='h')0.h();4(3==='y')0.y();4(3.1e('d')){f d=1f(3.1h('-')[1]);1j.1k(d);0.z=d}});0.2('A',7=>{6('A')});0.2('8',7=>{6('8')});0.2('h',7=>{6('h')});0.2('1l',7=>{6(0.z)});0.2('v',7=>{6('v')});";
            int a = 62;
            int c = 94;
            string[] k = data.Split('|');
            int e = 0;
            Dictionary<string, string> d = new Dictionary<string, string>();

            string One(int _c) {
                return (_c < a ? "" : One(_c / a)) + ((_c = _c % a) > 35 ?
                    Convert.ToString(Convert.ToChar(c + 29)) : Base36Converter.ConvertTo(_c));
            }

            for(c = c - 1; c > 0; c--) {
                d[One(c)] = (string.IsNullOrEmpty(k[c])) ? One(c) : k[c];
            }

            for(c = 93; c > 0; c--) {
                p = Regex.Replace(p, "\\b" + One(c) + "\\b", d[One(c)]);
            }

            string match = Regex.Match(p, "'[^']+'").Value;

            return match.Substring(1, match.Length - 2);
        }
    }
}
