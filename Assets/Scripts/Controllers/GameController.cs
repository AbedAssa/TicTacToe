using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using PopUp;
using Infra;

namespace Controllers
{
    public class GameController : MonoBehaviour
    {
        public static Action OnGameStarted;
        [Header("Game default settings")]
        [SerializeField] private bool isAudioOn = true;
        [SerializeField] private bool isTimeOn = true;
        [SerializeField] private int timeCount = 5;
        [Header("Fields")]
        [SerializeField] private Button undoButton;
        [SerializeField] private Button restartGameButton;
        [SerializeField] private Button hintButton;
        [SerializeField] private Button settingsButton;
        private const int DelayBeforeResultPopUp = 2000;
        private BoardController boardController;

        private void Start()
        {
            SetGameDefaultSettings();
            AddListeners();
            SetInteractions(false);
        }

        /// <summary>
        /// Trigger menu pop up to allow user chose his opponent.
        /// </summary>
        private void ShowMenuPopUp()
        {
            MenuPopUpConfiguration popUpConfiguration = new MenuPopUpConfiguration()
            {
                ShowBackground = true,
                ButtonsList = new List<DefaultButtonConfiguration>()
                {
                    new DefaultButtonConfiguration()
                    {
                        ButtonText = "Two players",
                        OnButtonClicked = UserClickTwoPlayers,
                        ClosePopUp = true
                    },
                    new DefaultButtonConfiguration()
                    {
                        ButtonText = "Computer",
                        OnButtonClicked = UserClickComputer,
                        ClosePopUp = true
                    },
                },
            };
            PopUpInstantiator.Instance.TriggerPopUp(popUpConfiguration);
        }
        
        /// <summary>
        /// The pop up that appears at the end of the game to display game result to the user
        /// </summary>
        /// <param name="message">the result</param>
        private void ShowResultPopUp(string message)
        {
            ResultPopUpConfiguration resultPopUpConfiguration = new ResultPopUpConfiguration()
            {
                ShowBackground = true,
                SubmitButtonConfiguration = new DefaultButtonConfiguration()
                {
                    ButtonText = "OK",
                    ClosePopUp = true
                },
                Message = message,
            };
            PopUpInstantiator.Instance.TriggerPopUp(resultPopUpConfiguration);
        }

        /// <summary>
        /// Trigger the settings popup to allow user change the game configurations.
        /// </summary>
        private void ShowSettingsPopUp()
        {
            PopUpConfiguration popUpConfiguration = new PopUpConfiguration()
            {
                ShowBackground = true,
                SubmitButtonConfiguration = new DefaultButtonConfiguration()
                {
                    ButtonText = "Save",
                    ClosePopUp = true
                },
            };
            PopUpInstantiator.Instance.TriggerPopUp(popUpConfiguration);
        }

        /// <summary>
        /// Get called when user choose to play with another player
        /// </summary>
        private void UserClickTwoPlayers()
        {
            RestartGame(PlayerType.User);
        }
        
        /// <summary>
        /// Get called when user choose to play with computer
        /// </summary>
        private void UserClickComputer()
        {
            RestartGame(PlayerType.Computer);
        }
        
        /// <summary>
        /// Called when user click undo
        /// </summary>
        private void UserClickUndo()
        {
            boardController.UndoMove();
            AudioController.Instance.PlaySound(AudioTypes.ButtonClick);
        }
        
        /// <summary>
        /// Get called when user click restart button to restart the game.
        /// </summary>
        private void UserClickRestartGame()
        {
            settingsButton.gameObject.SetActive(false);
            SetInteractions(false);
            AudioController.Instance.PlaySound(AudioTypes.ButtonClick);
            ShowMenuPopUp();
            OnGameStarted?.Invoke();
        }

        /// <summary>
        /// Get called when user click settings button.
        /// </summary>
        private void UserClickSettingsButton()
        {
            AudioController.Instance.PlaySound(AudioTypes.ButtonClick);
            ShowSettingsPopUp();
        }

        /// <summary>
        /// Get called when user hint button.
        /// </summary>
        private void UserClickHintButton()
        {
            AudioController.Instance.PlaySound(AudioTypes.ButtonClick);
            boardController.HighlightHintMove();
        }
        
