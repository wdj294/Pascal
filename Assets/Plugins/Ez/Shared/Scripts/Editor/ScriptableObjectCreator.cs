// Copyright (c) 2016 Ez Entertainment SRL. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Ez
{
    public static class ScriptableObjectCreator
    {
        [MenuItem("Assets/Create/Instance")]
        public static void CreateInstance()
        {
            foreach (Object o in Selection.objects)
            {
                if (o is MonoScript)
                {
                    MonoScript script = (MonoScript)o;
                    System.Type type = script.GetClass();
                    if (type.IsSubclassOf(typeof(ScriptableObject)))
                    {
                        CreateAsset(type);
                    }
                }
            }
        }

        [MenuItem("Assets/Create/Instance", true)]
        public static bool ValidateCreateInstance()
        {
            foreach (Object o in Selection.objects)
            {
                if (o is MonoScript)
                {
                    MonoScript script = (MonoScript)o;
                    System.Type type = script.GetClass();
                    if (type.IsSubclassOf(typeof(ScriptableObject)))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private static void CreateAsset(System.Type type)
        {
            var asset = ScriptableObject.CreateInstance(type);
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (path == "")
            {
                path = "Assets";
            }
            else if (Path.GetExtension(path) != "")
            {
                path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
            }
            string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/New " + type.ToString() + ".asset");
            AssetDatabase.CreateAsset(asset, assetPathAndName);
            AssetDatabase.SaveAssets();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
        }
    }
}
#endif

