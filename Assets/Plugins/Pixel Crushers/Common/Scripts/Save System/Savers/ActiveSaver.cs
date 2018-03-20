// Copyright © Pixel Crushers. All rights reserved.

using UnityEngine;
using System;

namespace PixelCrushers
{

    /// <summary>
    /// Saves the active/inactive state of a GameObject. This component should be
    /// on a different GameObject that's guaranteed to be active, or it won't
    /// take effect.
    /// </summary>
    [AddComponentMenu("")]
    public class ActiveSaver : Saver
    {

        [Serializable]
        public class Data
        {
            public bool active;
        }

        [Tooltip("GameObject to watch.")]
        [SerializeField]
        private GameObject m_gameObjectToWatch;

        public GameObject gameObjectToWatch
        {
            get { return m_gameObjectToWatch; }
            set { m_gameObjectToWatch = value; }
        }

        public override string RecordData()
        {
            var value = (gameObjectToWatch != null) ? gameObjectToWatch.activeSelf : false;
            var data = new Data();
            data.active = value;
            return SaveSystem.Serialize(data);
        }

        public override void ApplyData(string data)
        {
            if (gameObjectToWatch == null || string.IsNullOrEmpty(data)) return;
            var dataObject = SaveSystem.Deserialize<Data>(data);
            if (dataObject == null) return;
            gameObjectToWatch.SetActive(dataObject.active);
        }

    }
}
