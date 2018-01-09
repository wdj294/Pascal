// Copyright (c) 2016-2017 Ez Entertainment SRL. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Ez
{
    public partial class EResources
    {
        private static Font fontLithosBlack;
        public static Font FontLithosBlack { get { if (fontLithosBlack == null) { fontLithosBlack = AssetDatabase.LoadAssetAtPath<Font>(EFileHelper.GetRelativeDirectoryPath("Ez") + "/Shared/Fonts/LithosProBlack.otf"); } return fontLithosBlack; } }
        private static Font fontLithosRegular;
        public static Font FontLithosRegular { get { if (fontLithosRegular == null) { fontLithosRegular = AssetDatabase.LoadAssetAtPath<Font>(EFileHelper.GetRelativeDirectoryPath("Ez") + "/Shared/Fonts/LithosProRegular.otf"); } return fontLithosRegular; } }
        private static Font fontAwesome;
        public static Font FontAwesome { get { if (fontAwesome == null) { fontAwesome = Resources.Load("FontAwesome") as Font; } return fontAwesome; } }

        public static Texture2D GenerateTexture4x4(Color border, Color content)
        {
            return GenerateTexture4x4(border, border, content, content);
        }

        public static Texture2D GenerateTexture4x4(Color borderDark, Color borderLight, Color contentDark, Color contentLight)
        {
            Color border = EditorGUIUtility.isProSkin ? borderDark : borderLight;
            Color content = EditorGUIUtility.isProSkin ? contentDark : contentLight;
            Texture2D texture = new Texture2D(4, 4, TextureFormat.ARGB32, false);
            for (int x = 0; x < 4; x++) { for (int y = 0; y < 4; y++) { texture.SetPixel(x, y, (x == 0 || x == 3 || y == 0 || y == 3) ? border : content); } }
            texture.Apply();
            return texture;
        }

        private static Texture2D helpBackground;
        public static Texture2D HelpBackground { get { if (helpBackground == null) { helpBackground = GenerateTexture4x4(EColor.UnityMild, EColor.UnityMild, EColor.WhiteDark, EColor.WhiteLight); } return helpBackground; } }
        private static Texture2D infoBackground;
        public static Texture2D InfoBackground { get { if (infoBackground == null) { infoBackground = GenerateTexture4x4(EColor.BlueMild, EColor.BlueMild, EColor.WhiteDark, EColor.WhiteLight); } return infoBackground; } }
        private static Texture2D warningBackground;
        public static Texture2D WarningBackground { get { if (warningBackground == null) { warningBackground = GenerateTexture4x4(EColor.OrangeMild, EColor.OrangeMild, EColor.WhiteDark, EColor.WhiteLight); } return warningBackground; } }
        private static Texture2D errorBackground;
        public static Texture2D ErrorBackground { get { if (errorBackground == null) { errorBackground = GenerateTexture4x4(EColor.RedMild, EColor.RedMild, EColor.WhiteDark, EColor.WhiteLight); } return errorBackground; } }

        private static Texture2D whiteBackground;
        public static Texture2D WhiteBackground { get { if (whiteBackground == null) { whiteBackground = GenerateTexture4x4(EColor.UnityMild, EColor.UnityMild, EColor.WhiteDark, EColor.WhiteLight); } return whiteBackground; } }
        private static Texture blueBackground;
        public static Texture BlueBackground { get { if (blueBackground == null) { blueBackground = GenerateTexture4x4(EColor.BlueMild, EColor.BlueMild, EColor.BackgroundBlue, EColor.BackgroundBlue); } return blueBackground; } }
        private static Texture greenBackground;
        public static Texture GreendPackground { get { if (greenBackground == null) { greenBackground = GenerateTexture4x4(EColor.GreenMild, EColor.GreenMild, EColor.BackgroundGreen, EColor.BackgroundGreen); } return greenBackground; } }
        private static Texture orangeBackground;
        public static Texture OrangeBackground { get { if (orangeBackground == null) { orangeBackground = GenerateTexture4x4(EColor.OrangeMild, EColor.OrangeMild, EColor.BackgroundOrange, EColor.BackgroundOrange); } return orangeBackground; } }
        private static Texture purpleBackground;
        public static Texture PurpleBackground { get { if (purpleBackground == null) { purpleBackground = GenerateTexture4x4(EColor.PurpleMild, EColor.PurpleMild, EColor.BackgroundPurple, EColor.BackgroundPurple); } return purpleBackground; } }
        private static Texture redBackground;
        public static Texture RedBackground { get { if (redBackground == null) { redBackground = GenerateTexture4x4(EColor.RedMild, EColor.RedMild, EColor.BackgroundRed, EColor.BackgroundRed); } return redBackground; } }

        public static Texture GetTexture(string fileName, string path) { return AssetDatabase.LoadAssetAtPath<Texture>(path + fileName + ".png"); }

        private static string sharedImagesPath;
        public static string SharedImagesPath { get { if (string.IsNullOrEmpty(sharedImagesPath)) { sharedImagesPath = EFileHelper.GetRelativeDirectoryPath("Ez") + "/Shared/Images/"; } return sharedImagesPath; } }

        private static Texture barDisabled;
        public static Texture BarDisabled { get { if (barDisabled == null) barDisabled = GetTexture("barDisabled", SharedImagesPath); return barDisabled; } }
        private static Texture barEnabled;
        public static Texture BarEnabled { get { if (barEnabled == null) barEnabled = GetTexture("barEnabled", SharedImagesPath); return barEnabled; } }
        private static Texture buttonCheckboxDisabled;
        public static Texture ButtonCheckboxDisabled { get { if (buttonCheckboxDisabled == null) buttonCheckboxDisabled = GetTexture("buttonCheckboxDisabled", SharedImagesPath); return buttonCheckboxDisabled; } }
        private static Texture buttonCheckboxEnabled;
        public static Texture ButtonCheckboxEnabled { get { if (buttonCheckboxEnabled == null) buttonCheckboxEnabled = GetTexture("buttonCheckboxEnabled", SharedImagesPath); return buttonCheckboxEnabled; } }
        private static Texture buttonCheckboxPressed;
        public static Texture ButtonCheckboxPressed { get { if (buttonCheckboxPressed == null) buttonCheckboxPressed = GetTexture("buttonCheckboxPressed", SharedImagesPath); return buttonCheckboxPressed; } }
        private static Texture buttonBlue;
        public static Texture ButtonBlue { get { if (buttonBlue == null) buttonBlue = GetTexture("buttonBlue", SharedImagesPath); return buttonBlue; } }
        private static Texture buttonBlueActive;
        public static Texture ButtonBlueActive { get { if (buttonBlueActive == null) buttonBlueActive = GetTexture("buttonBlueActive", SharedImagesPath); return buttonBlueActive; } }
        private static Texture buttonGreen;
        public static Texture ButtonGreen { get { if (buttonGreen == null) buttonGreen = GetTexture("buttonGreen", SharedImagesPath); return buttonGreen; } }
        private static Texture buttonGreenActive;
        public static Texture ButtonGreenActive { get { if (buttonGreenActive == null) buttonGreenActive = GetTexture("buttonGreenActive", SharedImagesPath); return buttonGreenActive; } }
        private static Texture buttonGreyLight;
        public static Texture ButtonGreyLight { get { if (buttonGreyLight == null) buttonGreyLight = GetTexture("buttonGreyLight", SharedImagesPath); return buttonGreyLight; } }
        private static Texture buttonGreyLightActive;
        public static Texture ButtonGreyLightActive { get { if (buttonGreyLightActive == null) buttonGreyLightActive = GetTexture("buttonGreyLightActive", SharedImagesPath); return buttonGreyLightActive; } }
        private static Texture buttonGreyMild;
        public static Texture ButtonGreyMild { get { if (buttonGreyMild == null) buttonGreyMild = GetTexture("buttonGreyMild", SharedImagesPath); return buttonGreyMild; } }
        private static Texture buttonGreyMildActive;
        public static Texture ButtonGreyMildActive { get { if (buttonGreyMildActive == null) buttonGreyMildActive = GetTexture("buttonGreyMildActive", SharedImagesPath); return buttonGreyMildActive; } }
        private static Texture buttonGreyDark;
        public static Texture ButtonGreyDark { get { if (buttonGreyDark == null) buttonGreyDark = GetTexture("buttonGreyDark", SharedImagesPath); return buttonGreyDark; } }
        private static Texture buttonGreyDarkActive;
        public static Texture ButtonGreyDarkActive { get { if (buttonGreyDarkActive == null) buttonGreyDarkActive = GetTexture("buttonGreyDarkActive", SharedImagesPath); return buttonGreyDarkActive; } }
        private static Texture buttonHelpDisabled;
        public static Texture ButtonHelpDisabled { get { if (buttonHelpDisabled == null) buttonHelpDisabled = GetTexture("buttonHelpDisabled", SharedImagesPath); return buttonHelpDisabled; } }
        private static Texture buttonHelpEnabled;
        public static Texture ButtonHelpEnabled { get { if (buttonHelpEnabled == null) buttonHelpEnabled = GetTexture("buttonHelpEnabled", SharedImagesPath); return buttonHelpEnabled; } }
        private static Texture buttonHelpPressed;
        public static Texture ButtonHelpPressed { get { if (buttonHelpPressed == null) buttonHelpPressed = GetTexture("buttonHelpPressed", SharedImagesPath); return buttonHelpPressed; } }
        private static Texture buttonOrange;
        public static Texture ButtonOrange { get { if (buttonOrange == null) buttonOrange = GetTexture("buttonOrange", SharedImagesPath); return buttonOrange; } }
        private static Texture buttonOrangeActive;
        public static Texture ButtonOrangeActive { get { if (buttonOrangeActive == null) buttonOrangeActive = GetTexture("buttonOrangeActive", SharedImagesPath); return buttonOrangeActive; } }
        private static Texture buttonPurple;
        public static Texture ButtonPurple { get { if (buttonPurple == null) buttonPurple = GetTexture("buttonPurple", SharedImagesPath); return buttonPurple; } }
        private static Texture buttonPurpleActive;
        public static Texture ButtonPurpleActive { get { if (buttonPurpleActive == null) buttonPurpleActive = GetTexture("buttonPurpleActive", SharedImagesPath); return buttonPurpleActive; } }
        private static Texture buttonRed;
        public static Texture ButtonRed { get { if (buttonRed == null) buttonRed = GetTexture("buttonRed", SharedImagesPath); return buttonRed; } }
        private static Texture buttonRedActive;
        public static Texture ButtonRedActive { get { if (buttonRedActive == null) buttonRedActive = GetTexture("buttonRedActive", SharedImagesPath); return buttonRedActive; } }
        private static Texture separatorBlack;
        public static Texture SeparatorBlack { get { if (separatorBlack == null) separatorBlack = GetTexture("separatorBlack", SharedImagesPath); return separatorBlack; } }
        private static Texture separatorBlue;
        public static Texture SeparatorBlue { get { if (separatorBlue == null) separatorBlue = GetTexture("separatorBlue", SharedImagesPath); return separatorBlue; } }
        private static Texture separatorGreen;
        public static Texture SeparatorGreen { get { if (separatorGreen == null) separatorGreen = GetTexture("separatorGreen", SharedImagesPath); return separatorGreen; } }
        private static Texture separatorOrange;
        public static Texture SeparatorOrange { get { if (separatorOrange == null) separatorOrange = GetTexture("separatorOrange", SharedImagesPath); return separatorOrange; } }
        private static Texture separatorPurple;
        public static Texture SeparatorPurple { get { if (separatorPurple == null) separatorPurple = GetTexture("separatorPurple", SharedImagesPath); return separatorPurple; } }
        private static Texture separatorRed;
        public static Texture SeparatorRed { get { if (separatorRed == null) separatorRed = GetTexture("separatorRed", SharedImagesPath); return separatorRed; } }
    }
}
