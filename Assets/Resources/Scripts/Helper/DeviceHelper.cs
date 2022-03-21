using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeviceHelper
{
    public static string GetDeviceId()
    {
        if (IsAndroid())
        {
            var clsUnity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            var objActivity = clsUnity.GetStatic<AndroidJavaObject>("currentActivity");
            var objResolver = objActivity.Call<AndroidJavaObject>("getContentResolver");
            var clsSecure = new AndroidJavaClass("android.provider.Settings$Secure");
            return clsSecure.CallStatic<string>("getString", objResolver, "android_id");
        }
        else
        {
            return SystemInfo.deviceUniqueIdentifier;
            // return  UnityEngine.iOS.Device.vendorIdentifier;
        }
    }

    public static string GetDeviceType()
    {
        #if UNITY_ANDROID && !UNITY_EDITOR
            return "ANDROID";
        #elif UNITY_IPHONE
            return "IOS";
        #else
            return "EDITOR";
        #endif
    }

    public static void Vibrate(long ms = 250, bool overridePermission = false)
    {
        if (!overridePermission && DeviceCache.GetOptionVibration() == 0)
            return;

        if (IsAndroid())
        {
            var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            var currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            var vibrator = currentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");
            vibrator.Call("vibrate", ms);
        }
        else
            Handheld.Vibrate();

    }

    private static bool IsAndroid()
    {
        #if UNITY_ANDROID && !UNITY_EDITOR
            return true;
        #else
            return false;
        #endif

    }
}
