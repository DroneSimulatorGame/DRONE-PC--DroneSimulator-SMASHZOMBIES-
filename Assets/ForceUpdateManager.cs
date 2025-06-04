//using UnityEngine;
//using System.Collections;
//using Google.Play.AppUpdate;
//using Google.Play.Common;

//public class ForceUpdateManager : MonoBehaviour
//{
//    private AppUpdateManager appUpdateManager;

//    void Start()
//    {
//        appUpdateManager = new AppUpdateManager();
//        StartCoroutine(CheckForUpdate());
//    }

//    private IEnumerator CheckForUpdate()
//    {
//        // Step 1: Get update info
//        PlayAsyncOperation<AppUpdateInfo, AppUpdateErrorCode> appUpdateInfoOperation = appUpdateManager.GetAppUpdateInfo();
//        yield return appUpdateInfoOperation;

//        if (!appUpdateInfoOperation.IsSuccessful)
//        {
//            Debug.LogError($"Failed to retrieve update info: {appUpdateInfoOperation.Error}");
//            yield break;
//        }

//        // Step 2: Analyze update info
//        AppUpdateInfo appUpdateInfo = appUpdateInfoOperation.GetResult();

//        // Define the immediate update options
//        AppUpdateOptions immediateOptions = AppUpdateOptions.ImmediateAppUpdateOptions();

//        // Check if an immediate update is available and allowed
//        if (appUpdateInfo.UpdateAvailability == UpdateAvailability.UpdateAvailable &&
//            appUpdateInfo.IsUpdateTypeAllowed(immediateOptions))
//        {
//            Debug.Log("Immediate update is available and allowed.");

//            // Step 3: Request the immediate update
//            AppUpdateRequest updateRequest = appUpdateManager.StartUpdate(appUpdateInfo, immediateOptions);

//            // Step 4: Wait for the update to complete
//            yield return new WaitUntil(() => updateRequest.IsDone);

//            if (updateRequest.Error != AppUpdateErrorCode.NoError)
//            {
//                Debug.LogError($"Update failed: {updateRequest.Error}");
//                ShowUpdateRequiredMessage();
//            }
//            else
//            {
//                Debug.Log("Update started successfully. App will restart when complete.");
//            }
//        }
//        else
//        {
//            Debug.Log("No immediate update required or allowed.");
//        }
//    }

//    private void ShowUpdateRequiredMessage()
//    {
//        Debug.LogWarning("Please update the app to the latest version from the Play Store.");
//        // Add your custom UI here if desired
//    }
//}