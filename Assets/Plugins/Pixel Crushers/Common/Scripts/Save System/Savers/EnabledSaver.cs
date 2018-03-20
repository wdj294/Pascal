// Copyright © Pixel Crushers. All rights reserved.

using UnityEngine;
using System;

namespace PixelCrushers
{

    /// <summary>
    /// Saves the enabled/disabled state of a component. This component should be
    /// on a GameObject that's guaranteed to be active, or it won't take effect.
    /// </summary>
    [AddComponentMenu("")]
    public class EnabledSaver : Saver
    {

        [Serializable]
        public class Data
        {
            public bool enabled;
        }

        [Tooltip("Component to watch.")]
        [SerializeField]
        private Component m_componentToWatch;

        public Component componentToWatch
        {
            get { return m_componentToWatch; }
            set { m_componentToWatch = value; }
        }

        public override string RecordData()
        {
            var value = (componentToWatch != null) ? ComponentUtility.IsComponentEnabled(componentToWatch) : false;
            var data = new Data();
            data.enabled = value;
            return SaveSystem.Serialize(data);
        }

        public override void ApplyData(string data)
        {
            if (componentToWatch == null || string.IsNullOrEmpty(data)) return;
            var dataObject = SaveSystem.Deserialize<Data>(data);
            if (dataObject == null) return;
            ComponentUtility.SetComponentEnabled(componentToWatch, dataObject.enabled);
        }

    }
}
