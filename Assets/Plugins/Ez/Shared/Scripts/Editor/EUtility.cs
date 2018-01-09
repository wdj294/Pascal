// Copyright (c) 2016-2017 Ez Entertainment SRL. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using UnityEditor;
using UnityEngine;

namespace Ez
{
    public static class EUtility
    {
        /// <summary>
        /// Plays an audioClip in the editor. This is useful for previewing a sound in the inspector.
        /// </summary>
        /// <param name="clip">The AudioClip you want to play</param>
        public static void PlayAudioClip(AudioClip clip)
        {
            if (clip == null) { return; }
            Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
            Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
            MethodInfo method = audioUtilClass.GetMethod
                (
                    "PlayClip",
                    BindingFlags.Static | BindingFlags.Public,
                    null,
                    new System.Type[] { typeof(AudioClip) },
                    null
                );
            method.Invoke(null, new object[] { clip });
        }

        /// <summary>
        /// Stops all the audio clips that are currently playing in the editor
        /// </summary>
        public static void StopAllClips()
        {
            Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
            Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
            MethodInfo method = audioUtilClass.GetMethod
                (
                    "StopAllClips",
                    BindingFlags.Static | BindingFlags.Public,
                    null,
                    new System.Type[] { },
                    null
                );
            method.Invoke(null, new object[] { });
        }

        /// <summary>
        /// Created an unlinked copy of a prefab in the current scene, with the specified gameObjectName.
        /// </summary>
        /// <param name="prefabPath">Path to the prefab.</param>
        /// <param name="prefabName">Prefab name (without the '.prefab' extension).</param>
        /// <param name="gameObjectName">The name of the new gameObject.</param>
        public static void CreateGameObjectFromPrefab(string prefabPath, string prefabName, string gameObjectName)
        {
            var prefab = AssetDatabase.LoadAssetAtPath(prefabPath + prefabName + ".prefab", typeof(GameObject));
            if (prefab == null) { Debug.LogError("[Ez] Could not find the " + prefabName + " prefab. It should be at " + prefabPath); return; }
            var go = UnityEngine.Object.Instantiate(prefab) as GameObject;
            go.name = gameObjectName;
            Undo.RegisterCreatedObjectUndo(go, "Created the '" + go.name + "' gameObject from the '" + prefabName + "' prefab");
            Selection.activeObject = go;
        }

        public static void AddScriptingDefineSymbol(string symbol)
        {
            string currentDefinedSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(GetActiveBuildTargetGroup());
            if (currentDefinedSymbols.Contains(symbol) == false)
            {
                currentDefinedSymbols += ";" + symbol;
                PlayerSettings.SetScriptingDefineSymbolsForGroup(GetActiveBuildTargetGroup(), currentDefinedSymbols);
            }
        }

        public static void AddScriptingDefineSymbol(string symbol, BuildTargetGroup buildTargetGroup)
        {
            string currentDefinedSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
            if (currentDefinedSymbols.Contains(symbol) == false)
            {
                currentDefinedSymbols += ";" + symbol;
                PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, currentDefinedSymbols);
            }
        }

        public static void RemoveScriptingDefineSymbol(string symbol)
        {
            string currentDefinedSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            if (currentDefinedSymbols.Contains(symbol) == true)
            {
                currentDefinedSymbols = currentDefinedSymbols.Replace(symbol, "");
                PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, currentDefinedSymbols);
            }
        }

        public static bool DoesSymbolExist(string symbol)
        {
            string currentDefinedSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(GetActiveBuildTargetGroup());
            return currentDefinedSymbols.Contains(symbol);
        }

        public static bool DoesSymbolExist(string symbol, BuildTargetGroup buildTargetGroup)
        {
            string currentDefinedSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
            return currentDefinedSymbols.Contains(symbol);
        }

        public static BuildTargetGroup GetActiveBuildTargetGroup()
        {
            switch (EditorUserBuildSettings.activeBuildTarget)
            {
                case BuildTarget.StandaloneOSXUniversal: return BuildTargetGroup.Standalone;
                case BuildTarget.StandaloneOSXIntel: return BuildTargetGroup.Standalone;
                case BuildTarget.StandaloneWindows: return BuildTargetGroup.Standalone;
                //case BuildTarget.WebPlayer: return BuildTargetGroup.WebPlayer;
                //case BuildTarget.WebPlayerStreamed: return BuildTargetGroup.WebPlayer;
                case BuildTarget.iOS: return BuildTargetGroup.iOS;
                //case BuildTarget.PS3: return BuildTargetGroup.PS3;
                //case BuildTarget.XBOX360: return BuildTargetGroup.XBOX360;
                case BuildTarget.Android: return BuildTargetGroup.Android;
                case BuildTarget.StandaloneLinux: return BuildTargetGroup.Standalone;
                case BuildTarget.StandaloneWindows64: return BuildTargetGroup.Standalone;
                case BuildTarget.WebGL: return BuildTargetGroup.WebGL;
                case BuildTarget.WSAPlayer: return BuildTargetGroup.WSA;
                case BuildTarget.StandaloneLinux64: return BuildTargetGroup.Standalone;
                case BuildTarget.StandaloneLinuxUniversal: return BuildTargetGroup.Standalone;
                //case BuildTarget.WP8Player: return BuildTargetGroup.WP8;
                case BuildTarget.StandaloneOSXIntel64: return BuildTargetGroup.Standalone;
                //case BuildTarget.BlackBerry: return BuildTargetGroup.BlackBerry;
                case BuildTarget.Tizen: return BuildTargetGroup.Tizen;
                case BuildTarget.PSP2: return BuildTargetGroup.PSP2;
                case BuildTarget.PS4: return BuildTargetGroup.PS4;
                case BuildTarget.PSM: return BuildTargetGroup.PSM;
                case BuildTarget.XboxOne: return BuildTargetGroup.XboxOne;
                case BuildTarget.SamsungTV: return BuildTargetGroup.SamsungTV;
                //case BuildTarget.Nintendo3DS: return BuildTargetGroup.Nintendo3DS;
                case BuildTarget.WiiU: return BuildTargetGroup.WiiU;
                case BuildTarget.tvOS: return BuildTargetGroup.tvOS;
                //case BuildTarget.iPhone: return BuildTargetGroup.iPhone;
                //case BuildTarget.BB10: return BuildTargetGroup.BlackBerry;
                //case BuildTarget.MetroPlayer: return BuildTargetGroup.Metro;
                default: return BuildTargetGroup.Unknown;
            }
        }
    }
}
