using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Menus
{
    // Interface class for providing code access to the GUI
    // elements in the main menu prefab.
    public class BasicDialogGUI : BaseMenu
    {
        // These fields are set in the inspector.
        public GUIButton OkayButton;
        public UnityEngine.UI.Text DialogText;
        
    }


}
