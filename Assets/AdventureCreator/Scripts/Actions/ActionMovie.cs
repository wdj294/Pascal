/*
 *
 *	Adventure Creator
 *	by Chris Burton, 2013-2017
 *	
 *	"ActionMovie.cs"
 * 
 *	Plays movie clips either on a Texture, or full-screen on mobile devices.
 * 
 */

#if UNITY_5_6_OR_NEWER
#define ALLOW_VIDEOPLAYER
#endif

#if (UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_TVOS)
#define ALLOW_HANDHELD
#elif UNITY_STANDALONE && (UNITY_5 || UNITY_2017_1_OR_NEWER || UNITY_PRO_LICENSE)
#define ALLOW_MOVIETEXTURES
#endif

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

#if ALLOW_VIDEOPLAYER
using UnityEngine.Video;
#endif

namespace AC
{
	
	[System.Serializable]
	public class ActionMovie : Action
	{
		
		public MovieClipType movieClipType = MovieClipType.FullScreen;
		public MovieMaterialMethod movieMaterialMethod = MovieMaterialMethod.PlayMovie;

		#if ALLOW_VIDEOPLAYER
		public VideoPlayer videoPlayer;
		public int videoPlayerParameterID = -1;
		public int videoPlayerConstantID;
		public bool prepareOnly = false;
		#endif

		#if ALLOW_HANDHELD

		public string filePath;

		#elif ALLOW_MOVIETEXTURES
		public Material material;
		public int materialParameterID = -1;

		public MovieTexture movieClip;
		public int movieClipParameterID = -1;

		public Sound sound;
		public int soundID = 0;

		public bool includeAudio;
		#endif

		public string skipKey;
		public bool canSkip;

		private GUITexture guiTexture;

		
		public ActionMovie ()
		{
			this.isDisplayed = true;
			title = "Play movie clip";
			category = ActionCategory.Engine;
			description = "Plays movie clips either on a Texture, or full-screen on mobile devices.";
		}


		override public void AssignValues (List<ActionParameter> parameters)
		{
			#if ALLOW_VIDEOPLAYER
			videoPlayer = AssignFile <VideoPlayer> (parameters, videoPlayerParameterID, videoPlayerConstantID, videoPlayer);
			#endif

			#if ALLOW_MOVIETEXTURES
			material = (Material) AssignObject <Material> (parameters, materialParameterID, material);
			movieClip = (MovieTexture) AssignObject <MovieTexture> (parameters, movieClipParameterID, movieClip);
			sound = AssignFile (soundID, sound);
			#endif
		}
		

