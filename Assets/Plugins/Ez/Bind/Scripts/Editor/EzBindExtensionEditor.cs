// Copyright (c) 2016 - 2017 Ez Entertainment SRL. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Ez.Binding.Internal;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Ez.Binding
{
    [CustomEditor(typeof(EzBindExtension))]
    public class EzBindExtensionEditor : EBaseEditor
    {
        EzBindExtension ezBindExtension { get { return (EzBindExtension)target; } }

        public List<BindInfo> bindsInfo = new List<BindInfo>();
        private bool initialized = false;
        private bool markSceneDirty = false;

        private GUIStyle ComponentNormalCenterStyle;
        private GUIStyle ComponentSmallWhiteTextStyle;

        void GetStyles()
        {
            ComponentNormalCenterStyle = EStyles.GetStyle(EStyles.TextStyle.ComponentNormal).Copy();
            ComponentNormalCenterStyle.alignment = TextAnchor.MiddleCenter;

            ComponentSmallWhiteTextStyle = EStyles.GetStyle(EStyles.TextStyle.ComponentSmall).Copy();
            ComponentSmallWhiteTextStyle.normal.textColor = EColor.WhiteLight;
        }

        void InitializeBindInfo()
        {
            if(ezBindExtension.bindsData == null) { ezBindExtension.bindsData = new List<BindData>(); }
            if(bindsInfo == null) { bindsInfo = new List<BindInfo>(); }
            if(!initialized || ezBindExtension.bindsData.Count != bindsInfo.Count)
            {
                bindsInfo = new List<BindInfo>();
                for(int i = 0; i < ezBindExtension.bindsData.Count; i++)
                {
                    bindsInfo.Add(new BindInfo(Repaint));
                    bindsInfo[i].GetBindInfo(ezBindExtension.bindsData[i], true);
                }
                initialized = true;
            }
        }

        void OnEnable()
        {
            requiresContantRepaint = true;
            InitializeBindInfo();
            GetStyles();
        }

        public override void OnInspectorGUI()
        {
            DrawHeader(EResources.HeaderBarBindExtension.normal);
            EGUI.Space(SPACE_4);
            if(EditorApplication.isPlaying)
            {
                if(EGUI.Button("\uf112  Select EzBind", EStyles.GetStyle(EStyles.ButtonStyle.ButtonBlue), WIDTH_420, HEIGHT_24))
                {
                    Selection.activeGameObject = FindObjectOfType<EzBind>().gameObject;
                }
                EGUI.Label("Select the EzBind gameObject\nto see all the Binds and their values", ComponentNormalCenterStyle, WIDTH_420);
                return;
            }
            serializedObject.Update();
            DrawBinds();
            EGUI.Space(SPACE_4);
            serializedObject.ApplyModifiedProperties();
            if(markSceneDirty) { EGUI.MarkSceneDirty(); markSceneDirty = false; }
        }

        void DrawBinds()
        {

            if(EGUI.Button("\uf067  Add Bind", EStyles.GetStyle(EStyles.ButtonStyle.ButtonBlue), WIDTH_420, HEIGHT_24))
            {
                AddBind();
                markSceneDirty = true;
            }
            EGUI.Space(SPACE_4);
            if(bindsInfo == null || bindsInfo.Count == 0) { EGUI.Label("No binds were found."); EGUI.Space(SPACE_4); }
            if(bindsInfo == null || bindsInfo.Count == 0) { EGUI.Space(SPACE_4); return; }
            for(int i = 0; i < bindsInfo.Count; i++)
            {
                DrawBind(ezBindExtension.bindsData[i], bindsInfo[i], i);
                EGUI.Space(SPACE_4);
            }
            EGUI.Space(SPACE_4);
        }
        void DrawBind(BindData b, BindInfo bInfo, int index)
        {
            EzBindEditor.UpdateBindInfo(bInfo, b);
            EGUI.Space(SPACE_4);
            EGUI.BeginHorizontal(WIDTH_420);
            {
                EGUI.Label("Bind Name", EStyles.GetStyle(EStyles.TextStyle.ComponentNormal), 80, 18);
                EGUI.Space(-8);
                EGUI.BeginChangeCheck();
                bInfo.bindName = EGUI.TextField(bInfo.bindName, WIDTH_420 - 80 - 92);
                if(EGUI.EndChangeCheck())
                {
                    b.bindName = bInfo.bindName;
                    markSceneDirty = true;
                }
                EGUI.Space(2);
                if(EGUI.Button("\uf00d  Delete Bind", EStyles.GetStyle(EStyles.ButtonStyle.ButtonRed), 90, 20))
                {
                    DeleteBind(b, bInfo);
                    markSceneDirty = true;
                }
            }
            EGUI.EndHorizontal();
            EGUI.Space(1);
            EGUI.BeginHorizontal(WIDTH_420);
            {
                string buttonName = "  " + (string.IsNullOrEmpty(bInfo.bindName) ? "\uf06a  Unnamed Bind" : ((bInfo.show.target ? "\uf078  " : "\uf054  ") + bInfo.bindName));
                GUIStyle buttonStyle = EStyles.GetStyle(string.IsNullOrEmpty(bInfo.bindName) ? EStyles.ButtonStyle.LeftButtonRed : EStyles.ButtonStyle.LeftButtonBlue);
                if(EGUI.Button(buttonName, buttonStyle, WIDTH_420, 20))
                {
                    bInfo.show.target = string.IsNullOrEmpty(bInfo.bindName) ? false : !bInfo.show.target;
                }
            }
            EGUI.EndHorizontal();
            if(!string.IsNullOrEmpty(bInfo.bindName))
            {
                EGUI.Space(-SPACE_16 - 4);
                EGUI.BeginHorizontal(WIDTH_420);
                {
                    EGUI.FlexibleSpace();
                    EGUI.Label("Source: " + (bInfo.HasSource ? "Added" : "None") + " | " + "Observers: " + (bInfo.HasObservers ? bInfo.ObserversCount.ToString() : "None"), ComponentSmallWhiteTextStyle);
                    EGUI.Space(6);
                }
                EGUI.EndHorizontal();
            }

            if(EGUI.BeginFadeGroup(bInfo.show.faded) && !string.IsNullOrEmpty(bInfo.bindName))
            {
                DrawBindInfo(b, bInfo);
                if(!string.IsNullOrEmpty(ezBindExtension.bindsData[index].bindName))
                {
                    EGUI.BeginHorizontal(WIDTH_420);
                    {
                        EGUI.Space(SPACE_16);
                        EGUI.PropertyField(serializedObject.FindProperty("bindsData").GetArrayElementAtIndex(index).FindPropertyRelative("OnValueChanged"), "OnValueChanged", true, WIDTH_420 - SPACE_16);
                    }
                    EGUI.EndHorizontal();
                }
            }
            EGUI.EndFadeGroup();
        }
        void DrawBindInfo(BindData b, BindInfo bInfo)
        {
            EGUI.Space(SPACE_4);
            DrawSource(b, bInfo);
            EGUI.Space(SPACE_4);
            DrawObservers(b, bInfo);
            EzBindEditor.UpdateBind(b, bInfo);
            EGUI.Space(SPACE_4);

        }
        void DrawSource(BindData b, BindInfo bInfo)
        {
            EGUI.Space(SPACE_4);
            EGUI.BeginHorizontal(WIDTH_420);
            {
                EGUI.Space(SPACE_16);
                EGUI.Label("Source", EStyles.GetStyle(EStyles.TextStyle.ComponentNormal), WIDTH_105);
                EGUI.BeginChangeCheck();
                GameObject tempGO = bInfo.source.gameObject;
                tempGO = (GameObject)EGUI.ObjectField(tempGO, typeof(GameObject), true, 295 - 24);
                if(EGUI.EndChangeCheck())
                {
                    bInfo.source.gameObject = tempGO;
                    if(bInfo.source.gameObject == null)
                    {
                        bInfo.source.Reset();
                        EzBindEditor.UpdateBoundItem(b.source, bInfo.source);
                    }
                    else
                    {
                        bInfo.source.InitializeComponent();
                        bInfo.source.InitializeVariableName();
                    }
                    markSceneDirty = true;
                }
                if(EGUI.ButtonReset())
                {
                    bInfo.source.Reset();
                    b.source.Reset();
                    markSceneDirty = true;
                }
            }
            EGUI.EndHorizontal();
            EGUI.BeginHorizontal(WIDTH_420);
            {
                EGUI.Space(SPACE_16);
                EGUI.Label("Component", EStyles.GetStyle(EStyles.TextStyle.ComponentNormal), WIDTH_105);
                if(bInfo.source.gameObject == null)
                {
                    EGUI.Space(8);
                    EGUI.Label("Reference a GameObject", EStyles.GetStyle(EStyles.TextStyle.ComponentSmall), 295);
                    EGUI.FlexibleSpace();
                }
                else
                {
                    EGUI.BeginChangeCheck();
                    int tempComponentIndex = bInfo.source.GetComponentIndex();
                    tempComponentIndex = EGUI.Popup(tempComponentIndex, bInfo.source.componentsNamesArray == null ? bInfo.source.GetComponentsNamesArray() : bInfo.source.componentsNamesArray, 295);
                    if(EGUI.EndChangeCheck())
                    {
                        bInfo.source.componentIndex = tempComponentIndex;
                        bInfo.source.UpdateComponent(bInfo.source.componentIndex);
                        bInfo.source.InitializeVariableName();
                        markSceneDirty = true;
                    }
                }
            }
            EGUI.EndHorizontal();
            EGUI.BeginHorizontal(WIDTH_420);
            {
                EGUI.Space(SPACE_16);
                EGUI.Label("Variable", EStyles.GetStyle(EStyles.TextStyle.ComponentNormal), WIDTH_105);
                if(bInfo.source.component == null)
                {
                    EGUI.Space(8);
                    EGUI.Label("Select a Component", EStyles.GetStyle(EStyles.TextStyle.ComponentSmall), 300);
                    EGUI.FlexibleSpace();
                }
                else
                {
                    EGUI.BeginChangeCheck();
                    int tempVariableNameIndex = bInfo.source.GetVariableNameIndex();
                    tempVariableNameIndex = EGUI.Popup(tempVariableNameIndex, bInfo.source.variableNamesArray == null ? bInfo.source.GetVariableNamesArray() : bInfo.source.variableNamesArray, 295);
                    if(EGUI.EndChangeCheck())
                    {
                        bInfo.source.variableNameIndex = tempVariableNameIndex;
                        bInfo.source.UpdateVariableName(bInfo.source.variableNameIndex);
                        markSceneDirty = true;
                    }
                }
            }
            EGUI.EndHorizontal();
        }
        void DrawObservers(BindData b, BindInfo bInfo)
        {
            EGUI.BeginHorizontal(WIDTH_420);
            {
                EGUI.Space(SPACE_16);
                if(EGUI.Button("\uf067  Add Observer", EStyles.GetStyle(EStyles.ButtonStyle.ButtonBlue), WIDTH_420 - SPACE_16, HEIGHT_16))
                {
                    EzBindEditor.AddObserver(b, bInfo);
                    markSceneDirty = true;
                }
            }
            EGUI.EndHorizontal();
            if(b.observers == null || b.observers.Count == 0) { return; }
            for(int i = 0; i < b.observers.Count; i++)
            {
                EGUI.Space(SPACE_4);
                EGUI.Space(1);
                EGUI.BeginHorizontal(WIDTH_420, 18);
                {
                    EGUI.Space(SPACE_16);
                    EGUI.DrawTexture(bInfo.observers[i].gameObject == null ? EResources.RedBackground : EResources.GreenBackground, WIDTH_420 - SPACE_16 - 36, 18);
                    EGUI.Space(36);
                }
                EGUI.EndHorizontal();
                EGUI.Space(-19);
                EGUI.BeginHorizontal(WIDTH_420);
                {
                    EGUI.Space(SPACE_16);
                    EGUI.Label(" Observer " + i, EStyles.GetStyle(EStyles.TextStyle.ComponentNormal), WIDTH_105);
                    EGUI.BeginChangeCheck();
                    GameObject tempGO = bInfo.observers[i].gameObject;
                    tempGO = (GameObject)EGUI.ObjectField(tempGO, typeof(GameObject), true, 295 - 24 - 20);
                    if(EGUI.EndChangeCheck())
                    {
                        bInfo.observers[i].gameObject = tempGO;
                        if(bInfo.observers[i].gameObject == null)
                        {
                            bInfo.observers[i].Reset();
                            EzBindEditor.UpdateBoundItem(b.observers[i], bInfo.observers[i]);
                        }
                        else
                        {
                            bInfo.observers[i].InitializeComponent();
                            bInfo.observers[i].InitializeVariableName();
                        }
                        markSceneDirty = true;
                    }
                    if(EGUI.ButtonMinus())
                    {
                        bInfo.observers.RemoveAt(i);
                        b.observers.RemoveAt(i);
                        markSceneDirty = true;
                        EGUI.ExitGUI();
                    }
                    EGUI.Space(2);
                    if(EGUI.ButtonReset())
                    {
                        bInfo.observers[i].Reset();
                        b.observers[i].Reset();
                        markSceneDirty = true;
                    }
                }
                EGUI.EndHorizontal();
                EGUI.Space(1);
                EGUI.BeginHorizontal(WIDTH_420, 18);
                {
                    EGUI.Space(SPACE_16);
                    EGUI.DrawTexture(bInfo.observers[i].component == null ? EResources.RedBackground : EResources.GreenBackground, WIDTH_420 - SPACE_16, 18);
                }
                EGUI.EndHorizontal();
                EGUI.Space(-19);
                EGUI.BeginHorizontal(WIDTH_420);
                {
                    EGUI.Space(SPACE_16);
                    EGUI.Label(" Component", EStyles.GetStyle(EStyles.TextStyle.ComponentNormal), WIDTH_105);
                    if(bInfo.observers[i].gameObject == null)
                    {
                        EGUI.Space(8);
                        EGUI.Label("Reference a GameObject", EStyles.GetStyle(EStyles.TextStyle.ComponentSmall), 295);
                        EGUI.FlexibleSpace();
                    }
                    else
                    {
                        EGUI.BeginChangeCheck();
                        int tempComponentIndex = bInfo.observers[i].GetComponentIndex();
                        tempComponentIndex = EGUI.Popup(tempComponentIndex, bInfo.observers[i].componentsNamesArray == null ? bInfo.observers[i].GetComponentsNamesArray() : bInfo.observers[i].componentsNamesArray, 295);
                        if(EGUI.EndChangeCheck())
                        {
                            bInfo.observers[i].componentIndex = tempComponentIndex;
                            bInfo.observers[i].UpdateComponent(bInfo.observers[i].componentIndex);
                            bInfo.observers[i].InitializeVariableName();
                            markSceneDirty = true;
                        }
                    }
                }
                EGUI.EndHorizontal();
                EGUI.Space(1);
                EGUI.BeginHorizontal(WIDTH_420, 18);
                {
                    EGUI.Space(SPACE_16);
                    EGUI.DrawTexture((string.IsNullOrEmpty(bInfo.observers[i].variableName) || bInfo.observers[i].variableName.Equals("None")) ? EResources.RedBackground : EResources.GreenBackground, WIDTH_420 - SPACE_16, 18);
                }
                EGUI.EndHorizontal();
                EGUI.Space(-19);
                EGUI.BeginHorizontal(WIDTH_420);
                {
                    EGUI.Space(SPACE_16);
                    EGUI.Label(" Variable", EStyles.GetStyle(EStyles.TextStyle.ComponentNormal), WIDTH_105);
                    if(bInfo.observers[i].component == null)
                    {
                        EGUI.Space(8);
                        EGUI.Label("Select a Component", EStyles.GetStyle(EStyles.TextStyle.ComponentSmall), 300);
                        EGUI.FlexibleSpace();
                    }
                    else
                    {
                        EGUI.BeginChangeCheck();
                        int tempVariableNameIndex = bInfo.observers[i].GetVariableNameIndex();
                        tempVariableNameIndex = EGUI.Popup(tempVariableNameIndex, bInfo.observers[i].variableNamesArray == null ? bInfo.observers[i].GetVariableNamesArray() : bInfo.observers[i].variableNamesArray, 295);
                        if(EGUI.EndChangeCheck())
                        {
                            bInfo.observers[i].variableNameIndex = tempVariableNameIndex;
                            bInfo.observers[i].UpdateVariableName(bInfo.observers[i].variableNameIndex);
                            markSceneDirty = true;
                        }
                    }
                }
                EGUI.EndHorizontal();
                EGUI.Space(SPACE_4);
                EGUI.Space(2);
                EGUI.DrawSeparatorGreen(WIDTH_420, SPACE_16, 2);
                EGUI.Space(2);
            }
        }

        void AddBind()
        {
            if(ezBindExtension.bindsData == null)
            {
                ezBindExtension.bindsData = new List<BindData>();
                bindsInfo = new List<BindInfo>();
            }
            ezBindExtension.bindsData.Add(new BindData());
            bindsInfo.Add(new BindInfo(Repaint));
            InitializeBindInfo();
            markSceneDirty = true;
        }
        void DeleteBind(BindData b, BindInfo bi)
        {
            if(ezBindExtension.bindsData == null || ezBindExtension.bindsData.Count == 0) { return; }
            ezBindExtension.bindsData.Remove(b);
            bindsInfo.Remove(bi);
            InitializeBindInfo();
            markSceneDirty = true;
        }
    }
}
