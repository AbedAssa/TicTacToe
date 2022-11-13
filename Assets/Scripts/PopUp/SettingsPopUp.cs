using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Infra;

namespace PopUp
{
    /// <summary>
    /// This class for the settings popup
    /// </summary>
    public class SettingsPopUp : PopUpBase
    {
        [Header("Settings custom elements")]
        [SerializeField] private Toggle audioToggle;            //Toggle to switch audio on/off
        [SerializeField] private Toggle timeToggle;             //Toggle to switch time on/off
        [SerializeField] private Slider timeSlider;             //Slider to control time count.
        [SerializeField] private TextMeshProUGUI timeValueText; //Text to display the time value.
        
        public override void SetConfiguration(PopUpConfiguration configuration)
        {
            base.SetConfiguration(configuration);
            SetValues();
            SetListener();
        }

        /// <summary>
        /// Set saved values from player prefs to the popup elements
        /// </summary>
        private void SetValues()
        {
            audioToggle.isOn = PlayerPrefs.GetInt(PlayerPrefsKeys.AudioKey) != 0;
            timeToggle.isOn = PlayerPrefs.GetInt(PlayerPrefsKeys.TimeKey) != 0;
            int sliderValue = PlayerPrefs.GetInt(PlayerPrefsKeys.TimeValueKey);
            timeSlider.value = sliderValue;
            timeValueText.text = sliderValue.ToString();
        }

        /// <summary>
        /// Set listeners for the settings elements.
        /// </summary>
        private void SetListener()
        {
            SetToggleListener(audioToggle,PlayerPrefsKeys.AudioKey);
            SetToggleListener(timeToggle,PlayerPrefsKeys.TimeKey);
            timeSlider.onValueChanged.AddListener(SliderValueChanged);
        }

        /// <summary>
        /// Set listener for a given toggle.
        /// </summary>
        /// <param name="toggle">the toggle object</param>
        /// <param name="playerPrefKey">the key of the playerPrefs</param>
        private void SetToggleListener(Toggle toggle,string playerPrefKey)
        {
            if (toggle == null)
            {
                return;
            }
            toggle.onValueChanged.AddListener(delegate { ToggleValueChanged(toggle,playerPrefKey); });
        }

        /// <summary>
        /// Get called when toggle value changed to save to the new value to the playerPrefs.
        /// </summary>
        /// <param name="toggle">the toggle object</param>
        /// <param name="playerPrefKey">the key of the playerPrefs for the toggle value</param>
        private void ToggleValueChanged(Toggle toggle,string playerPrefKey)
        {
            PlayerPrefs.SetInt(playerPrefKey,toggle.isOn ? 1 : 0);
        }

        /// <summary>
        /// Get called when the slider value changed to save the new value to PlayerPrefs.
        /// </summary>
        /// <param name="sliderValue">The new value</param>
        private void SliderValueChanged(float sliderValue)
        {
            int newValue = (int) sliderValue;
            PlayerPrefs.SetInt(PlayerPrefsKeys.TimeValueKey,newValue);
            timeValueText.text = newValue.ToString();
        }
    }
}

