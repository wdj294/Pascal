using UnityEngine;
using System.Collections;

namespace TutorialDesigner {
		
	/// <summary>
	/// Game Logic and Player Interaction
	/// </summary>
	public class Game : MonoBehaviour {
	    // Soccer ball, Red aiming arrow, Goal, Button for getting in the next round
		public GameObject ball, arrow, goal, skipButton, TDButton;
	    public RectTransform strengthMask; // Mask for shoot strength
		public Goalkeeper goalKeeper; // Keeper
		public Transform ballSpawnArea; // Area where the ball spawns randomly
		public Ball ballScript; // Reference to the ball's functions

		public static AudioClip[] audioClips; // Array for Soundeffects
        public static Game self; // Self Reference

		private float strength; // Shoot strength
		private Transform canvas; // 3d Canvas for displaying strenghMask
	    // Some states for the Gameplay
	    private bool playersTurn, showSkipButton = true, playerScored, playerMissed, playerMayShoot = true;
	    private Rigidbody ballRigid; // Reference to the ball's Rigidbody Component

		// Use this for initialization
		void Start () {
            self = this;

	        // Decrease time to slow down physics
			Time.timeScale = 0.5f;
			canvas = strengthMask.transform.parent;

			ballRigid = ball.GetComponent<Rigidbody>();
			ballScript = ball.GetComponent<Ball>();

			audioClips = (AudioClip[])Resources.LoadAll<AudioClip>("Sounds");
			ballScript.enabled = true;

            // Check if this is a One-Time Tutorial which was already done
            GameObject TutorialSystem = GameObject.Find("TutorialSystem");
            if (TutorialSystem != null) {
                SavePoint TDSavePoint = TutorialSystem.GetComponent<SavePoint>();
                if (TDSavePoint.oneTimeTutorial && TDSavePoint.IsTutorialDone()) {
                    TDButton.SetActive(true);
                }
            }

			// Start shooting routine
			StartPrepareShot();
		}

		// Update is called once per frame
		void Update () {
			if (playersTurn) {
				// If Player moves the mouse over the ball, make red arrow appear and point to the ball
				RaycastHit hit;
				if (Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hit)) {
                    if (hit.collider.tag == "Ball" && Time.timeScale > 0) {
	                    // Mouse over the ball
						arrow.SetActive (true);
						arrow.transform.position = hit.point;
						arrow.transform.LookAt (ball.transform.position);
						Cursor.visible = false;
	                    // MouseHoverBall Event
                        EventManager.TriggerEvent("MouseHoverBall");
	                } else {
	                    // Mouse not over the ball
						arrow.SetActive (false);
						Cursor.visible = true;
						ResetStrengthMask();
					}
				}

				// Mouseclick behaviours
	            if (arrow.activeSelf && playerMayShoot) {
					if (Input.GetMouseButtonDown (0)) {						
						// Mouse Down -> Make shot-strength sprite appear
						canvas.position = ball.transform.position + Vector3.up * 0.1f;
						canvas.LookAt (Camera.main.transform.position);
						canvas.gameObject.SetActive (true);
						strength = 0f;
						ballScript.PlaySound(2);
	                } else if (Input.GetMouseButton (0) && strength < 1) {
						// As long as mouse button is down, let the strength-meter increase
						strength += Time.deltaTime * 1.5f;
						strengthMask.SetSizeWithCurrentAnchors (RectTransform.Axis.Vertical, strength * 128);
					} else if (Input.GetMouseButtonUp (0) || strength >= 1) {
						// Mouse Up -> Shoot with loaded strength
						if (arrow.activeSelf) {
							ballRigid.AddForceAtPosition (arrow.transform.forward * 5f * strength, arrow.transform.position, ForceMode.Impulse);
							ResetStrengthMask();
							ballScript.StopSound();
							ballScript.PlaySound(1);
							StartCoroutine (AfterShot ());
							playersTurn = false;
							arrow.SetActive (false);
	                        Cursor.visible = true;
	                        // PlayerShoots Event
                            EventManager.TriggerEvent("PlayerShoots");
	                    }
					}
				}
			}
		}

	    // Hide and reset shoot strength 
		private void ResetStrengthMask() {
			strength = 0f;
			canvas.gameObject.SetActive (false);
			strengthMask.SetSizeWithCurrentAnchors (RectTransform.Axis.Vertical, 0f);
		}

		// Short Animation that prepares the Ball and animates the Camera
		private IEnumerator PrepareShot() {
			playersTurn = false;
            playerScored = false;
            playerMissed = false;
			skipButton.SetActive (false);

			// Position the ball random on ballSpawnArea
			Vector3 areaPos = ballSpawnArea.localPosition;
			Vector3 areaSize = ballSpawnArea.localScale / 2f;
			Vector3 spawnpos = new Vector3 (			                 
				Random.Range (areaPos.x - areaSize.x, areaPos.x + areaSize.x),
				0f,
				Random.Range (areaPos.z - areaSize.y, areaPos.z + areaSize.y));
			ball.transform.localPosition = spawnpos;
			ballRigid.velocity = Vector3.zero;
			ballRigid.angularVelocity = Vector3.zero;

			goalKeeper.ResetGoalKeeper();

			// Let the Camera look at the ball and goal
			Transform cam = Camera.main.transform;
			cam.position = ball.transform.position + Vector3.up * 0.3f; // Cam to ball
			cam.LookAt (goal.transform.position); // Look at goal
			cam.position -= cam.forward; // Move back a little

			// Start little translation to move Camera forward
			Vector3 startPos = cam.position;
			Vector3 endPos = startPos + cam.forward * 0.4f;

			float t = 0f;
			while (t < 0.5f) {
				t += Time.deltaTime;
				cam.position = Vector3.Lerp (startPos, endPos, Mathf.SmoothStep(0f, 1f, t / 0.5f));
				yield return null;
			}

			// Let the player shoot
			playersTurn = true;
			yield return null;
		}

		// Routine that starts a new shot at a specific time
		private IEnumerator AfterShot() {
			yield return new WaitForSeconds (1.2f);
            if (!PlayerShotOver()) {
                if (!playerScored) EventManager.TriggerEvent("PlayerMisses");    			
            }
            if (showSkipButton) skipButton.SetActive (true);
		}

		// Public funtion that just calls the coroutine. because also GUI buttons have to call them
		public void StartPrepareShot() {
	        StopAllCoroutines();
			StartCoroutine(PrepareShot());
            EventManager.TriggerEvent("NewShot");
		}
	       
	    // Function for calling by UnityEvent
		public void PlayerScored(bool value) {
			playerScored = value;
		}

        // Function for calling by UnityEvent
        public void PlayerMissed(bool value) {
            playerMissed = value;
        }

        // Check if Player's Shot turn is over
        public static bool PlayerShotOver() {
            return self.playerScored || self.playerMissed;
        }

	    // Function for calling by UnityEvent
		public void ShowSkipButton(bool value) {
			showSkipButton = value;
	        Cursor.visible = true;
		}

	    // Function for calling by UnityEvent
	    public void PlayerMayShoot(bool value) {
	        playerMayShoot = value;
	    }
	}

}
