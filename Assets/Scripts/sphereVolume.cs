using UnityEngine;
using System.Collections;
using SmartMath;

public class sphereVolume : MonoBehaviour {


	
public float SphereVolume(float R)
		{
			return (4 * Mathf.Pow(R, 3) * Mathf.PI) / 3;
		}

}
