// Copyright © Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEditor;

namespace PixelCrushers.QuestMachine
{

    public class QuestTagsToTextTableWizard : ScriptableWizard
    {

        public TextTable textTable;

        public static void Open()
        {
            ScriptableWizard.DisplayWizard<QuestTagsToTextTableWizard>("Tags To Text Table", "Add Tags");
        }

        void OnWizardUpdate()
        {
            helpString = "Please select a Text Table.";
        }

        void OnWizardCreate()
        {
            QuestMachineTags.AddQuestTagsToTextTable(QuestEditorWindow.selectedQuest, textTable);
        }

    }
}