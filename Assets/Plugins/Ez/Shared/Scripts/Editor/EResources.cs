// Copyright (c) 2016-2017 Ez Entertainment SRL. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Ez.Internal;
using System;
using UnityEditor;
using UnityEngine;

namespace Ez
{
    public partial class EResources
    {
        private static Font fontLithosBlack;
        public static Font FontLithosBlack { get { if(fontLithosBlack == null) { fontLithosBlack = AssetDatabase.LoadAssetAtPath<Font>(EFileHelper.GetRelativeDirectoryPath("Ez") + "/Shared/Fonts/LithosProBlack.otf"); } return fontLithosBlack; } }
        private static Font fontLithosRegular;
        public static Font FontLithosRegular { get { if(fontLithosRegular == null) { fontLithosRegular = AssetDatabase.LoadAssetAtPath<Font>(EFileHelper.GetRelativeDirectoryPath("Ez") + "/Shared/Fonts/LithosProRegular.otf"); } return fontLithosRegular; } }
        private static Font fontAwesome;
        public static Font FontAwesome { get { if(fontAwesome == null) { fontAwesome = Resources.Load("FontAwesome") as Font; } return fontAwesome; } }

        public static Texture2D GenerateTexture4x4(Color border, Color content)
        {
            return GenerateTexture4x4(border, border, content, content);
        }

        public static Texture2D GenerateTexture4x4(Color borderDark, Color borderLight, Color contentDark, Color contentLight, float alpha = 1)
        {
            Color border = EditorGUIUtility.isProSkin ? new Color(borderDark.r, borderDark.g, borderDark.b, alpha) : new Color(borderLight.r, borderLight.g, borderLight.b, alpha);
            Color content = EditorGUIUtility.isProSkin ? new Color(contentDark.r, contentDark.g, contentDark.b, alpha) : new Color(contentLight.r, contentLight.g, contentLight.b, alpha);
            Texture2D texture = new Texture2D(4, 4, TextureFormat.ARGB32, false);
            for(int x = 0; x < 4; x++) { for(int y = 0; y < 4; y++) { texture.SetPixel(x, y, (x == 0 || x == 3 || y == 0 || y == 3) ? border : content); } }
            texture.Apply();
            return texture;
        }

        private static Texture2D helpBackground;
        public static Texture2D HelpBackground { get { if(helpBackground == null) { helpBackground = GenerateTexture4x4(EColor.UnityMild, EColor.UnityMild, EColor.WhiteDark, EColor.WhiteLight); } return helpBackground; } }
        private static Texture2D infoBackground;
        public static Texture2D InfoBackground { get { if(infoBackground == null) { infoBackground = GenerateTexture4x4(EColor.BlueMild, EColor.BlueMild, EColor.WhiteDark, EColor.WhiteLight); } return infoBackground; } }
        private static Texture2D warningBackground;
        public static Texture2D WarningBackground { get { if(warningBackground == null) { warningBackground = GenerateTexture4x4(EColor.OrangeMild, EColor.OrangeMild, EColor.WhiteDark, EColor.WhiteLight); } return warningBackground; } }
        private static Texture2D errorBackground;
        public static Texture2D ErrorBackground { get { if(errorBackground == null) { errorBackground = GenerateTexture4x4(EColor.RedMild, EColor.RedMild, EColor.WhiteDark, EColor.WhiteLight); } return errorBackground; } }

        private static Texture2D whiteBackground;
        public static Texture2D WhiteBackground { get { if(whiteBackground == null) { whiteBackground = GenerateTexture4x4(EColor.UnityMild, EColor.UnityMild, EColor.WhiteDark, EColor.WhiteLight, 0.2f); } return whiteBackground; } }
        private static Texture blueBackground;
        public static Texture BlueBackground { get { if(blueBackground == null) { blueBackground = GenerateTexture4x4(EColor.BlueMild, EColor.BlueMild, EColor.BackgroundBlue, EColor.BackgroundBlue, 0.2f); } return blueBackground; } }
        private static Texture greenBackground;
        public static Texture GreenBackground { get { if(greenBackground == null) { greenBackground = GenerateTexture4x4(EColor.GreenMild, EColor.GreenMild, EColor.BackgroundGreen, EColor.BackgroundGreen, 0.2f); } return greenBackground; } }
        private static Texture orangeBackground;
        public static Texture OrangeBackground { get { if(orangeBackground == null) { orangeBackground = GenerateTexture4x4(EColor.OrangeMild, EColor.OrangeMild, EColor.BackgroundOrange, EColor.BackgroundOrange, 0.2f); } return orangeBackground; } }
        private static Texture purpleBackground;
        public static Texture PurpleBackground { get { if(purpleBackground == null) { purpleBackground = GenerateTexture4x4(EColor.PurpleMild, EColor.PurpleMild, EColor.BackgroundPurple, EColor.BackgroundPurple, 0.2f); } return purpleBackground; } }
        private static Texture redBackground;
        public static Texture RedBackground { get { if(redBackground == null) { redBackground = GenerateTexture4x4(EColor.RedMild, EColor.RedMild, EColor.BackgroundRed, EColor.BackgroundRed, 0.2f); } return redBackground; } }

