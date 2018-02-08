using UnityEngine;
using System.Collections;

namespace TutorialDesigner {

	/// <summary>
	/// Goalkeeper behavior and movement
	/// </summary>
	public class Goalkeeper : MonoBehaviour {

		public Rigidbody ball; // Reference to the soccer ball
		private float gkMovement = 2f; // Current moving direction of the keeper
		private bool ballout = false; // If the ball passed the goal

		// Use this for initialization
		void Start () {
			
		}
		
		// Update is called once per frame
		void FixedUpdate () {
	        // If the ball is out, don't do anything
			if (ballout) return;

			if (ball.velocity != Vector3.zero) {
	            // If ball moves, let goalkeeper go after it
				Vector3 ballDirection = ball.position + ball.velocity * 0.1f;			
				gkMovement = (ballDirection.x - transform.position.x) * 5f;
			} else {
				// Standard movement when ball doesn't move
				if (transform.position.x > 0.4f) {
					gkMovement = -1f;
				} else if (transform.position.x < -0.4f) {
					gkMovement = 1f;
				}
			}

	        // Alter keeper position by gkMovement
			transform.position += transform.right * gkMovement * Time.deltaTime;

	        // Check ball position and eventually set ballout
			if (ball.transform.position.z > transform.position.z) { 
				ballout = true;
			}
		}

	    // Set Keeper to initial position
		public void ResetGoalKeeper() {
			Vector3 pos = transform.localPosition;
			pos.x = 0f;
			transform.localPosition = pos;
			ballout = false;
			gkMovement = 1f;
		}
	}
}
