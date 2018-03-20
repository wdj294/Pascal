/*******************************************************************************************
 *       Author: Lane Gresham, AKA LaneMax
 *         Blog: http://lanemax.blogspot.com/
 *      Version: 2.50
 * Created Date: 04/15/13 
 * Last Updated: 09/14/13
 *  
 *  Description: 
 *  
 *      Allows to display text on the screen.
  * 
 *  Inputs:
 * 
 *      helpInfo: Displays strings onto screen.
 * 
*******************************************************************************************/
using UnityEngine;
using System.Collections;

namespace CircularGravityForce
{
    public class ShowHelpInfo : MonoBehaviour
    {
        #region Properties

        public string[] helpInfo;

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

        void OnGUI()
        {
            float row = 0;
            foreach (var str in helpInfo)
            {
                GUI.Label(new Rect(5, (row * 20) + 5, 1000, 22), str);

                row = row + 1f;
            }
        }

        #endregion
    }
}