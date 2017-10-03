using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 class StringConstants {
     // Title screen text:
     public const string CouldNotFetchUserData = "Could not connect to user database.\n" +
       "Some features will be disabled.";
     // Default names:
     public const string DefaultUserId = "XYZ";
     public const string DefaultUserName = "Unnamed User";
     public const string DefaultPetName = "Unnamed Map";
     // Auth:
     //--------------------------
     public const string LabelLoading = "Loading";
     public const string SignInCanceled = "Could not sign in:\nSign in was canceled.";
     public const string SignInFailed = "Sign in failed.\n" +
         "Sign in can fail if you use an invalid username/password, " +
         "are not connected to the internet, or " +
         "if there is a problem connecting with our servers.";
     public const string LabelSigningIn = "Signing In";
     public const string SignInPasswordResetError = "Could not reset password.";

     public const string SignInSuccessful = "Sign in successful.";
     public const string SignInPasswordReset = "A password reset email has been sent to {0}";
     // Menu identifiers:
     //--------------------------
     public const string PrefabsSelectModeStateMenu = "SelectModeState";
     public const string PrefabsSelectPetStateMenu = "SelectPetState";
     public const string PrefabsSelectFriendStateMenu = "SelectFriendState";
     public const string PrefabsSimpleMenu = "SimpleMenu";
     public const string PrefabsARMenu = "AR_Meenu";
     public const string PrefabsWarningChoosePetMenu = "WarningChoosePet";
     public const string PrefabsSingleLabelMenu = "SingleLabelMenu";
     public const string PrefabBasicDialog = "BasicDialog";
     public const string PrefabsChooseSigninMenu = "ChooseSignInMenu";
     public const string PrefabsNewAccountMenu = "NewAccountMenu";
     public const string PrefabsSignInMenu = "SignInMenu";
     public const string PrefabsPasswordResetMenu = "PasswordResetMenu";
     public const string PrefabsManageAccountMenu = "ManageAccountMenu";
     // Key used to save names for uploading scores:
     public const string UploadScoreDefaultName = "My";
}
