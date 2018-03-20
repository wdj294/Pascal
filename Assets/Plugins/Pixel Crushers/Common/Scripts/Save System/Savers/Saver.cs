// Copyright © Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers
{

    /// <summary>
    /// Abstract base class for a "saver", which is a component that contributes
    /// to saved game data.
    /// </summary>
    public abstract class Saver : MonoBehaviour
    {

        [Tooltip("Save data under this key. If blank, use GameObject name.")]
        [SerializeField]
        private string m_key;

        [Tooltip("Save when changing scenes to be able to restore saved state when returning to scene.")]
        [SerializeField]
        private bool m_saveAcrossSceneChanges = false;

        /// <summary>
        /// Save data under this key. If blank, use GameObject name.
        /// </summary>
        public virtual string key
        {
            get { return !string.IsNullOrEmpty(m_key) ? m_key : name; }
            set { m_key = value; }
        }

        /// <summary>
        /// Save when changing scenes to be able to restore saved state when returning to scene.
        /// </summary>
        public virtual bool saveAcrossSceneChanges
        {
            get { return m_saveAcrossSceneChanges; }
            set { m_saveAcrossSceneChanges = value; }
        }

        public virtual void Awake() { }

        public virtual void Start() { }

        public virtual void Reset() { }

        public virtual void OnEnable()
        {
            SaveSystem.RegisterSaver(this);
        }

        public virtual void OnDisable()
        {
            SaveSystem.UnregisterSaver(this);
        }

        public virtual void OnDestroy() { }

        /// <summary>
        /// This method should return a string that represents the data you want to save.
        /// Typically you can use JsonUtility.ToJson() to serialize a serializable object
        /// to a string.
        /// </summary>
        public abstract string RecordData();

        /// <summary>
        /// This method should process the string representation of saved data and apply
        /// it to the current state of the game. Typically you can use JsonUtility.FromJson()
        /// to deserialize the string to an object that specifies the state to apply to
        /// the game.
        /// </summary>
        public abstract void ApplyData(string data);

        /// <summary>
        /// The Save System will call this method before scene changes. If your saver listens for 
        /// OnDisable or OnDestroy messages (see DestructibleSaver for example), it can use this 
        /// method to ignore the next OnDisable or OnDestroy message since they will be called
        /// because the entire scene is being unloaded.
        /// </summary>
        public virtual void OnBeforeSceneChange() { }

    }

}
