// Copyright (c) 2016-2017 Ez Entertainment SRL. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Ez
{
    public class EBaseWindow : EditorWindow
    {
        #region Colors
        private Color tempColor = Color.white;
        private Color tempContentColor = Color.white;
        private Color tempBackgroundColor = Color.white;

        public void SaveColors(bool resetColors = false)
        {
            tempColor = GUI.color;
            tempContentColor = GUI.contentColor;
            tempBackgroundColor = GUI.backgroundColor;
            if (resetColors) { EGUI.ResetColors(); }
        }

        public void RestoreColors()
        {
            GUI.color = tempColor;
            GUI.contentColor = tempContentColor;
            GUI.backgroundColor = tempBackgroundColor;
        }
        #endregion

        #region Dimensions
        public const float WIDTH_512 = 512f;
        public const float WIDTH_256 = 256f;
        public const float WIDTH_128 = 128f;
        public const float WIDTH_64 = 64f;


        public const float HEIGHT_8 = 8f;
        /// <summary>
        /// This is the EditorGUIUtility.singleLineHeight value
        /// </summary> 
        public const float HEIGHT_16 = 16f;
        public const float HEIGHT_24 = 24f;
        public const float HEIGHT_36 = 36f;

        public const float INDENT_24 = 24f;

        /// <summary>
        /// This is the EditorGUIUtility.standardVerticalSpacing value
        /// </summary>
        public const float SPACE_2 = 2;
        public const float SPACE_4 = 4;
        public const float SPACE_8 = 8;
        /// <summary>
        /// This is the EditorGUIUtility.singleLineHeight value
        /// </summary> 
        public const float SPACE_16 = 16;
        #endregion

        public Dictionary<string, EGUI.InfoMessage> help, info, warning, error;
    }
}
