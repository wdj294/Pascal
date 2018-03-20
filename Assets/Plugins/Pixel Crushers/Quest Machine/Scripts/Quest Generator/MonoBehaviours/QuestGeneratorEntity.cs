// Copyright © Pixel Crushers. All rights reserved.

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// A quest generator entity is an entity that can generate quests.
    /// </summary>
    [AddComponentMenu("")] // Use wrapper instead.
    [RequireComponent(typeof(QuestListContainer))]
    public class QuestGeneratorEntity : QuestEntity
    {

        #region Serialized Fields

        [Tooltip("Organize quests in this group. Leave blank for no group.")]
        [SerializeField]
        private StringField m_questGroup = new StringField();

        [Tooltip("The domain type where this quest giver is located.")]
        [SerializeField]
        private DomainType m_domainType;

        [Tooltip("The domains that this quest giver observes.")]
        [SerializeField]
        private QuestDomain[] m_domains = new QuestDomain[0];

        [Tooltip("Require the quester to speak to the quest giver to finish the quest.")]
        [SerializeField]
        private bool m_requireReturnToComplete = true;

        [Tooltip("The UI content to show above the list of rewards offered for a quest.")]
        [SerializeField]
        private List<QuestContent> m_rewardsUIContents = new List<QuestContent>();

        [Tooltip("Generate a quest on start.")]
        [SerializeField]
        private bool m_generateQuestOnStart;

        [Tooltip("Generate a quest only if the quest list is smaller than this.")]
        [SerializeField]
        private int m_maxQuestsToGenerate = 1;

        [NonSerialized]
        private QuestListContainer m_questListContainer;

        [NonSerialized]
        private List<RewardSystem> m_rewardSystems = new List<RewardSystem>();

        #endregion

        #region Property Accessors to Serialized Fields

        /// <summary>
        /// Organize quests in this group. Leave blank for no group.
        /// </summary>
        public StringField questGroup
        {
            get { return m_questGroup; }
            set { m_questGroup = value; }
        }

        /// <summary>
        /// The domain type where this quest giver is located.
        /// </summary>
        public DomainType domainType
        {
            get { return m_domainType; }
            set { m_domainType = value; }
        }

        /// <summary>
        /// Require the quester to speak to the quest giver to finish the quest.
        /// </summary>
        public bool requireReturnToComplete
        {
            get { return m_requireReturnToComplete; }
            set { m_requireReturnToComplete = value; }
        }

        /// <summary>
        /// The domains that this quest giver observes.
        /// </summary>
        public QuestDomain[] domains
        {
            get { return m_domains; }
            set { m_domains = value; }
        }

        /// <summary>
        /// The UI content to show above the list of rewards offered for a quest.
        /// </summary>
        public List<QuestContent> rewardsUIContents
        {
            get { return m_rewardsUIContents; }
            set { m_rewardsUIContents = value; }
        }

        /// <summary>
        /// Reward systems to use to generate rewards.
        /// </summary>
        public List<RewardSystem> rewardSystems
        {
            get { return m_rewardSystems; }
            set { m_rewardSystems = value; }
        }

        /// <summary>
        /// Generate a quest as soon as the component starts.
        /// </summary>
        public bool generateQuestOnStart
        {
            get { return m_generateQuestOnStart; }
            set { m_generateQuestOnStart = value; }
        }

        /// <summary>
        /// Generate a quest only if the quest list is smaller than this
        /// </summary>
        public int maxQuestsToGenerate
        {
            get { return m_maxQuestsToGenerate; }
            set { m_maxQuestsToGenerate = value; }
        }

        public QuestListContainer questListContainer
        {
            get { return m_questListContainer; }
            set { m_questListContainer = value; }
        }

        #endregion

        #region Runtime Properties

        public delegate void UpdateWorldModelDelegate(WorldModel worldModel);

        public event UpdateWorldModelDelegate updateWorldModel = delegate { };

        public event GeneratedQuestDelegate generatedQuest = delegate { };

        public QuestGenerator questGenerator { get; private set; }

        #endregion

        #region Initialization

        protected virtual void Awake()
        {
            questGenerator = new QuestGenerator();
            questListContainer = GetComponent<QuestListContainer>();
            RecordRewardSystems();
        }

        protected virtual IEnumerator Start()
        {
            yield return null;
            yield return null;
            if (generateQuestOnStart) GenerateQuest();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            questGenerator.CancelGeneration();
        }

        /// <summary>
        /// Updates the list of reward systems on this generator entity.
        /// </summary>
        public void RecordRewardSystems()
        {
            rewardSystems.Clear();
            foreach (var rewardSystem in GetComponentsInChildren<RewardSystem>())
            {
                if (!rewardSystems.Contains(rewardSystem) && rewardSystem.enabled)
                {
                    rewardSystems.Add(rewardSystem);
                }
            }
        }

        #endregion

        #region Generate Quest

        /// <summary>
        /// Returns the current number of generated quests.
        /// </summary>
        public int GetGeneratedQuestCount()
        {
            int count = 0;
            for (int i = 0; i < questListContainer.questList.Count; i++)
            {
                var quest = questListContainer.questList[i];
                if (quest != null && quest.isProcedurallyGenerated) count++;
            }
            return count;
        }

        /// <summary>
        /// Generates a quest if the current number of generated quests is under the max.
        /// </summary>
        public void GenerateQuest()
        {
            if (GetGeneratedQuestCount() >= maxQuestsToGenerate) return;
            var worldModel = BuildWorldModel();
            questGenerator.GenerateQuest(this, questGroup, domainType, worldModel, requireReturnToComplete, rewardsUIContents, rewardSystems, questListContainer.questList, OnGeneratedQuest);
        }

        private void OnGeneratedQuest(Quest quest)
        {
            GetComponent<QuestGiver>().AddQuest(quest);
            generatedQuest(quest);
        }

        /// <summary>
        /// Returns the world model as observed by this entity.
        /// </summary>
        public WorldModel BuildWorldModel()
        {
            var worldModel = BuildWorldModelFromDomains();
            updateWorldModel(worldModel);
            return worldModel;
        }

        public WorldModel BuildWorldModelFromDomains()
        {
            var worldModel = new WorldModel(new Fact(domainType, entityType, 1)); //[TODO] Pool.
            foreach (var domain in domains)
            {
                domain.AddEntitiesToWorldModel(worldModel);
            }
            return worldModel;
        }

        #endregion

    }

}
