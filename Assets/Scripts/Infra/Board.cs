using System;
using System.Collections.Generic;
using UnityEngine;

namespace Infra
{
    public class Board
    {
        public Position[] WinningPositions { get; private set; }
        private readonly int squareCount;  
        private readonly Symbol?[,] boardSymbols;
        private static Board instance;
        private Stack<Position> symbolPositionOrder;
        private Board(int diagonalRowLength)
        {
            squareCount = diagonalRowLength * diagonalRowLength;
            boardSymbols = new Symbol?[diagonalRowLength, diagonalRowLength];
            WinningPositions = new Position[diagonalRowLength];
            symbolPositionOrder = new Stack<Position>();
        }
        
        public static Board GetInstance(int boardSize)
        {
            return instance ??= new Board(boardSize);
        }

        /// <summary>
        /// Add a symbol to the board symbols array.
        /// </summary>
        /// <param name="symbol">the square</param>
        /// <param name="position">x position in the array</param>
        public void SetSymbolAtPosition(Symbol symbol,Position position)
        {
            if (boardSymbols.Length > squareCount)
            {
                Debug.LogWarning("The BoardSquares count is over the limits");
                return;
            }
            symbolPositionOrder.Push(position);
            boardSymbols[position.X, position.Y] = symbol;
        }

        /// <summary>
        /// reset board, set all board symbols items to null
        /// </summary>
        public void ResetBoard()
        {
            for (int i = 0; i <boardSymbols.GetLength(0); i++)
            {
                for (int j = 0; j < boardSymbols.GetLength(1); j++)
                {
                    boardSymbols[i, j] = null;
                }
            }

            symbolPositionOrder = new Stack<Position>();
        }
        
        /// <summary>
        /// Check if there is an identical symbol in a row or column.
        /// </summary>
        /// <param name="condition">Condition to be satisfied for winning</param>
        /// <returns>true if identical symbols founded</returns>
        private bool CheckVerticalOrHorizontal(Func<int,int,bool> condition)
        {
            if (boardSymbols == null)
            {
                return false;
            }

            for (int i = 0; i < boardSymbols.GetLength(0); i++)
            {
                bool isIdentical = true;
                for (int j = 0; j < boardSymbols.GetLength(1); j++)
                {
                    if (!condition(i,j))
                    {
                        isIdentical = false;
                        break;
                    }
                }

                if (isIdentical)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Check if there is an identical diagonal symbols.
        /// </summary>
        /// <param name="condition">Mark to compare to</param>
        /// <returns>true if identical diagonal found</returns>
        private bool CheckDiagonal(Func<int,int,bool> condition)
        {
            for (int i = 0; i < boardSymbols.GetLength(0); i++)
            {
                if (!condition(i,i))
                {
                    return false;
                }
            }
            return true;
        }
        
        /// <summary>
        /// Check if there is a row with identical marks, and save the position of the identical symbols.
        /// </summary>
        /// <param name="symbol">symbol to check</param>
        /// <returns>true if row with identical marks founded</returns>
        private bool HasIdenticalRow(Symbol symbol)
        {
            return CheckVerticalOrHorizontal((column, row) =>
            {
                if (boardSymbols[column, row] == symbol)
                { 
                    WinningPositions[row] = new Position(column, row);
                    return true;
                }
                return false;
            });
        }
        
        /// <summary>
        /// Check if there is a column with identical marks, and save the positions.
        /// </summary>
        /// <param name="symbol">symbol to check</param>
        /// <returns>true if column with identical marks founded</returns>
        private bool HasIdenticalColumn(Symbol symbol)
        {
            return CheckVerticalOrHorizontal((i, j) =>
            {
                if (boardSymbols[j, i] != null && boardSymbols[j, i] == symbol)
                {
                    WinningPositions[j] = new Position(j, i);
                    return true;
                }

                return false;
            });
        }

        /// <summary>
        /// Check if there is an identical diagonal symbols from top left corner to the bottom right corner.
        /// </summary>
        /// <param name="symbol">symbol to check</param>
        /// <returns>true if identical diagonal founded</returns>
        private bool HasIdenticalDiagonalLeft(Symbol symbol)
        {
            return CheckDiagonal((i, j) =>
            {
                if (boardSymbols[i, j] == symbol)
                {
                    WinningPositions[i] = new Position(i, j);
                    return true;
                }
                return false;
            });
        }
        
        /// <summary>
        /// Check if there is an identical diagonal symbols from top right corner to the bottom left corner.
        /// </summary>
        /// <param name="symbol">symbol to check</param>
        /// <returns>true if identical diagonal founded</returns>
        private bool HasIdenticalDiagonalRight(Symbol symbol)
        {
            return CheckDiagonal((firstIndex, secondIndex) =>
            {
                int secondIndexReversed = boardSymbols.GetLength(1) - 1 - secondIndex;
                if (boardSymbols[firstIndex, secondIndexReversed]== symbol)
                {
                    WinningPositions[firstIndex] = new Position(firstIndex, secondIndexReversed);
                    return true;
                }
                return false;
            });
        }

        /// <summary>
        /// Check if given symbol has a winning combination
        /// </summary>
        /// <param name="symbol">symbol to check</param>
        /// <returns>true if winning combination founded</returns>
        public bool CheckRawWin(Symbol symbol)
        {
            return HasIdenticalRow(symbol) 
                   || HasIdenticalColumn(symbol)
                   || HasIdenticalDiagonalLeft(symbol) 
                   ||HasIdenticalDiagonalRight(symbol);
        }

        /// <summary>
        /// Check if a given position is not set to any symbol.
        /// </summary>
        /// <param name="position">Position to check</param>
        /// <returns>true if the board at the given position is null</returns>
        public bool IsPositionOccupied(Position position)
        {
            if (boardSymbols == null)
            {
                return false;
            }

            return boardSymbols[position.X, position.Y] != null;
        }

        /// <summary>
        /// Check if the board is not full
        /// </summary>
        /// <returns>true if board not full</returns>
        public bool HasAvailablePositions()
        {
            for (int i = 0; i <boardSymbols.GetLength(0); i++)
            {
                for (int j = 0; j < boardSymbols.GetLength(1); j++)
                {
                    if (boardSymbols[i, j] == null)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Remove last symbol from the array
        /// </summary>
        public Position? RemoveLastSymbol()
        {
            if (symbolPositionOrder == null || symbolPositionOrder.Count <= 0)
            {
                return null;
            }

            Position lastSquarePosition = symbolPositionOrder.Pop();
            boardSymbols[lastSquarePosition.X, lastSquarePosition.Y] = null;
            return lastSquarePosition;
        }

        /// <summary>
        /// Check for available positions and add them to a list.
        /// </summary>
        /// <returns>list of available positions</returns>
        public List<Position> GetAvailablePositions()
        {
            List<Position> availablePositions = new List<Position>();
            for (int i = 0; i <boardSymbols.GetLength(0); i++)
            {
                for (int j = 0; j < boardSymbols.GetLength(1); j++)
                {
                    if (boardSymbols[i, j] == null)
                    {
                        availablePositions.Add(new Position(i,j));
                    }
                }
            }
            return availablePositions;
        }
    }
}