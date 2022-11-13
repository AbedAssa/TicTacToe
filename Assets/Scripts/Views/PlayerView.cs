using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Views
{
    /// <summary>
    /// responsible of displaying player view on the screen.
    /// </summary>
    public class PlayerView : MonoBehaviour
    {
        [Header("Player view elements")]
        [SerializeField] private TextMeshProUGUI playerNameText;
        [SerializeField] private Image timeImage;
        private void Start()
        {
            SetTimeIndicatorFill(0f);
        }

        /// <summary>
        /// Set player name to the text
        /// </summary>
        /// <param name="playerName">Player name</param>
        public void SetPlayerName(string playerName)
        {
            if (playerNameText == null)
            {
                Debug.LogWarning("Player name text is null");
                return;
            }
            playerNameText.text = playerName;
        }

        /// <summary>
        /// Set fill amount to the time indicator view
        /// </summary>
        /// <param name="fillAmount">fill amount range(0-1)</param>
        public void SetTimeIndicatorFill(float fillAmount)
        {
            if (timeImage == null)
            {
                return;
            }

            timeImage.fillAmount = fillAmount;
            timeImage.color = Vector4.Lerp(Color.white, Color.red, fillAmount / 1.5f);
        }
    }
}

