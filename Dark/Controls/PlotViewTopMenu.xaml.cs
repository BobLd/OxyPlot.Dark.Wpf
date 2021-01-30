using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace OxyPlot.Dark.Wpf
{
    /// <summary>
    /// Interaction logic for PlotViewTopMenu.xaml
    /// </summary>
    public partial class PlotViewTopMenu : UserControl
    {
        /// <summary>
        /// 
        /// </summary>
        public PlotViewTopMenu()
        {
            InitializeComponent();
        }

        private void OnShowButtonsMenuEvent(object sender, MouseEventArgs e)
        {
            ShowButtonsMenu();
        }

        private void OnHideButtonsMenuEvent(object sender, MouseEventArgs e)
        {
            HideButtonsMenu();
        }

        private void ShowButtonsMenu()
        {
            if (gridButtons.Visibility != Visibility.Visible)
            {
                gridButtons.Visibility = Visibility.Visible;
            }

            if (gridActivator.Visibility != Visibility.Collapsed)
            {
                gridActivator.Visibility = Visibility.Collapsed;
            }
        }

        private void HideButtonsMenu()
        {
            if (gridButtons.Visibility != Visibility.Collapsed)
            {
                gridButtons.Visibility = Visibility.Collapsed;
            }

            if (gridActivator.Visibility != Visibility.Visible)
            {
                gridActivator.Visibility = Visibility.Visible;
            }
        }

        //private void ButtonAlert_Checked(object sender, RoutedEventArgs e)
        //{
        //    ButtonAlert.Content = "\uf142";
        //    ButtonAlert.ToolTip = "No alert";
        //}

        //private void ButtonAlert_Unchecked(object sender, RoutedEventArgs e)
        //{
        //    ButtonAlert.Content = "\uf140";
        //    ButtonAlert.ToolTip = "Alert";
        //}

        private void ButtonChart_Unchecked(object sender, RoutedEventArgs e)
        {
            ButtonChart.Content = "\uf116";
            ButtonChart.ToolTip = "Line Chart";
        }

        private void ButtonChart_Checked(object sender, RoutedEventArgs e)
        {
            ButtonChart.Content = "\uf390";
            ButtonChart.ToolTip = "Candle Chart";
        }


        private void ButtonDraw_Checked(object sender, RoutedEventArgs e)
        {
            ButtonDraw.Content = "\uf298";
            ButtonDraw.ToolTip = "Close annotations";
        }

        private void ButtonDraw_Unchecked(object sender, RoutedEventArgs e)
        {
            ButtonDraw.Content = "\uf2b0";
            ButtonDraw.ToolTip = "Annotations";
        }
    }
}
