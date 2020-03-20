using anidl.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace anidl.Views {
    /// <summary>
    /// Interaction logic for AnimeListView.xaml
    /// </summary>
    public partial class AnimeListView : UserControl {

        public AnimeListView(List<Anime> animes) {
            InitializeComponent();

            foreach(Anime anime in animes) {
                this.animes.Items.Add(new Item() { Title = anime.title, URL = anime.url, Id = anime.id });
            }
        }

        public class Item {

            public string Title { get; set; }
            public string URL { get; set; }
            public object Id { get; set; }
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            Item item = (Item)this.animes.SelectedItem;
            Provider.Provider provider = Provider.Provider.GetProviderByName(Settings.provider);

            foreach (Window window in Application.Current.Windows) {
                if (window.GetType() == typeof(MainWindow)) {
                    (window as MainWindow).MainContentControl.Content = new DownloadView(provider.GetEpisodes(
                        new Anime(item.Title, item.URL, item.Id)));
                }
            }
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e) {
            this.animes.Width = e.NewSize.Width - 20;
            this.animes.Height = e.NewSize.Height - 125;
        }
    }
}
