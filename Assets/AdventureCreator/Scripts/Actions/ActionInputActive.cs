/*
 *
 *	Adventure Creator
 *	by Chris Burton, 2013-2016
 *	
 *	"ActionInputCheck.cs"
 * 
 *	This action checks if a specific key
 *	is being pressed
 * 
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AC
{
	
	[System.Serializable]
	public class ActionInputActive : Action
	{

		public int activeInputID;
		public bool newState;

		
		public ActionInputActive ()
		{
			this.isDisplayed = true;
			category = ActionCategory.Input;
			title = "Toggle active";
			description = "Enables or disables an Active Input";
		}


		override public float Run ()
		{
			if (KickStarter.settingsManager.activeInputs != null)
			{
				foreach (ActiveInput activeInput in KickStarter.settingsManager.activeInputs)
				{
					if (activeInput.ID == activeInputID)
					{
						activeInput.IsEnabled = newState;
						break;
					}
				}
			}

			return 0f;
		}
		

		
		#if UNITY_EDITOR
		
		override public void ShowGUI (List<ActionParameter> parameters)
		{
			int tempNumber = -1;

			if (KickStarter.settingsManager != null && KickStarter.settingsManager.activeInputs != null && KickStarter.settingsManager.activeInputs.Count > 0)
			{
				ActiveInput.Upgrade ();

				string[] labelList = new string[KickStarter.settingsManager.activeInputs.Count];
				for (int i=0; i<KickStarter.settingsManager.activeInputs.Count; i++)
				{
					labelList[i] = KickStarter.settingsManager.activeInputs[i].inputName;

					if (KickStarter.settingsManager.activeInputs[i].ID == activeInputID)
					{
						tempNumber = i;
					}
				}

				if (tempNumber == -1)
				{
					// Wasn't found (was deleted?), so revert to zero
					if (activeInputID != 0)
						ACDebug.LogWarning ("Previously chosen active input no longer exists!");
					tempNumber = 0;
					activeInputID = 0;
				}

				tempNumber = EditorGUILayout.Popup (tempNumber, labelList);
				activeInputID = KickStarter.settingsManager.activeInputs [tempNumber].ID;
				newState = EditorGUILayout.Toggle ("New state:", newState);
			}
			else
			{
				EditorGUILayout.HelpBox ("No active inputs exist! They can be defined in Adventure Creator -> Editors -> Active Inputs.", MessageType.Info);
				activeInputID = 0;
				tempNumber = 0;
			}

			AfterRunningOption ();
		}
		
		#endif
		
	}
	
}