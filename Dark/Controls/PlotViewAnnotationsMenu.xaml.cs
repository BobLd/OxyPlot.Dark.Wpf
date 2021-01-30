using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace OxyPlot.Dark.Wpf
{
    /// <summary>
    /// Interaction logic for DrawingToolsMenu.xaml
    /// </summary>
    public partial class PlotViewAnnotationsMenu : UserControl
    {
        /// <summary>
        /// 
        /// </summary>
        public PlotViewAnnotationsMenu()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        public void UncheckAllButtonDrawings()
        {
            foreach (var button in ControlHelper.FindVisualChildren<ToggleButton>(this))
            {
                button.IsChecked = false;
            }
        }

        private void ButtonDrawing_Checked(object sender, RoutedEventArgs e)
        {
            var button = (ToggleButton)sender;
            button.Foreground = Brushes.White;

            foreach (ToggleButton but in this.FindVisualChildren<ToggleButton>())
            {
                if (!but.Equals(button)) but.IsChecked = false;
            }

        }

        private void ButtonDrawing_Unchecked(object sender, RoutedEventArgs e)
        {
            var button = (ToggleButton)sender;
            button.Foreground = OxyColorsDark.SciChartTextBrush;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public enum DrawingType
    {
        /// <summary>
        /// Draw using a pen
        /// </summary>
        Pen,

        /// <summary>
        /// Draw a line
        /// </summary>
        Line,

        /// <summary>
        /// Draw a vertical line
        /// </summary>
        VerticalLine,

        /// <summary>
        /// Draw a arrow
        /// </summary>
        Arrow,

        /// <summary>
        /// Draw a form
        /// </summary>
        PctChange,

        /// <summary>
        /// Insert a text box
        /// </summary>
        Text,

        /// <summary>
        /// Path
        /// </summary>
        Path,

        /// <summary>
        /// 
        /// </summary>
        Rectangle,

        /// <summary>
        /// 
        /// </summary>
        Average,

        /// <summary>
        /// 
        /// </summary>
        Regression,

        /// <summary>
        /// No drawing
        /// </summary>
        None
    }
}
