using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

//Start Date= 8/11/2013
//Finish Date= 22/11/2013

namespace SecondProjectPSS
{
    class Program
    {
        //Global constants
        public const double probTolerance = ((double).0001);
        public const double clipIncrement = ((double).5);
        public const int emptyCell = -1;
        public const int invalidCell = -2;
        public const int maxIterations = 110;

        //Struc for heach probability used in the probabilistic algorithm
        public struct probability
        {
            public double  prob; //probability
	        public double  lck; //lock
        }

        //Globar vars used in the whole application
        static probability[, ,] probabilities = new probability[9, 9, 9];
        static int  numSolvedCells;	//num cells solved so far
        static int  numCellsToSolve;	//hint counter
        static int assignations=0;
        static int comparisons=0;
        static int exeLines = 0;
        static Stopwatch temporizator;
        static long tTime;

        //Predetermined Solved Sudoku (Matrix)
        static int[,] predSudoku = new int[9, 9] { 
                {9,8,1,2,4,5,6,3,7},
                {5,2,7,9,6,3,4,1,8},
                {4,6,3,7,8,1,2,5,9},
                {7,4,2,8,3,9,5,6,1},
                {8,1,6,4,5,2,7,9,3},
                {3,5,9,6,1,7,8,2,4},
                {6,3,8,1,2,4,9,7,5},
                {1,7,4,5,9,6,3,8,2},
                {2,9,5,3,7,8,1,4,6}
            };

        //Method to print the received Matrix
        static void printMatrix(int[,] pMatrix)
        {
            for (int i = 0; i < 9; i++)
            {
                for (int x = 0; x < 9; x++)
                {
                    if (pMatrix[i, x] != 0 && pMatrix[i, x] != -1)
                    {
                        Console.Write("{0}   ", pMatrix[i, x]);
                    }
                    else
                    {
                        Console.Write("*   ");
                    }
                    if (x == 2 || x == 5)
                    {
                        Console.Write("| ");
                    }
                }
                if (i == 2 || i == 5)
                {
                    Console.WriteLine("\n--------------------------------------");
                }
                else
                {
                    if (i != 8)
                    {
                        Console.WriteLine("\n            |             |");
                    }
                }
            }
        }

        //Method to randomly fill a matrix used in the probabilistic algorithm, fills it with the quantity of numbers received as argument
        static void fillMatrixProb(int numbers, int[,] nMatrix)
        {
            Random cell = new Random();
            int row = 0;
            int column = 0;
            int chosenCell = 0;
            List<int> chosenCells = new List<int>();
            int i = 0;
            while (i < numbers)
            {
                row = cell.Next(0, 9);
                column = cell.Next(0, 9);
                chosenCell = (row * 10) + column;
                if (chosenCells.Contains(chosenCell) == false)
                {
                    nMatrix[row, column] = predSudoku[row, column];
                    chosenCells.Add(chosenCell);
                    i++;
                }
            }
            for (int x = 0; x < 9; x++)
            {
                for (int y = 0; y < 9; y++)
                {
                    if (nMatrix[x, y] == 0)
                    {
                        nMatrix[x, y] = -1;
                    }
                }
            }
        }

        //Method to randomly fill a matrix used in the Backtracking algorithm, fills it with the quantity of numbers received as argument 
        static void fillMatrix(int numbers, int[,] nMatrix, bool[,] bMatrix)
        {
            Random cell = new Random();
            int row = 0;
            int column = 0;
            int chosenCell = 0;
            List<int> chosenCells = new List<int>();
            int i = 0;
            while (i < numbers)
            {
                row = cell.Next(0, 9);
                column = cell.Next(0, 9);
                chosenCell = (row * 10) + column;
                if (chosenCells.Contains(chosenCell) == false)
                {
                    nMatrix[row, column] = predSudoku[row, column];
                    bMatrix[row, column] = true;
                    chosenCells.Add(chosenCell);
                    i++;
                }
            }
        }

        //fills 30 numbers in a matrix
        static void fill30(int[,] backMatrix, bool[,] BMatrix)
        {
            backMatrix[0, 1] = 8;
            backMatrix[0, 7] = 3;
            backMatrix[0, 8] = 7;
            backMatrix[1, 0] = 5;
            backMatrix[1, 1] = 2;
            backMatrix[1, 3] = 9;
            backMatrix[1, 5] = 3;
            backMatrix[1, 8] = 8;
            backMatrix[2, 0] = 4;
            backMatrix[2, 3] = 7;
            backMatrix[2, 7] = 5;
            backMatrix[2, 8] = 9;
            backMatrix[3, 2] = 2;
            backMatrix[3, 3] = 8;
            backMatrix[3, 4] = 3;
            backMatrix[4, 4] = 5;
            backMatrix[4, 8] = 3;
            backMatrix[5, 0] = 3;
            backMatrix[5, 2] = 9;
            backMatrix[5, 3] = 6;
            backMatrix[5, 5] = 7;
            backMatrix[6, 0] = 6;
            backMatrix[6, 3] = 1;
            backMatrix[6, 2] = 8;
            backMatrix[6, 8] = 5;
            backMatrix[7, 5] = 6;
            backMatrix[8, 1] = 9;
            backMatrix[8, 2] = 5;
            backMatrix[8, 3] = 3;
            backMatrix[8, 4] = 7;

            BMatrix[0, 1] = true;
            BMatrix[0, 7] = true;
            BMatrix[0, 8] = true;
            BMatrix[1, 0] = true;
            BMatrix[1, 1] = true;
            BMatrix[1, 3] = true;
            BMatrix[1, 5] = true;
            BMatrix[1, 8] = true;
            BMatrix[2, 0] = true;
            BMatrix[2, 3] = true;
            BMatrix[2, 7] = true;
            BMatrix[2, 8] = true;
            BMatrix[3, 2] = true;
            BMatrix[3, 3] = true;
            BMatrix[3, 4] = true;
            BMatrix[4, 4] = true;
            BMatrix[4, 8] = true;
            BMatrix[5, 0] = true;
            BMatrix[5, 2] = true;
            BMatrix[5, 3] = true;
            BMatrix[5, 5] = true;
            BMatrix[6, 0] = true;
            BMatrix[6, 3] = true;
            BMatrix[6, 2] = true;
            BMatrix[6, 8] = true;
            BMatrix[7, 5] = true;
            BMatrix[8, 1] = true;
            BMatrix[8, 2] = true;
            BMatrix[8, 3] = true;
            BMatrix[8, 4] = true;
        }

        //fills 15 numbers in a matrix
        static void fill15(int[,] backMatrix, bool[,] BMatrix)
        {
            backMatrix[0, 2] = 1;
            backMatrix[0, 4] = 4;
            backMatrix[0, 7] = 3;
            backMatrix[1, 4] = 6;
            backMatrix[1, 5] = 3;
            backMatrix[3, 4] = 3;
            backMatrix[3, 5] = 9;
            backMatrix[3, 7] = 6;
            backMatrix[4, 1] = 1;
            backMatrix[5, 0] = 3;
            backMatrix[5, 1] = 5;
            backMatrix[5, 7] = 2;
            backMatrix[6, 2] = 8;
            backMatrix[8, 0] = 2;
            backMatrix[8, 2] = 5;

            BMatrix[0, 2] = true;
            BMatrix[0, 4] = true;
            BMatrix[0, 7] = true;
            BMatrix[1, 4] = true;
            BMatrix[1, 5] = true;
            BMatrix[3, 4] = true;
            BMatrix[3, 5] = true;
            BMatrix[3, 7] = true;
            BMatrix[4, 1] = true;
            BMatrix[5, 0] = true;
            BMatrix[5, 1] = true;
            BMatrix[5, 7] = true;
            BMatrix[6, 2] = true;
            BMatrix[8, 0] = true;
            BMatrix[8, 2] = true;
        }

        //Method that sets all values of a Matrix to 0
        static void clearMatrix(int[,] nMatrix)
        {
            for (int row = 0; row < 9; row++)
            {
                for (int column = 0; column < 9; column++)
                {
                    nMatrix[row, column] = 0;
                }
            }
        }

        //Method that set all values of a boolean Matrix to false
        static void clearBMatrix(bool[,] nMatrix)
        {
            for (int row = 0; row < 9; row++)
            {
                for (int column = 0; column < 9; column++)
                {
                    nMatrix[row, column] = false;
                }
            }
        }

