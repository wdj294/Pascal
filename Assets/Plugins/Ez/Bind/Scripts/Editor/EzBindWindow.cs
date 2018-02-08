// Copyright (c) 2016 - 2017 Ez Entertainment SRL. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using UnityEngine;
using System.Collections;
using UnityEditor;
using Ez.Internal;
using Ez.Binding.Internal;

namespace Ez.Binding
{
    public class EzBindWindow : EzWindow
    {
        private static bool _utility = true;
        private static string _title = "EzBind - Professional Data Binding Solution";
        private static bool _focus = true;

        private static float _minWidth = 512;
        private static float _minHeight = 512;

        EzBind GetEzBind { get { return FindObjectOfType<EzBind>(); } }
        bool SceneHasEzBind { get { return GetEzBind != null; } }

        EzBindExtension GetEzBindExtension { get { return FindObjectOfType<EzBindExtension>(); } }
        bool SceneHasEzBindExtension { get { return GetEzBindExtension != null; } }

        [MenuItem("Tools/Ez/Bind", false, 110)]
        static void Init()
        {
            GetWindow<EzBindWindow>(_utility, _title, _focus);
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
            EditorGUILayout.BeginHorizontal(GUILayout.Width(512), GUILayout.Height(128));
            {
                GUILayout.FlexibleSpace();
                EzEditorUtility.DrawTexture(EResources.IconEzBind.normal, 128, 128);
                GUILayout.FlexibleSpace();
            }
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(8);
            //EditorGUILayout.BeginHorizontal(GUILayout.Width(512), GUILayout.Height(40));
            //{
            //    GUILayout.Space(192);
            //    EzEditorUtility.DrawTexture(EResources.IconEzBindTitle.normal, 128, 40);
            //    GUILayout.FlexibleSpace();
            //}
            //EditorGUILayout.EndHorizontal();
            GUILayout.Space(32 + 40);
            EditorGUILayout.BeginHorizontal(GUILayout.Width(512), GUILayout.Height(148));
            {
                GUILayout.FlexibleSpace();
                EditorGUILayout.BeginVertical(GUILayout.Width(256), GUILayout.Height(60));
                {
                    EzEditorUtility.DrawTexture(EResources.BarEzBindMain.normal, 240, 36);
                    EditorGUILayout.BeginHorizontal(GUILayout.Height(24));
                    {
                        GUILayout.FlexibleSpace();
                        if(!SceneHasEzBind)
                        {
                            if(GUILayout.Button("Add to Scene", skin.GetStyle(EzStyle.StyleName.BtnGreen.ToString()), GUILayout.Width(220), GUILayout.Height(24)))
                            {
                                EzEditorUtility.CreateGameObjectFromPrefab(FileHelper.GetRelativeFolderPath("Ez") + "/Bind/Prefabs/", "EzBind", "EzBind");
                                Repaint();
                            }
                        }
                        else
                        {
                            if(GUILayout.Button("Remove from Scene", skin.GetStyle(EzStyle.StyleName.BtnRed.ToString()), GUILayout.Width(220), GUILayout.Height(24)))
                            {
                                Undo.DestroyObjectImmediate(GetEzBind.gameObject);
                            }
                        }
                        GUILayout.FlexibleSpace();
                    }
                    EditorGUILayout.EndHorizontal();
                    GUILayout.Space(16);
                    EzEditorUtility.DrawTexture(EResources.BarEzBindExtension.normal, 240, 36);
                    EditorGUILayout.BeginHorizontal(GUILayout.Height(24));
                    {
                        GUILayout.FlexibleSpace();
                        if(!SceneHasEzBindExtension)
                        {
                            if(GUILayout.Button("Add to Scene", skin.GetStyle(EzStyle.StyleName.BtnBlue.ToString()), GUILayout.Width(220), GUILayout.Height(24)))
                            {
                                EzEditorUtility.CreateGameObjectFromPrefab(FileHelper.GetRelativeFolderPath("Ez") + "/Bind/Prefabs/", "EzBindExtension", "EzBindExtension");
                                Repaint();
                            }
                        }
                        else
                        {
                            if(GUILayout.Button("Remove from Scene", skin.GetStyle(EzStyle.StyleName.BtnRed.ToString()), GUILayout.Width(220), GUILayout.Height(24)))
                            {
                                Undo.DestroyObjectImmediate(GetEzBindExtension.gameObject);
                            }
                        }
                        GUILayout.FlexibleSpace();
                    }
                    EditorGUILayout.EndHorizontal();

                }
                EditorGUILayout.EndVertical();
                GUILayout.FlexibleSpace();
            }
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(36);
            EditorGUILayout.BeginHorizontal(GUILayout.Width(512), GUILayout.Height(64));
            {
                GUILayout.FlexibleSpace();
                EzEditorUtility.DrawTexture(EzResources.IconEz, 64, 64);
                GUILayout.FlexibleSpace();
            }
            EditorGUILayout.EndHorizontal();
        }

    }
}
