﻿using RGeos.Geometries;
using System.Collections.Generic;

namespace RGeos.Geometries
{
    /// <summary>
    /// 拓扑关系：相交、相离
    /// </summary>
    public class RgTopologicRelationship
    {
        /// <summary>
        /// 点在直线上
        /// </summary>
        /// <param name="rPt"></param>
        /// <param name="rLine"></param>
        /// <returns></returns>
        public static bool IsInLine(RgPoint rPt, RgLine rLine)
        {
            bool flag = false;
            return flag;
        }
        /// <summary>
        /// 点是否在共线的线段上
        /// 1 = P is inside S;
        /// 0 = P is not inside S
        /// </returns> 
        /// </summary>
        /// <param name="P">a point P</param>
        /// <param name="S">a collinear segment S</param>
        /// <returns></returns>
        public static int InSegment(RgPoint P, RgSegment S)
        {
            if (S.P0.X != S.P1.X)
            {    // S is not vertical
                if (S.P0.X <= P.X && P.X <= S.P1.X)
                    return 1;
                if (S.P0.X >= P.X && P.X >= S.P1.X)
                    return 1;
            }
            else
            {    // S is vertical, so test y coordinate
                if (S.P0.Y <= P.Y && P.Y <= S.P1.Y)
                    return 1;
                if (S.P0.Y >= P.Y && P.Y >= S.P1.Y)
                    return 1;
            }
            return 0;
        }
        /// <summary>
        /// 点是否在线段上
        /// </summary>
        /// <param name="P">任意的点</param>
        /// <param name="S">任意线段</param>
        /// <returns></returns>
        public static int In2D_Point_Segment(RgPoint P, RgSegment S)
        {

            return 0;
        }
        public static int In2D_Point_BoundingBox(RgPoint P, BoundingBox envelop)
        {
            bool flag = envelop.Contains(P);
            if (flag)
            {
                return 1;
            }
            return 0;
        }
        /// <summary>
        /// 点在多边形中
        /// </summary>
        /// <param name="rPt"></param>
        /// <param name="rLine"></param>
        /// <returns></returns>
        public static bool IsInPolygon(RgPoint rPt, Polygon rPolygon)
        {
            //List<RgPoint> poly = new List<RgPoint>();
            //for (int i = 0; i < rPolygon.ExteriorRing.NumPoints; i++)
            //{
            //    poly.Add(rPolygon.ExteriorRing.Point(i));
            //}
            //return IsInPolygon(poly, rPt);
            int n = rPolygon.ExteriorRing.NumPoints;
            RgPoint[] poly = new RgPoint[n + 1];
            for (int i = 0; i < rPolygon.ExteriorRing.NumPoints; i++)
            {
                poly[i] = rPolygon.ExteriorRing.Point(i);
            }
            poly[n] = rPolygon.ExteriorRing.Point(n - 1);
            return cn_PnPoly(rPt, poly, n) == 1 ? true : false;
        }
        /// <summary>
        /// 射线相交算法1
        /// </summary>
        /// <param name="poly"></param>
        /// <param name="aPoint"></param>
        /// <returns></returns>
        public static bool IsInPolygon(List<RgPoint> poly, RgPoint aPoint)
        {
            bool flag = false;
            int count = poly.Count;
            if (count < 3)
            {
                return false;
            }
            double x = poly[count - 1].X;
            double y = poly[count - 1].Y;
            for (int i = 0; i < count; i++)
            {
                double num5;
                double num6;
                double num7;
                double num8;
                if (poly[i].X > x)
                {
                    num5 = x;
                    num7 = poly[i].X;
                    num6 = y;
                    num8 = poly[i].Y;
                }
                else
                {
                    num5 = poly[i].X;
                    num7 = x;
                    num6 = poly[i].Y;
                    num8 = y;
                }
                if (((poly[i].X < aPoint.X) == (aPoint.X <= x)) && (((aPoint.Y - num6) * (num7 - num5)) < ((num8 - num6) * (aPoint.X - num5))))
                {
                    flag = !flag;
                }
                x = poly[i].X;
                y = poly[i].Y;
            }
            return flag;

        }

        // 射线相交算法2
        //cn_PnPoly(): crossing number test for a point in a polygon
        //      Input:   P = a point,
        //               V[] = vertex points of a polygon V[n+1] with V[n]=V[0]
        //      Return:  0 = outside, 1 = inside
        // This code is patterned after [Franklin, 2000]
        public static int cn_PnPoly(RgPoint P, RgPoint[] V, int n)
        {
            int cn = 0;    // the crossing number counter

            // loop through all edges of the polygon
            for (int i = 0; i < n; i++)
            {    // edge from V[i] to V[i+1]
                if (((V[i].Y <= P.Y) && (V[i + 1].Y > P.Y))    // an upward crossing
                 || ((V[i].Y > P.Y) && (V[i + 1].Y <= P.Y)))
                { // a downward crossing
                    // compute the actual edge-ray intersect x-coordinate
                    float vt = (float)((P.Y - V[i].Y) / (V[i + 1].Y - V[i].Y));
                    if (P.X < V[i].X + vt * (V[i + 1].X - V[i].X)) // P.x < intersect
                        ++cn;   // a valid crossing of y=P.y right of P.x
                }
            }
            return (cn & 1);    // 0 if even (out), and 1 if odd (in)

        }
        //===================================================================
        //旋转
        // wn_PnPoly(): winding number test for a point in a polygon
        //      Input:   P = a point,
        //               V[] = vertex points of a polygon V[n+1] with V[n]=V[0]
        //      Return:  wn = the winding number (=0 only if P is outside V[])
        public static int wn_PnPoly(RgPoint P, RgPoint[] V, int n)
        {
            int wn = 0;    // the winding number counter

            // loop through all edges of the polygon
            for (int i = 0; i < n; i++)
            {   // edge from V[i] to V[i+1]
                if (V[i].Y <= P.Y)
                {         // start y <= P.y
                    if (V[i + 1].Y > P.Y)      // an upward crossing
                        if (RgMath.isLeft(V[i], V[i + 1], P) > 0)  // P left of edge
                            ++wn;            // have a valid up intersect
                }
                else
                {                       // start y > P.y (no test needed)
                    if (V[i + 1].Y <= P.Y)     // a downward crossing
                        if (RgMath.isLeft(V[i], V[i + 1], P) < 0)  // P right of edge
                            --wn;            // have a valid down intersect
                }
            }
            return wn;
        }
    }
}
