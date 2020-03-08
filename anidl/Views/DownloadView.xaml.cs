using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using anidl.Utils;

namespace anidl.Views {
    /// <summary>
    /// Interaction logic for DownloadView.xaml
    /// </summary>
    public partial class DownloadView : UserControl {

        private List<Item> checkedItems = new List<Item>();
        private Provider.Provider provider = Provider.Provider.GetProviderByName(Settings.provider);

        public DownloadView(List<Anime.Episode> episodes) {
            InitializeComponent();

            foreach(Anime.Episode episode in episodes) {
                this.episodes.Items.Add(new Item() { Anime = episode.anime.title, Title = episode.title,
                    URL = episode.url, Number = episode.number, Id = episode.id });
            }
        }

        public class Item {

            public string Anime { get; set; }
            public string Title { get; set; }
            public string URL { get; set; }
            public int Number { get; set; }
            public int Id { get; set; }
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e) {
            this.episodes.Width = e.NewSize.Width - 20;
            this.episodes.Height = e.NewSize.Height - 125;
        }

        private void DLAll_Click(object sender, RoutedEventArgs e) {
            foreach(Item item in this.episodes.Items) {
                provider.Download(item.URL);
            }
        }

        private void DLChecked_Click(object sender, RoutedEventArgs e) {
            foreach (Item item in this.checkedItems) {
                provider.Download(item.URL);
            }
        }

        private void episodes_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            foreach(Item item in e.AddedItems) {
                checkedItems.Add(item);
            }

            foreach(Item item in e.RemovedItems) {
                checkedItems.Remove(item);
            }
        }
    }
}
