// Copyright © Pixel Crushers. All rights reserved.

using UnityEngine;
using System;

namespace PixelCrushers
{

    /// <summary>
    /// Saves a GameObject's position.
    /// </summary>
    [AddComponentMenu("")] // Use wrapper instead.
    public class PositionSaver : Saver
    {

        [Serializable]
        public class PositionData
        {
            public Vector3 position;
            public Quaternion rotation;
        }

        public override string RecordData()
        {
            var data = new PositionData();
            data.position = transform.position;
            data.rotation = transform.rotation;
            return SaveSystem.Serialize(data);
        }

        public override void ApplyData(string data)
        {
            if (string.IsNullOrEmpty(data)) return;
            var positionData = SaveSystem.Deserialize<PositionData>(data);
            if (positionData == null) return;
            transform.position = positionData.position;
            transform.rotation = positionData.rotation;
        }

    }
}
