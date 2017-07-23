using UnityEngine;
using System.Collections;
using SmartMath;

public class sphereArea : MonoBehaviour {



public  float SphereArea(float R)
	{
		return 4*Mathf.Pow(R,2)*Mathf.PI;
	}
}
