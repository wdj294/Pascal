// Copyright (c) 2016 - 2017 Ez Entertainment SRL. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Ez.Binding.Vars;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;
using UnityEngine.Events;

namespace Ez.Binding
{
    #region Classes : BindInfo, BoundItemInfo
    public class BindInfo
    {
        public string bindName = string.Empty;
        public BoundItemInfo source = new BoundItemInfo();
        public List<BoundItemInfo> observers = new List<BoundItemInfo>();
        public AnimBool show;

        public BindInfo(UnityAction callback)
        {
            Reset(callback);
        }

        public void Reset(UnityAction callback)
        {
            bindName = string.Empty;
            ResetSource();
            ResetObservers();
            show = new AnimBool(false, callback);
        }

        public bool HasBind { get { return !string.IsNullOrEmpty(bindName); } }
        public bool HasSource { get { return source != null && source.gameObject != null && source.component != null && !string.IsNullOrEmpty(source.variableName) && !source.variableName.Equals("None"); } }
        public bool HasObservers { get { return ObserversCount != 0; } }
        public int ObserversCount { get { return observers == null ? 0 : observers.Count; } }

        public void ResetSource()
        {
            source = new BoundItemInfo();
        }

        public void ResetObservers()
        {
            observers = new List<BoundItemInfo>();
        }

        public void GetBindInfo(BindData bind, bool initialize = true)
        {
            bindName = bind.bindName;
            source.GetBoundItemInfo(bind.source, initialize);
            if(bind.observers == null || bind.observers.Count == 0) { return; }
            observers = new List<BoundItemInfo>();
            for(int i = 0; i < bind.observers.Count; i++)
            {
                observers.Add(new BoundItemInfo());
                observers[i].GetBoundItemInfo(bind.observers[i], initialize);
            }
        }
    }
    public class BoundItemInfo
    {
        public const string DEFAULT_SELECTION_NAME = "None";
        private const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static;

        public GameObject gameObject = null;
        public Component component = null;
        public Component[] componentsArray = null;
        public string[] componentsNamesArray = null;
        public int componentIndex = 0;
        public string variableName = string.Empty;
        public string[] variableNamesArray = null;
        public int variableNameIndex = 0;

        public BoundItemInfo() { Reset(); }

        public void Initialize()
        {
            if(gameObject == null) { Reset(); return; }
            if(component == null) { ResetComponent(); return; }
            InitializeComponent();
            if(string.IsNullOrEmpty(variableName)) { ResetVariableName(); return; }
            InitializeVariableName();
        }

        public void InitializeComponent()
        {
            if(gameObject == null) { return; }
            componentsArray = GetComponentsArray();
            componentsNamesArray = GetComponentsNamesArray();
            componentIndex = GetComponentIndex();
            UpdateComponent(componentIndex, componentsArray);
        }

        public void UpdateComponent(int index, Component[] array = null)
        {
            if(array == null) { array = GetComponentsArray(); componentsArray = array; }
            component = array[index];
            componentIndex = index;
        }

        public void InitializeVariableName()
        {
            if(gameObject == null || component == null) { variableName = ""; return; }
            variableNamesArray = GetVariableNamesArray();
            variableNameIndex = GetVariableNameIndex();
            UpdateVariableName(variableNameIndex, variableNamesArray);
        }

        public void UpdateVariableName(int index, string[] array = null)
        {
            if(array == null) { array = GetVariableNamesArray(); variableNamesArray = array; }
            variableName = array[index];
            variableNameIndex = index;
        }

        public void Reset()
        {
            gameObject = null;
            ResetComponent();
            ResetVariableName();
        }

        public void ResetComponent()
        {
            component = null;
            componentsArray = null;
            componentsNamesArray = null;
            componentIndex = 0;
        }

        public void ResetVariableName()
        {
            variableName = string.Empty;
            variableNamesArray = null;
            variableNameIndex = 0;
        }

        public void UpdateVariableNameIndex(int index)
        {
            if(gameObject == null) { Reset(); return; }
            if(component == null) { ResetComponent(); return; }
            if(string.IsNullOrEmpty(variableName)) { ResetVariableName(); return; }
            variableNameIndex = index;
        }

        public Component[] GetComponentsArray()
        {
            List<Component> list = new List<Component>() { null };
            if(gameObject != null) { list.AddRange(gameObject.GetComponents<Component>()); }
            return list.ToArray();
        }

        public int GetComponentIndex()
        {
            if(gameObject == null || component == null) { return 0; }
            componentsArray = GetComponentsArray();
            if(componentsArray == null) { return 0; }
            for(int i = 0; i < componentsArray.Length; i++)
            {
                if(component == componentsArray[i]) { return i; }
            }
            return 0;
        }

        public string[] GetComponentsNamesArray()
        {
            Component[] components = GetComponentsArray();
            List<string> list = new List<string>() { DEFAULT_SELECTION_NAME };
            for(int i = 0; i < components.Length; i++)
            {
                if(components[i] == null) { continue; }
                list.Add(components[i].GetType().Name);
            }
            return list.ToArray();
        }