        //Method that checks if certain number can be placed in a certain cell following the sudoku rules
        static bool available(int num, int row, int column, int[,] Matrix, bool[,] matrixBool)
        {
            int pos = Matrix[row, column]; assignations++; exeLines++;
            for (int i = 0; i < 9; i++)
            {
                assignations++; comparisons++; exeLines++;
                exeLines++;
                if (i > column)
                {
                    exeLines++;
                    if (Matrix[row, i] == num && matrixBool[row, i] == true)
                    {
                        exeLines++;
                        return false;
                    } comparisons++;
                } comparisons++;
                exeLines++;

                if (Matrix[row, i] == num && (i < column))
                {
                    exeLines++;
                    return false;
                } comparisons++;

            } assignations++; comparisons++;

            for (int x = 0; x < 9; x++)
            {
                assignations++; comparisons++; exeLines++;
                exeLines++;
                if (x > row)
                {
                    exeLines++;
                    if (Matrix[x, column] == num && matrixBool[x, column] == true)
                    {
                        exeLines++;
                        return false;
                    } comparisons++;
                } comparisons++;
                exeLines++;
                if (Matrix[x, column] == num && (x < row))
                {
                    exeLines++;
                    return false;
                } comparisons++;
            } assignations++; comparisons++;
            exeLines++;
            if (row == 0 || row == 3 || row == 6)
            {
                exeLines += 3;
                int i = 0; assignations++;
                int a = 0; assignations++;
                for (int x = row; x < 9; x++)
                {
                    exeLines+=2;
                    assignations++; comparisons++;
                    if (a < 3)
                    {
                        exeLines++;
                        if (column == 0 || column == 3 || column == 6)
                        {
                            exeLines++;
                            for (int y = column; y < 9; y++)
                            {
                                exeLines += 2;
                                assignations++; comparisons++;
                                if (i < 3)
                                {
                                    exeLines++;
                                    if (x > row || y > column)
                                    {
                                        exeLines++;
                                        if (Matrix[x, y] == num && matrixBool[x, y] == true)
                                        {
                                            exeLines++;
                                            return false;
                                        } comparisons++;
                                    } comparisons++;
                                    exeLines++;
                                    if (Matrix[x, y] == num && (x < row && y < column))
                                    {
                                        exeLines++;
                                        return false;
                                    } comparisons++;
                                } comparisons++;
                                i++; assignations++; exeLines++;
                            } comparisons++;
                            a++; assignations++; exeLines++;
                        } comparisons++;
                        exeLines++;
                        if (column == 1 || column == 4 || column == 7)
                        {
                            exeLines++;
                            for (int y = column - 1; y < 9; y++)
                            {
                                exeLines += 2;
                                assignations++; comparisons++;
                                if (i < 3)
                                {
                                    exeLines++;
                                    if (x > row || y > column)
                                    {
                                        exeLines++;
                                        if (Matrix[x, y] == num && matrixBool[x, y] == true)
                                        {
                                            exeLines++;
                                            return false;
                                        } comparisons++;
                                    } comparisons++;
                                    exeLines++;
                                    if (Matrix[x, y] == num && (x < row && y < column))
                                    {
                                        exeLines++;
                                        return false;
                                    } comparisons++;
                                } comparisons++;
                                i++; exeLines++;
                            } assignations++; comparisons++;
                            a++; exeLines++;
                        } comparisons++;
                        exeLines++;
                        if (column == 2 || column == 5 || column == 8)
                        {
                            exeLines++;
                            for (int y = column - 2; y < 9; y++)
                            {
                                assignations++; comparisons++; exeLines+=2;
                                if (i < 3)
                                {
                                    exeLines++;
                                    if (x > row || y > column)
                                    {
                                        exeLines++;
                                        if (Matrix[x, y] == num && matrixBool[x, y] == true)
                                        {
                                            exeLines++;
                                            return false;
                                        } comparisons++;
                                    } comparisons++;
                                    exeLines++;
                                    if (Matrix[x, y] == num && (x < row && y < column))
                                    {
                                        exeLines++;
                                        return false;
                                    } comparisons++;
                                } comparisons++;
                                i++; exeLines++;
                            } assignations++; comparisons++;
                            a++; exeLines++;
                        } comparisons++;
                    } comparisons++;
                    i = 0; exeLines++;
                } comparisons++; assignations++;
            } comparisons++;
            exeLines++;
            if (row == 1 || row == 4 || row == 7)
            {
                exeLines += 3;
                int i = 0; assignations++;
                int a = 0; assignations++;
                for (int x = row - 1; x < 9; x++)
                {
                    exeLines += 2;
                    assignations++; comparisons++;
                    if (a < 3)
                    {
                        exeLines++;
                        if (column == 0 || column == 3 || column == 6)
                        {
                            exeLines++;
                            for (int y = column; y < 9; y++)
                            {
                                assignations++; comparisons++; exeLines++;
                                if (i < 3)
                                {
                                    exeLines++;
                                    if (x > row || y > column)
                                    {
                                        exeLines++;
                                        if (Matrix[x, y] == num && matrixBool[x, y] == true)
                                        {
                                            exeLines++;
                                            return false;
                                        } comparisons++;
                                    } comparisons++;
                                    exeLines++;
                                    if (Matrix[x, y] == num && (x < row && y < column))
                                    {
                                        exeLines++;
                                        return false;
                                    } comparisons++;
                                } comparisons++;
                                i++; exeLines++;
                            } assignations++; comparisons++;
                            a++; exeLines++;
                        } comparisons++;
                        exeLines++;
                        if (column == 1 || column == 4 || column == 7)
                        {
                            exeLines++;
                            for (int y = column - 1; y < 9; y++)
                            {
                                assignations++; comparisons++; exeLines+=2;
                                if (i < 3)
                                {
                                    exeLines++;
                                    if (x > row || y > column)
                                    {
                                        exeLines++;
                                        if (Matrix[x, y] == num && matrixBool[x, y] == true)
                                        {
                                            exeLines++;
                                            return false;
                                        } comparisons++;
                                    } comparisons++;
                                    exeLines++;
                                    if (Matrix[x, y] == num && (x < row && y < column))
                                    {
                                        exeLines++; ;
                                        return false;
                                    } comparisons++;
                                } comparisons++;
                                i++; exeLines++;
                            } assignations++; comparisons++;
                            a++; exeLines++;
                        } comparisons++;
                        exeLines++;
                        if (column == 2 || column == 5 || column == 8)
                        {
                            exeLines++;
                            for (int y = column - 2; y < 9; y++)
                            {
                                exeLines += 2;
                                assignations++;
                                comparisons++;
                                if (i < 3)
                                {
                                    exeLines++;
                                    if (x > row || y > column)
                                    {
                                        exeLines++;
                                        if (Matrix[x, y] == num && matrixBool[x, y] == true)
                                        {
                                            exeLines++;
                                            return false;
                                        } comparisons++;
                                    } comparisons++;
                                    exeLines++;
                                    if (Matrix[x, y] == num && (x < row && y < column))
                                    {
                                        exeLines++;
                                        return false;
                                    } comparisons++;
                                } comparisons++;
                                i++; exeLines++;
                            } assignations++; comparisons++;
                            a++; exeLines++;
                        } comparisons++;
                    } comparisons++;
                    i = 0; exeLines++;
                } assignations++; comparisons++;
            }
            exeLines++;
            if (row == 2 || row == 5 || row == 8)
            {
                int i = 0; assignations++; exeLines++;
                int a = 0; assignations++; exeLines++;
                exeLines++;
                for (int x = row - 2; x < 9; x++)
                {
                    exeLines += 2;
                    assignations++; comparisons++;
                    if (a < 3)
                    {
                        exeLines++;
                        if (column == 0 || column == 3 || column == 6)
                        {
                            exeLines++;
                            for (int y = column; y < 9; y++)
                            {
                                exeLines += 2;
                                assignations++; comparisons++;
                                if (i < 3)
                                {
                                    exeLines++;
                                    if (x > row || y > column)
                                    {
                                        exeLines++;
                                        if (Matrix[x, y] == num && matrixBool[x, y] == true)
                                        {
                                            exeLines++;
                                            return false;
                                        } comparisons++;
                                    } comparisons++;
                                    exeLines++;
                                    if (Matrix[x, y] == num && (x < row && y < column))
                                    {
                                        exeLines++;
                                        return false;
                                    } comparisons++;
                                } comparisons++;
                                i++; exeLines++;
                            } assignations++; comparisons++;
                            a++; exeLines++;
                        } comparisons++;
                        exeLines++;
                        if (column == 1 || column == 4 || column == 7)
                        {
                            exeLines++;
                            for (int y = column - 1; y < 9; y++)
                            {
                                exeLines += 2;
                                assignations++; comparisons++;
                                if (i < 3)
                                {
                                    exeLines++;
                                    if (x > row || y > column)
                                    {
                                        exeLines++;
                                        if (Matrix[x, y] == num && matrixBool[x, y] == true)
                                        {
                                            exeLines++;
                                            return false;
                                        } comparisons++;
                                    } comparisons++;
                                    exeLines++;
                                    if (Matrix[x, y] == num && (x < row && y < column))
                                    {
                                        exeLines++;
                                        return false;
                                    } comparisons++;
                                } comparisons++;
                                i++; exeLines++;
                            } assignations++; comparisons++;
                            a++; exeLines++;
                        } comparisons++;
                        exeLines++;
                        if (column == 2 || column == 5 || column == 8)
                        {
                            exeLines++;
                            for (int y = column - 2; y < 9; y++)
                            {
                                exeLines += 2;
                                assignations++; comparisons++;
                                if (i < 3)
                                {
                                    exeLines++;
                                    if (x > row || y > column)
                                    {
                                        exeLines++;
                                        if (Matrix[x, y] == num && matrixBool[x, y] == true)
                                        {
                                            exeLines++;
                                            return false;
                                        } comparisons++;
                                    } comparisons++;
                                    exeLines++;
                                    if (Matrix[x, y] == num && (x < row && y < column))
                                    {
                                        exeLines++;
                                        return false;
                                    } comparisons++;
                                } comparisons++;
                                i++; exeLines++;
                            } assignations++; comparisons++;
                            a++; exeLines++;
                        } comparisons++;
                    } comparisons++;
                    i = 0; exeLines++;
                } assignations++; comparisons++;
            } comparisons++;
            exeLines++;
            return true;
        }

