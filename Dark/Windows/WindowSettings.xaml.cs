using System.Windows;

namespace OxyPlot.Dark.Wpf
{
    /// <summary>
    /// Interaction logic for WindowSettings.xaml
    /// </summary>
    public partial class WindowSettings : Window
    {
        /// <summary>
        /// 
        /// </summary>
        public WindowSettings()
        {
            InitializeComponent();
        }

        private void ButtonOK_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
