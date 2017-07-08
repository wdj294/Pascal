using UnityEngine;
using System.Collections;
using ParticlePlayground;
using PlaygroundSplines;

public class TargetSplineWithScattering : MonoBehaviour {

	public PlaygroundParticlesC particles;
	public PlaygroundSpline spline;

	[Range(0.01f, 10f)]
	public float emitTime = .1f;
	public Color32 color = Color.white;
	public bool applyPositionScatter = true;
	public MINMAXVECTOR3METHOD positionScatterMethod;
	public Vector3 positionScatterMin = new Vector3(0,-1);
	public Vector3 positionScatterMax = new Vector3(0,1);
	public bool applyScatterLifetimeMultiplier = true;
	public AnimationCurve positionScatterLifetimeMultiplier = new AnimationCurve(new Keyframe[]{new Keyframe(0,1f), new Keyframe(1f,1f)});

	private float _emitTimer;
	private Vector3[] _scatterPosition;

	void Awake () 
	{
		particles.scriptedEmissionIndex = 0;
		particles.gravity = Vector3.zero;
		particles.applyInitialVelocity = false;

		// Generate scatter for the available particles
		GenerateScatter();
	}
	
	void Update () 
	{
		// Ensure particle system is ready before we use it
		if (!particles.IsReady())
			return;

		// Emit
		_emitTimer += Time.deltaTime / emitTime;
		if (_emitTimer >= 1f)
		{
			// Emit onto the initial position of the spline
			Vector3 emitPos = spline.GetPoint(0);
			particles.Emit (emitPos, Vector3.zero, color);
			_emitTimer = 0;
		}

		// Ensure correct length of scatter positions (upon changing particle count)
		if (applyPositionScatter && particles.particleCount != _scatterPosition.Length)
			GenerateScatter();

		// Position
		for (int i = 0; i<particles.particleCount; i++)
		{
			// If Playground Cache is resized while in here we need to exit the loop
			if (applyPositionScatter && particles.playgroundCache.simulate.Length != _scatterPosition.Length)
				break;

			// Check if the particle is simulating
			if (particles.playgroundCache.simulate[i])
			{
				// Normalize the time for the particle
				float t = Mathf.Min (particles.playgroundCache.life[i] / (particles.lifetime - particles.playgroundCache.lifetimeSubtraction[i]), .99f);

				// Create a position on the spline along with the generated scattering
				Vector3 pos = spline.GetPoint (t);
				if (applyPositionScatter)
				{
					pos += applyScatterLifetimeMultiplier? _scatterPosition[i] * positionScatterLifetimeMultiplier.Evaluate(t) : _scatterPosition[i];
				}

				// Set the position of the particle
				particles.ParticlePosition(i, pos);
			}
		}
	}

	void GenerateScatter ()
	{
		if (!applyPositionScatter)
			return;

		_scatterPosition = new Vector3[particles.particleCount];
		System.Random random = new System.Random();
		for (int p = 0; p<_scatterPosition.Length; p++) 
		{
			if (positionScatterMethod==MINMAXVECTOR3METHOD.Rectangular)
				_scatterPosition[p] = PlaygroundParticlesC.RandomRange(random, positionScatterMin, positionScatterMax);
			else if (positionScatterMethod==MINMAXVECTOR3METHOD.RectangularLinear)
				_scatterPosition[p] = Vector3.Lerp (positionScatterMin, positionScatterMax, (p*1f)/(_scatterPosition.Length*1f));
			else if (positionScatterMethod==MINMAXVECTOR3METHOD.Spherical)
				_scatterPosition[p] = PlaygroundParticlesC.RandomRangeSpherical(random, positionScatterMin.x, positionScatterMax.x);
			else if (positionScatterMethod==MINMAXVECTOR3METHOD.SphericalLinear)
				_scatterPosition[p] = PlaygroundParticlesC.RandomRangeSpherical(random, positionScatterMin.x, positionScatterMax.x, (p*1f)/(_scatterPosition.Length*1f));
		}
	}
}
