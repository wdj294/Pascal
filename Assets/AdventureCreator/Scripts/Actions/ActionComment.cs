/*
 *
 *	Adventure Creator
 *	by Chris Burton, 2013-2017
 *	
 *	"ActionComment.cs"
 * 
 *	This action simply displays a comment in the Editor / Inspector.
 * 
 */

using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AC
{
	
	[System.Serializable]
	public class ActionComment : Action
	{
		
		public string commentText = "";
		public bool outputToDebugger;

		private string convertedText;
		
		
		public ActionComment ()
		{
			this.isDisplayed = true;
			category = ActionCategory.ActionList;
			title = "Comment";
			description = "Prints a comment for debug purposes.";
		}


		public override void AssignValues (System.Collections.Generic.List<ActionParameter> parameters)
		{
			convertedText = AdvGame.ConvertTokens (commentText, 0, null, parameters);
		}


		public override float Run ()
		{
			if (outputToDebugger && !string.IsNullOrEmpty (convertedText))
			{
				ACDebug.Log (convertedText);
			}
			return 0f;
		}
		
		
		#if UNITY_EDITOR
		
		override public void ShowGUI ()
		{
			EditorStyles.textField.wordWrap = true;
			commentText = EditorGUILayout.TextArea (commentText, GUILayout.MaxWidth (280f));

			outputToDebugger = EditorGUILayout.Toggle ("Print in Console?", outputToDebugger);
			
			AfterRunningOption ();
		}
		
		
		public override string SetLabel ()
		{
			if (!string.IsNullOrEmpty (commentText))
			{
				int i = commentText.IndexOf ("\n");
				if (i > 0)
				{
					return (" (" + commentText.Substring (0, i) + ")");
				}
				return (" (" + commentText + ")");
			}
			return "";
		}
		
		#endif
		
	}
	
}