using UnityEngine;
using System.Collections;
using ParticlePlayground;

[ExecuteInEditMode()]
public class ParticleSpiralBetweenTwoPoints : MonoBehaviour {

	public PlaygroundParticlesC particles;
	public Transform start;
	public Transform end;
	[Range(1, 100)]
	public float swirlIterations = 10.0f;
	public float radius = 1f;
	public AnimationCurve radiusTimeScale = new AnimationCurve(new Keyframe[]{new Keyframe(0,1f), new Keyframe(1f,1f)});
	public Color32 color = Color.white;

	private bool _ready = false;
	private Vector3[] _spiralInterpolations;
	private Vector3 _startPos;
	private Vector3 _endPos;
	private Vector3 _lookDirection;
	private float _r;
	private float _i;

	void OnEnable () 
	{
		if (particles == null || start == null || end == null)
			return;

		particles.source = SOURCEC.Script;
		particles.onlySourcePositioning = true;
		GenerateSpiralInterpolations();
		_ready = true;
	}
	
	void Update () 
	{
		if (!_ready || !particles.IsReady() || particles.particleCount < 2)
			return;

		// Generate new interpolation positions if needed
		if (_spiralInterpolations.Length != particles.particleCount ||
		    _startPos != start.position ||
		    _endPos != end.position ||
		    !Mathf.Approximately(_i, swirlIterations) ||
		    !Mathf.Approximately(_r, radius))
			GenerateSpiralInterpolations();

		for (int i = 0; i<particles.particleCount; i++)
		{
			// If particles cache is resized in the middle of execution we need to exit the loop
			if (_spiralInterpolations.Length != particles.playgroundCache.simulate.Length)
				break;

			// Emit or position particles to the calculated spiral position
			if (!particles.playgroundCache.simulate[i])
				particles.Emit (_spiralInterpolations[i]);
			else
				particles.ParticlePosition(i, _spiralInterpolations[i]);
		}
	}

	void GenerateSpiralInterpolations ()
	{
		_startPos = start.position;
		_endPos = end.position;
		_lookDirection = (_endPos - _startPos).normalized;
		_r = radius;
		_i = swirlIterations;
		_spiralInterpolations = new Vector3[particles.particleCount];
		float totalDistance = Vector3.Distance (_startPos, _endPos);
		float pi = Mathf.PI;

		for (int i = 0; i<_spiralInterpolations.Length; i++)
		{
			// Create the 'flat' (non-rotated) spiral position, the distance to the end point will determine the z depth
			float t = (i*1f) / particles.particleCount;
			float radiusT = radiusTimeScale.Evaluate(t);
			Vector3 spiralPosFlat = new Vector3(_r * radiusT * Mathf.Sin (_i * t * pi * 2), _r * radiusT * Mathf.Cos (_i * t * pi * 2), t*totalDistance);

			// Rotate the spiral position as an offset and take the start position offset into account
			Vector3 finalSpiralPos = _startPos + (Quaternion.LookRotation(_lookDirection) * spiralPosFlat);

			// Cache the summary (for performance in the Update loop)
			_spiralInterpolations[i] = finalSpiralPos;
		}
	}
}
