using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEditor.Build;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

static class EditorMenus
{
    // taken from: http://answers.unity3d.com/questions/282959/set-inspector-lock-by-code.html
    [MenuItem("Tools/Toggle Inspector Lock %l")] // Ctrl + L to lock inspector window
    static void ToggleInspectorLock()
    {
        ActiveEditorTracker.sharedTracker.isLocked = !ActiveEditorTracker.sharedTracker.isLocked;
        ActiveEditorTracker.sharedTracker.ForceRebuild();
    }



    [MenuItem("Edit/Run _F5")] // shortcut key F5 to Play (and exit playmode also)
    static void PlayGame()
    {
        if (!Application.isPlaying)
        {
            EditorSceneManager.SaveScene(SceneManager.GetActiveScene(), "", false);
        }
        EditorApplication.ExecuteMenuItem("Edit/Play");
    }
}

public class EditorFixing : IPreprocessBuildWithReport
{
    public int callbackOrder { get { return 0; } }
    public void OnPreprocessBuild(BuildReport report)
    {

        int incrementUpAt = 9; //if this is set to 9, then 1.0.9 will become 1.1.0

        string versionText = PlayerSettings.bundleVersion;
        if (string.IsNullOrEmpty(versionText))
        {
            versionText = "0.0.1";
        }
        else
        {
            versionText = versionText.Trim(); //clean up whitespace if necessary
            string[] lines = versionText.Split('.');

            int majorVersion = 0;
            int minorVersion = 0;
            int subMinorVersion = 0;

            if (lines.Length > 0) majorVersion = int.Parse(lines[0]);
            if (lines.Length > 1) minorVersion = int.Parse(lines[1]);
            if (lines.Length > 2) subMinorVersion = int.Parse(lines[2]);

            subMinorVersion++;
            if (subMinorVersion > incrementUpAt)
            {
                minorVersion++;
                subMinorVersion = 0;
            }
            if (minorVersion > incrementUpAt)
            {
                majorVersion++;
                minorVersion = 0;
            }

            versionText = majorVersion.ToString("0") + "." + minorVersion.ToString("0") + "." + subMinorVersion.ToString("0");

        }
        Debug.Log("Version Incremented to " + versionText);
        PlayerSettings.bundleVersion = versionText;
    }
}

