// Copyright (c) 2016 Ez Entertainment SRL. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using UnityEngine;

namespace Ez
{
    public partial class EzResources
    {
        private static string pathSharedImages;
        public static string PathSharedImages { get { if (string.IsNullOrEmpty(pathSharedImages)) pathSharedImages = FileHelper.GetRelativeFolderPath("Ez") + "/Shared/Images/"; return pathSharedImages; } }

        #region Generated Textures - HelpBoxDark, HelpBoxLight
        private static Texture2D helpBoxDark;
        public static Texture2D HelpBoxDark
        {
            get
            {
                if (helpBoxDark == null)
                {
                    Color borderColor = EzColors.D3_GREY;
                    Color contentColor = EzColors.D2_GREY;
                    helpBoxDark = new Texture2D(4, 4, TextureFormat.ARGB32, false); // Create a new 2x2 texture ARGB32 (32 bit with alpha) and no mipmaps
                    helpBoxDark.SetPixel(0, 0, borderColor);
                    helpBoxDark.SetPixel(0, 1, borderColor);
                    helpBoxDark.SetPixel(0, 2, borderColor);
                    helpBoxDark.SetPixel(0, 3, borderColor);
                    helpBoxDark.SetPixel(1, 0, borderColor);
                    helpBoxDark.SetPixel(1, 1, contentColor);
                    helpBoxDark.SetPixel(1, 2, contentColor);
                    helpBoxDark.SetPixel(1, 3, borderColor);
                    helpBoxDark.SetPixel(2, 0, borderColor);
                    helpBoxDark.SetPixel(2, 1, contentColor);
                    helpBoxDark.SetPixel(2, 2, contentColor);
                    helpBoxDark.SetPixel(2, 3, borderColor);
                    helpBoxDark.SetPixel(3, 0, borderColor);
                    helpBoxDark.SetPixel(3, 1, borderColor);
                    helpBoxDark.SetPixel(3, 2, borderColor);
                    helpBoxDark.SetPixel(3, 3, borderColor);
                    helpBoxDark.Apply();
                }
                return helpBoxDark;
            }
        }

        private static Texture2D helpBoxLight;
        public static Texture2D HelpBoxLight
        {
            get
            {
                if (helpBoxLight == null)
                {
                    Color borderColor = EzColors.UNITY_DARK;
                    Color contentColor = EzColors.UNITY_LIGHT;
                    helpBoxLight = new Texture2D(4, 4, TextureFormat.ARGB32, false); // Create a new 2x2 texture ARGB32 (32 bit with alpha) and no mipmaps
                    helpBoxLight.SetPixel(0, 0, borderColor);
                    helpBoxLight.SetPixel(0, 1, borderColor);
                    helpBoxLight.SetPixel(0, 2, borderColor);
                    helpBoxLight.SetPixel(0, 3, borderColor);
                    helpBoxLight.SetPixel(1, 0, borderColor);
                    helpBoxLight.SetPixel(1, 1, contentColor);
                    helpBoxLight.SetPixel(1, 2, contentColor);
                    helpBoxLight.SetPixel(1, 3, borderColor);
                    helpBoxLight.SetPixel(2, 0, borderColor);
                    helpBoxLight.SetPixel(2, 1, contentColor);
                    helpBoxLight.SetPixel(2, 2, contentColor);
                    helpBoxLight.SetPixel(2, 3, borderColor);
                    helpBoxLight.SetPixel(3, 0, borderColor);
                    helpBoxLight.SetPixel(3, 1, borderColor);
                    helpBoxLight.SetPixel(3, 2, borderColor);
                    helpBoxLight.SetPixel(3, 3, borderColor);
                    helpBoxLight.Apply();
                }
                return helpBoxLight;
            }
        }

        private static Texture2D boxOrange;
        public static Texture2D BoxOrange
        {
            get
            {
                if (boxOrange == null)
                {
                    Color borderColor = EzColors.D_ORANGE;
                    Color contentColor = EzColors.ORANGE;
                    boxOrange = new Texture2D(4, 4, TextureFormat.ARGB32, false); // Create a new 2x2 texture ARGB32 (32 bit with alpha) and no mipmaps
                    boxOrange.SetPixel(0, 0, borderColor);
                    boxOrange.SetPixel(0, 1, borderColor);
                    boxOrange.SetPixel(0, 2, borderColor);
                    boxOrange.SetPixel(0, 3, borderColor);
                    boxOrange.SetPixel(1, 0, borderColor);
                    boxOrange.SetPixel(1, 1, contentColor);
                    boxOrange.SetPixel(1, 2, contentColor);
                    boxOrange.SetPixel(1, 3, borderColor);
                    boxOrange.SetPixel(2, 0, borderColor);
                    boxOrange.SetPixel(2, 1, contentColor);
                    boxOrange.SetPixel(2, 2, contentColor);
                    boxOrange.SetPixel(2, 3, borderColor);
                    boxOrange.SetPixel(3, 0, borderColor);
                    boxOrange.SetPixel(3, 1, borderColor);
                    boxOrange.SetPixel(3, 2, borderColor);
                    boxOrange.SetPixel(3, 3, borderColor);
                    boxOrange.Apply();
                }
                return boxOrange;
            }
        }

