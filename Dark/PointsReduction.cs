using System.Collections.Generic;
using System.Linq;

namespace OxyPlot.Dark.Wpf
{
    /// <summary>
    /// https://www.codeproject.com/articles/18936/a-c-implementation-of-douglas-peucker-line-appro
    /// https://codereview.stackexchange.com/questions/29002/ramer-douglas-peucker-algorithm
    /// </summary>
    public class PointsReduction
    {
        /// <summary>
        /// Uses the Douglas Peucker algorithm to reduce the number of points.
        /// https://codereview.stackexchange.com/questions/29002/ramer-douglas-peucker-algorithm
        /// TO CHECK: https://math.stackexchange.com/questions/65115/how-to-predict-the-tolerance-value-that-will-yield-a-given-reduction-with-the-do
        /// </summary>
        /// <param name="points">The points.</param>
        /// <param name="tolerance">The tolerance.</param>
        /// <returns></returns>
        public static List<ScreenPoint> DouglasPeuckerReduction(List<ScreenPoint> points, double tolerance)
        {
            if (points == null || points.Count() < 3) return points;
            if (double.IsInfinity(tolerance) || double.IsNaN(tolerance)) return points;
            tolerance *= tolerance;
            if (tolerance <= float.Epsilon) return points;

            int firstPoint = 0;
            int lastPoint = points.Count() - 1;
            List<int> pointIndexsToKeep = new List<int>();

            // Add the first and last index to the keepers
            pointIndexsToKeep.Add(firstPoint);
            pointIndexsToKeep.Add(lastPoint);

            // The first and the last point cannot be the same
            while (points[firstPoint].Equals(points[lastPoint]))
            {
                lastPoint--;
            }

            DouglasPeuckerReduction(points, firstPoint, lastPoint, tolerance, ref pointIndexsToKeep);

            int l = pointIndexsToKeep.Count;
            List<ScreenPoint> returnPoints = new List<ScreenPoint>(l);
            pointIndexsToKeep.Sort();

            for (int i = 0; i < l; ++i)
            {
                returnPoints.Add(points[pointIndexsToKeep[i]]);
            }
            return returnPoints;
        }

        /// <summary>
        /// Douglases the peucker reduction.
        /// </summary>
        /// <param name="points">The points.</param>
        /// <param name="firstPoint">The first point.</param>
        /// <param name="lastPoint">The last point.</param>
        /// <param name="tolerance">The tolerance.</param>
        /// <param name="pointIndexsToKeep">The point index to keep.</param>
        internal static void DouglasPeuckerReduction(List<ScreenPoint> points, int firstPoint,
            int lastPoint, double tolerance, ref List<int> pointIndexsToKeep)
        {
            double maxDistance = 0;
            int indexFarthest = 0;

            ScreenPoint Point1 = points[firstPoint];
            ScreenPoint Point2 = points[lastPoint];
            double distXY = Point1.X * Point2.Y - Point2.X * Point1.Y;
            double distX = Point2.X - Point1.X;
            double distY = Point1.Y - Point2.Y;
            double bottom = distX * distX + distY * distY;

            for (int index = firstPoint; index < lastPoint; index++)
            {
                ScreenPoint Point = points[index];
                double area = distXY + distX * Point.Y + distY * Point.X;
                double distance = (area / bottom) * area;

                if (distance > maxDistance)
                {
                    maxDistance = distance;
                    indexFarthest = index;
                }
            }

            if (maxDistance > tolerance) // && indexFarthest != 0)
            {
                //Add the largest point that exceeds the tolerance
                pointIndexsToKeep.Add(indexFarthest);
                DouglasPeuckerReduction(points, firstPoint, indexFarthest, tolerance, ref pointIndexsToKeep);
                DouglasPeuckerReduction(points, indexFarthest, lastPoint, tolerance, ref pointIndexsToKeep);
            }
        }

        /// <summary>
        /// Uses the Douglas Peucker algorithm to reduce the number of points.
        /// https://codereview.stackexchange.com/questions/29002/ramer-douglas-peucker-algorithm
        /// TO CHECK: https://math.stackexchange.com/questions/65115/how-to-predict-the-tolerance-value-that-will-yield-a-given-reduction-with-the-do
        /// </summary>
        /// <param name="points">The points.</param>
        /// <param name="tolerance">The tolerance.</param>
        /// <returns></returns>
        public static ScreenPoint[] DouglasPeuckerReduction(ScreenPoint[] points, double tolerance)
        {
            if (points == null || points.Count() < 3) return points;
            if (double.IsInfinity(tolerance) || double.IsNaN(tolerance)) return points;
            tolerance *= tolerance;
            if (tolerance <= float.Epsilon) return points;

            int firstPoint = 0;
            int lastPoint = points.Count() - 1;
            List<int> pointIndexsToKeep = new List<int>();

            // Add the first and last index to the keepers
            pointIndexsToKeep.Add(firstPoint);
            pointIndexsToKeep.Add(lastPoint);

            // The first and the last point cannot be the same
            while (points[firstPoint].Equals(points[lastPoint]))
            {
                lastPoint--;
            }

            DouglasPeuckerReduction(points, firstPoint, lastPoint, tolerance, ref pointIndexsToKeep);

            int l = pointIndexsToKeep.Count;
            ScreenPoint[] returnPoints = new ScreenPoint[l];
            pointIndexsToKeep.Sort();

            unsafe
            {
                fixed (ScreenPoint* ptr = points, result = returnPoints)
                {
                    for (int i = 0; i < l; ++i)
                        *(result + i) = *(ptr + pointIndexsToKeep[i]);
                }
            }

            return returnPoints;
        }

