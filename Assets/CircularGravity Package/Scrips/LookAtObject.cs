/*******************************************************************************************
 *       Author: Lane Gresham, AKA LaneMax
 *         Blog: http://lanemax.blogspot.com/
 *      Version: 2.50
 * Created Date: 04/15/13 
 * Last Updated: 09/14/13
 *  
 *  Description: 
 *  
 *      Allows the camera to look at the given object.
 * 
 * 
 *  Inputs:
 * 
 *      lookAtObject: Looks at whatever GameObejct is assigned to it.
 * 
*******************************************************************************************/
using UnityEngine;
using System.Collections;

namespace CircularGravityForce
{
    public class LookAtObject : MonoBehaviour
    {
        #region Properties

        //Looks at whatever GameObejct is assigned to it
        public GameObject lookAtObject;

        #endregion

        #region Unity Functions

        //Use this for initialization
        void Start()
        {

        }

        //Update is called once per frame
        void Update()
        {
            Camera.main.transform.LookAt(lookAtObject.transform);
        }

        #endregion
    }
}