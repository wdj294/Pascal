// Copyright © Pixel Crushers. All rights reserved.

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Unity UI implementation of QuestDialogueUI.
    /// </summary>
    [AddComponentMenu("")] // Use wrapper.
    public class UnityUIQuestDialogueUI : UnityUIBaseUI, IQuestDialogueUI
    {

        #region Serialized Fields

        [SerializeField]
        private UnityEngine.UI.Button m_closeButton;
        [SerializeField]
        private UnityEngine.UI.Button m_backButton;
        [SerializeField]
        private UnityEngine.UI.Button m_acceptButton;
        [SerializeField]
        private UnityEngine.UI.Button m_declineButton;
        [SerializeField]
        private RectTransform m_contentContainer;

        [Header("UI Templates")]

        [SerializeField]
        private UnityUITextTemplate m_headingTemplate;
        [SerializeField]
        private UnityUITextTemplate[] m_subheadingTemplates;
        [SerializeField]
        private UnityUITextTemplate m_bodyTemplate;
        [SerializeField]
        private UnityUIIconListTemplate m_iconListTemplate;
        [SerializeField]
        private UnityUIButtonListTemplate m_buttonListTemplate;

        #endregion

        #region Accessor Properties for Serialized Fields

        public UnityEngine.UI.Button closeButton
        {
            get { return m_closeButton; }
            set { m_closeButton = value; }
        }
        public UnityEngine.UI.Button backButton
        {
            get { return m_backButton; }
            set { m_backButton = value; }
        }
        public UnityEngine.UI.Button acceptButton
        {
            get { return m_acceptButton; }
            set { m_acceptButton = value; }
        }
        public UnityEngine.UI.Button declineButton
        {
            get { return m_declineButton; }
            set { m_declineButton = value; }
        }
        public RectTransform contentContainer
        {
            get { return m_contentContainer; }
            set { m_contentContainer = value; }
        }

        public UnityUITextTemplate headingTemplate
        {
            get { return m_headingTemplate; }
            set { m_headingTemplate = value; }
        }

        public UnityUITextTemplate[] subheadingTemplates
        {
            get { return m_subheadingTemplates; }
            set { m_subheadingTemplates = value; }
        }
        public UnityUITextTemplate bodyTemplate
        {
            get { return m_bodyTemplate; }
            set { m_bodyTemplate = value; }
        }
        public UnityUIIconListTemplate iconListTemplate
        {
            get { return m_iconListTemplate; }
            set { m_iconListTemplate = value; }
        }
        public UnityUIButtonListTemplate buttonListTemplate
        {
            get { return m_buttonListTemplate; }
            set { m_buttonListTemplate = value; }
        }

        #endregion

        #region Runtime Properties

        protected UnityUIInstancedContentManager contentManager { get; set; }
        protected override RectTransform currentContentContainer { get { return contentContainer; } }
        protected override UnityUIInstancedContentManager currentContentManager { get { return contentManager; } }
        protected override UnityUITextTemplate currentHeadingTemplate { get { return headingTemplate; } }
        protected override UnityUITextTemplate[] currentSubheadingTemplates { get { return subheadingTemplates; } }
        protected override UnityUITextTemplate currentBodyTemplate { get { return bodyTemplate; } }
        protected override UnityUIIconListTemplate currentIconListTemplate { get { return iconListTemplate; } }
        protected override UnityUIButtonListTemplate currentButtonListTemplate { get { return buttonListTemplate; } }

        protected Quest selectedQuest { get; set; }
        protected QuestParameterDelegate acceptHandler { get; set; }
        protected QuestParameterDelegate declineHandler { get; set; }
        protected Coroutine selectCoroutine { get; set; }

        #endregion

        protected override void Awake()
        {
            base.Awake();
            contentManager = new UnityUIInstancedContentManager();
            if (contentContainer == null && Debug.isDebugBuild) Debug.LogError("Quest Machine: Content Container is unassigned.", this);
        }

        public virtual void ShowContents(QuestParticipantTextInfo speaker, List<QuestContent> contents)
        {
            Show();
            mainPanel.gameObject.SetActive(true);
            SetContents(speaker, contents);
            SetControlButtons(true, false, false);
        }

        protected virtual void SetControlButtons(bool enableClose, bool enableBack, bool enableAcceptDecline)
        {
            closeButton.gameObject.SetActive(enableClose);
            backButton.gameObject.SetActive(enableBack);
            acceptButton.gameObject.SetActive(enableAcceptDecline);
            declineButton.gameObject.SetActive(enableAcceptDecline);
            if (InputDeviceManager.autoFocus)
            {
                var selectable = enableAcceptDecline ? declineButton
                    : (enableBack ? backButton : closeButton);
                if (selectCoroutine != null) StopCoroutine(selectCoroutine);
                selectCoroutine = StartCoroutine(SelectAfterOneFrame(selectable));
            }
            RefreshNavigableSelectables();
        }

        protected IEnumerator SelectAfterOneFrame(UnityEngine.UI.Selectable selectable)
        {
            yield return null;
            if (selectable != null)
            {
                selectable.Select();
            }
            selectCoroutine = null;
        }

        public virtual void ShowOfferConditionsUnmet(QuestParticipantTextInfo speaker, List<QuestContent> contents, List<Quest> quests)
        {
            ShowContents(speaker, contents);
            //--- Don't show the unofferable quests:
            // ShowQuestList(speaker, null, null, quests, null, null); 

        }

        public virtual void ShowOfferQuest(QuestParticipantTextInfo speaker, Quest quest, QuestParameterDelegate acceptHandler, QuestParameterDelegate declineHandler)
        {
            selectedQuest = quest;
            this.acceptHandler = acceptHandler;
            this.declineHandler = declineHandler;
            ShowContents(speaker, quest.offerContentList);
            SetControlButtons(false, false, true);
        }

        public void AcceptQuest()
        {
            acceptHandler(selectedQuest);
        }

        public void DeclineQuest()
        {
            declineHandler(selectedQuest);
        }

        public virtual void ShowActiveQuest(QuestParticipantTextInfo speaker, Quest quest, QuestParameterDelegate continueHandler, QuestParameterDelegate backHandler)
        {
            selectedQuest = quest;
            ShowContents(speaker, quest.GetContentList(QuestContentCategory.Dialogue, speaker));
            SetControlButtons(true, false, false);
        }

        public virtual void ShowCompletedQuest(QuestParticipantTextInfo speaker, List<Quest> quests)
        {
            if (quests == null || quests.Count == 0) return;
            var quest = quests[0];
            ShowContents(speaker, quest.GetContentList(QuestContentCategory.Dialogue));
            SetControlButtons(true, false, false);
        }

        public virtual void ShowQuestList(QuestParticipantTextInfo speaker, List<QuestContent> activeQuestsContents, List<Quest> activeQuests,
            List<QuestContent> offerableQuestsContents, List<Quest> offerableQuests, QuestParameterDelegate selectHandler)
        {
            ShowContents(speaker, null);
            SetControlButtons(true, false, false);
            if (activeQuests != null && activeQuests.Count > 0)
            {
                currentButtonList = null;
                AddQuestList(activeQuestsContents, activeQuests, selectHandler);
            }
            if (offerableQuests != null && offerableQuests.Count > 0)
            {
                currentButtonList = null;
                AddQuestList(offerableQuestsContents, offerableQuests, selectHandler);
            }
        }

        public void AddQuestList(List<QuestContent> contents, List<Quest> quests, QuestParameterDelegate selectHandler)
        {
            AddContents(contents);
            if (quests == null) return;
            for (int i = 0; i < quests.Count; i++)
            {
                var quest = quests[i];
                if (quest == null) continue;
                PrepareButtonList();
                if (selectHandler != null)
                {
                    currentButtonList.AddButton(quest.icon, 1, StringField.GetStringValue(quest.title), delegate { selectHandler(quest); });
                }
                else
                {
                    currentButtonList.AddButton(quest.icon, 1, StringField.GetStringValue(quest.title), null);
                }
            }
        }

    }

}