        //Method that solves the sudoku using backtracking algorithm, fills every cell with the lowest number avaliable
        static void backtrackingSudoku(int row, int column, int[,] pMatrix, bool[,] bMatrix, int number, bool backStep)
        {
            comparisons++; exeLines++;
            if (number == 10)
            {
                comparisons++; exeLines++;
                if (column > 0)
                {
                    exeLines += 2;
                    backtrackingSudoku(row, column - 1, pMatrix, bMatrix, (pMatrix[row, column - 1]) + 1, true);
                    return;
                }
                else
                {
                    exeLines += 2;
                    backtrackingSudoku(row - 1, 8, pMatrix, bMatrix, (pMatrix[row - 1, 8]) + 1, true);
                    return;
                }
            }
            else
            {
                exeLines+=3;
                int number2 = number; assignations++;
                int column1 = column; assignations++;
                for (int row1 = row; row1 < 9; row1++)
                {
                    exeLines += 2;
                    assignations++; comparisons++;
                    if (column1 == 9)
                    {
                        exeLines++;
                        column1 = 0;
                    } comparisons++;
                    exeLines++;
                    while (column1 < 9)
                    {
                        exeLines += 3;
                        comparisons++;
                        bool placed = false; assignations++; comparisons++;
                        if (bMatrix[row1, column1] == false)
                        {
                            exeLines += 3;
                            backStep = false; assignations++;
                            while (number2 < 10)
                            {
                                exeLines += 2;
                                comparisons++;
                                comparisons++;
                                if (available(number2, row1, column1, pMatrix, bMatrix) == true)
                                {
                                    exeLines += 7;
                                    pMatrix[row1, column1] = number2; assignations++;
                                    Thread.Sleep(50);
                                    Console.Clear();
                                    printMatrix(pMatrix);
                                    placed = true; assignations++;
                                    number2 = 1; assignations++;
                                    break;
                                }
                                else
                                {
                                    exeLines++;
                                    number2++;
                                }
                            }
                            comparisons++; exeLines++;
                            if (placed == false)
                            {
                                comparisons++; exeLines++;
                                if (column1 > 0)
                                {
                                    exeLines += 2;
                                    backtrackingSudoku(row1, column1 - 1, pMatrix, bMatrix, (pMatrix[row1, column1 - 1]) + 1, true);
                                    return;
                                }
                                else
                                {
                                    exeLines += 3;
                                    backtrackingSudoku(row1 - 1, 8, pMatrix, bMatrix, (pMatrix[row1 - 1, 8]) + 1, true);
                                    return;
                                }
                            } comparisons++;
                        }

                        else
                        {
                            exeLines += 2;
                            if (backStep == true)
                            {
                                comparisons++; exeLines++;
                                if (column1 > 0)
                                {
                                    exeLines += 2;
                                    backtrackingSudoku(row1, column1 - 1, pMatrix, bMatrix, (pMatrix[row1, column1 - 1]) + 1, true);
                                    return;
                                }
                                else
                                {
                                    exeLines += 3;
                                    backtrackingSudoku(row1 - 1, 8, pMatrix, bMatrix, (pMatrix[row1 - 1, 8]) + 1, true);
                                    return;
                                }
                            } comparisons++;
                        }
                        column1++; assignations++;
                    } comparisons++;
                } assignations++; comparisons++;
                exeLines++;
                return;
            }
        }

        //Simple method used in the probabilistic algorithm, more like a macro
        static double myMax(double a, double b)
        {
            comparisons++;
            exeLines += 2;
            if (a > b)
            {
                return a;
            }
            else
            {
                return b;
            }
        }

        //Simple method used in the probabilistic algorithm, more like a macro
        static double myAbs( double a ) 
        {
            comparisons++;
            exeLines += 2;
            if (a >= 0)
            {
                return a;
            }
            else
            {
                return -a;
            }
        }

        //Method that chooses the highest probalities and lock them up for later asignation
        static double setCell(int row, int col, int ch, double pr)
        {
	        //round probabilities close to 0.0 and 1.0
            double p = ((pr < probTolerance) ? 0.0 : ((pr > 1.0) ? 1.0 : pr)); assignations++; comparisons += 2;
	        //set probability, and lock if 0.0 or 1.0
            probabilities[row, col, ch].prob = p; assignations++;
            probabilities[row, col, ch].lck = (p == 0.0 || p == 1.0) ? 1 : 0; comparisons++; assignations++; exeLines += 4;

	        if (p == 1.0) //a new cell was solved
	        {
                ++numSolvedCells; assignations++;
                --numCellsToSolve; assignations++;
                exeLines += 2;
            } comparisons++;
            exeLines++;
	        return(p);
        }

        //Method that initialize the probabilities for each cell and save them in a global array
        static double initiateCell(int row, int col, int ch)
        {
            int i, j, rr, cc; assignations += 4;
            exeLines += 2;
            if (probabilities[row, col, ch].prob == 1.0)
            {
                exeLines++;
                return 1.0;
            } comparisons++;
            exeLines++;
            if (probabilities[row, col, ch].prob == 0.0)
            {
                exeLines++;
                goto fail;
            } comparisons++;
            exeLines++;
	        for (i=0; i<9; i++) 
	        {
                exeLines += 2;
                assignations++;
                comparisons++;
                if ((probabilities[row, col, i].prob == 1.0) || (probabilities[row, i, ch].prob == 1.0) || (probabilities[i, col, ch].prob == 1.0))
                {
                    exeLines++;
                    goto fail;
                } comparisons++;
                exeLines += 3;
		        setCell(row,col,i,0.0);	//clear (and lock) conflicting cell probs
		        setCell(row,i,ch,0.0);	//clear (and lock) conflicting row probs
		        setCell(i,col,ch,0.0);	//clear (and lock) conflicting column probs
            } comparisons++;

	        //clear(and lock) conflicting region probs
            rr = 3 * (row / 3); assignations++;
            cc = 3 * (col / 3); assignations++; exeLines += 3;
            for (i = rr; i < rr + 3; i++)
            {
                comparisons++; assignations++; exeLines += 2;
                for (j = cc; j < cc + 3; j++)
                {
                    exeLines += 2;
                    comparisons++; assignations++;
                    if (probabilities[i, j, ch].prob == 1.0)
                    {
                        exeLines++;
                        goto fail;
                    } comparisons++;
                    setCell(i, j, ch, 0.0); exeLines++;
                } assignations++; comparisons++;
            } assignations++; comparisons++;
            exeLines++;
	        return(setCell(row,col,ch,1.0)); //set (and lock) prob to 1.0

            fail: exeLines++; return -1.0;
        }

