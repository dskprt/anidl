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
    /// Interaction logic for SettingsView.xaml
    /// </summary>
    public partial class SettingsView : UserControl {
        public SettingsView() {
            InitializeComponent();
        }

        private void Back_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            foreach (Window window in Application.Current.Windows) {
                if (window.GetType() == typeof(MainWindow)) {
                    (window as MainWindow).MainContentControl.Content = new SearchView();
                }
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            Settings.provider = provider.Text;
        }
    }
}