        #endregion

        #region  Buttons 18x18 - Green, Blue, Purple, Red, Orange, GreyLight, GreyMild, GreyDark
        private static Texture btnGreenActive, btnGreenNormal, btnBlueActive, btnBlueNormal, btnPurpleActive, btnPurpleNormal, btnRedActive, btnRedNormal, btnOrangeActive, btnOrangeNormal, btnGreyLightNormal, btnGreyLightActive, btnGreyMildNormal, btnGreyMildActive, btnGreyDarkNormal, btnGreyDarkActive;
        public static Texture BtnGreenActive { get { if (btnGreenActive == null) btnGreenActive = EzEditorUtility.GetTexture("b_green_active", PathSharedImages) as Texture; return btnGreenActive; } }
        public static Texture BtnGreenNormal { get { if (btnGreenNormal == null) btnGreenNormal = EzEditorUtility.GetTexture("b_green_normal", PathSharedImages) as Texture; return btnGreenNormal; } }
        public static Texture BtnBlueActive { get { if (btnBlueActive == null) btnBlueActive = EzEditorUtility.GetTexture("b_blue_active", PathSharedImages) as Texture; return btnBlueActive; } }
        public static Texture BtnBlueNormal { get { if (btnBlueNormal == null) btnBlueNormal = EzEditorUtility.GetTexture("b_blue_normal", PathSharedImages) as Texture; return btnBlueNormal; } }
        public static Texture BtnPurpleActive { get { if (btnPurpleActive == null) btnPurpleActive = EzEditorUtility.GetTexture("b_purple_active", PathSharedImages) as Texture; return btnPurpleActive; } }
        public static Texture BtnPurpleNormal { get { if (btnPurpleNormal == null) btnPurpleNormal = EzEditorUtility.GetTexture("b_purple_normal", PathSharedImages) as Texture; return btnPurpleNormal; } }
        public static Texture BtnRedActive { get { if (btnRedActive == null) btnRedActive = EzEditorUtility.GetTexture("b_red_active", PathSharedImages) as Texture; return btnRedActive; } }
        public static Texture BtnRedNormal { get { if (btnRedNormal == null) btnRedNormal = EzEditorUtility.GetTexture("b_red_normal", PathSharedImages) as Texture; return btnRedNormal; } }
        public static Texture BtnOrangeActive { get { if (btnOrangeActive == null) btnOrangeActive = EzEditorUtility.GetTexture("b_orange_active", PathSharedImages) as Texture; return btnOrangeActive; } }
        public static Texture BtnOrangeNormal { get { if (btnOrangeNormal == null) btnOrangeNormal = EzEditorUtility.GetTexture("b_orange_normal", PathSharedImages) as Texture; return btnOrangeNormal; } }
        public static Texture BtnGreyLightNormal { get { if (btnGreyLightNormal == null) btnGreyLightNormal = EzEditorUtility.GetTexture("b_grey_light_normal", PathSharedImages) as Texture; return btnGreyLightNormal; } }
        public static Texture BtnGreyLightActive { get { if (btnGreyLightActive == null) btnGreyLightActive = EzEditorUtility.GetTexture("b_grey_light_active", PathSharedImages) as Texture; return btnGreyLightActive; } }
        public static Texture BtnGreyMildNormal { get { if (btnGreyMildNormal == null) btnGreyMildNormal = EzEditorUtility.GetTexture("b_grey_mild_normal", PathSharedImages) as Texture; return btnGreyMildNormal; } }
        public static Texture BtnGreyMildActive { get { if (btnGreyMildActive == null) btnGreyMildActive = EzEditorUtility.GetTexture("b_grey_mild_active", PathSharedImages) as Texture; return btnGreyMildActive; } }
        public static Texture BtnGreyDarkNormal { get { if (btnGreyDarkNormal == null) btnGreyDarkNormal = EzEditorUtility.GetTexture("b_grey_dark_normal", PathSharedImages) as Texture; return btnGreyDarkNormal; } }
        public static Texture BtnGreyDarkActive { get { if (btnGreyDarkActive == null) btnGreyDarkActive = EzEditorUtility.GetTexture("b_grey_dark_active", PathSharedImages) as Texture; return btnGreyDarkActive; } }
        #endregion


