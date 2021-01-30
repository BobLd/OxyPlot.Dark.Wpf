using System.Collections.Generic;
using System.Linq;

namespace OxyPlot.Dark.Wpf
{
    /// <summary>
    /// 
    /// </summary>
    public class TwoColorAreaDarkSeries : OxyPlot.Series.TwoColorAreaSeries, IOptimisedSeries
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
        /// 
        /// </summary>
        public TwoColorAreaDarkSeries() : base()
        {
            Color = OxyColorsDark.SciChartCandleStickIncreasingOxy;
            Color2 = OxyColorsDark.SciChartCandleStickDecreasingOxy;
            TrackerFormatString = "{4:0.00}";
            Optimise = true;
            OptimisationFactor = 1.0;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="points"></param>
        /// <returns></returns>
        protected override List<ScreenPoint> RenderScreenPoints(AreaRenderContext context, List<ScreenPoint> points)
        {
            var screenPoints = points;
            if (Optimise) screenPoints = PointsReduction.DouglasPeuckerReduction(points, OptimisationFactor);
            return base.RenderScreenPoints(context, screenPoints);
        }
    }
}
