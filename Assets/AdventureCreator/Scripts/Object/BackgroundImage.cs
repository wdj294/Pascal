/*
 *
 *	Adventure Creator
 *	by Chris Burton, 2013-2017
 *	
 *	"BackgroundImage.cs"
 * 
 *	The BackgroundImage prefab is used to store a GUITexture for use in background images for 2.5D games.
 * 
 */

#if UNITY_STANDALONE && (UNITY_5 || UNITY_2017_1_OR_NEWER || UNITY_PRO_LICENSE)
#define ALLOW_MOVIETEXTURES
#endif

using UnityEngine;
using System.Collections;

namespace AC
{

	/**
	 * Controls a GUITexture for use in background images in 2.5D games.
	 */
	[RequireComponent (typeof (GUITexture))]
	#if !(UNITY_4_6 || UNITY_4_7 || UNITY_5_0)
	[HelpURL("http://www.adventurecreator.org/scripting-guide/class_a_c_1_1_background_image.html")]
	#endif
	public class BackgroundImage : MonoBehaviour
	{

		#if ALLOW_MOVIETEXTURES
		/** If True, then any MovieTexture set as the background will be looped */
		public bool loopMovie = true;
		/** If True, then any MovieTexture set as the background will start from the beginning when the associated Camera is activated */
		public bool restartMovieWhenTurnOn = false;
		#endif

		private float shakeDuration;
		private float startTime;
		private float startShakeIntensity;
		private float shakeIntensity;
		private Rect originalPixelInset;


		/**
		 * <summary>Sets the background image to a supplied texture</summary>
		 * <param name = "_texture">The texture to set the background image to</param>
		 */
		public void SetImage (Texture2D _texture)
		{
			GetComponent <GUITexture>().texture = _texture;
		}


		/**
		 * Displays the background image (within the GUITexture) fullscreen.
		 */
		public void TurnOn ()
		{
			if (LayerMask.NameToLayer (KickStarter.settingsManager.backgroundImageLayer) == -1)
			{
				ACDebug.LogWarning ("No '" + KickStarter.settingsManager.backgroundImageLayer + "' layer exists - please define one in the Tags Manager.");
			}
			else
			{
				gameObject.layer = LayerMask.NameToLayer (KickStarter.settingsManager.backgroundImageLayer);
			}
			
			if (GetComponent <GUITexture>())
			{
				GetComponent <GUITexture>().enabled = true;
			}
			else
			{
				ACDebug.LogWarning (this.name + " has no GUITexture component");
			}

			#if ALLOW_MOVIETEXTURES			
			if (GetComponent <GUITexture>().texture != null && GetComponent <GUITexture>().texture is MovieTexture)
			{
				MovieTexture movieTexture = (MovieTexture) GetComponent <GUITexture>().texture;
				if (restartMovieWhenTurnOn)
				{
					movieTexture.Stop ();
				}
				movieTexture.loop = loopMovie;
				movieTexture.Play ();
			}
			#endif
		}
		

		/**
		 * Hides the background image (within the GUITexture) from view.
		 */
		public void TurnOff ()
		{
			gameObject.layer = LayerMask.NameToLayer (KickStarter.settingsManager.deactivatedLayer);
			
			if (GetComponent <GUITexture>())
			{
				GetComponent <GUITexture>().enabled = false;
			}
			else
			{
				ACDebug.LogWarning (this.name + " has no GUITexture component");
			}
		}
		

		/**
		 * <summary>Shakes the background image (within the GUITexture) for an earthquake-like effect.</summary>
		 * <param name = "_shakeIntensity">How intense the shake effect should be</param>
		 * <param name = "_duration">How long the shake effect should last, in seconds</param>
		 */
		public void Shake (float _shakeIntensity, float _duration)
		{
			if (shakeIntensity > 0f)
			{
				this.GetComponent <GUITexture>().pixelInset = originalPixelInset;
			}
			
			originalPixelInset = this.GetComponent <GUITexture>().pixelInset;

			shakeDuration = _duration;
			startTime = Time.time;
			shakeIntensity = _shakeIntensity;

			startShakeIntensity = shakeIntensity;

			if (this.GetComponent <GUITexture>())
			{
				StopCoroutine (UpdateShake ());
				StartCoroutine (UpdateShake ());
			}
		}
		

		private IEnumerator UpdateShake ()
		{
			while (shakeIntensity > 0f)
			{
				float _size = Random.Range (0, shakeIntensity) * 0.2f;
				
				this.GetComponent <GUITexture>().pixelInset = new Rect
				(
					originalPixelInset.x - Random.Range (0, shakeIntensity) * 0.1f,
					originalPixelInset.y - Random.Range (0, shakeIntensity) * 0.1f,
					originalPixelInset.width + _size,
					originalPixelInset.height + _size
				);

				shakeIntensity = Mathf.Lerp (startShakeIntensity, 0f, AdvGame.Interpolate (startTime, shakeDuration, MoveMethod.Linear, null));

				yield return new WaitForEndOfFrame ();
			}
			
			shakeIntensity = 0f;
			this.GetComponent <GUITexture>().pixelInset = originalPixelInset;
		}

		
	}

}