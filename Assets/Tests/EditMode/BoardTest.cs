using System.Collections;
using System.Collections.Generic;
using Infra;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class BoardTest
{
    
    [Test]
    public void Board_IsPositionOccupiedTest()
    {
        //Arrange
        Board board = Board.GetInstance(3);
        board.ResetBoard();
        //Act
        board.SetSymbolAtPosition(Symbol.O,new Position(0,0));
        board.SetSymbolAtPosition(Symbol.X,new Position(1,1));
        board.SetSymbolAtPosition(Symbol.O,new Position(2,2));
        board.SetSymbolAtPosition(Symbol.X,new Position(1,0));
        
        //Assert
        Assert.IsTrue(board.IsPositionOccupied(new Position(0,0)));
        Assert.IsTrue(board.IsPositionOccupied(new Position(1,1)));
        Assert.IsTrue(board.IsPositionOccupied(new Position(2,2)));
        Assert.IsTrue(board.IsPositionOccupied(new Position(1,0)));
        
        Assert.IsFalse(board.IsPositionOccupied(new Position(0,1)));
        Assert.IsFalse(board.IsPositionOccupied(new Position(0,2)));
        Assert.IsFalse(board.IsPositionOccupied(new Position(1,2)));
        Assert.IsFalse(board.IsPositionOccupied(new Position(2,0)));
        Assert.IsFalse(board.IsPositionOccupied(new Position(2,1)));
    }
    
    [Test]
    [TestCase(1)]
    [TestCase(2)]
    [TestCase(3)]
    [TestCase(4)]
    [TestCase(5)]
    [TestCase(6)]
    [TestCase(7)]
    [TestCase(8)]
    [TestCase(9)]
     public void Board_UndoTest(int numOfSteps)
     {
         //Arrange
         Board board = Board.GetInstance(3);
         board.ResetBoard();
         List<Position> randomPositions = new List<Position>()
                {
                new Position(0, 1),
                new Position(1, 1),
                new Position(2, 0),
                new Position(0, 0),
                new Position(1, 2),
                new Position(2, 2),
                new Position(0, 2),
                new Position(2, 1),
                new Position(1, 0),
            };
         //Act
         foreach (Position position in randomPositions)
         {
             board.SetSymbolAtPosition(Symbol.X,position);
         }
         
         for (int i = 0; i < numOfSteps; i++)
         {
             board.RemoveLastSymbol();
         }
         
         //Assert
         for (int i = 0; i < numOfSteps; i++)
         {
             Assert.IsFalse(board.IsPositionOccupied(randomPositions[randomPositions.Count - i - 1]));
         }
     }
     
     [Test]
     public void Board_WinLoseTestRow()
     {
         //Arrange
         Board board = Board.GetInstance(3);
         board.ResetBoard();
         //Act
         board.SetSymbolAtPosition(Symbol.O,new Position(0,0));
         board.SetSymbolAtPosition(Symbol.X,new Position(1,1));
         board.SetSymbolAtPosition(Symbol.O,new Position(0,1));
         board.SetSymbolAtPosition(Symbol.X,new Position(1,0));
         board.SetSymbolAtPosition(Symbol.O,new Position(0,2));
         //Assert
         Assert.IsTrue(board.CheckRawWin(Symbol.O));
         Assert.IsFalse(board.CheckRawWin(Symbol.X));
     }
     
     [Test]
     public void Board_WinLoseTestColumn()
     {
         //Arrange
         Board board = Board.GetInstance(3);
         board.ResetBoard();
         //Act
         board.SetSymbolAtPosition(Symbol.O,new Position(0,0));
         board.SetSymbolAtPosition(Symbol.X,new Position(1,1));
         board.SetSymbolAtPosition(Symbol.O,new Position(0,2));
         board.SetSymbolAtPosition(Symbol.X,new Position(0,1));
         board.SetSymbolAtPosition(Symbol.O,new Position(2,2));
         board.SetSymbolAtPosition(Symbol.X,new Position(2,1));
         //Assert
         Assert.IsTrue(board.CheckRawWin(Symbol.X));
         Assert.IsFalse(board.CheckRawWin(Symbol.O));
     }
     
     [Test]
     public void Board_WinLoseTestDiagonal()
     {
         //Arrange
         Board board = Board.GetInstance(3);
         board.ResetBoard();
         //Act
         board.SetSymbolAtPosition(Symbol.O,new Position(0,0));
         board.SetSymbolAtPosition(Symbol.X,new Position(0,1));
         board.SetSymbolAtPosition(Symbol.O,new Position(1,1));
         board.SetSymbolAtPosition(Symbol.X,new Position(1,2));
         board.SetSymbolAtPosition(Symbol.O,new Position(2,2));
         //Assert
         Assert.IsTrue(board.CheckRawWin(Symbol.O));
         Assert.IsFalse(board.CheckRawWin(Symbol.X));
     }
     
     [Test]
     public void Board_DrawTest()
     {
         //Arrange
         Board board = Board.GetInstance(3);
         board.ResetBoard();
         //Act
         board.SetSymbolAtPosition(Symbol.X,new Position(0,0));
         board.SetSymbolAtPosition(Symbol.O,new Position(0,1));
         board.SetSymbolAtPosition(Symbol.X,new Position(0,2));
         board.SetSymbolAtPosition(Symbol.O,new Position(1,1));
         board.SetSymbolAtPosition(Symbol.X,new Position(1,2));
         board.SetSymbolAtPosition(Symbol.O,new Position(2,0));
         board.SetSymbolAtPosition(Symbol.X,new Position(1,0));
         board.SetSymbolAtPosition(Symbol.O,new Position(2,2));
         board.SetSymbolAtPosition(Symbol.X,new Position(2,1));

         //Assert
         Assert.IsFalse(board.CheckRawWin(Symbol.X));
         Assert.IsFalse(board.CheckRawWin(Symbol.O));
     }
}
