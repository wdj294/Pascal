// Copyright © Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Custom inspector for QuestDatabase.
    /// </summary>
    [CustomEditor(typeof(QuestDatabase), true)]
    public class QuestDatabaseEditor : Editor
    {

        private ReorderableList questReorderableList { get; set; }

        protected virtual void OnEnable()
        {
            Undo.undoRedoPerformed += RepaintEditorWindow;
            QuestEditorWindow.currentEditor = this;
            questReorderableList = new ReorderableList(serializedObject, serializedObject.FindProperty("m_questAssets"), true, true, true, true);
            questReorderableList.drawHeaderCallback = OnDrawHeader;
            questReorderableList.onSelectCallback = OnChangeSelection;
            questReorderableList.onAddCallback = OnAddElement;
            questReorderableList.onRemoveCallback = OnRemoveElement;
            questReorderableList.drawElementCallback = OnDrawElement;
            questReorderableList.onReorderCallback = OnReorder;
        }

        protected virtual void OnDisable()
        {
            Undo.undoRedoPerformed -= RepaintEditorWindow;
            QuestEditorWindow.currentEditor = null;
        }

        protected void RepaintEditorWindow()
        {
            QuestEditorWindow.RepaintNow();
        }

        public override void OnInspectorGUI()
        {
#if DEBUG_QUEST_EDITOR
            var key = "PixelCrushers.QuestMachine.EditorPrefsDebug.DefaultInspectorFoldout";
            var foldout = EditorPrefs.GetBool(key);
            var newFoldout = EditorGUILayout.Foldout(foldout, "Default Inspector");
            if (newFoldout != foldout) EditorPrefs.SetBool(key, newFoldout);
            if (newFoldout) base.OnInspectorGUI();
#endif

            serializedObject.Update();
            DrawDescription();
            DrawQuestList();
            serializedObject.ApplyModifiedProperties();
            DrawGetAllInSceneButton();
        }

        private void DrawDescription()
        {
            var descriptionProperty = serializedObject.FindProperty("m_description");
            if (descriptionProperty == null) return;
            EditorGUILayout.PropertyField(descriptionProperty);
        }

        private void DrawQuestList()
        {
            if (!(0 <= questReorderableList.index && questReorderableList.index <= questReorderableList.count))
            {
                questReorderableList.index = 0;
                SetQuestInEditorWindow(questReorderableList.index);
            }

            questReorderableList.DoLayoutList();
        }

        private void OnDrawHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, "Quest Assets");
        }

        private void OnDrawElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            if (!(0 <= index && index < questReorderableList.serializedProperty.arraySize)) return;
            var buttonWidth = 48f;
            var questRect = new Rect(rect.x, rect.y + 1, rect.width - buttonWidth - 2, EditorGUIUtility.singleLineHeight);
            var buttonRect = new Rect(rect.x + rect.width - buttonWidth, rect.y + 1, buttonWidth, EditorGUIUtility.singleLineHeight);
            var questProperty = questReorderableList.serializedProperty.GetArrayElementAtIndex(index);
            var isQuestAssigned = questProperty.objectReferenceValue != null;
            var buttonGUIContent = isQuestAssigned ? new GUIContent("Edit", "Edit in Quest Editor window.") : new GUIContent("New", "Create new quest asset in this slot.");
            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(questRect, questProperty, GUIContent.none, false);
            if (EditorGUI.EndChangeCheck()) SetQuestInEditorWindow(index);
            if (GUI.Button(buttonRect, buttonGUIContent))
            {
                if (!isQuestAssigned)
                {
                    questProperty.objectReferenceValue = QuestEditorAssetUtility.CreateNewQuestAssetFromDialog();
                }
                QuestEditorWindow.ShowWindow();
                SetQuestInEditorWindow(index);
                questReorderableList.index = index;
            }
        }

        private void OnReorder(ReorderableList list)
        {
            if (QuestEditorWindow.instance == null) return;
            SetQuestInEditorWindow(list.index);
        }

        private void OnChangeSelection(ReorderableList list)
        {
            SetQuestInEditorWindow(list.index);
        }

        private void OnRemoveElement(ReorderableList list)
        {
            var element = (0 <= list.index && list.index < list.count) ? list.serializedProperty.GetArrayElementAtIndex(list.index) : null;
            var quest = (element != null) ? (element.objectReferenceValue as Quest) : null;
            var permanentlyDelete = false;
            if (quest != null)
            {
                var option = EditorUtility.DisplayDialogComplex("Remove Quest",
                    "Do you want to remove the reference to '" + quest.title.value + "' from this database or permanently delete the quest asset from your whole project?",
                    "Remove Reference", "Permanently Delete", "Cancel");
                if (option == 2) return; // Cancel.
                permanentlyDelete = (option == 1);
                ReorderableList.defaultBehaviours.DoRemoveButton(list);
                if (permanentlyDelete)
                {
                    AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(quest));
                }
            }
            ReorderableList.defaultBehaviours.DoRemoveButton(list);
            OnChangeSelection(list);
        }

        private void OnAddElement(ReorderableList list)
        {
            ReorderableList.defaultBehaviours.DoAddButton(list);
            var newIndex = list.serializedProperty.arraySize - 1;
            var element = list.serializedProperty.GetArrayElementAtIndex(newIndex);
            if (element != null) element.objectReferenceValue = null;
        }

        private void SetQuestInEditorWindow(int questListIndex)
        {
            if (!QuestEditorWindow.isOpen) return;
            serializedObject.ApplyModifiedProperties();
            var questDatabase = target as QuestDatabase;
            if (questDatabase == null) return;
            if (!(0 <= questListIndex && questListIndex < questDatabase.questAssets.Count)) return;
            QuestEditorWindow.ShowWindow();
            var quest = questDatabase.questAssets[questListIndex];
            QuestEditorWindow.instance.SelectQuest(quest);
        }

        private void DrawGetAllInSceneButton()
        {
            try
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Add All In Scene", GUILayout.Width(128)))
                {
                    if (EditorUtility.DisplayDialog("Add All In Scene", "Add all quests assigned in the current scene?", "OK", "Cancel"))
                    {
                        AddAllInScene();
                    }
                }
            }
            finally
            {
                EditorGUILayout.EndHorizontal();
            }
        }

        private void AddAllInScene()
        {
            var database = target as QuestDatabase;
            foreach (var questListContainer in FindObjectsOfType<QuestListContainer>())
            {
                foreach (var quest in questListContainer.questList)
                {
                    if (quest.isAsset && !database.questAssets.Contains(quest))
                    {
                        database.questAssets.Add(quest);
                    }
                }
            }
        }

    }
}