        //Method that checks if the probalities obtained follow the rules of the sudoku
        static int validateProbs()
        {
            int i, j, k, ri, rj, n0, n1; assignations += 7;
            exeLines += 2;
            //check cell groups
            for (i = 0; i < 9; i++)
            {
                exeLines += 2;
                comparisons++; assignations++;
                for (j = 0; j < 9; j++)
                {
                    exeLines += 2;
                    comparisons++; assignations++;
                    for (n0 = n1 = k = 0; k < 9; k++)
                    {
                        exeLines += 2;
                        assignations++; comparisons++;
                        comparisons++;
                        if (probabilities[i, j, k].prob < 0.05)
                        {
                            exeLines++;
                            ++n0;
                        }
                        else if (probabilities[i, j, k].prob > 0.99)
                        {
                            ++n1; exeLines++;
                        }
                    }comparisons++;
                    exeLines++;
                    if (n0 != 8 || n1 != 1)
                    {
                        exeLines++;
                        goto invalid; 
                    } comparisons++;
                }comparisons++;
            }comparisons++;
            //check row groups 
            exeLines++;
            for (i = 0; i < 9; i++)
            {
                assignations++; comparisons++; exeLines += 2;
                for (k = 0; k < 9; k++)
                {
                    assignations++; comparisons++; exeLines += 2;
                    for (n0 = n1 = j = 0; j < 9; j++)
                    {
                        assignations++; comparisons++; exeLines += 2;
                        comparisons++;
                        if (probabilities[i, j, k].prob < 0.05)
                        {
                            ++n0; exeLines++;
                        }
                        else if (probabilities[i, j, k].prob > 0.99)
                        {
                            ++n1; exeLines++;
                        }
                    }comparisons++;
                    if (n0 != 8 || n1 != 1)
                    {
                        exeLines++;
                        goto invalid;
                    }
                } comparisons++;
            } comparisons++;

            //check col groups 
            exeLines++;
            for (j = 0; j < 9; j++)
            {
                assignations++; comparisons++; exeLines += 2;
                for (k = 0; k < 9; k++)
                {
                    assignations++; comparisons++; exeLines += 2;
                    for (n0 = n1 = i = 0; i < 9; i++)
                    {
                        assignations++; comparisons++;
                        comparisons++;
                        if (probabilities[i, j, k].prob < 0.05)
                        {
                            ++n0; exeLines++;
                        }
                        else if (probabilities[i, j, k].prob > 0.99)
                        {
                            ++n1; exeLines++;
                        }
                    } comparisons++;
                    exeLines++;
                    if (n0 != 8 || n1 != 1)
                    {
                        exeLines++;
                        goto invalid;
                    } comparisons++;
                } comparisons++;
            } comparisons++;

            //check row groups
            exeLines++;
            for (k = 0; k < 9; k++)
            {
                assignations++; comparisons++; exeLines += 2;
                for (ri = 0; ri < 3; ri++)
                {
                    assignations++; comparisons++; exeLines += 2;
                    for (rj = 0; rj < 3; rj++)
                    {
                        assignations++; comparisons++; exeLines += 2;
                        for (n0 = n1 = i = 0; i < 3; i++)
                        {
                            assignations++; comparisons++; exeLines += 2;
                            for (j = 0; j < 3; j++)
                            {
                                assignations++; comparisons++; exeLines += 2;
                                comparisons++;
                                if (probabilities[3 * ri + i, 3 * rj + j, k].prob < 0.05)
                                {
                                    ++n0; exeLines++;
                                }
                                else if (probabilities[3 * ri + i, 3 * rj + j, k].prob > 0.99)
                                {
                                    ++n1; exeLines++;
                                }
                            } comparisons++;
                        } comparisons++;
                        exeLines++;
                        if (n0 != 8 || n1 != 1)
                        {
                            exeLines++;
                            goto invalid;
                        } comparisons++;
                    } comparisons++;
                } comparisons++;
            } comparisons++;
            exeLines++;
            return 1; //all groups are valid

            invalid: exeLines++; return 0; //some group is invalid
        }

        //Method that asign the highest prob for each cell
        static int getProbs(int[,] Sudoku) 
        {
            int i, j, k; assignations += 3;

            if (numSolvedCells == 81 && validateProbs()==0)
            {
                return 0;
            } comparisons++;

            for (i = 0; i < 9; i++)
            {
                assignations++; comparisons++;
                for (j = 0; j < 9; j++)
                {
                    assignations++; comparisons++;
                    Sudoku[i, j] = emptyCell; assignations++;
                    Thread.Sleep(100);
                    Console.Clear();
                    printMatrix(Sudoku);
                } comparisons++;
            } comparisons++;

            for (i = 0; i < 9; i++)
            {
                assignations++; comparisons++;
                for (j = 0; j < 9; j++)
                {
                    assignations++; comparisons++;
                    for (k = 0; k < 9; k++)
                    {
                        assignations++; comparisons++;
                        if (probabilities[i, j, k].prob == 1.0)
                        {
                            Sudoku[i, j] = (Sudoku[i, j] == emptyCell) ? k + 1 : invalidCell; assignations++;
                            Thread.Sleep(100);
                            Console.Clear();
                            printMatrix(Sudoku);
                        } comparisons++;
                    } comparisons++;
                } comparisons++;
            } comparisons++;
	        return 1;
        }

        //Method that initialize the global probabilities (1-9) for each cell of the grid
        static int initProbs(int[,] Sudoku) 
        {
            int i, j, k, n0, n1, ch; assignations += 5;
            double t, p; assignations += 2;
            double[,] celltotals = new double[9, 9]; assignations++; exeLines += 4;
	
	        //initialize probs to -1.0 and clear locks
            for (i = 0; i < 9; i++)
            {
                assignations++; comparisons++; exeLines += 2;
                for (j = 0; j < 9; j++)
                {
                    assignations++; comparisons++; exeLines += 2;
                    for (k = 0; k < 9; k++)
                    {
                        assignations++; comparisons++;
                        probabilities[i, j, k].prob = -1.0; assignations++;
                        probabilities[i, j, k].lck = 0; assignations++; exeLines += 2;
                    }
                }
            }
            numSolvedCells = 0; assignations++; exeLines++;
	
	        //initialize probs according to input puzzle
            exeLines++;
            for (i = 0; i < 9; i++)
            {
                assignations++; comparisons++; exeLines++;
                for (j = 0; j < 9; j++)
                {
                    assignations++; comparisons++; exeLines += 2;
                    ch = Sudoku[i, j]; assignations++;
                    if (ch != emptyCell)
                    {
                        exeLines++;
                        if (initiateCell(i, j, ch - 1) != 1.0)
                        {
                            exeLines++;
                            return (-1);
                        } comparisons++;
                    } comparisons++;
                } comparisons++;
            } comparisons++;

	        //set remaining probs to normalize total char probs (to 9.0)
            exeLines++;
	        for (k=0; k<9; k++)
	        {
                assignations++; comparisons++; exeLines += 2;
                for (n0 = n1 = i = 0; i < 9; i++)
                {
                    assignations++; comparisons++; exeLines += 2;
                    for (j = 0; j < 9; j++)
                    {
                        assignations++; comparisons++;
                        comparisons++; exeLines++;
                        if (probabilities[i, j, k].prob == 0.0)
                        {
                            ++n0; exeLines++;
                        }
                        else if (probabilities[i, j, k].prob == 1.0)
                        {
                            ++n1; exeLines++;
                        }
                    } comparisons++;
                } comparisons++;
                exeLines++;
		        if (n0+n1 != 81)
		        {
                    p = (9.0 - n1) / (81 - (n0 + n1)); assignations++;
                    exeLines += 2;
                    for (i = 0; i < 9; i++)
                    {
                        assignations++; comparisons++; exeLines += 2;
                        for (j = 0; j < 9; j++)
                        {
                            assignations++; comparisons++;
                            if (probabilities[i, j, k].prob == -1.0)
                            {
                                probabilities[i, j, k].prob = p; exeLines++;
                            } comparisons++;
                        } comparisons++;
                    } comparisons++;
                } comparisons++;
            } comparisons++;

	        //normalize cell probs (to 1.0) 
            exeLines++;
            for (i = 0; i < 9; i++)
            {
                assignations++; comparisons++; exeLines += 2;
                for (j = 0; j < 9; j++)
                {
                    assignations++; comparisons++;
                    for (t = k = 0; k < 9; k++) t += probabilities[i, j, k].prob; comparisons++;
                    celltotals[i, j] = t; comparisons++; exeLines++;
                } comparisons++;
            } comparisons++;
            exeLines++;
            for (i = 0; i < 9; i++)
            {
                assignations++; comparisons++; exeLines += 2;
                for (j = 0; j < 9; j++)
                {
                    assignations++; comparisons++; exeLines += 2;
                    for (k = 0; k < 9; k++)
                    {
                        assignations++; comparisons++; exeLines += 2;
                        if (probabilities[i, j, k].lck == 0)
                        {
                            probabilities[i, j, k].prob /= celltotals[i, j]; assignations++; exeLines++;
                        } comparisons++;
                    } comparisons++;
                } comparisons++;
            } comparisons++;
            exeLines++;
	        return 0;
        }

