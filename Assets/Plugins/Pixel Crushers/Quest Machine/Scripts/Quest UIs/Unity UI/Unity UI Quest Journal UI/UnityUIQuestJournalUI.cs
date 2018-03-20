// Copyright © Pixel Crushers. All rights reserved.

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Unity UI implementation of Quest Journal UI.
    /// </summary>
    [AddComponentMenu("")] // Use wrapper.
    public class UnityUIQuestJournalUI : UnityUIBaseUI, IQuestJournalUI
    {

        #region Serialized Fields

        [Header("Selection Panel")]

        [SerializeField]
        private RectTransform m_questSelectionContentContainer;
        [SerializeField]
        private UnityUIFoldoutTemplate m_questGroupTemplate;
        [SerializeField]
        private UnityUIQuestNameButtonTemplate m_activeQuestNameTemplate;
        [SerializeField]
        private UnityUIQuestNameButtonTemplate m_completedQuestNameTemplate;

        [Header("Details Panel")]

        [SerializeField]
        private RectTransform m_questDetailsContentContainer;
        [SerializeField]
        private UnityUITextTemplate m_questHeadingTextTemplate;
        [SerializeField]
        private UnityUITextTemplate[] m_subheadingTemplates;
        [SerializeField]
        private UnityUITextTemplate m_questBodyTextTemplate;
        [SerializeField]
        private UnityUIIconListTemplate m_iconListTemplate;
        [SerializeField]
        private UnityUIButtonListTemplate m_buttonListTemplate;
        [SerializeField]
        private UnityUIButtonTemplate m_abandonButtonTemplate;

        [Header("Abandon Quest Panel")]

        [SerializeField]
        private UIPanel m_abandonQuestPanel;
        [SerializeField]
        private UnityEngine.UI.Text m_abandonQuestNameText;

        [Header("Misc")]

        [SerializeField]
        private bool m_showDialogueContentIfNoJournalContent = false;

        public enum SendMessageOnOpen { Never, Always, NotWhenUsingMouse }
        [SerializeField]
        private SendMessageOnOpen m_sendMessageOnOpen = SendMessageOnOpen.NotWhenUsingMouse;
        [SerializeField]
        private string m_openMessage = "Pause Player";
        [SerializeField]
        private string m_closeMessage = "Unpause Player";

        #endregion

        #region Accessor Properties for Serialized Fields

        public RectTransform questSelectionContentContainer
        {
            get { return m_questSelectionContentContainer; }
            set { m_questSelectionContentContainer = value; }
        }
        public UnityUIFoldoutTemplate questGroupTemplate
        {
            get { return m_questGroupTemplate; }
            set { m_questGroupTemplate = value; }
        }
        public UnityUIQuestNameButtonTemplate activeQuestNameTemplate
        {
            get { return m_activeQuestNameTemplate; }
            set { m_activeQuestNameTemplate = value; }
        }
        public UnityUIQuestNameButtonTemplate completedQuestNameTemplate
        {
            get { return m_completedQuestNameTemplate; }
            set { m_completedQuestNameTemplate = value; }
        }
        public RectTransform questDetailsContentContainer
        {
            get { return m_questDetailsContentContainer; }
            set { m_questDetailsContentContainer = value; }
        }
        public UnityUITextTemplate questHeadingTextTemplate
        {
            get { return m_questHeadingTextTemplate; }
            set { m_questHeadingTextTemplate = value; }
        }
        public UnityUITextTemplate[] subheadingTemplates
        {
            get { return m_subheadingTemplates; }
            set { m_subheadingTemplates = value; }
        }
        public UnityUITextTemplate questBodyTextTemplate
        {
            get { return m_questBodyTextTemplate; }
            set { m_questBodyTextTemplate = value; }
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
        public UnityUIButtonTemplate abandonButtonTemplate
        {
            get { return m_abandonButtonTemplate; }
            set { m_abandonButtonTemplate = value; }
        }
        public UIPanel abandonQuestPanel
        {
            get { return m_abandonQuestPanel; }
            set { m_abandonQuestPanel = value; }
        }
        public UnityEngine.UI.Text abandonQuestNameText
        {
            get { return m_abandonQuestNameText; }
            set { m_abandonQuestNameText = value; }
        }
        public bool showDialogueContentIfNoJournalContent
        {
            get { return m_showDialogueContentIfNoJournalContent; }
            set { m_showDialogueContentIfNoJournalContent = value; }
        }

        #endregion

        #region Runtime Properties

        protected UnityUIInstancedContentManager selectionPanelContentManager { get; set; }
        protected UnityUIInstancedContentManager detailsPanelContentManager { get; set; }
        protected bool isDrawingSelectionPanel { get; set; }

        protected override RectTransform currentContentContainer { get { return isDrawingSelectionPanel ? questSelectionContentContainer : questDetailsContentContainer; } }
        protected override UnityUIInstancedContentManager currentContentManager { get { return isDrawingSelectionPanel ? selectionPanelContentManager : detailsPanelContentManager; } }
        protected override UnityUITextTemplate currentHeadingTemplate { get { return isDrawingSelectionPanel ? null : questHeadingTextTemplate; } }
        protected override UnityUITextTemplate[] currentSubheadingTemplates { get { return subheadingTemplates; } }
        protected override UnityUITextTemplate currentBodyTemplate { get { return isDrawingSelectionPanel ? null : questBodyTextTemplate; } }
        protected override UnityUIIconListTemplate currentIconListTemplate { get { return isDrawingSelectionPanel ? null : iconListTemplate; } }
        protected override UnityUIButtonListTemplate currentButtonListTemplate { get { return isDrawingSelectionPanel ? null : buttonListTemplate; } }

        protected List<string> expandedGroupNames = new List<string>();
        protected Quest selectedQuest { get; set; }
        protected QuestJournal questJournal { get; set; }

        private Coroutine m_refreshCoroutine = null;

        private bool m_mustSendCloseMessage = false;

        #endregion

        protected override void Awake()
        {
            base.Awake();
            selectionPanelContentManager = new UnityUIInstancedContentManager();
            detailsPanelContentManager = new UnityUIInstancedContentManager();
        }

        protected override void InitializeTemplates()
        {
            if (Debug.isDebugBuild)
            {
                if (mainPanel == null) Debug.LogError("Quest Machine: Main Panel is unassigned.", this);
                if (questSelectionContentContainer == null) Debug.LogError("Quest Machine: Quest Selection Content Container is unassigned.", this);
                if (questGroupTemplate == null) Debug.LogError("Quest Machine: Quest Group Template is unassigned.", this);
                if (activeQuestNameTemplate == null) Debug.LogError("Quest Machine: Active Quest Name Template is unassigned.", this);
                if (completedQuestNameTemplate == null) Debug.LogError("Quest Machine: Completed Quest Name Template is unassigned.", this);
                if (questDetailsContentContainer == null) Debug.LogError("Quest Machine: Quest Details Content Container is unassigned.", this);
                if (questHeadingTextTemplate == null) Debug.LogError("Quest Machine: Quest Heading Text Template is unassigned.", this);
                if (questBodyTextTemplate == null) Debug.LogError("Quest Machine: Quest Body Text Template is unassigned.", this);
                if (iconListTemplate == null) Debug.LogError("Quest Machine: Icon List Template is unassigned.", this);
                if (iconListTemplate != null && iconListTemplate.iconTemplate == null) Debug.LogError("Quest Machine: Icon List Template's Icon Template is unassigned.", this);
                if (buttonListTemplate == null) Debug.LogError("Quest Machine: Button List Template is unassigned.", this);
                if (buttonListTemplate != null && buttonListTemplate.buttonTemplate == null) Debug.LogError("Quest Machine: Button List Template's Button Template is unassigned.", this);
                if (abandonQuestPanel == null) Debug.LogError("Quest Machine: Abandon Quest Panel is unassigned.", this);
                if (abandonQuestNameText == null) Debug.LogError("Quest Machine: Abandon Quest Name Text is unassigned.", this);
            }
            if (questGroupTemplate != null) questGroupTemplate.gameObject.SetActive(false);
            if (activeQuestNameTemplate != null) activeQuestNameTemplate.gameObject.SetActive(false);
            if (completedQuestNameTemplate!= null) completedQuestNameTemplate.gameObject.SetActive(false);
            if (questHeadingTextTemplate != null) questHeadingTextTemplate.gameObject.SetActive(false);
            if (questBodyTextTemplate != null) questBodyTextTemplate.gameObject.SetActive(false);
            if (iconListTemplate != null) iconListTemplate.gameObject.SetActive(false);
            if (buttonListTemplate != null) buttonListTemplate.gameObject.SetActive(false);
            if (abandonButtonTemplate != null) abandonButtonTemplate.gameObject.SetActive(false);
        }

        public virtual void Toggle(QuestJournal questJournal)
        {
            if (isVisible)
            {
                Hide();
            }
            else
            {
                Show(questJournal);
            }
        }

        /// <summary>
        /// True if the group is expanded in the UI.
        /// </summary>
        public virtual bool IsGroupExpanded(string groupName)
        {
            return expandedGroupNames.Contains(groupName);
        }

        /// <summary>
        /// Toggles whether a group is expanded or not.
        /// </summary>
        /// <param name="groupName">Group to toggle.</param>
        public virtual void ToggleGroup(string groupName)
        {
            if (IsGroupExpanded(groupName))
            {
                expandedGroupNames.Remove(groupName);
            }
            else
            {
                expandedGroupNames.Add(groupName);
            }
        }

        public virtual void Show(QuestJournal questJournal)
        {
            this.questJournal = questJournal;
            Show();
            Repaint();
            m_mustSendCloseMessage = ShouldSendOpenCloseMessage();
            if (ShouldSendOpenCloseMessage()) MessageSystem.SendMessage(this, m_openMessage, string.Empty);
        }

        public override void Hide()
        {
            base.Hide();
            if (m_mustSendCloseMessage) MessageSystem.SendMessage(this, m_closeMessage, string.Empty);
            m_mustSendCloseMessage = false;
        }

        private bool ShouldSendOpenCloseMessage()
        {
            switch (m_sendMessageOnOpen)
            {
                case SendMessageOnOpen.Always:
                    return true;
                case SendMessageOnOpen.Never:
                    return false;
                case SendMessageOnOpen.NotWhenUsingMouse:
                    return InputDeviceManager.currentInputDevice != InputDevice.Mouse;
                default:
                    return false;
            }
        }

        public virtual void Repaint(QuestJournal questJournal)
        {
            this.questJournal = questJournal;
            Repaint();
        }

        public virtual void Repaint()
        {
            if (!(isVisible && enabled && gameObject.activeInHierarchy)) return;
            if (m_refreshCoroutine == null) m_refreshCoroutine = StartCoroutine(RefreshAtEndOfFrame());
        }

        private IEnumerator RefreshAtEndOfFrame()
        {
            // Wait until end of frame so we only refresh once in case we receive multiple
            // requests to refresh during the same frame.
            yield return new WaitForEndOfFrame();
            m_refreshCoroutine = null;
            RefreshNow();
        }

        protected virtual void RefreshNow() //[TODO] Optimize.
        {
            isDrawingSelectionPanel = true;
            selectionPanelContentManager.Clear();            

            // Get group names:
            var groupNames = new List<string>();
            int numGroupless = 0;
            foreach (var quest in questJournal.questList)
            {
                var groupName = StringField.GetStringValue(quest.group);
                if (string.IsNullOrEmpty(groupName)) numGroupless++;
                if (string.IsNullOrEmpty(groupName) || groupNames.Contains(groupName)) continue;
                groupNames.Add(groupName);
            }

            // Add quests by group:
            foreach (var groupName in groupNames)
            {
                var groupFoldout = Instantiate<UnityUIFoldoutTemplate>(questGroupTemplate);
                selectionPanelContentManager.Add(groupFoldout, questSelectionContentContainer);
                groupFoldout.Assign(groupName, IsGroupExpanded(groupName));
                groupFoldout.foldoutButton.onClick.AddListener(() => { OnClickGroup(groupName, groupFoldout); });
                foreach (var quest in questJournal.questList)
                {
                    if (string.Equals(quest.group.value, groupName))
                    {
                        var questName = Instantiate<UnityUIQuestNameButtonTemplate>(GetQuestNameTemplateForState(quest.GetState()));
                        questName.Assign(quest, OnToggleTracking);
                        selectionPanelContentManager.Add(questName, groupFoldout.interiorPanel);
                        var target = quest;
                        questName.buttonTemplate.button.onClick.AddListener(() => { OnClickQuest(target); });
                    }
                }
            }

            // Add groupless quests:
            foreach (var quest in questJournal.questList)
            {
                var groupName = StringField.GetStringValue(quest.group);
                if (!string.IsNullOrEmpty(groupName)) continue;
                var questName = Instantiate<UnityUIQuestNameButtonTemplate>(GetQuestNameTemplateForState(quest.GetState()));
                questName.Assign(quest, OnToggleTracking);
                selectionPanelContentManager.Add(questName, questSelectionContentContainer);
                var target = quest;
                questName.buttonTemplate.button.onClick.AddListener(() => { OnClickQuest(target); });
            }

            RepaintSelectedQuest();

            RefreshNavigableSelectables();
        }

        private UnityUIQuestNameButtonTemplate GetQuestNameTemplateForState(QuestState state)
        {
            return (state == QuestState.Active) ? activeQuestNameTemplate : completedQuestNameTemplate;
        }

        protected virtual void OnClickGroup(string groupName, UnityUIFoldoutTemplate groupFoldout)
        {
            ToggleGroup(groupName);
            groupFoldout.ToggleInterior();
        }

        protected virtual void OnClickQuest(Quest quest)
        {
            SelectQuest(quest);
        }

        public virtual void SelectQuest(Quest quest)
        { 
            selectedQuest = quest;
            RepaintSelectedQuest();
        }

        protected virtual void RepaintSelectedQuest()
        {
            isDrawingSelectionPanel = false;
            currentContentManager.Clear();
            currentIconList = null;
            currentButtonList = null;
            if (selectedQuest != null)
            {
                var contents = selectedQuest.GetContentList(QuestContentCategory.Journal);
                if (contents.Count == 0 && showDialogueContentIfNoJournalContent) contents = selectedQuest.GetContentList(QuestContentCategory.Dialogue);
                AddContents(contents);
                if (selectedQuest.GetState() == QuestState.Active && selectedQuest.isAbandonable)
                {
                    var instance = Instantiate<UnityUIButtonTemplate>(abandonButtonTemplate);  //[TODO] Pool.
                    detailsPanelContentManager.Add(instance, questDetailsContentContainer);
                }
            }
            isDrawingSelectionPanel = true;

            RefreshNavigableSelectables();
        }

        public void OnToggleTracking(bool value, object data)
        {
            var quest = data as Quest;
            if (quest == null) return;
            quest.showInTrackHUD = value;
            QuestMachineMessages.RefreshUIs(quest);
        }

        public void OpenAbandonQuestConfirmationDialog()
        {
            if (abandonQuestPanel == null || selectedQuest == null) return;
            if (abandonQuestNameText != null) abandonQuestNameText.text = StringField.GetStringValue(selectedQuest.title);
            abandonQuestPanel.Open();
        }

        public void ConfirmAbandonQuest()
        {
            if (questJournal == null) return;
            questJournal.AbandonQuest(selectedQuest);
            Repaint();
        }

    }
}
