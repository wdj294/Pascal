// Copyright (c) 2016 Ez Entertainment SRL. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEditorInternal;
using UnityEngine;

namespace Ez.DataManager
{
    [CustomEditor(typeof(EzDataManager))]
    public class EzDataManagerEditor : EzEditor
    {
        private EzDataManager ezDataManager { get { return (EzDataManager)target; } }

        public const string CATEGORY_START = "CTGSTRT_";
        public const string CATEGORY_END = "CTGEND_";

        #region variableTypes, typeNames, invalidVariableNames
        public string[] variableTypes = new string[]
        {
            "variable", "array", "list"
        };

        public string[] typeNames = new string[]
        {
            "AnimationCurve","AudioClip",
            "bool",
            "Color","Color32",
            "double",
            "float",
            "GameObject",
            "int",
            "long",
            "Material","Mesh",
            "Object",
            "ParticleSystem",
            "Quaternion",
            "Rect","RectTransform",
            "Sprite","string",
            "TerrainData","Texture","Transform",
            "Vector2","Vector3","Vector4"
        };

        public string[] invalidVariableNames = new string[]
        {
            //typeNames
            "AnimationCurve","AudioClip",
            "bool",
            "Color","Color32",
            "double",
            "float",
            "GameObject",
            "int",
            "long",
            "Material","Mesh",
            "Object",
            "ParticleSystem",
            "Quaternion",
            "Rect","RectTransform",
            "Sprite","string",
            "TerrainData","Texture","Transform",
            "Vector2","Vector3","Vector4",
            //C# Keywords
            "abstract", "as", "base", "break", "byte", "case", "catch", "char", "checked", "class", "const", "continue", "decimal", "default", "delegate", "do", "else", "enum", "event", "explicit", "extern",
            "false", "finally", "fixed", "for", "foreach", "goto", "if", "implicit", "in", "interface", "internal", "is", "lock", "namespace", "new", "null", "object", "operator", "out", "override",
            "params", "private", "protected", "public", "readonly", "ref", "return", "sbyte", "sealed", "short", "sizeof", "stackalloc", "static", "struct", "switch", "this", "throw", "true", "try", "typeof",
            "uint", "ulong", "unchecked", "unsafe", "ushort", "using", "virtual", "void", "volatile", "while",
            //Contextual Keywords
            "add", "alias", "ascending", "async", "await", "descending", "dynamic", "from", "get", "global", "group", "into", "join", "let", "orderby", "partial", "remove", "select", "set", "value", "var", "where", "yield"
        };
        #endregion

        [Serializable]
        public class Variable { public string variableType; public string typeName; public string name; public SerializedProperty sp; }

        FieldInfo[] fields;
        const BindingFlags fieldsFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;

        Dictionary<string, List<Variable>> data;
        Dictionary<string, List<float>> dataHeight;
        Dictionary<string, AnimBool> showCategory;
        Dictionary<string, ReorderableList> rData;

        AnimFloat variableNameWidth = new AnimFloat(120);

        AnimBool addCategory = new AnimBool(false);
        AnimBool showSettings = new AnimBool(false);
        bool renameCategory, addVariable = false;
        string newCategoryName, renameCategoryName, activeCategoryName = string.Empty;
        string addVariableVariableType, addVariableTypeName, addVariableName = string.Empty;
        int addVariableVariableTypeIndex = 0;
        int addVariableTypeNameIndex = 4;

        GUIStyle arrayBarOpen, arrayBarClosed, arrayItemIndex, dropBoxStyle;

        bool usavedChanges = false;

        void OnEnable()
        {
            repaintOn = RepaintOn.InspectorUpdate;
            showHelp = false;
            saying = EzResources.GetSaying;
            SetupVariables();
            SetupCustomStyles();

            addCategory.valueChanged.AddListener(Repaint);
            showSettings.valueChanged.AddListener(Repaint);
            variableNameWidth.valueChanged.AddListener(Repaint);
            variableNameWidth.valueChanged.AddListener(() => { ezDataManager.editorVariableNameWidth = variableNameWidth.target; });
        }

        void SetupVariables()
        {
            usavedChanges = false;

            ezDataManager.editorVariableNameWidth = Mathf.Clamp(ezDataManager.editorVariableNameWidth, 40, 210);
            variableNameWidth.value = ezDataManager.editorVariableNameWidth;


            data = new Dictionary<string, List<Variable>>();
            dataHeight = new Dictionary<string, List<float>>();
            showCategory = new Dictionary<string, AnimBool>();
            rData = new Dictionary<string, ReorderableList>();
            fields = ezDataManager.GetType().GetFields(fieldsFlags);
            string currentCategory = string.Empty;
            foreach (var field in fields)
            {
                if (field.Name.Contains(CATEGORY_START)) //starting to add variables to a new active category
                {
                    currentCategory = serializedObject.FindProperty(field.Name).stringValue;
                    AddCategory(currentCategory);
                }
                else
                {
                    if (field.Name.Contains(CATEGORY_END)) currentCategory = string.Empty; //stopping adding values to the active category
                }

                if (!string.IsNullOrEmpty(currentCategory) && !field.Name.Contains(CATEGORY_START)) //add the variable to the database only if we have an active category selected and if this is not a category 'header'
                {
                    data[currentCategory].Add(new Variable() { name = field.Name, variableType = GetVariableType(field.FieldType.ToString()), typeName = GetTypeName(field.FieldType.ToString()), sp = serializedObject.FindProperty(field.Name) });
                    dataHeight[currentCategory].Add(EditorGUIUtility.singleLineHeight);
                }
            }

            if (renameCategory) { RenameCategoryReset(); }
            if (addVariable) { AddVariableReset(); }
        }

