/*
 *
 *	Adventure Creator
 *	by Chris Burton, 2013-2017
 *	
 *	"ActionDirector.cs"
 * 
 *	This action plays and stops controls Playable Directors
 * 
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_2017_1_OR_NEWER
using UnityEngine.Timeline;
using UnityEngine.Playables;
#endif

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AC
{
	
	[System.Serializable]
	public class ActionTimeline : Action
	{

		public bool disableCamera;

		#if UNITY_2017_1_OR_NEWER
		public PlayableDirector director;
		public int directorConstantID = 0;
		public int directorParameterID = -1;

		public enum ActionDirectorMethod { Play, Stop };
		public ActionDirectorMethod method = ActionDirectorMethod.Play;
		public bool restart = true;
		public bool pause = false;
		#endif

		
		public ActionTimeline ()
		{
			this.isDisplayed = true;
			category = ActionCategory.Engine;
			title = "Control Timeline";
			description = "Controls a Timeline.  This is only compatible with Unity 2017 or newer.";
		}
		
		
		override public void AssignValues (List<ActionParameter> parameters)
		{
			#if UNITY_2017_1_OR_NEWER
			director = AssignFile <PlayableDirector> (parameters, directorParameterID, directorConstantID, director);
			#endif
		}
		
		
		override public float Run ()
		{
			#if UNITY_2017_1_OR_NEWER
			if (!isRunning)
			{
				isRunning = true;

				if (director != null)
				{
					if (method == ActionDirectorMethod.Play)
					{
						isRunning = true;

						if (restart)
						{
							director.time = 0f;
							director.Play ();
						}
						else
						{
							director.Resume ();
						}


						if (willWait)
						{
							if (disableCamera)
							{
								KickStarter.mainCamera.Disable ();
							}
							return ((float) director.duration - (float) director.time);
						}
					}
					else if (method == ActionDirectorMethod.Stop)
					{
						if (disableCamera)
						{
							KickStarter.mainCamera.Enable ();
						}

						if (pause)
						{
							director.Pause ();
						}
						else
						{
							director.time = director.duration;
							director.Stop ();
						}
					}
				}
			}
			else
			{
				if (method == ActionDirectorMethod.Play && disableCamera)
				{
					KickStarter.mainCamera.Enable ();
				}

				isRunning = false;
			}
			#endif
			
			return 0f;
		}


		override public void Skip ()
		{
			#if UNITY_2017_1_OR_NEWER
			if (director != null)
			{
				if (disableCamera)
				{
					KickStarter.mainCamera.Enable ();
				}

				if (method == ActionDirectorMethod.Play)
				{
					if (director.extrapolationMode == DirectorWrapMode.Loop)
					{
						if (restart)
						{
							director.Play ();
						}
						else
						{
							director.Resume ();
						}
						return;
					}

					director.Stop ();
					director.time = director.duration;
				}
				else if (method == ActionDirectorMethod.Stop)
				{
					if (pause)
					{
						director.Pause ();
					}
					else
					{
						director.Stop ();
					}
				}
			}
			#endif
		}
		
		
		#if UNITY_EDITOR
		
		override public void ShowGUI (List<ActionParameter> parameters)
		{
			#if UNITY_2017_1_OR_NEWER
			method = (ActionDirectorMethod) EditorGUILayout.EnumPopup ("Method:", method);

			directorParameterID = Action.ChooseParameterGUI ("Director:", parameters, directorParameterID, ParameterType.GameObject);
			if (directorParameterID >= 0)
			{
				directorConstantID = 0;
				director = null;
			}
			else
			{
				director = (PlayableDirector) EditorGUILayout.ObjectField ("Director:", director, typeof (PlayableDirector), true);
				
				directorConstantID = FieldToID <PlayableDirector> (director, directorConstantID);
				director = IDToField <PlayableDirector> (director, directorConstantID, false);
			}

			if (method == ActionDirectorMethod.Play)
			{
				restart = EditorGUILayout.Toggle ("Play from beginning?", restart);
				willWait = EditorGUILayout.Toggle ("Wait until finish?", willWait);

				if (willWait)
				{
					disableCamera = EditorGUILayout.Toggle ("Disable AC camera?", disableCamera);
				}
			}
			else if (method == ActionDirectorMethod.Stop)
			{
				pause = EditorGUILayout.Toggle ("Pause timeline?", pause);
				disableCamera = EditorGUILayout.Toggle ("Enable AC camera?", disableCamera);
			}

			#else
			EditorGUILayout.HelpBox ("This Action is only compatible with Unity 5.6 or newer.", MessageType.Info);
			#endif

			AfterRunningOption ();
		}


		override public void AssignConstantIDs (bool saveScriptsToo)
		{
			#if UNITY_2017_1_OR_NEWER
			if (saveScriptsToo)
			{
				AddSaveScript <RememberTimeline> (director);
			}
			AssignConstantID <PlayableDirector> (director, directorConstantID, directorParameterID);
			#endif
		}

		
		public override string SetLabel ()
		{
			#if UNITY_2017_1_OR_NEWER
			if (director != null)
			{
				return " (" + method.ToString () + " " + director.gameObject.name + ")";
			}
			#endif
			return "";
		}
		
		#endif
	}
	
}