using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace anidl.Utils {

    public class Anime {

        public string title;
        public string url;
        public int id;

        public Anime(string title, string url, int id) {
            this.title = title;
            this.url = url;
            this.id = id;
        }

        public class Episode {

            public Anime anime;
            public string title;
            public int number;
            public string url;
            public int id;

            public Episode(Anime anime, string title, int number, string url, int id) {
                this.anime = anime;
                this.title = title;
                this.number = number;
                this.url = url;
                this.id = id;
            }
        }
    }
}
