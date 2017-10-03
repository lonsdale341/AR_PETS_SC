using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace States
{
    // State for basic dialog boxes on the screen.
    // Simply displays a message, and waits for the user to click the
    // button, and then returns to the previous state.
    class BasicDialog : BaseState
    {
        string dialogText;
        Menus.BasicDialogGUI dialogComponent;

        public BasicDialog(string dialogText)
        {
            this.dialogText = dialogText;
        }

        public override void Initialize()
        {
            Debug.Log("Init BasicDialog");
            dialogComponent = SpawnUI<Menus.BasicDialogGUI>(StringConstants.PrefabBasicDialog);
            dialogComponent.DialogText.text = dialogText;
        }

        public override void Resume(StateExitValue results)
        {
            ShowUI();
        }

        public override void Suspend()
        {
            HideUI();
        }

        public override StateExitValue Cleanup()
        {
            DestroyUI();
            return null;
        }

        public override void HandleUIEvent(GameObject source, object eventData)
        {
            if (source == dialogComponent.OkayButton.gameObject)
            {
                manager.PopState();
            }
        }
        
    }


}
 