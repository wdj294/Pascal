// Copyright © Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers
{

    /// <summary>
    /// Provides more routines for Physics2D.
    /// </summary>
    public static class MorePhysics2D
    {

        /// Platform-dependent wrapper for Physics2D.raycastsStartInColliders (pre 5.2.1p2) and
        /// queriesStartInColliders (5.2.1p2+).
        public static bool queriesStartInColliders
        {
#if UNITY_4_6 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2_1
            get
            {
                return Physics2D.raycastsStartInColliders;
            }
            set
            {
                Physics2D.raycastsStartInColliders = value;
            }
#else
            get
            {
                return Physics2D.queriesStartInColliders;
            }
            set
            {
                Physics2D.queriesStartInColliders = value;
            }
#endif
        }

        /// <summary>
        /// Size of the preallocated array for nonallocating raycasts.
        /// </summary>
        public static int maxRaycastResults
        {
            get
            {
                return raycastResults.Length;
            }
            set
            {
                if (value != raycastResults.Length)
                {
                    raycastResults = new RaycastHit2D[value];
                }
            }
        }

        private static RaycastHit2D[] raycastResults = new RaycastHit2D[20];

        /// <summary>
        /// Runs a nonallocating linecast, ignoring the source.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <param name="layerMask"></param>
        /// <returns></returns>
        public static GameObject Raycast2DWithoutSelf(Transform source, Transform destination, LayerMask layerMask)
        {
            var start2D = new Vector2(source.position.x, source.position.y);
            var end2D = new Vector2(destination.position.x, destination.position.y);
            var originalRaycastsStartInColliders = MorePhysics2D.queriesStartInColliders;
            MorePhysics2D.queriesStartInColliders = false;
            var numResults = Physics2D.LinecastNonAlloc(start2D, end2D, raycastResults, layerMask);
            MorePhysics2D.queriesStartInColliders = originalRaycastsStartInColliders;
            for (int i = 0; i < numResults; i++)
            {
                var result = raycastResults[i];
                if (result.transform == source) continue; // Skip source.
                return result.collider.gameObject; // Array is in distance order, so return first non-source.
            }
            return null;
        }

    }

}
