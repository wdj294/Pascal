// Copyright © Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEditor;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Custom inspector for ButtonQuestUIContent assets.
    /// </summary>
    [CustomEditor(typeof(ButtonQuestContent), true)]
    public class ButtonQuestUIContentEditor : IconQuestUIContentEditor
    {

        private QuestActionListInspectorGUI m_actionListDrawer = null;

        protected override void OnEnable()
        {
            base.OnEnable();
            if (m_actionListDrawer == null) m_actionListDrawer = new QuestActionListInspectorGUI(new GUIContent("Actions", "Actions that run when this button is clicked."));
        }

        protected override void Draw()
        {
            base.Draw();
            var actionListProperty = serializedObject.FindProperty("m_actionList");
            UnityEngine.Assertions.Assert.IsNotNull(actionListProperty, "Quest Machine: Internal error - m_actionList is null.");
            if (actionListProperty == null) return;
            m_actionListDrawer.Draw(actionListProperty);
        }

    }
}
