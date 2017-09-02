/*
 *
 *	Adventure Creator
 *	by Chris Burton, 2013-2017
 *	
 *	"RememberTimeline.cs"
 * 
 *	This script is attached to PlayableDirector objects in the scene
 *	we wish to save (Unity 2017+ only).
 * 
 */

using UnityEngine;
using System.Collections;

#if UNITY_2017_1_OR_NEWER
using UnityEngine.Timeline;
using UnityEngine.Playables;
#endif

namespace AC
{

	/**
	 * Attach this script to PlayableDirector objects you wish to save.
	 */
	#if UNITY_2017_1_OR_NEWER
	[RequireComponent (typeof (PlayableDirector))]
	#endif
	[AddComponentMenu("Adventure Creator/Save system/Remember Timeline")]
	#if !(UNITY_4_6 || UNITY_4_7 || UNITY_5_0)
	[HelpURL("http://www.adventurecreator.org/scripting-guide/class_a_c_1_1_remember_timeline.html")]
	#endif
	public class RememberTimeline : Remember
	{

		/**
		 * <summary>Serialises appropriate GameObject values into a string.</summary>
		 * <returns>The data, serialised as a string</returns>
		 */
		public override string SaveData ()
		{
			TimelineData timelineData = new TimelineData ();
			timelineData.objectID = constantID;
			timelineData.savePrevented = savePrevented;

			#if UNITY_2017_1_OR_NEWER
			PlayableDirector director = GetComponent <PlayableDirector>();
			timelineData.isPlaying = (director.state == PlayState.Playing);
			timelineData.currentTime = director.time;
			#else
			ACDebug.LogWarning ("The 'Remember Director' component is only compatible with Unity 5.6 onward.", this);
			#endif

			return Serializer.SaveScriptData <TimelineData> (timelineData);
		}
		

		/**
		 * <summary>Deserialises a string of data, and restores the GameObject to its previous state.</summary>
		 * <param name = "stringData">The data, serialised as a string</param>
		 * <param name = "restoringSaveFile">True if the game is currently loading a saved game file, as opposed to just switching scene</param>
		 */
		public override void LoadData (string stringData, bool restoringSaveFile = false)
		{
			TimelineData data = Serializer.LoadScriptData <TimelineData> (stringData);
			if (data == null) return;
			SavePrevented = data.savePrevented; if (savePrevented) return;

			#if UNITY_2017_1_OR_NEWER
			PlayableDirector director = GetComponent <PlayableDirector>();
			director.time = data.currentTime;
			if (data.isPlaying)
			{
				director.Play ();
			}
			else
			{
				director.Stop ();
			}
			#else
			ACDebug.LogWarning ("The 'Remember Director' component is only compatible with Unity 5.6 onward.", this);
			#endif
		}
		
	}
	

	/**
	 * A data container used by the RememberTimeline script.
	 */
	[System.Serializable]
	public class TimelineData : RememberData
	{

		public bool isPlaying;
		public double currentTime;

		
		/**
		 * The default Constructor.
		 */
		public TimelineData () { }

	}
	
}