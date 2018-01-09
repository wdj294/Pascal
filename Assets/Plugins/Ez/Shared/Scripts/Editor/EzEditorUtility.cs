// Copyright (c) 2016 Ez Entertainment SRL. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using UnityEditor;
using UnityEngine;

namespace Ez
{
    public class EzEditorUtility
    {
        public static Texture GetTexture(string fileName, string folderPath)
        {
            return AssetDatabase.LoadAssetAtPath<Texture>(folderPath + fileName + ".png");
        }

        #region SetGlobalTintColor, SetTextColor, SetBackgroundColor, ResetColors
        public static void SetGlobalTintColor(Color darkSkinColor, Color lightSkinColor) { if (EditorGUIUtility.isProSkin) GUI.color = darkSkinColor; else GUI.color = lightSkinColor; }
        public static void SetGlobalTintColor(Color color) { GUI.color = color; }

        public static void SetTextColor(Color darkSkinColor, Color lightSkinColor) { if (EditorGUIUtility.isProSkin) GUI.contentColor = darkSkinColor; else GUI.contentColor = lightSkinColor; }
        public static void SetTextColor(Color color) { GUI.contentColor = color; }

        public static void SetBackgroundColor(Color darkSkinColor, Color lightSkinColor) { if (EditorGUIUtility.isProSkin) GUI.backgroundColor = darkSkinColor; else GUI.backgroundColor = lightSkinColor; }
        public static void SetBackgroundColor(Color color) { GUI.backgroundColor = color; }

        public static void ResetColors()
        {
            GUI.color = Color.white;
            GUI.contentColor = Color.white;
            GUI.backgroundColor = Color.white;
        }
        #endregion

        public static void VerticalSpace(int pixels)
        {
            EditorGUILayout.BeginVertical();
            {
                GUILayout.Space(pixels);
            }
            EditorGUILayout.EndVertical();
        }

        public static void HorizontalSpace(int pixels)
        {
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Space(pixels);
            }
            EditorGUILayout.EndHorizontal();
        }

        public static bool EditorIsCompiling(EditorWindow editorWindow, Texture texture, float width, float height)
        {
            if (EditorApplication.isCompiling)
            {
                DrawTexture(texture, width, height);
                editorWindow.Repaint();
            }
            return EditorApplication.isCompiling;
        }

        public static bool EditorIsInPlayMode(EditorWindow editorWindow, Texture texture, float width, float height)
        {
            if (Application.isPlaying)
            {
                DrawTexture(texture, width, height);
                editorWindow.Repaint();
            }
            return Application.isPlaying;
        }

        public static void DrawTexture(Texture tex)
        {
            if (tex == null)
            {
                Debug.Log("[EzEditorUtility] Texture is null");
                return;
            }

            //tex.hideFlags = HideFlags.DontSave;
            var rect = GUILayoutUtility.GetRect(0f, 0f);
            rect.width = tex.width;
            rect.height = tex.height;
            GUILayout.Space(rect.height);
            GUI.DrawTexture(rect, tex);
        }

        public static void DrawTexture(Texture tex, float width, float height)
        {
            if (tex == null)
            {
                Debug.Log("[EzEditorUtility] Texture is null");
                return;
            }

            var rect = GUILayoutUtility.GetRect(0f, 0f);
            rect.width = width;
            rect.height = height;
            GUILayout.Space(rect.height);
            GUI.DrawTexture(rect, tex);
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

        /// <summary>
        /// Creates a GameObject found at the specified path with the given name.
        /// </summary>
        /// <param name="path">The folder path where the prefab is located.</param>
        /// <param name="prefabName">The prefab file name. Without the '.prefab' extension.</param>
        /// <param name="goName">The newly created GamObject name.</param>
        public static void CreateGameObjectFromPrefab(string path, string prefabName, string goName)
        {
            var prefab = AssetDatabase.LoadAssetAtPath(path + prefabName + ".prefab", typeof(GameObject));
            if (prefab == null)
            {
                Debug.LogError("Could not find the " + prefabName + " prefab. It should be at " + path);
                return;
            }
            var go = UnityEngine.Object.Instantiate(prefab) as GameObject;
            go.name = goName;
            Undo.RegisterCreatedObjectUndo(go, "Added " + go.name);
            Selection.activeObject = go;
        }
    }
}
