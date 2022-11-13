using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Random = UnityEngine.Random;
using Infra;
using UnityEngine;
using Views;

namespace Controllers
{
    public class BoardController
    {
        public static event Action<Player,bool> OnGameOver;
        private const int BoardSize = 3;
        private readonly PlayerController playerController;
        private Board board;
        private List<ISymbolView> boardSquaresView;
        private Timer timer;
        
        public BoardController(PlayerController playerController,int? time)
        {
            this.playerController = playerController;
            AddListeners();
            InstantiateBoard();
        }
        
        /// <summary>
        /// Instantiate board and symbols view view once
        /// </summary>
        private void InstantiateBoard()
        {
            InstantiateSymbolsView();
            board = Board.GetInstance(BoardSize);
        }
        
        /// <summary>
        /// Instantiate symbol view and save the instantiated views in a list.
        /// </summary>
        private void InstantiateSymbolsView()
        {
            boardSquaresView = new List<ISymbolView>();
            for (int i = 0; i < BoardSize; i++)
            {
                for (int j = 0; j < BoardSize; j++)
                {
                    ISymbolView symbolView = Factory.Instance.InstantiateSquare();
                    symbolView.SetPosition(new Position(i,j));
                    boardSquaresView.Add(symbolView);
                }
            }
        }

        /// <summary>
        /// Make auto move for the computer.
        /// </summary>
        /// <param name="delay">delay before making the move</param>
        private async void MakeAutoMoveIfNeeded(int delay)
        {
            if (playerController.PlayerTurn.PlayerType == PlayerType.Computer)
            {
                SetSymbolsInteractions(false);
                await Task.Delay(delay);
                if (playerController.PlayerTurn.PlayerType != PlayerType.Computer)
                {
                    SetSymbolsInteractions(true);
                    return;
                }
                Position? randomPosition = Utils.GetRandomPosition(board.GetAvailablePositions());
                if (randomPosition != null)
                {
                    SetSymbol(randomPosition.Value);
                }
            }
        }

        /// <summary>
        /// High light hint move when the hint button clicked.
        /// </summary>
        public void HighlightHintMove()
        {
            Position? randomPosition = Utils.GetRandomPosition(board.GetAvailablePositions());
            if (randomPosition == null)
            {
                return;
            }

            ISymbolView symbolView = Utils.GetSymbolViewByPosition(boardSquaresView, randomPosition.Value);
            if (symbolView != null)
            {
                symbolView.HighlightView();
            }
        }
        
        /// <summary>
        /// Set timer to follow the player turn time when time is active
        /// if time is null the timer will be ignored and players will play with no time.
        /// </summary>
        /// <param name="time">Time for each turn</param>
        private void SetTimer(int? time)
        {
            if (time == null)
            {
                timer?.Stop();
                timer = null;
            }
            else if (timer == null)
            {
                timer = new Timer(time.Value, TimeOut,playerController.UpdateViewTimer);
            }
            timer?.Restart(time);
        }
        
        /// <summary>
        /// Reset the board time, view and data.
        /// </summary>
        /// <param name="time">new time for players turn</param>
        public void ResetBoard(int? time)
        {
            board.ResetBoard();
            ResetSymbolView();
            SetTimer(time);
            SetSymbolsInteractions(playerController.PlayerTurn.PlayerType != PlayerType.Computer);
            MakeAutoMoveIfNeeded(Random.Range(1000,2000));
        }
        
        /// <summary>
        /// Remove X,O symbols from the view
        /// </summary>
        private void ResetSymbolView()
        {
            foreach (ISymbolView squareView in boardSquaresView)
            {
                squareView.Reset();
            }
        }
        
        /// <summary>
        /// Get called when user click on a square symbol in the board
        /// </summary>
        /// <param name="symbolPosition">The clicked symbol position</param>
        private void UserClickedSymbol(Position symbolPosition)
        {
            SetSymbol(symbolPosition);
        }

