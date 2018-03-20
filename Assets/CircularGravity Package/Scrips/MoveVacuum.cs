/*******************************************************************************************
 *       Author: Lane Gresham, AKA LaneMax
 *         Blog: http://lanemax.blogspot.com/
 *      Version: 2.50
 * Created Date: 05/18/13
 * Last Updated: 09/14/13
 *  
 *  Description: 
 *  
 *      Controls for Vacuum.
 * 
 * 
 *  Inputs:
 * 
 *      inputCircularGravity: Input control to move the vacuum.
 *      
 *      movementSpeed: Movement speed.
 * 
*******************************************************************************************/
using UnityEngine;
using System.Collections;

namespace CircularGravityForce
{
    public class MoveVacuum : MonoBehaviour
    {
        #region Properties

        public float movementSpeed;

        #endregion

        #region Unity Functions

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            float horMovement = movementSpeed * Input.GetAxis("Horizontal");
            float verMovement = movementSpeed * Input.GetAxis("Vertical");

            if (horMovement != 0)
            {
                this.transform.Translate(new Vector3(horMovement * Time.deltaTime, 0, 0));
            }

            if (verMovement != 0)
            {
                this.transform.Translate(new Vector3(0, 0, verMovement * Time.deltaTime));
            }
        }

        #endregion
    }
}