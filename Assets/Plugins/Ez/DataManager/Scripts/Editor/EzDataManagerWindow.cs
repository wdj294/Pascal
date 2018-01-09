// Copyright (c) 2016 Ez Entertainment SRL. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using UnityEditor;
using UnityEngine;

namespace Ez.DataManager
{
    public class EzDataManagerWindow : EzWindow
    {
        private static bool _utility = true;
        private static string _title = "Ez Data";
        private static bool _focus = true;

        private static float _minWidth = 512;
        private static float _minHeight = 512;

        EzDataManager GetEzDataManager { get { return FindObjectOfType<EzDataManager>(); } }
        bool SceneHasEzDataManager { get { return GetEzDataManager != null; } }

        [MenuItem("Tools/Ez/Data Manager", false, 100)]
        static void Init()
        {
            GetWindow<EzDataManagerWindow>(_utility, _title, _focus);
        }

        void OnEnable()
        {
            repaintOn = RepaintOn.Update;
            showHelp = false;
            SetupWindow();
        }

        void SetupWindow()
        {
            titleContent = new GUIContent(_title);
            minSize = new Vector2(_minWidth, _minHeight);
            maxSize = minSize;
        }

        void OnGUI()
        {
            EzEditorUtility.DrawTexture(EzResources.GeneralBackground, _minWidth, _minHeight);
            GUILayout.Space(-_minHeight);
            GUILayout.Space(32);
            EditorGUILayout.BeginHorizontal(GUILayout.Height(128));
            {
                GUILayout.FlexibleSpace();
                EzEditorUtility.DrawTexture(EzResources.IconDataManager, 128, 128);
                GUILayout.FlexibleSpace();
            }
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(16);
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();
                EditorGUILayout.BeginVertical(GUILayout.Width(256));
                {
                    EditorGUILayout.LabelField("Ez Data Manager", skin.GetStyle(EzStyle.StyleName.WindowTitle.ToString()), GUILayout.Width(WIDTH_1));
                    GUILayout.Space(54);
                    GUILayout.Space(48);
                    EditorGUILayout.BeginHorizontal(GUILayout.Height(40));
                    {
                        GUILayout.FlexibleSpace();
                        if (!SceneHasEzDataManager)
                        {
                            if (GUILayout.Button("Add to Scene", skin.GetStyle(EzStyle.StyleName.BtnGreen.ToString()), GUILayout.Width(160), GUILayout.Height(40)))
                            {
                                EzEditorUtility.CreateGameObjectFromPrefab(FileHelper.GetRelativeFolderPath("Ez") + "/DataManager/Prefabs/", "EzDataManager", "EzDataManager");
                                Repaint();
                            }
                        }
                        else
                        {
                            if (GUILayout.Button("Remove from Scene", skin.GetStyle(EzStyle.StyleName.BtnRed.ToString()), GUILayout.Width(160), GUILayout.Height(40)))
                            {
                                Undo.DestroyObjectImmediate(GetEzDataManager.gameObject);
                            }
                        }
                        GUILayout.FlexibleSpace();
                    }
                    EditorGUILayout.EndHorizontal();
                    GUILayout.Space(34);

                }
                EditorGUILayout.EndVertical();
                GUILayout.FlexibleSpace();
            }
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(48);
            EditorGUILayout.BeginHorizontal(GUILayout.Height(64));
            {
                GUILayout.FlexibleSpace();
                EzEditorUtility.DrawTexture(EzResources.IconEz, 64, 64);
                GUILayout.FlexibleSpace();
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}
