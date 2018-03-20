// Copyright © Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEditor;

namespace PixelCrushers.QuestMachine
{

    [CustomEditor(typeof(QuestGeneratorEntity), true)]
    public class QuestGeneratorEntityEditor : Editor
    {

        #if DEBUG_QUEST_EDITOR
        private static bool defaultInspectorFoldout = false;
        #endif

        private QuestContentListInspectorGUI rewardsContentGUI { get; set; }

        private void OnEnable()
        {
            rewardsContentGUI = new QuestContentListInspectorGUI(new GUIContent("Dialogue for Rewards Section", "Show this UI content above the list of rewards offered for a quest."), QuestContentCategory.Dialogue);
        }

        public override void OnInspectorGUI()
        {
            #if DEBUG_QUEST_EDITOR
            defaultInspectorFoldout = EditorGUILayout.Foldout(defaultInspectorFoldout, "Default Inspector");
            if (defaultInspectorFoldout) base.OnInspectorGUI();
            #endif

            serializedObject.Update();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_entityType"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_questGroup"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_domainType"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_domains"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_generateQuestOnStart"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_maxQuestsToGenerate"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_requireReturnToComplete"), true);
            rewardsContentGUI.Draw(serializedObject, serializedObject.FindProperty("m_rewardsUIContents"), false);
            serializedObject.ApplyModifiedProperties();

            if (Application.isPlaying)
            {
                if (GUILayout.Button("Generate Quest"))
                {
                    (target as QuestGeneratorEntity).GenerateQuest();
                }
            }
        }

    }

}
