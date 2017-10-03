using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace States
{
    class SelectFriendsMenu : BaseState
    {
        protected Menus.SelectFriendsMenuGUI menuComponent;
        public override void Initialize()
        {
            menuComponent = SpawnUI<Menus.SelectFriendsMenuGUI>(StringConstants.PrefabsSelectFriendStateMenu);
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
            Menus.SelectFriendsMenuGUI buttonComponent = source.GetComponent<Menus.SelectFriendsMenuGUI>();
            if (source == menuComponent.Friend_1_Button.gameObject)
            {
                Debug.Log(source.name);
                CommonData.currentUser.data.nameFriendPet = "cat_1";
                CommonData.modeState = "facebook";
                manager.PushState(new AR());
            }
            else if (source == menuComponent.Friend_2_Button.gameObject)
            {
                Debug.Log(source.name);
                CommonData.currentUser.data.nameFriendPet = "cat_2";
                CommonData.modeState = "facebook";
                manager.PushState(new AR());
            }
            else if (source == menuComponent.Friend_3_Button.gameObject)
            {
                Debug.Log(source.name);
                CommonData.currentUser.data.nameFriendPet = "puppy_1";
                CommonData.modeState = "facebook";
                manager.PushState(new AR());
            }
            else if (source == menuComponent.Friend_4_Button.gameObject)
            {
                Debug.Log(source.name);
                CommonData.currentUser.data.nameFriendPet = "puppy_2";
                CommonData.modeState = "facebook";
                manager.PushState(new AR());
            }
            else if (source == menuComponent.MainMenu_Button.gameObject)
            {
                Debug.Log(source.name);
                CommonData.modeState = "simple";
                manager.SwapState(new AR());
                //manager.PopState();
            }

        }

        public override void Resume(StateExitValue results)
        {
            ShowUI();

        }
        
    }

}
 
