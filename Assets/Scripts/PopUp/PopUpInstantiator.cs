using UnityEngine;

namespace PopUp
{
    /// <summary>
    /// This class responsible of instantiating a popups. 
    /// </summary>
    public class PopUpInstantiator : MonoBehaviour
    {
        [Header("Popups canvas")]
        [SerializeField] private Transform modalCanvas;       //The popup canvas.
        [Header("Popups prefabs")]
        [SerializeField] private MenuPopUp menuPopUp;         //Menu popup prefab.
        [SerializeField] private ResultPopUp resultPopUp;     //Result popup prefab object.
        [SerializeField] private SettingsPopUp settingsPopUp; //Settings popup prefab object.
        public static PopUpInstantiator Instance { get; private set; } //Instance (singleton)
        
        private void Awake()
        {
            Instance = this;
        }

        /// <summary>
        /// Instantiate popup with the given configurations.
        /// </summary>
        /// <param name="configurations"></param>
        public void TriggerPopUp(PopUpConfiguration configurations)
        {
            if (configurations == null)
            {
                Debug.LogWarning("Trigger popup was ignored, configuration is null");
                return;
            }
            PopUpBase popUp = Instantiate(GetDesiredPrefab(configurations), modalCanvas);
            popUp.SetConfiguration(configurations);
        }

        /// <summary>
        /// Get the proper prefab to be instantiated depends on the given configurations.
        /// </summary>
        /// <param name="configurations"></param>
        /// <returns></returns>
        private PopUpBase GetDesiredPrefab(PopUpConfiguration configurations)
        {
            if (configurations.GetType() == typeof(MenuPopUpConfiguration))
            {
                return menuPopUp;
            }
            if (configurations.GetType() == typeof(ResultPopUpConfiguration))
            {
                return resultPopUp;
            }
            if (configurations.GetType() == typeof(PopUpConfiguration))
            {
                return settingsPopUp;
            }
            return null;
        }
    }
}