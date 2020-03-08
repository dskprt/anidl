using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace anidl.Utils {

    public class HttpUtils {

        private const string FIREFOX_USER_AGENT = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:54.0) Gecko/20100101 Firefox/73.0";

        public static HttpWebResponse Get(string url) {
            WebRequest request = WebRequest.Create(url);
            request.Method = "GET";
            request.Headers.Add(HttpRequestHeader.UserAgent, FIREFOX_USER_AGENT);

            return (HttpWebResponse) request.GetResponse();
        }

        public static string GetHTML(HttpWebResponse response) {
            Stream responseStream = response.GetResponseStream();
            StreamReader reader = null;

            if(string.IsNullOrWhiteSpace(response.CharacterSet)) {
                reader = new StreamReader(responseStream);
            } else {
                reader = new StreamReader(responseStream, Encoding.GetEncoding(response.CharacterSet));
            }

            try {
                return reader.ReadToEnd();
            } finally {
                response.Close();
                reader.Close();
            }
        }
    }
}
