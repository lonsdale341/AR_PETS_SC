using System.Collections;
using System.Collections.Generic;
using Firebase.Unity.Editor;
using UnityEngine;


public class MainManager : MonoBehaviour {
    [HideInInspector]
    public States.StateManager stateManager = new States.StateManager();
	// Use this for initialization
	void Start ()
	{
	    InitializeAndStart();
	}
	
	// Update is called once per frame
	void Update () {
        stateManager.Update();
	}
    void FixedUpdate()
    {
        stateManager.FixedUpdate();
    }
    void InitializeAndStart()
    {
        InitializeFirebaseAndStart();
       
        
    }
    // When the app starts, check to make sure that we have
    // the required dependencies to use Firebase, and if not,
    // add them if possible.
    void InitializeFirebaseAndStart()
    {
        Firebase.DependencyStatus dependencyStatus = Firebase.FirebaseApp.CheckDependencies();

        if (dependencyStatus != Firebase.DependencyStatus.Available)
        {
            Firebase.FirebaseApp.FixDependenciesAsync().ContinueWith(task =>
            {
                dependencyStatus = Firebase.FirebaseApp.CheckDependencies();
                if (dependencyStatus == Firebase.DependencyStatus.Available)
                {
                    //InitializeFirebaseComponents();
                   // StartGame();
                }
                else
                {
                    Debug.LogError(
                        "Could not resolve all Firebase dependencies: " + dependencyStatus);
                    Application.Quit();
                }
            });
        }
        else
        {
            //InitializeFirebaseComponents();
            StartGame();
        }
    }
    void StartGame()
    {
         
         
        CommonData.prefabs = FindObjectOfType<PrefabList>();
        CommonData.canvasHolder = GameObject.Find("CanvasHolder");
        CommonData.mainManager = this;
        CommonData.poseController = GameObject.Find("ModelRoot").GetComponent<PoseController>();
        Firebase.AppOptions ops = new Firebase.AppOptions();
        CommonData.app = Firebase.FirebaseApp.Create(ops);
        CommonData.app.SetEditorDatabaseUrl("https://augemented-reality.firebaseio.com/");

        Screen.orientation = ScreenOrientation.Landscape;
      //  Screen.orientation = ScreenOrientation.Landscape;




     // UserData temp = new UserData();
     // temp.nameUser = StringConstants.DefaultUserName;
     // temp.id = StringConstants.DefaultUserId;
     // temp.nameMyPet = "";
     //
     // CommonData.currentUser = new DBStruct<UserData>(
     //   CommonData.DBUserTablePath + StringConstants.DefaultUserId, CommonData.app);
     // CommonData.currentUser.Initialize(temp);
     // stateManager.SwapState(new States.SelectModeState());
        stateManager.PushState(new States.Startup());
    }
}
