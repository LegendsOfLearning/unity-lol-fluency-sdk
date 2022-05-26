#if UNITY_EDITOR_OSX
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
 
public class PreBuildProcessing : IPreprocessBuildWithReport
{
    public int callbackOrder => 1;
    public void OnPreprocessBuild(BuildReport report)
    {
        System.Environment.SetEnvironmentVariable("EMSDK_PYTHON", "/Users/jeffluciana/.pyenv/versions/2.7.18/bin/python2");
    }
}
#endif