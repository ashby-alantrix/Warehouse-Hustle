#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using System;
using System.IO; 
using Davanci.Utils;

public class ScreenshotCapture : SingletonMB<ScreenshotCapture>
{
    [SerializeField] string fileName = "File Name";

    public void MultipleScreenshot()
    {
       // StartCoroutine(ScreenShotRoutine());
    }

    public static Vector2 GetMainGameViewSize()
    {
        System.Type T = System.Type.GetType("UnityEditor.GameView,UnityEditor");
        System.Reflection.MethodInfo GetSizeOfMainGameView = T.GetMethod("GetSizeOfMainGameView", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        System.Object Res = GetSizeOfMainGameView.Invoke(null, null);
        return (Vector2)Res;
    }

    public void SingleScreenshot()
    {
        var t = GetMainGameViewSize();
        int width = (int)t.x;
        int height = (int)t.y;
        string resolutionText = width + "x" + height;

        // Create a folder for the resolution in the project's root directory
        string projectPath = Directory.GetParent(Application.dataPath).FullName;
        string folderPath = Path.Combine(projectPath, resolutionText);
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        // Capture screenshot with the resolution in the file name and save it in the created folder
        string fullFileName = Path.Combine(folderPath, fileName + " - " + resolutionText + ".png");
        ScreenCapture.CaptureScreenshot(fullFileName);
        Debug.Log("Screen Shot Taken: " + fullFileName);
    }
}
#endif
