using UnityEngine;
using System.Collections;

namespace TutorialDesigner {
    
	/// <summary>
	/// Ball interactions
	/// </summary>
	public class Ball : MonoBehaviour {

		private AudioSource audioSource;

		// Use this for initialization
		void Start () {
			audioSource = GetComponent<AudioSource>();
		}

		// Update is called once per frame
		void Update () {
		
		}

	    // Any collision with other colliders
		void OnCollisionEnter(Collision col) {
			// Play Sounds at Collision with different Objects
			if (col.collider.tag == "Lawn") {						
				PlaySound(0);
			}
		}

	    // Other collider with "Trigger" enabled, enters this collider
		void OnTriggerEnter(Collider other) {
	        // Scorezone is the trigger. It is a defined area where the ball must land to trigger a goal
            if (other.tag == "Scorezone" && !Game.PlayerShotOver()) {
	            // Ball scores
				EventManager.TriggerEvent("PlayerScores");
			}
		}

	    // Play an audioclip
		public void PlaySound(byte no) {		
			if (audioSource != null) audioSource.PlayOneShot(Game.audioClips[no]);			
		}

	    // Stop all sounds
		public void StopSound() {
			audioSource.Stop();
		}
	}

}