        void SetupCustomStyles()
        {
            arrayBarOpen = new GUIStyle
            {
                normal = { background = (Texture2D)EzResources.BtnBarGreenNormalOpen, textColor = EzColors.L_GREEN },
                active = { background = (Texture2D)EzResources.BtnBarGreenActive, textColor = EzColors.GREEN },
                fontSize = 9,
                fontStyle = FontStyle.Normal,
                alignment = TextAnchor.MiddleLeft,
                fixedHeight = 16,
                padding = new RectOffset(18, 2, 1, 2),
                border = new RectOffset(1, 1, 1, 1),
                margin = new RectOffset(1, 1, 1, 1)
            };
            arrayBarClosed = new GUIStyle
            {
                normal = { background = (Texture2D)EzResources.BtnBarGreyNormalOpen, textColor = EzColors.UNITY_LIGHT },
                active = { background = (Texture2D)EzResources.BtnBarGreyActive, textColor = EzColors.UNITY_MILD },
                fontSize = 9,
                fontStyle = FontStyle.Normal,
                alignment = TextAnchor.MiddleLeft,
                fixedHeight = 16,
                padding = new RectOffset(18, 2, 1, 2),
                border = new RectOffset(1, 1, 1, 1),
                margin = new RectOffset(1, 1, 1, 1)
            };

            arrayItemIndex = new GUIStyle
            {
                normal = { textColor = EzColors.UNITY_MILD },
                fontSize = 9,
                alignment = TextAnchor.MiddleRight
            };

            dropBoxStyle = new GUIStyle(skin.GetStyle(EzStyle.StyleName.HelpBox.ToString())); dropBoxStyle.alignment = TextAnchor.MiddleCenter;
        }

        public override void OnInspectorGUI()
        {
            EzEditorUtility.VerticalSpace(VERTICAL_SPACE_BETWEEN_ELEMENTS);
            DrawHeader(EzResources.HeaderBarDataManager);
            EzEditorUtility.VerticalSpace(VERTICAL_SPACE_BETWEEN_ELEMENTS);
            if (EditorApplication.isCompiling)
            {
                DrawEditorIsCompiling(saying);
                EzEditorUtility.VerticalSpace(VERTICAL_SPACE_BETWEEN_ELEMENTS);
                EzEditorUtility.VerticalSpace(VERTICAL_SPACE_BETWEEN_ELEMENTS);
                return;
            }
            CheckDragNDrop();
            DrawTopMenu();
            EzEditorUtility.VerticalSpace(VERTICAL_SPACE_BETWEEN_ELEMENTS);
            serializedObject.Update();
            //SetBaseColor();
            if (data == null || data.Keys == null) SetupVariables();
            if (data.Keys.Count > 0)
            {
                foreach (var key in data.Keys)
                {
                    SaveCurrentColorsAndResetColors();
                    EditorGUILayout.BeginHorizontal();
                    {
                        if (showCategory[key].value)
                        {
                            if (GUILayout.Button(key, skin.GetStyle(EzStyle.StyleName.BtnBarGreen.ToString()), GUILayout.Width(WIDTH_1)))
                            {
                                showCategory[key].target = false;
                                CloseCategory(key);
                                if (key.Equals(activeCategoryName))
                                {
                                    if (renameCategory) { RenameCategoryReset(); }
                                    if (addVariable) { AddVariableReset(); }
                                }
                                GUIUtility.keyboardControl = 0;
                            }
                        }
                        else
                        {
                            if (GUILayout.Button(key, skin.GetStyle(EzStyle.StyleName.BtnBarGrey.ToString()), GUILayout.Width(WIDTH_1)))
                            {
                                showCategory[key].target = true;
                                GUIUtility.keyboardControl = 0;
                            }
                        }
                        GUILayout.FlexibleSpace();
                    }
                    EditorGUILayout.EndHorizontal();
                    LoadPreviousColors();

                    if (EditorGUILayout.BeginFadeGroup(showCategory[key].faded))
                    {
                        DrawCategory(key);
                    }
                    EditorGUILayout.EndFadeGroup();
                }
            }
            EzEditorUtility.VerticalSpace(VERTICAL_SPACE_BETWEEN_ELEMENTS);
            EzEditorUtility.ResetColors();
            serializedObject.ApplyModifiedProperties();
        }

        void CheckDragNDrop()
        {
            switch (Event.current.type)
            {
                //case EventType.MouseDown: DragAndDrop.PrepareStartDrag(); break; //Debug.Log("MouseDown"); //reset the DragAndDrop Data
                case EventType.DragUpdated: DragAndDrop.visualMode = DragAndDropVisualMode.Copy; break; //Debug.Log("DragUpdated " + Event.current.mousePosition);
                case EventType.DragPerform: DragAndDrop.AcceptDrag(); break; //Debug.Log("Drag accepted");
                //case EventType.MouseDrag: DragAndDrop.StartDrag("Dragging"); Event.current.Use(); break; //Debug.Log("MouseDrag: " + Event.current.mousePosition);
                case EventType.MouseUp: DragAndDrop.PrepareStartDrag(); break; //Debug.Log("MouseUp had " + DragAndDrop.GetGenericData("GameObject"));  //Clean up, in case MouseDrag never occurred
                case EventType.DragExited: break; //Debug.Log("DragExited");
            }
        }