        //Method that evolve the probs obtained, increasing or decreasing the chance of every prob
        static void evolveProbs(double tol) 
        {
            double a, p, mpd; assignations += 3;
            int cellocks, rowlocks, collocks, reglocks; assignations += 4;
            int i, j, k, ri, rj, rr, cc, n; assignations += 8;

            double[,] celltotals = new double[9, 9]; assignations++;
            double[,] rowtotals = new double[9, 9]; assignations++;
            double[,] coltotals = new double[9, 9]; assignations++;
            double[, ,] regtotals = new double[3, 3, 9]; assignations++;
            exeLines += 8;
	        do
	        {
		        //calculate cell group total probs
                exeLines++;
                for (i = 0; i < 9; i++)
                {
                    assignations++; comparisons++; exeLines += 2;
                    for (j = 0; j < 9; j++)
                    {
                        assignations++; comparisons++; exeLines += 2;
                        for (a = k = 0; k < 9; k++) a += probabilities[i, j, k].prob; assignations++;
                        celltotals[i, j] = a; assignations++; exeLines++;
                    } comparisons++;
                } comparisons++;

		        //calculate row group total probs
                exeLines++;
                for (i = 0; i < 9; i++)
                {
                    assignations++; comparisons++; exeLines += 2;
                    for (k = 0; k < 9; k++)
                    {
                        assignations++; comparisons++; exeLines += 2;
                        for (a = j = 0; j < 9; j++) a += probabilities[i, j, k].prob; assignations++;
                        rowtotals[i, k] = a; assignations++; exeLines++;
                    } comparisons++;
                } comparisons++;

		        //calculate col group total probs
                exeLines++;
                for (j = 0; j < 9; j++)
                {
                    assignations++; comparisons++; exeLines += 2;
                    for (k = 0; k < 9; k++)
                    {
                        assignations++; comparisons++; exeLines += 2;
                        for (a = i = 0; i < 9; i++) a += probabilities[i, j, k].prob; assignations++;
                        coltotals[j, k] = a; assignations++; exeLines++;
                    } comparisons++;
                } comparisons++;

		        //calculate reg group total probs
                exeLines++;
                for (k = 0; k < 9; k++)
                {
                    assignations++; comparisons++; exeLines += 2;
                    for (ri = 0; ri < 3; ri++)
                    {
                        assignations++; comparisons++; exeLines += 2;
                        for (rj = 0; rj < 3; rj++)
                        {
                            assignations++; comparisons++; exeLines += 2;
                            for (a = i = 0; i < 3; i++)
                            {
                                assignations++; comparisons++; exeLines += 2;
                                for (j = 0; j < 3; j++)
                                {
                                    assignations++; comparisons++;
                                    a += probabilities[3 * ri + i, 3 * rj + j, k].prob; exeLines++;
                                } comparisons++;
                            } comparisons++;
                            regtotals[ri, rj, k] = a; assignations++; exeLines++;
                        } comparisons++;
                    } comparisons++;
                } comparisons++;

		        //update probs
                exeLines++;
                for (mpd = i = 0; i < 9; i++)
                {
                    assignations++; comparisons++; exeLines += 2;
                    for (j = 0; j < 9; j++)
                    {
                        assignations++; comparisons++; exeLines += 2;
                        for (k = 0; k < 9; k++)
                        {
                            assignations++; comparisons++; exeLines += 2;
                            if (probabilities[i, j, k].lck == 0) //don't update if locked
                            {
                                //count locked probs in related groups
                                cellocks = rowlocks = collocks = reglocks = 1; assignations++; exeLines += 2;
                                for (n = 0; n < 9; ++n)
                                {
                                    assignations++; comparisons++;
                                    if (probabilities[i, j, n].lck == 1) ++cellocks;
                                    if (probabilities[i, n, k].lck == 1) ++rowlocks;
                                    if (probabilities[n, j, k].lck == 1) ++collocks; exeLines += 3;
                                } comparisons++;

                                rr = 3 * (i / 3); cc = 3 * (j / 3); assignations++; exeLines += 2;
                                for (ri = rr; ri < rr + 3; ri++)
                                {
                                    assignations++; comparisons++; exeLines += 2;
                                    for (rj = cc; rj < cc + 3; rj++)
                                    {
                                        assignations++; comparisons++; exeLines++;
                                        if (probabilities[ri, rj, k].lck == 1)
                                        {
                                            ++reglocks; exeLines++;
                                        }
                                    } comparisons++;
                                } comparisons++;

                                a = ((cellocks / celltotals[i, j]) +
                                    (rowlocks / rowtotals[i, k]) +
                                    (collocks / coltotals[j, k]) +
                                    (reglocks / regtotals[i / 3, j / 3, k])) / (cellocks + rowlocks + collocks + reglocks); assignations++;

                                p = probabilities[i, j, k].prob; assignations++;

                                mpd = myMax(mpd, myAbs(p - setCell(i, j, k, a * p)));
                                exeLines += 3;
                            } comparisons++;
                        } comparisons++;
                    } comparisons++;
                } comparisons++;    
	        } while(mpd>tol);
        }

        //Method That locks a probability that is definitive, cannot be changed
        static void clipProb() 
        {
            int i, j, k, n, u, f, rr, cc, ri, rj; assignations += 10;
            double mp, p, dp; assignations += 3;
            exeLines += 3;
            for (mp = i = 0; i < 9; i++)
            {
                assignations++; comparisons++; exeLines += 2;
                for (j = 0; j < 9; j++)
                {
                    assignations++; comparisons++; exeLines += 2;
                    for (k = 0; k < 9; k++)
                    {
                        assignations++; comparisons++;
                        p = probabilities[i, j, k].prob; assignations++; exeLines += 2;
                        if (p > 0.0 && p < 1.0)
                        {
                            mp = myMax(mp, p); exeLines++;
                        } comparisons++;
                    } comparisons++;
                } comparisons++;
            } comparisons++;
            exeLines++;
            for (i = 0; i < 9; i++)
            {
                assignations++; comparisons++; exeLines += 2;
                for (j = 0; j < 9; j++)
                {
                    assignations++; comparisons++; exeLines += 2;
                    for (k = 0; k < 9; k++)
                    {
                        assignations++; comparisons++;
                        if (probabilities[i, j, k].prob == mp)
                        {
                            rr = 3 * (i / 3); cc = 3 * (j / 3); assignations++; exeLines += 4;

                            f = 0; assignations++;//don't promote probs that should eventually be 0.0
                            for (n = 0; n < 9; ++n)
                            {
                                assignations++; comparisons++; exeLines++;
                                if ((probabilities[i, j, n].prob == 1.0 && probabilities[i, j, n].lck == 1) ||
                                     (probabilities[i, n, k].prob == 1.0 && probabilities[i, n, k].lck == 1) ||
                                     (probabilities[n, j, k].prob == 1.0 && probabilities[n, j, k].lck == 1))
                                {
                                    exeLines++;
                                    goto nextk;
                                } comparisons++;
                            } comparisons++;
                            exeLines++;
                            for (ri = rr; ri < rr + 3; ri++)
                            {
                                assignations++; comparisons++; exeLines += 2;
                                for (rj = cc; rj < cc + 3; rj++)
                                {
                                    assignations++; comparisons++; exeLines += 2;
                                    if (probabilities[ri, rj, k].prob == 1.0 && probabilities[ri, rj, k].lck == 1.0)
                                    {
                                        exeLines++;
                                        goto nextk;
                                    } comparisons++;
                                } comparisons++;
                            } comparisons++;

                            //count and mark (lck = -1) all unlocked cells in related groups
                            exeLines++;
                            for (u = n = 0; n < 9; ++n)
                            {
                                assignations++; comparisons++; exeLines += 2;
                                if (probabilities[i, j, n].lck == 0) 
                                {
                                    ++u; assignations++;
                                    probabilities[i, j, n].lck = -1; assignations++; exeLines += 2;
                                } comparisons++;
                                exeLines++;
                                if (probabilities[i, n, k].lck == 0) 
                                {
                                    ++u; assignations++;
                                    probabilities[i, n, k].lck = -1; assignations++; exeLines += 2;
                                } comparisons++;
                                exeLines++;
                                if (probabilities[n, j, k].lck == 0) 
                                {
                                    ++u; assignations++;
                                    probabilities[n, j, k].lck = -1; assignations++; exeLines += 2;
                                } comparisons++;
                            } comparisons++;
                            exeLines++;
                            for (ri = rr; ri < rr + 3; ri++)
                            {
                                assignations++; comparisons++; exeLines += 2;
                                for (rj = cc; rj < cc + 3; rj++)
                                {
                                    assignations++; comparisons++; exeLines += 2;
                                    if (probabilities[ri, rj, k].lck == 0)
                                    {
                                        ++u; assignations++;
                                        probabilities[ri, rj, k].lck = -1; assignations++; exeLines += 2;
                                    } comparisons++;
                                } comparisons++;
                            } comparisons++;

                            //increment and unmark this cell
                            p = setCell(i, j, k, mp + clipIncrement); assignations++; exeLines++;

                            //decrement and unmark all unlocked cells in related groups
                            dp = (p - mp) / u; assignations++; exeLines+=2;

                            for (n = 0; n < 9; ++n)
                            {
                                assignations++; comparisons++; exeLines += 2;
                                if (probabilities[i, j, n].lck == -1)
                                {
                                    setCell(i, j, n, probabilities[i, j, n].prob - dp); exeLines++;
                                } comparisons++;
                                exeLines++;
                                if (probabilities[i, n, k].lck == -1)
                                {
                                    setCell(i, n, k, probabilities[i, n, k].prob - dp); exeLines++;
                                } comparisons++;
                                exeLines++;
                                if (probabilities[n, j, k].lck == -1)
                                {
                                    setCell(n, j, k, probabilities[n, j, k].prob - dp); exeLines++;
                                } comparisons++;
                            } comparisons++;
                            exeLines++;
                            for (ri = rr; ri < rr + 3; ri++)
                            {
                                assignations++; comparisons++; exeLines += 2;
                                for (rj = cc; rj < cc + 3; rj++)
                                {
                                    assignations++; comparisons++; exeLines += 2;
                                    if (probabilities[ri, rj, k].lck == -1)
                                    {
                                        setCell(ri, rj, k, probabilities[ri, rj, k].prob - dp); exeLines++;
                                    } comparisons++;
                                } comparisons++;
                            } comparisons++;
                            exeLines++;
                            return;
                        } comparisons++;
                    nextk: ;
                    //if we get here that means that the highest probability (of a digit in a cell) is impossible,
                    //which implies that somewhere down the line the wrong digit was locked into a cell, and now the 
                    //puzzle can not be solved! This would be the time to backtrack.
                    } comparisons++;
                } comparisons++;
            } comparisons++;
        }

