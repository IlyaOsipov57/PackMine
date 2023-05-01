using PackMine.Geometry;
using PackMine.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PackMine.Puzzle
{
    class Room
    {
        public Room (ZMap map, ZMap[] solutions)
        {
            this.source = map;
            this.solutions = solutions;
        }
        ZMap source = new ZMap(5, 5, (a, b) =>  CellValue.Open, CellValue.Wall);
        ZMap[] solutions = new ZMap[0];
        public enum RoomState { Unsolvable, Solvable, Solved };
        public static ZPoint FixRoomIndex(ZPoint room)
        {
            return new ZPoint(
                Math.Min(3, Math.Max(0, room.x)),
                Math.Min(3, Math.Max(0, room.y)));
        }
        public static String GetName(ZPoint room)
        {
            return Names[room.y][room.x];
        }
        public bool Clear(ZMap map, ZPoint position)
        {
            if(last == position)
            {
                return false;
            }
            last = new ZPoint(position);
            var p = position / 6;
            {
                var v = metaMap.Get(p);
                if (v == -1 || v == 0)
                {
                    v = metaPattern[p.y][p.x];
                }
                else
                    v = 0;
                metaMap.Set(p, v);
                var pattern = array[v];
                for (int i = 0; i < 5; i++)
                {
                    for (int j = 0; j < 5; j++)
                    {
                        var z1 = new ZPoint(i, j);
                        var z2 = position + z1;
                        map.Set(z2, pattern[j][i]);
                    }
                }
            }
            var correct = 0;
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    var z = new ZPoint(i, j);
                    var v = metaMap.Get(z);
                    if (v == -1)
                    {
                        return false;
                    }
                    if(v!=0)
                    {
                        var c = 0;
                        for(int ii = -1; ii<2;ii++)
                        {
                            for (int jj = -1; jj < 2; jj++)
                            {
                                if (metaMap.Get(i + ii, j + jj) == 0)
                                    c++;
                            }
                        }
                        if (c != v)
                            return false;
                        else
                            correct++;
                    }
                }
            }
            return correct > 0;
        }

        private static String[][] Names = new String[][] {
            new String[] {"Primer","Deja vu","Maze","Cherry on top"},
            new String[] {"Winding path","Deja vu","Wall","Red carpet"},
            new String[] {"Eat the bullet","Crossing","Separated","Free bird"},
            new String[] {"Cornerstone","Left or right?","Possibilities","Dead end"}};
        private static ZPoint last;
        private static int[][] metaPattern = new int[][] {new int[]{1,2,2,1},new int[]{2,2,3,1},new int[]{1,3,4,2},new int[]{2,2,1,2}};
        private static ZMap metaMap = new ZMap(4, 4, (i, j) => -1, -1);
        private static int[][] mine = new int[][] { new int[] { 10, 9, 10, 9, 10 }, new int[] { 9, 10, 10, 10, 9 }, new int[] { 10, 10, 10, 10, 10 }, new int[] { 9, 10, 10, 10, 9 }, new int[] { 10, 9, 10, 9, 10 } };
        private static int[][] one = new int[][] { new int[] { 9, 9, 1, 9, 9 }, new int[] { 9, 1, 1, 9, 9 }, new int[] { 9, 9, 1, 9, 9 }, new int[] { 9, 9, 1, 9, 9 }, new int[] { 9, 1, 1, 1, 9 } };
        private static int[][] two = new int[][] { new int[] { 9, 2, 2, 9, 9 }, new int[] { 9, 9, 9, 2, 9 }, new int[] { 9, 9, 9, 2, 9 }, new int[] { 9, 9, 2, 9, 9 }, new int[] { 9, 2, 2, 2, 9 } };
        private static int[][] three = new int[][] { new int[] { 9, 3, 3, 3, 9 }, new int[] { 9, 9, 9, 3, 9 }, new int[] { 9, 3, 3, 3, 9 }, new int[] { 9, 9, 9, 3, 9 }, new int[] { 9, 3, 3, 3, 9 } };
        private static int[][] four = new int[][] { new int[] { 9, 4, 9, 4, 9 }, new int[] { 9, 4, 9, 4, 9 }, new int[] { 9, 4,4,4, 9 }, new int[] { 9, 9, 9, 4, 9 }, new int[] { 9,9,9,4, 9 } };
        //private static int[][] five = new int[][] { new int[] { 9, 5, 5, 5, 9 }, new int[] { 9, 5, 9, 9, 9 }, new int[] { 9, 5, 5, 5, 9 }, new int[] { 9, 9, 9, 5, 9 }, new int[] { 9, 5, 5, 5, 9 } };
        //private static int[][] six = new int[][] { new int[] { 9, 6, 6, 6, 9 }, new int[] { 9, 6, 9, 9, 9 }, new int[] { 9, 6, 6, 6, 9 }, new int[] { 9, 6, 9, 6, 9 }, new int[] { 9, 6, 6, 6, 9 } };
        //private static int[][] seven = new int[][] { new int[] { 9, 7, 7, 7, 9 }, new int[] { 9, 9, 9, 7, 9 }, new int[] { 9, 9, 9, 7, 9 }, new int[] { 9, 9, 7, 9, 9 }, new int[] { 9, 7, 9, 9, 9 } };
        //private static int[][] eight = new int[][] { new int[] { 9, 8, 8, 8, 9 }, new int[] { 9, 8, 9, 8, 9 }, new int[] { 9, 8, 8, 8, 9 }, new int[] { 9, 8, 9, 8, 9 }, new int[] { 9, 8, 8, 8, 9 } };
        //private static int[][] free = new int[][] { new int[] { 9, 9, 9, 9, 9 }, new int[] { 9, 9, 9, 9, 9 }, new int[] { 9, 9, 9, 9, 9 }, new int[] { 9, 9, 9, 9, 9 }, new int[] { 9, 9, 9, 9, 9 } };
        private static int[][][] array = new int[][][] { mine, one, two, three, four};//, five, six, seven, eight, free };
        public bool Verify(ZMap map, ZPoint position)
        {
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    var z1 = new ZPoint(i, j);
                    var z2 = position + z1;
                    var mv = map.Get(z2);
                    var sv = source.Get(z1);
                    if (sv != CellValue.Open && sv != CellValue.Flag && sv != mv)
                        return false;
                }
            }
            return true;
        }
        public void Repair(ZMap map, ZPoint position)
        {
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    var z1 = new ZPoint(i, j);
                    var z2 = position + z1;
                    map.Set(z2, source.Get(z1));
                }
            }
            UpdateFlags(map, position);
        }
        public void UpdateFlags (ZMap map, ZPoint position)
        {
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    var z1 = new ZPoint(i, j);
                    var z2 = position + z1;
                    
                    if(map.Get(z2) != CellValue.Open && map.Get(z2) != CellValue.Flag)
                        continue;
                    map.Set(z2, CellValue.Flag);

                    foreach (var solution in solutions)
                    {
                        if (solution.Get(z1) != 1 && FitSolution(map, position, solution))
                        {
                            map.Set(z2, CellValue.Open);
                            break;
                        }
                    }
                }
            }
        }
        public RoomState GetRoomState (ZMap map, ZPoint position)
        {
            ZMap match = null;
            int solutionNumber = 0;
            foreach(var solution in solutions)
            {
                if(FitSolution(map,position,solution))
                {
                    match = solution;
                    solutionNumber++;
                    if(solutionNumber>1)
                        return RoomState.Solvable;
                }
            }
            if (solutionNumber == 0)
                return RoomState.Unsolvable;
            if (MatchSolution(map, position, match))
                return RoomState.Solved;
            return RoomState.Solvable;
        }
        private bool FitSolution (ZMap map, ZPoint position, ZMap solution)
        {
            for (int i = 0; i < 5;i++ )
            {
                for(int j = 0;j<5;j++)
                {
                    var z1 = new ZPoint(i,j);
                    var z2 = position + z1;
                    if (solution.Get(z1) == 1 && !(map.Get(z2) == CellValue.Open || map.Get(z2) == CellValue.Flag))
                        return false;
                }
            }
            return true;
        }
        private bool MatchSolution(ZMap map, ZPoint position, ZMap solution)
        {
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    var z1 = new ZPoint(i, j);
                    var z2 = position + z1;
                    if (solution.Get(z1) != 1 && (map.Get(z2) == CellValue.Open || map.Get(z2) == CellValue.Flag))
                        return false;
                }
            }
            return true;
        }
#if DEBUG
        public void Solve()
        {
            RoomSolver rs = new RoomSolver(this.source.width);
            solutions = rs.Solve(source);
        }
        public void Save(String filename)
        {
            Loader.SaveToFile(source, solutions, filename);
        }
        internal void Print()
        {
            Console.WriteLine("Solutions:");
            foreach(var solution in solutions)
            {
                for(int i = 0; i< 5; i++)
                {
                    Console.WriteLine(solution.Get(0, i)
                        + " " +
                        solution.Get(1, i)
                        + " " +
                        solution.Get(2, i)
                        + " " +
                        solution.Get(3, i)
                        + " " +
                        solution.Get(4, i)
                        );
                }
                Console.WriteLine();
            }
        }
#endif
    }
}
