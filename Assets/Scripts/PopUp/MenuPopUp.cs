using UnityEngine;
using UnityEngine.UI;

namespace PopUp
{
    public class MenuPopUp : PopUpBase
    {
        /// <summary>
        /// This class for the menu popup that including two buttons.
        /// </summary>
        [Header("Menu custom elements")]
        [SerializeField] private Button topButton;    
        [SerializeField] private Button bottomButton;
        
        /// <summary>
        /// Set two buttons to the popup, and add listeners and update the buttons text,
        /// </summary>
        /// <param name="configuration"></param>
        private void SetButtons(MenuPopUpConfiguration configuration)
        {
            if (configuration.ButtonsList == null)
            {
                return;
            }

            if (configuration.ButtonsList.Count != 2)
            {
                Debug.LogError("Menu Popup support only 2 buttons");
                return;
            }
            AddButtonText(topButton,configuration.ButtonsList[0]);
            AddButtonText(bottomButton,configuration.ButtonsList[1]);
            AddButtonListener(topButton,configuration.ButtonsList[0]);
            AddButtonListener(bottomButton,configuration.ButtonsList[1]);
        }

        /// <summary>
        /// Override SetConfiguration method and add more functionality.
        /// </summary>
        /// <param name="configuration"></param>
        public override void SetConfiguration(PopUpConfiguration configuration)
        {
            base.SetConfiguration(configuration);
            SetButtons(configuration as MenuPopUpConfiguration);
        }
    }
}