        //General Probabilistic method that calls the necesary functions while the number of solved cells is bigger than 0
        static int solveSudoku(int[,] Sudoku, int n)
        {
            int k; assignations++; exeLines+=2;

	        //initialize probabilities
	        if (initProbs(Sudoku) < 0)
	        {
                Console.WriteLine("The puzzle is inconsistent.\n"); exeLines += 2;
		        return 0;
            } comparisons++;

            numCellsToSolve = n; assignations++;
            n = numSolvedCells; assignations++;
            k = 0; assignations++; exeLines += 4;
	        while(numSolvedCells<81 && numCellsToSolve>0)
	        {
                comparisons++;
                comparisons++; exeLines += 3;
                if (numSolvedCells > n)
                {
                    n = numSolvedCells;
                    k = 0; exeLines += 2;
                }
                else
                {
                    ++k; exeLines++;
                }
                exeLines++;
		        if (k>=maxIterations)
		        {
                    Console.WriteLine("\a\n*** Solver failed!! ***\n"); exeLines += 2;
			        return 0;
                } comparisons++;
                evolveProbs(probTolerance); exeLines += 2;
		        clipProb();
            } comparisons++; exeLines++;
	        return 1;
        }

        //----------------------------------------------Genetic---------------------------------------------------------//


        // method to validate if a number can be positioned in the cell
        static bool geneticAvailable(int num, int row, int column, int[,] Matrix)
        {
            int[] row1 = new int[9]; assignations++; exeLines++;
            int[] column1 = new int[9]; assignations++; exeLines++;
            assignations++; exeLines++; comparisons++;
            for (int i = 0; i < 9; i++)
            {
                assignations++; exeLines++; comparisons++;
                exeLines++; comparisons++;
                if (Matrix[row, i] == num)
                {
                    exeLines++;
                    return false;
                }
            }
            assignations++; exeLines++; comparisons++;
            for (int x = 0; x < 9; x++)
            {
                assignations++; exeLines++; comparisons++;
                exeLines++; comparisons++;
                if (Matrix[x, column] == num)
                {
                    exeLines++;
                    return false;
                }
            }
            assignations++; exeLines++;
            if (row == 0 || row == 3 || row == 6)
            {
                assignations++; exeLines++;
                int i = 0;
                assignations++; exeLines++;
                int a = 0;
                assignations++; exeLines++; comparisons++;
                for (int x = row; x < 9; x++)
                {
                    assignations++; exeLines++; comparisons++;
                    exeLines++; comparisons++;
                    if (a < 3)
                    {
                        exeLines++; comparisons++;
                        if (column == 0 || column == 3 || column == 6)
                        {
                            assignations++; exeLines++; comparisons++;
                            for (int y = column; y < 9; y++)
                            {
                                assignations++; exeLines++; comparisons++;
                                exeLines++; comparisons++;
                                if (i < 3)
                                {
                                    exeLines++; comparisons++;
                                    if (Matrix[x, y] == num)
                                    {
                                        exeLines++;
                                        return false;
                                    }
                                }
                                assignations++; exeLines++;
                                i++;
                            }
                            assignations++; exeLines++;
                            a++;
                        }
                        exeLines++; comparisons++;
                        if (column == 1 || column == 4 || column == 7)
                        {
                            assignations++; exeLines++; comparisons++;
                            for (int y = column - 1; y < 9; y++)
                            {
                                assignations++; exeLines++; comparisons++;
                                if (i < 3)
                                {
                                    exeLines++; comparisons++;
                                    if (Matrix[x, y] == num)
                                    {
                                        exeLines++;
                                        return false;
                                    }
                                }
                                assignations++; exeLines++;
                                i++;
                            }
                            assignations++; exeLines++;
                            a++;
                        }
                        exeLines++; comparisons++;
                        if (column == 2 || column == 5 || column == 8)
                        {
                            assignations++; exeLines++; comparisons++;
                            for (int y = column - 2; y < 9; y++)
                            {
                                assignations++; exeLines++; comparisons++;
                                exeLines++; comparisons++;
                                if (i < 3)
                                {
                                    exeLines++; comparisons++;
                                    if (Matrix[x, y] == num)
                                    {
                                        exeLines++;
                                        return false;
                                    }
                                }
                                assignations++; exeLines++;
                                i++;
                            }
                            assignations++; exeLines++;
                            a++;
                        }
                    }
                    assignations++; exeLines++;
                    i = 0;
                }
            }
            exeLines++; comparisons++;
            if (row == 1 || row == 4 || row == 7)
            {
                int i = 0; assignations++; exeLines++;
                int a = 0; assignations++; exeLines++;
                assignations++; exeLines++; comparisons++;
                for (int x = row - 1; x < 9; x++)
                {
                    assignations++; exeLines++; comparisons++;
                    exeLines++; comparisons++;
                    if (a < 3)
                    {
                        exeLines++; comparisons++;
                        if (column == 0 || column == 3 || column == 6)
                        {
                            assignations++; exeLines++; comparisons++;
                            for (int y = column; y < 9; y++)
                            {
                                assignations++; exeLines++; comparisons++;
                                exeLines++; comparisons++;
                                if (i < 3)
                                {
                                    exeLines++; comparisons++;
                                    if (Matrix[x, y] == num)
                                    {
                                        exeLines++;
                                        return false;
                                    }
                                }
                                i++; assignations++; exeLines++;
                            }
                            a++; assignations++; exeLines++;
                        }
                        exeLines++; comparisons++;
                        if (column == 1 || column == 4 || column == 7)
                        {
                            assignations++; exeLines++; comparisons++;
                            for (int y = column - 1; y < 9; y++)
                            {
                                assignations++; exeLines++; comparisons++;
                                exeLines++; comparisons++;
                                if (i < 3)
                                {
                                    exeLines++; comparisons++;
                                    if (Matrix[x, y] == num)
                                    {
                                        exeLines++;
                                        return false;
                                    }
                                }
                                i++; assignations++; exeLines++;
                            }
                            a++; assignations++; exeLines++;
                        }
                        exeLines++; comparisons++;
                        if (column == 2 || column == 5 || column == 8)
                        {
                            assignations++; exeLines++; comparisons++;
                            for (int y = column - 2; y < 9; y++)
                            {
                                assignations++; exeLines++; comparisons++;
                                exeLines++; comparisons++;
                                if (i < 3)
                                {
                                    exeLines++; comparisons++;
                                    if (Matrix[x, y] == num)
                                    {
                                        exeLines++;
                                        return false;
                                    }
                                }
                                i++; assignations++; exeLines++;
                            }
                            a++; assignations++; exeLines++;
                        }
                    }
                    i = 0; assignations++; exeLines++;
                }
            }
            exeLines++; comparisons++;
            if (row == 2 || row == 5 || row == 8)
            {
                int i = 0; assignations++; exeLines++;
                int a = 0; assignations++; exeLines++;
                assignations++; exeLines++; comparisons++;
                for (int x = row - 2; x < 9; x++)
                {
                    assignations++; exeLines++; comparisons++;
                    exeLines++; comparisons++;
                    if (a < 3)
                    {
                        exeLines++; comparisons++;
                        if (column == 0 || column == 3 || column == 6)
                        {
                            assignations++; exeLines++; comparisons++;
                            for (int y = column; y < 9; y++)
                            {
                                assignations++; exeLines++; comparisons++;
                                exeLines++; comparisons++;
                                if (i < 3)
                                {
                                    exeLines++; comparisons++;
                                    if (Matrix[x, y] == num)
                                    {
                                        exeLines++;
                                        return false;
                                    }
                                }
                                i++; assignations++; exeLines++;
                            }
                            a++; assignations++; exeLines++;
                        }
                        exeLines++; comparisons++;
                        if (column == 1 || column == 4 || column == 7)
                        {
                            assignations++; exeLines++; comparisons++;
                            for (int y = column - 1; y < 9; y++)
                            {
                                assignations++; exeLines++; comparisons++;
                                exeLines++; comparisons++;
                                if (i < 3)
                                {
                                    exeLines++; comparisons++;
                                    if (Matrix[x, y] == num)
                                    {
                                        exeLines++;
                                        return false;
                                    }
                                }
                                i++;
                            }
                            a++;
                        }
                        exeLines++; comparisons++;
                        if (column == 2 || column == 5 || column == 8)
                        {
                            assignations++; exeLines++; comparisons++;
                            for (int y = column - 2; y < 9; y++)
                            {
                                assignations++; exeLines++; comparisons++;
                                exeLines++; comparisons++;
                                if (i < 3)
                                {
                                    exeLines++; comparisons++;
                                    if (Matrix[x, y] == num)
                                    {
                                        exeLines++;
                                        return false;
                                    }
                                }
                                i++; assignations++; exeLines++;
                            }
                            a++; assignations++; exeLines++;
                        }
                    }
                    i = 0; assignations++; exeLines++;

                }
            }
            exeLines++;
            return true;
        }

