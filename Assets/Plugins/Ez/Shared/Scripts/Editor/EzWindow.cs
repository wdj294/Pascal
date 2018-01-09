// Copyright (c) 2016 Ez Entertainment SRL. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using UnityEngine;
using UnityEditor;

namespace Ez
{
    public class EzWindow : EditorWindow
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

        public const int VERTICAL_SPACE_BETWEEN_ELEMENTS = 4;

        public Color DarkBaseColor = EzColors.GREEN;
        public Color LightBaseColor = EzColors.L_GREEN;

        public bool showHelp = false;

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

        public void SetDefaultBackgroundColor()
        {
            if (EditorGUIUtility.isProSkin)
                EzEditorUtility.SetBackgroundColor(DarkBaseColor);
            else
                EzEditorUtility.SetBackgroundColor(LightBaseColor);
        }

        public void SetCustomBackgroundColor(Color darkColor, Color lightColor)
        {
            if (EditorGUIUtility.isProSkin)
                EzEditorUtility.SetBackgroundColor(darkColor);
            else
                EzEditorUtility.SetBackgroundColor(lightColor);
        }

        public void SetCustomTextColor(Color darkColor, Color lightColor)
        {
            if (EditorGUIUtility.isProSkin)
                EzEditorUtility.SetTextColor(darkColor);
            else
                EzEditorUtility.SetTextColor(lightColor);
        }

        public void SetCustomGlobalTintColor(Color darkColor, Color lightColor)
        {
            if (EditorGUIUtility.isProSkin)
                EzEditorUtility.SetGlobalTintColor(darkColor);
            else
                EzEditorUtility.SetGlobalTintColor(lightColor);
        }

        public void ResetColors()
        {
            EzEditorUtility.ResetColors();
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
    }
}
