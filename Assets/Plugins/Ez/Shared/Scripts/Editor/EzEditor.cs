// Copyright (c) 2016 Ez Entertainment SRL. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

namespace Ez
{
    public class EzEditor : Editor
    {
        public const float WIDTH_1 = 420f;
        public const float WIDTH_2 = 210f;
        public const float WIDTH_3 = 140f;
        public const float WIDTH_4 = 105f;
        public const float WIDTH_5 = 84f;
        public const float WIDTH_6 = 70f;
        public const float WIDTH_7 = 60f;
        public const float WIDTH_8 = 52.5f;

        public const float HEIGHT_1 = 22f;
        public const float HEADER_HEIGHT = 36f;

        public const float INDENT = 24f;

        public const int VERTICAL_SPACE_BETWEEN_ELEMENTS = 4;

        public Color DarkBaseColor = EzColors.GREEN;
        public Color LightBaseColor = EzColors.L_GREEN;

        public bool showHelp = false;
        public string saying;

        /// <summary>
        /// Sets the Repaint() update interval. 
        /// Never = it never does an auto Repaint();
        /// Update = calls the Repaint() method on every frame, making the framerate normal for the editor window;
        /// InspectorUpdate = calls the Repaint() method 10 times per second;
        /// </summary>
        protected enum RepaintOn { Never, Update, InspectorUpdate }
        protected RepaintOn repaintOn = RepaintOn.Never;

        public GUISkin skin { get { return EditorGUIUtility.isProSkin ? EzStyle.DarkSkin : EzStyle.LightSkin; } }
        private Color tempColor;
        private Color tempContentColor;
        private Color tempBackgroundColor;

        public void SaveCurrentColors()
        {
            tempColor = GUI.color;
            tempContentColor = GUI.contentColor;
            tempBackgroundColor = GUI.backgroundColor;
        }

        public void SaveCurrentColorsAndResetColors()
        {
            SaveCurrentColors();
            EzEditorUtility.ResetColors();
        }

        public void LoadPreviousColors()
        {
            GUI.color = tempColor;
            GUI.contentColor = tempContentColor;
            GUI.backgroundColor = tempBackgroundColor;
        }

        public void SetBaseColor()
        {
            if (EditorGUIUtility.isProSkin)
                EzEditorUtility.SetBackgroundColor(DarkBaseColor);
            else
                EzEditorUtility.SetBackgroundColor(LightBaseColor);
        }

        /// <summary>
        /// Marks the scene dirty, prompting a save
        /// </summary>
        public static void MarkSceneDirty()
        {
            if(EditorApplication.isPlaying) { return; }
            EditorSceneManager.MarkAllScenesDirty();
            //EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        }

        /// <summary>
        /// Sets the controlID that has keybard control to 0
        /// </summary>
        public static void ResetKeyboardFocus()
        {
            GUIUtility.keyboardControl = 0;
        }

        /// <summary>
        /// This is necessary to make the framerate normal for the editor window.
        /// </summary>
        public void Update()
        {
            if (repaintOn == RepaintOn.Update)
                Repaint();
        }

        /// <summary>
        /// This will only get called 10 times per second.
        /// </summary>
        public void OnInspectorUpdate()
        {
            if (repaintOn == RepaintOn.InspectorUpdate)
                Repaint();
        }

        /// <summary>
        /// Draws the component's header.
        /// </summary>
        /// <param name="t"></param>
        public void DrawHeader(Texture t)
        {
            SaveCurrentColorsAndResetColors();
            EzEditorUtility.DrawTexture(t, WIDTH_1, HEADER_HEIGHT);
            LoadPreviousColors();
        }

        /// <summary>
        /// Draws the toolbar with showHelp, Debug and the component mover buttons
        /// </summary>
        /// <param name="debugThis"></param>
        /// <param name="c"></param>
        public void DrawHeaderToolbar(Component c = null, SerializedProperty debugThis = null)
        {
            EditorGUILayout.BeginHorizontal(GUILayout.Width(WIDTH_1));
            {
                showHelp = EditorGUILayout.ToggleLeft("Show Help", showHelp, GUILayout.Width(WIDTH_5));

                if (debugThis != null)
                    debugThis.boolValue = EditorGUILayout.ToggleLeft("Debug", debugThis.boolValue, GUILayout.Width(WIDTH_5));

                GUILayout.FlexibleSpace();

                if (c != null)
                {
                    SaveCurrentColorsAndResetColors();
                    if (GUILayout.Button("UP", skin.GetStyle(EzStyle.StyleName.BtnGreen.ToString()), GUILayout.Width(WIDTH_6)))
                    {
                        UnityEditorInternal.ComponentUtility.MoveComponentUp(c);
                        GUIUtility.keyboardControl = 0;
                    }
                    if (GUILayout.Button("DOWN", skin.GetStyle(EzStyle.StyleName.BtnGreen.ToString()), GUILayout.Width(WIDTH_6)))
                    {
                        UnityEditorInternal.ComponentUtility.MoveComponentDown(c);
                        GUIUtility.keyboardControl = 0;
                    }
                    LoadPreviousColors();
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// Draws a custom Help Box.
        /// </summary>
        /// <param name="show"></param>
        /// <param name="message"></param>
        /// <param name="title"></param>
        /// <param name="width"></param>
        public void DrawHelpBox(bool show, string message, string title = "", float width = WIDTH_1)
        {
            if (!show)
                return;

            GUIStyle style = skin.GetStyle(EzStyle.StyleName.HelpBox.ToString());
            string helpText = string.Empty;

            if (!string.IsNullOrEmpty(title))
                helpText += "<b>" + title + "</b> - ";

            helpText += message;

            SaveCurrentColorsAndResetColors();
            EditorGUILayout.LabelField(helpText, style, GUILayout.Width(width));
            LoadPreviousColors();
        }

        /// <summary>
        /// Draws a custom Help Box afer an UnityEvent.
        /// </summary>
        /// <param name="show"></param>
        /// <param name="message"></param>
        /// <param name="title"></param>
        /// <param name="width"></param>
        public void DrawEventHelpBox(bool show, string message, float width = WIDTH_1)
        {
            if (!show)
                return;

            GUIStyle style = skin.GetStyle(EzStyle.StyleName.HelpBox.ToString());
            string helpText = string.Empty;

            helpText += message;

            SaveCurrentColorsAndResetColors();
            GUILayout.Space(-17);
            EditorGUILayout.BeginHorizontal(GUILayout.Width(width));
            {
                GUILayout.Space(4);
                EditorGUILayout.LabelField(helpText, style, GUILayout.Width(width - 60));
            }
            EditorGUILayout.EndHorizontal();
            LoadPreviousColors();
        }

        /// <summary>
        /// Removes the object/component by using DestroyImmediate.
        /// </summary>
        public void RemoveComponent(Object obj)
        {
            DestroyImmediate(obj);
            EditorGUIUtility.ExitGUI();
        }

        public void DrawEditorIsCompiling(string message = "")
        {
            EzEditorUtility.DrawTexture(EzResources.EditorIsCompiling, 420, 72);
            if (!string.IsNullOrEmpty(message))
            {
                GUILayout.Space(-40);
                EditorGUILayout.BeginHorizontal(GUILayout.Width(WIDTH_1));
                {
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.LabelField(message, skin.GetStyle(EzStyle.StyleName.CompilingMessage.ToString()), GUILayout.Width(400), GUILayout.Height(36));
                    GUILayout.FlexibleSpace();
                }
                EditorGUILayout.EndHorizontal();
            }
        }
    }
}
