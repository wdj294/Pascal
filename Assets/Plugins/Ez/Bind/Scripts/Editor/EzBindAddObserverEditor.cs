// Copyright (c) 2016 - 2017 Ez Entertainment SRL. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Ez.Binding.Internal;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;

namespace Ez.Binding
{
    [CustomEditor(typeof(EzBindAddObserver))]
    public class EzBindAddObserverEditor : EBaseEditor
    {
        EzBindAddObserver ezBindAddObserver { get { return (EzBindAddObserver)target; } }
        private bool markSceneDirty = false;
        private string bindName = "";
        private BoundItemInfo sourceInfo;
        private AnimBool show;
        private GUIStyle ComponentNormalCenterStyle;
        private GUIStyle ComponentSmallWhiteTextStyle;
        private GUIStyle LeftButtonPurpleWhiteTextStyle;

        private SerializedProperty updateWhenDisabled;

        void GetStyles()
        {
            ComponentNormalCenterStyle = EStyles.GetStyle(EStyles.TextStyle.ComponentNormal).Copy();
            ComponentNormalCenterStyle.alignment = TextAnchor.MiddleCenter;

            ComponentSmallWhiteTextStyle = EStyles.GetStyle(EStyles.TextStyle.ComponentSmall).Copy();
            ComponentSmallWhiteTextStyle.normal.textColor = EColor.WhiteLight;

            LeftButtonPurpleWhiteTextStyle = EStyles.GetStyle(EStyles.ButtonStyle.LeftButtonPurple).Copy();
            LeftButtonPurpleWhiteTextStyle.normal.textColor = EColor.WhiteLight;
            LeftButtonPurpleWhiteTextStyle.onNormal.textColor = EColor.WhiteLight;
            LeftButtonPurpleWhiteTextStyle.hover.textColor = EColor.WhiteLight;
            LeftButtonPurpleWhiteTextStyle.onHover.textColor = EColor.WhiteLight;
            LeftButtonPurpleWhiteTextStyle.active.textColor = EColor.WhiteDark;
            LeftButtonPurpleWhiteTextStyle.onActive.textColor = EColor.WhiteDark;
        }

        void InitializeBindInfo()
        {
            sourceInfo = new BoundItemInfo();
            sourceInfo.Initialize();
            show = new AnimBool(string.IsNullOrEmpty(bindName), Repaint);
        }

        void OnEnable()
        {
            requiresContantRepaint = true;
            InitializeBindInfo();
            GetStyles();
            updateWhenDisabled = serializedObject.FindProperty("updateWhenDisabled");
        }

        public override void OnInspectorGUI()
        {
            DrawHeader(EResources.HeaderBarBindAddObserver.normal);
            EGUI.Space(SPACE_4);
            if(EditorApplication.isPlaying)
            {
                if(EGUI.Button("\uf112  Select EzBind", EStyles.GetStyle(EStyles.ButtonStyle.ButtonPurple), WIDTH_420, HEIGHT_24))
                {
                    Selection.activeGameObject = FindObjectOfType<EzBind>().gameObject;
                }
                EGUI.Label("Select the EzBind gameObject\nto see all the Binds and their values", ComponentNormalCenterStyle, WIDTH_420);
                return;
            }
            serializedObject.Update();
            DrawEditor();
            EzBindEditor.UpdateBoundItem(ezBindAddObserver.observer, sourceInfo);
            EGUI.Space(SPACE_8);
            serializedObject.ApplyModifiedProperties();
            if(markSceneDirty) { EGUI.MarkSceneDirty(); markSceneDirty = false; }
        }

