// Copyright (c) 2016 Ez Entertainment SRL. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using UnityEngine;

namespace Ez
{
    public static class EzColors
    {
        public static Color GREEN { get { return GraphicUtility.ColorFrom256(194, 255, 26); } }
        public static Color BLUE { get { return GraphicUtility.ColorFrom256(41, 166, 222); } }
        public static Color PURPLE { get { return GraphicUtility.ColorFrom256(132, 30, 232); } }
        public static Color RED { get { return GraphicUtility.ColorFrom256(228, 50, 38); } }
        public static Color ORANGE { get { return GraphicUtility.ColorFrom256(229, 193, 23); } }

        public static Color L_GREEN { get { return GraphicUtility.ColorFrom256(218, 255, 118); } }
        public static Color L_BLUE { get { return GraphicUtility.ColorFrom256(128, 202, 235); } }
        public static Color L_PURPLE { get { return GraphicUtility.ColorFrom256(181, 120, 241); } }
        public static Color L_RED { get { return GraphicUtility.ColorFrom256(239, 132, 125); } }
        public static Color L_ORANGE { get { return GraphicUtility.ColorFrom256(239, 218, 116); } }

        public static Color D_GREEN { get { return GraphicUtility.ColorFrom256(97, 128, 13); } }
        public static Color D_BLUE { get { return GraphicUtility.ColorFrom256(21, 83, 111); } }
        public static Color D_PURPLE { get { return GraphicUtility.ColorFrom256(66, 15, 116); } }
        public static Color D_RED { get { return GraphicUtility.ColorFrom256(114, 25, 19); } }
        public static Color D_ORANGE { get { return GraphicUtility.ColorFrom256(115, 97, 12); } }

        public static Color D1_GREY { get { return GraphicUtility.ColorFrom256(83, 83, 83); } }
        public static Color D2_GREY { get { return GraphicUtility.ColorFrom256(55, 55, 55); } }
        public static Color D3_GREY { get { return GraphicUtility.ColorFrom256(28, 28, 28); } }

        public static Color UNITY_LIGHT { get { return GraphicUtility.ColorFrom256(194, 194, 194); } }
        public static Color UNITY_MILD { get { return GraphicUtility.ColorFrom256(125, 125, 125); } }
        public static Color UNITY_DARK { get { return GraphicUtility.ColorFrom256(56, 56, 56); } }
    }
}
