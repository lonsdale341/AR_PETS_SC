using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace States
{
    class PasswordReset : BaseState
    {

        Firebase.Auth.FirebaseAuth auth;
        Menus.PasswordResetGUI dialogComponent;
        bool canceled = false;

        public override void Initialize()
        {
            auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
            dialogComponent = SpawnUI<Menus.PasswordResetGUI>(StringConstants.PrefabsPasswordResetMenu);
        }

        public override void Suspend()
        {
            HideUI();
        }

        public override void Resume(StateExitValue results)
        {
            ShowUI();
            if (results != null)
            {
                if (results.sourceState == typeof(WaitForTask))
                {
                    WaitForTask.Results taskResults = results.data as WaitForTask.Results;
                    if (taskResults.task.IsFaulted)
                    {
                        manager.SwapState(new BasicDialog(StringConstants.SignInPasswordResetError));
                    }
                    else
                    {
                        manager.SwapState(new BasicDialog(string.Format(
                            StringConstants.SignInPasswordReset, dialogComponent.Email.text)));
                    }
                }
            }
        }

        public override StateExitValue Cleanup()
        {
            DestroyUI();
            return new StateExitValue(typeof(SignInWithEmail), new SignInResult(canceled));
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
                manager.PushState(new WaitForTask(
                    auth.SendPasswordResetEmailAsync(dialogComponent.Email.text)));
            }
        }
    }


}
