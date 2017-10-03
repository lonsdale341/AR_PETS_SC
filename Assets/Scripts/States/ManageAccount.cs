using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace States
{
    class ManageAccount : BaseState
    {
        private Menus.ManageAccountGUI menuComponent;

        Firebase.Auth.FirebaseAuth auth;
        public override void Initialize()
        {
            auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
            InitializeUI();
        }

        private void InitializeUI()
        {
            if (menuComponent == null)
            {
                menuComponent = SpawnUI<Menus.ManageAccountGUI>(StringConstants.PrefabsManageAccountMenu);
            }
            ShowUI();
            string text;
            if (!string.IsNullOrEmpty(auth.CurrentUser.DisplayName))
            {
                text = auth.CurrentUser.DisplayName + '\n';
            }
            else
            {
                text = StringConstants.UploadScoreDefaultName + '\n';
            }
            text += auth.CurrentUser.Email;
            menuComponent.EmailText.text = text;

           
        }

        public override void Suspend()
        {
            HideUI();
        }

        public override StateExitValue Cleanup()
        {
            DestroyUI();
            return new StateExitValue(typeof(ManageAccount));
        }

        public override void HandleUIEvent(GameObject source, object eventData)
        {
            
            if (source == menuComponent.SignOutButton.gameObject)
            {
                auth.SignOut();
                manager.ClearStack(new Startup());
            }
            
            else if (source == menuComponent.MainButton.gameObject)
            {
                manager.PopState();
            }
        }
    }


}
 