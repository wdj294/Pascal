// Copyright (c) 2016 Ez Entertainment SRL. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using UnityEngine;

namespace Ez
{
    public partial class EzResources
    {
        private static string pathDataManagerImages;
        public static string PathDataManagerImages { get { if (string.IsNullOrEmpty(pathDataManagerImages)) pathDataManagerImages = FileHelper.GetRelativeFolderPath("Ez") + "/DataManager/Images/"; return pathDataManagerImages; } }

        //h_bar_data_manager
        private static Texture headerBarDataManager;
        public static Texture HeaderBarDataManager { get { if (headerBarDataManager == null) headerBarDataManager = EzEditorUtility.GetTexture("h_bar_data_manager", PathDataManagerImages) as Texture; return headerBarDataManager; } }

        //icon_ads @128x128
        private static Texture iconDataManager;
        public static Texture IconDataManager { get { if (iconDataManager == null) iconDataManager = EzEditorUtility.GetTexture("icon_data_manager", PathDataManagerImages) as Texture; return iconDataManager; } }
    }
}