		override public float Run ()
		{
			if (movieClipType == MovieClipType.VideoPlayer)
			{
				#if ALLOW_VIDEOPLAYER
				if (videoPlayer != null)
				{
					if (!isRunning)
					{
						isRunning = true;

						if (movieMaterialMethod == MovieMaterialMethod.PlayMovie)
						{
							if (prepareOnly)
							{
								videoPlayer.Prepare ();

								if (willWait)
								{
									return defaultPauseTime;
								}
							}
							else
							{
								KickStarter.playerInput.skipMovieKey = "";
								videoPlayer.Play ();

								if (videoPlayer.isLooping)
								{
									ACDebug.LogWarning ("Cannot wait for " + videoPlayer.name + " to finish because it is looping!");
									return 0f;
								}

								if (canSkip && skipKey != "")
								{
									KickStarter.playerInput.skipMovieKey = skipKey;
								}

								if (willWait)
								{
									return defaultPauseTime;
								}
							}
						}
						else if (movieMaterialMethod == MovieMaterialMethod.PauseMovie)
						{
							videoPlayer.Pause ();
						}
						else if (movieMaterialMethod == MovieMaterialMethod.StopMovie)
						{
							videoPlayer.Stop ();
						}

						return 0f;
					}
					else
					{
						if (prepareOnly)
						{
							if (!videoPlayer.isPrepared)
							{
								return defaultPauseTime;
							}
						}
						else
						{
							if (canSkip && skipKey != "" && KickStarter.playerInput.skipMovieKey == "")
							{
								videoPlayer.Stop ();
								isRunning = false;
								return 0f;
							}

							if (!videoPlayer.isPrepared || videoPlayer.isPlaying)
							{
								return defaultPauseTime;
							}
						}

						isRunning = false;
						return 0f;
					}
				}
				else
				{
					ACDebug.LogWarning ("Cannot play video - no Video Player found!");
				}
				#else
				ACDebug.LogWarning ("Use of the VideoPlayer for movie playback is only available in Unity 5.6 or later.");
				#endif
				return 0f;
			}

			#if ALLOW_HANDHELD

			if (!isRunning && filePath != "")
			{
				isRunning = true;

				if (canSkip)
				{
					Handheld.PlayFullScreenMovie (filePath, Color.black, FullScreenMovieControlMode.CancelOnInput);
				}
				else
				{
					Handheld.PlayFullScreenMovie (filePath, Color.black, FullScreenMovieControlMode.Hidden);
				}
				return defaultPauseTime;
			}
			else
			{
				isRunning = false;
				return 0f;
			}

			#elif ALLOW_MOVIETEXTURES

			if (movieClip == null)
			{
				ACDebug.LogWarning ("Cannot play movie - no movie clip set!");
				return 0f;
			}
			if (movieClipType == MovieClipType.OnMaterial && material == null)
			{
				ACDebug.LogWarning ("Cannot play movie - no material has been assigned. A movie clip can only be played as a material's texture, so a material must be assigned.");
				return 0f;
			}
			if (includeAudio && sound == null)
			{
				ACDebug.LogWarning ("Cannot play movie audio - no Sound object has been assigned.");
			}

			if (!isRunning)
			{
				isRunning = true;
				guiTexture = null;

				KickStarter.playerInput.skipMovieKey = "";

				if (movieClipType == MovieClipType.FullScreen)
				{
					CreateFullScreenMovie ();
				}
				else if (movieClipType == MovieClipType.OnMaterial)
				{
					if (movieMaterialMethod == MovieMaterialMethod.PlayMovie)
					{
						material.mainTexture = movieClip;
					}
					else if (movieMaterialMethod == MovieMaterialMethod.PauseMovie)
					{
						if (material.mainTexture == movieClip)
						{
							movieClip.Pause ();
							isRunning = false;
							return 0f;
						}
					}
					else if (movieMaterialMethod == MovieMaterialMethod.StopMovie)
					{
						if (material.mainTexture == movieClip)
						{
							movieClip.Stop ();
							isRunning = false;
							return 0f;
						}
					}
				}

				movieClip.Play ();

				if (includeAudio && sound != null)
				{
					if (movieClipType == MovieClipType.OnMaterial && movieMaterialMethod != MovieMaterialMethod.PlayMovie)
					{
						if (movieMaterialMethod == MovieMaterialMethod.PauseMovie)
						{
							sound.GetComponent <AudioSource>().Pause ();
						}
						else if (movieMaterialMethod == MovieMaterialMethod.StopMovie)
						{
							sound.Stop ();
						}
					}
					else
					{
						sound.GetComponent <AudioSource>().clip = movieClip.audioClip;
						sound.Play (false);
					}
				}

				if (movieClipType == MovieClipType.FullScreen || willWait)
				{
					if (canSkip && skipKey != "")
					{
						KickStarter.playerInput.skipMovieKey = skipKey;
					}
					return defaultPauseTime;
				}
				return 0f;
			}
			else
			{
				if (movieClip.isPlaying)
				{
					if (!canSkip || KickStarter.playerInput.skipMovieKey != "")
					{
						return defaultPauseTime;
					}
				}

				OnComplete ();
				isRunning = false;
				return 0f;
			}

			#else

			ACDebug.LogWarning ("On non-mobile platforms, this Action is only available in Unity 5 or Unity Pro.");
			return 0f;

			#endif
		}


