// Copyright © Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// This helper component invokes a delegate method when disabled.
    /// </summary>
    [AddComponentMenu("")]
    public class SpawnedEntity : MonoBehaviour
    {
        public delegate void SpawnedObjectDelegate(SpawnedEntity spawnedEntity);

        public event SpawnedObjectDelegate disabled = delegate { };

        private void OnDisable()
        {
            disabled(this);
        }
    }
}