        //method to do the crossovers between the rows
        static void crossOver(int[,] matrix, bool[,] bMatrix)
        {
            int[] row = new int[9]; assignations++;
            int[] nextRow = new int[9]; assignations++;
            comparisons++; exeLines += 3;
            for (int pos = 0; pos < 8; pos++)
            {
                comparisons++; exeLines += 2;
                for (int i = 0; i < 9; i++)
                {
                    row[i] = matrix[pos, i]; assignations++; exeLines++;
                    nextRow[i] = matrix[pos + 1, i]; assignations++; exeLines++;
                }
                comparisons++;
                for (int i = 0; i < 9; i++)
                {
                    exeLines += 2; comparisons++;
                    for (int cont = 0; cont < 9; cont++)
                    {
                        comparisons += 2; exeLines += 2;
                        if (nextRow[i] != 0 && !bMatrix[pos, cont] && geneticAvailable(nextRow[i], pos, cont, matrix))
                        {
                            matrix[pos, cont] = nextRow[i]; assignations++; exeLines += 4;
                            Console.Clear();
                            printMatrix(matrix);
                            Thread.Sleep(50);
                        }
                    }
                }
            }
            comparisons++; exeLines++;
            for (int cont2 = 0; cont2 < 9; cont2++)
            {
                comparisons++; exeLines += 2; ;
                for (int x = 0; x < 9; x++)
                {
                    comparisons++; exeLines += 2;
                    if (matrix[7, cont2] != 0 && !bMatrix[7, x] && geneticAvailable(matrix[7, cont2], 8, x, matrix))
                    {
                        matrix[8, x] = matrix[7, cont2];
                        Console.Clear();
                        printMatrix(matrix); assignations++; exeLines += 4;
                        Thread.Sleep(50);
                    }
                }
            }

        }
        //method to count the zeros of every row
        static Dictionary<int, int> auxSort(int[,] matrix)
        {

            int[] rows = new int[9];
            int[] counts = new int[9];
            Dictionary<int, int> list = new Dictionary<int, int>(); assignations += 4; comparisons++; exeLines += 4;
            for (int x = 0; x < 9; x++)
            {
                int cont = 0; assignations++; exeLines += 2; ; comparisons++;
                for (int y = 0; y < 9; y++)
                {
                    exeLines += 2; comparisons++;
                    if (matrix[x, y] != 0)
                    {

                        cont++; assignations++; exeLines++;
                    }
                }
                list.Add(x, cont); assignations++;
                counts[x] = cont; exeLines += 3;
            }
            return list;
        }
        // method to sort descending the quantity of zeros in the rows
        static Dictionary<int, int> sort(int[,] matrix)
        {
            var vDictionary = new Dictionary<int, int>(9);
            vDictionary = auxSort(matrix);
            Dictionary<int, int> nDictionary = new Dictionary<int, int>();
            var items = from pair in vDictionary
                        orderby pair.Value descending
                        select pair;
            exeLines += 5; assignations += 3; comparisons++;
            foreach (KeyValuePair<int, int> pair in items)
            {
                nDictionary.Add(pair.Key, pair.Value); comparisons++; exeLines++;
            }
            exeLines++;
            return nDictionary;
        }

        //method to mutate the empty cells in the sudoku
        static void mutation(int[,] matrix, bool[,] bMatrix)
        {
            Dictionary<int, int> dictionary = new Dictionary<int, int>();
            dictionary = sort(matrix);
            exeLines += 3; comparisons++; assignations += 2;
            foreach (var i in dictionary)
            {
                comparisons++; exeLines++;
                int row = i.Key; assignations++; exeLines++; comparisons++;
                for (int x = 0; x < 9; x++)
                {
                    comparisons++;
                    int num = 0; assignations++; comparisons++; exeLines += 2;
                    if (matrix[row, x] == 0 && !bMatrix[row, x] && available(num, row, x, matrix, bMatrix))
                    {
                        matrix[row, x] = num;
                        Console.Clear();
                        printMatrix(matrix);
                        Thread.Sleep(100);
                        exeLines += 4; assignations++;

                    }
                    num++; assignations++;
                }
            }
        }

        // method that call all the methods necessaries to solve the sudoku
        static void geneticSudoku(int[,] matrix, bool[,] bMatrix)
        {
            comparisons++; exeLines++;
            for (int i = 0; i < 9; i++)
            {
                comparisons++; exeLines += 4;
                crossOver(matrix, bMatrix);
                mutation(matrix, bMatrix);
                crossOver(matrix, bMatrix);
                mutation(matrix, bMatrix);
            }
        }

