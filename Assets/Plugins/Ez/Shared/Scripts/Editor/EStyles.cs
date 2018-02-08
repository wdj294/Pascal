// Copyright (c) 2016-2017 Ez Entertainment SRL. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Ez.Internal;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Ez
{
    public static class EStyles
    {
        public static GUIStyle GetStyle(TextStyle style) { return Skin.GetStyle(style.ToString()); }
        public static GUIStyle GetStyle(ButtonStyle style) { return Skin.GetStyle(style.ToString()); }

        private static GUISkin skin;
        private static GUISkin Skin { get { if(skin == null) { skin = GetSkin(); } return skin; } }

        private static GUISkin GetSkin()
        {
            GUISkin s = ScriptableObject.CreateInstance<GUISkin>();
            List<GUIStyle> styles = new List<GUIStyle>();
            styles.AddRange(GetTextStyles());
            styles.AddRange(GetButtonStyles());
            s.customStyles = styles.ToArray();
            return s;
        }

        public enum TextStyle
        {
            Help,
            Info,
            Warning,
            Error,
            WindowTitle,
            WindowSubtitle,
            WindowNormal,
            WindowSmall,
            ComponentTitle,
            ComponentSubtitle,
            ComponentNormal,
            ComponentSmall
        }

        private static List<GUIStyle> GetTextStyles()
        {
            List<GUIStyle> textStyles = new List<GUIStyle>();
            textStyles.Add(GetTextStyleWithBackground(TextStyle.Help, EResources.HelpBackground, EColor.GreyDarker3, EResources.HelpBackground, EColor.GreyDarker3, 12, FontStyle.Normal, TextAnchor.MiddleLeft, true, true, EResources.FontAwesome));
            textStyles.Add(GetTextStyleWithBackground(TextStyle.Info, EResources.InfoBackground, EColor.BlueDark, EResources.InfoBackground, EColor.BlueDark, 12, FontStyle.Normal, TextAnchor.MiddleLeft, true, true, EResources.FontAwesome));
            textStyles.Add(GetTextStyleWithBackground(TextStyle.Warning, EResources.WarningBackground, EColor.OrangeDark, EResources.WarningBackground, EColor.OrangeDark, 12, FontStyle.Normal, TextAnchor.MiddleLeft, true, true, EResources.FontAwesome));
            textStyles.Add(GetTextStyleWithBackground(TextStyle.Error, EResources.ErrorBackground, EColor.RedDark, EResources.ErrorBackground, EColor.RedDark, 12, FontStyle.Normal, TextAnchor.MiddleLeft, true, true, EResources.FontAwesome));
            textStyles.Add(GetTextStyle(TextStyle.ComponentTitle, EColor.UnityLight, EColor.UnityDark, 16, FontStyle.Bold, TextAnchor.MiddleLeft, new RectOffset(0, 0, 6, 0), true, true, EResources.FontLithosRegular));
            textStyles.Add(GetTextStyle(TextStyle.ComponentSubtitle, EColor.UnityLight, EColor.UnityDark, 16, FontStyle.Normal, TextAnchor.MiddleLeft, new RectOffset(0, 0, 6, 0), true, true, EResources.FontLithosRegular));
            textStyles.Add(GetTextStyle(TextStyle.ComponentNormal, EColor.UnityLight, EColor.UnityDark, 12, FontStyle.Normal, TextAnchor.MiddleLeft, new RectOffset(0, 0, 6, 0), true, true, EResources.FontLithosRegular));
            textStyles.Add(GetTextStyle(TextStyle.ComponentSmall, EColor.UnityLight, EColor.UnityDark, 8, FontStyle.Normal, TextAnchor.MiddleLeft, new RectOffset(0, 0, 6, 0), true, true, EResources.FontLithosRegular));
            return textStyles;
        }

        private static GUIStyle GetTextStyle(TextStyle style, Color textColorDark, Color textColorLight, int fontSize, FontStyle fontStyle, TextAnchor alignment, RectOffset padding, bool richText = true, bool wordWrap = true, Font font = null)
        {
            return new GUIStyle()
            {
                name = style.ToString(),
                normal = { textColor = EditorGUIUtility.isProSkin ? textColorDark : textColorLight },
                fontSize = fontSize,
                fontStyle = fontStyle,
                alignment = alignment,
                padding = padding,
                richText = richText,
                wordWrap = wordWrap,
                font = font
            };
        }

        private static GUIStyle GetTextStyleWithBackground(TextStyle style, Texture backgroundDark, Color textColorDark, Texture backgroundLight, Color textColorLight, int fontSize, FontStyle fontStyle, TextAnchor alignment, bool richText = true, bool wordWrap = true, Font font = null)
        {
            return new GUIStyle()
            {
                name = style.ToString(),
                normal =
                {
                    background = EditorGUIUtility.isProSkin ? (Texture2D) backgroundDark : (Texture2D) backgroundLight,
                    textColor = EditorGUIUtility.isProSkin ? textColorDark : textColorLight
                },
                fontSize = fontSize,
                fontStyle = fontStyle,
                alignment = alignment,
                richText = richText,
                wordWrap = wordWrap,
                padding = new RectOffset(8, 8, 8, 8),
                border = new RectOffset(2, 2, 2, 2),
                font = font
            };
        }

        public enum ButtonStyle
        {
            ButtonBlue,
            ButtonGreen,
            ButtonOrange,
            ButtonPurple,
            ButtonRed,
            ButtonGreyLight,
            ButtonGreyMild,
            ButtonGreyDark,
            LeftButtonBlue,
            LeftButtonGreen,
            LeftButtonOrange,
            LeftButtonPurple,
            LeftButtonRed,
            LeftButtonGreyLight,
            LeftButtonGreyMild,
            LeftButtonGreyDark,
            ToggleOn,
            ToggleOff,
            ButtonPlus,
            ButtonMinus,
            ButtonReset,
        }

        private static List<GUIStyle> GetButtonStyles()
        {
            List<GUIStyle> buttonStyles = new List<GUIStyle>();
            buttonStyles.Add(GetButtonStyle(ButtonStyle.ButtonBlue, EResources.ButtonBlue, EColor.Blue, EColor.Blue, EColor.BlueMild, 12, FontStyle.Bold, TextAnchor.MiddleCenter, new RectOffset(2, 2, 1, 2), new RectOffset(8, 8, 8, 8), new RectOffset(0, 0, 0, 0), EResources.FontAwesome));
            buttonStyles.Add(GetButtonStyle(ButtonStyle.ButtonGreen, EResources.ButtonGreen, EColor.Green, EColor.Green, EColor.GreenMild, 12, FontStyle.Bold, TextAnchor.MiddleCenter, new RectOffset(2, 2, 1, 2), new RectOffset(8, 8, 8, 8), new RectOffset(0, 0, 0, 0), EResources.FontAwesome));
            buttonStyles.Add(GetButtonStyle(ButtonStyle.ButtonPurple, EResources.ButtonPurple, EColor.Purple, EColor.Purple, EColor.PurpleMild, 12, FontStyle.Bold, TextAnchor.MiddleCenter, new RectOffset(2, 2, 1, 2), new RectOffset(8, 8, 8, 8), new RectOffset(0, 0, 0, 0), EResources.FontAwesome));
            buttonStyles.Add(GetButtonStyle(ButtonStyle.ButtonOrange, EResources.ButtonOrange, EColor.Orange, EColor.Orange, EColor.OrangeMild, 12, FontStyle.Bold, TextAnchor.MiddleCenter, new RectOffset(2, 2, 1, 2), new RectOffset(8, 8, 8, 8), new RectOffset(0, 0, 0, 0), EResources.FontAwesome));
            buttonStyles.Add(GetButtonStyle(ButtonStyle.ButtonRed, EResources.ButtonRed, EColor.Red, EColor.Red, EColor.RedMild, 12, FontStyle.Bold, TextAnchor.MiddleCenter, new RectOffset(2, 2, 1, 2), new RectOffset(8, 8, 8, 8), new RectOffset(0, 0, 0, 0), EResources.FontAwesome));
            buttonStyles.Add(GetButtonStyle(ButtonStyle.ButtonGreyLight, EResources.ButtonGreyLight, EColor.UnityDark, EColor.UnityDark, EColor.UnityMild, 12, FontStyle.Bold, TextAnchor.MiddleCenter, new RectOffset(2, 2, 1, 2), new RectOffset(8, 8, 8, 8), new RectOffset(0, 0, 0, 0), EResources.FontAwesome));
            buttonStyles.Add(GetButtonStyle(ButtonStyle.ButtonGreyMild, EResources.ButtonGreyMild, EColor.UnityLight, EColor.UnityLight, EColor.UnityMild, 12, FontStyle.Bold, TextAnchor.MiddleCenter, new RectOffset(2, 2, 1, 2), new RectOffset(8, 8, 8, 8), new RectOffset(0, 0, 0, 0), EResources.FontAwesome));
            buttonStyles.Add(GetButtonStyle(ButtonStyle.ButtonGreyDark, EResources.ButtonGreyDark, EColor.UnityLight, EColor.UnityLight, EColor.UnityMild, 12, FontStyle.Bold, TextAnchor.MiddleCenter, new RectOffset(2, 2, 1, 2), new RectOffset(8, 8, 8, 8), new RectOffset(0, 0, 0, 0), EResources.FontAwesome));

            buttonStyles.Add(GetButtonStyle(ButtonStyle.LeftButtonBlue, EResources.ButtonBlue, EColor.Blue, EColor.Blue, EColor.BlueMild, 12, FontStyle.Bold, TextAnchor.MiddleLeft, new RectOffset(2, 2, 1, 2), new RectOffset(8, 8, 8, 8), new RectOffset(0, 0, 0, 0), EResources.FontAwesome));
            buttonStyles.Add(GetButtonStyle(ButtonStyle.LeftButtonGreen, EResources.ButtonGreen, EColor.Green, EColor.Green, EColor.GreenMild, 12, FontStyle.Bold, TextAnchor.MiddleLeft, new RectOffset(2, 2, 1, 2), new RectOffset(8, 8, 8, 8), new RectOffset(0, 0, 0, 0), EResources.FontAwesome));
            buttonStyles.Add(GetButtonStyle(ButtonStyle.LeftButtonPurple, EResources.ButtonPurple, EColor.Purple, EColor.Purple, EColor.PurpleMild, 12, FontStyle.Bold, TextAnchor.MiddleLeft, new RectOffset(2, 2, 1, 2), new RectOffset(8, 8, 8, 8), new RectOffset(0, 0, 0, 0), EResources.FontAwesome));
            buttonStyles.Add(GetButtonStyle(ButtonStyle.LeftButtonOrange, EResources.ButtonOrange, EColor.Orange, EColor.Orange, EColor.OrangeMild, 12, FontStyle.Bold, TextAnchor.MiddleLeft, new RectOffset(2, 2, 1, 2), new RectOffset(8, 8, 8, 8), new RectOffset(0, 0, 0, 0), EResources.FontAwesome));
            buttonStyles.Add(GetButtonStyle(ButtonStyle.LeftButtonRed, EResources.ButtonRed, EColor.Red, EColor.Red, EColor.RedMild, 12, FontStyle.Bold, TextAnchor.MiddleLeft, new RectOffset(2, 2, 1, 2), new RectOffset(8, 8, 8, 8), new RectOffset(0, 0, 0, 0), EResources.FontAwesome));
            buttonStyles.Add(GetButtonStyle(ButtonStyle.LeftButtonGreyLight, EResources.ButtonGreyLight, EColor.UnityDark, EColor.UnityDark, EColor.UnityMild, 12, FontStyle.Bold, TextAnchor.MiddleLeft, new RectOffset(2, 2, 1, 2), new RectOffset(8, 8, 8, 8), new RectOffset(0, 0, 0, 0), EResources.FontAwesome));
            buttonStyles.Add(GetButtonStyle(ButtonStyle.LeftButtonGreyMild, EResources.ButtonGreyMild, EColor.UnityLight, EColor.UnityLight, EColor.UnityMild, 12, FontStyle.Bold, TextAnchor.MiddleLeft, new RectOffset(2, 2, 1, 2), new RectOffset(8, 8, 8, 8), new RectOffset(0, 0, 0, 0), EResources.FontAwesome));
            buttonStyles.Add(GetButtonStyle(ButtonStyle.LeftButtonGreyDark, EResources.ButtonGreyDark, EColor.UnityLight, EColor.UnityLight, EColor.UnityMild, 12, FontStyle.Bold, TextAnchor.MiddleLeft, new RectOffset(2, 2, 1, 2), new RectOffset(8, 8, 8, 8), new RectOffset(0, 0, 0, 0), EResources.FontAwesome));


            buttonStyles.Add(GetButtonStyle(ButtonStyle.ToggleOn, EResources.ButtonCheckboxEnabled, EColor.WhiteDark, EResources.ButtonCheckboxPressed, EColor.WhiteLight, 12, FontStyle.Bold, TextAnchor.MiddleCenter, EResources.FontAwesome, new RectOffset(2, 2, 1, 2), new RectOffset(2, 2, 2, 2), new RectOffset(0, 0, 0, 0)));
            buttonStyles.Add(GetButtonStyle(ButtonStyle.ToggleOff, EResources.ButtonCheckboxDisabled, EColor.WhiteDark, EResources.ButtonCheckboxPressed, EColor.WhiteLight, 12, FontStyle.Bold, TextAnchor.MiddleCenter, EResources.FontAwesome, new RectOffset(2, 2, 1, 2), new RectOffset(2, 2, 2, 2), new RectOffset(0, 0, 0, 0)));

            buttonStyles.Add(GetButtonStyle(ButtonStyle.ButtonPlus, EResources.ButtonGreen, EColor.Green, EColor.Green, EColor.GreenDark, 10, FontStyle.Bold, TextAnchor.MiddleCenter, new RectOffset(1, 1, 1, 1), new RectOffset(4, 4, 4, 4), new RectOffset(0, 0, 0, 0), EResources.FontAwesome));
            buttonStyles.Add(GetButtonStyle(ButtonStyle.ButtonMinus, EResources.ButtonRed, EColor.Red, EColor.Red, EColor.RedDark, 10, FontStyle.Bold, TextAnchor.MiddleCenter, new RectOffset(1, 1, 1, 1), new RectOffset(4, 4, 4, 4), new RectOffset(0, 0, 0, 0), EResources.FontAwesome));
            buttonStyles.Add(GetButtonStyle(ButtonStyle.ButtonReset, EResources.ButtonGreyDark, EColor.UnityLight, EColor.UnityLight, EColor.UnityLight, 10, FontStyle.Bold, TextAnchor.MiddleCenter, new RectOffset(1, 1, 1, 1), new RectOffset(4, 4, 4, 4), new RectOffset(0, 0, 0, 0), EResources.FontAwesome));

            return buttonStyles;
        }

        private static GUIStyle GetButtonStyle(
            ButtonStyle style,
            Texture normalBackground, Color normalTextColor,
            Texture activeBackground, Color activeTextColor,
            int fontSize, FontStyle fontStyle, TextAnchor alignment, Font font,
            RectOffset padding, RectOffset border, RectOffset margin)
        {
            return new GUIStyle()
            {
                name = style.ToString(),
                normal = { background = (Texture2D)normalBackground, textColor = normalTextColor },
                onNormal = { background = (Texture2D)normalBackground, textColor = normalTextColor },
                active = { background = (Texture2D)activeBackground, textColor = activeTextColor },
                onActive = { background = (Texture2D)activeBackground, textColor = activeTextColor },
                fontSize = fontSize,
                fontStyle = fontStyle,
                alignment = alignment,
                font = font,
                padding = padding,
                border = border,
                margin = margin
            };
        }

        private static GUIStyle GetButtonStyle(ButtonStyle s, GTexture t, Color cNormal, Color cHover, Color cActive, int fontSize, FontStyle fontStyle, TextAnchor alignment, RectOffset padding, RectOffset border, RectOffset margin, Font font)
        {
            GUIStyle style = new GUIStyle();
            style.name = s.ToString();
            style.normal = new GUIStyleState { background = t.normal2D, textColor = cNormal };
            style.onNormal = new GUIStyleState { background = t.normal2D, textColor = cNormal };
            if(t.HasHover)
            {
                style.hover = new GUIStyleState { background = t.hover2D, textColor = cHover };
                style.onHover = new GUIStyleState { background = t.hover2D, textColor = cHover };
            }
            if(t.HasActive)
            {
                style.active = new GUIStyleState { background = t.active2D, textColor = cActive };
                style.onActive = new GUIStyleState { background = t.active2D, textColor = cActive };
            }
            style.fontSize = fontSize;
            style.alignment = alignment;
            style.padding = padding;
            style.border = border;
            style.margin = margin;
            style.font = font;
            return style;
        }

        public static GUIStyle Copy(this GUIStyle style)
        {
            GUIStyle copy = new GUIStyle
            {
                name = style.name,
                normal = style.normal,
                onNormal = style.onNormal,
                hover = style.hover,
                onHover = style.onHover,
                active = style.active,
                onActive = style.onActive,
                focused = style.focused,
                onFocused = style.onFocused,
                alignment = style.alignment,
                border = style.border,
                clipping = style.clipping,
                contentOffset = style.contentOffset,
                fixedHeight = style.fixedHeight,
                fixedWidth = style.fixedWidth,
                font = style.font,
                fontSize = style.fontSize,
                fontStyle = style.fontStyle,
                imagePosition = style.imagePosition,
                margin = style.margin,
                overflow = style.overflow,
                padding = style.padding,
                richText = style.richText,
                stretchHeight = style.stretchHeight,
                stretchWidth = style.stretchWidth,
                wordWrap = style.wordWrap
            };
            return copy;
        }
    }
}
