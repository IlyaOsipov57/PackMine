using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PackMine.Geometry
{
    public struct ZPoint
    {
        public int x;
        public int y;
        public ZPoint(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
        public ZPoint(ZPoint z)
        {
            this.x = z.x;
            this.y = z.y;
        }
        public double Length
        {
            get
            {
                return Math.Sqrt(SquaredLength);
            }
        }
        public int SquaredLength
        {
            get
            {
                return x * x + y * y;
            }
        }
        public void PlanarNormalize()
        {
            x = Math.Sign(x);
            y = Math.Sign(y);
        }
        public static ZPoint Zero
        {
            get
            {
                return new ZPoint(0, 0);
            }
        }
        public static ZPoint operator +(ZPoint A, ZPoint B)
        {
            return new ZPoint(A.x + B.x, A.y + B.y);
        }
        public static ZPoint operator -(ZPoint A, ZPoint B)
        {
            return new ZPoint(A.x - B.x, A.y - B.y);
        }
        public static ZPoint operator -(ZPoint A)
        {
            return new ZPoint(-A.x, -A.y);
        }
        public static double operator *(ZPoint A, ZPoint B)
        {
            return A.x * B.x + A.y * B.y;
        }
        public static double operator ^(ZPoint A, ZPoint B)
        {
            return A.x * B.y - B.x * A.y;
        }
        public static ZPoint operator *(ZPoint A, int k)
        {
            return new ZPoint(A.x * k, A.y * k);
        }
        public static ZPoint operator *(int k, ZPoint A)
        {
            return new ZPoint(A.x * k, A.y * k);
        }
        public static ZPoint operator /(ZPoint A, int k)
        {
            return new ZPoint(A.x / k, A.y / k);
        }
        public static bool operator ==(ZPoint A, ZPoint B)
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
        public static bool operator !=(ZPoint A, ZPoint B)
        {
            return !(A == B);
        }
        public override int GetHashCode()
        {
            return x.GetHashCode() ^ (y.GetHashCode() << 16);
        }
        public override bool Equals(object obj)
        {
            var p = (ZPoint)obj;
            if ((object)p == null)
                return false;

            return x == p.x && y == p.y;
        }
        public override string ToString()
        {
            return String.Format("ZPoint: {0:f2}; {1:f2}", x, y);
        }
    }
}
