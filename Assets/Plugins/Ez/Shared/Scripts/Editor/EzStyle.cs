// Copyright (c) 2016 Ez Entertainment SRL. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using UnityEngine;

namespace Ez
{
    public static class EzStyle
    {
        public enum StyleName
        {
            HelpBox,
            BtnGreen,
            BtnBlue,
            BtnPurple,
            BtnRed,
            BtnOrange,
            BtnGreyLight,
            BtnGreyMild,
            BtnGreyDark,
            BtnBarGreen,
            BtnBarGrey,
            BtnBarGreenIndented,
            BtnBarGreyIndented,
            LabelMidLeft,
            LabelMidCenter,
            LabelMidRight,
            CompilingMessage,
            WindowTitle
        }

        private static GUISkin darkSkin = null;
        public static GUISkin DarkSkin { get { if (darkSkin == null) darkSkin = GetDarkSkin(); return darkSkin; } }
        private static GUISkin lightSkin = null;
        public static GUISkin LightSkin { get { if (lightSkin == null) lightSkin = GetLightSkin(); return lightSkin; } }

        private static GUISkin GetDarkSkin()
        {
            GUISkin skin = ScriptableObject.CreateInstance<GUISkin>();
            List<GUIStyle> styles = new List<GUIStyle>();

            styles.Add(GetStyle_HelpBox(StyleName.HelpBox, EzResources.HelpBoxDark, EzColors.UNITY_LIGHT, 9, FontStyle.Normal, true, true));
            styles.Add(GetStyle_Label(StyleName.LabelMidLeft, EzColors.UNITY_LIGHT, TextAnchor.MiddleLeft));
            styles.Add(GetStyle_Label(StyleName.LabelMidCenter, EzColors.UNITY_LIGHT, TextAnchor.MiddleCenter));
            styles.Add(GetStyle_Label(StyleName.LabelMidRight, EzColors.UNITY_LIGHT, TextAnchor.MiddleRight));

            styles.AddRange(GetCommonSkinStyles());
            skin.customStyles = styles.ToArray();
            return skin;
        }

        private static GUISkin GetLightSkin()
        {
            GUISkin skin = ScriptableObject.CreateInstance<GUISkin>();
            List<GUIStyle> styles = new List<GUIStyle>();

            styles.Add(GetStyle_HelpBox(StyleName.HelpBox, EzResources.HelpBoxLight, EzColors.UNITY_DARK, 9, FontStyle.Normal, true, true));
            styles.Add(GetStyle_Label(StyleName.LabelMidLeft, EzColors.UNITY_DARK, TextAnchor.MiddleLeft));
            styles.Add(GetStyle_Label(StyleName.LabelMidCenter, EzColors.UNITY_DARK, TextAnchor.MiddleCenter));
            styles.Add(GetStyle_Label(StyleName.LabelMidRight, EzColors.UNITY_DARK, TextAnchor.MiddleRight));

            styles.AddRange(GetCommonSkinStyles());
            skin.customStyles = styles.ToArray();
            return skin;
        }

        /// <summary>
        /// Get all the syles that are the same for both skins
        /// </summary>
        private static GUIStyle[] GetCommonSkinStyles()
        {
            List<GUIStyle> styles = new List<GUIStyle>();

            styles.Add(GetStyle_Button(StyleName.BtnGreen, EzResources.BtnGreenNormal, EzColors.L_GREEN, EzResources.BtnGreenActive, EzColors.GREEN, 10, FontStyle.Bold, TextAnchor.MiddleCenter));
            styles.Add(GetStyle_Button(StyleName.BtnBlue, EzResources.BtnBlueNormal, EzColors.L_BLUE, EzResources.BtnBlueActive, EzColors.BLUE, 10, FontStyle.Bold, TextAnchor.MiddleCenter));
            styles.Add(GetStyle_Button(StyleName.BtnPurple, EzResources.BtnPurpleNormal, EzColors.L_PURPLE, EzResources.BtnPurpleActive, EzColors.PURPLE, 10, FontStyle.Bold, TextAnchor.MiddleCenter));
            styles.Add(GetStyle_Button(StyleName.BtnRed, EzResources.BtnRedNormal, EzColors.L_RED, EzResources.BtnRedActive, EzColors.RED, 10, FontStyle.Bold, TextAnchor.MiddleCenter));
            styles.Add(GetStyle_Button(StyleName.BtnOrange, EzResources.BtnOrangeNormal, EzColors.L_ORANGE, EzResources.BtnOrangeActive, EzColors.ORANGE, 10, FontStyle.Bold, TextAnchor.MiddleCenter));
            styles.Add(GetStyle_Button(StyleName.BtnGreyLight, EzResources.BtnGreyLightNormal, EzColors.UNITY_DARK, EzResources.BtnGreyLightActive, EzColors.UNITY_MILD, 10, FontStyle.Bold, TextAnchor.MiddleCenter));
            styles.Add(GetStyle_Button(StyleName.BtnGreyMild, EzResources.BtnGreyMildNormal, EzColors.UNITY_LIGHT, EzResources.BtnGreyMildActive, EzColors.UNITY_MILD, 10, FontStyle.Bold, TextAnchor.MiddleCenter));
            styles.Add(GetStyle_Button(StyleName.BtnGreyDark, EzResources.BtnGreyDarkNormal, EzColors.UNITY_LIGHT, EzResources.BtnGreyDarkActive, EzColors.UNITY_MILD, 10, FontStyle.Bold, TextAnchor.MiddleCenter));

            styles.Add(GetStyle_ButtonBar(StyleName.BtnBarGreen, EzResources.BtnBarGreenNormalOpen, EzColors.L_GREEN, EzResources.BtnBarGreenActive, EzColors.GREEN, 10, FontStyle.Bold, TextAnchor.MiddleLeft));
            styles.Add(GetStyle_ButtonBar(StyleName.BtnBarGrey, EzResources.BtnBarGreyNormalClosed, EzColors.UNITY_LIGHT, EzResources.BtnBarGreyActive, EzColors.UNITY_MILD, 10, FontStyle.Bold, TextAnchor.MiddleLeft));

            styles.Add(GetStyle_ButtonBarIndented(StyleName.BtnBarGreenIndented, EzResources.BtnBarGreenNormalOpen, EzColors.L_GREEN, EzResources.BtnBarGreenActive, EzColors.GREEN, 10, FontStyle.Normal, TextAnchor.MiddleLeft));
            styles.Add(GetStyle_ButtonBarIndented(StyleName.BtnBarGreyIndented, EzResources.BtnBarGreyNormalClosed, EzColors.UNITY_LIGHT, EzResources.BtnBarGreyActive, EzColors.UNITY_MILD, 10, FontStyle.Normal, TextAnchor.MiddleLeft));

            styles.Add(GetStyle_Label(StyleName.CompilingMessage, EzColors.GREEN, TextAnchor.MiddleCenter));

            styles.Add(GetStyle_Title(StyleName.WindowTitle, EzColors.GREEN, 20, TextAnchor.MiddleCenter));

            return styles.ToArray();
        }

