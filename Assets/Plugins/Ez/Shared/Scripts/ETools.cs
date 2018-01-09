// Copyright (c) 2016 - 2017 Ez Entertainment SRL. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using UnityEngine;
using System.Collections;
using System;

namespace Ez
{
    public static class ETools
    {
        public static Vector2 Vector2Round(Vector2 v2, int decimals = 1) { return new Vector2((float)Math.Round(v2.x, decimals), (float)Math.Round(v2.y, decimals)); }
        public static Vector3 Vector3Round(Vector3 v3, int decimals = 1) { return new Vector3((float)Math.Round(v3.x, decimals), (float)Math.Round(v3.y, decimals), (float)Math.Round(v3.z, decimals)); }

        public static string Vector2ToString(Vector2 v2, int decimals = 1) { return "(" + Math.Round(v2.x, decimals) + ", " + Math.Round(v2.y, decimals) + ")"; }
        public static string Vector3ToString(Vector3 v3, int decimals = 1) { return "(" + Math.Round(v3.x, decimals) + ", " + Math.Round(v3.y, decimals) + ", " + Math.Round(v3.z, decimals) + ")"; }
    }
}
