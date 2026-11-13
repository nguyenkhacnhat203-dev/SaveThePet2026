
//using System;
//using System.Linq;
//using System.Reflection;
//using UnityEditor;
//using UnityEngine;

//public static class FirebaseSdkChecker
//{
//    private class SdkInfo
//    {
//        public string name;
//        public string typeName;

//        public SdkInfo(string name, string typeName)
//        {
//            this.name = name;
//            this.typeName = typeName;
//        }
//    }

//    private static readonly SdkInfo[] firebaseSdks =
//    {
//        new SdkInfo("Firebase Core", "Firebase.FirebaseApp"),
//        new SdkInfo("Firebase Authentication", "Firebase.Auth.FirebaseAuth"),
//        new SdkInfo("Firebase Realtime Database", "Firebase.Database.FirebaseDatabase"),
//        new SdkInfo("Firebase Firestore", "Firebase.Firestore.FirebaseFirestore"),
//        new SdkInfo("Firebase Analytics", "Firebase.Analytics.FirebaseAnalytics"),
//        new SdkInfo("Firebase Crashlytics", "Firebase.Crashlytics.FirebaseCrashlytics"),
//        new SdkInfo("Firebase Functions", "Firebase.Functions.FirebaseFunctions"),
//        new SdkInfo("Firebase Storage", "Firebase.Storage.FirebaseStorage"),
//        new SdkInfo("Firebase Messaging", "Firebase.Messaging.FirebaseMessaging"),
//        new SdkInfo("Firebase Remote Config", "Firebase.RemoteConfig.FirebaseRemoteConfig"),
//        new SdkInfo("Firebase App Check", "Firebase.AppCheck.FirebaseAppCheck"),
//        new SdkInfo("Firebase Installations", "Firebase.Installations.FirebaseInstallations")
//    };

//    [MenuItem("Tools/Firebase/Check Installed SDKs")]
//    public static void CheckFirebaseSDKs()
//    {
//        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

//        int installedCount = 0;

//        Debug.Log("========== FIREBASE SDK CHECK ==========");

//        foreach (SdkInfo sdk in firebaseSdks)
//        {
//            bool installed = assemblies.Any(
//                assembly => assembly.GetType(sdk.typeName, false) != null
//            );

//            if (installed)
//            {
//                installedCount++;
//                Debug.Log($"<color=green>[INSTALLED]</color> {sdk.name}");
//            }
//            else
//            {
//                Debug.LogWarning($"[NOT FOUND] {sdk.name}");
//            }
//        }

//        Debug.Log("========================================");
//        Debug.Log($"Firebase SDK installed: {installedCount}/{firebaseSdks.Length}");
//        Debug.Log("========================================");

//        EditorUtility.DisplayDialog(
//            "Firebase SDK Checker",
//            $"Đã kiểm tra Firebase SDK.\n\nTìm thấy: {installedCount}/{firebaseSdks.Length}\n\nMở Console để xem chi tiết.",
//            "OK"
//        );
//    }
//}
