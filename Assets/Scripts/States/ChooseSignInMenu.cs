using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace States
{
    class ChooseSignInMenu : BaseState
    {

        Firebase.Auth.FirebaseAuth auth;
        Menus.ChooseSignInGUI dialogComponent;

        public override StateExitValue Cleanup()
        {
            DestroyUI();
            return new StateExitValue(typeof(ChooseSignInMenu), null);
        }
        public override void Resume(StateExitValue results)
        {
            ShowUI();

            // SignInResult is used with a email/password, while WaitForTask.Results
            // is used when signing in with an anonymous account.
            // SignInResult используется с адресом электронной почты / паролем, а WaitForTask.Results используется при входе в анонимную учетную запись.
            SignInResult result = results.data as SignInResult;
           

            if (result != null && !result.Canceled)
            {

                if (auth.CurrentUser != null)
                {
                    CommonData.isNotSignedIn = false;
                    manager.PopState();
                }
                
            }
        }
        public override void Suspend()
        {
            HideUI();
        }

        // Initialization method.  Called after the state
        // is added to the stack.
        public override void Initialize()
        {
            Debug.Log("Init ChooseSignInMenu");
            auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
            dialogComponent = SpawnUI<Menus.ChooseSignInGUI>(StringConstants.PrefabsChooseSigninMenu);
        }

        public override void HandleUIEvent(GameObject source, object eventData)
        {
            if (source == dialogComponent.CreateAccountButton.gameObject)
            {
                manager.PushState(new CreateAccount());
            }
            else if (source == dialogComponent.SignInButton.gameObject)
            {
                manager.PushState(new SignInWithEmail());
            }
            
        }
    }
    public class SignInResult
    {
        public bool Canceled = false;
        public SignInResult(bool canceled)
        {
            this.Canceled = canceled;
        }

    }

}
