using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace PackMine.Geometry
{
    public struct RPoint
    {
        public double x;
        public double y;
        public RPoint(double x, double y)
        {
            this.x = x;
            this.y = y;
        }
        public RPoint(RPoint z)
        {
            this.x = z.x;
            this.y = z.y;
        }
        public double Length
        {
            get
            {
                return Math.Sqrt(x * x + y * y);
            }
        }
        public static RPoint Zero
        {
            get
            {
                return new RPoint(0, 0);
            }
        }
        public static RPoint operator +(RPoint A, RPoint B)
        {
            return new RPoint(A.x + B.x, A.y + B.y);
        }
        public static RPoint operator -(RPoint A, RPoint B)
        {
            return new RPoint(A.x - B.x, A.y - B.y);
        }
        public static RPoint operator -(RPoint A)
        {
            return new RPoint(-A.x, -A.y);
        }
        public static double operator *(RPoint A, RPoint B)
        {
            return A.x * B.x + A.y * B.y;
        }
        public static double operator ^(RPoint A, RPoint B)
        {
            return A.x * B.y - B.x * A.y;
        }
        public static RPoint operator *(RPoint A, double k)
        {
            return new RPoint(A.x * k, A.y * k);
        }
        public static RPoint operator *(double k, RPoint A)
        {
            return new RPoint(A.x * k, A.y * k);
        }
        public static RPoint operator /(RPoint A, double k)
        {
            return new RPoint(A.x / k, A.y / k);
        }
        public static bool operator ==(RPoint A, RPoint B)
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
        public static bool operator !=(RPoint A, RPoint B)
        {
            return !(A == B);
        }
        public static explicit operator PointF(RPoint A)
        {
            return new PointF((float)A.x, (float)A.y);
        }
        public static explicit operator SizeF(RPoint A)
        {
            return new SizeF((float)A.x, (float)A.y);
        }
        public static explicit operator RPoint(Point A)
        {
            return new RPoint(A.X, A.Y);
        }
        public static explicit operator RPoint(PointF A)
        {
            return new RPoint(A.X, A.Y);
        }
        public static explicit operator RPoint(ZPoint A)
        {
            return new RPoint(A.x, A.y);
        }
        public static explicit operator RPoint(Size A)
        {
            return new RPoint(A.Width, A.Height);
        }
        public override int GetHashCode()
        {
            return x.GetHashCode() ^ (y.GetHashCode() << 16);
        }
        public override bool Equals(object obj)
        {
            var p = (RPoint)obj;
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