        /// <summary>
        /// Set new symbol to the board in the given position, and check if need to stop the game
        /// in case there is a winner or the board is full (draw), if not then switch players and reset time
        /// in case the current player is computer the function will make a move for it.
        /// </summary>
        /// <param name="position">the position to add symbol at</param>
        private void SetSymbol(Position position)
        {
            if (board.IsPositionOccupied(position))
            {
                return;
            }
            ISymbolView symbolViewView = GetSymbolViewAtPosition(position);
            symbolViewView.DrawSymbol(playerController.PlayerTurn.PlayerSymbol);
            board.SetSymbolAtPosition(playerController.PlayerTurn.PlayerSymbol,position);
            
            if (HasWinnerOrBoardFull())
            {
                StopGame();
                return;
            }
            timer?.Restart();
            playerController.SwitchPlayersTurn();
            SetSymbolsInteractions(playerController.PlayerTurn.PlayerType != PlayerType.Computer);
            MakeAutoMoveIfNeeded(Random.Range(1000,2000));
        }

        /// <summary>
        /// Check if there is a winner or board is full.
        /// </summary>
        /// <returns>true has a winner or board is full</returns>
        private bool HasWinnerOrBoardFull()
        {
            return board.CheckRawWin(playerController.PlayerTurn.PlayerSymbol) || !board.HasAvailablePositions();
        }

        /// <summary>
        /// Stopping the game, timer and the symbol view interactions and trigger the game over event
        /// if the winner user is null that mean it is a draw.
        /// </summary>
        private void StopGame()
        {
            SetSymbolsInteractions(false);
            timer?.Stop();
            bool hasWinner = board.CheckRawWin(playerController.PlayerTurn.PlayerSymbol);
            Player winner = hasWinner ? playerController.PlayerTurn : null;
            if (hasWinner)
            {
                ShowWiningSymbols();
            }
            OnGameOver?.Invoke(winner,false);
        }

        /// <summary>
        /// Trigger the animation to high light the winning symbols in the board.
        /// </summary>
        private void ShowWiningSymbols()
        {
            Position[] positionsArray = board.WinningPositions;
            foreach (Position position in positionsArray)
            {
                ISymbolView symbolView = Utils.GetSymbolViewByPosition(boardSquaresView, position);
                if (symbolView != null)
                {
                    symbolView.PlayWinningAnimation();
                }
            }
        }

        /// <summary>
        /// Getting the symbol at given position
        /// </summary>
        /// <param name="position">symbol position</param>
        /// <returns>symbol if exist or null if not exist</returns>
        private ISymbolView GetSymbolViewAtPosition(Position position)
        {
            if (boardSquaresView == null)
            {
                return null;
            }
            return boardSquaresView.Find(squareView => squareView.Position.IsEqual(position));
        }

        /// <summary>
        /// Remove last symbol was add to the board.
        /// </summary>
        public void UndoMove()
        {
           Position? position = board.RemoveLastSymbol();
           if (position == null)
           {
               return;
           }
           ISymbolView symbolView = Utils.GetSymbolViewByPosition(boardSquaresView, position);
           if (symbolView != null)
           {
               symbolView.Reset();
               timer?.Restart();
               playerController.SwitchPlayersTurn();
               MakeAutoMoveIfNeeded(Random.Range(1000,2000));
           }
        }
        
        /// <summary>
        /// Get triggered when the time is over.
        /// </summary>
        private void TimeOut()
        {
            OnGameOver?.Invoke(playerController.GetPreviousPlayerTurn(),true);
        }
        
        /// <summary>
        /// To disable/enable user interaction with the symbol view.
        /// </summary>
        /// <param name="isInteractable">true to set interactable</param>
        private void SetSymbolsInteractions(bool isInteractable)
        {
            foreach (ISymbolView squareView in boardSquaresView)
            {
                if (squareView != null)
                {
                    squareView.SetInteractable(isInteractable);
                }
            }
        }
        
        private void AddListeners()
        {
            SymbolView.OnSymbolButtonClicked += UserClickedSymbol;
        }
    }
}

