using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PackMine.Geometry
{
    public class IntMap
    {
        int[][] data;
        public int width
        {
            get;
            private set;
        }
        int height;
        int defaultValue;
        public IntMap(int width, int height, Func<int, int, int> generator, int defaultValue)
        {
            this.width = width;
            this.height = height;
            this.defaultValue = defaultValue;
            data = new int[width][];
            for (int i = 0; i < width; i++)
            {
                data[i] = new int[height];
                for (int j = 0; j < height; j++)
                {
                    data[i][j] = generator(i, j);
                }
            }
        }
        public static int Roomy (int x, int y)
        {
            if (x % 6 != 5 && y % 6 != 5)
                return CellValue.Open;
            if (x % 6 == 2 || y % 6 == 2)
                return CellValue.Open;
            return CellValue.Wall;
        }
        public int Get(IntPoint p)
        {
            return Get(p.x, p.y);
        }
        public int MagicGet(IntPoint p)
        {
            return MagicGet(p.x, p.y);
        }
        public int MagicGet(int x, int y)
        {
            if (0 <= x && x < width && 0 <= y && y < height)
            {
                return data[x][y];
            }
            if (x < -1 || x > width || y < -1 || y > height)
                return -10;
            else
                return defaultValue;
        }
        public int Get(int x, int y)
        {
            if (0 <= x && x < width && 0 <= y && y < height)
            {
                return data[x][y];
            }
            return defaultValue;
        }
        public void Set(IntPoint p, int value)
        {
            Set(p.x, p.y, value);
        }
        public void Set(int x, int y, int value)
        {
            if (0 <= x && x < width && 0 <= y && y < height)
            {
                data[x][y] = value;
            }
        }
    }
}