		override public void Skip ()
		{
			OnComplete ();
		}


		private void OnComplete ()
		{
			if (movieClipType == MovieClipType.VideoPlayer)
			{
				#if ALLOW_VIDEOPLAYER
				if (videoPlayer != null)
				{
					if (prepareOnly)
					{
						videoPlayer.Prepare ();
					}
					else
					{
						videoPlayer.Stop ();
					}
				}
				#endif
			}
			else if (movieClipType == MovieClipType.FullScreen || (movieClipType == MovieClipType.OnMaterial && movieMaterialMethod == MovieMaterialMethod.PlayMovie))
			{
				if (isRunning)
				{
					#if ALLOW_MOVIETEXTURES
					if (includeAudio)
					{
						sound.Stop ();
					}
					movieClip.Stop ();
					KickStarter.playerInput.skipMovieKey = "";

					if (movieClipType == MovieClipType.FullScreen)
					{
						EndFullScreenMovie ();
					}
					#endif
				}
			}
			else if (movieClipType == MovieClipType.OnMaterial && movieMaterialMethod != MovieMaterialMethod.PlayMovie)
			{
				Run ();
			}
		}

		
		#if UNITY_EDITOR

		override public void ShowGUI (List<ActionParameter> parameters)
		{
			movieClipType = (MovieClipType) EditorGUILayout.EnumPopup ("Play clip:", movieClipType);

			if (movieClipType == MovieClipType.VideoPlayer)
			{
				#if ALLOW_VIDEOPLAYER

				videoPlayerParameterID = Action.ChooseParameterGUI ("Video player:", parameters, videoPlayerParameterID, ParameterType.GameObject);
				if (videoPlayerParameterID >= 0)
				{
					videoPlayerConstantID = 0;
					videoPlayer = null;
				}
				else
				{
					videoPlayer = (VideoPlayer) EditorGUILayout.ObjectField ("Video player:", videoPlayer, typeof (VideoPlayer), true);

					videoPlayerConstantID = FieldToID <VideoPlayer> (videoPlayer, videoPlayerConstantID);
					videoPlayer = IDToField <VideoPlayer> (videoPlayer, videoPlayerConstantID, false);
				}

				movieMaterialMethod = (MovieMaterialMethod) EditorGUILayout.EnumPopup ("Method:", movieMaterialMethod);

				if (movieMaterialMethod == MovieMaterialMethod.PlayMovie)
				{
					prepareOnly = EditorGUILayout.Toggle ("Prepare only?", prepareOnly);
					willWait = EditorGUILayout.Toggle ("Wait until finish?", willWait);

					if (willWait && !prepareOnly)
					{
						canSkip = EditorGUILayout.Toggle ("Player can skip?", canSkip);
						if (canSkip)
						{
							skipKey = EditorGUILayout.TextField ("Skip with Input Button:", skipKey);
						}
					}
				}

				#else

				EditorGUILayout.HelpBox ("This option is only available when using Unity 5.6 or later.", MessageType.Info);

				#endif

				AfterRunningOption ();
				return;
			}

			#if ALLOW_HANDHELD

			if (movieClipType == MovieClipType.OnMaterial)
			{
				EditorGUILayout.HelpBox ("This option is not available on the current platform.", MessageType.Info);
			}
			else
			{
				filePath = EditorGUILayout.TextField ("Path to clip file:", filePath);
				canSkip = EditorGUILayout.Toggle ("Player can skip?", canSkip);

				EditorGUILayout.HelpBox ("The clip must be placed in a folder named 'StreamingAssets'.", MessageType.Info);
			}

			#elif ALLOW_MOVIETEXTURES

			movieClipParameterID = Action.ChooseParameterGUI ("Movie clip:", parameters, movieClipParameterID, ParameterType.UnityObject);
			if (movieClipParameterID < 0)
			{
				movieClip = (MovieTexture) EditorGUILayout.ObjectField ("Movie clip:", movieClip, typeof (MovieTexture), false);
			}

			if (movieClipType == MovieClipType.OnMaterial)
			{
				movieMaterialMethod = (MovieMaterialMethod) EditorGUILayout.EnumPopup ("Method:", movieMaterialMethod);

				string label = "Material to play on:";
				if (movieMaterialMethod == MovieMaterialMethod.PauseMovie)
				{
					label = "Material to pause:";
				}
				else if (movieMaterialMethod == MovieMaterialMethod.StopMovie)
				{
					label = "Material to stop:";
				}

				materialParameterID = Action.ChooseParameterGUI (label, parameters, materialParameterID, ParameterType.UnityObject);
				if (materialParameterID < 0)
				{
					material = (Material) EditorGUILayout.ObjectField (label, material, typeof (Material), true);
				}
			}

			if (movieClipType == MovieClipType.OnMaterial && movieMaterialMethod != MovieMaterialMethod.PlayMovie)
			{ }
			else
			{
				includeAudio = EditorGUILayout.Toggle ("Include audio?", includeAudio);
				if (includeAudio)
				{
					sound = (Sound) EditorGUILayout.ObjectField ("'Sound' to play audio:", sound, typeof (Sound), true);

					soundID = FieldToID (sound, soundID);
					sound = IDToField (sound, soundID, false);
				}

				if (movieClipType == MovieClipType.OnMaterial && movieMaterialMethod == MovieMaterialMethod.PlayMovie)
				{
					willWait = EditorGUILayout.Toggle ("Wait until finish?", willWait);
				}
				if (movieClipType == MovieClipType.FullScreen || willWait)
				{
					canSkip = EditorGUILayout.Toggle ("Player can skip?", canSkip);
					if (canSkip)
					{
						skipKey = EditorGUILayout.TextField ("Skip with Input Button:", skipKey);
					}
				}
			}

			#else

			EditorGUILayout.HelpBox ("On standalone, this Action is only available in Unity 5 or Unity Pro.", MessageType.Warning);

			#endif

			AfterRunningOption ();
		}


