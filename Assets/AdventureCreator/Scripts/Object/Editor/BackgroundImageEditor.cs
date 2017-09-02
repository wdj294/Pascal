using UnityEngine;
using System.Collections;
using UnityEditor;

namespace AC
{

	#if UNITY_STANDALONE && (UNITY_5 || UNITY_2017_1_OR_NEWER || UNITY_PRO_LICENSE)
	[CustomEditor (typeof (BackgroundImage))]
	public class BackgroundImageEditor : Editor
	{
		
		private BackgroundImage _target;
		
		
		private void OnEnable ()
		{
			_target = (BackgroundImage) target;
		}
		
		
		public override void OnInspectorGUI ()
		{
			EditorGUILayout.BeginVertical ("Button");
			EditorGUILayout.LabelField ("When playing a MovieTexture:");
			_target.loopMovie = EditorGUILayout.Toggle ("Loop clip?", _target.loopMovie);
			_target.restartMovieWhenTurnOn = EditorGUILayout.Toggle ("Restart clip each time?", _target.restartMovieWhenTurnOn);
			EditorGUILayout.EndVertical ();
		
			UnityVersionHandler.CustomSetDirty (_target);
		}

	}
	#endif

}