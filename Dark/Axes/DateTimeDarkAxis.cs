// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DateTimeAxis.cs" company="OxyPlot">
//   Copyright (c) 2014 OxyPlot contributors
// </copyright>
// <summary>
//   Represents an axis presenting <see cref="System.DateTime" /> values.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using OxyPlot.Axes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;

namespace OxyPlot.Dark.Wpf
{
    /// <summary>
    /// 
    /// </summary>
    public class DateTimeDarkAxis : Axes.DateTimeAxis
    {
        /// <summary>
        /// 
        /// </summary>
        public const string SecondaryDateTimeAxisKey = "SecondaryDateTimeAxis";

        /// <summary>
        /// 
        /// </summary>
        public DateTimeDarkAxis() : base()
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ppt"></param>
        /// <param name="cpt"></param>
        public override void Pan(ScreenPoint ppt, ScreenPoint cpt)
        {
            base.Pan(ppt, cpt);
            if (this.Key != SecondaryDateTimeAxisKey && PlotModel.Axes.Any(x => x.Key == SecondaryDateTimeAxisKey))
            {
                PlotModel.Axes.Where(x => x.Key == SecondaryDateTimeAxisKey).First().Pan(ppt, cpt);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="factor"></param>
        public override void ZoomAtCenter(double factor)
        {
            base.ZoomAtCenter(factor);
            if (this.Key != SecondaryDateTimeAxisKey && PlotModel.Axes.Any(k => k.Key == SecondaryDateTimeAxisKey))
            {
                PlotModel.Axes.Where(k => k.Key == SecondaryDateTimeAxisKey).First().ZoomAtCenter(factor);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="factor"></param>
        /// <param name="x"></param>
        public override void ZoomAt(double factor, double x)
        {
            base.ZoomAt(factor, x);
            if (this.Key != SecondaryDateTimeAxisKey && PlotModel.Axes.Any(k => k.Key == SecondaryDateTimeAxisKey))
            {
                PlotModel.Axes.Where(k => k.Key == SecondaryDateTimeAxisKey).First().ZoomAt(factor, x);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x0"></param>
        /// <param name="x1"></param>
        public override void Zoom(double x0, double x1)
        {
            base.Zoom(x0, x1);
            if (this.Key != SecondaryDateTimeAxisKey && PlotModel.Axes.Any(x => x.Key == SecondaryDateTimeAxisKey))
            {
                PlotModel.Axes.Where(x => x.Key == SecondaryDateTimeAxisKey).First().Zoom(x0, x1);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            if (this.Key != SecondaryDateTimeAxisKey && PlotModel.Axes.Any(x => x.Key == SecondaryDateTimeAxisKey))
            {
                PlotModel.Axes.Where(x => x.Key == SecondaryDateTimeAxisKey).First().Reset();
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="majorLabelValues"></param>
        /// <param name="majorTickValues"></param>
        /// <param name="minorTickValues"></param>
        public override void GetTickValues(out IList<double> majorLabelValues, out IList<double> majorTickValues, out IList<double> minorTickValues)
        {
            if (Key == SecondaryDateTimeAxisKey)
            {
                var ts = ToDateTime(this.ActualMaximum) - ToDateTime(this.ActualMinimum);
                var dateTimeIntervalType = DateTimeIntervalType.Auto;
                switch (this.ActualStringFormat)
                {
                    case "yyyy":
                        dateTimeIntervalType = DateTimeIntervalType.Years;
                        break;

                    case "yyyy/ww":
                        dateTimeIntervalType = DateTimeIntervalType.Weeks;
                        break;

                    case "yyyy-MM-dd":

                        if (ts.Days > 365)
                        {
                            dateTimeIntervalType = DateTimeIntervalType.Months;
                        }
                        else if (ts.Days >= 14)
                        {
                            dateTimeIntervalType = DateTimeIntervalType.Weeks;
                        }
                        else
                        {
                            dateTimeIntervalType = DateTimeIntervalType.Days;

                        }
                        break;

                    case "HH:mm":
                        dateTimeIntervalType = DateTimeIntervalType.Hours;

                        break;

                    case "HH:mm:ss":
                        dateTimeIntervalType = DateTimeIntervalType.Seconds;

                        break;

                    case "HH:mm:ss.fff":
                        dateTimeIntervalType = DateTimeIntervalType.Milliseconds;

                        break;
                }

                var secondaryDateTimeIntervalType = DateTimeIntervalType.Auto;

                if (Key == SecondaryDateTimeAxisKey)
                {
                    switch (dateTimeIntervalType)
                    {
                        case DateTimeIntervalType.Days:
                            secondaryDateTimeIntervalType = DateTimeIntervalType.Days;
                            break;

                        case DateTimeIntervalType.Hours:
                            secondaryDateTimeIntervalType = DateTimeIntervalType.Hours;

                            break;

                        case DateTimeIntervalType.Milliseconds:
                            secondaryDateTimeIntervalType = DateTimeIntervalType.Milliseconds;
                            break;

                        case DateTimeIntervalType.Minutes:
                            secondaryDateTimeIntervalType = DateTimeIntervalType.Minutes;
                            break;

                        case DateTimeIntervalType.Weeks:
                            secondaryDateTimeIntervalType = DateTimeIntervalType.Weeks;
                            break;

                        case DateTimeIntervalType.Months:
                            secondaryDateTimeIntervalType = DateTimeIntervalType.Years;
                            break;

                        case DateTimeIntervalType.Seconds:
                            secondaryDateTimeIntervalType = DateTimeIntervalType.Seconds;
                            break;

                        case DateTimeIntervalType.Years:
                            secondaryDateTimeIntervalType = DateTimeIntervalType.Years;
                            break;

                        case DateTimeIntervalType.Auto:
                        case DateTimeIntervalType.Manual:
                        default:
                            secondaryDateTimeIntervalType = DateTimeIntervalType.Auto;
                            break;
                    }
                }

                minorTickValues = new List<double>(); //this.CreateDateTimeTickValues(
                                                      // this.ActualMinimum, this.ActualMaximum, this.ActualMinorStep, secondaryDateTimeIntervalType);
                majorTickValues = this.CreateDateTimeTickValues(this.ActualMinimum, this.ActualMaximum, this.ActualMajorStep, secondaryDateTimeIntervalType);

                if (secondaryDateTimeIntervalType == DateTimeIntervalType.Hours)
                {
                    // 2nd Axis displays date
                    majorLabelValues = majorTickValues.Select(x => ToDouble(ToDateTime(x).Date.AddHours(12))).Distinct().ToList();
                    majorTickValues = majorTickValues.Select(x => ToDouble(ToDateTime(x).Date)).Distinct().ToList();
                }
                else if (secondaryDateTimeIntervalType == DateTimeIntervalType.Days)
                {
                    // 2nd Axis displays date
                    majorLabelValues = majorTickValues.Select(x => ToDouble(ToDateTime(x).Date.AddHours(12))).Distinct().ToList();
                    majorTickValues = majorTickValues.Select(x => ToDouble(ToDateTime(x).Date)).Concat(majorTickValues.Select(x => x + 1)).Distinct().ToList(); // add all days and remove duplicates
                }
                else if (secondaryDateTimeIntervalType == DateTimeIntervalType.Weeks)
                {
                    var dates = majorTickValues.Select(x => ToDateTime(x).Date);
                    var grouped = dates.GroupBy(x => x.Month).Select(x => x.First()).ToList();
                    grouped.ForEach(x => x = x.AddDays(-x.Day + 1));

                    for (int d = 0; d < grouped.Count; d++)
                    {
                        grouped[d] = new DateTime(grouped[d].Year, grouped[d].Month, 1);
                    }
                    majorTickValues = grouped.Select(x => ToDouble(x.AddDays(-1))).ToList();
                    majorLabelValues = grouped.Select(x => ToDouble(x.AddDays(14))).ToList();

                }
                else if (secondaryDateTimeIntervalType == DateTimeIntervalType.Months)
                {
                    majorLabelValues = majorTickValues.Select(x => ToDouble(ToDateTime(x).Date.AddMonths(6))).Distinct().ToList(); // not correct
                }
                else if (secondaryDateTimeIntervalType == DateTimeIntervalType.Years)
                {
                    majorLabelValues = majorTickValues.Select(x => ToDouble(ToDateTime(x).Date.AddMonths(6))).Distinct().ToList(); // not correct

                    var year = ToDateTime(majorLabelValues.Min());
                    var min = ToDateTime(this.ActualMinimum);
                    if (min.Year < year.Year)
                    {
                        var midYear = new DateTime(min.Year, 06, 14);
                        if (min < midYear)
                        {
                            majorLabelValues.Add(ToDouble(midYear));
                        }
                        else
                        {
                            //majorLabelValues.Add(this.ActualMinimum);
                        }
                    }
                }
                else
                {
                    majorLabelValues = majorTickValues;
                }
            }
            else
            {
                base.GetTickValues(out majorLabelValues, out majorTickValues, out minorTickValues);
            }
        }

        /// <summary>
        /// Creates date/time tick values.
        /// </summary>
        /// <param name="min">
        /// The min.
        /// </param>
        /// <param name="max">
        /// The max.
        /// </param>
        /// <param name="interval">
        /// The interval.
        /// </param>
        /// <param name="intervalType">
        /// The interval type.
        /// </param>
        /// DateTime tick values.
        /// <returns>
        /// DateTime tick values.
        /// </returns>
        private IList<double> CreateDateTimeTickValues(
            double min, double max, double interval, DateTimeIntervalType intervalType)
        {
            // If the step size is more than 7 days (e.g. months or years) we use a specialized tick generation method that adds tick values with uneven spacing...
            if (intervalType > DateTimeIntervalType.Days)
            {
                return this.CreateDateTickValues(min, max, interval, intervalType);
            }

            // For shorter step sizes we use the method from Axis
            return CreateTickValues(min, max, interval);
        }

        /// <summary>
        /// Creates the date tick values.
        /// </summary>
        /// <param name="min">
        /// The min.
        /// </param>
        /// <param name="max">
        /// The max.
        /// </param>
        /// <param name="step">
        /// The step.
        /// </param>
        /// <param name="intervalType">
        /// Type of the interval.
        /// </param>
        /// <returns>
        /// Date tick values.
        /// </returns>
        private IList<double> CreateDateTickValues(
            double min, double max, double step, DateTimeIntervalType intervalType)
        {
            // to do: edit here to center
            DateTime start = ToDateTime(min);
            switch (intervalType)
            {
                case DateTimeIntervalType.Weeks:

                    // make sure the first tick is at the 1st day of a week
                    start = start.AddDays(-(int)start.DayOfWeek + (int)this.FirstDayOfWeek);
                    break;
                case DateTimeIntervalType.Months:

                    // make sure the first tick is at the 1st of a month
                    start = new DateTime(start.Year, start.Month, 1);
                    break;
                case DateTimeIntervalType.Years:

                    // make sure the first tick is at Jan 1st
                    start = new DateTime(start.Year, 1, 1);
                    break;
            }

            // Adds a tick to the end time to make sure the end DateTime is included.
            DateTime end = ToDateTime(max).AddTicks(1);

            DateTime current = start;
            var values = new Collection<double>();
            double eps = step * 1e-3;
            DateTime minDateTime = ToDateTime(min - eps);
            DateTime maxDateTime = ToDateTime(max + eps);
            while (current < end)
            {
                if (current > minDateTime && current < maxDateTime)
                {
                    values.Add(ToDouble(current));
                }

                switch (intervalType)
                {
                    case DateTimeIntervalType.Months:
                        current = current.AddMonths((int)Math.Ceiling(step));
                        break;
                    case DateTimeIntervalType.Years:
                        current = current.AddYears((int)Math.Ceiling(step));
                        break;
                    default:
                        current = current.AddDays(step);
                        break;
                }
            }

            return values;
        }

        private DateTimeIntervalType SecondaryDateTimeIntervalType;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        protected override string FormatValueOverride(double x)
        {
            var ts = ToDateTime(this.ActualMaximum) - ToDateTime(this.ActualMinimum);
            DateTimeIntervalType dateTimeIntervalType = DateTimeIntervalType.Auto;
            DateTimeIntervalType minorIntervalType = DateTimeIntervalType.Auto;
            string fmt = this.ActualStringFormat;

            var inter = this.Key;
            switch (this.ActualStringFormat)
            {
                case "yyyy":
                    dateTimeIntervalType = DateTimeIntervalType.Years;
                    minorIntervalType = DateTimeIntervalType.Months;
                    break;

                //case "yyyy-MM-dd":
                //    dateTimeIntervalType = DateTimeIntervalType.Months;
                //    break;

                case "yyyy/ww":
                    dateTimeIntervalType = DateTimeIntervalType.Weeks;
                    minorIntervalType = DateTimeIntervalType.Days;
                    break;

                case "yyyy-MM-dd":

                    if (ts.Days > 365)
                    {
                        dateTimeIntervalType = DateTimeIntervalType.Months;
                        minorIntervalType = DateTimeIntervalType.Weeks;
                    }
                    else if (ts.Days >= 14)
                    {
                        dateTimeIntervalType = DateTimeIntervalType.Weeks;
                    }
                    else
                    {
                        dateTimeIntervalType = DateTimeIntervalType.Days;
                        minorIntervalType = DateTimeIntervalType.Hours;
                    }
                    break;

                case "HH:mm":
                    dateTimeIntervalType = DateTimeIntervalType.Hours;
                    minorIntervalType = dateTimeIntervalType;
                    break;

                case "HH:mm:ss":
                    dateTimeIntervalType = DateTimeIntervalType.Seconds;
                    minorIntervalType = dateTimeIntervalType;
                    break;

                case "HH:mm:ss.fff":
                    dateTimeIntervalType = DateTimeIntervalType.Milliseconds;
                    minorIntervalType = dateTimeIntervalType;
                    break;
            }


            if (Key == SecondaryDateTimeAxisKey)
            {
                switch (dateTimeIntervalType)
                {
                    case DateTimeIntervalType.Days:
                    //SecondaryDateTimeIntervalType = DateTimeIntervalType.Days;
                    //break;


                    case DateTimeIntervalType.Hours:
                        SecondaryDateTimeIntervalType = DateTimeIntervalType.Hours;
                        fmt = "dd MMM yyyy";
                        break;

                    case DateTimeIntervalType.Milliseconds:
                        SecondaryDateTimeIntervalType = DateTimeIntervalType.Milliseconds;
                        break;

                    case DateTimeIntervalType.Minutes:
                        SecondaryDateTimeIntervalType = DateTimeIntervalType.Minutes;
                        break;

                    case DateTimeIntervalType.Weeks:
                        fmt = "MMM yyyy";
                        SecondaryDateTimeIntervalType = DateTimeIntervalType.Months;
                        break;

                    case DateTimeIntervalType.Months:
                        fmt = "yyyy";
                        SecondaryDateTimeIntervalType = DateTimeIntervalType.Years;
                        break;

                    case DateTimeIntervalType.Seconds:
                        SecondaryDateTimeIntervalType = DateTimeIntervalType.Seconds;
                        break;

                    case DateTimeIntervalType.Years:
                        SecondaryDateTimeIntervalType = DateTimeIntervalType.Years;
                        break;

                    case DateTimeIntervalType.Auto:
                    case DateTimeIntervalType.Manual:
                    default:
                        SecondaryDateTimeIntervalType = DateTimeIntervalType.Auto;
                        break;
                }

                switch (SecondaryDateTimeIntervalType)
                {
                    default:
                        break;
                }
            }
            else
            {
                switch (dateTimeIntervalType)
                {
                    case DateTimeIntervalType.Days:
                        fmt = "HH:mm";
                        break;

                    case DateTimeIntervalType.Hours:
                        break;

                    case DateTimeIntervalType.Milliseconds:
                    case DateTimeIntervalType.Minutes:
                        break;

                    case DateTimeIntervalType.Weeks:
                    case DateTimeIntervalType.Months:
                    case DateTimeIntervalType.Years:
                        fmt = "MMM dd";
                        break;

                    case DateTimeIntervalType.Seconds:
                    case DateTimeIntervalType.Auto:
                    case DateTimeIntervalType.Manual:
                        break;
                }
            }

            // convert the double value to a DateTime
            var time = ToDateTime(x);

            // If a time zone is specified, convert the time
            if (this.TimeZone != null)
            {
                time = TimeZoneInfo.ConvertTime(time, this.TimeZone);
            }

            if (fmt == null)
            {
                return time.ToString(CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern);
            }

            int week = this.GetWeek(time);
            fmt = fmt.Replace("ww", week.ToString("00"));
            fmt = fmt.Replace("w", week.ToString(CultureInfo.InvariantCulture));
            fmt = string.Concat("{0:", fmt, "}");
            return string.Format(this.ActualCulture, fmt, time);

            //string test = base.FormatValueOverride(x); 
            //return base.FormatValueOverride(x);
        }

        /// <summary>
        /// Gets the week number for the specified date.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns>The week number for the current culture.</returns>
        private int GetWeek(DateTime date)
        {
            return this.ActualCulture.Calendar.GetWeekOfYear(date, this.CalendarWeekRule, this.FirstDayOfWeek);
        }
    }
}
