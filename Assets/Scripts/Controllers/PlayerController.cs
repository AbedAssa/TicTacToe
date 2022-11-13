using UnityEngine;
using Views;
using Infra;
using Random = UnityEngine.Random;

namespace Controllers
{
    /// <summary>
    /// Responsible for player logic
    /// </summary>
    public class PlayerController:MonoBehaviour
    {
        [Header("Players view")]
        [SerializeField] private PlayerView userPlayerView;
        [SerializeField] private PlayerView opponentPlayerView;
        public static PlayerController Instance { get; private set; }
        public Player PlayerTurn { get; private set; } //The player whose turn is to play
        private Player userPlayer;
        private Player opponentPlayer;
        private void Awake()
        {
            AddListeners();
            ResetPlayers();
        }

        /// <summary>
        /// Set players and players view
        /// </summary>
        /// <param name="opponentType">the opponent type user/computer</param>
        public void ResetPlayers(PlayerType opponentType = PlayerType.User)
        {
            SetPlayers(opponentType);
            SetPlayersView(); 
        }

        /// <summary>
        /// Set players data with random symbol.
        /// </summary>
        /// <param name="opponentType">the opponent player type computer/user</param>
        private void SetPlayers(PlayerType opponentType)
        {
            Instance = this;
            (Symbol userSymbol, Symbol opponentSymbol) randomMarks = GetRandomSymbol();
            userPlayer = new Player("Player 1", randomMarks.userSymbol, PlayerType.User);
            opponentPlayer = new Player("Player 2", randomMarks.opponentSymbol, opponentType);
            PlayerTurn = GetOpeningPlayer();
        }

        /// <summary>
        /// Getting random symbols to be set to players.
        /// </summary>
        /// <returns>random pair of symbols</returns>
        private (Symbol userSymbol, Symbol opponentSymbol) GetRandomSymbol()
        {
            float randomNumber = Random.Range(0f, 1f);
            if (randomNumber <= 0.5f)
            {
                return (Symbol.O,Symbol.X);
            }
            return (Symbol.X,Symbol.O);
        }

        /// <summary>
        /// Update both players view.
        /// </summary>
        private void SetPlayersView()
        {
            SetPlayerView(userPlayerView,$"{userPlayer.PlayerSymbol} - {userPlayer.PlayerName}");
            SetPlayerView(opponentPlayerView,$"{opponentPlayer.PlayerSymbol} - {opponentPlayer.PlayerName}");
        }

        /// <summary>
        /// Update a single player view
        /// </summary>
        /// <param name="playerView">Player view</param>
        /// <param name="playerName">Player name to be displayed in the view</param>
        private void SetPlayerView(PlayerView playerView,string playerName)
        {
            if (playerName == null)
            {
                Debug.LogError("PlayerView is null");
                return;
            }
            playerView.SetPlayerName(playerName);
        }

        /// <summary>
        /// Updating the view timers of the players.
        /// </summary>
        /// <param name="fillAmount">the amount to fill the timer view</param>
        public void UpdateViewTimer(float fillAmount)
        {
            if (PlayerTurn == userPlayer)
            {
                userPlayerView.SetTimeIndicatorFill(fillAmount);
                opponentPlayerView.SetTimeIndicatorFill(0f);
            }
            else
            {
                opponentPlayerView.SetTimeIndicatorFill(fillAmount);
                userPlayerView.SetTimeIndicatorFill(0f);
            }
        }

        private void ResetViewTimer()
        {
            opponentPlayerView.SetTimeIndicatorFill(0f);
            userPlayerView.SetTimeIndicatorFill(0f);
        }
        
        /// <summary>
        /// Switch player turn.
        /// </summary>
        public void SwitchPlayersTurn()
        {
            PlayerTurn = PlayerTurn == userPlayer ? opponentPlayer : userPlayer;
        }

        /// <summary>
        /// Check who is the player with the X symbol to make the first move.
        /// </summary>
        /// <returns>Player to start the game</returns>
        private Player GetOpeningPlayer()
        {
            if (userPlayer.PlayerSymbol == Symbol.X)
            {
                return userPlayer;
            }

            return opponentPlayer;
        }
        
        /// <summary>
        /// Get the player who already mad a move.
        /// </summary>
        /// <returns>Previous player</returns>
        public Player GetPreviousPlayerTurn()
        {
            return PlayerTurn == userPlayer ? opponentPlayer : userPlayer;
        }
        
        private void AddListeners()
        {
            GameController.OnGameStarted += ResetViewTimer;
        }

        private void RemoveListeners()
        {
            GameController.OnGameStarted -= ResetViewTimer;
        }

        private void OnDestroy()
        {
            RemoveListeners();
        }
    }
}