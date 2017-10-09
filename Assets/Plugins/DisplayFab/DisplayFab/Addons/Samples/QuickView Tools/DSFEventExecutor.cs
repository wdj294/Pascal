/******************************
DisplayFab by Techooka Labs 
http://displayfab.techooka.com
******************************/


using System;
using System.Collections.Generic;
using UnityEngine;



namespace Techooka.DisplayFab.QuickViewTools
{
    /// <summary>
    /// This is an example of how to execute an Event, given an eventUID. 
    /// Primarily used for quick debugging and for learning purposes. Not used by DisplayFab
    /// </summary>
    [DSFHide]
    public class DSFEventExecutor : MonoBehaviour
    {
        
        public int linkUpUID;
        public DisplayFabSystem _dispFabRef;

        /// <summary>
        /// Executes the above eventUID
        /// </summary>
        [ContextMenu("Execute")]
        void Execute()
        {
            if (_dispFabRef == null) return;

            _dispFabRef.ExecuteLinkUp(linkUpUID); 

        }

    }
}
