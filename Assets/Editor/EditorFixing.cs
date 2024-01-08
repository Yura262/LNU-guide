using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEditor.Build;

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

