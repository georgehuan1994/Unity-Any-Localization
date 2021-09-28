
// Any Localization - © 2020-2021 George Huan. All rights reserved
// https://gorh.cn/any-localization/

using System.IO;
using UnityEditor;
using UnityEngine;

public static class EditorScreenshotExtension
{
    //ctrl + shift + y 截图
    [MenuItem("Screenshot/Take Screenshot %#y")]
    private static void Screenshot()
    {
        string folderPath = Directory.GetCurrentDirectory() + "/Screenshots";
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        var timestamp = System.DateTime.Now;
        var stampString = string.Format("_{0}-{1:00}-{2:00}_{3:00}-{4:00}-{5:00}", timestamp.Year, timestamp.Month, timestamp.Day, timestamp.Hour, timestamp.Minute, timestamp.Second);
        ScreenCapture.CaptureScreenshot(Path.Combine(folderPath, stampString + ".png"));

        Debug.Log("截图" + stampString + ".png");
    }
}