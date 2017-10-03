using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace States
{
    class SelectModeState : BaseState
    {
        protected Menus.SelectModeStateGUI menuComponent;
        public override void Initialize()
        {
            menuComponent = SpawnUI<Menus.SelectModeStateGUI>(StringConstants.PrefabsSelectModeStateMenu);
            if (string.IsNullOrEmpty(CommonData.currentUser.data.nameMyPet))
            {
                menuComponent.PlayButton.gameObject.SetActive(false);
               // menuComponent.FacebookButton.gameObject.SetActive(false);
            }
            else
            {
                menuComponent.PlayButton.gameObject.SetActive(true);
                //menuComponent.FacebookButton.gameObject.SetActive(true);
            }
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
            if (source == menuComponent.ChooseButton.gameObject)
            {
                Debug.Log(source.name);
                manager.SwapState(new SelectPetMenu());
            }
            else if (source == menuComponent.PlayButton.gameObject)
            {
                Debug.Log(source.name);
                CommonData.modeState = "simple";
                manager.PushState(new AR());
                
            }
            else if (source == menuComponent.FacebookButton.gameObject)
            {
                Debug.Log(source.name);
                CommonData.modeState = "facebook";
                manager.PushState(new SelectFriendsMenu());
                
            }
            else if (source == menuComponent.AccountButton.gameObject)
            {
                Debug.Log(source.name);
                manager.PushState(new ManageAccount());
            }
            


        }

        public override void Resume(StateExitValue results)
        {
            ShowUI();

        }
    }


}
