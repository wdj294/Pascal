/*******************************************************************************************
 *       Author: Lane Gresham, AKA LaneMax
 *         Blog: http://lanemax.blogspot.com/
 *      Version: 2.50
 * Created Date: 04/15/13 
 * Last Updated: 04/21/14
 *  
 *  Description: 
 *  
 *      Used to rotate around a given object.
  * 
 *  Inputs:
 * 
 *      rotateAroundObject: Rotates around this GameObject.
 *      
 *      speed: Speed of rotation.
 *      
 *      axis: Axis of the rotation.
 * 
*******************************************************************************************/
using System.Collections;
using UnityEngine;

namespace CircularGravityForce
{
    public class RotateAround : MonoBehaviour
    {
        #region Properties

        //Rotates around this GameObject 
        public GameObject rotateAroundObject;

        //Speed of rotation
        public float speed = 10f;

        //Axis of the rotation
        public Vector3 axis = Vector3.up;

        #endregion

        #region Unity Functions

        //Use this for initialization
        void Start()
        {

        }

        //Update is called once per frame
        void Update()
        {
            this.transform.RotateAround(rotateAroundObject.transform.position, axis, speed * Time.deltaTime);
        }

        #endregion
    }
}