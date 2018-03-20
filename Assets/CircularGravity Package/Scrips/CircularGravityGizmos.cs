/*******************************************************************************************
 *       Author: Lane Gresham, AKA LaneMax
 *         Blog: http://lanemax.blogspot.com/
 *      Version: 2.50
 * Created Date: 12/20/13 
 * Last Updated: 04/21/14
 *  
 *  Description: 
 *  
 *      Allows you to see the Circular Gravity Force in EditMode/Gizmos.
 *      
 *  How To Use:
 *  
 *      Simply drag and drop / assign this script to whatever GameObject that has the 
 *      component CircularGravity, then you should see the CircularGravity in EditMode.
 * 
 *  Inputs:
 * 
 *      none
 *          
*******************************************************************************************/
using UnityEngine;
using System.Collections;

namespace CircularGravityForce
{
    [ExecuteInEditMode]
    public class CircularGravityGizmos : MonoBehaviour
    {
        #region Properties

        public bool enable = true;

        private CircularGravity circularGravity;

        #endregion

        #region Unity Functions

        void Start()
        {

        }

        void OnDrawGizmos()
        {
            if (enable)
            {
                circularGravity = this.GetComponent<CircularGravity>();
                DrawGravityForceGizmos();
            }
        }

        #endregion

        #region Functions

        //Draws effected area by forces with debug draw line, so you can see it in Gizmos
        private void DrawGravityForceGizmos()
        {
            //Circular Gravity Force Transform
            Transform cgfTran = this.transform;

            Color DebugGravityLineColor;

            if (circularGravity.forcePower == 0)
                DebugGravityLineColor = Color.white;
            else if (circularGravity.forcePower > 0)
                DebugGravityLineColor = Color.green;
            else
                DebugGravityLineColor = Color.red;

            //Renders type outline
            switch (circularGravity.shape)
            {
                case CircularGravity.Shape.Sphere:

                    Debug.DrawLine(cgfTran.position + ((cgfTran.rotation * Vector3.up) * circularGravity.size), cgfTran.position, DebugGravityLineColor);
                    Debug.DrawLine(cgfTran.position + ((cgfTran.rotation * Vector3.down) * circularGravity.size), cgfTran.position, DebugGravityLineColor);
                    Debug.DrawLine(cgfTran.position + ((cgfTran.rotation * Vector3.left) * circularGravity.size), cgfTran.position, DebugGravityLineColor);
                    Debug.DrawLine(cgfTran.position + ((cgfTran.rotation * Vector3.right) * circularGravity.size), cgfTran.position, DebugGravityLineColor);
                    Debug.DrawLine(cgfTran.position + ((cgfTran.rotation * Vector3.forward) * circularGravity.size), cgfTran.position, DebugGravityLineColor);
                    Debug.DrawLine(cgfTran.position + ((cgfTran.rotation * Vector3.back) * circularGravity.size), cgfTran.position, DebugGravityLineColor);

                    break;

                case CircularGravity.Shape.Capsule:

                    Debug.DrawLine(cgfTran.position + ((cgfTran.rotation * Vector3.up) * circularGravity.capsuleRadius), cgfTran.position, DebugGravityLineColor);
                    Debug.DrawLine(cgfTran.position + ((cgfTran.rotation * Vector3.down) * circularGravity.capsuleRadius), cgfTran.position, DebugGravityLineColor);
                    Debug.DrawLine(cgfTran.position + ((cgfTran.rotation * Vector3.left) * circularGravity.capsuleRadius), cgfTran.position, DebugGravityLineColor);
                    Debug.DrawLine(cgfTran.position + ((cgfTran.rotation * Vector3.right) * circularGravity.capsuleRadius), cgfTran.position, DebugGravityLineColor);

                    Vector3 endPointLoc = cgfTran.position + ((cgfTran.rotation * Vector3.forward) * circularGravity.size);

                    Debug.DrawLine(cgfTran.position, endPointLoc, DebugGravityLineColor);

                    Debug.DrawLine(endPointLoc + ((cgfTran.rotation * Vector3.up) * circularGravity.capsuleRadius), endPointLoc, DebugGravityLineColor);
                    Debug.DrawLine(endPointLoc + ((cgfTran.rotation * Vector3.down) * circularGravity.capsuleRadius), endPointLoc, DebugGravityLineColor);
                    Debug.DrawLine(endPointLoc + ((cgfTran.rotation * Vector3.left) * circularGravity.capsuleRadius), endPointLoc, DebugGravityLineColor);
                    Debug.DrawLine(endPointLoc + ((cgfTran.rotation * Vector3.right) * circularGravity.capsuleRadius), endPointLoc, DebugGravityLineColor);

                    break;

                case CircularGravity.Shape.RayCast:

                    Debug.DrawLine(cgfTran.position + ((cgfTran.rotation * Vector3.forward) * circularGravity.size), cgfTran.position, DebugGravityLineColor);

                    break;
            }
        }

        #endregion
    }
}