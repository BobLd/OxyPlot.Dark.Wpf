using OxyPlot.Dark.Wpf.Dark.Annotations;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace OxyPlot.Dark.Wpf
{
    /// <summary>
    /// Interaction logic for PlotViewContextMenu.xaml
    /// </summary>
    public partial class PlotViewContextMenu : UserControl
    {
        private const double _width = 100;
        private const double _height = 123;
        private Annotations.Annotation _annotation;
        private DarkPlotView _candleStickPlotView;

        /// <summary>
        /// 
        /// </summary>
        public PlotViewContextMenu()
        {
            InitializeComponent();

            rowDefinitionDelete.Height = new GridLength(0);

            this.MouseLeave += (s, e) => { this.Visibility = Visibility.Collapsed; };

            // Save As
            this.Button1.Click += (s, e) =>
            {
                MessageBox.Show("Do Nothing");
            };

            // Copy to clipboard
            this.Button2.Click += (s, e) =>
            {
                _candleStickPlotView.CopyToClipboard();
            };
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="candleStickPlotView"></param>
        public void SetPlotView(DarkPlotView candleStickPlotView)
        {
            _candleStickPlotView = candleStickPlotView;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="annotation"></param>
        public void SetAnnotation(Annotations.Annotation annotation)
        {
            _annotation = annotation;

            if (_annotation.Tag is AnnotationTag)
            {
                this.labelType.Content = ((AnnotationTag)_annotation.Tag).Name;
            }
            else
            {
                this.labelType.Content = annotation.GetType().Name;
            }

            rowDefinitionDelete.Height = new GridLength(20);
            this.ButtonDelete.Click += (s, e) =>
            {
                _candleStickPlotView.RemoveAnnotation(_annotation, true);
            };
        }

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userControl"></param>
        /// <returns></returns>
        public static PlotViewContextMenu CreateContextMenu(PlotViewControl userControl)
        {
            Point cursorLocation = userControl.TransformToAncestor(userControl).Transform(Mouse.GetPosition(userControl));
            double x = cursorLocation.X + Math.Min(0, userControl.ActualWidth - (cursorLocation.X + _width)); // adjust for x> width
            double y = cursorLocation.Y + Math.Min(0, userControl.ActualHeight - (cursorLocation.Y + _height)); // adjust for x> width

            var candleStickPlotViewContextMenu = new PlotViewContextMenu()
            {
                Width = _width,
                Height = _height,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                VerticalAlignment = System.Windows.VerticalAlignment.Top,
                Margin = new Thickness(x - 5, y - 5, 0, 0)
            };

            return candleStickPlotViewContextMenu;
        }
    }
}
