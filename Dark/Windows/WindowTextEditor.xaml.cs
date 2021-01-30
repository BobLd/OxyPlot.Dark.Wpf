using System.Windows;

namespace OxyPlot.Dark.Wpf
{
    /// <summary>
    /// Interaction logic for WindowTextEditor.xaml
    /// </summary>
    public partial class WindowTextEditor : Window
    {
        /// <summary>
        /// 
        /// </summary>
        public WindowTextEditor()
        {
            InitializeComponent();
            this.Loaded += WindowTextEditor_Loaded;
            this.textBox.Focus();
        }

        private void WindowTextEditor_Loaded(object sender, RoutedEventArgs e)
        {
            this.textBox.SelectAll();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Text = this.textBox.Text;
            this.Close();
        }

        /// <summary>
        /// 
        /// </summary>
        public string Text { get; private set; }
    }
}
