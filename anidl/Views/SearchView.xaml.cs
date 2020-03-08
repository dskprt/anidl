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
    /// Interaction logic for SearchView.xaml
    /// </summary>
    public partial class SearchView : UserControl {
        public SearchView() {
            InitializeComponent();
        }

        private void Search_Click(object sender, RoutedEventArgs e) {
            Provider.Provider provider = Provider.Provider.GetProviderByName(anidl.Utils.Settings.provider);

            foreach (Window window in Application.Current.Windows) {
                if (window.GetType() == typeof(MainWindow)) {
                    (window as MainWindow).MainContentControl.Content = new AnimeListView(provider.Search(aniname.Text));
                }
            }
        }

        private void Settings_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            foreach (Window window in Application.Current.Windows) {
                if (window.GetType() == typeof(MainWindow)) {
                    (window as MainWindow).MainContentControl.Content = new SettingsView();
                }
            }
        }

        private void aniname_KeyDown(object sender, KeyEventArgs e) {
            if(e.Key == Key.Enter) {
                e.Handled = true;
                Search_Click(null, null);
            }
        }
    }
}
