﻿using ATMDesigner.UI.Controls;
using ATMDesigner.UI.Model;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace ATMDesigner.UI
{
    // Note: I couldn't find a useful open source library that does
    // orthogonal routing so started to write something on my own.
    // Categorize this as a quick and dirty short term solution.
    // I will keep on searching.

    // Helper class to provide an orthogonal connection path
    internal class ViewModelPathFinder
    {
        private const int margin = 20;

        internal static List<Point> GetConnectionLine(ModelConnectorInfo source, ModelConnectorInfo sink, bool showLastLine)
        {
            List<Point> linePoints = new List<Point>();

            Rect rectSource = GetRectWithMargin(source, margin);
            Rect rectSink = GetRectWithMargin(sink, margin);

            Point startPoint = GetOffsetPoint(source, rectSource);
            Point endPoint = GetOffsetPoint(sink, rectSink);

            linePoints.Add(startPoint);
            Point currentPoint = startPoint;

            if (!rectSink.Contains(currentPoint) && !rectSource.Contains(endPoint))
            {
                while (true)
                {
                    #region source node

                    if (IsPointVisible(currentPoint, endPoint, new Rect[] { rectSource, rectSink }))
                    {
                        linePoints.Add(endPoint);
                        currentPoint = endPoint;
                        break;
                    }

                    Point neighbour = GetNearestVisibleNeighborSink(currentPoint, endPoint, sink, rectSource, rectSink);
                    if (!double.IsNaN(neighbour.X))
                    {
                        linePoints.Add(neighbour);
                        linePoints.Add(endPoint);
                        currentPoint = endPoint;
                        break;
                    }

                    if (currentPoint == startPoint)
                    {
                        bool flag;
                        Point n = GetNearestNeighborSource(source, endPoint, rectSource, rectSink, out flag);
                        linePoints.Add(n);
                        currentPoint = n;

                        if (!IsRectVisible(currentPoint, rectSink, new Rect[] { rectSource }))
                        {
                            Point n1, n2;
                            GetOppositeCorners(source.Orientation, rectSource, out n1, out n2);
                            if (flag)
                            {
                                linePoints.Add(n1);
                                currentPoint = n1;
                            }
                            else
                            {
                                linePoints.Add(n2);
                                currentPoint = n2;
                            }
                            if (!IsRectVisible(currentPoint, rectSink, new Rect[] { rectSource }))
                            {
                                if (flag)
                                {
                                    linePoints.Add(n2);
                                    currentPoint = n2;
                                }
                                else
                                {
                                    linePoints.Add(n1);
                                    currentPoint = n1;
                                }
                            }
                        }
                    }
                    #endregion

                    #region sink node

                    else // from here on we jump to the sink node
                    {
                        Point n1, n2; // neighbour corner
                        Point s1, s2; // opposite corner
                        GetNeighborCorners(sink.Orientation, rectSink, out s1, out s2);
                        GetOppositeCorners(sink.Orientation, rectSink, out n1, out n2);

                        bool n1Visible = IsPointVisible(currentPoint, n1, new Rect[] { rectSource, rectSink });
                        bool n2Visible = IsPointVisible(currentPoint, n2, new Rect[] { rectSource, rectSink });

                        if (n1Visible && n2Visible)
                        {
                            if (rectSource.Contains(n1))
                            {
                                linePoints.Add(n2);
                                if (rectSource.Contains(s2))
                                {
                                    linePoints.Add(n1);
                                    linePoints.Add(s1);
                                }
                                else
                                    linePoints.Add(s2);

                                linePoints.Add(endPoint);
                                currentPoint = endPoint;
                                break;
                            }

                            if (rectSource.Contains(n2))
                            {
                                linePoints.Add(n1);
                                if (rectSource.Contains(s1))
                                {
                                    linePoints.Add(n2);
                                    linePoints.Add(s2);
                                }
                                else
                                    linePoints.Add(s1);

                                linePoints.Add(endPoint);
                                currentPoint = endPoint;
                                break;
                            }

                            if ((Distance(n1, endPoint) <= Distance(n2, endPoint)))
                            {
                                linePoints.Add(n1);
                                if (rectSource.Contains(s1))
                                {
                                    linePoints.Add(n2);
                                    linePoints.Add(s2);
                                }
                                else
                                    linePoints.Add(s1);
                                linePoints.Add(endPoint);
                                currentPoint = endPoint;
                                break;
                            }
                            else
                            {
                                linePoints.Add(n2);
                                if (rectSource.Contains(s2))
                                {
                                    linePoints.Add(n1);
                                    linePoints.Add(s1);
                                }
                                else
                                    linePoints.Add(s2);
                                linePoints.Add(endPoint);
                                currentPoint = endPoint;
                                break;
                            }
                        }
                        else if (n1Visible)
                        {
                            linePoints.Add(n1);
                            if (rectSource.Contains(s1))
                            {
                                linePoints.Add(n2);
                                linePoints.Add(s2);
                            }
                            else
                                linePoints.Add(s1);
                            linePoints.Add(endPoint);
                            currentPoint = endPoint;
                            break;
                        }
                        else
                        {
                            linePoints.Add(n2);
                            if (rectSource.Contains(s2))
                            {
                                linePoints.Add(n1);
                                linePoints.Add(s1);
                            }
                            else
                                linePoints.Add(s2);
                            linePoints.Add(endPoint);
                            currentPoint = endPoint;
                            break;
                        }
                    }
                    #endregion
                }
            }
            else
            {
                linePoints.Add(endPoint);
            }

            linePoints = OptimizeLinePoints(linePoints, new Rect[] { rectSource, rectSink }, source.Orientation, sink.Orientation);

            CheckPathEnd(source, sink, showLastLine, linePoints);
            return linePoints;
        }

        internal static List<Point> GetConnectionLine(ModelConnectorInfo source, Point sinkPoint, EnumConnectorOrientation preferredOrientation)
        {
            List<Point> linePoints = new List<Point>();
            Rect rectSource = GetRectWithMargin(source, 10);
            Point startPoint = GetOffsetPoint(source, rectSource);
            Point endPoint = sinkPoint;

            linePoints.Add(startPoint);
            Point currentPoint = startPoint;

            if (!rectSource.Contains(endPoint))
            {
                while (true)
                {
                    if (IsPointVisible(currentPoint, endPoint, new Rect[] { rectSource }))
                    {
                        linePoints.Add(endPoint);
                        break;
                    }

                    bool sideFlag;
                    Point n = GetNearestNeighborSource(source, endPoint, rectSource, out sideFlag);
                    linePoints.Add(n);
                    currentPoint = n;

                    if (IsPointVisible(currentPoint, endPoint, new Rect[] { rectSource }))
                    {
                        linePoints.Add(endPoint);
                        break;
                    }
                    else
                    {
                        Point n1, n2;
                        GetOppositeCorners(source.Orientation, rectSource, out n1, out n2);
                        if (sideFlag)
                            linePoints.Add(n1);
                        else
                            linePoints.Add(n2);

                        linePoints.Add(endPoint);
                        break;
                    }
                }
            }
            else
            {
                linePoints.Add(endPoint);
            }

            if (preferredOrientation != EnumConnectorOrientation.None)
                linePoints = OptimizeLinePoints(linePoints, new Rect[] { rectSource }, source.Orientation, preferredOrientation);
            else
                linePoints = OptimizeLinePoints(linePoints, new Rect[] { rectSource }, source.Orientation, GetOpositeOrientation(source.Orientation));

            return linePoints;
        }

        private static List<Point> OptimizeLinePoints(List<Point> linePoints, Rect[] rectangles, EnumConnectorOrientation sourceOrientation, EnumConnectorOrientation sinkOrientation)
        {
            List<Point> points = new List<Point>();
            int cut = 0;

            for (int i = 0; i < linePoints.Count; i++)
            {
                if (i >= cut)
                {
                    for (int k = linePoints.Count - 1; k > i; k--)
                    {
                        if (IsPointVisible(linePoints[i], linePoints[k], rectangles))
                        {
                            cut = k;
                            break;
                        }
                    }
                    points.Add(linePoints[i]);
                }
            }

            #region Line
            for (int j = 0; j < points.Count - 1; j++)
            {
                if (points[j].X != points[j + 1].X && points[j].Y != points[j + 1].Y)
                {
                    EnumConnectorOrientation orientationFrom;
                    EnumConnectorOrientation orientationTo;

                    // orientation from point
                    if (j == 0)
                        orientationFrom = sourceOrientation;
                    else
                        orientationFrom = GetOrientation(points[j], points[j - 1]);

                    // orientation to pint 
                    if (j == points.Count - 2)
                        orientationTo = sinkOrientation;
                    else
                        orientationTo = GetOrientation(points[j + 1], points[j + 2]);


                    if ((orientationFrom == EnumConnectorOrientation.Left || orientationFrom == EnumConnectorOrientation.Right) &&
                        (orientationTo == EnumConnectorOrientation.Left || orientationTo == EnumConnectorOrientation.Right))
                    {
                        double centerX = Math.Min(points[j].X, points[j + 1].X) + Math.Abs(points[j].X - points[j + 1].X) / 2;
                        points.Insert(j + 1, new Point(centerX, points[j].Y));
                        points.Insert(j + 2, new Point(centerX, points[j + 2].Y));
                        if (points.Count - 1 > j + 3)
                            points.RemoveAt(j + 3);
                        return points;
                    }

                    if ((orientationFrom == EnumConnectorOrientation.Top || orientationFrom == EnumConnectorOrientation.Bottom) &&
                        (orientationTo == EnumConnectorOrientation.Top || orientationTo == EnumConnectorOrientation.Bottom))
                    {
                        double centerY = Math.Min(points[j].Y, points[j + 1].Y) + Math.Abs(points[j].Y - points[j + 1].Y) / 2;
                        points.Insert(j + 1, new Point(points[j].X, centerY));
                        points.Insert(j + 2, new Point(points[j + 2].X, centerY));
                        if (points.Count - 1 > j + 3)
                            points.RemoveAt(j + 3);
                        return points;
                    }

                    if ((orientationFrom == EnumConnectorOrientation.Left || orientationFrom == EnumConnectorOrientation.Right) &&
                        (orientationTo == EnumConnectorOrientation.Top || orientationTo == EnumConnectorOrientation.Bottom))
                    {
                        points.Insert(j + 1, new Point(points[j + 1].X, points[j].Y));
                        return points;
                    }

                    if ((orientationFrom == EnumConnectorOrientation.Top || orientationFrom == EnumConnectorOrientation.Bottom) &&
                        (orientationTo == EnumConnectorOrientation.Left || orientationTo == EnumConnectorOrientation.Right))
                    {
                        points.Insert(j + 1, new Point(points[j].X, points[j + 1].Y));
                        return points;
                    }
                }
            }
            #endregion

            return points;
        }

        private static EnumConnectorOrientation GetOrientation(Point p1, Point p2)
        {
            if (p1.X == p2.X)
            {
                if (p1.Y >= p2.Y)
                    return EnumConnectorOrientation.Bottom;
                else
                    return EnumConnectorOrientation.Top;
            }
            else if (p1.Y == p2.Y)
            {
                if (p1.X >= p2.X)
                    return EnumConnectorOrientation.Right;
                else
                    return EnumConnectorOrientation.Left;
            }
            MessageBox.Show("Bağlantı Noktalarında beklenemeyen bir kordinasyon kayması ! ", "GetOrientation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return EnumConnectorOrientation.Top;
            //throw new Exception("Failed to retrieve orientation");
        }

        private static Orientation GetOrientation(EnumConnectorOrientation sourceOrientation)
        {
            switch (sourceOrientation)
            {
                case EnumConnectorOrientation.Left:
                    return Orientation.Horizontal;
                case EnumConnectorOrientation.Top:
                    return Orientation.Vertical;
                case EnumConnectorOrientation.Right:
                    return Orientation.Horizontal;
                case EnumConnectorOrientation.Bottom:
                    return Orientation.Vertical;
                default:
                    {
                        MessageBox.Show("Bağlantı Noktalarında beklenemeyen bir kordinasyon kayması ! ", "ConnectorOrientation", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return Orientation.Horizontal;
                    }
                    //throw new Exception("Unknown ConnectorOrientation");
            }
        }

        private static Point GetNearestNeighborSource(ModelConnectorInfo source, Point endPoint, Rect rectSource, Rect rectSink, out bool flag)
        {
            Point n1, n2; // neighbors
            GetNeighborCorners(source.Orientation, rectSource, out n1, out n2);

            if (rectSink.Contains(n1))
            {
                flag = false;
                return n2;
            }

            if (rectSink.Contains(n2))
            {
                flag = true;
                return n1;
            }

            if ((Distance(n1, endPoint) <= Distance(n2, endPoint)))
            {
                flag = true;
                return n1;
            }
            else
            {
                flag = false;
                return n2;
            }
        }

        private static Point GetNearestNeighborSource(ModelConnectorInfo source, Point endPoint, Rect rectSource, out bool flag)
        {
            Point n1, n2; // neighbors
            GetNeighborCorners(source.Orientation, rectSource, out n1, out n2);

            if ((Distance(n1, endPoint) <= Distance(n2, endPoint)))
            {
                flag = true;
                return n1;
            }
            else
            {
                flag = false;
                return n2;
            }
        }

        private static Point GetNearestVisibleNeighborSink(Point currentPoint, Point endPoint, ModelConnectorInfo sink, Rect rectSource, Rect rectSink)
        {
            Point s1, s2; // neighbors on sink side
            GetNeighborCorners(sink.Orientation, rectSink, out s1, out s2);

            bool flag1 = IsPointVisible(currentPoint, s1, new Rect[] { rectSource, rectSink });
            bool flag2 = IsPointVisible(currentPoint, s2, new Rect[] { rectSource, rectSink });

            if (flag1) // s1 visible
            {
                if (flag2) // s1 and s2 visible
                {
                    if (rectSink.Contains(s1))
                        return s2;

                    if (rectSink.Contains(s2))
                        return s1;

                    if ((Distance(s1, endPoint) <= Distance(s2, endPoint)))
                        return s1;
                    else
                        return s2;

                }
                else
                {
                    return s1;
                }
            }
            else // s1 not visible
            {
                if (flag2) // only s2 visible
                {
                    return s2;
                }
                else // s1 and s2 not visible
                {
                    return new Point(double.NaN, double.NaN);
                }
            }
        }

        private static bool IsPointVisible(Point fromPoint, Point targetPoint, Rect[] rectangles)
        {
            foreach (Rect rect in rectangles)
            {
                if (RectangleIntersectsLine(rect, fromPoint, targetPoint))
                    return false;
            }
            return true;
        }

        private static bool IsRectVisible(Point fromPoint, Rect targetRect, Rect[] rectangles)
        {
            if (IsPointVisible(fromPoint, targetRect.TopLeft, rectangles))
                return true;

            if (IsPointVisible(fromPoint, targetRect.TopRight, rectangles))
                return true;

            if (IsPointVisible(fromPoint, targetRect.BottomLeft, rectangles))
                return true;

            if (IsPointVisible(fromPoint, targetRect.BottomRight, rectangles))
                return true;

            return false;
        }

        private static bool RectangleIntersectsLine(Rect rect, Point startPoint, Point endPoint)
        {
            rect.Inflate(-1, -1);
            return rect.IntersectsWith(new Rect(startPoint, endPoint));
        }

        private static void GetOppositeCorners(EnumConnectorOrientation orientation, Rect rect, out Point n1, out Point n2)
        {
            switch (orientation)
            {
                case EnumConnectorOrientation.Left:
                    n1 = rect.TopRight; n2 = rect.BottomRight;
                    break;
                case EnumConnectorOrientation.Top:
                    n1 = rect.BottomLeft; n2 = rect.BottomRight;
                    break;
                case EnumConnectorOrientation.Right:
                    n1 = rect.TopLeft; n2 = rect.BottomLeft;
                    break;
                case EnumConnectorOrientation.Bottom:
                    n1 = rect.TopLeft; n2 = rect.TopRight;
                    break;
                default:
                    throw new Exception("No opposite corners found!");
            }
        }

        private static void GetNeighborCorners(EnumConnectorOrientation orientation, Rect rect, out Point n1, out Point n2)
        {
            switch (orientation)
            {
                case EnumConnectorOrientation.Left:
                    n1 = rect.TopLeft; n2 = rect.BottomLeft;
                    break;
                case EnumConnectorOrientation.Top:
                    n1 = rect.TopLeft; n2 = rect.TopRight;
                    break;
                case EnumConnectorOrientation.Right:
                    n1 = rect.TopRight; n2 = rect.BottomRight;
                    break;
                case EnumConnectorOrientation.Bottom:
                    n1 = rect.BottomLeft; n2 = rect.BottomRight;
                    break;
                default:
                    throw new Exception("No neighour corners found!");
            }
        }

        private static double Distance(Point p1, Point p2)
        {
            return Point.Subtract(p1, p2).Length;
        }

        private static Rect GetRectWithMargin(ModelConnectorInfo connectorThumb, double margin)
        {
            Rect rect = new Rect(connectorThumb.DesignerItemLeft,
                                 connectorThumb.DesignerItemTop,
                                 connectorThumb.DesignerItemSize.Width,
                                 connectorThumb.DesignerItemSize.Height);

            rect.Inflate(margin, margin);

            return rect;
        }

        private static Point GetOffsetPoint(ModelConnectorInfo connector, Rect rect)
        {
            Point offsetPoint = new Point();

            switch (connector.Orientation)
            {
                case EnumConnectorOrientation.Left:
                    offsetPoint = new Point(rect.Left, connector.Position.Y);
                    break;
                case EnumConnectorOrientation.Top:
                    offsetPoint = new Point(connector.Position.X, rect.Top);
                    break;
                case EnumConnectorOrientation.Right:
                    offsetPoint = new Point(rect.Right, connector.Position.Y);
                    break;
                case EnumConnectorOrientation.Bottom:
                    offsetPoint = new Point(connector.Position.X, rect.Bottom);
                    break;
                default:
                    break;
            }

            return offsetPoint;
        }

        private static void CheckPathEnd(ModelConnectorInfo source, ModelConnectorInfo sink, bool showLastLine, List<Point> linePoints)
        {
            if (showLastLine)
            {
                Point startPoint = new Point(0, 0);
                Point endPoint = new Point(0, 0);
                double marginPath = 15;
                switch (source.Orientation)
                {
                    case EnumConnectorOrientation.Left:
                        startPoint = new Point(source.Position.X - marginPath, source.Position.Y);
                        break;
                    case EnumConnectorOrientation.Top:
                        startPoint = new Point(source.Position.X, source.Position.Y - marginPath);
                        break;
                    case EnumConnectorOrientation.Right:
                        startPoint = new Point(source.Position.X + marginPath, source.Position.Y);
                        break;
                    case EnumConnectorOrientation.Bottom:
                        startPoint = new Point(source.Position.X, source.Position.Y + marginPath);
                        break;
                    default:
                        break;
                }

                switch (sink.Orientation)
                {
                    case EnumConnectorOrientation.Left:
                        endPoint = new Point(sink.Position.X - marginPath, sink.Position.Y);
                        break;
                    case EnumConnectorOrientation.Top:
                        endPoint = new Point(sink.Position.X, sink.Position.Y - marginPath);
                        break;
                    case EnumConnectorOrientation.Right:
                        endPoint = new Point(sink.Position.X + marginPath, sink.Position.Y);
                        break;
                    case EnumConnectorOrientation.Bottom:
                        endPoint = new Point(sink.Position.X, sink.Position.Y + marginPath);
                        break;
                    default:
                        break;
                }
                linePoints.Insert(0, startPoint);
                linePoints.Add(endPoint);
            }
            else
            {
                linePoints.Insert(0, source.Position);
                linePoints.Add(sink.Position);
            }
        }

        private static EnumConnectorOrientation GetOpositeOrientation(EnumConnectorOrientation connectorOrientation)
        {
            switch (connectorOrientation)
            {
                case EnumConnectorOrientation.Left:
                    return EnumConnectorOrientation.Right;
                case EnumConnectorOrientation.Top:
                    return EnumConnectorOrientation.Bottom;
                case EnumConnectorOrientation.Right:
                    return EnumConnectorOrientation.Left;
                case EnumConnectorOrientation.Bottom:
                    return EnumConnectorOrientation.Top;
                default:
                    return EnumConnectorOrientation.Top;
            }
        }
    }
}