        void DrawEditor()
        {
            UpdateData();
            EGUI.Space(SPACE_4);
            EGUI.BeginHorizontal(WIDTH_420);
            {
                EGUI.Label("Bind Name", EStyles.GetStyle(EStyles.TextStyle.ComponentNormal), 80, 18);
                EGUI.Space(-8);
                EGUI.BeginChangeCheck();
                bindName = EGUI.TextField(bindName, WIDTH_420 - 80 - 2);
                if(EGUI.EndChangeCheck())
                {
                    ezBindAddObserver.bindName = bindName;
                    markSceneDirty = true;
                }
            }
            EGUI.EndHorizontal();
            EGUI.Space(1);
            EGUI.BeginHorizontal(WIDTH_420);
            {
                string buttonName = "  " + (string.IsNullOrEmpty(bindName) ? "\uf06a  Unnamed Bind" : ((show.target ? "\uf078  " : "\uf054  ") + bindName));
                GUIStyle buttonStyle = string.IsNullOrEmpty(bindName) ? EStyles.GetStyle(EStyles.ButtonStyle.LeftButtonRed) : LeftButtonPurpleWhiteTextStyle;
                if(EGUI.Button(buttonName, buttonStyle, WIDTH_420, 20))
                {
                    show.target = string.IsNullOrEmpty(bindName) ? false : !show.target;
                }
            }
            EGUI.EndHorizontal();
            if(!string.IsNullOrEmpty(bindName))
            {
                EGUI.Space(-SPACE_16 - 4);
                EGUI.BeginHorizontal(WIDTH_420);
                {
                    EGUI.FlexibleSpace();
                    EGUI.Label("Observer: " + (sourceInfo.gameObject != null ? "Added" : "None"), ComponentSmallWhiteTextStyle);
                    EGUI.Space(6);
                }
                EGUI.EndHorizontal();
            }

            if(EGUI.BeginFadeGroup(show.faded) && !string.IsNullOrEmpty(bindName))
            {
                EGUI.Space(SPACE_8);
                EGUI.Space(1);
                EGUI.BeginHorizontal(WIDTH_420, 18);
                {
                    EGUI.Space(SPACE_16);
                    EGUI.DrawTexture(ezBindAddObserver.observer.gameObject == null ? EResources.RedBackground : EResources.PurpleBackground, WIDTH_420 - SPACE_16 - 18, 18);
                    EGUI.Space(36);
                }
                EGUI.EndHorizontal();
                EGUI.Space(-19);
                EGUI.BeginHorizontal(WIDTH_420);
                {
                    EGUI.Space(SPACE_16);
                    EGUI.Label(" Observer", EStyles.GetStyle(EStyles.TextStyle.ComponentNormal), WIDTH_105);
                    sourceInfo.gameObject = ezBindAddObserver.gameObject;
                    GUI.enabled = false;
                    sourceInfo.gameObject = (GameObject)EGUI.ObjectField(sourceInfo.gameObject, typeof(GameObject), true, 295 - 24);
                    GUI.enabled = true;
                    if(EGUI.ButtonReset())
                    {
                        sourceInfo.ResetComponent();
                        sourceInfo.ResetVariableName();
                        ezBindAddObserver.observer.Reset();
                        ezBindAddObserver.observer.gameObject = ezBindAddObserver.gameObject;
                        markSceneDirty = true;
                    }
                }
                EGUI.EndHorizontal();
                EGUI.Space(1);
                EGUI.BeginHorizontal(WIDTH_420, 18);
                {
                    EGUI.Space(SPACE_16);
                    EGUI.DrawTexture(ezBindAddObserver.observer.component == null ? EResources.RedBackground : EResources.PurpleBackground, WIDTH_420 - SPACE_16, 18);
                    EGUI.Space(36);
                }
                EGUI.EndHorizontal();
                EGUI.Space(-19);
                EGUI.BeginHorizontal(WIDTH_420);
                {
                    EGUI.Space(SPACE_16);
                    EGUI.Label(" Component", EStyles.GetStyle(EStyles.TextStyle.ComponentNormal), WIDTH_105);
                    if(sourceInfo.gameObject == null)
                    {
                        EGUI.Space(8);
                        EGUI.Label("Reference a GameObject", EStyles.GetStyle(EStyles.TextStyle.ComponentSmall), 295);
                        EGUI.FlexibleSpace();
                    }
                    else
                    {
                        EGUI.BeginChangeCheck();
                        int tempComponentIndex = sourceInfo.GetComponentIndex();
                        tempComponentIndex = EGUI.Popup(tempComponentIndex, sourceInfo.componentsNamesArray == null ? sourceInfo.GetComponentsNamesArray() : sourceInfo.componentsNamesArray, 295);
                        if(EGUI.EndChangeCheck())
                        {
                            sourceInfo.componentIndex = tempComponentIndex;
                            sourceInfo.UpdateComponent(sourceInfo.componentIndex);
                            sourceInfo.InitializeVariableName();
                            markSceneDirty = true;
                        }
                    }
                }
                EGUI.EndHorizontal();
                EGUI.Space(1);
                EGUI.BeginHorizontal(WIDTH_420, 18);
                {
                    EGUI.Space(SPACE_16);
                    EGUI.DrawTexture((string.IsNullOrEmpty(ezBindAddObserver.observer.variableName) || ezBindAddObserver.observer.variableName.Equals("None")) ? EResources.RedBackground : EResources.PurpleBackground, WIDTH_420 - SPACE_16, 18);
                    EGUI.Space(36);
                }
                EGUI.EndHorizontal();
                EGUI.Space(-19);
                EGUI.BeginHorizontal(WIDTH_420);
                {
                    EGUI.Space(SPACE_16);
                    EGUI.Label(" Variable", EStyles.GetStyle(EStyles.TextStyle.ComponentNormal), WIDTH_105);
                    if(sourceInfo.component == null)
                    {
                        EGUI.Space(8);
                        EGUI.Label("Select a Component", EStyles.GetStyle(EStyles.TextStyle.ComponentSmall), 300);
                        EGUI.FlexibleSpace();
                    }
                    else
                    {
                        EGUI.BeginChangeCheck();
                        int tempVariableNameIndex = sourceInfo.GetVariableNameIndex();
                        tempVariableNameIndex = EGUI.Popup(tempVariableNameIndex, sourceInfo.variableNamesArray == null ? sourceInfo.GetVariableNamesArray() : sourceInfo.variableNamesArray, 295);
                        if(EGUI.EndChangeCheck())
                        {
                            sourceInfo.variableNameIndex = tempVariableNameIndex;
                            sourceInfo.UpdateVariableName(sourceInfo.variableNameIndex);
                            markSceneDirty = true;
                        }
                    }
                }
                EGUI.EndHorizontal();
            }
            EGUI.EndFadeGroup();
            EGUI.Space(SPACE_4, !string.IsNullOrEmpty(bindName));
            EGUI.Space(SPACE_4);
            EGUI.BeginHorizontal(WIDTH_420);
            {
                EGUI.Toggle(updateWhenDisabled);
                EGUI.Label("Update Observer when this GameObject is disabled", WIDTH_420 - 12);
                EGUI.FlexibleSpace();
            }
            EGUI.EndHorizontal();
        }

        void UpdateData()
        {
            bindName = ezBindAddObserver.bindName;
            EzBindEditor.UpdateBoundItemInfo(sourceInfo, ezBindAddObserver.observer);
        }
    }
}