        public static Texture GetTexture(string fileName, string path) { return AssetDatabase.LoadAssetAtPath<Texture>(path + fileName + ".png"); }

        private static string sharedImagesPath;
        public static string SharedImagesPath { get { if(string.IsNullOrEmpty(sharedImagesPath)) { sharedImagesPath = EFileHelper.GetRelativeDirectoryPath("Ez") + "/Shared/Images/"; } return sharedImagesPath; } }

        private static Texture barDisabled;
        public static Texture BarDisabled { get { if(barDisabled == null) barDisabled = GetTexture("barDisabled", SharedImagesPath); return barDisabled; } }
        private static Texture barEnabled;
        public static Texture BarEnabled { get { if(barEnabled == null) barEnabled = GetTexture("barEnabled", SharedImagesPath); return barEnabled; } }
        private static Texture buttonCheckboxDisabled;
        public static Texture ButtonCheckboxDisabled { get { if(buttonCheckboxDisabled == null) buttonCheckboxDisabled = GetTexture("buttonCheckboxDisabled", SharedImagesPath); return buttonCheckboxDisabled; } }
        private static Texture buttonCheckboxEnabled;
        public static Texture ButtonCheckboxEnabled { get { if(buttonCheckboxEnabled == null) buttonCheckboxEnabled = GetTexture("buttonCheckboxEnabled", SharedImagesPath); return buttonCheckboxEnabled; } }
        private static Texture buttonCheckboxPressed;
        public static Texture ButtonCheckboxPressed { get { if(buttonCheckboxPressed == null) buttonCheckboxPressed = GetTexture("buttonCheckboxPressed", SharedImagesPath); return buttonCheckboxPressed; } }

        public static GTexture ButtonGreen = new GTexture(SharedImagesPath, "buttonGreen");
        public static GTexture ButtonBlue = new GTexture(SharedImagesPath, "buttonBlue");
        public static GTexture ButtonPurple = new GTexture(SharedImagesPath, "buttonPurple");
        public static GTexture ButtonOrange = new GTexture(SharedImagesPath, "buttonOrange");
        public static GTexture ButtonRed = new GTexture(SharedImagesPath, "buttonRed");
        public static GTexture ButtonGreyLight = new GTexture(SharedImagesPath, "buttonGreyLight");
        public static GTexture ButtonGreyMild = new GTexture(SharedImagesPath, "buttonGreyMild");
        public static GTexture ButtonGreyDark = new GTexture(SharedImagesPath, "buttonGreyDark");

        private static Texture separatorBlack;
        public static Texture SeparatorBlack { get { if(separatorBlack == null) separatorBlack = GetTexture("separatorBlack", SharedImagesPath); return separatorBlack; } }
        private static Texture separatorBlue;
        public static Texture SeparatorBlue { get { if(separatorBlue == null) separatorBlue = GetTexture("separatorBlue", SharedImagesPath); return separatorBlue; } }
        private static Texture separatorGreen;
        public static Texture SeparatorGreen { get { if(separatorGreen == null) separatorGreen = GetTexture("separatorGreen", SharedImagesPath); return separatorGreen; } }
        private static Texture separatorOrange;
        public static Texture SeparatorOrange { get { if(separatorOrange == null) separatorOrange = GetTexture("separatorOrange", SharedImagesPath); return separatorOrange; } }
        private static Texture separatorPurple;
        public static Texture SeparatorPurple { get { if(separatorPurple == null) separatorPurple = GetTexture("separatorPurple", SharedImagesPath); return separatorPurple; } }
        private static Texture separatorRed;
        public static Texture SeparatorRed { get { if(separatorRed == null) separatorRed = GetTexture("separatorRed", SharedImagesPath); return separatorRed; } }
    }
}

