// Copyright © Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEditor;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Custom inspector for QuestJournal.
    /// </summary>
    [CustomEditor(typeof(QuestJournal), true)]
    public class QuestJournalEditor : QuestListContainerEditor
    {

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawID("Quester identity.");
            var questJournalUIProperty = serializedObject.FindProperty("m_questJournalUI");
            var questHUDProperty = serializedObject.FindProperty("m_questHUD");
            var rememberProperty = serializedObject.FindProperty("m_rememberCompletedQuests");
            if (questJournalUIProperty != null) IQuestDialogueUIInspectorFieldAttributeDrawer.DoLayout(questJournalUIProperty,
                new GUIContent("Quest Journal UI", "The Quest Journal UI to use. If unassigned, use the default journal UI."));
            if (questHUDProperty != null) IQuestHUDInspectorFieldAttributeDrawer.DoLayout(questHUDProperty,
                new GUIContent("Quest HUD", "The Quest HUD to use. If unassigned, use the default HUD."));
            if (rememberProperty != null) EditorGUILayout.PropertyField(rememberProperty);

            serializedObject.ApplyModifiedProperties();
            base.OnInspectorGUI();
        }
    }
}
