using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace States
{

    class Startup : BaseState
    {

        Firebase.Auth.FirebaseAuth auth;
        public const string DefaultDesktopID = "XYZZY";
        public override void Initialize()
        {
            Debug.Log("Init StartUp");
            //Time.timeScale = 0.0f;
            // When the game starts up, it needs to either download the user data
            // or create a new profile.
            auth = Firebase.Auth.FirebaseAuth.DefaultInstance;


            // On mobile, we go through the full auth system, to request a user id.
            // If we need to sign in, do that.  Otherwise, if we know who we are,
            // so fetch the user data.
            if (auth.CurrentUser == null)
            {
                manager.PushState(new ChooseSignInMenu());
            }
            else
            {
                manager.PushState(new States.FetchUserData(auth.CurrentUser.UserId));
            }
        }
        public override void Resume(StateExitValue results)
        {
            Debug.Log("resume StartUp");
            if (results.sourceState == typeof(ChooseSignInMenu) ||
                results.sourceState == typeof(WaitForTask))
            {
                Debug.Log("resume1 StartUp");
                // We just got back from trying to sign in anonymously.
                // Did it work?
                if (auth.CurrentUser != null)
                {
                    // Yes!  Continue!
                    manager.PushState(new FetchUserData(auth.CurrentUser.UserId));
                }
                else
                {
                    // Nope.  Couldn't sign in.
                    CommonData.isNotSignedIn = true;
                    manager.SwapState(new States.SelectModeState());
                }
            }
            else if (results.sourceState == typeof(FetchUserData))
            {
                
                
                    Debug.Log("resume5 StartUp");
                    manager.SwapState(new States.SelectModeState());

                    if (CommonData.currentUser == null)
                    {
                        Debug.Log("resume4 StartUp");
                        //  If we can't fetch data, tell the user.
                        manager.PushState(new BasicDialog(StringConstants.CouldNotFetchUserData));
                    }
                
            }
            else
            {
                throw new System.Exception("Returned from unknown state: " + results.sourceState);
            }
        }
    }


}
