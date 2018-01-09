// Copyright (c) 2016-2017 Ez Entertainment SRL. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using UnityEngine;

namespace Ez
{
    public class EColor
    {
        public static Color ColorFrom256(int r, int g, int b) { return new Color(r / 256f, g / 256f, b / 256f); }
        public static Color ColorFrom256(int r, int g, int b, int a) { return new Color(r / 256f, g / 256f, b / 256f, a / 256f); }

        public static Color BackgroundBlue { get { return ColorFrom256(41, 166, 222, 25); } }
        public static Color BackgroundGreen { get { return ColorFrom256(122, 201, 67, 25); } }
        public static Color BackgroundOrange { get { return ColorFrom256(255, 147, 30, 25); } }
        public static Color BackgroundRed { get { return ColorFrom256(235, 29, 37, 25); } }
        public static Color BackgroundPurple { get { return ColorFrom256(173, 90, 137, 25); } }

        public static Color BlueLight { get { return ColorFrom256(169, 219, 242); } }
        public static Color GreenLight { get { return ColorFrom256(202, 233, 180); } }
        public static Color OrangeLight { get { return ColorFrom256(255, 212, 165); } }
        public static Color RedLight { get { return ColorFrom256(247, 165, 168); } }
        public static Color PurpleLight { get { return ColorFrom256(222, 189, 208); } }

        public static Color Blue { get { return ColorFrom256(41, 166, 222); } }
        public static Color Green { get { return ColorFrom256(122, 201, 67); } }
        public static Color Orange { get { return ColorFrom256(255, 147, 30); } }
        public static Color Red { get { return ColorFrom256(235, 29, 37); } }
        public static Color Purple { get { return ColorFrom256(173, 90, 137); } }

        public static Color BlueMild { get { return ColorFrom256(31, 125, 167); } }
        public static Color GreenMild { get { return ColorFrom256(92, 151, 50); } }
        public static Color OrangeMild { get { return ColorFrom256(191, 110, 22); } }
        public static Color RedMild { get { return ColorFrom256(176, 22, 28); } }
        public static Color PurpleMild { get { return ColorFrom256(130, 68, 103); } }

        public static Color BlueDark { get { return ColorFrom256(21, 83, 111); } }
        public static Color GreenDark { get { return ColorFrom256(61, 101, 34); } }
        public static Color OrangeDark { get { return ColorFrom256(128, 74, 15); } }
        public static Color RedDark { get { return ColorFrom256(118, 15, 19); } }
        public static Color PurpleDark { get { return ColorFrom256(87, 45, 69); } }


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