namespace Ez.Internal
{
    [Serializable]
    public class GTexture
    {
        private string _filePath = "";

        private string _normalFileName = "";
        private Texture _normal;
        public Texture normal { get { if(_normal == null) { _normal = GetTexture(_normalFileName, _filePath); } return _normal; } }
        public Texture2D normal2D { get { return (Texture2D)normal; } }

        private string _activeFileName = "";
        private Texture _active;
        public Texture active { get { if(_active == null) { _active = GetTexture(_activeFileName, _filePath); } return _active; } }
        public Texture2D active2D { get { return (Texture2D)active; } }

        private string _hoverFileName = "";
        private Texture _hover;
        public Texture hover { get { if(_hover == null) { _hover = GetTexture(_hoverFileName, _filePath); } return _hover; } }
        public Texture2D hover2D { get { return (Texture2D)hover; } }

        public GTexture(string filePath, string normalFileName, string activeFileName = "", string hoverFileName = "")
        {
            FilePath = filePath;
            NormalFileName = normalFileName;
            ActiveFileName = activeFileName;
            if(string.IsNullOrEmpty(ActiveFileName) && !string.IsNullOrEmpty(NormalFileName)) { ActiveFileName = NormalFileName + "Active"; }
            HoverFileName = hoverFileName;
            if(string.IsNullOrEmpty(HoverFileName) && !string.IsNullOrEmpty(NormalFileName)) { HoverFileName = NormalFileName + "Hover"; }
        }

        public static Texture GetTexture(string fileName, string path) { return AssetDatabase.LoadAssetAtPath<Texture>(path + fileName + ".png"); }

        public string FilePath { get { return _filePath; } set { _filePath = value; } }
        public string NormalFileName { get { return _normalFileName; } set { _normalFileName = value; } }
        public string ActiveFileName { get { return _activeFileName; } set { _activeFileName = value; } }
        public string HoverFileName { get { return _hoverFileName; } set { _hoverFileName = value; } }

        public bool HasFilePath { get { return !string.IsNullOrEmpty(FilePath); } }
        public bool HasNormal { get { return !string.IsNullOrEmpty(NormalFileName) && normal != null; } }
        public bool HasActive { get { return !string.IsNullOrEmpty(ActiveFileName) && active != null; } }
        public bool HasHover { get { return !string.IsNullOrEmpty(HoverFileName) && hover != null; } }
    }

    public enum BorderType
    {
        /// <summary>
        /// No border.
        /// </summary>
        None,
        /// <summary>
        /// Normal border, with the same color all around.
        /// </summary>
        Normal,
        /// <summary>
        /// Two tone border. Lighter on top left and darker on bottom right. Creates a 3D effect.
        /// </summary>
        Shaded
    }

    [Serializable]
    public class GColor
    {
        private Color _dark;
        private Color _light;

        public Color Dark { get { return _dark; } set { _dark = value; } }
        public Color Light { get { return _light; } set { _light = value; } }

        public Color Color { get { return EditorGUIUtility.isProSkin ? Dark : Light; } }

        public GColor(Color color)
        {
            Dark = color;
            Light = color;
        }

        public GColor(int r, int g, int b)
        {
            Dark = new Color(r, g, b);
            Light = new Color(r, g, b);
        }

        public GColor(int r, int g, int b, int a)
        {
            Dark = new Color(r, g, b, a);
            Light = new Color(r, g, b, a);
        }

        public GColor(Color dark, Color light)
        {
            Dark = dark;
            Light = light;
        }

        public GColor(int r_dark, int g_dark, int b_dark, int r_light, int g_light, int b_light)
        {
            Dark = ColorFrom256(r_dark, g_dark, b_dark);
            Light = ColorFrom256(r_light, g_light, b_light);
        }

        public GColor(int r_dark, int g_dark, int b_dark, int a_dark, int r_light, int g_light, int b_light, int a_light)
        {
            Dark = ColorFrom256(r_dark, g_dark, b_dark, a_dark);
            Light = ColorFrom256(r_light, g_light, b_light, a_light);
        }

        public static Color ColorFrom256(int r, int g, int b) { return new Color(r / 255f, g / 255f, b / 255f); }
        public static Color ColorFrom256(int r, int g, int b, int a) { return new Color(r / 255f, g / 255f, b / 255f, a / 255f); }
    }

