// Copyright (c) 2016 - 2017 Ez Entertainment SRL. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using UnityEngine;

namespace Ez
{
    public class EColor
    {
        public static Color ColorFrom256(int r, int g, int b) { return new Color(r / 255f, g / 255f, b / 255f); }
        public static Color ColorFrom256(int r, int g, int b, int a) { return new Color(r / 255f, g / 255f, b / 255f, a / 255f); }

        public static Color BackgroundGreen { get { return ColorFrom256(194, 255, 26, 25); } }
        public static Color BackgroundBlue { get { return ColorFrom256(41, 166, 222, 25); } }
        public static Color BackgroundPurple { get { return ColorFrom256(132, 30, 232, 25); } }
        public static Color BackgroundOrange { get { return ColorFrom256(229, 193, 23, 25); } }
        public static Color BackgroundRed { get { return ColorFrom256(228, 50, 38, 25); } }

        public static Color GreenLight { get { return ColorFrom256(218, 255, 118); } }
        public static Color BlueLight { get { return ColorFrom256(127, 202, 235); } }
        public static Color PurpleLight { get { return ColorFrom256(181, 120, 241); } }
        public static Color OrangeLight { get { return ColorFrom256(239, 218, 116); } }
        public static Color RedLight { get { return ColorFrom256(239, 132, 125); } }

        public static Color Green { get { return ColorFrom256(194, 255, 26); } }
        public static Color Blue { get { return ColorFrom256(41, 166, 222); } }
        public static Color Purple { get { return ColorFrom256(132, 30, 232); } }
        public static Color Orange { get { return ColorFrom256(229, 193, 23); } }
        public static Color Red { get { return ColorFrom256(228, 50, 38); } }

        public static Color GreenMild { get { return ColorFrom256(146, 191, 20); } }
        public static Color BlueMild { get { return ColorFrom256(31, 125, 167); } }
        public static Color PurpleMild { get { return ColorFrom256(99, 22, 174); } }
        public static Color OrangeMild { get { return ColorFrom256(172, 145, 17); } }
        public static Color RedMild { get { return ColorFrom256(171, 38, 28); } }

        public static Color GreenDark { get { return ColorFrom256(97, 128, 13); } }
        public static Color BlueDark { get { return ColorFrom256(21, 83, 111); } }
        public static Color PurpleDark { get { return ColorFrom256(66, 15, 116); } }
        public static Color OrangeDark { get { return ColorFrom256(115, 97, 12); } }
        public static Color RedDark { get { return ColorFrom256(114, 25, 19); } }


        public static Color BlueDarker1 { get { return ColorFrom256(20, 88, 122); } }
        public static Color BlueDarker2 { get { return ColorFrom256(12, 54, 71); } }
        public static Color BlueDarker3 { get { return ColorFrom256(4, 16, 20); } }

        public static Color GreyDarker1 { get { return ColorFrom256(83, 83, 83); } }
        public static Color GreyDarker2 { get { return ColorFrom256(55, 55, 55); } }
        public static Color GreyDarker3 { get { return ColorFrom256(28, 28, 28); } }

        public static Color UnityLight { get { return ColorFrom256(194, 194, 194); } }
        public static Color UnityMild { get { return ColorFrom256(125, 125, 125); } }
        public static Color UnityDark { get { return ColorFrom256(56, 56, 56); } }

        public static Color WhiteLight { get { return ColorFrom256(231, 231, 231); } }
        public static Color WhiteDark { get { return ColorFrom256(215, 215, 215); } }
    }
}