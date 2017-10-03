using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace States
{
    // Utility state for fetching the user data.  (Or making a new user
    // profile if data could not be fetched.)
    // Basically just does the fetch, and then returns the result to whatever
    // state invoked it.
    class FetchUserData : BaseState
    {
        private string userID;

        public FetchUserData(string userID)
        {
            this.userID = userID;
        }

        // This state is basically just a more convenient way to use the WaitingForDBLoad
        // state to get user data, and then handle the logic of what to do with the results.

        public override void Initialize()
        {
            Debug.Log("Init FetchUserData");
            manager.PushState(
              new WaitingForDBLoad<UserData>(CommonData.DBUserTablePath + userID));
        }

        public override StateExitValue Cleanup()
        {
            return new StateExitValue(typeof(FetchUserData), null);
        }

        // Resume the state.  Called when the state becomes active
        // when the state above is removed.  That state may send an
        // optional object containing any results/data.  Results
        // can also just be null, if no data is sent.
        public override void Resume(StateExitValue results)
        {
            Debug.Log("Resume FetchUserData");
            if (results != null)
            {
                if (results.sourceState == typeof(WaitingForDBLoad<UserData>))
                {
                    Debug.Log("Resume FetchUserData_1");
                    var resultData = results.data as WaitingForDBLoad<UserData>.Results;
                    if (resultData.wasSuccessful)
                    {
                        Debug.Log("Resume FetchUserData_2");
                        if (resultData.results != null)
                        {
                            Debug.Log("Resume FetchUserData_3");
                            // Got some results back!  Use this data.
                            CommonData.currentUser = new DBStruct<UserData>(
                                CommonData.DBUserTablePath + userID, CommonData.app);
                            CommonData.currentUser.Initialize(resultData.results);
                            Debug.Log("Fetched user " + CommonData.currentUser.data.nameUser);
                        }
                        else
                        {
                            Debug.Log("Resume FetchUserData_4");
                            // Make a new user, using default credentials.
                            Debug.Log("Could not find user " + userID + " - Creating new profile.");
                            UserData temp = new UserData();
                            temp.nameUser = Firebase.Auth.FirebaseAuth.DefaultInstance.CurrentUser.DisplayName;
                            temp.id = userID;
                            temp.nameMyPet = "";

                            CommonData.currentUser = new DBStruct<UserData>(
                            CommonData.DBUserTablePath + userID, CommonData.app);
                            CommonData.currentUser.Initialize(temp);
                            CommonData.currentUser.PushData();
                        }
                    }
                    else
                    {
                        Debug.Log("Resume FetchUserData_5");
                        // Can't fetch data.  Assume internet problems, stay offline.
                        CommonData.currentUser = null;
                    }
                }
            }
            // Whether we successfully fetched, or had to make a new user,
            // return control to the calling state.
            manager.PopState();
        }

    }

}