		override public void AssignConstantIDs (bool saveScriptsToo)
		{
			#if ALLOW_VIDEOPLAYER
			if (movieClipType == MovieClipType.VideoPlayer && videoPlayer != null)
			{
				if (saveScriptsToo)
				{
					AddSaveScript <RememberVideoPlayer> (videoPlayer);
				}

				AssignConstantID (videoPlayer, videoPlayerConstantID, videoPlayerParameterID);
			}
			#endif
		}
		
		
		public override string SetLabel ()
		{
			if (movieClipType == MovieClipType.VideoPlayer)
			{
				#if ALLOW_VIDEOPLAYER
				string labelAdd = " (" + movieMaterialMethod.ToString ();
				if (videoPlayer != null) labelAdd += " " + videoPlayer.name.ToString ();
				labelAdd += ")";
				return labelAdd;
				#else
				return "";
				#endif
			}

			#if ALLOW_HANDHELD

			if (filePath != "")
			{
				return " (" + filePath + ")";
			}

			#elif ALLOW_MOVIETEXTURES

			if (movieClip)
			{
				return " (" + movieClip.name + ")";
			}

			#endif
			return "";
		}
		
		#endif


		#if ALLOW_MOVIETEXTURES

		private void CreateFullScreenMovie ()
		{
			GameObject movieOb = new GameObject ("Movie clip");
			movieOb.transform.position = Vector3.zero;
			movieOb.transform.position = new Vector2 (0.5f, 0.5f);

			guiTexture = movieOb.AddComponent<GUITexture>();
			guiTexture.enabled = false;
			guiTexture.texture = movieClip;
			guiTexture.enabled = true;

			KickStarter.sceneSettings.SetFullScreenMovie (movieClip);
		}


		private void EndFullScreenMovie ()
		{
			KickStarter.sceneSettings.StopFullScreenMovie ();
			if (guiTexture != null)
			{
				guiTexture.enabled = false;
				Destroy (guiTexture.gameObject);
			}
		}

		#endif

	}
	
}