using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PackMine.Geometry
{
    public struct IntPoint
    {
        public int X
        {
            get;
            private set;
        }
        public int Y
        {
            get;
            private set;
        }
        public IntPoint(int x, int y)
            : this()
        {
            this.X = x;
            this.Y = y;
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
                return X * X + Y * Y;
            }
        }
        public void PlanarNormalize()
        {
            X = Math.Sign(X);
            Y = Math.Sign(Y);
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
            return new IntPoint(A.X + B.X, A.Y + B.Y);
        }
        public static IntPoint operator -(IntPoint A, IntPoint B)
        {
            return new IntPoint(A.X - B.X, A.Y - B.Y);
        }
        public static IntPoint operator -(IntPoint A)
        {
            return new IntPoint(-A.X, -A.Y);
        }
        public static double operator *(IntPoint A, IntPoint B)
        {
            return A.X * B.X + A.Y * B.Y;
        }
        public static double operator ^(IntPoint A, IntPoint B)
        {
            return A.X * B.Y - B.X * A.Y;
        }
        public static IntPoint operator *(IntPoint A, int k)
        {
            return new IntPoint(A.X * k, A.Y * k);
        }
        public static IntPoint operator *(int k, IntPoint A)
        {
            return new IntPoint(A.X * k, A.Y * k);
        }
        public static IntPoint operator /(IntPoint A, int k)
        {
            return new IntPoint(A.X / k, A.Y / k);
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

            return A.X == B.X && A.Y == B.Y;
        }
        public static bool operator !=(IntPoint A, IntPoint B)
        {
            return !(A == B);
        }
        public override int GetHashCode()
        {
            return (new Tuple<int, int>(X, Y)).GetHashCode();
        }
        public override bool Equals(object obj)
        {
            var p = (IntPoint)obj;
            if ((object)p == null)
                return false;

            return X == p.X && Y == p.Y;
        }
        public override string ToString()
        {
            return String.Format("IntPoint: {0:f2}; {1:f2}", X, Y);
        }
    }
}
