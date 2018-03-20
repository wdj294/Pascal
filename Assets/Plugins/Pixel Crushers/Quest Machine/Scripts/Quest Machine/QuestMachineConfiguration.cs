// Copyright © Pixel Crushers. All rights reserved.

using UnityEngine;
using System;
using System.Collections.Generic;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Configuration information for Quest Machine.
    /// </summary>
    [AddComponentMenu("")] // Use wrapper instead.
    public class QuestMachineConfiguration : MonoBehaviour
    {

        #region Serialized Fields

        [Tooltip("Quest asset databases to load into memory.")]
        [SerializeField]
        private List<QuestDatabase> m_questDatabases;

        [Tooltip("Default quest dialogue UI.")]
        [SerializeField]
        [IQuestDialogueUIInspectorField]
        private UnityEngine.Object m_questDialogueUI;

        [Tooltip("Default quest journal UI.")]
        [SerializeField]
        [IQuestJournalUIInspectorField]
        private UnityEngine.Object m_questJournalUI;

        [Tooltip("Default quest alert UI.")]
        [SerializeField]
        [IQuestAlertUIInspectorField]
        private UnityEngine.Object m_questAlertUI;

        [Tooltip("Default quest HUD.")]
        [SerializeField]
        [IQuestHUDInspectorField]
        private UnityEngine.Object m_questHUD;

        [Tooltip("What to show in dialogue when quest givers only have completed quests.")]
        [SerializeField]
        private CompletedQuestGlobalDialogueMode m_completedQuestDialogueMode = CompletedQuestGlobalDialogueMode.ShowCompletedQuest;

        [Tooltip("When saving and loading quests to JSON, format the output for readability over minimum size.")]
        [SerializeField]
        private bool m_prettyPrintJson;

        [Tooltip("Only allow one instance at a time.")]
        [SerializeField]
        private bool m_allowOnlyOneInstance = true;

        [Tooltip("Quest generation performance settings.")]
        [SerializeField]
        private QuestGeneratorSettings m_generatorSettings = new QuestGeneratorSettings();

        [Tooltip("Debug settings.")]
        [SerializeField]
        private QuestMachineConfigurationDebugSettings m_debugSettings = new QuestMachineConfigurationDebugSettings();

        #endregion

        #region Property Accessors to Serialized Fields

        /// <summary>
        /// Quest database to load into memory.
        /// </summary>
        public List<QuestDatabase> questDatabases
        {
            get { return m_questDatabases; }
            set { m_questDatabases = value; RegisterQuestDatabases(); }
        }

        /// <summary>
        /// Default quest dialogue UI.
        /// </summary>
        public IQuestDialogueUI questDialogueUI
        {
            get { return m_questDialogueUI as IQuestDialogueUI; }
            set
            {
                m_questDialogueUI = value as UnityEngine.Object;
                if (instance == this) QuestMachine.defaultQuestDialogueUI = value;
            }
        }

        /// <summary>
        /// Default quest journal UI.
        /// </summary>
        public IQuestJournalUI questJournalUI
        {
            get { return m_questJournalUI as IQuestJournalUI; }
            set
            {
                m_questJournalUI = value as UnityEngine.Object;
                if (instance == this) QuestMachine.defaultQuestJournalUI = value;
            }
        }

        /// <summary>
        /// Default quest alert UI.
        /// </summary>
        public IQuestAlertUI questAlertUI
        {
            get { return m_questAlertUI as IQuestAlertUI; }
            set
            {
                m_questAlertUI = value as UnityEngine.Object;
                if (instance == this) QuestMachine.defaultQuestAlertUI = value;
            }
        }

        /// <summary>
        /// Default quest HUD.
        /// </summary>
        public IQuestHUD questHUD
        {
            get { return m_questHUD as IQuestHUD; }
            set
            {
                m_questHUD = value as UnityEngine.Object;
                if (instance == this) QuestMachine.defaultQuestHUD = value;
            }
        }

        /// <summary>
        /// What to show in dialogue when quest givers only have completed quests.
        /// </summary>
        public CompletedQuestGlobalDialogueMode completedQuestDialogueMode
        {
            get { return m_completedQuestDialogueMode; }
            set { m_completedQuestDialogueMode = value; }
        }

        /// <summary>
        /// When serializing to JSON, format for readability (uses more whitespace) over minimum size.
        /// </summary>
        public bool prettyPrintJson
        {
            get { return m_prettyPrintJson; }
            set
            {
                m_prettyPrintJson = value;
                if (instance == this) QuestMachine.prettyPrintJson = value;
            }
        }

        /// <summary>
        /// Only allow one instance at a time.
        /// </summary>
        public bool allowOnlyOneInstance
        {
            get { return m_allowOnlyOneInstance; }
            set { m_allowOnlyOneInstance = value; }
        }

        /// <summary>
        /// Quest generation performance settings.
        /// </summary>
        public QuestGeneratorSettings generatorSettings
        {
            get { return m_generatorSettings; }
            set { m_generatorSettings = value; }
        }

        /// <summary>
        /// Debug settings.
        /// </summary>
        public QuestMachineConfigurationDebugSettings debugSettings
        {
            get { return m_debugSettings; }
            set { m_debugSettings = value; }
        }

        #endregion

        #region Private Static Variables

        /// <summary>
        /// The primary instance, which is the one most recently added.
        /// </summary>
        private static QuestMachineConfiguration m_instance = null;

        /// <summary>
        /// Tracks active instances of this component. When the current primary instance
        /// is destroyed, this list allows Quest Machine to promote the previous one
        /// as the primary instance.
        /// </summary>
        private static List<QuestMachineConfiguration> m_instances = new List<QuestMachineConfiguration>();

        /// <summary>
        /// The current primary instance.
        /// </summary>
        public static QuestMachineConfiguration instance
        {
            get { return m_instance; }
        }

        #endregion

        #region Initialization

        private void Awake()
        {
            if (m_allowOnlyOneInstance && m_instance != null)
            {
                Destroy(gameObject);
                return;
            }
            m_instance = this;
            QuestMachine.debug = debugSettings.debug;
            RegisterQuestDatabases();
            AddInstance(this);
        }

        private void OnDestroy()
        {
            RemoveInstance(this);
        }

        private void AddInstance(QuestMachineConfiguration instance)
        {
            if (instance == null) return;
            if (!m_instances.Contains(instance)) m_instances.Add(instance);
            instance.SetAsPrimaryInstance();
        }

        private void RemoveInstance(QuestMachineConfiguration instance)
        {
            if (instance == null) return;
            m_instances.Remove(instance);
            m_instances.RemoveAll(x => x == null);
            if (instance == m_instance) m_instance = (m_instances.Count > 0) ? m_instances[m_instances.Count - 1] : null;
        }

        private void SetAsPrimaryInstance()
        {
            m_instance = instance;
            QuestMachine.defaultQuestDialogueUI = questDialogueUI as IQuestDialogueUI;
            QuestMachine.defaultQuestJournalUI = questJournalUI as IQuestJournalUI;
            QuestMachine.defaultQuestAlertUI = questAlertUI as IQuestAlertUI;
            QuestMachine.defaultQuestHUD = questHUD as IQuestHUD;
            QuestMachine.completedQuestDialogueMode = completedQuestDialogueMode;
            QuestMachine.prettyPrintJson = prettyPrintJson;
            QuestMachine.debug = debugSettings.debug;
            MessageSystem.debug = debugSettings.debugMessageSystem;
            QuestGenerator.detailedDebug = debugSettings.debugQuestGenerator;
            QuestGenerator.maxSimultaneousPlanners = generatorSettings.maxSimultaneousPlanners;
            QuestGenerator.maxGoalActionChecksPerFrame = generatorSettings.maxGoalActionChecksPerFrame;
            QuestGenerator.maxStepsPerFrame = generatorSettings.maxStepsPerFrame;
        }

        public void RegisterQuestDatabases()
        {
            if (questDatabases == null) return;
            for (int i = 0; i < questDatabases.Count; i++)
            {
                var database = questDatabases[i];
                if (database == null || database.questAssets == null) continue;
                for (int j = 0; j < database.questAssets.Count; j++)
                {
                    QuestMachine.RegisterQuestAsset(database.questAssets[j]);
                }
            }
        }

        private void OnApplicationQuit()
        {
            QuestMachine.debug = false;
            MessageSystem.debug = false;
        }

        #endregion

        #region Evaluation Version Code

        #if EVALUATION_VERSION
        private GUIStyle evaluationWatermarkStyle = null;
        private Rect watermarkRect1;
        private Rect watermarkRect2;

        public void OnGUI()
        {
            if (Camera.main == null) return;
            if (Camera.main.GetComponent<GUILayer>() == null) Camera.main.gameObject.AddComponent<GUILayer>();
            if (evaluationWatermarkStyle == null)
            {
                evaluationWatermarkStyle = new GUIStyle(GUI.skin.label);
                evaluationWatermarkStyle.fontSize = 20;
                evaluationWatermarkStyle.fontStyle = FontStyle.Bold;
                evaluationWatermarkStyle.alignment = TextAnchor.MiddleCenter;
                evaluationWatermarkStyle.normal.textColor = new Color(1, 1, 1, 0.5f);
                Vector2 size = evaluationWatermarkStyle.CalcSize(new GUIContent("Evaluation Version"));
                if (UnityEngine.Random.value < 0.5f)
                {
                    watermarkRect1 = new Rect(8, 8, size.x, size.y);
                    watermarkRect2 = new Rect(8, 8 + size.y, size.x, size.y);
                }
                else
                {
                    watermarkRect1 = new Rect(Screen.width - size.x - 8, Screen.height - 2 * size.y - 8, size.x, size.y);
                    watermarkRect2 = new Rect(Screen.width - size.x - 8, Screen.height - size.y - 8, size.x, size.y);
                }
            }
            GUI.Label(watermarkRect1, "Quest Machine", evaluationWatermarkStyle);
            GUI.Label(watermarkRect2, "Evaluation Version", evaluationWatermarkStyle);
        }
        #endif

        #endregion

    }

    [Serializable]
    public class QuestMachineConfigurationDebugSettings
    {

        [Tooltip("Log verbose debugging info to the Console.")]
        [SerializeField]
        private bool m_debug;

        [Tooltip("Log verbose Message System info to the Console.")]
        [SerializeField]
        private bool m_debugMessageSystem;

        [Tooltip("Log verbose Quest Generator info to the Console.")]
        [SerializeField]
        private bool m_debugQuestGenerator;

        /// <summary>
        /// Log verbose debugging info to the Console.
        /// </summary>
        public bool debug
        {
            get { return m_debug; }
            set { m_debug = value; }
        }

        /// <summary>
        /// Log verbose Message System info to the Console.
        /// </summary>
        public bool debugMessageSystem
        {
            get { return m_debugMessageSystem; }
            set { m_debugMessageSystem = value; }
        }

        /// <summary>
        /// Log verbose Quest Generator info to the Console.
        /// </summary>
        public bool debugQuestGenerator
        {
            get { return m_debugQuestGenerator; }
            set { m_debugQuestGenerator = value; }
        }
    }

    [Serializable]
    public class QuestGeneratorSettings
    {

        [Tooltip("Limit simultaneous quest-planning processes to this number to control CPU load.")]
        [SerializeField]
        private int m_maxSimultaneousPlanners = 5;

        [Tooltip("When evaluating urgency of known entities, check this many actions on the goal entity each frame. Decrease the value to prevent stutter at the cost of longer planning times.")]
        [SerializeField]
        private int m_maxGoalActionChecksPerFrame = 100;

        [Tooltip("When planning the steps needed to accomplish a quest, evaluate this many possible steps each frame. Decrease the value to prevent stutter at the cost of longer planning times.")]
        [SerializeField]
        private int m_maxStepsPerFrame = 100;

        /// <summary>
        /// Limit simultaneous quest-planning processes to this number to control CPU load.
        /// </summary>
        public int maxSimultaneousPlanners
        {
            get { return m_maxSimultaneousPlanners; }
            set { m_maxSimultaneousPlanners = value; }
        }

        /// <summary>
        /// When evaluating urgency of known entities, check this many actions on the goal entity each frame. Decrease the value to prevent stutter at the cost of longer planning times.
        /// </summary>
        public int maxGoalActionChecksPerFrame
        {
            get { return m_maxGoalActionChecksPerFrame; }
            set { m_maxGoalActionChecksPerFrame = value; }
        }

        /// <summary>
        /// When planning the steps needed to accomplish a quest, evaluate this many possible steps each frame. Decrease the value to prevent stutter at the cost of longer planning times.
        /// </summary>
        public int maxStepsPerFrame
        {
            get { return m_maxStepsPerFrame; }
            set { m_maxStepsPerFrame = value; }
        }

    }

}
