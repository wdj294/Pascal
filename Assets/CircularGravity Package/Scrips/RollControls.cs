/*******************************************************************************************
 *       Author: Lane Gresham, AKA LaneMax
 *         Blog: http://lanemax.blogspot.com/
 *      Version: 2.50
 * Created Date: 04/15/13 
 * Last Updated: 09/14/13
 *  
 *  Description: 
 *  
 *      Controls for Marble.
 * 
 * 
 *  Inputs:
 *      
 *      movementForce: Movement force/speed.
 * 
*******************************************************************************************/
using UnityEngine;
using System.Collections;

namespace CircularGravityForce
{
    namespace CircularGravityForce
    {
        public class RollControls : MonoBehaviour
        {
            #region Properties

            //Movement force/speed
            public float movementForce = 3.0f;

            #endregion

            #region Unity Functions

            // Use this for initialization
            void Start()
            {

            }

            // Update is called once per frame
            void Update()
            {

            }

            void FixedUpdate()
            {
                //Right Left controls
                float horMovement = movementForce * Input.GetAxis("Horizontal");

                //Forward Backward controls
                float verMovement = movementForce * Input.GetAxis("Vertical");

                //Up Down controls. Note: For Zero G 
                float floatMovement = movementForce * Input.GetAxis("Mouse ScrollWheel");

                if (horMovement != 0)
                {
                    this.transform.GetComponent<Rigidbody>().AddForce(new Vector3(horMovement, 0, 0), ForceMode.Impulse);
                }

                if (verMovement != 0)
                {
                    this.transform.GetComponent<Rigidbody>().AddForce(new Vector3(0, 0, verMovement), ForceMode.Impulse);
                }

                if (floatMovement != 0)
                {
                    this.transform.GetComponent<Rigidbody>().AddForce(new Vector3(0, floatMovement * 4, 0), ForceMode.Impulse);
                }
            }

            #endregion
        }
    }
}