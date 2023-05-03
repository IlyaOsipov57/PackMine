using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace PackMine.Geometry
{
    public struct RealPoint
    {
        public double X
        {
            get;
            private set;
        }
        public double Y
        {
            get;
            private set;
        }
        public RealPoint(double x, double y)
            : this()
        {
            this.X = x;
            this.Y = y;
        }
        public double Length
        {
            get
            {
                return Math.Sqrt(X * X + Y * Y);
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
            return new RealPoint(A.X + B.X, A.Y + B.Y);
        }
        public static RealPoint operator -(RealPoint A, RealPoint B)
        {
            return new RealPoint(A.X - B.X, A.Y - B.Y);
        }
        public static RealPoint operator -(RealPoint A)
        {
            return new RealPoint(-A.X, -A.Y);
        }
        public static double operator *(RealPoint A, RealPoint B)
        {
            return A.X * B.X + A.Y * B.Y;
        }
        public static double operator ^(RealPoint A, RealPoint B)
        {
            return A.X * B.Y - B.X * A.Y;
        }
        public static RealPoint operator *(RealPoint A, double k)
        {
            return new RealPoint(A.X * k, A.Y * k);
        }
        public static RealPoint operator *(double k, RealPoint A)
        {
            return new RealPoint(A.X * k, A.Y * k);
        }
        public static RealPoint operator /(RealPoint A, double k)
        {
            return new RealPoint(A.X / k, A.Y / k);
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

            return A.X == B.X && A.Y == B.Y;
        }
        public static bool operator !=(RealPoint A, RealPoint B)
        {
            return !(A == B);
        }
        public static explicit operator PointF(RealPoint A)
        {
            return new PointF((float)A.X, (float)A.Y);
        }
        public static explicit operator SizeF(RealPoint A)
        {
            return new SizeF((float)A.X, (float)A.Y);
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
            return new RealPoint(A.X, A.Y);
        }
        public static explicit operator RealPoint(Size A)
        {
            return new RealPoint(A.Width, A.Height);
        }
        public override int GetHashCode()
        {
            return X.GetHashCode() ^ (Y.GetHashCode() << 16);
        }
        public override bool Equals(object obj)
        {
            var p = (RealPoint)obj;
            if ((object)p == null)
                return false;

            return X == p.X && Y == p.Y;
        }
        public override string ToString()
        {
            return String.Format("RPoint: {0:f2}; {1:f2}", X, Y);
        }
    }
}
