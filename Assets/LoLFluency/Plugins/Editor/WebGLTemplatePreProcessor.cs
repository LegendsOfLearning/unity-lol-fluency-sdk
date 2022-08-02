using UnityEngine;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor;
using System.IO;

#if UNITY_WEBGL
//modified from https://forum.unity.com/threads/webgltemplates-folder-in-packages.678823/
namespace LoL.Fluency.Editor
{
    /// <summary>
    /// Pre processor to set the right web gl template.
    /// </summary>
    public class WebGLTemplatePreProcessor : IPreprocessBuildWithReport
    {
        public int callbackOrder => 0;
 
        public void OnPreprocessBuild(BuildReport report)
        {
            AssignWebGLTemplate();
        }

        //[MenuItem("Test/AssignWebGLTemplate")]
        public static void AssignWebGLTemplate()
        {
            string oldTemplate = PlayerSettings.WebGL.template;
            string newTemplate = "PROJECT:Fluency";

            if (oldTemplate == newTemplate)
                return;

            Debug.Log($"[FluencySDK] Setting webgl template, old={oldTemplate}, new={newTemplate}");
            PlayerSettings.WebGL.template = newTemplate;
        }
    }
}
#endif