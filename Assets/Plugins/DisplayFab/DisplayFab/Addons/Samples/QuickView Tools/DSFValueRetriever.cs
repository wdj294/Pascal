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
    /// This is an example of how to display a result for a source, given tsUID and fpmUID.
    /// Primarily used for quick debugging and for learning purposes. Not used by DisplayFab
    /// </summary>
    [DSFHide]
    public  class DSFValueRetriever: MonoBehaviour
    {
      [Multiline]
      public string result;

      public int sourceTSUID;
      public int sourceFPMUID;

      public DisplayFabSystem _dispFabRef;

        /// <summary>
        /// Retrieves a value for the above source tsUID and fpmUID pair and displays it in the result string
        /// </summary>
      [ContextMenu("Retrieve Value")]
      void RetrieveValue()
      {
        if(_dispFabRef==null) return;

            object objResult = _dispFabRef.GetSourceValueByUID(sourceTSUID, sourceFPMUID); 
            if(objResult != null) result= objResult.ToString();
      }


    }
}
