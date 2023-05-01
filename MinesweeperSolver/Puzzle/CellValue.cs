using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PackMine
{
    static class CellValue
    {
        /// <summary>
        /// -2
        /// </summary>
        public static int Door = -2;
        /// <summary>
        /// -1
        /// </summary>
        public static int Wall = -1;
        public static bool IsWalkable(int value)
        {
            return value >= 0;
        }
        /// <summary>
        /// 0
        /// </summary>
        public static int Open = 0;
        public static bool IsNumber(int value)
        {
            return value > 0 && value < 9;
        }
        /// <summary>
        /// 9
        /// </summary>
        public static int Zero = 9;
        /// <summary>
        /// 10
        /// </summary>
        public static int Mine = 10;
        /// <summary>
        /// 11
        /// </summary>
        public static int Flag = 11;
    }
}