        public string[] GetVariableNamesArray()
        {
            string[] ignoreTypeNames = new string[] { "List`1", "[]" };
            if(gameObject == null || component == null) { return null; }
            List<string> list = new List<string>();
            list.AddRange
                (
                   component.GetType().GetFields(flags)
                   .Where(x => !ignoreTypeNames.Any(n => x.FieldType.Name.Contains(n))) // Don't list methods in the ignoreTypeNames array (so we can exclude Unity specific types, etc.)
                   .Where(x => !x.IsInitOnly)
                   .Select(x => GetTypeName(x.FieldType) + " " + x.Name)
                   .ToArray()
                );
            list.AddRange
               (
                  component.GetType().GetProperties(flags)
                  .Where(x => x.CanRead && x.CanWrite)
                  .Select(x => GetTypeName(x.PropertyType) + " " + x.Name)
                  .ToArray()
               );
            list.Sort();
            list.Insert(0, DEFAULT_SELECTION_NAME);
            return list.ToArray(); ;
        }

        private string GetTypeName(System.Type t)
        {
            string result;
            if(t == typeof(int))
            {
                result = "int";
            }
            else if(t == typeof(float))
            {
                result = "float";
            }
            else if(t == typeof(string))
            {
                result = "string";
            }
            else if(t == typeof(bool))
            {
                result = "bool";
            }
            else
            {
                result = t.IsSpecialName ? t.FullName : t.Name;
            }
            return result;
        }

        public int GetVariableNameIndex()
        {
            if(gameObject == null || component == null || string.IsNullOrEmpty(variableName) || variableNamesArray == null) { return 0; }
            for(int i = 0; i < variableNamesArray.Length; i++) { if(variableName.Equals(variableNamesArray[i])) { return i; } }
            return 0;
        }

        public void GetBoundItemInfo(BoundItem boundItem, bool initialize = true)
        {
            gameObject = boundItem.gameObject;
            component = boundItem.component;
            variableName = boundItem.variableName;
            if(initialize) { Initialize(); }
        }

    }
    #endregion

    [CustomEditor(typeof(EzBind))]
    public class EzBindEditor : EBaseEditor
    {
        EzBind ezBind { get { return (EzBind)target; } }

        public List<BindInfo> bindsInfo = new List<BindInfo>();
        private bool initialized = false;
        private bool markSceneDirty = false;

        Dictionary<string, Bind> BindingVariables;
        private GUIStyle ComponentSmallWhiteTextStyle;

        void GetStyles()
        {
            ComponentSmallWhiteTextStyle = EStyles.GetStyle(EStyles.TextStyle.ComponentSmall).Copy();
            ComponentSmallWhiteTextStyle.normal.textColor = EColor.WhiteLight;
        }

        void InitializeBindInfo()
        {
            if(ezBind.bindsData == null) { ezBind.bindsData = new List<BindData>(); }
            if(bindsInfo == null) { bindsInfo = new List<BindInfo>(); }
            if(!initialized || ezBind.bindsData.Count != bindsInfo.Count)
            {
                bindsInfo = new List<BindInfo>();
                for(int i = 0; i < ezBind.bindsData.Count; i++)
                {
                    bindsInfo.Add(new BindInfo(Repaint));
                    bindsInfo[i].GetBindInfo(ezBind.bindsData[i], true);
                }
                initialized = true;
            }
        }

        void OnEnable()
        {
            requiresContantRepaint = true;
            InitializeBindInfo();
            GetStyles();
            BindingVariables = new Dictionary<string, Bind>();
        }

