// Copyright © Pixel Crushers. All rights reserved.

using UnityEngine;
using System;

namespace PixelCrushers
{

    /// <summary>
    /// Always keeps the GameObject facing the main camera.
    /// </summary>
    [AddComponentMenu("")] // Use wrapper instead.
    public class AlwaysFaceCamera : MonoBehaviour
    {

        [Tooltip("Leave Y rotation untouched.")]
        [SerializeField]
        private bool m_yAxisOnly = false;

        /// <summary>
        /// Set `true` to leave Y rotation untouched.
        /// </summary>
        public bool yAxisOnly
        {
            get { return m_yAxisOnly; }
            set { m_yAxisOnly = value; }
        }

        private void Update()
        {
            if (Camera.main == null) return;
            if (yAxisOnly)
            {
                transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, Camera.main.transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.x);
            }
            else
            {
                transform.rotation = Camera.main.transform.rotation;
            }
        }

    }

}
