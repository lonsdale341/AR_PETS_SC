using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace States
{
    class WarningChosePet : BaseState
    {

        protected Menus.WarningChosePetGUI menuComponent;
        public override void Initialize()
        {
            menuComponent = SpawnUI<Menus.WarningChosePetGUI>(StringConstants.PrefabsWarningChoosePetMenu);
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
            Menus.SelectModeStateGUI buttonComponent = source.GetComponent<Menus.SelectModeStateGUI>();
            if (source == menuComponent.CancelButton.gameObject)
            {
                Debug.Log(source.name);
                CommonData.modeState = "simple";
                manager.SwapState(new SelectModeState());
            }
            



        }

        public override void Resume(StateExitValue results)
        {
            ShowUI();

        }
    }

}
 