        public override void OnInspectorGUI()
        {
            DrawHeader(EResources.HeaderBarBind.normal);
            EGUI.Space(SPACE_4);
            if(EditorApplication.isPlaying)
            {
                BindingVariables = ezBind.BindsHolder;
                if(BindingVariables == null || BindingVariables.Count == 0) { EGUI.Label("No binds were found."); EGUI.Space(SPACE_4); return; }
                EGUI.DrawSeparatorGreyMild(WIDTH_420);
                foreach(var bv in BindingVariables)
                {
                    EGUI.BeginHorizontal(WIDTH_420);
                    {
                        EGUI.Label(bv.Key + ": ", EStyles.GetStyle(EStyles.TextStyle.ComponentNormal));
                        if(bv.Value == null || bv.Value.Value == null)
                        {
                            EGUI.Label("---", EStyles.GetStyle(EStyles.TextStyle.ComponentSmall), WIDTH_210, 18);
                        }
                        else
                        {
                            EGUI.Label(bv.Value.Value.ToString(), EStyles.GetStyle(EStyles.TextStyle.ComponentSmall), WIDTH_210, 18);
                        }
                        EGUI.FlexibleSpace();
                    }
                    EGUI.EndHorizontal();
                    EGUI.Space(-SPACE_2);
                    EGUI.Label("Source: " + (bv.Value.HasSource ? "Added" : "None") + ", Observers: " + bv.Value.ObserverCount, EStyles.GetStyle(EStyles.TextStyle.ComponentSmall));
                    EGUI.DrawSeparatorGreyMild(WIDTH_420);
                    EGUI.Space(SPACE_2);
                }
                EGUI.Space(SPACE_2);
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
            if(EGUI.Button("\uf067  Add Bind", EStyles.GetStyle(EStyles.ButtonStyle.ButtonGreen), WIDTH_420, HEIGHT_24))
            {
                AddBind();
                markSceneDirty = true;
            }
            EGUI.Space(SPACE_4);
            if(bindsInfo == null || bindsInfo.Count == 0) { EGUI.Label("No binds have been found."); EGUI.Space(SPACE_4); }
            if(bindsInfo == null || bindsInfo.Count == 0) { EGUI.Space(SPACE_4); return; }
            for(int i = 0; i < bindsInfo.Count; i++)
            {
                DrawBind(ezBind.bindsData[i], bindsInfo[i], i);
                EGUI.Space(SPACE_4);
            }
            EGUI.Space(SPACE_4);
        }
        void DrawBind(BindData b, BindInfo bInfo, int index)
        {
            UpdateBindInfo(bInfo, b);
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
                GUIStyle buttonStyle = EStyles.GetStyle(string.IsNullOrEmpty(bInfo.bindName) ? EStyles.ButtonStyle.LeftButtonRed : EStyles.ButtonStyle.LeftButtonGreen);
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
                if(!string.IsNullOrEmpty(ezBind.bindsData[index].bindName))
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
            UpdateBind(b, bInfo);
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
                        UpdateBoundItem(b.source, bInfo.source);
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
                if(EGUI.Button("\uf067  Add Observer", EStyles.GetStyle(EStyles.ButtonStyle.ButtonGreen), WIDTH_420 - SPACE_16, HEIGHT_16))
                {
                    AddObserver(b, bInfo);
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
                        if(bInfo.observers[i].gameObject == null) { bInfo.observers[i].Reset(); UpdateBoundItem(b.observers[i], bInfo.observers[i]); }
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
            if(ezBind.bindsData == null)
            {
                ezBind.bindsData = new List<BindData>();
                bindsInfo = new List<BindInfo>();
            }
            ezBind.bindsData.Add(new BindData());
            bindsInfo.Add(new BindInfo(Repaint));
            InitializeBindInfo();
        }
        void DeleteBind(BindData b, BindInfo bi)
        {
            if(ezBind.bindsData == null || ezBind.bindsData.Count == 0) { return; }
            ezBind.bindsData.Remove(b);
            bindsInfo.Remove(bi);
            InitializeBindInfo();
            EGUI.SetDirty(target);
        }

        public static void AddObserver(BindData b, BindInfo bInfo)
        {
            if(b == null) { return; }
            if(b.observers == null) { b.observers = new List<BoundItem>(); bInfo.observers = new List<BoundItemInfo>(); }
            b.observers.Add(new BoundItem());
            bInfo.observers.Add(new BoundItemInfo());
        }

        public static void UpdateBindInfo(BindInfo bi, BindData b)
        {
            bi.bindName = b.bindName;
            bi.source.gameObject = b.source.gameObject;
            bi.source.component = b.source.component;
            bi.source.variableName = b.source.variableName;
            bi.source.Initialize();
            bi.observers = new List<BoundItemInfo>();
            if(b.observers == null || b.observers.Count == 0) { return; }
            for(int i = 0; i < b.observers.Count; i++)
            {
                bi.observers.Add(new BoundItemInfo());
                bi.observers[i].gameObject = b.observers[i].gameObject;
                bi.observers[i].component = b.observers[i].component;
                bi.observers[i].variableName = b.observers[i].variableName;
                bi.observers[i].Initialize();
            }
        }
        public static void UpdateBind(BindData b, BindInfo bi)
        {
            b.bindName = bi.bindName;
            b.source.gameObject = bi.source.gameObject;
            b.source.component = bi.source.component;
            b.source.variableName = bi.source.variableName;
            b.observers = new List<BoundItem>();
            if(bi.observers == null || bi.observers.Count == 0) { return; }
            for(int i = 0; i < bi.observers.Count; i++)
            {
                b.observers.Add(new BoundItem());
                b.observers[i].gameObject = bi.observers[i].gameObject;
                b.observers[i].component = bi.observers[i].component;
                b.observers[i].variableName = bi.observers[i].variableName;
            }
        }
        public static void UpdateBoundItem(BoundItem bItem, BoundItemInfo bItemInfo)
        {
            bItem.gameObject = bItemInfo.gameObject;
            bItem.component = bItemInfo.component;
            bItem.variableName = bItemInfo.variableName;
        }
        public static void UpdateBoundItemInfo(BoundItemInfo bi, BoundItem b)
        {
            bi.gameObject = b.gameObject;
            bi.component = b.component;
            bi.variableName = b.variableName;
            bi.Initialize();
        }
    }
}
