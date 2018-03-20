// Copyright © Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Editor GUI for a list of QuestCounter objects.
    /// </summary>
    public class QuestCounterListInspectorGUI
    {

        private ReorderableList m_list = null;
        private SerializedProperty m_selectedCounter = null;

        public void Draw(SerializedObject serializedObject, SerializedProperty countersProperty)
        {
            if (countersProperty == null) return;
            if (m_list == null) SetupReorderableList(countersProperty);
            if (m_list != null) m_list.DoLayoutList();
            DrawSelectedUIContent();
        }

        private void SetupReorderableList(SerializedProperty countersProperty)
        {
            if (countersProperty == null) return;
            m_list = new ReorderableList(QuestEditorWindow.selectedQuestSerializedObject, countersProperty, true, true, true, true);
            m_list.drawHeaderCallback = OnDrawHeader;
            m_list.drawElementCallback = OnDrawElement;
            m_list.onSelectCallback = OnSelectElement;
            m_list.onRemoveCallback = OnRemoveElement;
            m_list.onAddCallback = OnAddElement;
            m_selectedCounter = null;
        }

        private void OnDrawHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, new GUIContent("Counters", "Counters used in this quest."));
        }

        private void OnDrawElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            if (!(0 <= index && index < m_list.serializedProperty.arraySize)) return;
            var element = m_list.serializedProperty.GetArrayElementAtIndex(index);
            if (element == null) return;
            var currentValueProperty = element.FindPropertyRelative("m_currentValue");
            var currentValue = (currentValueProperty != null) ? currentValueProperty.intValue : 0;
            EditorGUI.LabelField(rect, StringFieldDrawer.GetStringFieldValue(element.FindPropertyRelative("m_name")) + ": " + currentValue);
        }

        private void OnSelectElement(ReorderableList list)
        {
            var isIndexValid = (0 <= list.index && list.index < list.count);
            m_selectedCounter = isIndexValid ? list.serializedProperty.GetArrayElementAtIndex(list.index) : null;
        }

        private void OnRemoveElement(ReorderableList list)
        {
            ReorderableList.defaultBehaviours.DoRemoveButton(list);
            OnSelectElement(list);
        }

        private void OnAddElement(ReorderableList list)
        {
            ReorderableList.defaultBehaviours.DoAddButton(list);
            var newCounterProperty = list.serializedProperty.GetArrayElementAtIndex(list.index);
            if (newCounterProperty != null)
            {
                var counterNameProperty = newCounterProperty.FindPropertyRelative("m_name");
                if (counterNameProperty != null)
                {
                    StringFieldDrawer.SetStringFieldValue(counterNameProperty, string.Empty);
                }
                var messageEventListProperty = newCounterProperty.FindPropertyRelative("m_messageEventList");
                if (messageEventListProperty != null)
                {
                    messageEventListProperty.ClearArray();
                }
            }
            OnSelectElement(list);
        }

        private void DrawSelectedUIContent()
        {
            if (m_selectedCounter == null) return;
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(m_selectedCounter);
            EditorGUI.indentLevel--;
        }

    }
}
