/*******************************************************************************************
 *       Author: Lane Gresham, AKA LaneMax
 *         Blog: http://lanemax.blogspot.com/
 *      Version: 2.50
 * Created Date: 11/24/13 
 * Last Updated: 11/24/13
 *  
 *  Description: 
 *  
 *      Enables CircularGravity with a control.
 *      
 *  Inputs:
 * 
 *      inputCircularGravity: Input control to shoot
 *      
 *      movementForce: Movement force/speed.
 * 
*******************************************************************************************/
using UnityEngine;
using System.Collections;

namespace CircularGravityForce
{
    public class EnableCircularGravityControls : MonoBehaviour
    {
        #region Properties

        //Input control to shoot
        public string inputCircularGravity = "Jump";

        #endregion

        #region Unity Functions

        // Use this for initialization
        void Start()
        {
            EnableCircularGravity(false);
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetButton(inputCircularGravity))
            {
                EnableCircularGravity(true);
            }
            else
            {
                EnableCircularGravity(false);
            }
        }

        #endregion

        #region Functions

        //Enables/Disable the circular gravity
        private void EnableCircularGravity(bool enable)
        {
            CircularGravity circularGravity = this.GetComponent<CircularGravity>();

            circularGravity.enable = enable;
        }

        #endregion
    }
}