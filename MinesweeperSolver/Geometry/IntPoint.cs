using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PackMine.Geometry
{
    public struct IntPoint
    {
        public int x;
        public int y;
        public IntPoint(int x, int y)
        {
            this.x = x;
            this.y = y;
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
        public static IntPoint Zero
        {
            get
            {
                return new IntPoint(0, 0);
            }
        }
        public static IntPoint operator +(IntPoint A, IntPoint B)
        {
            return new IntPoint(A.x + B.x, A.y + B.y);
        }
        public static IntPoint operator -(IntPoint A, IntPoint B)
        {
            return new IntPoint(A.x - B.x, A.y - B.y);
        }
        public static IntPoint operator -(IntPoint A)
        {
            return new IntPoint(-A.x, -A.y);
        }
        public static double operator *(IntPoint A, IntPoint B)
        {
            return A.x * B.x + A.y * B.y;
        }
        public static double operator ^(IntPoint A, IntPoint B)
        {
            return A.x * B.y - B.x * A.y;
        }
        public static IntPoint operator *(IntPoint A, int k)
        {
            return new IntPoint(A.x * k, A.y * k);
        }
        public static IntPoint operator *(int k, IntPoint A)
        {
            return new IntPoint(A.x * k, A.y * k);
        }
        public static IntPoint operator /(IntPoint A, int k)
        {
            return new IntPoint(A.x / k, A.y / k);
        }
        public static bool operator ==(IntPoint A, IntPoint B)
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
        public static bool operator !=(IntPoint A, IntPoint B)
        {
            return !(A == B);
        }
        public override int GetHashCode()
        {
            return x.GetHashCode() ^ (y.GetHashCode() << 16);
        }
        public override bool Equals(object obj)
        {
            var p = (IntPoint)obj;
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