        //icon_ez @64x64
        private static Texture iconEz;
        public static Texture IconEz { get { if (iconEz == null) iconEz = EzEditorUtility.GetTexture("icon_ez", PathSharedImages) as Texture; return iconEz; } }

        //bg_general_background
        private static Texture generalBackground;
        public static Texture GeneralBackground { get { if (generalBackground == null) generalBackground = EzEditorUtility.GetTexture("bg_general_background", PathSharedImages) as Texture; return generalBackground; } }

        //editor_is_compiling @420x72
        private static Texture editorIsCompiling;
        public static Texture EditorIsCompiling { get { if (editorIsCompiling == null) editorIsCompiling = EzEditorUtility.GetTexture("editor_is_compiling", PathSharedImages) as Texture; return editorIsCompiling; } }

        //editor_is_in_play_mode @420x72
        private static Texture editorIsInPlayMode;
        public static Texture EditorIsInPlayMode { get { if (editorIsInPlayMode == null) editorIsInPlayMode = EzEditorUtility.GetTexture("editor_is_in_play_mode", PathSharedImages) as Texture; return editorIsInPlayMode; } }

        #region Button Bars @420x16
        //b_bar_grey_normal_open | b_bar_grey_active | b_bar_grey_normal_closed
        private static Texture btnBarGreyNormalOpen, btnBarGreyActive, btnBarGreyNormalClosed;
        public static Texture BtnBarGreyNormalOpen { get { if (btnBarGreyNormalOpen == null) btnBarGreyNormalOpen = EzEditorUtility.GetTexture("b_bar_grey_normal_open", PathSharedImages) as Texture; return btnBarGreyNormalOpen; } }
        public static Texture BtnBarGreyActive { get { if (btnBarGreyActive == null) btnBarGreyActive = EzEditorUtility.GetTexture("b_bar_grey_active", PathSharedImages) as Texture; return btnBarGreyActive; } }
        public static Texture BtnBarGreyNormalClosed { get { if (btnBarGreyNormalClosed == null) btnBarGreyNormalClosed = EzEditorUtility.GetTexture("b_bar_grey_normal_closed", PathSharedImages) as Texture; return btnBarGreyNormalClosed; } }

        //b_bar_blue_normal_open | b_bar_blue_active | b_bar_blue_normal_closed
        private static Texture btnBarBlueNormalOpen, btnBarBlueActive, btnBarBlueNormalClosed;
        public static Texture BtnBarBlueNormalOpen { get { if (btnBarBlueNormalOpen == null) btnBarBlueNormalOpen = EzEditorUtility.GetTexture("b_bar_blue_normal_open", PathSharedImages) as Texture; return btnBarBlueNormalOpen; } }
        public static Texture BtnBarBlueActive { get { if (btnBarBlueActive == null) btnBarBlueActive = EzEditorUtility.GetTexture("b_bar_blue_active", PathSharedImages) as Texture; return btnBarBlueActive; } }
        public static Texture BtnBarBlueNormalClosed { get { if (btnBarBlueNormalClosed == null) btnBarBlueNormalClosed = EzEditorUtility.GetTexture("b_bar_blue_normal_closed", PathSharedImages) as Texture; return btnBarBlueNormalClosed; } }

        //b_bar_green_normal_open | b_bar_green_active | b_bar_green_normal_closed
        private static Texture btnBarGreenNormalOpen, btnBarGreenActive, btnBarGreenNormalClosed;
        public static Texture BtnBarGreenNormalOpen { get { if (btnBarGreenNormalOpen == null) btnBarGreenNormalOpen = EzEditorUtility.GetTexture("b_bar_green_normal_open", PathSharedImages) as Texture; return btnBarGreenNormalOpen; } }
        public static Texture BtnBarGreenActive { get { if (btnBarGreenActive == null) btnBarGreenActive = EzEditorUtility.GetTexture("b_bar_green_active", PathSharedImages) as Texture; return btnBarGreenActive; } }
        public static Texture BtnBarGreenNormalClosed { get { if (btnBarGreenNormalClosed == null) btnBarGreenNormalClosed = EzEditorUtility.GetTexture("b_bar_green_normal_closed", PathSharedImages) as Texture; return btnBarGreenNormalClosed; } }

