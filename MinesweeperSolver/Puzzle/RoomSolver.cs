using PackMine.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PackMine.Puzzle
{
#if DEBUG
    class RoomSolver
    {
        public RoomSolver(int n)
        {
            this.n = n;
        }
        int n = 5;
        ZMap map;

        int[][] matrix;
        int[][] matrixMap;
        int variableCount;

        public ZMap[] Solve(ZMap map)
        {
            this.map = map;

            BuildMatrix();

            return SolveMatrix();
        }
        
        private void BuildMatrix ()
        {
            matrixMap = new int[n+2][];
            for (int i = 0; i < n+2; i++)
            {
                matrixMap[i] = new int[n+2];
                for (int j = 0; j < n+2; j++)
                {
                    matrixMap[i][j] = -1;
                }
            }

            int k = 0;
            int l = 0;

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    var v = map.Get(i, j);
                    if (v == CellValue.Open || v == CellValue.Flag)
                    {
                        matrixMap[i+1][j+1] = k;
                        k++;
                    }
                    if (CellValue.IsNumber(v))
                    {
                        l++;
                    }
                }
            }

            matrix = new int[l][];
            l = 0;

            variableCount = k;

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    var v = map.Get(i,j);
                    if (CellValue.IsNumber(v))
                    {
                        matrix[l] = new int[k+1];

                        for (int m = 0; m < k; m++)
                        {
                            matrix[l][m] = 0;
                        }
                        for (int ii = 0; ii < 3;ii++ )
                        {
                            for (int jj = 0; jj < 3; jj++)
                            {
                                var m = matrixMap[i+ii][j+jj];
                                if(m >= 0)
                                {
                                    matrix[l][m] = 1;
                                }
                            }
                        }
                        matrix[l][k] = v;
                        l++;
                    }
                }
            }
        }
        private ZMap[] SolveMatrix ()
        {
            var solutions = new List<int[]>();
            var K = variableCount;
            var max = Math.Pow(2,K);
            for(int i =0; i< max; i++)
            {
                var s = SolutionFromIterator(i, K);
                if(CheckSolution(s,matrix))
                {
                    solutions.Add(s);
                }
            }

            var result = new ZMap[solutions.Count];
            for(int k = 0; k<solutions.Count;k++)
            {
                var s = solutions[k];
                result[k] = new ZMap(n,n,(i,j) => f(s,i,j),0);
            }

            return result;
        }

        private int f (int[] s, int i, int j)
        {
            var v = matrixMap[i + 1][j + 1];
            if (v >= 0 && s[v] > 0)
            {
                return 1;
            }
            return 0;
        }

        private int[] SolutionFromIterator (int iterator, int variableCount)
        {
            var result = new int[variableCount];
            for(int i = 0;i<variableCount;i++)
            {
                result[i] = iterator % 2;
                iterator /= 2;
            }
            return result;
        }
        private bool CheckSolution (int[] solution, int[][] mtx)
        {
            for(int i = 0; i< mtx.Length; i++)
            {
                int r = 0;
                for(int j = 0;j <solution.Length; j++)
                {
                    r += solution[j] * mtx[i][j];
                }
                if (r != mtx[i][solution.Length])
                    return false;
            }
            return true;
        }
    }
#endif
}