        void DrawTopMenu()
        {
            EditorGUILayout.BeginHorizontal(GUILayout.Width(WIDTH_1));
            {
                if (usavedChanges)
                {
                    if (GUILayout.Button("Apply Changes", skin.GetStyle(EzStyle.StyleName.BtnOrange.ToString()), GUILayout.Width(118), GUILayout.Height(20)))
                    {
                        EzDataUtility.GenerateEzDataManagerScript(data);
                        GUIUtility.keyboardControl = 0;
                    }
                    GUILayout.Space(1);
                }
                if (GUILayout.Button("Reload Data", skin.GetStyle(EzStyle.StyleName.BtnGreyDark.ToString()), usavedChanges ? GUILayout.Width(117) : GUILayout.Width(177), GUILayout.Height(20)))
                {
                    SetupVariables();
                    GUIUtility.keyboardControl = 0;
                }
                GUILayout.Space(1);
                if (GUILayout.Button("New Category", addCategory.value
                                                    ? skin.GetStyle(EzStyle.StyleName.BtnGreen.ToString())
                                                    : skin.GetStyle(EzStyle.StyleName.BtnGreyDark.ToString()), usavedChanges ? GUILayout.Width(117) : GUILayout.Width(177), GUILayout.Height(20)))
                {
                    addCategory.target = !addCategory.value;
                    newCategoryName = string.Empty;
                    if (showSettings.value)
                    {
                        showSettings.target = false;
                    }
                    GUIUtility.keyboardControl = 0;
                }
                GUILayout.Space(1);
                if (GUILayout.Button("Settings", showSettings.value
                                                ? skin.GetStyle(EzStyle.StyleName.BtnGreen.ToString())
                                                : skin.GetStyle(EzStyle.StyleName.BtnGreyDark.ToString()), usavedChanges ? GUILayout.Width(60) : GUILayout.Width(60), GUILayout.Height(20)))
                {
                    showSettings.target = !showSettings.value;
                    if (addCategory.value)
                    {
                        addCategory.target = false;
                        newCategoryName = string.Empty;
                    }
                    GUIUtility.keyboardControl = 0;
                }
            }
            EditorGUILayout.EndHorizontal();



            if (EditorGUILayout.BeginFadeGroup(addCategory.faded))
            {
                EzEditorUtility.VerticalSpace(VERTICAL_SPACE_BETWEEN_ELEMENTS);
                EditorGUILayout.BeginHorizontal(GUILayout.Width(WIDTH_1));
                {
                    newCategoryName = EditorGUILayout.TextField(newCategoryName, GUILayout.Width((WIDTH_1 - 60 - 1 - 60 - 8) * addCategory.faded));
                    if (GUILayout.Button("Ok", skin.GetStyle(EzStyle.StyleName.BtnGreen.ToString()), GUILayout.Width(60 * addCategory.faded), GUILayout.Height(18)))
                    {
                        AddCategory(newCategoryName);
                        usavedChanges = true;
                        GUIUtility.keyboardControl = 0;
                    }
                    GUILayout.Space(1);
                    if (GUILayout.Button("Cancel", skin.GetStyle(EzStyle.StyleName.BtnRed.ToString()), GUILayout.Width(60 * addCategory.faded), GUILayout.Height(18)))
                    {
                        AddCategoryReset();
                        GUIUtility.keyboardControl = 0;
                    }
                    GUILayout.FlexibleSpace();
                }
                EditorGUILayout.EndHorizontal();
                EzEditorUtility.VerticalSpace(VERTICAL_SPACE_BETWEEN_ELEMENTS);
            }
            EditorGUILayout.EndFadeGroup();


            if (EditorGUILayout.BeginFadeGroup(showSettings.faded))
            {
                EzEditorUtility.VerticalSpace(VERTICAL_SPACE_BETWEEN_ELEMENTS);
                EditorGUILayout.BeginHorizontal(GUILayout.Width(WIDTH_1));
                {
                    EditorGUILayout.LabelField("Variable Name Width", GUILayout.Width(126 * showSettings.faded));
                    variableNameWidth.target = EditorGUILayout.Slider(variableNameWidth.target, 40, 210, GUILayout.Width(224 * showSettings.faded));
                    if (GUILayout.Button("reset", skin.GetStyle(EzStyle.StyleName.BtnGreyMild.ToString()), GUILayout.Width(60 * showSettings.faded), GUILayout.Height(18)))
                    {
                        variableNameWidth.target = 120f;
                        GUIUtility.keyboardControl = 0;
                    }
                    GUILayout.FlexibleSpace();
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal(GUILayout.Width(WIDTH_1));
                {
                    GUILayout.Box("", skin.GetStyle(EzStyle.StyleName.BtnGreyMild.ToString()), GUILayout.Height(8), GUILayout.Width(20 * showSettings.faded));
                    GUILayout.Box("", skin.GetStyle(EzStyle.StyleName.BtnOrange.ToString()), GUILayout.Height(8), GUILayout.Width(variableNameWidth.value * showSettings.faded));
                    GUILayout.Box("", skin.GetStyle(EzStyle.StyleName.BtnGreyMild.ToString()), GUILayout.Height(8), GUILayout.Width((WIDTH_1 - 20 - variableNameWidth.value - 4) * showSettings.faded));
                    GUILayout.FlexibleSpace();
                }
                EditorGUILayout.EndHorizontal();
                EzEditorUtility.VerticalSpace(VERTICAL_SPACE_BETWEEN_ELEMENTS);
            }
            EditorGUILayout.EndFadeGroup();
        }

        void DrawCategory(string categoryName)
        {
            EditorGUILayout.BeginHorizontal(GUILayout.Width(WIDTH_1));
            {
                GUILayout.Space(4);
                if (!addVariable && !renameCategory)
                {
                    if (GUILayout.Button("Add Variable", skin.GetStyle(EzStyle.StyleName.BtnGreen.ToString()), GUILayout.Width(WIDTH_3 - 4), GUILayout.Height(18 * showCategory[categoryName].faded)))
                    {
                        addVariable = true;
                        addVariableTypeNameIndex = 0;
                        activeCategoryName = categoryName;
                        addVariableName = string.Empty;
                        usavedChanges = true;
                        GUIUtility.keyboardControl = 0;
                    }
                    GUILayout.Space(1);
                    if (GUILayout.Button("Rename Category", skin.GetStyle(EzStyle.StyleName.BtnOrange.ToString()), GUILayout.Width(WIDTH_3 - 4), GUILayout.Height(18 * showCategory[categoryName].faded)))
                    {
                        activeCategoryName = categoryName;

                        bool categoryExistsInTheFile = false;
                        foreach (var field in fields) { if (field.Name.Contains(CATEGORY_START) && serializedObject.FindProperty(field.Name).stringValue.Equals(activeCategoryName)) { categoryExistsInTheFile = true; } }
                        if (categoryExistsInTheFile == false) { EzDataUtility.GenerateEzDataManagerScript(data); } //we don't have this category in the file so we force a file write

                        renameCategory = true;
                        renameCategoryName = categoryName;
                        GUIUtility.keyboardControl = 0;
                    }
                    GUILayout.Space(1);
                    if (GUILayout.Button("Delete Category", skin.GetStyle(EzStyle.StyleName.BtnRed.ToString()), GUILayout.Width(WIDTH_3 - 4), GUILayout.Height(18 * showCategory[categoryName].faded)))
                    {
                        if (EditorUtility.DisplayDialog("Delete Category", "Are you sure you want to delete the '" + categoryName + "' Category? This will delete all the variables inside of it as well. Operation cannot be undone!", "Ok", "Cancel"))
                        {
                            DeleteCategory(categoryName);
                            GUIUtility.keyboardControl = 0;
                        }
                    }
                }
                else if (addVariable && activeCategoryName.Equals(categoryName))
                {
                    addVariableVariableTypeIndex = EditorGUILayout.Popup(addVariableVariableTypeIndex, variableTypes, GUILayout.Width(60));
                    addVariableVariableType = variableTypes[addVariableVariableTypeIndex];
                    addVariableTypeNameIndex = EditorGUILayout.Popup(addVariableTypeNameIndex, typeNames, GUILayout.Width(100));
                    addVariableTypeName = typeNames[addVariableTypeNameIndex];
                    addVariableName = EditorGUILayout.TextField(addVariableName, GUILayout.Width(138));
                    if (GUILayout.Button("Ok", skin.GetStyle(EzStyle.StyleName.BtnGreen.ToString()), GUILayout.Width(50), GUILayout.Height(18)))
                    {
                        AddVariable();
                        usavedChanges = true;
                        GUIUtility.keyboardControl = 0;
                    }
                    GUILayout.Space(1);
                    if (GUILayout.Button("Cancel", skin.GetStyle(EzStyle.StyleName.BtnRed.ToString()), GUILayout.Width(50), GUILayout.Height(18)))
                    {
                        AddVariableReset();
                        GUIUtility.keyboardControl = 0;
                    }
                }
                else if (renameCategory && activeCategoryName.Equals(categoryName))
                {
                    renameCategoryName = EditorGUILayout.TextField(renameCategoryName, GUILayout.Width(306));
                    if (GUILayout.Button("Ok", skin.GetStyle(EzStyle.StyleName.BtnGreen.ToString()), GUILayout.Width(50), GUILayout.Height(18)))
                    {
                        RenameCategory();
                        usavedChanges = true;
                        GUIUtility.keyboardControl = 0;
                    }
                    GUILayout.Space(1);
                    if (GUILayout.Button("Cancel", skin.GetStyle(EzStyle.StyleName.BtnRed.ToString()), GUILayout.Width(50), GUILayout.Height(18)))
                    {
                        RenameCategoryReset();
                        GUIUtility.keyboardControl = 0;
                    }
                }
                else { EzEditorUtility.VerticalSpace(19); }
                GUILayout.FlexibleSpace();
            }
            EditorGUILayout.EndHorizontal();

            rData[categoryName].DoLayoutList();
        }

        void AddCategory(string categoryName)
        {
            if (data == null) data = new Dictionary<string, List<Variable>>();
            if (dataHeight == null) dataHeight = new Dictionary<string, List<float>>();
            if (showCategory == null) showCategory = new Dictionary<string, AnimBool>();

            if (!data.ContainsKey(categoryName))
            {
                data.Add(categoryName, new List<Variable>());
                dataHeight.Add(categoryName, new List<float>());
                showCategory[categoryName] = new AnimBool(false);
                showCategory[categoryName].valueChanged.AddListener(Repaint);
                rData[categoryName] = new ReorderableList(data[categoryName], typeof(Variable), true, false, false, false);
                rData[categoryName].onReorderCallback = (list) =>
                {
                    usavedChanges = true;
                };
                rData[categoryName].showDefaultBackground = false;
                rData[categoryName].drawElementBackgroundCallback = (rect, index, active, focused) => { };
                rData[categoryName].elementHeightCallback = (index) =>
                {
                    Repaint();
                    float height = EditorGUIUtility.singleLineHeight + 2;

                    try { height = dataHeight[categoryName][index]; }
                    catch (ArgumentOutOfRangeException e) { Debug.LogWarning(e.Message); }

                    return height;
                };

                rData[categoryName].drawElementCallback = (rect, index, active, focused) =>
                {
                    if (index == rData[categoryName].list.Count) return;
                    float width = WIDTH_1;
                    float height = EditorGUIUtility.singleLineHeight + 2;
                    float buttonWidth = 20;
                    float indexWidth = 34;
                    float dragBumbWidth = 0;
                    rect.x += dragBumbWidth;
                    Variable v = (Variable)rData[categoryName].list[index];
                    EditorGUI.LabelField(new Rect(rect.x, rect.y, variableNameWidth.value, EditorGUIUtility.singleLineHeight), v.name);
                    rect.x += variableNameWidth.value;
                    rect.x += 2;
                    float valueFieldWidth = width - variableNameWidth.value - buttonWidth - 24;
                    #region Variable Value Field
                    if (v.sp == null) { if (!addVariable && !renameCategory) { if (GUI.Button(new Rect(rect.x, rect.y, valueFieldWidth, EditorGUIUtility.singleLineHeight), "Add variable to Data Manager", skin.GetStyle(EzStyle.StyleName.BtnGreen.ToString()))) { EzDataUtility.GenerateEzDataManagerScript(data); } } return; }

                    if (v.variableType.Equals("variable"))
                    {
                        switch (v.typeName)
                        {
                            case "Quaternion": DrawQuaternion(v.sp, rect, valueFieldWidth - 3); break;
                            case "Rect": DrawRect(v.sp, rect, valueFieldWidth - 3); break;
                            case "Vector4": DrawVector4(v.sp, rect, valueFieldWidth - 3); break;
                            default: EditorGUI.PropertyField(new Rect(rect.x, rect.y, valueFieldWidth, EditorGUIUtility.singleLineHeight), v.sp, GUIContent.none, true); break;
                        }

                        if (GUI.Button(new Rect(rect.x + valueFieldWidth + 4, rect.y, buttonWidth - 4, EditorGUIUtility.singleLineHeight), "x", skin.GetStyle(EzStyle.StyleName.BtnRed.ToString()))) { DeleteVariable(categoryName, index); }
                    }
                    else
                    {
                        string varBarText = string.Empty;
                        if (v.variableType.Equals("array")) varBarText = v.typeName + " [" + v.sp.arraySize + "]";
                        if (v.variableType.Equals("list")) varBarText = "List<" + v.typeName + "> [" + v.sp.arraySize + "]";
                        Rect dropBox = new Rect(rect.x - variableNameWidth.value - 2, rect.y, WIDTH_1 - 22, EditorGUIUtility.singleLineHeight);
                        if (!v.sp.isExpanded)
                        {
                            if (GUI.Button(new Rect(rect.x, rect.y, valueFieldWidth, EditorGUIUtility.singleLineHeight), varBarText, arrayBarClosed)) { v.sp.isExpanded = true; }
                            if (GUI.Button(new Rect(rect.x + valueFieldWidth + 4, rect.y, buttonWidth - 4, EditorGUIUtility.singleLineHeight), "x", skin.GetStyle(EzStyle.StyleName.BtnRed.ToString()))) { DeleteVariable(categoryName, index); }
                        }
                        else
                        {
                            valueFieldWidth += buttonWidth;
                            if (GUI.Button(new Rect(rect.x, rect.y, valueFieldWidth, EditorGUIUtility.singleLineHeight), varBarText, arrayBarOpen)) { v.sp.isExpanded = false; }
                            if (v.sp.arraySize > 0)
                            {
                                for (int i = 0; i < v.sp.arraySize; i++)
                                {
                                    rect.y += EditorGUIUtility.singleLineHeight + 2; height += EditorGUIUtility.singleLineHeight + 2;
                                    EditorGUI.LabelField(new Rect(rect.x - indexWidth - 2, rect.y, indexWidth, EditorGUIUtility.singleLineHeight), "[" + i + "]", arrayItemIndex);
                                    switch (v.typeName)
                                    {
                                        case "Quaternion": DrawQuaternion(v.sp.GetArrayElementAtIndex(i), rect, valueFieldWidth - 3 - 2 * buttonWidth, true); break;
                                        case "Rect": DrawRect(v.sp.GetArrayElementAtIndex(i), rect, valueFieldWidth - 3 - 2 * buttonWidth, true); break;
                                        case "Vector4": DrawVector4(v.sp.GetArrayElementAtIndex(i), rect, valueFieldWidth - 3 - 2 * buttonWidth, true); break;
                                        default: EditorGUI.PropertyField(new Rect(rect.x, rect.y, valueFieldWidth - 2 * buttonWidth - 4, EditorGUIUtility.singleLineHeight), v.sp.GetArrayElementAtIndex(i), GUIContent.none, true); break;
                                    }
                                    var currentIndex = i;
                                    if (GUI.Button(new Rect(rect.x + valueFieldWidth - 2 * buttonWidth - 3, rect.y, buttonWidth, EditorGUIUtility.singleLineHeight), "-", skin.GetStyle(EzStyle.StyleName.BtnRed.ToString()))) { DeleteArrayElement(v.sp, currentIndex); }
                                    if (GUI.Button(new Rect(rect.x + valueFieldWidth - 1 * buttonWidth - 2, rect.y, buttonWidth, EditorGUIUtility.singleLineHeight), "+", skin.GetStyle(EzStyle.StyleName.BtnGreen.ToString()))) { InsertArrayElement(v.sp, currentIndex, v.typeName); }
                                }
                            }
                            rect.y += EditorGUIUtility.singleLineHeight + 2; height += EditorGUIUtility.singleLineHeight + 2;
                            if (GUI.Button(new Rect(rect.x + valueFieldWidth - 1 * buttonWidth - 2, rect.y, buttonWidth, EditorGUIUtility.singleLineHeight), "+", skin.GetStyle(EzStyle.StyleName.BtnGreen.ToString()))) { InsertArrayElement(v.sp, v.sp.arraySize, v.typeName); }
                            rect.y += EditorGUIUtility.singleLineHeight + 2; height += EditorGUIUtility.singleLineHeight + 2;
                        }


                        if (v.typeName.Equals("AudioClip") ||
                            v.typeName.Equals("GameObject") ||
                            v.typeName.Equals("Material") ||
                            v.typeName.Equals("Mesh") ||
                            v.typeName.Equals("ParticleSystem") ||
                            v.typeName.Equals("RectTransform") ||
                            v.typeName.Equals("Sprite") ||
                            v.typeName.Equals("Texture") ||
                            v.typeName.Equals("TerrainData") ||
                            v.typeName.Equals("Transform") ||
                            v.typeName.Equals("Object"))
                        {
                            if (DragAndDrop.objectReferences.Length > 0 && dropBox.Contains(Event.current.mousePosition))
                            {
                                if (isObjectValid(DragAndDrop.objectReferences[0], v.typeName)) { GUI.Box(new Rect(dropBox.x + variableNameWidth.value, dropBox.y, dropBox.width - variableNameWidth.value, dropBox.height), DragAndDrop.objectReferences.Length == 1 ? "Drop " + DragAndDrop.objectReferences.Length + " item..." : "Drop " + DragAndDrop.objectReferences.Length + " items...", dropBoxStyle); }
                            }

                            if (Event.current.type == EventType.DragPerform)
                            {
                                if (!dropBox.Contains(Event.current.mousePosition)) return;
                                Event.current.Use();
                                DragAndDrop.AcceptDrag();
                                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                                foreach (var obj in DragAndDrop.objectReferences)
                                {

                                    if (isObjectValid(obj, v.typeName))
                                    {
                                        v.sp.isExpanded = true;
                                        v.sp.InsertArrayElementAtIndex(v.sp.arraySize);
                                        switch (v.typeName)
                                        {
                                            case "Sprite": v.sp.GetArrayElementAtIndex(v.sp.arraySize - 1).objectReferenceValue = AssetDatabase.LoadAssetAtPath(AssetDatabase.GetAssetPath(obj), typeof(Sprite)); break;
                                            case "Texture": v.sp.GetArrayElementAtIndex(v.sp.arraySize - 1).objectReferenceValue = AssetDatabase.LoadAssetAtPath(AssetDatabase.GetAssetPath(obj), typeof(Texture)); break;
                                            case "Material": v.sp.GetArrayElementAtIndex(v.sp.arraySize - 1).objectReferenceValue = AssetDatabase.LoadAssetAtPath(AssetDatabase.GetAssetPath(obj), typeof(Material)); break;
                                            case "ParticleSystem": GameObject go = (GameObject)obj; v.sp.GetArrayElementAtIndex(v.sp.arraySize - 1).objectReferenceValue = go.GetComponent<ParticleSystem>(); break;
                                            default: v.sp.GetArrayElementAtIndex(v.sp.arraySize - 1).objectReferenceValue = obj; break;
                                        }

                                    }
                                }
                            }
                        }
                    }
                    #endregion
                    dataHeight[categoryName][index] = height;
                };

                AddCategoryReset();
            }
            else
            {
                EditorUtility.DisplayDialog("New category", "There is another category with the name '" + categoryName + "' in the database. Try saving with another name or delete that one and try again.", "Ok");
            }
        }

        void AddCategoryReset()
        {
            addCategory.target = false;
            newCategoryName = string.Empty;
        }

        void DeleteArrayElement(SerializedProperty sp, int index)
        {
            if (sp.GetArrayElementAtIndex(index).propertyType == SerializedPropertyType.ObjectReference &&
                sp.GetArrayElementAtIndex(index).objectReferenceValue != null)
            {
                sp.DeleteArrayElementAtIndex(index);
            }
            sp.DeleteArrayElementAtIndex(index);
            serializedObject.ApplyModifiedProperties();
            GUIUtility.ExitGUI();
        }

        void InsertArrayElement(SerializedProperty sp, int index, string typeName)
        {
            sp.InsertArrayElementAtIndex(index);
            if (sp.GetArrayElementAtIndex(index).propertyType == SerializedPropertyType.ObjectReference)
            {
                sp.GetArrayElementAtIndex(index).objectReferenceValue = null;
            }
            else
            {
                switch (typeName)
                {
                    case "AnimationCurve": sp.GetArrayElementAtIndex(index).animationCurveValue = new AnimationCurve(); break;
                    case "bool": sp.GetArrayElementAtIndex(index).boolValue = false; break;
                    case "Color": sp.GetArrayElementAtIndex(index).colorValue = new Color(0, 0, 0, 0); break;
                    case "Color32": sp.GetArrayElementAtIndex(index).colorValue = new Color(0, 0, 0, 0); break;
                    case "double": sp.GetArrayElementAtIndex(index).doubleValue = 0; break;
                    case "float": sp.GetArrayElementAtIndex(index).floatValue = 0; break;
                    case "int": sp.GetArrayElementAtIndex(index).intValue = 0; break;
                    case "long": sp.GetArrayElementAtIndex(index).longValue = 0; break;
                    case "Quaternion": sp.GetArrayElementAtIndex(index).quaternionValue = new Quaternion(); break;
                    case "Rect": sp.GetArrayElementAtIndex(index).rectValue = new Rect(); break;
                    case "string": sp.GetArrayElementAtIndex(index).stringValue = string.Empty; break;
                    case "Vector2": sp.GetArrayElementAtIndex(index).vector2Value = Vector2.zero; break;
                    case "Vector3": sp.GetArrayElementAtIndex(index).vector3Value = Vector3.zero; break;
                    case "Vector4": sp.GetArrayElementAtIndex(index).vector4Value = Vector4.zero; break;
                }
            }
            serializedObject.ApplyModifiedProperties();
            GUIUtility.ExitGUI();
        }

        /// <summary>
        /// Renames a category.  If ok is TRUE it will consider that the user pressed OK, otherwise it will consider that the user pressed Cancel
        /// </summary>
        void RenameCategory()
        {
            if (string.IsNullOrEmpty(renameCategoryName))
            {
                EditorUtility.DisplayDialog("Rename Category", "Please enter a category name!", "Ok");
                return;
            }

            if (data.ContainsKey(renameCategoryName))
            {
                EditorUtility.DisplayDialog("Rename Category", "There is another category with the same name '" + renameCategoryName + "' in the database. Try renaming to another name or delete/rename that one and try again.", "Ok");
                return;
            }

            foreach (var field in fields)
            {
                if (field.Name.Contains(CATEGORY_START) && serializedObject.FindProperty(field.Name).stringValue.Equals(activeCategoryName))
                {
                    serializedObject.FindProperty(field.Name).stringValue = renameCategoryName;
                }
            }

            serializedObject.ApplyModifiedPropertiesWithoutUndo();

            SetupVariables();
            RenameCategoryReset();
            GUIUtility.ExitGUI();
        }

        void RenameCategoryReset()
        {
            renameCategory = false;
            activeCategoryName = string.Empty;
            renameCategoryName = string.Empty;
        }

        void DeleteCategory(string categoryName)
        {
            rData.Remove(categoryName);
            data.Remove(categoryName);
            dataHeight.Remove(categoryName);
            showCategory.Remove(categoryName);
            usavedChanges = true;
            GUIUtility.ExitGUI();
        }

        void CloseCategory(string categoryName) { if (data != null && data[categoryName].Count > 0) { foreach (var item in data[categoryName]) { if (item != null && item.sp != null) item.sp.isExpanded = false; } } }

        bool isObjectValid(UnityEngine.Object obj, string typeName)
        {
            GameObject go = null;
            switch (typeName)
            {
                case "AudioClip": return obj.GetType() == typeof(AudioClip);
                case "GameObject": return obj.GetType() == typeof(GameObject);
                case "Material": return obj.GetType() == typeof(Material);
                case "Mesh": return obj.GetType() == typeof(Mesh);
                case "ParticleSystem": if (obj.GetType() != typeof(GameObject)) { return false; } go = (GameObject)obj; return go != null && go.GetComponent<ParticleSystem>() != null;
                case "RectTransform": if (obj.GetType() != typeof(GameObject)) { return false; } go = (GameObject)obj; return go != null && go.GetComponent<RectTransform>() != null;
                case "Sprite": if (obj.GetType() == typeof(Texture2D)) { TextureImporter ti = TextureImporter.GetAtPath(AssetDatabase.GetAssetPath(obj)) as TextureImporter; return ti.textureType == TextureImporterType.Sprite; } return false;
                case "Texture": if (obj.GetType() == typeof(Texture2D)) { TextureImporter ti = TextureImporter.GetAtPath(AssetDatabase.GetAssetPath(obj)) as TextureImporter; return ti.textureType != TextureImporterType.Sprite; } return false;
                case "TerrainData": return obj.GetType() == typeof(TerrainData);
                case "Transform": if (obj.GetType() != typeof(GameObject)) { return false; } go = (GameObject)obj; return go != null && go.GetComponent<RectTransform>() == null;
                case "Object": return true;
                default: return false;
            }
        }

        bool VariableNameAlreadyExistInTheDatabase(string vName)
        {
            foreach (var field in fields) { if (field.Name.Equals(vName)) return true; }
            foreach (var v in data[activeCategoryName]) { if (v.name.Equals(vName)) return true; }
            return false;
        }

        bool IsVariableNameValid(string vName) { foreach (var s in invalidVariableNames) { if (s.Equals(vName)) return false; } return true; }

        string GetVariableType(string t)
        {
            if (t.Contains("[]")) return "array";
            else if (t.Contains("System.Collections.Generic.List")) return "list";
            else return "variable";
        }

        string GetTypeName(string t)
        {
            if (t.Contains("[]")) t = t.Replace("[]", "");
            else if (t.Contains("System.Collections.Generic.List`1")) { t = t.Replace("System.Collections.Generic.List`1", ""); t = t.Replace("[", ""); t = t.Replace("]", ""); }

            switch (t)
            {
                case "UnityEngine.AnimationCurve": return "AnimationCurve";
                case "UnityEngine.AudioClip": return "AudioClip";
                case "System.Boolean": return "bool";
                case "UnityEngine.Color": return "Color";
                case "UnityEngine.Color32": return "Color32";
                case "System.Double": return "double";
                case "System.Single": return "float";
                case "UnityEngine.GameObject": return "GameObject";
                case "System.Int32": return "int";
                case "System.Int64": return "long";
                case "UnityEngine.Material": return "Material";
                case "UnityEngine.Mesh": return "Mesh";
                case "UnityEngine.Object": return "Object";
                case "UnityEngine.ParticleSystem": return "ParticleSystem";
                case "UnityEngine.Quaternion": return "Quaternion";
                case "UnityEngine.Rect": return "Rect";
                case "UnityEngine.RectTransform": return "RectTransform";
                case "UnityEngine.Sprite": return "Sprite";
                case "System.String": return "string";
                case "UnityEngine.TerrainData": return "TerrainData";
                case "UnityEngine.Transform": return "Transform";
                case "UnityEngine.Texture": return "Texture";
                case "UnityEngine.Vector2": return "Vector2";
                case "UnityEngine.Vector3": return "Vector3";
                case "UnityEngine.Vector4": return "Vector4";
                default: return "Object";
            }
        }

        /// <summary>
        /// Adds a new variable to the active category. If ok is TRUE it will consider that the user pressed OK, otherwise it will consider that the user pressed Cancel
        /// </summary>
        /// <param name="ok">TURE if OK button was pressed. FALSE if Cancel button was pressed</param>
        void AddVariable()
        {
            addVariableName = EzDataUtility.CleanString(addVariableName);
            if (string.IsNullOrEmpty(addVariableName))
            {
                EditorUtility.DisplayDialog("New variable", "Please enter a variable name!", "Ok");
                return;
            }

            if (!IsVariableNameValid(addVariableName))
            {
                EditorUtility.DisplayDialog("New variable", "You cannot add a new variable named: " + addVariableName, "Ok");
                return;
            }

            if (VariableNameAlreadyExistInTheDatabase(addVariableName))
            {
                EditorUtility.DisplayDialog("New variable", "There is another variable with the name '" + addVariableName + "' in the database. Try saving with another name or delete that one and try again.", "Ok");
                return;
            }

            data[activeCategoryName].Add(new Variable() { variableType = addVariableVariableType, typeName = addVariableTypeName, name = addVariableName.Trim() });
            dataHeight[activeCategoryName].Add(EditorGUIUtility.singleLineHeight + 2);
            AddVariableReset();
            GUIUtility.ExitGUI();
        }

        void AddVariableReset()
        {
            addVariable = false;
            activeCategoryName = string.Empty;
            addVariableVariableType = string.Empty;
            addVariableTypeName = string.Empty;
            addVariableName = string.Empty;
        }

        void DeleteVariable(string categoryName, int index)
        {
            data[categoryName].RemoveAt(index); dataHeight[categoryName].RemoveAt(index); usavedChanges = true; GUIUtility.ExitGUI();
        }

        void DrawVector4(SerializedProperty sp, Rect rect, float width, bool isArrayElement = false)
        {
            Vector4 v4 = sp.vector4Value; int arrayElementAdjustment = isArrayElement ? 2 : 0;
            EditorGUI.LabelField(new Rect(rect.x + width / 4 * 0, rect.y, 13, EditorGUIUtility.singleLineHeight), "X"); v4.x = EditorGUI.FloatField(new Rect(rect.x + width / 4 * 0 + 13, rect.y, width / 4 - 14 - arrayElementAdjustment, EditorGUIUtility.singleLineHeight), v4.x);
            EditorGUI.LabelField(new Rect(rect.x + width / 4 * 1 + 2 - arrayElementAdjustment, rect.y, 16, EditorGUIUtility.singleLineHeight), "Y"); v4.y = EditorGUI.FloatField(new Rect(rect.x + width / 4 * 1 + 14 - arrayElementAdjustment, rect.y, width / 4 - 16 - arrayElementAdjustment, EditorGUIUtility.singleLineHeight), v4.y);
            EditorGUI.LabelField(new Rect(rect.x + width / 4 * 2 + 2 - arrayElementAdjustment, rect.y, 16, EditorGUIUtility.singleLineHeight), "Z"); v4.z = EditorGUI.FloatField(new Rect(rect.x + width / 4 * 2 + 14 - arrayElementAdjustment, rect.y, width / 4 - 16 - arrayElementAdjustment, EditorGUIUtility.singleLineHeight), v4.z);
            EditorGUI.LabelField(new Rect(rect.x + width / 4 * 3 + 2 - arrayElementAdjustment, rect.y, 16, EditorGUIUtility.singleLineHeight), "W"); v4.w = EditorGUI.FloatField(new Rect(rect.x + width / 4 * 3 + 16 - arrayElementAdjustment, rect.y, width / 4 - 13 - arrayElementAdjustment, EditorGUIUtility.singleLineHeight), v4.w);
            sp.vector4Value = v4;
        }

        void DrawRect(SerializedProperty sp, Rect rect, float width, bool isArrayElement = false)
        {
            Rect rct = sp.rectValue; int arrayElementAdjustment = isArrayElement ? 2 : 0;
            EditorGUI.LabelField(new Rect(rect.x + width / 4 * 0, rect.y, 13, EditorGUIUtility.singleLineHeight), "X"); rct.x = EditorGUI.FloatField(new Rect(rect.x + width / 4 * 0 + 13, rect.y, width / 4 - 14 - arrayElementAdjustment, EditorGUIUtility.singleLineHeight), rct.x);
            EditorGUI.LabelField(new Rect(rect.x + width / 4 * 1 + 2 - arrayElementAdjustment, rect.y, 16, EditorGUIUtility.singleLineHeight), "Y"); rct.y = EditorGUI.FloatField(new Rect(rect.x + width / 4 * 1 + 14 - arrayElementAdjustment, rect.y, width / 4 - 16 - arrayElementAdjustment, EditorGUIUtility.singleLineHeight), rct.y);
            EditorGUI.LabelField(new Rect(rect.x + width / 4 * 2 + 2 - arrayElementAdjustment, rect.y, 16, EditorGUIUtility.singleLineHeight), "W"); rct.width = EditorGUI.FloatField(new Rect(rect.x + width / 4 * 2 + 14 - arrayElementAdjustment, rect.y, width / 4 - 16 - arrayElementAdjustment, EditorGUIUtility.singleLineHeight), rct.width);
            EditorGUI.LabelField(new Rect(rect.x + width / 4 * 3 + 2 - arrayElementAdjustment, rect.y, 16, EditorGUIUtility.singleLineHeight), "H"); rct.height = EditorGUI.FloatField(new Rect(rect.x + width / 4 * 3 + 16 - arrayElementAdjustment, rect.y, width / 4 - 13 - arrayElementAdjustment, EditorGUIUtility.singleLineHeight), rct.height);
            sp.rectValue = rct;
        }

        void DrawQuaternion(SerializedProperty sp, Rect rect, float width, bool isArrayElement = false)
        {
            Quaternion quat = sp.quaternionValue; int arrayElementAdjustment = isArrayElement ? 2 : 0;
            EditorGUI.LabelField(new Rect(rect.x + width / 4 * 0, rect.y, 13, EditorGUIUtility.singleLineHeight), "X"); quat.x = EditorGUI.FloatField(new Rect(rect.x + width / 4 * 0 + 13, rect.y, width / 4 - 14 - arrayElementAdjustment, EditorGUIUtility.singleLineHeight), quat.x);
            EditorGUI.LabelField(new Rect(rect.x + width / 4 * 1 + 2 - arrayElementAdjustment, rect.y, 16, EditorGUIUtility.singleLineHeight), "Y"); quat.y = EditorGUI.FloatField(new Rect(rect.x + width / 4 * 1 + 14 - arrayElementAdjustment, rect.y, width / 4 - 16 - arrayElementAdjustment, EditorGUIUtility.singleLineHeight), quat.y);
            EditorGUI.LabelField(new Rect(rect.x + width / 4 * 2 + 2 - arrayElementAdjustment, rect.y, 16, EditorGUIUtility.singleLineHeight), "Z"); quat.z = EditorGUI.FloatField(new Rect(rect.x + width / 4 * 2 + 14 - arrayElementAdjustment, rect.y, width / 4 - 16 - arrayElementAdjustment, EditorGUIUtility.singleLineHeight), quat.z);
            EditorGUI.LabelField(new Rect(rect.x + width / 4 * 3 + 2 - arrayElementAdjustment, rect.y, 16, EditorGUIUtility.singleLineHeight), "W"); quat.w = EditorGUI.FloatField(new Rect(rect.x + width / 4 * 3 + 16 - arrayElementAdjustment, rect.y, width / 4 - 13 - arrayElementAdjustment, EditorGUIUtility.singleLineHeight), quat.w);
            sp.quaternionValue = quat;
        }
    }
}
