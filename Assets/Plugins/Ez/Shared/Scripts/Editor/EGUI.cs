// Copyright (c) 2016-2017 Ez Entertainment SRL. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Ez
{
    public static class EGUI
    {
        #region MarkSceneDirty
        /// <summary>
        /// Marks the current active scene as dirty, prompting a save. To mark all the currently opened scenes as dirty, just pass markAllScenesDirty as true.
        /// </summary>
        public static void MarkSceneDirty(bool markAllScenesDirty = false)
        {
            if(EditorApplication.isPlaying) { return; }
            if(markAllScenesDirty) { EditorSceneManager.MarkAllScenesDirty(); }
            else { EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene()); }
        }
        #endregion

        #region ResetKeyboardFocus
        /// <summary>
        /// Sets the controlID that has keybard control to 0
        /// </summary>
        public static void ResetKeyboardFocus()
        {
            GUIUtility.keyboardControl = 0;
        }
        #endregion

        #region SetDirty
        /// <summary>
        /// Marks target object as dirty. (Only suitable for non-scene objects).
        /// </summary>
        /// <param name="target">The object to mark as dirty.</param>
        public static void SetDirty(Object target)
        {
            EditorUtility.SetDirty(target);
        }
        #endregion

        #region RepaintAllViews / RepaintAnimationWindow / RepaintHierarchyWindow / RepaintProjectWindow
        /// <summary>
        /// Repaints ALL the views. Helps with updating the SceneViews and GameViews from a custom EditorWindow. This is done without the need to have these windows in focus.
        /// </summary>
        public static void RepaintAllViews()
        {
            UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
        }

        /// <summary>
        ///  Can be used to ensure repaint of the AnimationWindow.
        /// </summary>
        public static void RepaintAnimationWindow()
        {
            EditorApplication.RepaintAnimationWindow();
        }

        /// <summary>
        ///  Can be used to ensure repaint of the HierarchyWindow.
        /// </summary>
        public static void RepaintHierarchyWindow()
        {
            EditorApplication.RepaintHierarchyWindow();
        }

        /// <summary>
        ///  Can be used to ensure repaint of the ProjectWindow.
        /// </summary>
        public static void RepaintProjectWindow()
        {
            EditorApplication.RepaintProjectWindow();
        }
        #endregion

        #region ExitGUI
        public static void ExitGUI()
        {
            GUIUtility.ExitGUI();
        }
        #endregion;

        #region IsPersistent
        /// <summary>
        /// Determines if an object is stored on disk.
        /// Typically assets like prefabs, textures, audio clips, animation clips, materials
        /// are stored on disk.Returns false if the object lives in the scene. Typically
        /// this is a game object or component but it could also be a material that was created
        /// from code and not stored in an asset but instead stored in the scene.
        /// </summary>
        /// <param name="target">The object you want to test.</param>
        /// <returns>Returns false if it's a scene object.</returns>
        public static bool IsPersistent(Object target)
        {
            if(target == null) { return false; }
            return EditorUtility.IsPersistent(target);
        }
        #endregion

        #region BeginChangeCheck / EndChangeCheck
        /// <summary>
        /// Check if any control was changed inside a block of code.
        /// When needing to check if GUI.changed is set to true inside a block of code, wrap
        /// the code inside BeginChangeCheck () and EndChangeCheck () like this:
        /// EndChangeCheck will only return true if GUI.changed was set to true inside the
        /// block, but GUI.changed will be true afterwards both if it was set to true inside
        /// and if it was already true to begin with.
        /// </summary>
        public static void BeginChangeCheck()
        {
            EditorGUI.BeginChangeCheck();
        }

        /// <summary>
        /// Ends a change check started with BeginChangeCheck ().
        /// </summary>
        /// <returns>True if GUI.changed was set to true, otherwise false.</returns>
        public static bool EndChangeCheck()
        {
            return EditorGUI.EndChangeCheck();
        }
        #endregion

        #region ResetColors
        /// <summary>
        /// Resets all the GUI colors to their default values
        /// </summary>
        public static void ResetColors() { GUI.color = Color.white; GUI.contentColor = Color.white; GUI.backgroundColor = Color.white; }
        #endregion

        #region GUI.color
        /// <summary>
        /// Set the GUI.color value
        /// </summary>
        public static void SetGUIColor(Color color) { GUI.color = color; }
        /// <summary>
        /// Set the GUI.color value, taking into account if the Editor skin is set to Dark or Light.
        /// </summary>
        /// <param name="colorDark">Dark skin color</param>
        /// <param name="colorLight">Light skin color</param>
        public static void SetGUIColor(Color colorDark, Color colorLight) { GUI.color = EditorGUIUtility.isProSkin ? colorDark : colorLight; }
        /// <summary>
        /// Returns the current value of GUI.color
        /// </summary>
        public static Color GetGUIColor { get { return GUI.color; } }
        #endregion
        #region GUI.contentColor
        /// <summary>
        /// Set the GUI.contentColor value
        /// </summary>
        public static void SetGUIContentColor(Color color) { GUI.contentColor = color; }
        /// <summary>
        /// Set the GUI.contentColor value, taking into account if the Editor skin is set to Dark or Light.
        /// </summary>
        /// <param name="colorDark">Dark skin color</param>
        /// <param name="colorLight">Light skin color</param>
        public static void SetGUIContentColor(Color colorDark, Color colorLight) { GUI.contentColor = EditorGUIUtility.isProSkin ? colorDark : colorLight; }
        /// <summary>
        /// Returns the current value of GUI.contentColor
        /// </summary>
        public static Color GetGUIContentColor { get { return GUI.contentColor; } }
        #endregion
        #region GUI.backgroundColor
        /// <summary>
        /// Set the GUI.backgroundColor value
        /// </summary>
        public static void SetGUIBackgroundColor(Color color) { GUI.backgroundColor = color; }
        /// <summary>
        /// Set the GUI.backgroundColor value, taking into account if the Editor skin is set to Dark or Light.
        /// </summary>
        /// <param name="colorDark">Dark skin color</param>
        /// <param name="colorLight">Light skin color</param>
        public static void SetGUIBackgroundColor(Color colorDark, Color colorLight) { GUI.backgroundColor = EditorGUIUtility.isProSkin ? colorDark : colorLight; }
        /// <summary>
        /// Returns the current value of GUI.backgroundColor
        /// </summary>
        public static Color GetGUIBackgroundColor { get { return GUI.backgroundColor; } }
        #endregion

        #region VerticalSpace / HorizontalSpace
        /// <summary>
        /// Adds a vertical space
        /// </summary>
        /// <param name="pixels">Space height in pixels</param>
        public static void VerticalSpace(float pixels) { EditorGUILayout.BeginVertical(); { GUILayout.Space(pixels); } EditorGUILayout.EndVertical(); }
        /// <summary>
        /// Adds a horizontal space
        /// </summary>
        /// <param name="pixels">Space width in pixels</param>
        public static void HorizontalSpace(float pixels) { EditorGUILayout.BeginHorizontal(); { GUILayout.Space(pixels); } EditorGUILayout.EndHorizontal(); }
        #endregion

        #region Space / FlexibleSpace
        /// <summary>
        /// Insert a space in the current layout group.
        /// The direction of the space is dependent on the layout group you're currently
        /// in when issuing the command. If in a vertical group, the space will be vertical:
        /// Note: This will override the GUILayout.ExpandWidth and GUILayout.ExpandHeightSpace
        /// of 20px between two buttons.
        /// In horizontal groups, the pixels are measured horizontally:
        /// </summary>
        /// <param name="pixels">The number of empty pixels that make up this Space.</param>
        public static void Space(float pixels) { GUILayout.Space(pixels); }
        /// <summary>
        /// Insert a space in the current layout group if the condition is true.
        /// The direction of the space is dependent on the layout group you're currently
        /// in when issuing the command. If in a vertical group, the space will be vertical:
        /// Note: This will override the GUILayout.ExpandWidth and GUILayout.ExpandHeightSpace
        /// of 20px between two buttons.
        /// In horizontal groups, the pixels are measured horizontally:
        /// </summary>
        /// <param name="pixels">The number of empty pixels that make up this Space.</param>
        /// <param name="condition">If true it will insert the Space, with the given pixels value. If false nothing will happen.</param>
        public static void Space(float pixels, bool condition) { if(condition) { Space(pixels); } }

        /// <summary>
        /// Insert a flexible space element.
        /// Flexible spaces use up any leftover space in a layout. Note: This will override
        /// the GUILayout.ExpandWidth and GUILayout.ExpandHeightFlexible Space in a GUILayout
        /// Area.
        /// </summary>
        public static void FlexibleSpace() { GUILayout.FlexibleSpace(); }
        /// <summary>
        /// Insert a flexible space element if the condition is true.
        /// Flexible spaces use up any leftover space in a layout. Note: This will override
        /// the GUILayout.ExpandWidth and GUILayout.ExpandHeightFlexible Space in a GUILayout
        /// Area.
        /// </summary>
        /// <param name="condition">If true it will insert the FlexibleSpace. If false nothing will happen.</param>
        public static void FlexibleSpace(bool condition) { if(condition) { FlexibleSpace(); } }
        #endregion

        #region GetTexture
        /// <summary>
        /// Returns the Texture found at the given path, with the given fileName and the given fileExtension.
        /// </summary>
        /// <param name="fileName">Texture fileName (without the extenstion - eg. '.png')</param>
        /// <param name="path">The path to the texture file</param>
        /// <param name="fileExtension">File extension (default: '.png')</param>
        /// <returns></returns>
        public static Texture GetTexture(string fileName, string path, string fileExtension = ".png") { return AssetDatabase.LoadAssetAtPath<Texture>(path + fileName + fileExtension); }
        #endregion

        #region DrawTexture
        /// <summary>
        /// Draws a Texture at it's default values (width and height)
        /// </summary>
        public static void DrawTexture(Texture texture)
        {
            if(texture == null) { Debug.Log("[Doozy] Texture is null!"); return; }
            Rect rect = GUILayoutUtility.GetRect(0f, 0f);
            rect.width = texture.width;
            rect.height = texture.height;
            GUILayout.Space(rect.height);
            GUI.DrawTexture(rect, texture);
        }
        /// <summary>
        /// Draws a Texture at specified with and height values
        /// </summary>
        public static void DrawTexture(Texture texture, float width, float height)
        {
            if(texture == null) { Debug.Log("[Doozy] Texture is null!"); return; }
            Rect rect = GUILayoutUtility.GetRect(0f, 0f);
            rect.width = width;
            rect.height = height;
            GUILayout.Space(rect.height);
            GUI.DrawTexture(rect, texture);
        }
        #endregion

        #region BeginHorizontal / EndHorizontal
        /// <summary>
        /// Begin a horizontal group and get its rect back.
        /// This is an extension to UnityEngine.GUILayout.BeginHorizontal. It can be used
        /// for making compound controlsHorizontal Compound group.
        /// </summary>
        public static Rect BeginHorizontal()
        {
            return EditorGUILayout.BeginHorizontal();
        }

        /// <summary>
        /// Begin a horizontal group and get its rect back.
        /// This is an extension to UnityEngine.GUILayout.BeginHorizontal. It can be used
        /// for making compound controlsHorizontal Compound group.
        /// </summary>
        /// <param name="width">The Horizontal Compound group's width.</param>
        public static Rect BeginHorizontal(float width)
        {
            return EditorGUILayout.BeginHorizontal(GUILayout.Width(width));
        }

        /// <summary>
        /// Begin a horizontal group and get its rect back.
        /// This is an extension to UnityEngine.GUILayout.BeginHorizontal. It can be used
        /// for making compound controlsHorizontal Compound group.
        /// </summary>
        /// <param name="width">The Horizontal Compound group's width.</param>
        /// <param name="height">The Horizontal Compound group's height.</param>
        public static Rect BeginHorizontal(float width, float height)
        {
            return EditorGUILayout.BeginHorizontal(GUILayout.Width(width), GUILayout.Height(height));
        }

        /// <summary>
        /// Close a group started with BeginHorizontal.
        /// </summary>
        public static void EndHorizontal()
        {
            EditorGUILayout.EndHorizontal();
        }
        #endregion

        #region BeginVertical / EndVertical
        /// <summary>
        /// Begin a vertical group and get its rect back.
        /// This is an extension to UnityEngine.GUILayout.BeginVertical. It can be used
        /// for making compound controlsVertical Compound group.
        /// </summary>
        public static Rect BeginVertical()
        {
            return EditorGUILayout.BeginVertical();
        }

        /// <summary>
        /// Begin a vertical group and get its rect back.
        /// This is an extension to UnityEngine.GUILayout.BeginVertical. It can be used
        /// for making compound controlsVertical Compound group.
        /// </summary>
        /// <param name="width">The Vertical Compound group's width.</param>
        public static Rect BeginVertical(float width)
        {
            return EditorGUILayout.BeginVertical(GUILayout.Width(width));
        }

        /// <summary>
        /// Begin a vertical group and get its rect back.
        /// This is an extension to UnityEngine.GUILayout.BeginVertical. It can be used
        /// for making compound controlsVertical Compound group.
        /// </summary>
        /// <param name="width">The Vertical Compound group's width.</param>
        /// <param name="height">The Vertical Compound group's height.</param>
        public static Rect BeginVertical(float width, float height)
        {
            return EditorGUILayout.BeginVertical(GUILayout.Width(width), GUILayout.Height(height));
        }

        /// <summary>
        /// Close a group started with BeginVertical.
        /// </summary>
        public static void EndVertical()
        {
            EditorGUILayout.EndVertical();
        }
        #endregion

        #region BeginFadeGroup / EndFadeGroup
        /// <summary>
        /// Begins a group that can be be hidden/shown and the transition will be animated.
        /// </summary>
        /// <param name="value">A value between 0 and 1, 0 being hidden, and 1 being fully visible.</param>
        /// <returns>If the group is visible or not.</returns>
        public static bool BeginFadeGroup(float value)
        {
            return EditorGUILayout.BeginFadeGroup(value);
        }

        /// <summary>
        /// Closes a group started with BeginFadeGroup.
        /// </summary>
        public static void EndFadeGroup()
        {
            EditorGUILayout.EndFadeGroup();
        }
        #endregion

        #region Button
        /// <summary>
        /// Make a single press button. The user clicks them and something happens immediately.
        /// </summary>
        /// <param name="text">Text to display on the button.</param>
        /// <param name="style">The style to use.</param>
        /// <returns>Returns true when the users clicks the button.</returns>
        public static bool Button(string text, GUIStyle style) { if(GUILayout.Button(text, style)) { ResetKeyboardFocus(); return true; } return false; }
        /// <summary>
        ///  Make a single press button. The user clicks them and something happens immediately.
        /// </summary>
        /// <param name="text">Text to display on the button.</param>
        /// <param name="style">The style to use.</param>
        /// <param name="width">Set the button's width.</param>
        /// <returns></returns>
        public static bool Button(string text, GUIStyle style, float width) { if(GUILayout.Button(text, style, GUILayout.Width(width))) { ResetKeyboardFocus(); return true; } return false; }

        /// <summary>
        ///  Make a single press button. The user clicks them and something happens immediately.
        /// </summary>
        /// <param name="text">Text to display on the button.</param>
        /// <param name="style">The style to use.</param>
        /// <param name="width">Set the button's width.</param>
        /// <param name="height">Set the button's height.</param>
        /// <returns></returns>
        public static bool Button(string text, GUIStyle style, float width, float height) { if(GUILayout.Button(text, style, GUILayout.Width(width), GUILayout.Height(height))) { ResetKeyboardFocus(); return true; } return false; }
        /// <summary>
        /// Make a single press button. The user clicks them and something happens immediately.
        /// </summary>
        /// <param name="style">The style to use.</param>
        /// <returns>Returns true when the users clicks the button.</returns>
        public static bool Button(GUIStyle style) { if(GUILayout.Button(GUIContent.none, style)) { ResetKeyboardFocus(); return true; } return false; }
        /// <summary>
        /// Make a single press button. The user clicks them and something happens immediately.
        /// </summary>
        /// <param name="style">The style to use.</param>
        /// <param name="width">Set the button's width.</param>
        /// <param name="height">Set the button's height.</param>
        /// <returns>Returns true when the users clicks the button.</returns>
        public static bool Button(GUIStyle style, float width, float height) { if(GUILayout.Button(GUIContent.none, style, GUILayout.Width(width), GUILayout.Height(height))) { ResetKeyboardFocus(); return true; } return false; }
        #endregion

        #region ButtonPlus / ButtonMinus
        /// <summary>
        /// Make a single '+' green button that is 18x18 pixels. The user clicks them and something happens immediately.
        /// </summary>
        /// <returns>Returns true when the users clicks the button.</returns>
        public static bool ButtonPlus()
        {
            return Button("\uf067", EStyles.GetStyle(EStyles.ButtonStyle.ButtonPlus), 18, 18);
        }

        /// <summary>
        /// Make a single '-' red button that is 18x18 pixels. The user clicks them and something happens immediately.
        /// </summary>
        /// <returns>Returns true when the users clicks the button.</returns>
        public static bool ButtonMinus()
        {
            return Button("\uf068", EStyles.GetStyle(EStyles.ButtonStyle.ButtonMinus), 18, 18);
        }

        /// <summary>
        /// Make a single 'reset symbol' dark grey button that is 18x18 pixels. The user clicks them and something happens immediately.
        /// </summary>
        /// <returns>Returns true when the users clicks the button.</returns>
        public static bool ButtonReset()
        {
            return Button("\uf021", EStyles.GetStyle(EStyles.ButtonStyle.ButtonReset), 18, 18);
        }
        #endregion

        #region DrawSeparator
        /// <summary>
        /// Draws a horizontal line with the given texture. The line has a height of 2 pixels.
        /// </summary>
        /// <param name="texture">The texture you want drawn.</param>
        /// <param name="width">Separator's width.</param>
        /// <param name="spacePrefix">Adds a set number of empty pixels in front (indent)</param>
        /// <param name="spaceSufix">Adds a set number of empty pixels at the end</param>
        public static void DrawSeparator(Texture texture, float width, float spacePrefix = 0, float spaceSufix = 0)
        {
            BeginHorizontal(width);
            {
                Space(spacePrefix);
                DrawTexture(texture, width - spacePrefix - spaceSufix, 2);
                Space(spaceSufix);
            }
            EndHorizontal();
        }
        /// <summary>
        /// Draws a green horizontal line. The line has a height of 2 pixels.
        /// </summary>
        /// <param name="width">Separator's width.</param>
        /// <param name="spacePrefix">Adds a set number of empty pixels in front (indent)</param>
        /// <param name="spaceSufix">Adds a set number of empty pixels at the end</param>
        public static void DrawSeparatorGreen(float width, float spacePrefix = 0, float spaceSufix = 0)
        {
            DrawSeparator(EResources.ButtonGreen.normal, width, spacePrefix, spaceSufix);
        }
        /// <summary>
        /// Draws a blue horizontal line. The line has a height of 2 pixels.
        /// </summary>
        /// <param name="width">Separator's width.</param>
        /// <param name="spacePrefix">Adds a set number of empty pixels in front (indent)</param>
        /// <param name="spaceSufix">Adds a set number of empty pixels at the end</param>
        public static void DrawSeparatorBlue(float width, float spacePrefix = 0, float spaceSufix = 0)
        {
            DrawSeparator(EResources.ButtonBlue.normal, width, spacePrefix, spaceSufix);
        }
        /// <summary>
        /// Draws a purple horizontal line. The line has a height of 2 pixels.
        /// </summary>
        /// <param name="width">Separator's width.</param>
        /// <param name="spacePrefix">Adds a set number of empty pixels in front (indent)</param>
        /// <param name="spaceSufix">Adds a set number of empty pixels at the end</param>
        public static void DrawSeparatorPurple(float width, float spacePrefix = 0, float spaceSufix = 0)
        {
            DrawSeparator(EResources.ButtonPurple.normal, width, spacePrefix, spaceSufix);
        }
        /// <summary>
        /// Draws an orange horizontal line. The line has a height of 2 pixels.
        /// </summary>
        /// <param name="width">Separator's width.</param>
        /// <param name="spacePrefix">Adds a set number of empty pixels in front (indent)</param>
        /// <param name="spaceSufix">Adds a set number of empty pixels at the end</param>
        public static void DrawSeparatorOrange(float width, float spacePrefix = 0, float spaceSufix = 0)
        {
            DrawSeparator(EResources.ButtonOrange.normal, width, spacePrefix, spaceSufix);
        }
        /// <summary>
        /// Draws a red horizontal line. The line has a height of 2 pixels.
        /// </summary>
        /// <param name="width">Separator's width.</param>
        /// <param name="spacePrefix">Adds a set number of empty pixels in front (indent)</param>
        /// <param name="spaceSufix">Adds a set number of empty pixels at the end</param>
        public static void DrawSeparatorRed(float width, float spacePrefix = 0, float spaceSufix = 0)
        {
            DrawSeparator(EResources.ButtonRed.normal, width, spacePrefix, spaceSufix);
        }
        /// <summary>
        /// Draws a light grey horizontal line. The line has a height of 2 pixels.
        /// </summary>
        /// <param name="width">Separator's width.</param>
        /// <param name="spacePrefix">Adds a set number of empty pixels in front (indent)</param>
        /// <param name="spaceSufix">Adds a set number of empty pixels at the end</param>
        public static void DrawSeparatorGreyLight(float width, float spacePrefix = 0, float spaceSufix = 0)
        {
            DrawSeparator(EResources.ButtonGreyLight.normal, width, spacePrefix, spaceSufix);
        }
        /// <summary>
        /// Draws a mild grey horizontal line. The line has a height of 2 pixels.
        /// </summary>
        /// <param name="width">Separator's width.</param>
        /// <param name="spacePrefix">Adds a set number of empty pixels in front (indent)</param>
        /// <param name="spaceSufix">Adds a set number of empty pixels at the end</param>
        public static void DrawSeparatorGreyMild(float width, float spacePrefix = 0, float spaceSufix = 0)
        {
            DrawSeparator(EResources.ButtonGreyMild.normal, width, spacePrefix, spaceSufix);
        }
        /// <summary>
        /// Draws a dark grey horizontal line. The line has a height of 2 pixels.
        /// </summary>
        /// <param name="width">Separator's width.</param>
        /// <param name="spacePrefix">Adds a set number of empty pixels in front (indent)</param>
        /// <param name="spaceSufix">Adds a set number of empty pixels at the end</param>
        public static void DrawSeparatorGreyDark(float width, float spacePrefix = 0, float spaceSufix = 0)
        {
            DrawSeparator(EResources.ButtonGreyDark.normal, width, spacePrefix, spaceSufix);
        }
        #endregion

        #region Toggle
        /// <summary>
        /// Make an on/off toggle button.
        /// </summary>
        /// <param name="value">Is the button on or off?</param>
        /// <param name="trueText">The text label to use when true.</param>
        /// <param name="trueStyle">The style to use when true.</param>
        /// <param name="falseText">The text label to use when false.</param>
        /// <param name="falseStyle">The style to use when false.</param>
        /// <param name="width">Set the button's width.</param>
        /// <param name="height">Set the button's height.</param>
        /// <returns>Returns the new value of the button.</returns>
        public static bool Toggle(bool value, string trueText, GUIStyle trueStyle, string falseText, GUIStyle falseStyle, float width, float height)
        {
            return value ? GUILayout.Toggle(value, trueText, trueStyle, GUILayout.Width(width), GUILayout.Height(height)) : GUILayout.Toggle(value, falseText, falseStyle, GUILayout.Width(width), GUILayout.Height(height));
        }
        /// <summary>
        /// Make an on/off toggle button.
        /// </summary>
        /// <param name="value">Is the button on or off?</param>
        /// <param name="trueText">The text label to use when true.</param>
        /// <param name="trueStyle">The style to use when true.</param>
        /// <param name="falseText">The text label to use when false.</param>
        /// <param name="falseStyle">The style to use when false.</param>
        /// <returns>Returns the new value of the button.</returns>
        public static bool Toggle(bool value, string trueText, GUIStyle trueStyle, string falseText, GUIStyle falseStyle)
        {
            return value ? GUILayout.Toggle(value, trueText, trueStyle) : GUILayout.Toggle(value, falseText, falseStyle);
        }
        /// <summary>
        /// Make an on/off toggle button.
        /// </summary>
        /// <param name="value">Is the button on or off?</param>
        /// <param name="trueStyle">The style to use when true.</param>
        /// <param name="falseStyle">The style to use when false.</param>
        /// <param name="width">Set the button's width.</param>
        /// <param name="height">Set the button's height.</param>
        /// <returns>Returns the new value of the button.</returns>
        public static bool Toggle(bool value, GUIStyle trueStyle, GUIStyle falseStyle, float width, float height)
        {
            return value ? GUILayout.Toggle(value, GUIContent.none, trueStyle, GUILayout.Width(width), GUILayout.Height(height)) : GUILayout.Toggle(value, GUIContent.none, falseStyle, GUILayout.Width(width), GUILayout.Height(height));
        }
        /// <summary>
        /// Make an on/off toggle button.
        /// </summary>
        /// <param name="value">Is the button on or off?</param>
        /// <param name="trueStyle">The style to use when true.</param>
        /// <param name="falseStyle">The style to use when false.</param>
        /// <returns>Returns the new value of the button.</returns>
        public static bool Toggle(bool value, GUIStyle trueStyle, GUIStyle falseStyle)
        {
            return value ? GUILayout.Toggle(value, GUIContent.none, trueStyle) : GUILayout.Toggle(value, GUIContent.none, falseStyle);
        }
        /// <summary>
        /// Make an on/off toggle button.
        /// </summary>
        /// <param name="value">Is the button on or off?</param>
        /// <param name="text">The text label to use.</param>
        /// <param name="style">The style to use.</param>
        /// <param name="width">Set the button's width.</param>
        /// <param name="height">Set the button's height.</param>
        /// <returns>Returns the new value of the button.</returns>
        public static bool Toggle(bool value, string text, GUIStyle style, float width, float height)
        {
            return GUILayout.Toggle(value, text, style, GUILayout.Width(width), GUILayout.Height(height));
        }
        /// <summary>
        /// Make an on/off toggle button.
        /// </summary>
        /// <param name="value">Is the button on or off?</param>
        /// <param name="text">The text label to use.</param>
        /// <param name="style">The style to use.</param>
        /// <returns>Returns the new value of the button.</returns>
        public static bool Toggle(bool value, string text, GUIStyle style)
        {
            return GUILayout.Toggle(value, text, style);
        }
        /// <summary>
        /// Make an on/off toggle button.
        /// </summary>
        /// <param name="value">Is the button on or off?</param>
        /// <param name="style">The style to use.</param>
        /// <param name="width">Set the button's width.</param>
        /// <param name="height">Set the button's height.</param>
        /// <returns>Returns the new value of the button.</returns>
        public static bool Toggle(bool value, GUIStyle style, float width, float height)
        {
            return GUILayout.Toggle(value, GUIContent.none, style, GUILayout.Width(width), GUILayout.Height(height));
        }
        /// <summary>
        /// Make an on/off toggle button.
        /// </summary>
        /// <param name="value">Is the button on or off?</param>
        /// <param name="style">The style to use.</param>
        /// <returns>Returns the new value of the button.</returns>
        public static bool Toggle(bool value, GUIStyle style)
        {
            return GUILayout.Toggle(value, GUIContent.none, style, GUILayout.Width(12));
        }
        /// <summary>
        /// Make an on/off toggle button. This is a shhorthand method for the native GUILayout.Toggle method.
        /// </summary>
        /// <param name="value">Is the button on or off?</param>
        /// <param name="text">The text label to use.</param>
        /// <returns>Returns the new value of the button.</returns>
        public static bool Toggle(bool value, string text)
        {
            return GUILayout.Toggle(value, text);
        }
        /// <summary>
        /// Make an on/off toggle button. This is a shhorthand method for the native GUILayout.Toggle method. width = 12
        /// </summary>
        /// <param name="value">Is the button on or off?</param>
        /// <returns>Returns the new value of the button.</returns>
        public static bool Toggle(bool value)
        {
            return GUILayout.Toggle(value, GUIContent.none, GUILayout.Width(12));
        }

        /// <summary>
        /// Make an on/off toggle button.
        /// </summary>
        /// <param name="serializedProperty">The SerializedProperty to make a toggle for.</param>
        /// <param name="trueText">The text label to use when true.</param>
        /// <param name="trueStyle">The style to use when true.</param>
        /// <param name="falseText">The text label to use when false.</param>
        /// <param name="falseStyle">The style to use when false.</param>
        /// <param name="width">Set the button's width.</param>
        /// <param name="height">Set the button's height.</param>
        public static void Toggle(SerializedProperty serializedProperty, string trueText, GUIStyle trueStyle, string falseText, GUIStyle falseStyle, float width, float height)
        {
            serializedProperty.boolValue = serializedProperty.boolValue ? GUILayout.Toggle(serializedProperty.boolValue, trueText, trueStyle, GUILayout.Width(width), GUILayout.Height(height)) : GUILayout.Toggle(serializedProperty.boolValue, falseText, falseStyle, GUILayout.Width(width), GUILayout.Height(height));
        }
        /// <summary>
        /// Make an on/off toggle button.
        /// </summary>
        /// <param name="serializedProperty">The SerializedProperty to make a toggle for.</param>
        /// <param name="trueText">The text label to use when true.</param>
        /// <param name="trueStyle">The style to use when true.</param>
        /// <param name="falseText">The text label to use when false.</param>
        /// <param name="falseStyle">The style to use when false.</param>
        public static void Toggle(SerializedProperty serializedProperty, string trueText, GUIStyle trueStyle, string falseText, GUIStyle falseStyle)
        {
            serializedProperty.boolValue = serializedProperty.boolValue ? GUILayout.Toggle(serializedProperty.boolValue, trueText, trueStyle) : GUILayout.Toggle(serializedProperty.boolValue, falseText, falseStyle);
        }
        /// <summary>
        /// Make an on/off toggle button.
        /// </summary>
        /// <param name="serializedProperty">The SerializedProperty to make a toggle for.</param>
        /// <param name="trueStyle">The style to use when true.</param>
        /// <param name="falseStyle">The style to use when false.</param>
        /// <param name="width">Set the button's width.</param>
        /// <param name="height">Set the button's height.</param>
        public static void Toggle(SerializedProperty serializedProperty, GUIStyle trueStyle, GUIStyle falseStyle, float width, float height)
        {
            serializedProperty.boolValue = serializedProperty.boolValue ? GUILayout.Toggle(serializedProperty.boolValue, GUIContent.none, trueStyle, GUILayout.Width(width), GUILayout.Height(height)) : GUILayout.Toggle(serializedProperty.boolValue, GUIContent.none, falseStyle, GUILayout.Width(width), GUILayout.Height(height));
        }
        /// <summary>
        /// Make an on/off toggle button.
        /// </summary>
        /// <param name="serializedProperty">The SerializedProperty to make a toggle for.</param>
        /// <param name="trueStyle">The style to use when true.</param>
        /// <param name="falseStyle">The style to use when false.</param>
        public static void Toggle(SerializedProperty serializedProperty, GUIStyle trueStyle, GUIStyle falseStyle)
        {
            serializedProperty.boolValue = serializedProperty.boolValue ? GUILayout.Toggle(serializedProperty.boolValue, GUIContent.none, trueStyle) : GUILayout.Toggle(serializedProperty.boolValue, GUIContent.none, falseStyle);
        }
        /// <summary>
        /// Make an on/off toggle button.
        /// </summary>
        /// <param name="serializedProperty">The SerializedProperty to make a toggle for.</param>
        /// <param name="text">The text label to use.</param>
        /// <param name="style">The style to use.</param>
        /// <param name="width">Set the button's width.</param>
        /// <param name="height">Set the button's height.</param>
        public static void Toggle(SerializedProperty serializedProperty, string text, GUIStyle style, float width, float height)
        {
            serializedProperty.boolValue = GUILayout.Toggle(serializedProperty.boolValue, text, style, GUILayout.Width(width), GUILayout.Height(height));
        }
        /// <summary>
        /// Make an on/off toggle button.
        /// </summary>
        /// <param name="serializedProperty">The SerializedProperty to make a toggle for.</param>
        /// <param name="text">The text label to use.</param>
        /// <param name="style">The style to use.</param>
        public static void Toggle(SerializedProperty serializedProperty, string text, GUIStyle style)
        {
            serializedProperty.boolValue = GUILayout.Toggle(serializedProperty.boolValue, text, style);
        }
        /// <summary>
        /// Make an on/off toggle button.
        /// </summary>
        /// <param name="serializedProperty">The SerializedProperty to make a toggle for.</param>
        /// <param name="style">The style to use.</param>
        /// <param name="width">Set the button's width.</param>
        /// <param name="height">Set the button's height.</param>
        public static void Toggle(SerializedProperty serializedProperty, GUIStyle style, float width, float height)
        {
            serializedProperty.boolValue = GUILayout.Toggle(serializedProperty.boolValue, GUIContent.none, style, GUILayout.Width(width), GUILayout.Height(height));
        }
        /// <summary>
        /// Make an on/off toggle button.
        /// </summary>
        /// <param name="serializedProperty">The SerializedProperty to make a toggle for.</param>
        /// <param name="style">The style to use.</param>
        public static void Toggle(SerializedProperty serializedProperty, GUIStyle style)
        {
            serializedProperty.boolValue = GUILayout.Toggle(serializedProperty.boolValue, GUIContent.none, style, GUILayout.Width(12));
        }
        /// <summary>
        /// Make an on/off toggle button. This is a shhorthand method for the native GUILayout.Toggle method.
        /// </summary>
        /// <param name="serializedProperty">The SerializedProperty to make a toggle for.</param>
        /// <param name="text">The text label to use.</param>
        public static void Toggle(SerializedProperty serializedProperty, string text)
        {
            serializedProperty.boolValue = GUILayout.Toggle(serializedProperty.boolValue, text);
        }
        /// <summary>
        /// Make an on/off toggle button. This is a shhorthand method for the native GUILayout.Toggle method. width = 12
        /// </summary>
        /// <param name="serializedProperty">The SerializedProperty to make a toggle for.</param>
        public static void Toggle(SerializedProperty serializedProperty)
        {
            serializedProperty.boolValue = GUILayout.Toggle(serializedProperty.boolValue, GUIContent.none, GUILayout.Width(12));
        }
        #endregion

        #region Popup
        /// <summary>
        /// Make a generic popup selection field.
        /// Takes the currently selected index as a parameter and returns the index selected
        /// by the user.Create a primitive depending on the option selected.
        /// </summary>
        /// <param name="serializedProperty">SerializedProperty of an enum variable that you want to show and change.</param>
        public static void Popup(SerializedProperty serializedProperty)
        {
            serializedProperty.enumValueIndex = EditorGUILayout.Popup(serializedProperty.enumValueIndex, serializedProperty.enumDisplayNames);
        }
        /// <summary>
        /// Make a generic popup selection field.
        /// Takes the currently selected index as a parameter and returns the index selected
        /// by the user.Create a primitive depending on the option selected.
        /// </summary>
        /// <param name="serializedProperty">SerializedProperty of an enum variable that you want to show and change.</param>
        /// <param name="width">Set the popup's width.</param>
        public static void Popup(SerializedProperty serializedProperty, float width)
        {
            serializedProperty.enumValueIndex = EditorGUILayout.Popup(serializedProperty.enumValueIndex, serializedProperty.enumDisplayNames, GUILayout.Width(width));
        }
        /// <summary>
        /// Make a generic popup selection field.
        /// Takes the currently selected index as a parameter and returns the index selected
        /// by the user.Create a primitive depending on the option selected.
        /// </summary>
        /// <param name="selectedIndex">The current selectedIndex.</param>
        /// <param name="displayedOptions">The popup's selection options.</param>
        /// <returns>The new selectedIndex value.</returns>
        public static int Popup(int selectedIndex, string[] displayedOptions)
        {
            return EditorGUILayout.Popup(selectedIndex, displayedOptions);
        }
        /// <summary>
        /// Make a generic popup selection field.
        /// Takes the currently selected index as a parameter and returns the index selected
        /// by the user.Create a primitive depending on the option selected.
        /// </summary>
        /// <param name="selectedIndex">The current selectedIndex.</param>
        /// <param name="displayedOptions">The popup's selection options.</param>
        /// <param name="width">Set the popup's width.</param>
        /// <returns>The new selectedIndex value.</returns>
        public static int Popup(int selectedIndex, string[] displayedOptions, float width)
        {
            return EditorGUILayout.Popup(selectedIndex, displayedOptions, GUILayout.Width(width));
        }
        #endregion

        #region Label
        /// <summary>
        /// Make a label field. (Useful for showing read-only info.)
        /// </summary>
        /// <param name="label">Label in front of the label field.</param>
        public static void Label(GUIContent label)
        {
            EditorGUILayout.LabelField(label);
        }

        /// <summary>
        /// Make a label field. (Useful for showing read-only info.)
        /// </summary>
        /// <param name="label">Label in front of the label field.</param>
        /// <param name="width">The label's width.</param>
        public static void Label(GUIContent label, float width)
        {
            EditorGUILayout.LabelField(label, GUILayout.Width(width));
        }

        /// <summary>
        /// Make a label field. (Useful for showing read-only info.)
        /// </summary>
        /// <param name="label">Label in front of the label field.</param>
        /// <param name="width">The label's width.</param>
        /// <param name="height">The label's height.</param>
        public static void Label(GUIContent label, float width, float height)
        {
            EditorGUILayout.LabelField(label, GUILayout.Width(width), GUILayout.Height(height));
        }

        /// <summary>
        /// Make a label field. (Useful for showing read-only info.)
        /// </summary>
        /// <param name="label">Label in front of the label field.</param>
        /// <param name="style">Set a custom style for the label.</param>
        public static void Label(GUIContent label, GUIStyle style)
        {
            EditorGUILayout.LabelField(label, style);
        }

        /// <summary>
        /// Make a label field. (Useful for showing read-only info.)
        /// </summary>
        /// <param name="label">Label in front of the label field.</param>
        /// <param name="style">Set a custom style for the label.</param>
        /// <param name="width">The label's width.</param>
        public static void Label(GUIContent label, GUIStyle style, float width)
        {
            EditorGUILayout.LabelField(label, style, GUILayout.Width(width));
        }

        /// <summary>
        /// Make a label field. (Useful for showing read-only info.)
        /// </summary>
        /// <param name="label">Label in front of the label field.</param>
        /// <param name="style">Set a custom style for the label.</param>
        /// <param name="width">The label's width.</param>
        /// <param name="height">The label's height.</param>
        public static void Label(GUIContent label, GUIStyle style, float width, float height)
        {
            EditorGUILayout.LabelField(label, style, GUILayout.Width(width), GUILayout.Height(height));
        }

        /// <summary>
        /// Make a label field. (Useful for showing read-only info.)
        /// </summary>
        /// <param name="label">Label in front of the label field.</param>
        public static void Label(string label)
        {
            EditorGUILayout.LabelField(label);
        }

        /// <summary>
        /// Make a label field. (Useful for showing read-only info.)
        /// </summary>
        /// <param name="label">Label in front of the label field.</param>
        /// <param name="width">The label's width.</param>
        public static void Label(string label, float width)
        {
            EditorGUILayout.LabelField(label, GUILayout.Width(width));
        }

        /// <summary>
        /// Make a label field. (Useful for showing read-only info.)
        /// </summary>
        /// <param name="label">Label in front of the label field.</param>
        /// <param name="width">The label's width.</param>
        /// <param name="height">The label's height.</param>
        public static void Label(string label, float width, float height)
        {
            EditorGUILayout.LabelField(label, GUILayout.Width(width), GUILayout.Height(height));
        }

        /// <summary>
        /// Make a label field. (Useful for showing read-only info.)
        /// </summary>
        /// <param name="label">Label in front of the label field.</param>
        /// <param name="tooltip">The tooltip associtated with this label.</param>
        public static void Label(string label, string tooltip)
        {
            EditorGUILayout.LabelField(new GUIContent { text = label, tooltip = tooltip });
        }

        /// <summary>
        /// Make a label field. (Useful for showing read-only info.)
        /// </summary>
        /// <param name="label">Label in front of the label field.</param>
        /// <param name="tooltip">The tooltip associtated with this label.</param>
        /// <param name="width">The label's width.</param>
        public static void Label(string label, string tooltip, float width)
        {
            EditorGUILayout.LabelField(new GUIContent { text = label, tooltip = tooltip }, GUILayout.Width(width));
        }

        /// <summary>
        /// Make a label field. (Useful for showing read-only info.)
        /// </summary>
        /// <param name="label">Label in front of the label field.</param>
        /// <param name="tooltip">The tooltip associtated with this label.</param>
        /// <param name="width">The label's width.</param>
        /// <param name="height">The label's height.</param>
        public static void Label(string label, string tooltip, float width, float height)
        {
            EditorGUILayout.LabelField(new GUIContent { text = label, tooltip = tooltip }, GUILayout.Width(width), GUILayout.Height(height));
        }

        /// <summary>
        /// Make a label field. (Useful for showing read-only info.)
        /// </summary>
        /// <param name="label">Label in front of the label field.</param>
        /// <param name="style">Set a custom style for the label.</param>        
        public static void Label(string label, GUIStyle style)
        {
            EditorGUILayout.LabelField(label, style);
        }

        /// <summary>
        /// Make a label field. (Useful for showing read-only info.)
        /// </summary>
        /// <param name="label">Label in front of the label field.</param>
        /// <param name="style">Set a custom style for the label.</param>
        /// <param name="width">The label's width.</param>
        public static void Label(string label, GUIStyle style, float width)
        {
            EditorGUILayout.LabelField(label, style, GUILayout.Width(width));
        }

        /// <summary>
        /// Make a label field. (Useful for showing read-only info.)
        /// </summary>
        /// <param name="label">Label in front of the label field.</param>
        /// <param name="style">Set a custom style for the label.</param>
        /// <param name="width">The label's width.</param>
        /// <param name="height">The lable's height.</param>
        public static void Label(string label, GUIStyle style, float width, float height)
        {
            EditorGUILayout.LabelField(label, style, GUILayout.Width(width), GUILayout.Height(height));
        }

        /// <summary>
        /// Make a label field. (Useful for showing read-only info.)
        /// </summary>
        /// <param name="label">Label in front of the label field.</param>
        /// <param name="tooltip">The tooltip associtated with this label.</param>
        /// <param name="style">Set a custom style for the label.</param>
        public static void Label(string label, string tooltip, GUIStyle style)
        {
            EditorGUILayout.LabelField(new GUIContent { text = label, tooltip = tooltip }, style);
        }

        /// <summary>
        /// Make a label field. (Useful for showing read-only info.)
        /// </summary>
        /// <param name="label">Label in front of the label field.</param>
        /// <param name="tooltip">The tooltip associtated with this label.</param>
        /// <param name="style">Set a custom style for the label.</param>
        /// <param name="width">The label's width.</param>
        public static void Label(string label, string tooltip, GUIStyle style, float width)
        {
            EditorGUILayout.LabelField(new GUIContent { text = label, tooltip = tooltip }, style, GUILayout.Width(width));
        }

        /// <summary>
        /// Make a label field. (Useful for showing read-only info.)
        /// </summary>
        /// <param name="label">Label in front of the label field.</param>
        /// <param name="tooltip">The tooltip associtated with this label.</param>
        /// <param name="style">Set a custom style for the label.</param>
        /// <param name="width">The label's width.</param>
        /// <param name="height">The label's height.</param>
        public static void Label(string label, string tooltip, GUIStyle style, float width, float height)
        {
            EditorGUILayout.LabelField(new GUIContent { text = label, tooltip = tooltip }, style, GUILayout.Width(width), GUILayout.Height(height));
        }

        /// <summary>
        /// Make a label field. (Useful for showing read-only info.)
        /// </summary>
        /// <param name="texture">The icon image contained.</param>
        public static void Label(Texture texture)
        {
            EditorGUILayout.LabelField(new GUIContent { image = texture });
        }

        /// <summary>
        /// Make a label field. (Useful for showing read-only info.)
        /// </summary>
        /// <param name="texture">The icon image contained.</param>
        /// <param name="width">The label's width.</param>
        /// <param name="height">The label's height.</param>
        public static void Label(Texture texture, float width, float height)
        {
            EditorGUILayout.LabelField(new GUIContent { image = texture }, GUILayout.Width(width), GUILayout.Height(height));
        }

        /// <summary>
        /// Make a label field. (Useful for showing read-only info.)
        /// </summary>
        /// <param name="texture">The icon image contained.</param>
        /// <param name="tooltip">The tooltip associtated with this label.</param>
        public static void Label(Texture texture, string tooltip)
        {
            EditorGUILayout.LabelField(new GUIContent { image = texture, tooltip = tooltip });
        }

        /// <summary>
        /// Make a label field. (Useful for showing read-only info.)
        /// </summary>
        /// <param name="texture">The icon image contained.</param>
        /// <param name="tooltip">The tooltip associtated with this label.</param>
        /// <param name="width">The label's width.</param>
        /// <param name="height">The label's height.</param>
        public static void Label(Texture texture, string tooltip, float width, float height)
        {
            EditorGUILayout.LabelField(new GUIContent { image = texture, tooltip = tooltip }, GUILayout.Width(width), GUILayout.Height(height));
        }
        #endregion

        #region PropertyField
        /// <summary>
        /// Make a field for UnityEditor.SerializedProperty.
        /// </summary>
        /// <param name="serializedProperty">The SerializedProperty to make a field for.</param>
        public static void PropertyField(SerializedProperty serializedProperty)
        {
            EditorGUILayout.PropertyField(serializedProperty, GUIContent.none);
        }

        /// <summary>
        /// Make a field for UnityEditor.SerializedProperty.
        /// </summary>
        /// <param name="serializedProperty">The SerializedProperty to make a field for.</param>
        /// <param name="width">The serializedProperty's width</param>
        public static void PropertyField(SerializedProperty serializedProperty, float width)
        {
            EditorGUILayout.PropertyField(serializedProperty, GUIContent.none, GUILayout.Width(width));
        }

        /// <summary>
        /// Make a field for UnityEditor.SerializedProperty.
        /// </summary>
        /// <param name="serializedProperty">The SerializedProperty to make a field for.</param>
        /// <param name="includeChildren">If true the property including children is drawn; otherwise only the control itself (such as only a foldout but nothing below it).</param>
        public static void PropertyField(SerializedProperty serializedProperty, bool includeChildren)
        {
            EditorGUILayout.PropertyField(serializedProperty, GUIContent.none, includeChildren);
        }

        /// <summary>
        /// Make a field for UnityEditor.SerializedProperty.
        /// </summary>
        /// <param name="serializedProperty">The SerializedProperty to make a field for.</param>
        /// <param name="includeChildren">If true the property including children is drawn; otherwise only the control itself (such as only a foldout but nothing below it).</param>
        /// <param name="width">The serializedProperty's width</param>
        public static void PropertyField(SerializedProperty serializedProperty, bool includeChildren, float width)
        {
            EditorGUILayout.PropertyField(serializedProperty, GUIContent.none, includeChildren, GUILayout.Width(width));
        }

        /// <summary>
        /// Make a field for UnityEditor.SerializedProperty.
        /// </summary>
        /// <param name="serializedProperty">The SerializedProperty to make a field for.</param>
        /// <param name="text">Useful for UnityEvents as it will write the event's name in the event's bar</param>
        public static void PropertyField(SerializedProperty serializedProperty, string text)
        {
            EditorGUILayout.PropertyField(serializedProperty, new GUIContent { text = text });
        }

        /// <summary>
        /// Make a field for UnityEditor.SerializedProperty.
        /// </summary>
        /// <param name="serializedProperty">The SerializedProperty to make a field for.</param>
        /// <param name="text">Useful for UnityEvents as it will write the event's name in the event's bar</param>
        /// <param name="width">The serializedProperty's width</param>
        public static void PropertyField(SerializedProperty serializedProperty, string text, float width)
        {
            EditorGUILayout.PropertyField(serializedProperty, new GUIContent { text = text }, GUILayout.Width(width));
        }

        /// <summary>
        /// Make a field for UnityEditor.SerializedProperty.
        /// </summary>
        /// <param name="serializedProperty">The SerializedProperty to make a field for.</param>
        /// <param name="text">Useful for UnityEvents as it will write the event's name in the event's bar</param>
        /// <param name="includeChildren">If true the property including children is drawn; otherwise only the control itself (such as only a foldout but nothing below it).</param>
        public static void PropertyField(SerializedProperty serializedProperty, string text, bool includeChildren)
        {
            EditorGUILayout.PropertyField(serializedProperty, new GUIContent { text = text }, includeChildren);
        }

        /// <summary>
        /// Make a field for UnityEditor.SerializedProperty.
        /// </summary>
        /// <param name="serializedProperty">The SerializedProperty to make a field for.</param>
        /// <param name="text">Useful for UnityEvents as it will write the event's name in the event's bar</param>
        /// <param name="includeChildren">If true the property including children is drawn; otherwise only the control itself (such as only a foldout but nothing below it).</param>
        /// <param name="width">The serializedProperty's width</param>
        public static void PropertyField(SerializedProperty serializedProperty, string text, bool includeChildren, float width)
        {
            EditorGUILayout.PropertyField(serializedProperty, new GUIContent { text = text }, includeChildren, GUILayout.Width(width));
        }
        #endregion

        #region ObjectField
        public static Object ObjectField(Object obj, System.Type objType, bool allowSceneObjects)
        {
            return EditorGUILayout.ObjectField(obj, objType, allowSceneObjects);
        }

        public static Object ObjectField(Object obj, System.Type objType, bool allowSceneObjects, float width)
        {
            return EditorGUILayout.ObjectField(obj, objType, allowSceneObjects, GUILayout.Width(width));
        }
        #endregion

        #region Vector3
        /// <summary>
        /// Make an X, Y & Z field for entering a UnityEngine.Vector3.
        /// </summary>
        /// <param name="value">The value to edit.</param>
        /// <returns>The value entered by the user.</returns>
        public static Vector3 Vector3(Vector3 value)
        {
            return EditorGUILayout.Vector3Field(GUIContent.none, value);
        }

        /// <summary>
        /// Make an X, Y & Z field for entering a UnityEngine.Vector3.
        /// </summary>
        /// <param name="value">The value to edit.</param>
        /// <param name="width">The field's width.</param>
        /// <returns>The value entered by the user.</returns>
        public static Vector3 Vector3(Vector3 value, float width)
        {
            return EditorGUILayout.Vector3Field(GUIContent.none, value, GUILayout.Width(width));
        }
        #endregion

        #region ColorField
        /// <summary>
        /// Make a field for selecting a UnityEngine.Color.
        /// </summary>
        /// <param name="value">The color to edit.</param>
        /// <returns>The color selected by the user.</returns>
        public static Color ColorField(Color value)
        {
            return EditorGUILayout.ColorField(value);
        }

        /// <summary>
        /// Make a field for selecting a UnityEngine.Color.
        /// </summary>
        /// <param name="value">The color to edit.</param>
        /// <param name="width">The field's width.</param>
        /// <returns>The color selected by the user.</returns>
        public static Color ColorField(Color value, float width)
        {
            return EditorGUILayout.ColorField(value, GUILayout.Width(width));
        }

        /// <summary>
        /// Make a field for selecting a UnityEngine.Color.
        /// </summary>
        /// <param name="value">The color to edit.</param>
        /// <param name="width">The field's width.</param>
        /// <param name="height">The field's height.</param>
        /// <returns>The color selected by the user.</returns>
        public static Color ColorField(Color value, float width, float height)
        {
            return EditorGUILayout.ColorField(value, GUILayout.Width(width), GUILayout.Height(height));
        }

        /// <summary>
        /// Make a field for selecting a UnityEngine.Color.
        /// </summary>
        /// <param name="value">The color to edit.</param>
        /// <param name="showEyedropper">If true, the color picker should show the eyedropper control. If false, don't show it.</param>
        /// <param name="showAlpha">If true, allow the user to set an alpha value for the color. If false, hide the alpha component.</param>
        /// <param name="hdr">If true, treat the color as an HDR value. If false, treat it as a standard LDR value.</param>
        /// <param name="hdrConfig">An object that sets the presentation parameters for an HDR color. If not using an HDR color, set this to null.</param>
        /// <returns>The color selected by the user.</returns>
        public static Color ColorField(Color value, bool showEyedropper, bool showAlpha, bool hdr, ColorPickerHDRConfig hdrConfig)
        {
            return EditorGUILayout.ColorField(GUIContent.none, value, showEyedropper, showAlpha, hdr, hdrConfig);
        }

        /// <summary>
        /// Make a field for selecting a UnityEngine.Color.
        /// </summary>
        /// <param name="value">The color to edit.</param>
        /// <param name="showEyedropper"></param>
        /// <param name="showAlpha"></param>
        /// <param name="hdr"></param>
        /// <param name="hdrConfig"></param>
        /// <param name="width">The field's width.</param>
        /// <returns>The color selected by the user.</returns>
        public static Color ColorField(Color value, bool showEyedropper, bool showAlpha, bool hdr, ColorPickerHDRConfig hdrConfig, float width)
        {
            return EditorGUILayout.ColorField(GUIContent.none, value, showEyedropper, showAlpha, hdr, hdrConfig, GUILayout.Width(width));
        }

        /// <summary>
        /// Make a field for selecting a UnityEngine.Color.
        /// </summary>
        /// <param name="value">The color to edit.</param>
        /// <param name="showEyedropper"></param>
        /// <param name="showAlpha"></param>
        /// <param name="hdr"></param>
        /// <param name="hdrConfig"></param>
        /// <param name="width">The field's width.</param>
        /// <param name="height">The field's height.</param>
        /// <returns>The color selected by the user.</returns>
        public static Color ColorField(Color value, bool showEyedropper, bool showAlpha, bool hdr, ColorPickerHDRConfig hdrConfig, float width, float height)
        {
            return EditorGUILayout.ColorField(GUIContent.none, value, showEyedropper, showAlpha, hdr, hdrConfig, GUILayout.Width(width), GUILayout.Height(height));
        }
        #endregion

        #region TextField
        /// <summary>
        /// Make a text field.
        /// </summary>
        /// <param name="text">The text to edit.</param>
        /// <returns>The edited text value.</returns>
        public static string TextField(string text)
        {
            return EditorGUILayout.TextField(text);
        }
        /// <summary>
        /// Make a text field.
        /// </summary>
        /// <param name="text">The text to edit.</param>
        /// <param name="width">The field's width.</param>
        /// <returns>The edited text value.</returns>
        public static string TextField(string text, float width)
        {
            return EditorGUILayout.TextField(text, GUILayout.Width(width));
        }
        /// <summary>
        /// Make a text field.
        /// </summary>
        /// <param name="text">The text to edit.</param>
        /// <param name="width">The field's width.</param>
        /// <param name="height">The field's height.</param>
        /// <returns>The edited text value.</returns>
        public static string TextField(string text, float width, float height)
        {
            return EditorGUILayout.TextField(text, GUILayout.Width(width), GUILayout.Height(height));
        }
        /// <summary>
        /// Make a text field.
        /// </summary>
        /// <param name="label">Adds a label in front of the text field.</param>
        /// <param name="text">The text to edit.</param>
        /// <returns>The edited text value.</returns>
        public static string TextField(GUIContent label, string text)
        {
            return EditorGUILayout.TextField(label, text);
        }
        /// <summary>
        /// Make a text field.
        /// </summary>
        /// <param name="label">Adds a label in front of the text field.</param>
        /// <param name="text">The text to edit.</param>
        /// <param name="width">The field's width.</param>
        /// <returns>The edited text value.</returns>
        public static string TextField(GUIContent label, string text, float width)
        {
            return EditorGUILayout.TextField(label, text, GUILayout.Width(width));
        }
        /// <summary>
        /// Make a text field.
        /// </summary>
        /// <param name="label">Adds a label in front of the text field.</param>
        /// <param name="text">The text to edit.</param>
        /// <param name="width">The field's width.</param>
        /// <param name="height">The field's height.</param>
        /// <returns>The edited text value.</returns>
        public static string TextField(GUIContent label, string text, float width, float height)
        {
            return EditorGUILayout.TextField(label, text, GUILayout.Width(width), GUILayout.Height(height));
        }
        #endregion

        #region InfoMessage / DrawInfoMessage
        /// <summary>
        /// Stores an AnimBool (used for show/hide animation), a title (optional), and a message.
        /// </summary>
        public class InfoMessage
        {
            /// <summary>
            /// Used to toggle show/hide of the message via an editor animation
            /// </summary>
            public AnimBool show = new AnimBool(false);
            /// <summary>
            /// (optional) The title will appear as a bold text when displyed through DrawInfoMessage
            /// </summary>
            public string title = string.Empty;
            /// <summary>
            /// The main text of the InfoMessage. This will get displayed with or without a title
            /// </summary>
            public string message = string.Empty;
        }
        /// <summary>
        /// Enum used by the DrawInfoMessage in order to show an icon and set the proper style for each InfoMessage type.
        /// </summary>
        public enum InfoMessageType
        {
            /// <summary>
            /// Uses the TextStyle.Help style.
            /// </summary>
            Help,
            /// <summary>
            /// Uses the TextStyle.Info style.
            /// </summary>
            Info,
            /// <summary>
            /// Uses the TextStyle.Warning style.
            /// </summary>
            Warning,
            /// <summary>
            /// Uses the TextStyle.Error style.
            /// </summary>
            Error
        }
        /// <summary>
        /// Draws an InfoMessage box with the specified InfoMessage settings, with the set width and of the set InfoMessageType.
        /// </summary>
        /// <param name="im">Contains an AnimBool that manages the show/hide animation, a title (optional), and a message.</param>
        /// <param name="width">The width of this box. The height is determined automatically by the amount of text contained in the InfoMessage.</param>
        /// <param name="type">Depending on the type,it will draw a HelpBox, an InfoBox, a WarningBox or an ErrorBox. Each box has it's own style and icon.</param>
        public static void DrawInfoMessage(InfoMessage im, float width, InfoMessageType type)
        {
            string message = (string.IsNullOrEmpty(im.title) ? "" : "<b>" + im.title + "</b> - ") + im.message;
            switch(type)
            {
                case InfoMessageType.Help: DrawInfoMessage(im.show, "\uf059 " + message, EStyles.GetStyle(EStyles.TextStyle.Help), width); break;
                case InfoMessageType.Info: DrawInfoMessage(im.show, "\uf05a " + message, EStyles.GetStyle(EStyles.TextStyle.Info), width); break;
                case InfoMessageType.Warning: DrawInfoMessage(im.show, "\uf071 " + message, EStyles.GetStyle(EStyles.TextStyle.Warning), width); break;
                case InfoMessageType.Error: DrawInfoMessage(im.show, "\uf057 " + message, EStyles.GetStyle(EStyles.TextStyle.Error), width); break;
            }
        }
        /// <summary>
        /// Draws an InfoMessage box with the specified InfoMessage settings, with the set width and of the set InfoMessageType.
        /// </summary>
        /// <param name="im">Contains an AnimBool that manages the show/hide animation, a title (optional), and a message.</param>
        /// <param name="type">Depending on the type,it will draw a HelpBox, an InfoBox, a WarningBox or an ErrorBox. Each box has it's own style and icon.</param>
        /// <param name="width">The width of this box. The height is determined automatically by the amount of text contained in the InfoMessage.</param>
        public static void DrawInfoMessage(InfoMessage im, InfoMessageType type, float width)
        {
            string message = (string.IsNullOrEmpty(im.title) ? "" : "<b>" + im.title + "</b> - ") + im.message;
            switch(type)
            {
                case InfoMessageType.Help: DrawInfoMessage(im.show, "\uf059 " + message, EStyles.GetStyle(EStyles.TextStyle.Help), width); break;
                case InfoMessageType.Info: DrawInfoMessage(im.show, "\uf05a " + message, EStyles.GetStyle(EStyles.TextStyle.Info), width); break;
                case InfoMessageType.Warning: DrawInfoMessage(im.show, "\uf071 " + message, EStyles.GetStyle(EStyles.TextStyle.Warning), width); break;
                case InfoMessageType.Error: DrawInfoMessage(im.show, "\uf057 " + message, EStyles.GetStyle(EStyles.TextStyle.Error), width); break;
            }
        }
        /// <summary>
        /// This does the acctual drawing of the InfoMessage Box. The DrawInfoMessage method only does the initial setup for this.
        /// </summary>
        private static void DrawInfoMessage(AnimBool show, string message, GUIStyle style, float width)
        {
            if(!show.value) { return; }
            if(EditorGUILayout.BeginFadeGroup(show.faded))
            {
                EditorGUILayout.LabelField(message, style, GUILayout.Width(width));
            }
            EditorGUILayout.EndFadeGroup();
        }
        #endregion
    }
}
