using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace OxyPlot.Dark.Wpf
{
    /// <summary>
    /// Interaction logic for PlotViewControl.xaml
    /// </summary>
    public partial class PlotViewControl : UserControl
    {
        private WindowSettings _windowSettings;

        /// <summary>
        /// 
        /// </summary>
        public PlotViewControl()
        {
            InitializeComponent();

            this.OptionsMenu.dataGrid.ItemsSource = this.CandleStickPlotView.ObservableSeries;
            this.OptionsMenu.dataGrid.MouseLeave += (s, e) =>
            {
                this.OptionsMenu.dataGrid.CommitEdit();
                this.CandleStickPlotView.Model.InvalidatePlot(false);
            };
            OptionsMenu.buttonHide.Click += (s, e) => { TopMenu.ButtonOptions.IsChecked = false; };
            OptionsMenu.checkBoxAdjustY.Click += (s, e) => { this.CandleStickPlotView.AdjustY = (bool)OptionsMenu.checkBoxAdjustY.IsChecked; };

            // PlotViewMenu events
            TopMenu.ButtonChart.ToolTip = "Line Chart";
            TopMenu.ButtonChart.Checked += ButtonChart_Checked;
            TopMenu.ButtonChart.Unchecked += ButtonChart_Unchecked;
            TopMenu.ButtonZoom.Click += ButtonZoom_Click;
            TopMenu.ButtonDraw.Checked += ButtonDraw_Checked;
            TopMenu.ButtonDraw.Unchecked += ButtonDraw_Unchecked;
            TopMenu.ButtonSettings.Click += ButtonSettings_Click;
            TopMenu.ButtonOptions.Checked += ButtonOptions_Checked;
            TopMenu.ButtonOptions.Unchecked += ButtonOptions_Unchecked;

            // DrawingToolsMenu events
            AnnotationsMenu.ButtonDrawingPen.Checked += ButtonDrawingPen_Checked;
            AnnotationsMenu.ButtonDrawingPen.Unchecked += ButtonDrawing_Unchecked;

            AnnotationsMenu.ButtonDrawingArrow.Checked += ButtonDrawingArrow_Checked;
            AnnotationsMenu.ButtonDrawingArrow.Unchecked += ButtonDrawing_Unchecked;

            AnnotationsMenu.ButtonDrawingLine.Checked += ButtonDrawingLine_Checked;
            AnnotationsMenu.ButtonDrawingLine.Unchecked += ButtonDrawing_Unchecked;

            AnnotationsMenu.ButtonDrawingVerticalLine.Checked += ButtonDrawingVerticalLine_Checked;
            AnnotationsMenu.ButtonDrawingVerticalLine.Unchecked += ButtonDrawing_Unchecked;

            AnnotationsMenu.ButtonDrawingPctChange.Checked += ButtonDrawingPctChange_Checked;
            AnnotationsMenu.ButtonDrawingPctChange.Unchecked += ButtonDrawing_Unchecked;

            AnnotationsMenu.ButtonDrawingText.Checked += ButtonDrawingText_Checked;
            AnnotationsMenu.ButtonDrawingText.Unchecked += ButtonDrawing_Unchecked;

            AnnotationsMenu.ButtonDrawingRectangle.Checked += ButtonDrawingRectangle_Checked;
            AnnotationsMenu.ButtonDrawingRectangle.Unchecked += ButtonDrawing_Unchecked;

            AnnotationsMenu.ButtonDrawingAverage.Checked += ButtonDrawingAverage_Checked;
            AnnotationsMenu.ButtonDrawingAverage.Unchecked += ButtonDrawing_Unchecked;

            AnnotationsMenu.ButtonDrawingRegression.Checked += ButtonDrawingRegression_Checked;
            AnnotationsMenu.ButtonDrawingRegression.Unchecked += ButtonDrawing_Unchecked;

            this.KeyDown += OnChartKeyDown;
            this.AnnotationsMenu.KeyDown += OnChartKeyDown;
            this.TopMenu.KeyDown += OnChartKeyDown;
            this.MouseRightButtonDown += CandleStickPlotView.PlotViewControl_MouseRightButtonDown;
        }

        private void ButtonChart_Unchecked(object sender, RoutedEventArgs e)
        {
            //TopMenu.ButtonChart.ToolTip = "Line Chart";
            this.CandleStickPlotView.ToggleSeriesType();
            e.Handled = true;
            CandleStickPlotView.Focus();
        }

        private void ButtonChart_Checked(object sender, RoutedEventArgs e)
        {
            //TopMenu.ButtonChart.ToolTip = "Candle Chart";
            this.CandleStickPlotView.ToggleSeriesType();
            e.Handled = true;
            CandleStickPlotView.Focus();
        }

        private void OnChartKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    AnnotationsMenu.UncheckAllButtonDrawings();
                    if (CandleStickPlotView.IsZoomActivated)
                    {
                        CandleStickPlotView.SetCursorType(CursorType.Default);
                        CandleStickPlotView.IsZoomActivated = false;
                    }
                    e.Handled = true;
                    break;

                default:
                    break;
            }
        }

        private void ButtonOptions_Unchecked(object sender, RoutedEventArgs e)
        {
            foreach (var serie in this.CandleStickPlotView.Model.Series)
            {
                if (serie is IOptimisedSeries) ((IOptimisedSeries)serie).SetCanTrack(false);
            }

            Storyboard storyboard = new Storyboard();

            DoubleAnimation animation = new DoubleAnimation();
            animation.EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut };
            animation.Duration = new Duration(TimeSpan.FromMilliseconds(1000));
            storyboard.Children.Add(animation);
            animation.From = PlotViewOptionsMenu.ControlWidth;
            animation.To = 0;
            Storyboard.SetTarget(animation, optionColumnDefinition);
            Storyboard.SetTargetProperty(animation, new PropertyPath("(ColumnDefinition.MaxWidth)"));
            storyboard.Completed += (s, e1) =>
            {
                OptionsMenu.Visibility = Visibility.Collapsed;
            };

            storyboard.Completed += Storyboard_Completed;
            storyboard.Begin();
        }

        private void ButtonOptions_Checked(object sender, RoutedEventArgs e)
        {
            foreach (var serie in this.CandleStickPlotView.Model.Series)
            {
                if (serie is IOptimisedSeries) ((IOptimisedSeries)serie).SetCanTrack(false);
            }

            OptionsMenu.Visibility = Visibility.Visible;
            Storyboard storyboard = new Storyboard();

            DoubleAnimation animation = new DoubleAnimation();
            animation.EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut };
            animation.Duration = new Duration(TimeSpan.FromMilliseconds(1000));
            storyboard.Children.Add(animation);
            animation.From = 0;
            animation.To = PlotViewOptionsMenu.ControlWidth;
            Storyboard.SetTarget(animation, optionColumnDefinition);
            Storyboard.SetTargetProperty(animation, new PropertyPath("(ColumnDefinition.MaxWidth)"));

            storyboard.Completed += Storyboard_Completed;
            storyboard.Begin();
        }

        private void Storyboard_Completed(object sender, EventArgs e)
        {
            foreach (var serie in this.CandleStickPlotView.Model.Series)
            {
                if (serie is IOptimisedSeries) ((IOptimisedSeries)serie).SetCanTrack(true);
            }
            this.CandleStickPlotView.Model.InvalidatePlot(true);
        }

        private void ButtonSettings_Click(object sender, RoutedEventArgs e)
        {
            if (_windowSettings == null)
            {
                _windowSettings = new WindowSettings();
                _windowSettings.Closing += (s, e2) =>
                {
                    if (_windowSettings.AdjustY.IsChecked != null)
                    {
                        this.CandleStickPlotView.AdjustY = (bool)_windowSettings.AdjustY.IsChecked;
                    }
                    _windowSettings = null;
                };
                _windowSettings.AdjustY.IsChecked = this.CandleStickPlotView.AdjustY;
                _windowSettings.Show();
            }
        }

        private void ButtonDrawingRegression_Checked(object sender, RoutedEventArgs e)
        {
            CandleStickPlotView.DrawingType = DrawingType.Regression;
            e.Handled = true;
            CandleStickPlotView.Focus();
        }

        private void ButtonDrawingAverage_Checked(object sender, RoutedEventArgs e)
        {
            CandleStickPlotView.DrawingType = DrawingType.Average;
            e.Handled = true;
            CandleStickPlotView.Focus();
        }

        private void ButtonDrawingRectangle_Checked(object sender, RoutedEventArgs e)
        {
            CandleStickPlotView.DrawingType = DrawingType.Rectangle;
            e.Handled = true;
            CandleStickPlotView.Focus();
        }

        private void ButtonDrawingLine_Checked(object sender, RoutedEventArgs e)
        {
            CandleStickPlotView.DrawingType = DrawingType.Line;
            e.Handled = true;
            CandleStickPlotView.Focus();
        }

        private void ButtonDrawingVerticalLine_Checked(object sender, RoutedEventArgs e)
        {
            CandleStickPlotView.DrawingType = DrawingType.VerticalLine;
            e.Handled = true;
            CandleStickPlotView.Focus();
        }

        private void ButtonDrawingText_Checked(object sender, RoutedEventArgs e)
        {
            CandleStickPlotView.DrawingType = DrawingType.Text;
            e.Handled = true;
            CandleStickPlotView.Focus();
        }

        private void ButtonDrawingPctChange_Checked(object sender, RoutedEventArgs e)
        {
            CandleStickPlotView.SetCursorType(CursorType.ZoomRectangle);
            CandleStickPlotView.DrawingType = DrawingType.PctChange;
            e.Handled = true;
            CandleStickPlotView.Focus();
        }

        private void ButtonDrawingArrow_Checked(object sender, RoutedEventArgs e)
        {
            CandleStickPlotView.DrawingType = DrawingType.Arrow;
            e.Handled = true;
            CandleStickPlotView.Focus();
        }

        private void ButtonDrawingPen_Checked(object sender, RoutedEventArgs e)
        {
            CandleStickPlotView.DrawingType = DrawingType.Pen;
            e.Handled = true;
            CandleStickPlotView.Focus();
        }

        private void ButtonDrawing_Unchecked(object sender, RoutedEventArgs e)
        {
            CandleStickPlotView.SetCursorType(CursorType.Default);
            CandleStickPlotView.EndAnnotations();
            e.Handled = true;
            CandleStickPlotView.Focus();
        }

        private void ButtonDraw_Unchecked(object sender, RoutedEventArgs e)
        {
            CandleStickPlotView.SetCursorType(CursorType.Default);
            CandleStickPlotView.EndAnnotations();
            AnnotationsMenu.mainGrid.Visibility = Visibility.Collapsed;
            AnnotationsMenu.UncheckAllButtonDrawings();
            e.Handled = true;
            CandleStickPlotView.Focus();
        }

        private void ButtonDraw_Checked(object sender, RoutedEventArgs e)
        {
            AnnotationsMenu.mainGrid.Visibility = Visibility.Visible;
            e.Handled = true;
            CandleStickPlotView.Focus();
        }

        private void ButtonZoom_Click(object sender, RoutedEventArgs e)
        {
            CandleStickPlotView.SetCursorType(CursorType.ZoomRectangle);
            CandleStickPlotView.IsZoomActivated = true;
            e.Handled = true;
            CandleStickPlotView.Focus();
        }

        /// <summary>
        /// 
        /// </summary>
        public DarkPlotView CandleStickPlotView
        {
            get
            {
                return candleStickPlotView;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public PlotViewTopMenu TopMenu
        {
            get
            {
                return plotViewTopMenu;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public PlotViewAnnotationsMenu AnnotationsMenu
        {
            get
            {
                return plotViewAnnotationsMenu;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public PlotViewOptionsMenu OptionsMenu
        {
            get
            {
                return plotViewOptionsMenu;
            }
        }
    }
}
