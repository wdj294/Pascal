// Copyright © Pixel Crushers. All rights reserved.

using System;
using UnityEngine;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// This subclass of QuestList provides facilities to show the list in a QuestJournalUI.
    /// </summary>
    [AddComponentMenu("")] // Use wrapper instead.
    public class QuestJournal : IdentifiableQuestListContainer, IMessageHandler
    {

        [Tooltip("The Quest Journal UI to use. If unassigned, use the QuestMachine's default UI.")]
        [SerializeField]
        [IQuestJournalUIInspectorField]
        private UnityEngine.Object m_questJournalUI;

        [Tooltip("The Quest HUD to use. If unassigned, use the QuestMachine's default HUD.")]
        [SerializeField]
        [IQuestHUDInspectorField]
        private UnityEngine.Object m_questHUD;

        [Tooltip("Keep completed quests in the journal.")]
        [SerializeField]
        private bool m_rememberCompletedQuests;

        /// <summary>
        /// The quest journal UI to use. If not set, defaults to the QuestMachine's default UI.
        /// </summary>
        public IQuestJournalUI questJournalUI
        {
            get { return (m_questJournalUI != null) ? m_questJournalUI as IQuestJournalUI : QuestMachine.defaultQuestJournalUI; }
            set { m_questJournalUI = value as UnityEngine.Object; }
        }

        /// <summary>
        /// The quest HUD to use. If not set, defaults to the QuestMachine's default UI.
        /// </summary>
        public IQuestHUD questHUD
        {
            get { return (m_questHUD != null) ? m_questHUD as IQuestHUD : QuestMachine.defaultQuestHUD; }
            set { m_questHUD = value as UnityEngine.Object; }
        }

        /// <summary>
        /// Keep completed quests in the journal.
        /// </summary>
        public bool rememberCompletedQuests
        {
            get { return m_rememberCompletedQuests; }
            set { m_rememberCompletedQuests = value; }
        }

        public override void Reset()
        {
            base.Reset();
            includeInSavedGameData = true;
        }

        public override void OnEnable()
        {
            base.OnEnable();
            MessageSystem.AddListener(this, QuestMachineMessages.QuestStateChangedMessage, string.Empty);
            MessageSystem.AddListener(this, QuestMachineMessages.QuestCounterChangedMessage, string.Empty);
            MessageSystem.AddListener(this, QuestMachineMessages.RefreshUIsMessage, string.Empty);
        }

        public override void OnDisable()
        {
            base.OnDisable();
            MessageSystem.RemoveListener(this);
        }

        public void OnMessage(MessageArgs messageArgs)
        {
            if (string.Equals(messageArgs.message, QuestMachineMessages.QuestStateChangedMessage) ||
                string.Equals(messageArgs.message, QuestMachineMessages.QuestCounterChangedMessage) ||
                string.Equals(messageArgs.message, QuestMachineMessages.RefreshUIsMessage))
            {
                RepaintUIs();
            }
        }
        /// <summary>
        /// Show the quest journal.
        /// </summary>
        public virtual void ShowJournalUI()
        {
            if (questJournalUI != null) questJournalUI.Show(this);
        }

        /// <summary>
        /// Hide the quest journal.
        /// </summary>
        public virtual void HideJournalUI()
        {
            if (questJournalUI != null) questJournalUI.Hide();
        }

        /// <summary>
        /// Toggle visibility of the journal.
        /// </summary>
        public virtual void ToggleJournalUI()
        {
            if (questJournalUI != null) questJournalUI.Toggle(this);
        }

        public virtual void AbandonQuest(Quest quest)
        {
            if (quest == null || !quest.isAbandonable) return;
            if (quest.rememberIfAbandoned)
            {
                quest.SetState(QuestState.Abandoned);
            }
            else
            {
                DeleteQuest(quest.id);
            }
            QuestMachineMessages.QuestAbandoned(this, quest.id);
            if (questJournalUI != null) questJournalUI.SelectQuest(null);
            RepaintUIs();
        }

        public override void ApplyData(string data)
        {
            base.ApplyData(data);
            RepaintUIs();
        }

        public void RepaintUIs()
        {
            if (questJournalUI != null) questJournalUI.Repaint(this);
            if (questHUD != null) questHUD.Repaint(this);
        }

    }
}