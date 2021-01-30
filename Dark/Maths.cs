using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace OxyPlot.Dark.Wpf.Dark
{
    /// <summary>
    /// 
    /// </summary>
    public static class Maths
    {
        /// <summary>
        /// https://stackoverflow.com/questions/15623129/simple-linear-regression-for-data-set
        /// </summary>
        /// <param name="xVals"></param>
        /// <param name="yVals"></param>
        /// <param name="inclusiveStart"></param>
        /// <param name="exclusiveEnd"></param>
        /// <param name="rsquared"></param>
        /// <param name="variance"></param>
        /// <param name="yintercept"></param>
        /// <param name="slope"></param>
        public static void LinearRegression(double[] xVals, double[] yVals, int inclusiveStart, int exclusiveEnd,
                                        out double rsquared, out double variance, out double yintercept, out double slope)
        {
            double sumOfX = 0;
            double sumOfY = 0;
            double sumOfXSq = 0;
            double sumOfYSq = 0;
            double ssX = 0;
            double ssY = 0;
            double sumCodeviates = 0;
            double sCo = 0;
            double count = exclusiveEnd - inclusiveStart;

            unsafe
            {
                fixed (double* xvalues = xVals)
                {
                    fixed (double* yvalues = yVals)
                    {
                        for (int ctr = inclusiveStart; ctr < exclusiveEnd; ctr++)
                        {
                            double x = *(xvalues + ctr);
                            double y = *(yvalues + ctr);
                            sumCodeviates += x * y;
                            sumOfX += x;
                            sumOfY += y;
                            sumOfXSq += x * x;
                            sumOfYSq += y * y;
                        }
                    }
                }
            }

            double sumOfXSqr = sumOfX * sumOfX;
            double sumOfYSqr = sumOfY * sumOfY;
            double sumOfXY = sumOfX * sumOfY;

            ssX = sumOfXSq - (sumOfXSqr / count);
            ssY = sumOfYSq - (sumOfYSqr / count);
            double RNumerator = (count * sumCodeviates) - (sumOfXY);
            double RDenom = (count * sumOfXSq - sumOfXSqr) * (count * sumOfYSq - sumOfYSqr);
            sCo = sumCodeviates - ((sumOfXY) / count);

            double meanX = sumOfX / count;
            double meanY = sumOfY / count;
            double dblR = RNumerator / Math.Sqrt(RDenom);
            rsquared = dblR * dblR;
            yintercept = meanY - ((sCo / ssX) * meanX);
            slope = sCo / ssX;

            double ssr = 0;

            unsafe
            {
                fixed (double* xvalues = xVals)
                {
                    fixed (double* yvalues = yVals)
                    {
                        for (int ctr = inclusiveStart; ctr < exclusiveEnd; ctr++)
                        {
                            double x = *(xvalues + ctr);
                            double y = *(yvalues + ctr);

                            var tmp = y - (yintercept + slope * x);
                            ssr += tmp * tmp;
                        }
                    }
                }
                variance = ssr / (count - 2);
            }
        }
    }
}
