/*******************************************************************************************
 *       Author: Lane Gresham, AKA LaneMax
 *         Blog: http://lanemax.blogspot.com/
 *      Version: 2.50
 * Created Date: 04/15/13 
 * Last Updated: 09/14/13
 *  
 *  Description: 
 *  
 *      Sets the gravity.
  * 
 *  Inputs:
 * 
 *      worldGravity: Used for setting gravity, defaults to zero g.
 * 
*******************************************************************************************/
using UnityEngine;
using System.Collections;

namespace CircularGravityForce
{
    public class WorldGravity : MonoBehaviour
    {
        #region Properties

        //Used for setting gravity, defaults to zero g
        public Vector3 worldGravity = new Vector3(0f, 0f, 0f);

        #endregion

        #region Unity Functions

        //Use this for initialization
        void Start()
        {
            Physics.gravity = worldGravity;
        }

        //Update is called once per frame
        void Update()
        {

        }

        #endregion
    }
}