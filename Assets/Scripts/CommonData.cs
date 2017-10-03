using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommonData 
{

	 public static PrefabList prefabs;

    public static PoseController poseController;
    public static MainManager mainManager;
    public static GameObject canvasHolder;
   
    public static string modeState;
    public static Firebase.FirebaseApp app;
    public static DBStruct<UserData> currentUser;
    // Whether we're signed in or not.
    public static bool isNotSignedIn = false;
    // Paths to various database tables:
    // Trailing slashes required, because in some cases
    // we append further paths onto these.
   
    public const string DBUserTablePath = "DB_Users/";

}
