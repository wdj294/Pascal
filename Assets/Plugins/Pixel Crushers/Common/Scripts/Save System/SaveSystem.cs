// Copyright © Pixel Crushers. All rights reserved.

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PixelCrushers
{

    /// <summary>
    /// This is the main Save System class. It runs as a singleton MonoBehaviour
    /// and provides static methods to save and load games.
    /// </summary>
    [AddComponentMenu("")] // Use wrapper instead.
    public class SaveSystem : MonoBehaviour
    {

        public const int NoSceneIndex = -1;

        [Tooltip("Log debug info.")]
        [SerializeField]
        private bool m_debug = false;

        private static SaveSystem m_instance = null;

        private static List<Saver> m_savers = new List<Saver>();

        private static SavedGameData m_savedGameData = new SavedGameData();

        private static DataSerializer m_serializer = null;

        private static SavedGameDataStorer m_storer = null;

        private static SceneTransitionManager m_sceneTransitionManager = null;

        private static GameObject m_playerSpawnpoint = null;

        private static int m_currentSceneIndex = NoSceneIndex;

        public static bool debug
        {
            get
            {
                return (m_instance != null) ? m_instance.m_debug && Debug.isDebugBuild : false;
            }
        }

        private static SaveSystem instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = new GameObject("Save System", typeof(SaveSystem)).GetComponent<SaveSystem>();
                }
                return m_instance;
            }
        }

        private static DataSerializer serializer
        {
            get
            {
                if (m_serializer == null)
                {
                    m_serializer = instance.GetComponent<DataSerializer>();
                    if (m_serializer == null)
                    {
                        Debug.Log("Save System: No DataSerializer found on " + instance.name + ". Adding JsonDataSerializer.", instance);
                        m_serializer = instance.gameObject.AddComponent<JsonDataSerializer>();
                    }
                }
                return m_serializer;
            }
        }

        private static SavedGameDataStorer storer
        {
            get
            {
                if (m_storer == null)
                {
                    m_storer = instance.GetComponent<SavedGameDataStorer>();
                    if (m_storer == null)
                    {
                        Debug.Log("Save System: No SavedGameDataStorer found on " + instance.name + ". Adding PlayerPrefsSavedGameDataStorer.", instance);
                        m_storer = instance.gameObject.AddComponent<PlayerPrefsSavedGameDataStorer>();
                    }
                }
                return m_storer;
            }
        }

        public static SceneTransitionManager sceneTransitionManager
        {
            get
            {
                if (m_sceneTransitionManager == null)
                {
                    m_sceneTransitionManager = instance.GetComponentInChildren<SceneTransitionManager>();
                }
                return m_sceneTransitionManager;
            }
        }

        /// <summary>
        /// Where the player should spawn in the current scene.
        /// </summary>
        public static GameObject playerSpawnpoint
        {
            get { return m_playerSpawnpoint; }
            set { m_playerSpawnpoint = value; }
        }

        /// <summary>
        /// Build index of the current scene.
        /// </summary>
        public static int currentSceneIndex
        {
            get
            {
                if (m_currentSceneIndex == NoSceneIndex) m_currentSceneIndex = GetCurrentSceneIndex();
                return m_currentSceneIndex;
            }
        }

        public delegate void SceneLoadedDelegate(string sceneName, int sceneIndex);

        public static event SceneLoadedDelegate sceneLoaded = delegate { };

        private void Awake()
        {
            if (m_instance == null)
            {
                m_instance = this;
                if (transform.parent != null) transform.SetParent(null);
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void OnApplicationQuit()
        {
            BeforeSceneChange();
        }

#if UNITY_5_4_OR_NEWER
        private void OnEnable()
        {
            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        public void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
        {
            FinishedLoadingScene(scene.name, scene.buildIndex);
        }

#else
        public void OnLevelWasLoaded(int level)
        {
            FinishedLoadingScene(GetCurrentSceneName(), level);
        }
#endif

#if UNITY_5_3 || UNITY_5_3_OR_NEWER
        private static string GetCurrentSceneName()
        {
            return UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        }

        private static int GetCurrentSceneIndex()
        {
            return UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
        }

        private static IEnumerator LoadSceneInternal(string sceneName)
        {
            if (sceneTransitionManager == null)
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
                yield break;
            }
            else
            {
                yield return instance.StartCoroutine(LoadSceneInternalTransitionCoroutine(sceneName));
            }
        }

        private static IEnumerator LoadSceneInternalTransitionCoroutine(string sceneName)
        {
            yield return instance.StartCoroutine(sceneTransitionManager.LeaveScene());
            yield return UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
            instance.StartCoroutine(sceneTransitionManager.EnterScene());
        }
#else
        private static string GetCurrentSceneName()
        {
            return Application.loadedLevelName;
        }

        private static int GetCurrentSceneIndex()
        {
            return Application.loadedLevel;
        }

        private static IEnumerator LoadSceneInternal(string sceneName)
        {
            Application.LoadLevel(sceneName);
            yield break;
        }
#endif

        /// <summary>
        /// Saves a game into a slot using the storage provider on the 
        /// Save System GameObject.
        /// </summary>
        /// <param name="slotNumber">Slot in which to store saved game data.</param>
        public void SaveGameToSlot(int slotNumber)
        {
            SaveToSlot(slotNumber);
        }

        /// <summary>
        /// Loads a game from a slot using the storage provider on the
        /// Save System GameObject.
        /// </summary>
        /// <param name="slotNumber"></param>
        public void LoadGameFromSlot(int slotNumber)
        {
            LoadFromSlot(slotNumber);
        }

        /// <summary>
        /// Loads a scene, optionally positioning the player at a
        /// specified spawnpoint.
        /// </summary>
        /// <param name="sceneNameAndSpawnpoint">
        /// A string containing the name of the scene to load, optionally
        /// followed by "@spawnpoint" where "spawnpoint" is the name of
        /// a GameObject in that scene. The player will be spawned at that
        /// GameObject's position.
        /// </param>
        public void LoadSceneAtSpawnpoint(string sceneNameAndSpawnpoint)
        {
            LoadScene(sceneNameAndSpawnpoint);
        }

        public static bool HasSavedGameInSlot(int slotNumber)
        {
            return storer.HasDataInSlot(slotNumber);
        }

        public static void DeleteSavedGameInSlot(int slotNumber)
        {
            storer.DeleteSavedGameData(slotNumber);
        }

        public static void SaveToSlot(int slotNumber)
        {
            storer.StoreSavedGameData(slotNumber, RecordSavedGameData());
        }

        public static void LoadFromSlot(int slotNumber)
        {
            if (!HasSavedGameInSlot(slotNumber))
            {
                if (Debug.isDebugBuild) Debug.LogWarning("Save System: LoadFromSlot(" + slotNumber + ") but there is no saved game in this slot.");
                return;
            }
            LoadGame(storer.RetrieveSavedGameData(slotNumber));
        }

        public static void RegisterSaver(Saver saver)
        {
            if (saver == null || m_savers.Contains(saver)) return;
            m_savers.Add(saver);
        }

        public static void UnregisterSaver(Saver saver)
        {
            m_savers.Remove(saver);
        }

        public static void ClearSavedGameData()
        {
            m_savedGameData = new SavedGameData();
        }

        public static SavedGameData RecordSavedGameData()
        {
            m_savedGameData.sceneName = GetCurrentSceneName();
            for (int i = 0; i < m_savers.Count; i++)
            {
                try
                {
                    var saver = m_savers[i];
                    m_savedGameData.SetData(saver.key, GetSaverSceneIndex(saver), saver.RecordData());
                }
                catch (System.Exception e)
                {
                    Debug.LogException(e);
                }
            }
            return m_savedGameData;
        }

        private static int GetSaverSceneIndex(Saver saver)
        {
            return (saver == null || !saver.saveAcrossSceneChanges) ? currentSceneIndex : NoSceneIndex;
        }

        public static void UpdateSaveData(Saver saver, string data)
        {
            m_savedGameData.SetData(saver.key, GetSaverSceneIndex(saver), data);
        }

        public static void ApplySavedGameData(SavedGameData savedGameData)
        {
            for (int i = m_savers.Count - 1; i >= 0; i--) // A saver may remove itself from list during apply.
            {
                try
                {
                    var saver = m_savers[i];
                    saver.ApplyData(savedGameData.GetData(saver.key));
                }
                catch (System.Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }

        public static void BeforeSceneChange()
        {
            // Notify savers:
            for (int i = 0; i < m_savers.Count; i++)
            {
                try
                {
                    var saver = m_savers[i];
                    saver.OnBeforeSceneChange();
                }
                catch (System.Exception e)
                {
                    Debug.LogException(e);
                }
            }
            // Notify SceneNotifier:
            try
            {
                SceneNotifier.NotifyWillUnloadScene(m_currentSceneIndex);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }
        }

        public static void LoadGame(SavedGameData savedGameData)
        {
            instance.StartCoroutine(LoadSceneCoroutine(savedGameData, null));
        }

        public static void LoadScene(string sceneNameAndSpawnpoint)
        {
            if (string.IsNullOrEmpty(sceneNameAndSpawnpoint)) return;
            string sceneName = sceneNameAndSpawnpoint;
            string spawnpointName = string.Empty;
            if (sceneNameAndSpawnpoint.Contains("@"))
            {
                var strings = sceneNameAndSpawnpoint.Split('@');
                sceneName = strings[0];
                spawnpointName = (strings.Length > 1) ? strings[1] : null;
            }
            var savedGameData = RecordSavedGameData();
            savedGameData.sceneName = sceneName;
            instance.StartCoroutine(LoadSceneCoroutine(savedGameData, spawnpointName));
        }

        private static IEnumerator LoadSceneCoroutine(SavedGameData savedGameData, string spawnpointName)
        {
            if (savedGameData == null) yield break;
            if (debug) Debug.Log("Save System: Loading scene " + savedGameData.sceneName +
                (string.IsNullOrEmpty(spawnpointName) ? string.Empty : " [spawn at " + spawnpointName + "]"));
            m_savedGameData = savedGameData;
            BeforeSceneChange();
            yield return LoadSceneInternal(savedGameData.sceneName);
            yield return null; // Need time to spin up scene first.
            m_playerSpawnpoint = !string.IsNullOrEmpty(spawnpointName) ? GameObject.Find(spawnpointName) : null;
            ApplySavedGameData(savedGameData);
        }

        private void FinishedLoadingScene(string sceneName, int sceneIndex)
        {
            m_currentSceneIndex = sceneIndex;
            m_savedGameData.DeleteObsoleteSaveData(sceneIndex);
            sceneLoaded(sceneName, sceneIndex);
        }

        public static void RestartGame(string startingSceneName)
        {
            ClearSavedGameData();
            BeforeSceneChange();
            LoadSceneInternal(startingSceneName);
            // Reserved for future async loading / loading scenes.
        }

        public static string Serialize(object data)
        {
            return serializer.Serialize(data);
        }

        public static T Deserialize<T>(string s)
        {
            return serializer.Deserialize<T>(s);
        }

    }
}