        //b_bar_orange_normal_open | b_bar_orange_active | b_bar_orange_normal_closed
        private static Texture btnBarOrangeNormalOpen, btnBarOrangeActive, btnBarOrangeNormalClosed;
        public static Texture BtnBarOrangeNormalOpen { get { if (btnBarOrangeNormalOpen == null) btnBarOrangeNormalOpen = EzEditorUtility.GetTexture("b_bar_orange_normal_open", PathSharedImages) as Texture; return btnBarOrangeNormalOpen; } }
        public static Texture BtnBarOrangeActive { get { if (btnBarOrangeActive == null) btnBarOrangeActive = EzEditorUtility.GetTexture("b_bar_orange_active", PathSharedImages) as Texture; return btnBarOrangeActive; } }
        public static Texture BtnBarOrangeNormalClosed { get { if (btnBarOrangeNormalClosed == null) btnBarOrangeNormalClosed = EzEditorUtility.GetTexture("b_bar_orange_normal_closed", PathSharedImages) as Texture; return btnBarOrangeNormalClosed; } }

        //b_bar_red_normal_open | b_bar_red_active | b_bar_red_normal_closed
        private static Texture btnBarRedNormalOpen, btnBarRedActive, btnBarRedNormalClosed;
        public static Texture BtnBarRedNormalOpen { get { if (btnBarRedNormalOpen == null) btnBarRedNormalOpen = EzEditorUtility.GetTexture("b_bar_red_normal_open", PathSharedImages) as Texture; return btnBarRedNormalOpen; } }
        public static Texture BtnBarRedActive { get { if (btnBarRedActive == null) btnBarRedActive = EzEditorUtility.GetTexture("b_bar_red_active", PathSharedImages) as Texture; return btnBarRedActive; } }
        public static Texture BtnBarRedNormalClosed { get { if (btnBarRedNormalClosed == null) btnBarRedNormalClosed = EzEditorUtility.GetTexture("b_bar_red_normal_closed", PathSharedImages) as Texture; return btnBarRedNormalClosed; } }

        //b_bar_purple_normal_open | b_bar_purple_active | b_bar_purple_normal_closed
        private static Texture btnBarPurpleNormalOpen, btnBarPurpleActive, btnBarPurpleNormalClosed;
        public static Texture BtnBarPurpleNormalOpen { get { if (btnBarPurpleNormalOpen == null) btnBarPurpleNormalOpen = EzEditorUtility.GetTexture("b_bar_purple_normal_open", PathSharedImages) as Texture; return btnBarPurpleNormalOpen; } }
        public static Texture BtnBarPurpleActive { get { if (btnBarPurpleActive == null) btnBarPurpleActive = EzEditorUtility.GetTexture("b_bar_purple_active", PathSharedImages) as Texture; return btnBarPurpleActive; } }
        public static Texture BtnBarPurpleNormalClosed { get { if (btnBarPurpleNormalClosed == null) btnBarPurpleNormalClosed = EzEditorUtility.GetTexture("b_bar_purple_normal_closed", PathSharedImages) as Texture; return btnBarPurpleNormalClosed; } }
        #endregion