    [Serializable]
    public class GeneratedTexture
    {
        private GColor _contentColor;
        private GColor _borderMainColor;
        private GColor _borderSecondaryColor;
        private int _borderWidth;
        private Texture2D _texture2D;

        public GColor ContentColor { get { return _contentColor; } set { _contentColor = value; } }
        public GColor BorderMainColor { get { return _borderMainColor; } set { _borderMainColor = value; } }
        public GColor BorderSecondaryColor { get { return _borderSecondaryColor; } set { _borderSecondaryColor = value; } }
        public int BorderWidth { get { return _borderWidth; } set { _borderWidth = value; } }
        public BorderType BorderType = BorderType.None;

        public Texture2D Texture2D { get { if(_texture2D == null) { _texture2D = GenerateTexture(ContentColor, BorderMainColor, BorderSecondaryColor, BorderType, BorderWidth); } return _texture2D; } }
        public Texture Texture { get { return Texture2D; } }

        public GeneratedTexture(GColor content)
        {
            ContentColor = content;
            BorderMainColor = content;
            BorderSecondaryColor = content;
            BorderType = BorderType.None;
            BorderWidth = 1;
        }

        public GeneratedTexture(GColor content, GColor border, BorderType borderType = BorderType.Normal, int borderWidth = 1)
        {
            ContentColor = content;
            BorderMainColor = border;
            BorderSecondaryColor = border;
            BorderType = borderType;
            BorderWidth = borderWidth;
        }

        public GeneratedTexture(GColor content, GColor lightBorder, GColor darkBorder, BorderType borderType = BorderType.Shaded, int borderWidth = 2)
        {
            ContentColor = content;
            BorderMainColor = lightBorder;
            BorderSecondaryColor = darkBorder;
            BorderType = borderType;
            BorderWidth = borderWidth;
        }

        public Texture2D GetTexture2D(int width = 8, int height = 8, bool saveTexture = true)
        {
            if(!saveTexture) { return GenerateTexture(ContentColor, BorderMainColor, BorderSecondaryColor, BorderType, BorderWidth, width, height); }
            _texture2D = GenerateTexture(ContentColor, BorderMainColor, BorderSecondaryColor, BorderType, BorderWidth, width, height);
            return Texture2D;
        }

        public Texture GetTexture(int width = 8, int height = 8, bool saveTexture = true)
        {
            return GetTexture2D(width, height, saveTexture);
        }

        /// <summary>
        /// Generates a Texture2D with the given settings.
        /// </summary>
        /// <param name="content">Colors for the content(center color).</param>
        /// <param name="borderMain">Colors for the main border. </param>
        /// <param name="borderSecondary">Colors for the secondary border (used with BorderType.Shaded)</param>
        /// <param name="borderType">Border Type.</param>
        /// <param name="borderWidth">Border thickness.</param>
        /// <param name="width">Texture's width.</param>
        /// <param name="height">Texture's height.</param>
        /// <returns></returns>
        public static Texture2D GenerateTexture(GColor content, GColor borderMain, GColor borderSecondary, BorderType borderType, int borderWidth = 1, int width = 8, int height = 8)
        {
            if(borderWidth < 0) { borderWidth = 0; } else if(borderWidth > width / 2 || borderWidth > height / 2) { borderWidth = ((width > height ? height : width) / 2) - 1; }
            Texture2D texture = new Texture2D(width, height, TextureFormat.ARGB32, false);
            if(borderType == BorderType.None)
            {
                for(int y = 0; y < width; y++) { for(int x = 0; x < height; x++) { texture.SetPixel(x, y, content.Color); } }
            }
            else if(borderType == BorderType.Normal)
            {
                for(int y = 0; y < width; y++)
                {
                    for(int x = 0; x < height; x++)
                    {
                        if(x <= (borderWidth - 1) || x >= (width - borderWidth))
                        {
                            texture.SetPixel(x, y, borderMain.Color);
                        }
                        else if(y <= (borderWidth - 1) || y >= (height - borderWidth))
                        {
                            texture.SetPixel(x, y, borderMain.Color);
                        }
                        else
                        {
                            texture.SetPixel(x, y, content.Color);
                        }
                    }
                }
            }
            else if(borderType == BorderType.Shaded)
            {
                //ToDo: make the shaded texture
            }
            texture.Apply();
            return texture;
        }
    }
}