using UnityEngine;
using System.Collections;

// Get SBLS
using SBLS;

[RequireComponent(typeof(CharacterController))]
public class FPSController : MonoBehaviour
{
	// Create a SBLS character instance;
	private SBLSCharacter character;
	public float speed = 6.0f;
	public float jumpSpeed = 8.0f;
	public float gravity = 20.0f;
	public float runSpeed = 12.0f;
	private float actualSpeed = 0.0f;
	private int noJumps = 0;
	CharacterController controller;
	CollisionFlags flags;
	bool isJumping = false;
	float actualJump = 0.0f;

	Vector3 moveDirection = Vector3.zero;
	bool grounded = false;
	
	void Awake()
	{
		controller = GetComponent<CharacterController>();
	}

	void Start() {
		// Get the SBLS component
		character = GetComponent<SBLSCharacter> ();
	}
	
	void FixedUpdate()
	{
		
		if (grounded)
		{
			if (Input.GetKey(KeyCode.LeftShift)) {
				// Let's run!
				// We'll also add the current running level so we get faster as we level up
				actualSpeed = runSpeed + character.getSkill("Running").getLevel();

				// Tell the running skill that we are running, but only count if we are moving
				if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0) {
					character.getSkill("Running").inUse = true;
				}
			} else {
				actualSpeed = speed;

				// We're walking, better let the skill know
				character.getSkill("Running").inUse = false;
			}
			// We are grounded, so recalculate movedirection directly from axes
			moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
			moveDirection = transform.TransformDirection(moveDirection);
			moveDirection *= actualSpeed;
			
			if (Input.GetButton("Jump"))
			{
				isJumping = true;
				// Unrealistic? Sure, but it's a demo
				actualJump = jumpSpeed * character.getSkill("Jumping").getLevel();
				moveDirection.y = actualJump;
			}


		}
		
		// Apply gravity
		moveDirection.y -= gravity * Time.deltaTime;
		
		// Move the controller
		flags = controller.Move(moveDirection * Time.deltaTime);

		if (flags == CollisionFlags.CollidedBelow) {
			if (!grounded && isJumping) {
				noJumps++;
				if (noJumps % 2 == 0) {
					character.getSkill("Jumping").adjustXp(1);
				}
			}
			isJumping = false;
			grounded = true;
		} else {
			grounded = false;
		}
	}
	
}