        #region Sayings
        private static string[] sayings = new string[]
        {
            "A team of highly trained monkeys is checking your code.",
            "Much code, so smart.",
            "Panda is watching you.",
            "Watcha doin?",
            "Keep calm and code.",
            "If you don't succeed at first, hide all evidence that you tried.",
            "There is no present. There is only the immediate future \nand the recent past.",
            "Artificial Intelligence usually beats natural stupidity.",
            "If at first you don't succeed; call it version 1.0",
            "See daddy? All the keys are in alphabetical order now.",
            "Hey! If it compiles, ship it!",
            "Relax, it's only ONES and ZEROS!",
            "If brute force doesn't solve your problems, \nthen you aren't using enough.",
            "Programming is like sex, one mistake and you have to support it \nfor the rest of your life.",
            "There are 10 types of people in the world: \nthose who understand binary, and those who don't.",
            "1f u c4n r34d th1s u r34lly n33d t0 g37 l41d",
            "Don't worry if it doesn't work right. \nIf everything did, you'd be out of a job.",
            "In theory, theory and practice are the same. \nIn practice, they're not.",
            "To iterate is human, to recurse divine.",
            "The best thing about a boolean is even if you are wrong, \nyou are only off by a bit.",
            "But what is it good for?",
            "The best method for accelerating a computer \nis the one that boosts it by 9.8 m/s2.",
            "Evil does not wear a BONNET!",
            "Chillax",
            "Fo Shizzle",
            "YOLO",
            "To make a mistake is human, but to blame it on someone else, \nthat's even more human.",
            "There is a fine line between coding \nand looking at the screen like an idiot.",
            "You never run out of things that can go wrong.",
            "Programmers come and go, but bugs remain and build up.",
            "Fingers crossed.",
            "If you have an issue, get a tissue.",
            "If at first you don't succeed, order some pizza.",
            "I have a new hair style today, it's called 'I tried.'",
            "The only thing you have to fear is fear itself... and spiders.",
            "Have some patience, I'm screwing things up as fast as possible.",
            "If you get an error just add LOL at the end.",
            "I just wanted you to know that somebody cares. \nNot me, but somebody does.",
            "I solemnly swear that I am up to no good.",
            "My advice is to never listen to any advice, not even this one.",
            "Doing nothing is hard, you never know when you're done.",
            "Sometimes you succeed... and other times you learn.",
            "Tell your boss what you really think about him, \nand the truth shall set you free, from your job.",
            "3 monkeys escaped from the zoo, one was caught watching TV, the other playing games, and the 3rd one was caught reading your code!",
            "You aren't doing it wrong, if no one knows what you are doing.",
            "You inspire my inner serial killer.",
            "Well, aren't you a ray of pitch black.",
            "It's beginning to look a lot like fuck this.",
            "Hands up if you didn't get enough sleep last night!",
            "I wish everything was as easy as getting fat.",
            "The key to happiness is low expectations. \nLower. Nope, even lower. There you go.",
            "Nothing sucks more than that moment during a compile \nwhen you realize you're wrong.",
            "Most people have 'ah ha' moments. \nI have 'oh for fuck's sake, fuck this shit' moments.",
            "PATIENCE: What you have when there are too many witnesses.",
            "Brain: I see you are trying to sleep. May I offer a selection of your worst memories from the last 10 years?",
            "My favorite time of the year is... when the bugs start to die.",
            "I try to contain my crazy, but then you pressed 'compile'.",
            "I would like to thank my middle finger \nfor always sticking up for me when I needed it.",
            "Panic, Chaos, Pandemonium... My work here is finished.",
            "Roses are red, violets are blue, I'm schizophrenic, /nand so am I.",
            "Progress is man's ability to complicate simplicity.",
            "All generalizations are false, \nincluding this one.",
            "I'm undaunted in my quest to amuse myself \nby constantly changing my code.",
            "The next time you have a thought... let it go.",
            "This suspense is terrible. I hope it will last.",
            "If you don't mind, it doesn't matter.",
            "Doesn't expecting the unexpected make the unexpected expected?",
            "Smile... It confuses people...",
            "If at first you don't succeed, \nthen skydiving is not for you.",
            "The universe contains protons, neutrons, electrons and morons.",
            "Dude! You're scaring me... Stop Smiling!",
            "Programmer(noun.): A machine that turns coffee into code.",
            "Hardware(noun.): The part of a computer that you can kick.",
            "Q: 0 is false and 1 is true, right? /nA: 1",
            "Real programmers count from 0.",
            "3 Database SQL walked into a NoSQL bar. A little while later...\nthey walked out. Because they couldnt find a table.",
            "A SQL query goes into a bar, walks up to two tables and asks... 'Can I join you?'",
            "UNIX is user friendly... It's just very particular about who its friends are.",
            "I don't see women as objects. \nI consider each to be in a class of her own.",
            "How many programmers does it take to change a light bulb?/nNone, it's a hardware problem.",
            "There are three kinds of lies: Lies, damned lies, and benchmarks.",
            "The generation of random numbers is too important to be left to chance.",
            "Debugging: Removing the needles from the haystack.",
            "All your base are belong to us.",
            "Do these pants make my ass look fat?",
            "Click 'Ok' to continue",
            "Run as fast as you can and don't look back.",
            "Replace user and press any key to continue.",
            "Computers make very fast, very accurate mistakes.",
            "The truth is out there. \nAnybody got the URL?",
            "I'm not anti-social; I'm just not user friendly.",
            "My attitude isn't bad. It's in beta.",
            "Enter any 11-digit prime number to continue.",
            "E-mail returned to sender, insufficient voltage.",
            "If I wanted a warm fuzzy feeling, \nI'd antialias my graphics!"
        };
        public static string GetSaying { get { return sayings[Random.Range(0, sayings.Length - 1)]; } }
        #endregion
    }
}
