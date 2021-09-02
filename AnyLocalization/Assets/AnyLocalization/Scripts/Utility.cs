//------------------------------------------------------------
// Any Localization
// Author: George Huan
// Date: 2020-11-12
// Homepage: 
// Feedback: 
//------------------------------------------------------------

using UnityEngine;

public static partial class Utility
{
    public static bool IsEditor()
    {
        if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
        {
            return true;
        }
        return false;
    }

    public static bool IsPC()
    {
        if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.OSXPlayer)
        {
            return true;
        }
        return false;
    }

    public static bool IsIOS()
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            return true;
        }
        return false;
    }

    public static bool IsAndroid()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            return true;
        }
        return false;
    }

}
