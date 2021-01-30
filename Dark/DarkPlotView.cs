using OxyPlot.Annotations;
using OxyPlot.Axes;
using OxyPlot.Dark.Wpf.Dark;
using OxyPlot.Dark.Wpf.Dark.Annotations;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace OxyPlot.Dark.Wpf
{
    /// <summary>
    /// https://csharp.hotexamples.com/examples/OxyPlot/TrackerHitResult/-/php-trackerhitresult-class-examples.html
    /// </summary>
    public class DarkPlotView : PlotView
    {
        /// <summary>
        /// 
        /// </summary>
        public readonly ObservableCollection<OxyPlot.Series.Series> ObservableSeries;

        /// <summary>
        /// 
        /// </summary>

        public DateTimeDarkAxis DateTimeAxis { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTimeDarkAxis DateTimeAxis2 { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public Axes.LinearAxis LinearAxis { get; private set; }

        private object _zoomLock;
        private bool _isZoomActivated;
        private object _adjustYLock;
        private bool _adjustY;

        private LineDarkSeries _tempSerie = null;
        private Annotations.ArrowAnnotation _tempArrowAnnotation = null;
        private Annotations.ArrowAnnotation _tempArrowAnnotation2 = null;
        private Annotations.ArrowAnnotation _tempArrowAnnotation3 = null;
        private Annotations.ArrowAnnotation _tempArrowAnnotation4 = null;
        private Annotations.ArrowAnnotation _tempArrowAnnotation5 = null;
        private Annotations.LineAnnotation _tempVerticalLineAnnotation = null;
        private Annotations.TextAnnotation _tempTextAnnotation = null;
        private Annotations.RectangleAnnotation _tempRectangleAnnotation = null;
        private WindowTextEditor _windowTextEditor;
        private PngExporter _pngExporter = new PngExporter { Width = 600, Height = 400, Background = OxyColors.White };

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        public void SaveToFile(string path)
        {
            try
            {
                _pngExporter.ExportToFile(this.Model, path);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void CopyToClipboard()
        {
            try
            {
                var bitmap = _pngExporter.ExportToBitmap(this.Model);
                Clipboard.SetImage(bitmap);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void CreateCurrentVerticalLineAnnotation()
        {
            _tempVerticalLineAnnotation = new Annotations.LineAnnotation
            {
                Type = LineAnnotationType.Vertical,
                X = 4,
                StrokeThickness = 1.0,
                LineStyle = LineStyle.Solid,
                Tag = new AnnotationTag("Vertical Line")
            };
            this.Model.Annotations.Add(_tempVerticalLineAnnotation);
            this.Model.InvalidatePlot(true);
        }

        private void uncheckAllAnnotations()
        {
            var mainGrid = this.GetUIParentCore() as Grid;
            if (mainGrid != null)
            {
                foreach (var annotationsMenu in mainGrid.FindVisualChildren<PlotViewAnnotationsMenu>())
                {
                    annotationsMenu.UncheckAllButtonDrawings();
                }
            }
        }

        /// <summary>
        /// To clean when Annotating is finished.
        /// </summary>
        public void EndAnnotations()
        {
            if (DrawingType == DrawingType.VerticalLine)
            {
                if (_tempVerticalLineAnnotation != null)
                {
                    this.Model.Annotations.Remove(_tempVerticalLineAnnotation);
                    this.Model.InvalidatePlot(false);
                }
            }
            DrawingType = DrawingType.None;
        }

        /// <summary>
        /// True if the zoom is activated from the Menu
        /// </summary>
        public bool IsZoomActivated
        {
            get
            {
                lock (_zoomLock)
                {
                    return _isZoomActivated;
                }
            }

            set
            {
                lock (_zoomLock)
                {
                    _isZoomActivated = value;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool AdjustY
        {
            get
            {
                lock (_adjustYLock)
                {
                    return _adjustY;
                }
            }

            set
            {
                lock (_adjustYLock)
                {
                    _adjustY = value;
                }
            }
        }

        private object _lockDrawing;
        private DrawingType _drawingType;

        /// <summary>
        /// 
        /// </summary>
        public DrawingType DrawingType
        {
            get
            {
                lock (_lockDrawing)
                {
                    return _drawingType;
                }
            }

            set
            {
                lock (_lockDrawing)
                {
                    _drawingType = value;

                    if (this.Model != null && this.Model.Series != null)
                    {
                        if (_drawingType == DrawingType.None)
                        {
                            // show tracker
                            foreach (var serie in this.Model.Series)
                            {
                                if (serie is IOptimisedSeries) ((IOptimisedSeries)serie).SetCanTrack(true);
                            }
                        }
                        else
                        {
                            // do not show tracker
                            foreach (var serie in this.Model.Series)
                            {
                                if (serie is IOptimisedSeries) ((IOptimisedSeries)serie).SetCanTrack(false);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void ToggleSeriesType()
        {
            //if (this.CandleStickSeries == null) return;
            //if (this.CandleStickSeries.SerieType != CandleStickSeries.SerieTypes.Line)
            //{
            //    this.CandleStickSeries.SerieType = CandleStickSeries.SerieTypes.Line;
            //}
            //else if (this.CandleStickSeries.SerieType != CandleStickSeries.SerieTypes.Candles)
            //{
            //    this.CandleStickSeries.SerieType = CandleStickSeries.SerieTypes.Candles;
            //}

            //this.Model.InvalidatePlot(true);
        }

        /// <summary>
        /// 
        /// </summary>
        public DarkPlotView()
                : base()
        {
            ObservableSeries = new ObservableCollection<OxyPlot.Series.Series>();

            _zoomLock = new object();
            IsZoomActivated = false;
            _adjustYLock = new object();
            AdjustY = true;
            _lockDrawing = new object();
            DrawingType = DrawingType.None;

            this.Background = OxyColorsDark.SciChartBackgroungBrush;
            this.Foreground = OxyColorsDark.SciChartMajorGridLineBrush;
            this.Controller = new DarkPlotController();
            
            var plotModel = new PlotModel
            {
                TextColor = OxyColorsDark.SciChartTextOxy,
                PlotAreaBorderColor = OxyColorsDark.SciChartMajorGridLineOxy,
                LegendTextColor = OxyColorsDark.SciChartLegendTextOxy,
                LegendTitleColor = OxyColorsDark.SciChartTextOxy,
                TitleColor = OxyColorsDark.SciChartTextOxy,
                SubtitleColor = OxyColorsDark.SciChartTextOxy,
            };

            plotModel.Series.CollectionChanged += Series_CollectionChanged;
            this.Model = plotModel;

            DateTimeAxis = new DateTimeDarkAxis
            {
                Key = "Primary",
                Position = AxisPosition.Bottom,
                AxislineColor = OxyColorsDark.SciChartMajorGridLineOxy,
                ExtraGridlineColor = OxyColorsDark.SciChartMajorGridLineOxy,
                ExtraGridlineStyle = LineStyle.DashDot,
                TicklineColor = OxyColorsDark.SciChartMajorGridLineOxy,
                MajorGridlineColor = OxyColorsDark.SciChartMajorGridLineOxy,
                TextColor = OxyColorsDark.SciChartTextOxy,
                MajorGridlineStyle = LineStyle.Solid,
                TickStyle = TickStyle.Outside,
            };
            this.Model.Axes.Add(DateTimeAxis);

            DateTimeAxis2 = new DateTimeDarkAxis
            {
                Key = DateTimeDarkAxis.SecondaryDateTimeAxisKey,
                Position = AxisPosition.Bottom,
                AxislineColor = OxyColorsDark.SciChartMajorGridLineOxy,
                ExtraGridlineColor = OxyColorsDark.SciChartMajorGridLineOxy,
                ExtraGridlineStyle = LineStyle.DashDot,
                TicklineColor = OxyColorsDark.SciChartTextOxy,
                Selectable = false,
                IntervalType = DateTimeIntervalType.Auto,
                AxisDistance = 30
            };
            this.Model.Axes.Add(DateTimeAxis2);

            LinearAxis = new Axes.LinearAxis
            {
                Position = AxisPosition.Right,
                AxislineColor = OxyColorsDark.SciChartMajorGridLineOxy,
                ExtraGridlineColor = OxyColorsDark.SciChartMajorGridLineOxy,
                MajorGridlineColor = OxyColorsDark.SciChartMajorGridLineOxy,
                TicklineColor = OxyColorsDark.SciChartMajorGridLineOxy,
                MinorGridlineColor = OxyColorsDark.SciChartMinorGridLineOxy,
                MinorTicklineColor = OxyColorsDark.SciChartMinorGridLineOxy,
                TextColor = OxyColorsDark.SciChartTextOxy,
                TitleColor = OxyColorsDark.SciChartTextOxy,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Solid,
                TickStyle = TickStyle.Outside
            };
            this.Model.Axes.Add(LinearAxis);

            //CandleStickSeries = new CandleStickSeries
            //{
            //    RenderInLegend = false,
            //    MarketOpen = new TimeSpan(8, 0, 0),
            //    MarketClose = new TimeSpan(16, 30, 0)
            //};
            //plotModel.Series.Add(CandleStickSeries);

            DateTimeAxis.AxisChanged += DateTimeAxis_AxisChanged;
            DateTimeAxis2.TransformChanged += DateTimeAxis2_TransformChanged;

            this.Model.MouseDown += PlotModel_MouseDown;
            this.Model.MouseMove += PlotModel_MouseMove;
            this.Model.MouseUp += PlotModel_MouseUp;
            //this.Model.RenderingDecorator = rc => new XkcdRenderingDecorator(rc);
        }

        private void DateTimeAxis_AxisChanged(object sender, AxisChangedEventArgs e)
        {
            AdjustYExtent(this.Model.Series.FirstOrDefault(), DateTimeAxis, LinearAxis);
        }

        private void DateTimeAxis2_TransformChanged(object sender, EventArgs e)
        {
            if (this.Model.Series.FirstOrDefault() is CandleStickSeries)
            {
                CandleStickSeries candleStickSeries = (CandleStickSeries)this.Model.Series.FirstOrDefault();

                var axis = (Axes.DateTimeAxis)sender;
                var maxDate = Axes.DateTimeAxis.ToDateTime(axis.ActualMaximum);
                var minDate = Axes.DateTimeAxis.ToDateTime(axis.ActualMinimum);

                // adding extra grid line for market open & close
                if ((maxDate - minDate).Days < 25)
                {
                    List<double> extraGL = new List<double>();
                    var currentDate = minDate;
                    while (currentDate.Date <= maxDate.Date)
                    {
                        if (currentDate.DayOfWeek != DayOfWeek.Saturday && currentDate.DayOfWeek != DayOfWeek.Sunday)
                        {
                            if (candleStickSeries.MarketOpen != null) extraGL.Add(Axes.DateTimeAxis.ToDouble(currentDate.Date.Add((TimeSpan)candleStickSeries.MarketOpen)));
                            if (candleStickSeries.MarketClose != null) extraGL.Add(Axes.DateTimeAxis.ToDouble(currentDate.Date.Add((TimeSpan)candleStickSeries.MarketClose)));
                        }
                        currentDate = currentDate.AddDays(1);
                    }
                    axis.ExtraGridlines = extraGL.ToArray();
                }
                else
                {
                    axis.ExtraGridlines = null;
                }
            }
        }

        private void Series_CollectionChanged(object sender, ElementCollectionChangedEventArgs<OxyPlot.Series.Series> e)
        {
            foreach (var serie in e.AddedItems)
            {
                ObservableSeries.Add(serie);
            }

            foreach (var serie in e.RemovedItems)
            {
                ObservableSeries.Remove(serie);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="annotation"></param>
        public void AddAnnotation(Annotations.Annotation annotation)
        {
            annotation.MouseDown += _annotation_MouseDown;
            this.Model.Annotations.Add(annotation);
            this.Model.InvalidatePlot(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="annotation"></param>
        /// <param name="removeSameTag">True to remove all annotation with same tag</param>
        public void RemoveAnnotation(Annotations.Annotation annotation, bool removeSameTag = false)
        {
            this.Model.Annotations.Remove(annotation);
            if (removeSameTag && annotation.Tag != null)
            {
                var tagged = this.Model.Annotations.Where(a => a.Tag == annotation.Tag).ToList();
                foreach (var ann in tagged)
                {
                    this.Model.Annotations.Remove(ann);
                }
            }
            this.Model.InvalidatePlot(false);
        }

        private void _annotation_MouseDown(object sender, OxyMouseDownEventArgs e)
        {
            if (e.ChangedButton == OxyMouseButton.Left)
            {
                if (sender is Annotations.TextAnnotation)
                {
                    if (e.ClickCount == 1)
                    {

                    }
                    else if (e.ClickCount == 2)
                    {
                        if (_windowTextEditor == null)
                        {
                            var annotation = (Annotations.TextAnnotation)sender;
                            _windowTextEditor = new WindowTextEditor();
                            _windowTextEditor.Closing += (s, e2) =>
                            {
                                if (_windowTextEditor.Text != null)
                                {
                                    annotation.Text = _windowTextEditor.Text.Trim();
                                    this.Model.InvalidatePlot(false);
                                    _windowTextEditor = null;
                                }
                            };
                            _windowTextEditor.textBox.Text = annotation.Text;
                            _windowTextEditor.Show();
                        }
                    }

                    e.Handled = true;
                }
                else
                {

                }
            }
            else if (e.ChangedButton == OxyMouseButton.Right)
            {
                var parent = this.Parent;
                if (!(parent is Grid)) return;
                if (((Grid)parent).Parent is PlotViewControl)
                {
                    ShowContextMenu((PlotViewControl)((Grid)parent).Parent, (Annotations.Annotation)sender);
                    e.Handled = true;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void PlotViewControl_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is PlotViewControl)
            {
                ShowContextMenu((PlotViewControl)sender);
            }
        }

        private PlotViewContextMenu _plotViewContextMenu = null;

        private void ShowContextMenu(PlotViewControl sender, Annotations.Annotation annotation = null)
        {
            sender.AnnotationsMenu.UncheckAllButtonDrawings();

            if (_plotViewContextMenu != null)
            {
                _plotViewContextMenu.Visibility = Visibility.Collapsed;
            }

            if (_plotViewContextMenu == null ||
            _plotViewContextMenu.Visibility != Visibility.Visible)
            {
                _plotViewContextMenu = PlotViewContextMenu.CreateContextMenu(sender);
                _plotViewContextMenu.SetPlotView(this);

                if (annotation != null)
                {
                    _plotViewContextMenu.SetAnnotation(annotation);
                }

                _plotViewContextMenu.IsVisibleChanged += (s, e) =>
                {
                    if (_plotViewContextMenu.Visibility != Visibility.Visible)
                    {
                        _plotViewContextMenu = null;
                    }
                };

                sender.MainGrid.Children.Add(_plotViewContextMenu);
            }
        }

        // https://github.com/oxyplot/oxyplot/blob/develop/Source/Examples/ExampleLibrary/Examples/MouseEventExamples.cs
        private void PlotModel_MouseDown(object sender, OxyMouseDownEventArgs e)
        {
            if (DrawingType == DrawingType.None) return;

            switch (DrawingType)
            {
                case DrawingType.Pen:
                    if (e.ChangedButton == OxyMouseButton.Left)
                    {
                        int iteration = (this.Model.Series.Count + 1);
                        _tempSerie = new LineDarkSeries
                        {
                            Title = "Pen " + iteration,
                            MarkerType = MarkerType.None,
                            StrokeThickness = 2,
                            RenderInLegend = false,
                            Color = OxyColors.HotPink,
                            TrackerFormatString = "(P" + iteration + "){4:0.00#}"
                        };

                        _tempSerie.Points.Add(DateTimeAxis.InverseTransform(e.Position.X, e.Position.Y, LinearAxis));
                        this.Model.Series.Add(_tempSerie);
                        this.Model.InvalidatePlot(true);
                        e.Handled = true;
                    }
                    break;

                case DrawingType.Arrow:
                case DrawingType.Line:
                case DrawingType.Average:
                    if (e.ChangedButton == OxyMouseButton.Left)
                    {
                        var point = DateTimeAxis.InverseTransform(e.Position.X, e.Position.Y, LinearAxis);
                        _tempArrowAnnotation = new Annotations.ArrowAnnotation()
                        {
                            Tag = new AnnotationTag(DrawingType.ToString()),
                            StartPoint = point,
                            EndPoint = point
                        };
         
                        if (DrawingType != DrawingType.Arrow)
                        {
                            _tempArrowAnnotation.HeadLength = 0;
                            _tempArrowAnnotation.HeadWidth = 0;
                            _tempArrowAnnotation.Color = OxyColorsDark.SciChartLegendTextOxy;
                            _tempArrowAnnotation.LineStyle = DrawingType == DrawingType.Average ? LineStyle.Dash : LineStyle.Solid;
                        }
                       
                        AddAnnotation(_tempArrowAnnotation);
                        e.Handled = true;
                    }
                    break;

                case DrawingType.VerticalLine:
                    if (e.ChangedButton == OxyMouseButton.Left)
                    {
                        CreateCurrentVerticalLineAnnotation();
                        e.Handled = true;
                    }
                    break;

                case DrawingType.Regression:
                    if (e.ChangedButton == OxyMouseButton.Left)
                    {
                        AnnotationTag tag = new AnnotationTag(DrawingType.ToString());

                        var point = DateTimeAxis.InverseTransform(e.Position.X, e.Position.Y, LinearAxis);
                        _tempArrowAnnotation = new Annotations.ArrowAnnotation()
                        {
                            HeadLength = 0,
                            HeadWidth = 0,
                            Color = OxyColorsDark.SciChartLegendTextOxy,
                            LineStyle = LineStyle.Solid,
                            StrokeThickness = 1.0,
                            StartPoint = point,
                            EndPoint = point,
                            Tag = tag
                        };
                        AddAnnotation(_tempArrowAnnotation);

                        _tempArrowAnnotation2 = new Annotations.ArrowAnnotation()
                        {
                            HeadLength = 0,
                            HeadWidth = 0,
                            Color = OxyColorsDark.SciChartCandleStickIncreasingOxy,
                            LineStyle = LineStyle.Solid,
                            StrokeThickness = 1.0,
                            StartPoint = point,
                            EndPoint = point,
                            Tag = tag
                        };
                        AddAnnotation(_tempArrowAnnotation2);

                        _tempArrowAnnotation3 = new Annotations.ArrowAnnotation()
                        {
                            HeadLength = 0,
                            HeadWidth = 0,
                            Color = OxyColorsDark.SciChartCandleStickIncreasingOxy,
                            LineStyle = LineStyle.Solid,
                            StrokeThickness = 1.0,
                            StartPoint = point,
                            EndPoint = point,
                            Tag = tag
                        };
                        AddAnnotation(_tempArrowAnnotation3);

                        _tempArrowAnnotation4 = new Annotations.ArrowAnnotation()
                        {
                            HeadLength = 0,
                            HeadWidth = 0,
                            Color = OxyColorsDark.SciChartCandleStickDecreasingOxy,
                            LineStyle = LineStyle.Solid,
                            StrokeThickness = 1.0,
                            StartPoint = point,
                            EndPoint = point,
                            Tag = tag
                        };
                        AddAnnotation(_tempArrowAnnotation4);

                        _tempArrowAnnotation5 = new Annotations.ArrowAnnotation()
                        {
                            HeadLength = 0,
                            HeadWidth = 0,
                            Color = OxyColorsDark.SciChartCandleStickDecreasingOxy,
                            LineStyle = LineStyle.Solid,
                            StrokeThickness = 1.0,
                            StartPoint = point,
                            EndPoint = point,
                            Tag = tag
                        };
                        AddAnnotation(_tempArrowAnnotation5);

                        e.Handled = true;
                    }
                    break;

                case DrawingType.PctChange:
                    if (e.ChangedButton == OxyMouseButton.Left)
                    {
                        AnnotationTag tag = new AnnotationTag(DrawingType.ToString());
                        var point = DateTimeAxis.InverseTransform(e.Position.X, e.Position.Y, LinearAxis);
                        _tempArrowAnnotation = new Annotations.ArrowAnnotation()
                        {
                            HeadLength = 0,
                            HeadWidth = 0,
                            LineStyle = LineStyle.Solid,
                            StrokeThickness = 1.0,
                            Tag = tag,
                            TextColor = OxyColorsDark.SciChartLegendTextOxy
                        };

                        _tempArrowAnnotation.StartPoint = _tempArrowAnnotation.EndPoint = point;
                        AddAnnotation(_tempArrowAnnotation);

                        _tempArrowAnnotation2 = new Annotations.ArrowAnnotation()
                        {
                            HeadLength = 0,
                            HeadWidth = 0,
                            LineStyle = LineStyle.Solid,
                            StrokeThickness = 1.0,
                            Tag = tag,
                            TextColor = OxyColorsDark.SciChartLegendTextOxy
                        };
                        _tempArrowAnnotation2.StartPoint = _tempArrowAnnotation2.EndPoint = point;
                        AddAnnotation(_tempArrowAnnotation2);

                        _tempArrowAnnotation3 = new Annotations.ArrowAnnotation()
                        {
                            LineStyle = LineStyle.Solid,
                            StrokeThickness = 1.0,
                            Tag = tag,
                            TextColor = OxyColorsDark.SciChartLegendTextOxy
                        };
                        _tempArrowAnnotation3.StartPoint = _tempArrowAnnotation3.EndPoint = point;
                        AddAnnotation(_tempArrowAnnotation3);
                        e.Handled = true;
                    }
                    break;

                case DrawingType.Text:
                    if (e.ChangedButton == OxyMouseButton.Left)
                    {
                        _tempTextAnnotation = new Annotations.TextAnnotation
                        {
                            TextPosition = DateTimeAxis.InverseTransform(e.Position.X, e.Position.Y, LinearAxis),
                            Text = "Text annotation",
                            Background = OxyColorsDark.SciChartLegendTextOxy,
                            TextColor = OxyColors.Black,
                            Tag = new AnnotationTag("Text")
                        };
                        AddAnnotation(_tempTextAnnotation);
                        e.Handled = true;
                    }
                    break;

                case DrawingType.Rectangle:
                    if (e.ChangedButton == OxyMouseButton.Left)
                    {
                        var startPoint = DateTimeAxis.InverseTransform(e.Position.X, e.Position.Y, LinearAxis);
                        _tempRectangleAnnotation = new Annotations.RectangleAnnotation
                        {
                            MinimumX = startPoint.X,
                            MaximumX = startPoint.X,
                            MinimumY = startPoint.Y,
                            MaximumY = startPoint.Y,
                            TextHorizontalAlignment = OxyPlot.HorizontalAlignment.Left,
                            TextVerticalAlignment = OxyPlot.VerticalAlignment.Bottom,
                            Fill = OxyColors.Transparent,
                            Stroke = OxyColors.Blue,
                            StrokeThickness = 1.0,
                            Tag = new AnnotationTag("Rectangle")
                        };

                        AddAnnotation(_tempRectangleAnnotation);
                        e.Handled = true;
                    }
                    break;

                case DrawingType.Path:
                    break;
                default:
                    break;
            }
        }

        private IEnumerable<DataPoint> GetPoints(OxyPlot.Series.Series serie, double firstX, double secondX)
        {
            double startX = firstX;
            double endX = secondX;

            if (firstX > secondX)
            {
                startX = secondX;
                endX = firstX;
            }

            if (serie is LineDarkSeries)
            {
                return ((LineDarkSeries)serie).PointsDark
                    .Where(x => x.X >= startX)
                    .Where(x => x.X <= endX);
            }
            else if (serie is LinearBarDarkSeries)
            {
                return ((LinearBarDarkSeries)serie).PointsDark
                    .Where(x => x.X >= startX)
                    .Where(x => x.X <= endX);
            }
            else if (serie is TwoColorAreaDarkSeries)
            {
                return ((TwoColorAreaDarkSeries)serie).PointsDark
                    .Where(x => x.X >= startX)
                    .Where(x => x.X <= endX);
            }
            else if (serie is CandleStickSeries)
            {
                return ((CandleStickSeries)serie).Items
                    .Where(x => x.X >= startX)
                    .Where(x => x.X <= endX).Select(p => new DataPoint(p.X, p.Close));
            }

            throw new ArgumentException("DarkPLotView.GetPoints(): Serie type not recognise (" + serie.GetType() + ").");
        }

        private void PlotModel_MouseMove(object sender, OxyMouseEventArgs e)
        {
            if (DrawingType == DrawingType.None) return;

            switch (DrawingType)
            {
                case DrawingType.Pen:
                    if (_tempSerie != null)
                    {
                        _tempSerie.Points.Add(DateTimeAxis.InverseTransform(e.Position.X, e.Position.Y, LinearAxis));
                        this.Model.InvalidatePlot(false);
                        e.Handled = true;
                    }
                    break;

                case DrawingType.Arrow:
                case DrawingType.Line:
                    if (_tempArrowAnnotation != null)
                    {
                        // Modify the end point
                        _tempArrowAnnotation.EndPoint = DateTimeAxis.InverseTransform(e.Position.X, e.Position.Y, LinearAxis);
                        _tempArrowAnnotation.Text = string.Format("Y = {0:0.###}", _tempArrowAnnotation.EndPoint.Y);

                        // Redraw the plot
                        this.Model.InvalidatePlot(false);
                        e.Handled = true;
                    }
                    break;

                case DrawingType.VerticalLine:
                    if (_tempVerticalLineAnnotation != null)
                    {
                        _tempVerticalLineAnnotation.X = DateTimeAxis.InverseTransform(e.Position.X, e.Position.Y, LinearAxis).X;
                        _tempVerticalLineAnnotation.Text = Axes.DateTimeAxis.ToDateTime(_tempVerticalLineAnnotation.X).ToString(trackerFormatStringHorizontal);
                        this.Model.InvalidatePlot(false);
                        e.Handled = true;
                    }
                    break;

                case DrawingType.Rectangle:
                    if (_tempRectangleAnnotation != null)
                    {
                        var currentPoint = DateTimeAxis.InverseTransform(e.Position.X, e.Position.Y, LinearAxis);
                        _tempRectangleAnnotation.MaximumX = currentPoint.X;
                        _tempRectangleAnnotation.MaximumY = currentPoint.Y;

                        this.Model.InvalidatePlot(false);
                        e.Handled = true;
                    }
                    break;

                case DrawingType.Average:
                    if (_tempArrowAnnotation != null)
                    {
                        var startX = _tempArrowAnnotation.StartPoint.X;
                        var currentX = DateTimeAxis.InverseTransform(e.Position.X);

                        var points = GetPoints(this.Model.Series.FirstOrDefault(), startX, currentX);
                        var Ys = points.Select(x => x.Y);

                        if (Ys.Count() > 2)
                        {
                            // Computation of Average 
                            double avg = Ys.Average();
                            _tempArrowAnnotation.StartPoint = new DataPoint(startX, avg);
                            _tempArrowAnnotation.EndPoint = new DataPoint(currentX, avg);
                            _tempArrowAnnotation.Text = avg.ToString("0.00");
                        }

                        // Redraw the plot
                        this.Model.InvalidatePlot(false);
                        e.Handled = true;
                    }
                    break;

                case DrawingType.Regression:
                    if (_tempArrowAnnotation != null)
                    {
                        var startX = _tempArrowAnnotation.StartPoint.X;
                        var currentX = DateTimeAxis.InverseTransform(e.Position.X);

                        var points = GetPoints(this.Model.Series.FirstOrDefault(), startX, currentX);
                        var Ys = points.Select(x => x.Y);
                        var yCount = Ys.Count();
                        if (yCount > 2)
                        {
                            double rSquare;
                            double yIntercept;
                            double slope;
                            double variance;
    
                            Maths.LinearRegression(points.Select(x => x.X).ToArray(), Ys.ToArray(), 0, yCount,
                                out rSquare, out variance, out yIntercept, out slope);

                            double stdDev = Math.Sqrt(variance);
                            double stdDev2 = 2 * stdDev; 
                            double startY = yIntercept + slope * startX;
                            double endY = yIntercept + slope * currentX;

                            _tempArrowAnnotation.StartPoint = new DataPoint(startX, startY);
                            _tempArrowAnnotation.EndPoint = new DataPoint(currentX, endY);

                            _tempArrowAnnotation2.StartPoint = new DataPoint(startX, startY + stdDev);
                            _tempArrowAnnotation2.EndPoint = new DataPoint(currentX, endY + stdDev);

                            _tempArrowAnnotation3.StartPoint = new DataPoint(startX, startY - stdDev);
                            _tempArrowAnnotation3.EndPoint = new DataPoint(currentX, endY - stdDev);

                            _tempArrowAnnotation4.StartPoint = new DataPoint(startX, startY + stdDev2);
                            _tempArrowAnnotation4.EndPoint = new DataPoint(currentX, endY + stdDev2);

                            _tempArrowAnnotation5.StartPoint = new DataPoint(startX, startY - stdDev2);
                            _tempArrowAnnotation5.EndPoint = new DataPoint(currentX, endY - stdDev2);

                            //_tempArrowAnnotation.Text = rSquare.ToString("0.00%");
                        }

                        // Redraw the plot
                        this.Model.InvalidatePlot(false);
                        e.Handled = true;
                    }
                    break;

                case DrawingType.PctChange:
                    if (_tempArrowAnnotation != null && _tempArrowAnnotation2 != null && _tempArrowAnnotation3 != null)
                    {
                        // Modify the end point
                        var endPoint = DateTimeAxis.InverseTransform(e.Position.X, e.Position.Y, LinearAxis);

                        _tempArrowAnnotation.EndPoint = new DataPoint(endPoint.X, _tempArrowAnnotation.StartPoint.Y);

                        _tempArrowAnnotation2.StartPoint = new DataPoint(_tempArrowAnnotation.StartPoint.X, endPoint.Y);
                        _tempArrowAnnotation2.EndPoint = endPoint;
                        double pctReturn = double.NaN;

                        if (_tempArrowAnnotation.EndPoint.Y != 0) pctReturn = _tempArrowAnnotation2.EndPoint.Y / _tempArrowAnnotation.EndPoint.Y - 1.0;
                        _tempArrowAnnotation2.Text = string.Format("{0:+0.###%;-0.###%;0%}   {1:+0.###;-0.###;0}",
                            pctReturn,
                            _tempArrowAnnotation2.EndPoint.Y - _tempArrowAnnotation.EndPoint.Y);

                        double midPoint = (_tempArrowAnnotation.EndPoint.X + _tempArrowAnnotation.StartPoint.X) / 2.0;
                        double quarterPoint = (_tempArrowAnnotation.EndPoint.X + 3.0 * _tempArrowAnnotation.StartPoint.X) / 4.0;

                        _tempArrowAnnotation2.TextPosition = new DataPoint(quarterPoint, _tempArrowAnnotation2.StartPoint.Y + 0.40);
                        _tempArrowAnnotation3.StartPoint = new DataPoint(midPoint, _tempArrowAnnotation.StartPoint.Y);
                        _tempArrowAnnotation3.EndPoint = new DataPoint(midPoint, endPoint.Y);

                        // Redraw the plot
                        this.Model.InvalidatePlot(false);
                        e.Handled = true;
                    }
                    break;

                default:
                    break;
            }
        }

        private void PlotModel_MouseUp(object sender, OxyMouseEventArgs e)
        {
            if (DrawingType == DrawingType.None) return;

            switch (DrawingType)
            {
                case DrawingType.Pen:
                    if (_tempSerie != null)
                    {
                        _tempSerie = null;
                        e.Handled = true;
                        uncheckAllAnnotations();
                    }
                    break;

                case DrawingType.Arrow:
                case DrawingType.Line:
                    if (_tempArrowAnnotation != null)
                    {
                        _tempArrowAnnotation.Text = "";
                        _tempArrowAnnotation = null;
                        e.Handled = true;
                        uncheckAllAnnotations();
                    }
                    break;

                case DrawingType.Average:
                    if (_tempArrowAnnotation != null)
                    {
                        _tempArrowAnnotation = null;
                        e.Handled = true;
                        uncheckAllAnnotations();
                    }
                    break;

                case DrawingType.VerticalLine:
                    if (_tempVerticalLineAnnotation != null)
                    {
                        _tempVerticalLineAnnotation = null;
                        e.Handled = true;
                        uncheckAllAnnotations();
                    }
                    break;

                case DrawingType.Regression:
                    if (_tempArrowAnnotation != null && _tempArrowAnnotation2 != null && _tempArrowAnnotation3 != null
                        && _tempArrowAnnotation4 != null && _tempArrowAnnotation5 != null)
                    {
                        _tempArrowAnnotation = null;
                        _tempArrowAnnotation2 = null;
                        _tempArrowAnnotation3 = null;
                        _tempArrowAnnotation4 = null;
                        _tempArrowAnnotation5 = null;
                        e.Handled = true;
                        uncheckAllAnnotations();
                    }
                    break;

                case DrawingType.PctChange:
                    if (_tempArrowAnnotation != null && _tempArrowAnnotation2 != null && _tempArrowAnnotation3 != null)
                    {
                        _tempArrowAnnotation = null;
                        _tempArrowAnnotation2 = null;
                        _tempArrowAnnotation3 = null;
                        e.Handled = true;
                        uncheckAllAnnotations();
                    }
                    break;

                case DrawingType.Rectangle:
                    if (_tempRectangleAnnotation != null)
                    {
                        _tempRectangleAnnotation = null;
                        e.Handled = true;
                        uncheckAllAnnotations();
                    }
                    break;

                case DrawingType.Text:
                    uncheckAllAnnotations();
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Adjusts the Y extent.
        /// </summary>
        /// <param name="series">Series.</param>
        /// <param name="xaxis">Xaxis.</param>
        /// <param name="yaxis">Yaxis.</param>
        private void AdjustYExtent(OxyPlot.Series.Series series, Axes.DateTimeAxis xaxis, Axes.LinearAxis yaxis)
        {
            if (!AdjustY) return;

            if (series is CandleStickSeries)
            {
                CandleStickSeries candleStickSeries = (CandleStickSeries)series;
                if (candleStickSeries.Items.Count == 0) return;

                var xmin = xaxis.ActualMinimum;
                var xmax = xaxis.ActualMaximum;

                var istart = candleStickSeries.FindByX(xmin);
                var iend = candleStickSeries.FindByX(xmax, istart);

                var ymin = double.MaxValue;
                var ymax = double.MinValue;
                for (int i = istart; i <= iend; i++)
                {
                    var bar = candleStickSeries.Items[i];
                    ymin = Math.Min(ymin, bar.Low);
                    ymax = Math.Max(ymax, bar.High);
                }

                var extent = ymax - ymin;
                var margin = extent * 0.010;

                yaxis.Zoom(ymin - margin, ymax + margin);
            }
            else if (series is LineDarkSeries)
            {
                LineDarkSeries lineDarkSeries = (LineDarkSeries)series;
                if (lineDarkSeries.PointsDark.Count == 0) return;

                var xmin = xaxis.ActualMinimum;
                var xmax = xaxis.ActualMaximum;

                var istart = lineDarkSeries.FindByX(xmin);
                var iend = lineDarkSeries.FindByX(xmax, istart);

                var ymin = double.MaxValue;
                var ymax = double.MinValue;
                for (int i = istart; i <= iend; i++)
                {
                    var point = lineDarkSeries.PointsDark[i];
                    ymin = Math.Min(ymin, point.Y);
                    ymax = Math.Max(ymax, point.Y);
                }

                var extent = ymax - ymin;
                var margin = extent * 0.010;

                yaxis.Zoom(ymin - margin, ymax + margin);
            }
        }
    }
}
