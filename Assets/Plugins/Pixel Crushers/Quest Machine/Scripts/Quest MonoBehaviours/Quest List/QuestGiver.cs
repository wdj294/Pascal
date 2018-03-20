// Copyright © Pixel Crushers. All rights reserved.

using UnityEngine;
using System.Collections.Generic;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// A GameObject that can offer quests. 
    /// </summary>
    [AddComponentMenu("")] // Use wrapper instead.
    public class QuestGiver : IdentifiableQuestListContainer
    {

        #region Serialized Fields

        [Tooltip("Text table used to look up tags.")]
        [SerializeField]
        private TextTable m_textTable;

        [Tooltip("The UI content to show when the quest giver has no quests to offer.")]
        [SerializeField]
        private BasicUIContent m_noQuestsUIContents = new BasicUIContent();

        [Tooltip("The UI content to show when the quest giver has more than one quest to offer. Shown above the quest list.")]
        [SerializeField]
        private BasicUIContent m_offerableQuestsUIContents = new BasicUIContent();

        [Tooltip("The UI content to show when the quest giver has more than one active quest. Shown above the quest list.")]
        [SerializeField]
        private BasicUIContent m_activeQuestsUIContents = new BasicUIContent();

        [Tooltip("What to show in dialogue when quest giver only has completed quests.")]
        [SerializeField]
        private CompletedQuestDialogueMode m_completedQuestDialogueMode = CompletedQuestDialogueMode.SameAsGlobal;

        [Tooltip("The quest dialogue UI to use when conversing with the player. If unassigned, uses the default dialogue UI.")]
        [SerializeField]
        [IQuestDialogueUIInspectorField]
        private UnityEngine.Object m_questDialogueUI;

        #endregion

        #region Property Accessors to Serialized Fields

        /// <summary>
        /// The text table to use for tags.
        /// </summary>
        public TextTable textTable
        {
            get { return m_textTable; }
            set { m_textTable = value; }
        }

        /// <summary>
        /// The UI content to show when the quest giver has no quests to offer.
        /// </summary>
        public BasicUIContent noQuestsUIContents
        {
            get { return m_noQuestsUIContents; }
            set { m_noQuestsUIContents = value; }
        }

        /// <summary>
        /// The UI content to show when the quest giver has more than one quest to offer. Shown above the quest list.
        /// </summary>
        public BasicUIContent offerableQuestsUIContents
        {
            get { return m_offerableQuestsUIContents; }
            set { m_offerableQuestsUIContents = value; }
        }

        /// <summary>
        /// The UI content to show when the quest giver has more than one active quest. Shown above the quest list.
        /// </summary>
        public BasicUIContent activeQuestsUIContents
        {
            get { return m_activeQuestsUIContents; }
            set { m_activeQuestsUIContents = value; }
        }

        /// <summary>
        /// What to show in dialogue when quest givers only have completed quests.
        /// </summary>
        public CompletedQuestDialogueMode completedQuestDialogueMode
        {
            get
            {
                switch (m_completedQuestDialogueMode)
                {
                    case CompletedQuestDialogueMode.SameAsGlobal:
                        switch (QuestMachine.completedQuestDialogueMode)
                        {
                            case CompletedQuestGlobalDialogueMode.ShowCompletedQuest:
                                return CompletedQuestDialogueMode.ShowCompletedQuest;
                            default:
                                return CompletedQuestDialogueMode.ShowNoQuests;
                        }
                    default:
                        return m_completedQuestDialogueMode;
                }
            }
            set
            {
                m_completedQuestDialogueMode = value;
            }
        }

        /// <summary>
        /// The QuestDialogueUI to use when conversing with the player.
        /// </summary>
        public IQuestDialogueUI questDialogueUI
        {
            get { return ((m_questDialogueUI as IQuestDialogueUI) != null) ? m_questDialogueUI as IQuestDialogueUI : QuestMachine.defaultQuestDialogueUI; }
            set { m_questDialogueUI = value as UnityEngine.Object; }
        }

        public static string GetDisplayName(QuestGiver questGiver)
        {
            return (questGiver != null) ? StringField.GetStringValue(questGiver.displayName) : string.Empty;
        }

        #endregion

        #region Runtime Info

        // Runtime info:
        protected List<Quest> nonOfferableQuests { get; set; }
        protected List<Quest> offerableQuests { get; set; }
        protected List<Quest> activeQuests { get; set; }
        protected List<Quest> completedQuests { get; set; }
        protected QuestEntity questEntity { get; set; }
        protected GameObject player { get; set; }
        protected QuestParticipantTextInfo playerTextInfo { get; set; }
        protected QuestListContainer playerQuestListContainer { get; set; }

        private QuestParticipantTextInfo m_myQuestGiverTextinfo = null;

        protected QuestParticipantTextInfo myQuestGiverTextInfo
        {
            get
            {
                if (m_myQuestGiverTextinfo == null) m_myQuestGiverTextinfo = new QuestParticipantTextInfo(id, displayName, image, textTable);
                return m_myQuestGiverTextinfo;
            }
        }

        #endregion

        #region Initialization

        public override void Awake()
        {
            base.Awake();
            nonOfferableQuests = new List<Quest>();
            offerableQuests = new List<Quest>();
            activeQuests = new List<Quest>();
            completedQuests = new List<Quest>();
            questEntity = GetComponent<QuestEntity>();
       }

        public override void Start()
        {
            base.Start();
            BackfillInfoFromEntityType();
            DeleteUnavailableQuests();
            AssignGiverIDToQuests();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            DestroyBasicUIContent();
        }

        private void DestroyBasicUIContent()
        { 
            if (noQuestsUIContents != null) noQuestsUIContents.DestroyContentList();
            if (offerableQuestsUIContents != null) offerableQuestsUIContents.DestroyContentList();
            if (activeQuestsUIContents != null) activeQuestsUIContents.DestroyContentList();
        }

        /// <summary>
        /// If UI info is unassigned, get it from the quest giver's QuestEntity if present.
        /// </summary>
        protected void BackfillInfoFromEntityType()
        {
            if (questEntity == null) return;
            if (StringField.IsNullOrEmpty(id))
            {
                id = questEntity.id;
            }
            if (StringField.IsNullOrEmpty(displayName))
            {
                displayName = questEntity.displayName;
            }
            if (image == null)
            {
                image = questEntity.image;
            }
        }

        /// <summary>
        /// Deletes a quest from this quest giver's list.
        /// </summary>
        /// <param name="questID"></param>
        public override void DeleteQuest(StringField questID)
        {
            var quest = FindQuest(questID);
            quest.ClearQuestIndicatorStates();
            base.DeleteQuest(questID);
        }

        /// <summary>
        /// Deletes quests whose maxTimes have been reached.
        /// </summary>
        protected void DeleteUnavailableQuests()
        {
            if (questList == null) return;
            for (int i = questList.Count - 1; i >= 0; i--)
            {
                var quest = questList[i];
                if (quest != null && quest.timesAccepted >= quest.maxTimes)
                {
                    DeleteQuest(quest);
                }
            }
        }

        protected void AssignGiverIDToQuests()
        {
            for (int i = 0; i < questList.Count; i++)
            {
                var quest = questList[i];
                if (quest == null) continue;
                quest.AssignQuestGiver(myQuestGiverTextInfo);
            }
        }

        /// <summary>
        /// Adds a quest to this quest giver's list.
        /// </summary>
        /// <param name="quest"></param>
        public override Quest AddQuest(Quest quest)
        {
            if (quest == null) return null;
            var instance = base.AddQuest(quest);
            instance.AssignQuestGiver(myQuestGiverTextInfo);
            QuestMachineMessages.RefreshUIs(this);
            return instance;
        }

        #endregion

        #region Record Quests By State

        /// <summary>
        /// Records the current offerable and player-assigned quests in the runtime lists.
        /// </summary>
        protected virtual void RecordQuestsByState()
        {
            RecordRelevantPlayerQuests();
            RecordOfferableQuests();
        }

        /// <summary>
        /// Records quests in the player's QuestList that were given by this quest giver
        /// or active quests for which this quest giver has dialogue content.
        /// </summary>
        protected virtual void RecordRelevantPlayerQuests()
        {
            activeQuests.Clear();
            completedQuests.Clear();
            if (playerQuestListContainer == null || playerQuestListContainer.questList == null) return;
            for (int i = 0; i < playerQuestListContainer.questList.Count; i++)
            {
                var quest = playerQuestListContainer.questList[i];
                if (quest == null) continue;
                var questState = quest.GetState();
                if (StringField.Equals(quest.questGiverID, id))
                {
                    switch (questState)
                    {
                        case QuestState.Active:
                            activeQuests.Add(quest);
                            break;
                        case QuestState.Successful:
                        case QuestState.Failed:
                            completedQuests.Add(quest);
                            break;
                    }
                }
                else if (questState == QuestState.Active && quest.speakers.Contains(StringField.GetStringValue(id)))
                {
                    activeQuests.Add(quest);
                }
            }
        }

        /// <summary>
        /// Removes completed quests that have no dialogue to offer.
        /// </summary>
        protected virtual void RemoveCompletedQuestsWithNoDialogue()
        {
            if (completedQuests == null || completedQuests.Count == 0) return;
            var info = new QuestParticipantTextInfo(id, displayName, image, textTable);
            for (int i = completedQuests.Count - 1; i >= 0; i--)
            {
                if (!completedQuests[i].HasContent(QuestContentCategory.Dialogue, info))
                {
                    completedQuests.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Records which quests are offerable or not in the runtime lists.
        /// </summary>
        protected virtual void RecordOfferableQuests()
        {
            nonOfferableQuests.Clear();
            offerableQuests.Clear();
            if (questList == null) return;
            if (playerQuestListContainer == null || playerQuestListContainer.questList == null) return;
            for (int i = 0; i < questList.Count; i++)
            {
                var quest = questList[i];
                if (quest == null || quest.GetState() != QuestState.WaitingToStart) continue;
                var playerCopy = playerQuestListContainer.FindQuest(quest.id);
                var isPlayerCopyActive = (playerCopy != null && playerCopy.GetState() == QuestState.Active);
                quest.UpdateCooldown();
                if (quest.canOffer && !isPlayerCopyActive)
                {
                    offerableQuests.Add(quest);
                }
                else if (playerCopy == null)
                {
                    nonOfferableQuests.Add(quest);
                }
            }
        }

        #endregion

        #region Dialogue

        /// <summary>
        /// Starts dialogue with the player. The content of the dialogue will depend on the quest giver's
        /// offerable quests and the player's quests.
        /// </summary>
        /// <param name="player">Player conversing with this QuestGiver. If null, searches the scene for a GameObject tagged Player.</param>
        public virtual void StartDialogue(GameObject player)
        {
            if (player == null) player = GameObject.FindGameObjectWithTag("Player");
            if (questDialogueUI == null || player == null) return;

            // Record quests related to this player and me:
            this.player = player;
            playerTextInfo = new QuestParticipantTextInfo(QuestMachineMessages.GetID(player), QuestMachineMessages.GetDisplayName(player), null, null);
            playerQuestListContainer = player.GetComponent<QuestListContainer>();
            if (playerQuestListContainer == null && Debug.isDebugBuild)
            {
                Debug.LogWarning("Quest Machine: Can't start dialogue with " + name + ". Player doesn't have a Quest Journal.", this);
                return;
            }
            RecordQuestsByState();

            // Start the most appropriate dialogue based on the recorded quests:
            if (QuestMachine.debug) Debug.Log("Quest Machine: " + name + ".StartDialogue: #offerable=" + offerableQuests.Count + " #active=" + activeQuests.Count + " #completed=" + completedQuests.Count, this);
            QuestMachineMessages.Greet(player, this, id);
            if (activeQuests.Count + offerableQuests.Count >= 2)
            {
                questDialogueUI.ShowQuestList(myQuestGiverTextInfo, activeQuestsUIContents.contentList, activeQuests, offerableQuestsUIContents.contentList, offerableQuests, OnSelectQuest);
            }
            else if (activeQuests.Count == 1)
            {
                ShowActiveQuest(activeQuests[0]);
            }
            else if (offerableQuests.Count == 1)
            {
                ShowOfferQuest(offerableQuests[0]);
            }
            else if (nonOfferableQuests.Count >= 1)
            {
                questDialogueUI.ShowOfferConditionsUnmet(myQuestGiverTextInfo, noQuestsUIContents.contentList, nonOfferableQuests);
            }
            else
            {
                RemoveCompletedQuestsWithNoDialogue();
                if (completedQuests.Count > 0 && completedQuestDialogueMode == CompletedQuestDialogueMode.ShowCompletedQuest)
                {

                    questDialogueUI.ShowCompletedQuest(myQuestGiverTextInfo, completedQuests);
                }
                else
                {
                    questDialogueUI.ShowContents(myQuestGiverTextInfo, noQuestsUIContents.contentList);
                }
            }
            QuestMachineMessages.Greeted(player, this, id);
        }

        /// <summary>
        /// Stops dialogue with the player.
        /// </summary>
        public virtual void StopDialogue()
        {
            if (questDialogueUI == null) return;
            questDialogueUI.Hide();
        }

        protected virtual void ShowOfferQuest(Quest quest)
        {
            QuestMachineMessages.DiscussQuest(player, this, id, quest.id);
            questDialogueUI.ShowOfferQuest(myQuestGiverTextInfo, quest, OnAcceptQuest, OnQuestBackButton);
            QuestMachineMessages.DiscussedQuest(player, this, id, quest.id);
        }

        protected virtual void ShowActiveQuest(Quest quest)
        {
            QuestMachineMessages.DiscussQuest(player, this, id, quest.id);
            questDialogueUI.ShowActiveQuest(myQuestGiverTextInfo, quest, OnContinueActiveQuest, OnQuestBackButton);
            QuestMachineMessages.DiscussedQuest(player, this, id, quest.id);
        }

        protected virtual void OnSelectQuest(Quest quest)
        {
            switch (quest.GetState())
            {
                case QuestState.WaitingToStart:
                    ShowOfferQuest(quest);
                    break;
                case QuestState.Active:
                    ShowActiveQuest(quest);
                    break;
            }
        }

        protected virtual void OnAcceptQuest(Quest quest)
        {
            GiveQuestToQuester(quest, playerTextInfo, playerQuestListContainer);
        }

        protected virtual void OnQuestBackButton(Quest quest)
        {
            if (activeQuests.Count + offerableQuests.Count >= 2)
            {
                questDialogueUI.ShowQuestList(myQuestGiverTextInfo, activeQuestsUIContents.contentList, activeQuests, offerableQuestsUIContents.contentList, offerableQuests, OnSelectQuest);
            }
            else
            {
                questDialogueUI.Hide();
            }
        }

        protected virtual void OnContinueActiveQuest(Quest quest)
        {
            questDialogueUI.Hide();
        }

        #endregion

        #region Give Quest

        /// <summary>
        /// Adds an instance of a quest to a quester's list. If the quest's maxTimes are reached,
        /// deletes the quest from the giver. Otherwise starts cooldown timer until it can be
        /// given again.
        /// </summary>
        /// <param name="quest">Quest to give to quester.</param>
        /// <param name="questerTextInfo">Quester's text info.</param>
        /// <param name="questerQuestListContainer">Quester's quest list container</param>
        public virtual void GiveQuestToQuester(Quest quest, QuestParticipantTextInfo questerTextInfo, QuestListContainer questerQuestListContainer)
        {
            if (quest == null)
            {
                Debug.LogWarning("Quest Machine: " + name + ".GiveQuestToQuester - quest is null.", this);
                return;
            }
            if (questerTextInfo == null)
            {
                Debug.LogWarning("Quest Machine: " + name + ".GiveQuestToQuester - questerTextInfo is null.", this);
                return;
            }
            if (questerQuestListContainer == null)
            {
                Debug.LogWarning("Quest Machine: " + name + ".GiveQuestToQuester - questerQuestListContainer is null.", this);
                return;
            }

            // Make a copy of the quest for the quester:
            var questInstance = quest.Clone();

            // Update the version on this QuestGiver:
            quest.timesAccepted++;
            if (quest.timesAccepted >= quest.maxTimes)
            {
                DeleteQuest(quest.id);
            }
            else
            {
                quest.StartCooldown();
            }

            // Add the copy to the quester and activate it:
            questInstance.AssignQuestGiver(myQuestGiverTextInfo);
            questInstance.AssignQuester(questerTextInfo);
            questerQuestListContainer.AddQuest(questInstance);
            questInstance.SetState(QuestState.Active);
            QuestMachineMessages.RefreshIndicators(questInstance);
        }

        #endregion

    }

}
