using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace PackMine.Geometry
{
    public struct RealPoint
    {
        public double x;
        public double y;
        public RealPoint(double x, double y)
        {
            this.x = x;
            this.y = y;
        }
        public double Length
        {
            get
            {
                return Math.Sqrt(x * x + y * y);
            }
        }
        public static RealPoint Zero
        {
            get
            {
                return new RealPoint(0, 0);
            }
        }
        public static RealPoint operator +(RealPoint A, RealPoint B)
        {
            return new RealPoint(A.x + B.x, A.y + B.y);
        }
        public static RealPoint operator -(RealPoint A, RealPoint B)
        {
            return new RealPoint(A.x - B.x, A.y - B.y);
        }
        public static RealPoint operator -(RealPoint A)
        {
            return new RealPoint(-A.x, -A.y);
        }
        public static double operator *(RealPoint A, RealPoint B)
        {
            return A.x * B.x + A.y * B.y;
        }
        public static double operator ^(RealPoint A, RealPoint B)
        {
            return A.x * B.y - B.x * A.y;
        }
        public static RealPoint operator *(RealPoint A, double k)
        {
            return new RealPoint(A.x * k, A.y * k);
        }
        public static RealPoint operator *(double k, RealPoint A)
        {
            return new RealPoint(A.x * k, A.y * k);
        }
        public static RealPoint operator /(RealPoint A, double k)
        {
            return new RealPoint(A.x / k, A.y / k);
        }
        public static bool operator ==(RealPoint A, RealPoint B)
        {
            if (System.Object.ReferenceEquals(A, B))
            {
                return true;
            }

            if (((object)A == null) || ((object)B == null))
            {
                return false;
            }

            return A.x == B.x && A.y == B.y;
        }
        public static bool operator !=(RealPoint A, RealPoint B)
        {
            return !(A == B);
        }
        public static explicit operator PointF(RealPoint A)
        {
            return new PointF((float)A.x, (float)A.y);
        }
        public static explicit operator SizeF(RealPoint A)
        {
            return new SizeF((float)A.x, (float)A.y);
        }
        public static explicit operator RealPoint(Point A)
        {
            return new RealPoint(A.X, A.Y);
        }
        public static explicit operator RealPoint(PointF A)
        {
            return new RealPoint(A.X, A.Y);
        }
        public static explicit operator RealPoint(IntPoint A)
        {
            return new RealPoint(A.x, A.y);
        }
        public static explicit operator RealPoint(Size A)
        {
            return new RealPoint(A.Width, A.Height);
        }
        public override int GetHashCode()
        {
            return x.GetHashCode() ^ (y.GetHashCode() << 16);
        }
        public override bool Equals(object obj)
        {
            var p = (RealPoint)obj;
            if ((object)p == null)
                return false;

            return x == p.x && y == p.y;
        }
        public override string ToString()
        {
            return String.Format("RPoint: {0:f2}; {1:f2}", x, y);
        }
    }
}
