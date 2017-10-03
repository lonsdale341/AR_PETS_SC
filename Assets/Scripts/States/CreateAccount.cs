using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace States
{
    class CreateAccount : BaseState
    {

        Firebase.Auth.FirebaseAuth auth;
        Menus.CreateAccountGUI dialogComponent;
        bool canceled = false;

        public override void Initialize()
        {
            Debug.Log("Init CreateAccount");
            auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
            dialogComponent = SpawnUI<Menus.CreateAccountGUI>(StringConstants.PrefabsNewAccountMenu);
        }

        public override void Resume(StateExitValue results)
        {
            Debug.Log("Resume CreateAccount");
            ShowUI();
            if (results != null)
            {
                Debug.Log("Resume 1 CreateAccount");
                if (results.sourceState == typeof(WaitForTask))
                {
                    Debug.Log("Resume 2 CreateAccount");
                    WaitForTask.Results taskResults = results.data as WaitForTask.Results;
                    if (taskResults.task.IsFaulted)
                    {
                        Debug.Log("Resume 3 CreateAccount");
                        manager.PushState(new BasicDialog("Could not create account."));
                    }
                    else
                    {
                        Debug.Log("Resume 4 CreateAccount");
                        if (!string.IsNullOrEmpty(dialogComponent.DisplayName.text))
                        {
                            Debug.Log("Resume 5 CreateAccount");
                            Firebase.Auth.UserProfile profile =
                              new Firebase.Auth.UserProfile();
                            profile.DisplayName = dialogComponent.DisplayName.text;
                            // We are fine with this happening in the background,
                            // so just return to the previous state after triggering the update.
                            auth.CurrentUser.UpdateUserProfileAsync(profile);
                        }
                        manager.PopState();
                    }
                }
            }
        }

        public override void Suspend()
        {
            HideUI();
        }

        public override StateExitValue Cleanup()
        {
            DestroyUI();
            return new StateExitValue(typeof(CreateAccount), new SignInResult(canceled));
        }

        public override void HandleUIEvent(GameObject source, object eventData)
        {
            if (source == dialogComponent.CancelButton.gameObject)
            {
                canceled = true;
                manager.PopState();
            }
            else if (source == dialogComponent.ContinueButton.gameObject)
            {
                Debug.Log("email=" + dialogComponent.Email.text + "  pass=" + dialogComponent.Password.text);
                manager.PushState(new WaitForTask(auth.CreateUserWithEmailAndPasswordAsync(
                    dialogComponent.Email.text, dialogComponent.Password.text)));
            }
        }
    }

}
 
