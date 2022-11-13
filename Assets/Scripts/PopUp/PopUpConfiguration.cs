using System;
using System.Collections.Generic;

namespace PopUp
{
    /// <summary>
    /// This is the base class for each popup configuration that include all shared logic and component
    /// new configuration most inherent from this class.
    /// </summary>
    public class PopUpConfiguration
    {
        public bool ShowBackground;                                 //The dimmed background behind the popup panel.                              
        public CloseButtonConfiguration CloseButtonConfiguration;   //Configuration for the top right close button in the popup panel.
        public DefaultButtonConfiguration SubmitButtonConfiguration;//Configuration for the submit button that located at the bottom of the popup panel
        public Action OnPopUpClose;                                 //Action get called when the popup instantiated.
        public Action OnPopUpOpen;                                  //Action get called when the popup closed.
    }
    
    /// <summary>
    /// A configuration for the close button
    /// </summary>
    public class CloseButtonConfiguration
    {
        public Action OnCloseClicked;   //Extra action to be called when the button clicked.
    }

    /// <summary>
    /// A configuration for default button that could be instantiated in the popup panel.
    /// </summary>
    public class DefaultButtonConfiguration
    {
        public Action OnButtonClicked;  //Action to be called when this button clicked.
        public string ButtonText;       //The button displayed text.
        public bool ClosePopUp;         //Indicate if clicking the button should destroy the panel.
    }
    
    /// <summary>
    /// A configuration specific for a popup that contains buttons only (including close button and submit button).
    /// </summary>
    public class MenuPopUpConfiguration : PopUpConfiguration
    {
        public List<DefaultButtonConfiguration> ButtonsList;
    }

    /// <summary>
    /// A configuration for a popup that contain only text.
    /// </summary>
    public class ResultPopUpConfiguration : PopUpConfiguration
    {
        public string Message;   //The message displayed in the popup
    }
}