        private static GUIStyle GetStyle_HelpBox(StyleName styleName, Texture2D nBackground, Color nTextColor, int fontSize = 9, FontStyle fontStyle = FontStyle.Normal, bool richText = true, bool wordWrap = true)
        {
            return new GUIStyle
            {
                name = styleName.ToString(),
                normal =
                {
                    background = nBackground,
                    textColor = nTextColor
                },
                fontSize = fontSize,
                fontStyle = fontStyle,
                richText = richText,
                wordWrap = wordWrap,
                padding = new RectOffset(8, 8, 8, 8),
                border = new RectOffset(2, 2, 2, 2)
            };
        }
        private static GUIStyle GetStyle_Button(StyleName styleName, Texture nBackground, Color nTextColor, Texture aBackground, Color aTextColor, int fontSize = 10, FontStyle fontStyle = FontStyle.Bold, TextAnchor alignment = TextAnchor.MiddleCenter)
        {
            return new GUIStyle
            {
                name = styleName.ToString(),
                normal =
                {
                    background = (Texture2D) nBackground,
                    textColor = nTextColor
                },
                active =
                {
                    background = (Texture2D) aBackground,
                    textColor = aTextColor
                },
                fontSize = fontSize,
                fontStyle = fontStyle,
                alignment = alignment,
                padding = new RectOffset(2, 2, 1, 3),
                border = new RectOffset(4, 4, 4, 4),
                margin = new RectOffset(1, 1, 1, 1)
            };
        }
        private static GUIStyle GetStyle_ButtonBar(StyleName styleName, Texture nBackground, Color nTextColor, Texture aBackground, Color aTextColor, int fontSize = 10, FontStyle fontStyle = FontStyle.Bold, TextAnchor alignment = TextAnchor.MiddleLeft)
        {
            return new GUIStyle
            {
                name = styleName.ToString(),
                normal =
                {
                    background = (Texture2D) nBackground,
                    textColor = nTextColor
                },
                active =
                {
                    background = (Texture2D) aBackground,
                    textColor = aTextColor
                },
                fontSize = fontSize,
                fontStyle = fontStyle,
                alignment = alignment,
                //fixedWidth = 420,
                fixedHeight = 16,
                padding = new RectOffset(24, 2, 1, 3),
                border = new RectOffset(1, 1, 1, 1),
                margin = new RectOffset(1, 1, 1, 1)
            };
        }
        private static GUIStyle GetStyle_ButtonBarIndented(StyleName styleName, Texture nBackground, Color nTextColor, Texture aBackground, Color aTextColor, int fontSize = 10, FontStyle fontStyle = FontStyle.Normal, TextAnchor alignment = TextAnchor.MiddleLeft)
        {
            return new GUIStyle
            {
                name = styleName.ToString(),
                normal =
                {
                    background = (Texture2D) nBackground,
                    textColor = nTextColor
                },
                active =
                {
                    background = (Texture2D) aBackground,
                    textColor = aTextColor
                },
                fontSize = fontSize,
                fontStyle = fontStyle,
                alignment = alignment,
                fixedWidth = 420 - 24,
                fixedHeight = 16,
                padding = new RectOffset(24, 2, 1, 2),
                margin = new RectOffset(1, 1, 1, 1)
            };
        }
        private static GUIStyle GetStyle_Label(StyleName styleName, Color nTextColor, TextAnchor alignment)
        {
            return new GUIStyle
            {
                name = styleName.ToString(),
                normal = { textColor = nTextColor },
                alignment = alignment,
                wordWrap = true
            };
        }
        private static GUIStyle GetStyle_Title(StyleName styleName, Color nTextColor, int fontSize = 20, TextAnchor alignment = TextAnchor.MiddleCenter)
        {
            return new GUIStyle
            {
                name = styleName.ToString(),
                normal = { textColor = nTextColor },
                alignment = alignment,
                fontSize = fontSize
            };
        }
    }
}
