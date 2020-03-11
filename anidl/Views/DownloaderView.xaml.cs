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
    /// Interaction logic for DownloaderView.xaml
    /// </summary>
    public partial class DownloaderView : UserControl {

        private Provider.Provider provider = Provider.Provider.GetProviderByName(Settings.provider);

        public DownloaderView(Dictionary<string, string> toDownload) {
            InitializeComponent();

            foreach(var item in toDownload) {
                provider.Download(item.Value);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e) {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to cancel?", "anidl", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes) {
                // TODO: stop downloading here

                foreach (Window window in Application.Current.Windows) {
                    if (window.GetType() == typeof(MainWindow)) {
                        (window as MainWindow).MainContentControl.Content = new SearchView();
                    }
                }
            }
        }

        private void Pause_Click(object sender, RoutedEventArgs e) {
            // TODO
        }
    }
}
