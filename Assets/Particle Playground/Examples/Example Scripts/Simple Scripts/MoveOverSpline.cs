using UnityEngine;
using System.Collections;
using PlaygroundSplines;

[ExecuteInEditMode()]
public class MoveOverSpline : MonoBehaviour {

	public PlaygroundSpline spline;
	[Range(0.0f, 1.0f)]
	public float progression = 0;
	public bool loop = false;

	private float _previousProgression = -1;

	void Update () 
	{
		if (!Mathf.Approximately (progression, _previousProgression))
		{
			transform.position = spline.GetPoint (loop? progression : Mathf.Min (progression, .999f));
			_previousProgression = progression;
		}
	}
}