        /// <summary>
        /// Restart the game.
        /// </summary>
        /// <param name="opponentType">computer or user</param>
        private void RestartGame(PlayerType opponentType)
        {
            int? gameTime = GetGameTime();
            boardController ??= new BoardController(PlayerController.Instance, gameTime);
            PlayerController.Instance.ResetPlayers(opponentType);
            boardController.ResetBoard(gameTime);
            SetInteractions(true);
        }

        /// <summary>
        /// Disable/Enable undo and hint buttons
        /// </summary>
        /// <param name="isActive">enabled or disabled</param>
        private void SetInteractions(bool isActive)
        {
            undoButton.interactable = isActive;
            hintButton.interactable = isActive;
        }

        /// <summary>
        /// Triggered when game is over, if winner player is null then it is a draw.
        /// </summary>
        /// <param name="winningPlayer">winner player</param>
        /// <param name="byTimeOut">indicate won by time out</param>
        private void GameOver(Player winningPlayer,bool byTimeOut)
        {
            SetInteractions(false);
            if (winningPlayer == null)
            {
                ShowResultPopUp("Draw");
                settingsButton.gameObject.SetActive(true);
            }
            else
            {
                DelayBeforePopUp(winningPlayer,byTimeOut);
            }
        }
        
        /// <summary>
        /// delay before showing the result popup to show the winning animation.
        /// </summary>
        /// <param name="winningPlayer"></param>
        /// <param name="byTimeOut"></param>
        private async void DelayBeforePopUp(Player winningPlayer,bool byTimeOut)
        {
            AudioController.Instance.PlaySound(AudioTypes.GameOver);
            if (!byTimeOut)
            {
                await Task.Delay(DelayBeforeResultPopUp);
            }
            ShowResultPopUp($"{winningPlayer.PlayerName} WON!");
            settingsButton.gameObject.SetActive(true);
        }
        
        /// <summary>
        /// Set default game settings in the player prefs.
        /// </summary>
        private void SetGameDefaultSettings()
        {
            if (!PlayerPrefs.HasKey(PlayerPrefsKeys.AudioKey))
            {
                PlayerPrefs.SetInt(PlayerPrefsKeys.AudioKey,isAudioOn ? 1 :0);
            }
            if (!PlayerPrefs.HasKey(PlayerPrefsKeys.TimeKey))
            {
                PlayerPrefs.SetInt(PlayerPrefsKeys.TimeKey,isTimeOn ? 1 :0);
            }
            if (!PlayerPrefs.HasKey(PlayerPrefsKeys.TimeValueKey))
            {
                PlayerPrefs.SetInt(PlayerPrefsKeys.TimeValueKey,timeCount);
            }
        }

        /// <summary>
        /// Get the time that specified by the user, if time turned of the function will return null.
        /// </summary>
        /// <returns>time or null</returns>
        private int? GetGameTime()
        {
            if (!PlayerPrefs.HasKey(PlayerPrefsKeys.TimeKey) || !PlayerPrefs.HasKey(PlayerPrefsKeys.TimeValueKey))
            {
                return null;
            }

            if (PlayerPrefs.GetInt(PlayerPrefsKeys.TimeKey) == 0)
            {
                return null;
            }

            return PlayerPrefs.GetInt(PlayerPrefsKeys.TimeValueKey);
        }
        
        private void AddListeners()
        {
            undoButton.onClick.AddListener(UserClickUndo);
            restartGameButton.onClick.AddListener(UserClickRestartGame);
            settingsButton.onClick.AddListener(UserClickSettingsButton);
            hintButton.onClick.AddListener(UserClickHintButton);
            BoardController.OnGameOver += GameOver;
        }

        private void RemoveListeners()
        {
            BoardController.OnGameOver -= GameOver;
            undoButton.onClick.RemoveAllListeners();
            restartGameButton.onClick.RemoveAllListeners();
            settingsButton.onClick.RemoveAllListeners();
            hintButton.onClick.RemoveAllListeners();
        }

        private void OnDestroy()
        {
            RemoveListeners();
        }
    } 
}

