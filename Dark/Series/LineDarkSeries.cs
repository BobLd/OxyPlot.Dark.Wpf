using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OxyPlot.Dark.Wpf
{
    /// <summary>
    /// 
    /// </summary>
    public class LineDarkSeries : OxyPlot.Series.LineSeries, IOptimisedSeries
    {
        /// <summary>
        /// Gets the x-axis.
        /// </summary>
        /// <value> The x-axis. </value>
        public Axes.Axis XAxis2 { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public bool Optimise { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public double OptimisationFactor { get; set; }

        /// <summary>
        /// Makes ActualPoints accessible to external.
        /// </summary>
        public List<DataPoint> PointsDark
        {
            get
            {
                return this.ActualPoints;
            }
        }

        /// <summary>
        /// Fast index of bar where max(bar[i].X) &lt;= x 
        /// </summary>
        /// <returns>The index of the bar closest to X, where max(bar[i].X) &lt;= x.</returns>
        /// <param name="x">The x coordinate.</param>
        /// <param name="startIndex">starting index</param> 
        public int FindByX(double x, int startIndex = -1)
        {
            if (startIndex < 0)
            {
                startIndex = this.WindowStartIndex;
            }

            return this.FindWindowStartIndex(this.ActualPoints, item => item.X, x, startIndex);
        }

        bool _tracking;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tracking"></param>
        public void SetCanTrack(bool tracking)
        {
            _tracking = tracking;
        }

        /// <summary>
        /// Faster implementation
        /// </summary>
        /// <param name="point"></param>
        /// <param name="interpolate"></param>
        /// <returns></returns>
        public override TrackerHitResult GetNearestPoint(ScreenPoint point, bool interpolate)
        {
            if (!_tracking) return null;

            if (Optimise && screenPoints != null)
            {
                var spn = default(ScreenPoint);
                double index = -1;

                double minimumDistance = double.MaxValue;
                int i = 0;

                for (int p = 0; p < screenPoints.Count(); p++)
                {
                    var sp = screenPoints[i];
                    double d2 = (sp - point).LengthSquared;

                    if (d2 < minimumDistance)
                    {
                        spn = sp;
                        minimumDistance = d2;
                        index = i;
                    }

                    i++;
                }

                var dpn = this.XAxis.InverseTransform(spn.X, spn.Y, this.YAxis);
                if (minimumDistance < double.MaxValue)
                {
                    var item = this.GetItem((int)Math.Round(index));
                    return new TrackerHitResult
                    {
                        Series = this,
                        DataPoint = dpn,
                        Position = spn,
                        Item = item,
                        Index = index,
                        Text = StringHelper.Format(
                            this.ActualCulture,
                            this.TrackerFormatString,
                            item,
                            this.Title,
                            this.XAxis.Title ?? DefaultXAxisTitle,
                            this.XAxis.GetValue(dpn.X),
                            this.YAxis.Title ?? DefaultYAxisTitle,
                            this.YAxis.GetValue(dpn.Y))
                    };
                }
                return null;
            }

            return base.GetNearestPoint(point, interpolate);
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void EnsureAxes()
        {
            base.EnsureAxes();
            if (PlotModel.Axes.Any(x => x.Key == DateTimeDarkAxis.SecondaryDateTimeAxisKey))
            {
                this.XAxis2 = PlotModel.Axes.Where(x => x.Key == DateTimeDarkAxis.SecondaryDateTimeAxisKey).First();
                this.XAxis2.IntervalLength = this.XAxis.IntervalLength;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void UpdateAxisMaxMin()
        {
            base.UpdateAxisMaxMin();
            this.XAxis2.Include(this.MinX);
            this.XAxis2.Include(this.MaxX);
            this.XAxis2.IntervalLength = this.XAxis.IntervalLength;
        }

        /// <summary>
        /// 
        /// </summary>
        public LineDarkSeries() : base()
        {
            Color = OxyColors.White;
            TrackerFormatString = "{4:0.00}";
            Optimise = true;
            OptimisationFactor = 1.0;
            _tracking = true;
        }

        private ScreenPoint[] screenPoints;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rc"></param>
        /// <param name="clippingRect"></param>
        /// <param name="pointsToRender"></param>
        protected override void RenderLine(IRenderContext rc, OxyRect clippingRect, IList<ScreenPoint> pointsToRender)
        {
            if (_tracking || screenPoints == null)
            {
                screenPoints = pointsToRender.ToArray();
                if (Optimise) screenPoints = PointsReduction.DouglasPeuckerReduction(screenPoints, OptimisationFactor);
            }
            //this.Title = screenPoints.Count().ToString() + "/" + pointsToRender.Count.ToString();
            base.RenderLine(rc, clippingRect, screenPoints);
        }
    }
}
