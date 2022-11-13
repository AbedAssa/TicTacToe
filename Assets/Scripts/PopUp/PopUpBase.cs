using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Controllers;
using Infra;

namespace PopUp
{
    /// <summary>
    /// Abstract base class for all popups in the app, the base class includes all logic are shared
    /// between the popups, all popup classes most inherent from this class.
    /// </summary>
    public abstract class PopUpBase : MonoBehaviour
    {
        [SerializeField] private float delayBeforeDestroy;           //Delay before destroy the popup.
        [SerializeField] private Animator animator;                  //The popup animator attached component.
        [SerializeField] private GameObject background;              //The dimmed background object.
        [SerializeField] private Button closeButton;                 //Popup close button.
        [SerializeField] private Button submitButton;                //Popup submit button.
        [SerializeField] private string hideAnimClipName = "PopHide";//The name of the animation clip to hide the popup.
        private PopUpConfiguration popUpConfiguration;               //The received popup configuration to be set. 
        
        /// <summary>
        /// Show or hide the dimmed background according to the received configuration
        /// </summary>
        /// <param name="showBackground">true to show false to hide</param>
        private void SetBackgroundIfNeeded(bool showBackground)
        {
            if (background == null)
            {
                Debug.LogWarning("background reference is null");
                return;
            }
            background.SetActive(showBackground);
        }

        /// <summary>
        /// Responsible of displaying or hiding the close button of the popup.
        /// </summary>
        /// <param name="configuration">popup configuration</param>
        private void SetCloseButtonIfNeeded(PopUpConfiguration configuration)
        {
            if (closeButton == null)
            {
                Debug.LogWarning("close button reference is null");
                return;
            }
            if (configuration.CloseButtonConfiguration == null)
            {
                if (closeButton.transform.parent != null)
                {
                    closeButton.transform.parent.gameObject.SetActive(false);
                }
                closeButton.gameObject.SetActive(false);
            }
            else
            {
                closeButton.gameObject.SetActive(true);
                closeButton.onClick.AddListener(() =>
                {
                    ClosePopUp();
                    AudioController.Instance.PlaySound(AudioTypes.ButtonClick);
                    configuration.CloseButtonConfiguration.OnCloseClicked?.Invoke();
                });
            }
        }

        /// <summary>
        /// Responsible of displaying or hiding the submit button of the pop up,
        /// and add listener to the button and invoke corresponding actions.
        /// </summary>
        /// <param name="configuration">popup configuration</param>
        private void SetSubmitButton(PopUpConfiguration configuration)
        {
            if (configuration.SubmitButtonConfiguration == null)
            {
                if (submitButton.transform.parent != null)
                {
                    submitButton.transform.parent.gameObject.SetActive(false);
                }
                return;
            }

            AddButtonText(submitButton, configuration.SubmitButtonConfiguration);
            if (configuration.SubmitButtonConfiguration.ClosePopUp)
            {
                AddButtonListener(submitButton, configuration.SubmitButtonConfiguration);
            }
        }

        /// <summary>
        /// Close the popup with animation.
        /// </summary>
        private void ClosePopUp()
        {
            if (animator != null)
            {
                animator.Play(hideAnimClipName);
                StartCoroutine(DelayBeforeDestroy(delayBeforeDestroy));
            }
            else
            {
                StartCoroutine(DelayBeforeDestroy(0f)); 
            }
        }

        /// <summary>
        /// Destroy the game object after delay (after hiding popup animation is over).
        /// </summary>
        private IEnumerator DelayBeforeDestroy(float delay)
        {
            float passedTime = 0;
            while (passedTime < delay)
            {
                passedTime += Time.deltaTime;
                yield return null;
            }
            Destroy(gameObject);
        }

        /// <summary>
        /// Virtual function to set the configurations to the popup, override this function
        /// in the derived classes to implement more custom popups. 
        /// </summary>
        /// <param name="configuration"></param>
        public virtual void SetConfiguration(PopUpConfiguration configuration)
        {
            popUpConfiguration = configuration;
            SetSubmitButton(configuration);
            SetCloseButtonIfNeeded(configuration);
            SetBackgroundIfNeeded(configuration.ShowBackground);
            configuration.OnPopUpOpen?.Invoke();
        }
        
        /// <summary>
        /// Add a listener to the button and invoke the relative event.
        /// </summary>
        /// <param name="button">The button</param>
        /// <param name="buttonConfiguration">Button configuration</param>
        protected void AddButtonListener(Button button,DefaultButtonConfiguration buttonConfiguration)
        {
            if (buttonConfiguration == null)
            {
                return;
            }
            button.onClick.AddListener(() =>
            {
                if (buttonConfiguration.ClosePopUp)
                {
                    ClosePopUp();
                }
                AudioController.Instance.PlaySound(Infra.AudioTypes.ButtonClick);
                buttonConfiguration.OnButtonClicked?.Invoke();
            });
        }
        
        /// <summary>
        /// Static function to update the button text.
        /// </summary>
        /// <param name="button">The button</param>
        /// <param name="buttonConfiguration">Button configuration</param>
        protected void AddButtonText(Button button,DefaultButtonConfiguration buttonConfiguration)
        {
            if (buttonConfiguration == null)
            {
                return;
            }

            if (button == null)
            {
                Debug.LogWarning("Can't set text to the button, button is null");
                return;
            }

            TextMeshProUGUI label = button.GetComponentInChildren<TextMeshProUGUI>();
            if (label == null)
            {
                Debug.LogWarning("Button has no child with TextMeshPro component attached to it");
                return;
            }

            label.text = buttonConfiguration.ButtonText;
        }
        
        /// <summary>
        /// Destroy the game object and invoke PopUp close event.
        /// </summary>
        private void OnDestroy()
        {
            popUpConfiguration?.OnPopUpClose?.Invoke();
        }
    }
}

