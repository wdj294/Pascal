#region Namespace Imports
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.SceneManagement;
#endregion

namespace UI.ThreeDimensional
{    
    public class UIObject3DProcessor: UnityEditor.AssetModificationProcessor
    {
        public static string[] OnWillSaveAssets(string[] paths)
        {
            var objects = GameObject.FindObjectsOfType<UIObject3D>().ToList();

            foreach (var o in objects)
            {
                o.Cleanup();
            }

            var container = GameObject.Find("UIObject3D Scenes");
            if (!Application.isPlaying) GameObject.DestroyImmediate(container);
            
            return paths;
        }
    }
}
