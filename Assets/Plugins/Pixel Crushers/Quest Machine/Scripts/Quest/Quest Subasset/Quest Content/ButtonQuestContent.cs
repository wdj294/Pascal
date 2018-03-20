// Copyright © Pixel Crushers. All rights reserved.

using UnityEngine;
using System;
using System.Collections.Generic;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Button UI content that executes actions when clicked.
    /// </summary>
    public class ButtonQuestContent : IconQuestContent
    {

        [Tooltip("The actions to run when the button is clicked.")]
        [SerializeField]
        private List<QuestAction> m_actionList = new List<QuestAction>();

        [HideInInspector]
        [SerializeField]
        public QuestActionProxyContainer m_actionsProxy;

        /// <summary>
        /// The actions to run when the button is clicked.
        /// </summary>
        public List<QuestAction> actionList
        {
            get { return m_actionList; }
            set { m_actionList = value; }
        }

        public bool interactable { get { return actionList != null && actionList.Count > 0; } }

        public override string GetEditorName()
        {
            return !StringField.IsNullOrEmpty(caption) ? ("Button: " + ((count > 1) ? count + " " : string.Empty) + caption)
                : ((image != null) ? "Button: " + image.name : "Button");
        }

        public override void SetRuntimeReferences(Quest quest, QuestNode questNode)
        {
            base.SetRuntimeReferences(quest, questNode);
            if (actionList == null) return;
            for (int i = 0; i < actionList.Count; i++)
            {
                if (actionList[i] == null) continue;
                actionList[i].SetRuntimeReferences(quest, questNode);
            }
        }

        public override void OnBeforeProxySerialization()
        {
            base.OnBeforeProxySerialization();
            m_actionsProxy = new QuestActionProxyContainer(actionList);
        }

        public override void OnAfterProxyDeserialization()
        {
            base.OnAfterProxyDeserialization();
            actionList = QuestActionProxyContainer.CreateList(m_actionsProxy);
            // After deserializing, free proxy memory:
            m_actionsProxy = null;
        }

        public override void CloneSubassetsInto(QuestSubasset copy)
        {
            base.CloneSubassetsInto(copy);
            var copyButton = copy as ButtonQuestContent;
            if (copyButton == null) return;
            copyButton.actionList = CloneList(actionList);

        }

    }

}
