// Instantiates a projectile every 0.5 seconds,
// if the Fire1 button (default is Ctrl) is pressed.

using UnityEngine;
using System.Collections;

public class Temperature : MonoBehaviour
{
	public GameObject projectile;
	public float fireDelta = 0.5F;

	private float nextFire = 0.5F;
	private GameObject newProjectile;
	private float myTime = 0.0F;

	void Update()
	{
		myTime = myTime + Time.deltaTime;

		if (Input.GetButton("Fire1") && myTime > nextFire)
		{
			nextFire = myTime + fireDelta;
			newProjectile = Instantiate(projectile, transform.position, transform.rotation) as GameObject;

			// create code here that animates the newProjectile

			nextFire = nextFire - myTime;
			myTime = 0.0F;
		}
	}
}