        static void Main(string[] args)
        {
            int[,] newMatrix = new int[9, 9];
            bool[,] boolMatrix = new bool[9, 9];
            int[,] TheSolution = new int[9, 9];
            Console.WriteLine("\n");
            Console.WriteLine("60 Initial numbers backtracking Sudoku");
            fillMatrix(60, newMatrix,boolMatrix);
            printMatrix(newMatrix);
            Console.Write("\nPress any key to begin solving");
            Console.ReadKey();
            temporizator = Stopwatch.StartNew();
            backtrackingSudoku(0, 0, newMatrix, boolMatrix, 1, false);
            tTime = temporizator.ElapsedMilliseconds;
            Console.WriteLine("\nComparisons= {0}   Assignations= {1}", comparisons, assignations);
            Console.WriteLine("Executed lines= {0}   Elaspsed Milliseconds= {1}", exeLines, tTime);
            assignations = 0;
            exeLines = 0;
            comparisons = 0;
            Console.WriteLine("\nThe Sudoku has been solved, press any key");
            Console.ReadKey();
            Console.Clear();
            Console.WriteLine("\n");
            Console.WriteLine("45 Initial numbers backtracking Sudoku");
            clearMatrix(newMatrix);
            clearBMatrix(boolMatrix);
            fillMatrix(45, newMatrix, boolMatrix);
            printMatrix(newMatrix);
            Console.Write("\nPress any key to begin solving");
            Console.ReadKey();
            temporizator.Restart();
            backtrackingSudoku(0, 0, newMatrix, boolMatrix, 1, false);
            tTime = temporizator.ElapsedMilliseconds;
            Console.WriteLine("\nComparisons= {0}   Assignations= {1}", comparisons, assignations);
            Console.WriteLine("Executed lines= {0}   Elaspsed Milliseconds= {1}", exeLines, tTime);
            assignations = 0;
            comparisons = 0;
            exeLines = 0;
            Console.WriteLine("\nThe Sudoku has been solved, press any key");
            Console.ReadKey();
            Console.Clear();
            Console.WriteLine("\n");
            Console.WriteLine("30 Initial numbers backtracking Sudoku");
            clearMatrix(newMatrix);
            clearBMatrix(boolMatrix);
            fill30(newMatrix,boolMatrix);
            printMatrix(newMatrix);
            Console.Write("\nPress any key to begin solving");
            Console.ReadKey();
            temporizator.Restart();
            backtrackingSudoku(0, 0, newMatrix, boolMatrix, 1, false);
            tTime = temporizator.ElapsedMilliseconds;
            Console.WriteLine("\nComparisons= {0}   Assignations= {1}", comparisons, assignations);
            Console.WriteLine("Executed lines= {0}   Elaspsed Milliseconds= {1}", exeLines, tTime);
            assignations = 0;
            comparisons = 0;
            exeLines = 0;
            Console.WriteLine("\nThe Sudoku has been solved, press any key");
            Console.ReadKey();
            Console.Clear();
            Console.WriteLine("\n");
            Console.WriteLine("15 Initial numbers backtracking Sudoku");
            clearMatrix(newMatrix);
            clearBMatrix(boolMatrix);
            fill15(newMatrix,boolMatrix);
            printMatrix(newMatrix);
            Console.Write("\nPress any key to begin solving");
            Console.ReadKey();
            temporizator.Restart();
            backtrackingSudoku(0, 0, newMatrix, boolMatrix, 1, false);
            tTime = temporizator.ElapsedMilliseconds;
            Console.WriteLine("\nComparisons= {0}   Assignations= {1}", comparisons, assignations);
            Console.WriteLine("Executed lines= {0}   Elaspsed Milliseconds= {1}", exeLines, tTime);
            assignations = 0;
            comparisons = 0;
            exeLines = 0;
            Console.WriteLine("\nThe Sudoku has been solved, press any key");
            Console.ReadKey();
            Console.Clear();
            clearMatrix(newMatrix);
            clearBMatrix(boolMatrix);
            clearMatrix(TheSolution);
            fillMatrixProb(60, newMatrix);
            Console.WriteLine("60 Initial numbers probabilistic Sudoku");
            printMatrix(newMatrix);
            Console.Write("\nPress any key to begin solving");
            Console.ReadKey();
            temporizator.Restart();
            solveSudoku(newMatrix, 81);
            Console.WriteLine("\n");
            getProbs(TheSolution);
            tTime = temporizator.ElapsedMilliseconds;
            Console.WriteLine("\nComparisons= {0}   Assignations= {1}", comparisons, assignations);
            Console.WriteLine("Executed lines= {0}   Elaspsed Milliseconds= {1}", exeLines, tTime);
            assignations = 0;
            comparisons = 0;
            exeLines = 0;
            Console.WriteLine("\nThe Sudoku has been solved, press any key");
            Console.ReadKey();
            Console.Clear();
            clearMatrix(newMatrix);
            clearBMatrix(boolMatrix);
            clearMatrix(TheSolution);
            fillMatrixProb(45, newMatrix);
            Console.WriteLine("45 Initial numbers probabilistic Sudoku");
            printMatrix(newMatrix);
            Console.Write("\nPress any key to begin solving");
            Console.ReadKey();
            temporizator.Restart();
            solveSudoku(newMatrix, 81);
            Console.WriteLine("\n");
            getProbs(TheSolution);
            tTime = temporizator.ElapsedMilliseconds;
            Console.WriteLine("\nComparisons= {0}   Assignations= {1}", comparisons, assignations);
            Console.WriteLine("Executed lines= {0}   Elaspsed Milliseconds= {1}", exeLines, tTime);
            assignations = 0;
            exeLines = 0;
            comparisons = 0;
            Console.WriteLine("\nThe Sudoku has been solved, press any key");
            Console.ReadKey();
            Console.Clear();
            clearMatrix(newMatrix);
            clearBMatrix(boolMatrix);
            clearMatrix(TheSolution);
            fillMatrixProb(30, newMatrix);
            Console.WriteLine("30 Initial numbers probabilistic Sudoku");
            printMatrix(newMatrix);
            Console.Write("\nPress any key to begin solving");
            Console.ReadKey();
            temporizator.Restart();
            Console.WriteLine("\n");
            solveSudoku(newMatrix, 81);
            Console.WriteLine("\n");
            getProbs(TheSolution);
            tTime = temporizator.ElapsedMilliseconds;
            Console.WriteLine("\nComparisons= {0}   Assignations= {1}", comparisons, assignations);
            Console.WriteLine("Executed lines= {0}   Elaspsed Milliseconds= {1}", exeLines, tTime);
            assignations = 0;
            exeLines = 0;
            comparisons = 0;
            Console.WriteLine("\nThe Sudoku has been solved, press any key");
            Console.ReadKey();
            Console.Clear();
            clearMatrix(newMatrix);
            clearBMatrix(boolMatrix);
            clearMatrix(TheSolution);
            fillMatrixProb(15, newMatrix);
            Console.WriteLine("15 Initial numbers probabilistic Sudoku");
            printMatrix(newMatrix);
            Console.Write("\nPress any key to begin solving");
            Console.ReadKey();
            temporizator.Restart();
            solveSudoku(newMatrix, 81);
            Console.WriteLine("\n");
            getProbs(TheSolution);
            tTime = temporizator.ElapsedMilliseconds;
            Console.WriteLine("\nComparisons= {0}   Assignations= {1}", comparisons, assignations);
            Console.WriteLine("Executed lines= {0}   Elaspsed Milliseconds= {1}", exeLines, tTime);
            assignations = 0;
            exeLines = 0;
            comparisons = 0;
            Console.WriteLine("\nThe Sudoku has been solved, press any key");
            Console.ReadKey();
            Console.Clear();
            Console.WriteLine("\n");
            Console.WriteLine("60 Initial numbers genetic Sudoku");
            clearMatrix(newMatrix);
            clearBMatrix(boolMatrix);
            fillMatrix(60, newMatrix, boolMatrix);
            printMatrix(newMatrix);
            Console.Write("\nPress any key to begin solving");
            Console.ReadKey();
            temporizator.Restart();
            geneticSudoku(newMatrix, boolMatrix);
            tTime = temporizator.ElapsedMilliseconds;
            Console.WriteLine("\nComparisons= {0}   Assignations= {1}", comparisons, assignations);
            Console.WriteLine("Executed lines= {0}   Elaspsed Milliseconds= {1}", exeLines, tTime);
            assignations = 0;
            exeLines = 0;
            comparisons = 0;
            Console.WriteLine("\nThe Sudoku has been solved, press any key");
            Console.ReadKey();
            Console.Clear();
            Console.WriteLine("\n");
            Console.WriteLine("45 Initial numbers genetic Sudoku");
            clearMatrix(newMatrix);
            clearBMatrix(boolMatrix);
            fillMatrix(45, newMatrix, boolMatrix);
            printMatrix(newMatrix);
            Console.Write("\nPress any key to begin solving");
            Console.ReadKey();
            temporizator.Restart();
            geneticSudoku(newMatrix, boolMatrix);
            tTime = temporizator.ElapsedMilliseconds;
            Console.WriteLine("\nComparisons= {0}   Assignations= {1}", comparisons, assignations);
            Console.WriteLine("Executed lines= {0}   Elaspsed Milliseconds= {1}", exeLines, tTime);
            assignations = 0;
            exeLines = 0;
            comparisons = 0;
            Console.WriteLine("\nThe Sudoku has been solved, press any key");
            Console.ReadKey();
            Console.Clear();
            Console.WriteLine("\n");
            Console.WriteLine("30 Initial numbers genetic Sudoku");
            clearMatrix(newMatrix);
            clearBMatrix(boolMatrix);
            fillMatrix(30, newMatrix, boolMatrix);
            printMatrix(newMatrix);
            Console.Write("\nPress any key to begin solving");
            Console.ReadKey();
            temporizator.Restart();
            geneticSudoku(newMatrix, boolMatrix);
            tTime = temporizator.ElapsedMilliseconds;
            Console.WriteLine("\nComparisons= {0}   Assignations= {1}", comparisons, assignations);
            Console.WriteLine("Executed lines= {0}   Elaspsed Milliseconds= {1}", exeLines, tTime);
            assignations = 0;
            exeLines = 0;
            comparisons = 0;
            Console.WriteLine("\nThe Sudoku has been solved, press any key");
            Console.ReadKey();
            Console.Clear();
            Console.WriteLine("\n");
            Console.WriteLine("15 Initial numbers genetic Sudoku");
            clearMatrix(newMatrix);
            clearBMatrix(boolMatrix);
            fillMatrix(15, newMatrix, boolMatrix);
            printMatrix(newMatrix);
            Console.Write("\nPress any key to begin solving");
            Console.ReadKey();
            temporizator.Restart();
            geneticSudoku(newMatrix, boolMatrix);
            tTime = temporizator.ElapsedMilliseconds;
            Console.WriteLine("\nComparisons= {0}   Assignations= {1}", comparisons, assignations);
            Console.WriteLine("Executed lines= {0}   Elaspsed Milliseconds= {1}", exeLines, tTime);
            assignations = 0;
            exeLines = 0;
            comparisons = 0;
            Console.WriteLine("\nThe Sudoku has been solved, press any key");
            Console.ReadKey();
            Console.Clear();
        }
    }
}
