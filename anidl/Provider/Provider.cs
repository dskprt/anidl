using anidl.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace anidl.Provider {

    public abstract class Provider {

        public abstract List<Anime> Search(string query);
        public abstract List<Anime.Episode> GetEpisodes(object id);
        public abstract void Download(string url);

        public static Provider GetProviderByName(string name) {
            try {
                return (Provider) Activator.CreateInstance(Type.GetType(string.Format("anidl.Provider.Providers.{0}Provider", name)));
            } catch(Exception e) {
                MessageBox.Show(e.Message);
                return null;
            }
        }
    }
}
