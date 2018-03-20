// Copyright © Pixel Crushers. All rights reserved.

using UnityEngine;
using System;
using System.Collections.Generic;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Maintains a list of quests on a GameObject.
    /// </summary>
    [AddComponentMenu("")] // Use wrapper instead.
    public class QuestListContainer : Saver
    {

        #region Serialized Fields

        [Tooltip("Forward quest state events to listeners.")]
        [SerializeField]
        private bool m_forwardEventsToListeners = false;

        [Tooltip("Include in saved game data.")]
        [SerializeField]
        private bool m_includeInSavedGameData = false;

        [Tooltip("The current quest list. At runtime, these are runtime instances of quests.")]
        [SerializeField]
        private List<Quest> m_questList = new List<Quest>();

        [Tooltip("IDs of static quests that have been deleted and shouldn't be instantiated.")]
        [SerializeField]
        private List<string> m_deletedStaticQuests = new List<string>();

        #endregion

        #region Property Accessors to Serialized Fields

        /// <summary>
        /// Forward quest state events to listeners that have registered to events such as
        /// questBecameOfferable, questStateChanged, and questNodeStateChanged.
        /// </summary>
        public bool forwardEventsToListeners
        {
            get { return m_forwardEventsToListeners; }
            set { m_forwardEventsToListeners = value; }
        }

        /// <summary>
        /// Include in saved game data, which is used for saved games and scene persistence.
        /// </summary>
        public bool includeInSavedGameData
        {
            get { return m_includeInSavedGameData; }
            set { m_includeInSavedGameData = value; }
        }

        /// <summary>
        /// Quest assets.
        /// </summary>
        public List<Quest> questList
        {
            get { return m_questList; }
            protected set { m_questList = value; }
        }

        /// <summary>
        /// IDs of static quests that have been deleted and shouldn't be instantiated.
        /// </summary>
        public List<string> deletedStaticQuests
        {
            get { return m_deletedStaticQuests; }
            protected set { m_deletedStaticQuests = value; }
        }

        #endregion

        #region Runtime Properties

        /// <summary>
        /// The original design-time quest list.
        /// </summary>
        protected List<Quest> originalQuestList { get; set; }

        /// <summary>
        /// Raised when a quest is added to the list.
        /// </summary>
        public event QuestParameterDelegate questAdded = delegate { };

        /// <summary>
        /// Raised when a quest is removed from the list.
        /// </summary>
        public event QuestParameterDelegate questRemoved = delegate { };

        /// <summary>
        /// Raised when a quest in the list becomes offerable.
        /// </summary>
        public event QuestParameterDelegate questBecameOfferable = delegate { };

        /// <summary>
        /// Raised when the state of a quest in the list changes.
        /// </summary>
        public event QuestParameterDelegate questStateChanged = delegate { };

        /// <summary>
        /// Raised when the state of a quest node in a quest in the list changes.
        /// </summary>
        public event QuestNodeParameterDelegate questNodeStateChanged = delegate { };

        #endregion

        #region Initialization

        public override void Reset()
        {
            base.Reset();
            saveAcrossSceneChanges = true;
        }

        public override void Awake()
        {
            base.Awake();
            originalQuestList = questList;
            InstantiateQuestAssets();
        }

        public override void OnDestroy()
        {
            DestroyQuestInstances();
            base.OnDestroy();
        }

        /// <summary>
        /// Instantiates copies of quest assets into the runtime
        /// quest list and enables their autostart and offer condition checking.
        /// </summary>
        private void InstantiateQuestAssets()
        {
            questList = new List<Quest>();
            AddQuests(originalQuestList);
        }

        public void DestroyQuestInstances()
        {
            for (int i = questList.Count - 1; i >= 0; i--)
            {
                DeleteQuest(questList[i]);
            }
        }

        #endregion

        #region Add/Remove Quests

        public virtual void AddQuests(List<Quest> listToAdd)
        {
            if (listToAdd == null) return;
            for (int i = 0; i < listToAdd.Count; i++)
            {
                AddQuest(listToAdd[i]);
            }
        }

        public virtual Quest AddQuest(Quest quest)
        {
            if (quest == null) return null;
            if (deletedStaticQuests.Contains(StringField.GetStringValue(quest.id))) return null;
            var instance = quest.isAsset ? quest.Clone() : quest;
            if (instance == null) return null;
            questList.Add(instance);
            QuestMachine.RegisterQuestInstance(instance);
            RegisterForQuestEvents(instance);
            instance.RuntimeStartup();
            return instance;
        }

        public virtual Quest FindQuest(StringField questID)
        {
            if (StringField.IsNullOrEmpty(questID)) return null;
            for (int i = 0; i < questList.Count; i++)
            {
                var quest = questList[i];
                if (quest == null) continue;
                if (StringField.Equals(questID, quest.id)) return quest;
            }
            return null;
        }

        public virtual bool ContainsQuest(StringField questID)
        {
            return FindQuest(questID) != null;
        }

        public virtual void DeleteQuest(StringField questID)
        {
            DeleteQuest(FindQuest(questID));
        }

        public virtual void DeleteQuest(Quest quest)
        {
            if (quest == null) return;
            questList.Remove(quest);
            if (!quest.isProcedurallyGenerated)
            {
                var questID = StringField.GetStringValue(quest.id);
                if (!deletedStaticQuests.Contains(questID)) deletedStaticQuests.Add(questID);
            }
            UnregisterForQuestEvents(quest);
            QuestMachine.UnregisterQuestInstance(quest);
            Quest.DestroyInstance(quest);
        }

        public virtual void RegisterForQuestEvents(Quest quest)
        {
            if (quest == null || !forwardEventsToListeners) return;
            questAdded(quest);
            quest.questOfferable += OnQuestBecameOfferable;
            quest.stateChanged += OnQuestStateChanged;
            for (int i = 0; i < quest.nodeList.Count; i++)
            {
                quest.nodeList[i].stateChanged += OnQuestNodeStateChanged;
            }
        }

        public virtual void UnregisterForQuestEvents(Quest quest)
        {
            if (quest == null || !forwardEventsToListeners) return;
            questRemoved(quest);
            quest.questOfferable -= OnQuestBecameOfferable;
            quest.stateChanged -= OnQuestStateChanged;
            for (int i = 0; i < quest.nodeList.Count; i++)
            {
                quest.nodeList[i].stateChanged -= OnQuestNodeStateChanged;
            }
        }

        public virtual void OnQuestBecameOfferable(Quest quest)
        {
            questBecameOfferable(quest);
        }

        public virtual void OnQuestStateChanged(Quest quest)
        {
            questStateChanged(quest);
        }

        public virtual void OnQuestNodeStateChanged(QuestNode questNode)
        {
            questNodeStateChanged(questNode);
        }

        #endregion

        #region Save/Load

        [Serializable]
        public class SaveData
        {
            public List<string> staticQuestIds = new List<string>();
            public List<ByteData> staticQuestData = new List<ByteData>();

            public List<string> proceduralQuests = new List<string>();

            public List<string> deletedStaticQuests = new List<string>();
        }

        public override string RecordData()
        {
            if (!includeInSavedGameData) return string.Empty;
            var saveData = new SaveData();
            for (int i = 0; i < questList.Count; i++)
            {
                var quest = questList[i];
                if (quest == null) continue;
                if (quest.isProcedurallyGenerated)
                {
                    saveData.proceduralQuests.Add(JsonUtility.ToJson(new QuestProxy(quest)));
                }
                else
                {
                    saveData.staticQuestIds.Add(StringField.GetStringValue(quest.id));
                    var bytes = QuestStateSerializer.Serialize(quest);
                    saveData.staticQuestData.Add(new ByteData(bytes));
                }
            }
            saveData.deletedStaticQuests.AddRange(deletedStaticQuests);
            return SaveSystem.Serialize(saveData);
        }

        public override void ApplyData(string data)
        {
            if (!includeInSavedGameData) return;
            if (string.IsNullOrEmpty(data)) return;
            var saveData = SaveSystem.Deserialize<SaveData>(data);
            if (saveData == null) return;

            // Clear current quest list:
            DestroyQuestInstances();

            // Restore dynamic quests:
            for (int i = 0; i < saveData.proceduralQuests.Count; i++)
            {
                var questProxy = JsonUtility.FromJson<QuestProxy>(saveData.proceduralQuests[i]);
                if (questProxy == null) continue;
                var quest = ScriptableObject.CreateInstance<Quest>();
                quest.name = questProxy.displayName;
                questProxy.CopyTo(quest);
                AddQuest(quest);
                //questList.Add(quest);
                //QuestMachine.RegisterQuestInstance(quest);
            }

            // Restore list of deleted static quests:
            deletedStaticQuests.Clear();
            deletedStaticQuests.AddRange(saveData.deletedStaticQuests);

            // Restore static quests:
            for (int i = 0; i < Mathf.Min(saveData.staticQuestIds.Count, saveData.staticQuestData.Count); i++)
            {
                var questID = saveData.staticQuestIds[i];
                var questData = saveData.staticQuestData[i];
                if (string.IsNullOrEmpty(questID) || questData == null || questData.bytes == null) continue;
                if (deletedStaticQuests.Contains(questID)) continue;
                var quest = QuestMachine.GetQuestAsset(questID);
                if (quest == null && Debug.isDebugBuild) Debug.LogError("Quest Machine: " + name + " Can't find quest " + saveData.staticQuestIds[i] + ". Is it registered with Quest Machine?", this);
                if (quest == null) continue;
                quest = quest.Clone();
                QuestStateSerializer.DeserializeInto(quest, questData.bytes);
                AddQuest(quest);
                //questList.Add(quest);
                //QuestMachine.RegisterQuestInstance(quest);
            }
            QuestMachineMessages.RefreshIndicator(this, QuestMachineMessages.GetID(this));
        }

        #endregion

    }
}