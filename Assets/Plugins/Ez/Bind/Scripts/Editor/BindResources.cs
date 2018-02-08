// Copyright (c) 2016 - 2017 Ez Entertainment SRL. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Ez.Internal;

namespace Ez
{
    public partial class EResources
    {
        private static string pathBindImages;
        public static string PathBindImages { get { if(string.IsNullOrEmpty(pathBindImages)) pathBindImages = FileHelper.GetRelativeFolderPath("Ez") + "/Bind/Images/"; return pathBindImages; } }

        public static GTexture HeaderBarBind = new GTexture(PathBindImages, "h_bar_bind"); //h_bar_bind @420x36
        public static GTexture HeaderBarBindExtension = new GTexture(PathBindImages, "h_bar_bind_extension"); //h_bar_bind_extension @420x36
        public static GTexture HeaderBarBindAddSource = new GTexture(PathBindImages, "h_bar_bind_add_source"); //h_bar_bind_add_source @420x36
        public static GTexture HeaderBarBindAddObserver = new GTexture(PathBindImages, "h_bar_bind_add_observer"); //h_bar_bind_add_observer @420x36

        public static GTexture BarEzBindMain = new GTexture(PathBindImages, "bar_ez_bind_main"); //bar_ez_bind_main @240x36
        public static GTexture BarEzBindExtension = new GTexture(PathBindImages, "bar_ez_bind_extension"); //bar_ez_bind_extension @240x36

        public static GTexture IconEzBind = new GTexture(PathBindImages, "icon_ez_bind"); //icon_ez_bind @128x128
        public static GTexture IconEzBindTitle = new GTexture(PathBindImages, "icon_ez_bind_title"); //icon_ez_bind_title @128x40
    }
}