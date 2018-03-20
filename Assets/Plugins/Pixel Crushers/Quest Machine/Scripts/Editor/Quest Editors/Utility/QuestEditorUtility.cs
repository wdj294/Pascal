// Copyright © Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Collections.Generic;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Utility methods used by the custom editors.
    /// </summary>
    public static class QuestEditorUtility
    {

        #region Class Types

        public static System.Type GetWrapperType(System.Type type)
        {
            try
            {
                if (string.Equals(type.Namespace, "PixelCrushers.QuestMachine"))
                {
                    var wrapperName = "PixelCrushers.QuestMachine.Wrappers." + type.Name;
                    var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(p => !(p.ManifestModule is System.Reflection.Emit.ModuleBuilder)); // Exclude dynamic assemblies.
                    var wrapperList = (from domainAssembly in assemblies
                                       from assemblyType in domainAssembly.GetExportedTypes()
                                       where string.Equals(assemblyType.FullName, wrapperName)
                                       select assemblyType).ToArray();
                    if (wrapperList.Length > 0) return wrapperList[0];
                }
            }
            catch (NotSupportedException)
            {
                // If an assembly complains, ignore it and move on.
            }
            return null;
        }

        public static bool HasWrapperType(System.Type type)
        {
            return GetWrapperType(type) != null;
        }

        public static List<Type> GetSubtypes<T>() where T : class
        {
            var subtypes = TypeUtility.GetSubtypes<T>();
            subtypes.RemoveAll(x => HasWrapperType(x));
            return subtypes;
        }

        #endregion

        #region Editor GUI

        public static bool EditorGUILayoutFoldout(string label, string tooltip, bool foldout, bool topLevel = true)
        {
            try
            {
                GUILayout.BeginHorizontal();
                GUI.backgroundColor = foldout ? QuestEditorStyles.collapsibleHeaderOpenColor : QuestEditorStyles.collapsibleHeaderClosedColor;
                var text = topLevel ? ("<b>" + label + "</b>") : label;
                var guiContent = new GUIContent((foldout ? QuestEditorStyles.FoldoutOpenArrow : QuestEditorStyles.FoldoutClosedArrow) + text, tooltip);
                var guiStyle = topLevel ? QuestEditorStyles.CollapsibleHeaderButtonStyleName : QuestEditorStyles.CollapsibleSubheaderButtonStyleName;
                if (!GUILayout.Toggle(true, guiContent, guiStyle))
                {
                    foldout = !foldout;
                }
                GUI.backgroundColor = Color.white;
            }
            finally
            {
                EditorGUILayout.EndHorizontal();
            }
            return foldout;
        }

        public static void EditorGUILayoutVerticalSpace(float pixels)
        {
            EditorGUILayout.BeginVertical();
            GUILayout.Space(pixels);
            EditorGUILayout.EndVertical();
        }

        public static void EditorGUILayoutBeginGroup()
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.BeginHorizontal(QuestEditorStyles.GroupBoxStyle, GUILayout.MinHeight(EditorGUIUtility.singleLineHeight));
            GUILayout.BeginVertical();
            GUILayout.Space(2);
        }

        public static void EditorGUILayoutEndGroup()
        {
            try
            {
                GUILayout.Space(3);
                GUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
                GUILayout.EndHorizontal();
                GUILayout.Space(3);
            }
            catch (ArgumentException)
            {
                // If Unity opens a popup bwindow such as a color picker, it raises an exception.
            }
        }

        public static void EditorGUILayoutBeginIndent()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(18);
            EditorGUILayout.BeginVertical();
        }

        public static void EditorGUILayoutEndIndent()
        {
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }

        public static void RemoveReorderableListElementWithoutLeavingNull(UnityEditorInternal.ReorderableList list)
        {
            // If an objectReferenceValue is assigned to the list element, the default DoRemoveButton method
            // will only unassign it but it won't actually delete the element. To cleanly delete the element,
            // this method first unassigns the objectReferenceValue, then calls DoRemoveButton.
            if (!(list != null && 0 <= list.index && list.index < list.serializedProperty.arraySize)) return;
            var element = list.serializedProperty.GetArrayElementAtIndex(list.index);
            if (element.propertyType == SerializedPropertyType.ObjectReference)
            {
                element.objectReferenceValue = null;
            }
            UnityEditorInternal.ReorderableList.defaultBehaviours.DoRemoveButton(list);
        }
        
        public static string[] GetCounterNames()
        {
            if (QuestEditorWindow.selectedQuest == null || QuestEditorWindow.selectedQuest.counterList == null) return null;
            var nameList = new string[QuestEditorWindow.selectedQuest.counterList.Count];
            for (int i = 0; i < QuestEditorWindow.selectedQuest.counterList.Count; i++)
            {
                var counter = QuestEditorWindow.selectedQuest.counterList[i];
                nameList[i] = (counter != null) ? StringField.GetStringValue(counter.name) : "<unassigned>";
            }
            return nameList;
        }

        public static void EditorGUILayoutCounterNamePopup(SerializedProperty counterIndexProperty, string[] nameList)
        {
            if (counterIndexProperty == null) return;
            if (nameList != null)
            {
                counterIndexProperty.intValue = EditorGUILayout.Popup("Counter", counterIndexProperty.intValue, nameList);
            }
            else
            {
                EditorGUILayout.PropertyField(counterIndexProperty);
            }
        }

        public static void EditorGUICounterNamePopup(Rect rect, SerializedProperty counterIndexProperty, string[] nameList)
        {
            if (counterIndexProperty == null) return;
            if (nameList != null)
            {
                counterIndexProperty.intValue = EditorGUI.Popup(rect, counterIndexProperty.intValue, nameList);
            }
            else
            {
                EditorGUI.PropertyField(rect, counterIndexProperty, GUIContent.none, true);
            }
        }

        public static void SetMessageParticipantID(SerializedProperty senderSpecifierProperty, SerializedProperty senderIDProperty)
        {
            if (senderSpecifierProperty == null || senderIDProperty == null) return;
            switch ((QuestMessageParticipant)senderSpecifierProperty.enumValueIndex)
            {
                case QuestMessageParticipant.Quester:
                    SetTextFieldProperty(senderIDProperty, QuestMachineTags.QUESTERID);
                    break;
                case QuestMessageParticipant.QuestGiver:
                    SetTextFieldProperty(senderIDProperty, QuestMachineTags.QUESTGIVERID);
                    break;
                case QuestMessageParticipant.Any:
                    SetTextFieldProperty(senderIDProperty, string.Empty);
                    break;
            }
        }

        public static void SetTextFieldProperty(SerializedProperty textFieldProperty, string value)
        {
            if (textFieldProperty == null) return;
            var textProperty = textFieldProperty.FindPropertyRelative("m_text");
            var stringAssetProperty = textFieldProperty.FindPropertyRelative("m_stringAsset");
            var textTableProperty = textFieldProperty.FindPropertyRelative("m_textTable");
            if (textProperty != null) textProperty.stringValue = value;
            if (stringAssetProperty != null) stringAssetProperty.objectReferenceValue = null;
            if (textTableProperty != null) textTableProperty.objectReferenceValue = null;
        }

        #endregion

    }

}