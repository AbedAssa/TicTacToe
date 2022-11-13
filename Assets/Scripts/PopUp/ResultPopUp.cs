using TMPro;
using UnityEngine;

namespace PopUp
{
    /// <summary>
    /// This class for the result popup that include text.
    /// </summary>
    public class ResultPopUp : PopUpBase
    {
        [Header("Result custom elements")]
        [SerializeField] private TextMeshProUGUI textMessage;  //The text element displayed in the popup
        private void SetTextMessage(ResultPopUpConfiguration configuration)
        {
            if (textMessage == null)
            {
                Debug.LogError("label is null");
                return;
            }

            textMessage.text = configuration.Message;
        }

        public override void SetConfiguration(PopUpConfiguration configuration)
        {
            base.SetConfiguration(configuration);
            SetTextMessage(configuration as ResultPopUpConfiguration);
        }
    }
}

