// Copyright © Pixel Crushers. All rights reserved.

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Spawner. Methods are virtual so you can override them if you need custom behavior.
    /// </summary>
    [AddComponentMenu("")]
    public class Spawner : MonoBehaviour, IMessageHandler
    {

        #region Subtypes

        [Serializable]
        public class PrefabInfo
        {
            public GameObject prefab;

            [Tooltip("Relative probability with which to spawn this prefab.")]
            public float weight = 1;
        }

        [Serializable]
        public class PositionInfo
        {
            public enum PositionType { Radius, Spawnpoints }
            public enum Plane { X_Y, X_Z }

            [Tooltip("Position in a radius around the spawner or at specific spawnpoints.")]
            public PositionType positionType = PositionType.Radius;

            [Tooltip("If radius, the maximum distance from spawner at which to spawn entities.")]
            public float radius = 10;

            [Tooltip("If radius, the X-Z plane (3D) or X-Y plane (2D)")]
            public Plane plane = Plane.X_Z;

            [Tooltip("If Spawnpoints, place entities at these exact spawnpoints.")]
            public GameObject[] spawnpoints;
        }

        #endregion

        #region Serialized Fields

        [Tooltip("Name by which Quest Machine can reference this spawner.")]
        [SerializeField]
        private StringField m_spawnerName = new StringField();

        [Tooltip("Prefabs to spawn.")]
        [SerializeField]
        private PrefabInfo[] m_prefabs = new PrefabInfo[0];

        [Tooltip("Where to spawn.")]
        [SerializeField]
        private PositionInfo m_positionInfo = new PositionInfo();

        [Tooltip("Entities that have been spawned.")]
        [SerializeField]
        private List<SpawnedEntity> m_spawnedEntities = new List<SpawnedEntity>();

        [Tooltip("Minimum number of entities to spawn.")]
        [SerializeField]
        private int m_min = 1;

        [Tooltip("Maximum number of entities to spawn.")]
        [SerializeField]
        private int m_max = 5;

        [Tooltip("Once above the minimum, spawn one entity at this frequency in seconds.")]
        [SerializeField]
        private float m_spawnRate = 5;

        [Tooltip("Start spawning as soon as this component starts.")]
        [SerializeField]
        private bool m_autoStart = false;

        [Tooltip("Stop spawning as soon as the minimum number of entities has been reached.")]
        [SerializeField]
        private bool m_stopWhenMinReached = true;

        [Tooltip("Despawn all spawned entities when this component is destroyed.")]
        [SerializeField]
        private bool m_despawnOnDestroy = false;

        #endregion

        #region Accessor Properties to Serialized Fields

        /// <summary>
        /// Name by which Quest Machine can reference this spawner.
        /// </summary>
        public StringField spawnerName
        {
            get { return m_spawnerName; }
            set { m_spawnerName = value; }
        }

        /// <summary>
        /// Prefabs to spawn.
        /// </summary>
        public PrefabInfo[] prefabs
        {
            get { return m_prefabs; }
            set { m_prefabs = value; }
        }

        /// <summary>
        /// Where to spawn.
        /// </summary>
        public PositionInfo positionInfo
        {
            get { return m_positionInfo; }
            set { m_positionInfo = value; }
        }

        /// <summary>
        /// Entities that have been spawned.
        /// </summary>
        public List<SpawnedEntity> spawnedEntities
        {
            get { return m_spawnedEntities; }
            set { m_spawnedEntities = value; }
        }

        /// <summary>
        /// Minimum number of entities to spawn.
        /// </summary>
        public int min
        {
            get { return m_min; }
            set { m_min = value; }
        }

        /// <summary>
        /// Maximum number of entities to spawn.
        /// </summary>
        public int max
        {
            get { return m_max; }
            set { m_max = value; }
        }

        /// <summary>
        /// Once above the minimum, spawn one entity at this frequency in seconds.
        /// </summary>
        public float spawnRate
        {
            get { return m_spawnRate; }
            set { m_spawnRate = value; }
        }

        /// <summary>
        /// Start spawning as soon as this component starts.
        /// </summary>
        public bool autoStart
        {
            get { return m_autoStart; }
            set { m_autoStart = value; }
        }

        /// <summary>
        /// Stop spawning as soon as the minimum number of entities has been reached.
        /// </summary>
        public bool stopWhenMinReached
        {
            get { return m_stopWhenMinReached; }
            set { m_stopWhenMinReached = value; }
        }

        /// <summary>
        /// Despawn all spawned entities when this component is destroyed.
        /// </summary>
        public bool despawnOnDestroy
        {
            get { return m_despawnOnDestroy; }
            set { m_despawnOnDestroy = value; }
        }

        private int spawnCount = 0;

        #endregion

        #region Initialization

        protected virtual void Start()
        {
            RegisterWithMessageSystem();
            if (autoStart) StartSpawning();
        }

        protected virtual void OnDestroy()
        {
            UnregisterWithMessageSystem();
            if (despawnOnDestroy) DespawnAll();
        }

        protected virtual void RegisterWithMessageSystem()
        {
            // Listen for Start, Stop, and Despawn messages:
            MessageSystem.AddListener(this, QuestMachineMessages.StartSpawnerMessage, spawnerName);
            MessageSystem.AddListener(this, QuestMachineMessages.StopSpawnerMessage, spawnerName);
            MessageSystem.AddListener(this, QuestMachineMessages.DespawnSpawnerMessage, spawnerName);
        }

        protected virtual void UnregisterWithMessageSystem()
        {
            MessageSystem.RemoveListener(this);
        }

        public virtual void OnMessage(MessageArgs messageArgs)
        {
            switch (messageArgs.message)
            {
                case QuestMachineMessages.StartSpawnerMessage:
                    StartSpawning();
                    break;
                case QuestMachineMessages.StopSpawnerMessage:
                    StopSpawning();
                    break;
                case QuestMachineMessages.DespawnSpawnerMessage:
                    DespawnAll();
                    break;
            }
        }

        #endregion

        #region Spawning

        /// <summary>
        /// Starts spawning. May stop automatically if stopWhenMinReached is true.
        /// </summary>
        public virtual void StartSpawning()
        {
            StartCoroutine(SpawnCoroutine());
        }

        /// <summary>
        /// Stops spawning.
        /// </summary>
        public virtual void StopSpawning()
        {
            StopAllCoroutines();
        }

        /// <summary>
        /// Stops spawning and despawns all spawned entities.
        /// </summary>
        public virtual void DespawnAll()
        {
            StopAllCoroutines();
            for (int i = 0; i < spawnedEntities.Count; i++)
            {
                var spawnedEntity = spawnedEntities[i];
                if (spawnedEntity == null) continue;
                spawnedEntity.disabled -= OnSpawnedEntityDisabled;
                RemoveSpawnedEntity(spawnedEntity);
                DespawnEntity(spawnedEntity);
            }
        }

        /// <summary>
        /// This coroutine runs until killed, spawning entities so the count
        /// remains between min and max.
        /// </summary>
        protected virtual IEnumerator SpawnCoroutine()
        {
            SetupSpawnpoints();

            // Keep spawning until made to stop:
            var secondsToWait = new WaitForSeconds(spawnRate);
            while (true)
            {
                // Fill out min count:
                for (int i = spawnCount; i < min; i++)
                {
                    SpawnAndPlaceEntity();
                }
                if (stopWhenMinReached) yield break;

                // Then keep spawning at the specified rate if under max count:
                yield return secondsToWait;
                if (spawnCount < max) SpawnAndPlaceEntity();
            }
        }

        /// <summary>
        /// Prepares for spawning.
        /// </summary>
        protected virtual void SetupSpawnpoints()
        {
            // If spawning around radius, use the existing spawnedEntities list:
            if (positionInfo.positionType == PositionInfo.PositionType.Radius)
            {
                spawnCount = spawnedEntities.Count;
                for (int i = 0; i < spawnedEntities.Count; i++)
                {
                    var spawnedEntity = spawnedEntities[i];
                    if (spawnedEntity == null) continue;
                    spawnedEntity.disabled -= OnSpawnedEntityDisabled;
                    spawnedEntity.disabled += OnSpawnedEntityDisabled;
                }
                return;
            }

            spawnCount = 0;
            // If spawning at spawnpoints, allocate empty spaces in the spawnedEntities list:
            for (int i = spawnedEntities.Count; i < positionInfo.spawnpoints.Length; i++)
            {
                spawnedEntities.Add(null);
            }

            // If spawnpoints point to any SpawnedEntities, record them in the spawnedEntities 
            // list and replace them in the spawnpoints list with an empty GameObject.
            for (int i = 0; i < positionInfo.spawnpoints.Length; i++)
            {
                var spawnpoint = positionInfo.spawnpoints[i];
                if (spawnpoint == null) continue;
                var spawnedEntity = spawnpoint.GetComponent<SpawnedEntity>();
                if (spawnedEntity != null)
                {
                    RemoveSpawnedEntity(spawnedEntities[i]);
                    spawnedEntities[i] = spawnedEntity;
                    spawnedEntity.disabled -= OnSpawnedEntityDisabled;
                    spawnedEntity.disabled += OnSpawnedEntityDisabled;
                    var emptyGameObject = new GameObject("Spawnpoint " + i);
                    emptyGameObject.transform.SetParent(this.transform);
                    emptyGameObject.transform.position = spawnedEntity.transform.position;
                    emptyGameObject.transform.rotation = spawnedEntity.transform.rotation;
                    positionInfo.spawnpoints[i] = emptyGameObject;
                    spawnCount++;
                }
            }
        }

        /// <summary>
        /// Spawns an entity and places it in the scene.
        /// </summary>
        protected virtual void SpawnAndPlaceEntity()
        {
            if (!IsThereSpaceForEntity()) return;
            var spawnedEntity = SpawnEntity();
            if (spawnedEntity == null) return;
            spawnedEntity.disabled -= OnSpawnedEntityDisabled;
            spawnedEntity.disabled += OnSpawnedEntityDisabled;
            PlaceSpawnedEntity(spawnedEntity);
        }

        /// <summary>
        /// Checks if there is space (e.g., an available spawnpoint) for the entity.
        /// </summary>
        protected virtual bool IsThereSpaceForEntity()
        {
            if (positionInfo.positionType == PositionInfo.PositionType.Radius) return true;
            for (int i = 0; i < spawnedEntities.Count; i++)
            {
                if (spawnedEntities[i] == null) return true;
            }
            return false;
        }

        /// <summary>
        /// Places a spawned entity in the scene.
        /// </summary>
        protected virtual void PlaceSpawnedEntity(SpawnedEntity spawnedEntity)
        {
            if (spawnedEntity == null) return;
            if (positionInfo.positionType == PositionInfo.PositionType.Radius)
            {
                PlaceSpawnedEntityInRadius(spawnedEntity);
            }
            else
            {
                PlaceSpawnedEntityInSpawnpoint(spawnedEntity);
            }
        }

        /// <summary>
        /// Places an entity within the specified radius of the spawner.
        /// </summary>
        protected void PlaceSpawnedEntityInRadius(SpawnedEntity spawnedEntity)
        {
            var rand1 = UnityEngine.Random.Range(-positionInfo.radius, positionInfo.radius);
            var rand2 = UnityEngine.Random.Range(-positionInfo.radius, positionInfo.radius);
            var position = (positionInfo.plane == PositionInfo.Plane.X_Z)
                ? new Vector3(transform.position.x + rand1, transform.position.y, transform.position.z + rand2)
                : new Vector3(transform.position.x + rand1, transform.position.y + rand2, transform.position.z);
            spawnedEntity.transform.position = position;
            spawnedEntities.Add(spawnedEntity);
        }

        /// <summary>
        /// Places an entity at an available spawnpoint.
        /// </summary>
        protected virtual void PlaceSpawnedEntityInSpawnpoint(SpawnedEntity spawnedEntity) // [TODO] Use random available spawnpoint.
        {
            for (int i = 0; i < spawnedEntities.Count; i++)
            {
                if (spawnedEntities[i] == null)
                {
                    spawnedEntities[i] = spawnedEntity;
                    var spawnpoint = positionInfo.spawnpoints[i];
                    if (spawnpoint != null)
                    {
                        spawnedEntity.transform.position = spawnpoint.transform.position;
                        spawnedEntity.transform.rotation = spawnpoint.transform.rotation;
                    }
                    return;
                }
            }
        }

        /// <summary>
        /// Removes an entity from the spawnedEntities list.
        /// </summary>
        /// <param name="spawnedEntity"></param>
        protected virtual void RemoveSpawnedEntity(SpawnedEntity spawnedEntity)
        {
            if (spawnedEntity == null) return;
            if (positionInfo.positionType == PositionInfo.PositionType.Radius)
            {
                // If using radius, just remove from list:
                spawnedEntities.Remove(spawnedEntity);
            }
            else
            {
                // If using spawnpoints, assign null to the list element.
                for (int i = 0; i < spawnedEntities.Count; i++)
                {
                    if (spawnedEntities[i] == spawnedEntity)
                    {
                        spawnedEntities[i] = null;
                    }
                }
            }
        }

        /// <summary>
        /// Spawns an entity and returns the SpawnedEntity component on it, adding
        /// the component if necessary.
        /// </summary>
        /// <returns></returns>
        protected virtual SpawnedEntity SpawnEntity() //[TODO] Use weights.
        {
            var prefab = prefabs[UnityEngine.Random.Range(0, prefabs.Length)];
            if (prefab == null)
            {
                if (Debug.isDebugBuild) Debug.LogWarning("Quest Machine: A prefab entry is blank in this spawner. Not spawning.", this);
                return null;
            }
            var instance = Instantiate<GameObject>(prefab.prefab);
            instance.transform.SetParent(this.transform);
            return instance.GetComponent<SpawnedEntity>() ?? instance.AddComponent<SpawnedEntity>();
        }

        /// <summary>
        /// Despawns an entity.
        /// </summary>
        /// <param name="spawnedEntity"></param>
        protected virtual void DespawnEntity(SpawnedEntity spawnedEntity)
        {
            Destroy(spawnedEntity);
        }

        /// <summary>
        /// Invoked by a SpawnedEntity when it's disabled. Removes it from the spawnedEntities list.
        /// </summary>
        /// <param name="spawnedEntity"></param>
        protected virtual void OnSpawnedEntityDisabled(SpawnedEntity spawnedEntity)
        {
            RemoveSpawnedEntity(spawnedEntity);
        }

        #endregion

    }

}