        /// <summary>
        /// Douglases the peucker reduction.
        /// </summary>
        /// <param name="points">The points.</param>
        /// <param name="firstPoint">The first point.</param>
        /// <param name="lastPoint">The last point.</param>
        /// <param name="tolerance">The tolerance.</param>
        /// <param name="pointIndexsToKeep">The point index to keep.</param>
        internal static void DouglasPeuckerReduction(ScreenPoint[] points, int firstPoint,
            int lastPoint, double tolerance, ref List<int> pointIndexsToKeep)
        {
            double maxDistance = 0;
            int indexFarthest = 0;

            unsafe
            {
                fixed (ScreenPoint* samples = points)
                {
                    ScreenPoint Point1 = *(samples + firstPoint);
                    ScreenPoint Point2 = *(samples + lastPoint);
                    double distXY = Point1.X * Point2.Y - Point2.X * Point1.Y;
                    double distX = Point2.X - Point1.X;
                    double distY = Point1.Y - Point2.Y;
                    double bottom = distX * distX + distY * distY;

                    for (int index = firstPoint; index < lastPoint; index++)
                    {
                        // Perpendicular Distance

                        ScreenPoint Point = *(samples + index);
                        double area = distXY + distX * Point.Y + distY * Point.X;
                        double distance = (area / bottom) * area;

                        if (distance > maxDistance)
                        {
                            maxDistance = distance;
                            indexFarthest = index;
                        }
                    }
                }
            }

            if (maxDistance > tolerance) // && indexFarthest != 0)
            {
                //Add the largest point that exceeds the tolerance
                pointIndexsToKeep.Add(indexFarthest);
                DouglasPeuckerReduction(points, firstPoint, indexFarthest, tolerance, ref pointIndexsToKeep);
                DouglasPeuckerReduction(points, indexFarthest, lastPoint, tolerance, ref pointIndexsToKeep);
            }
        }

        /// <summary>
        /// Uses the Douglas Peucker algorithm to reduce the number of points.
        /// https://codereview.stackexchange.com/questions/29002/ramer-douglas-peucker-algorithm
        /// TO CHECK: https://math.stackexchange.com/questions/65115/how-to-predict-the-tolerance-value-that-will-yield-a-given-reduction-with-the-do
        /// </summary>
        /// <param name="points">The points.</param>
        /// <param name="tolerance">The tolerance.</param>
        /// <returns></returns>
        public static DataPoint[] DouglasPeuckerReduction(DataPoint[] points, double tolerance)
        {
            if (points == null || points.Count() < 3) return points;
            if (double.IsInfinity(tolerance)|| double.IsNaN(tolerance)) return points;
            tolerance *= tolerance;
            if (tolerance <= float.Epsilon) return points;

            int firstPoint = 0;
            int lastPoint = points.Count() - 1;
            List<int> pointIndexsToKeep = new List<int>();

            // Add the first and last index to the keepers
            pointIndexsToKeep.Add(firstPoint);
            pointIndexsToKeep.Add(lastPoint);

            // The first and the last point cannot be the same
            while (points[firstPoint].Equals(points[lastPoint]))
            {
                lastPoint--;
            }

            DouglasPeuckerReduction(points, firstPoint, lastPoint, tolerance, ref pointIndexsToKeep);

            int l = pointIndexsToKeep.Count;
            DataPoint[] returnPoints = new DataPoint[l];
            pointIndexsToKeep.Sort();

            unsafe
            {
                fixed (DataPoint* ptr = points, result = returnPoints)
                {
                    for (int i = 0; i < l; ++i)
                        *(result + i) = *(ptr + pointIndexsToKeep[i]);
                }
            }

            return returnPoints;
        }

        /// <summary>
        /// Douglases the peucker reduction.
        /// </summary>
        /// <param name="points">The points.</param>
        /// <param name="firstPoint">The first point.</param>
        /// <param name="lastPoint">The last point.</param>
        /// <param name="tolerance">The tolerance.</param>
        /// <param name="pointIndexsToKeep">The point index to keep.</param>
        internal static void DouglasPeuckerReduction(DataPoint[] points, int firstPoint,
            int lastPoint, double tolerance, ref List<int> pointIndexsToKeep)
        {
            double maxDistance = 0;
            int indexFarthest = 0;

            unsafe
            {
                fixed (DataPoint* samples = points)
                {
                    DataPoint Point1 = *(samples + firstPoint);
                    DataPoint Point2 = *(samples + lastPoint);
                    double distXY = Point1.X * Point2.Y - Point2.X * Point1.Y;
                    double distX = Point2.X - Point1.X;
                    double distY = Point1.Y - Point2.Y;
                    double bottom = distX * distX + distY * distY;

                    for (int index = firstPoint; index < lastPoint; index++)
                    {
                        //Area = |(1/2)(x1y2 + x2y3 + x3y1 - x2y1 - x3y2 - x1y3)|   *Area of triangle
                        //Base = v((x1-x2)²+(x1-x2)²)                               *Base of Triangle*
                        //Area = .5*Base*H                                          *Solve for height
                        //Height = Area/.5/Base

                        DataPoint Point = *(samples + index);
                        double area = distXY + distX * Point.Y + distY * Point.X;
                        double distance = (area / bottom) * area;

                        if (distance > maxDistance)
                        {
                            maxDistance = distance;
                            indexFarthest = index;
                        }
                    }
                }
            }

            if (maxDistance > tolerance) // && indexFarthest != 0)
            {
                //Add the largest point that exceeds the tolerance
                pointIndexsToKeep.Add(indexFarthest);
                DouglasPeuckerReduction(points, firstPoint, indexFarthest, tolerance, ref pointIndexsToKeep);
                DouglasPeuckerReduction(points, indexFarthest, lastPoint, tolerance, ref pointIndexsToKeep);
            }
        }
    }
}
