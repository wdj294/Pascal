using UnityEngine;
using System.Collections;
using ParticlePlayground;

[ExecuteInEditMode()]
public class EmitParticlesOnTrailPointEvents : MonoBehaviour {

	/// <summary>
	/// The particle system to emit from.
	/// </summary>
	public PlaygroundParticlesC particles;
	/// <summary>
	/// The trails you wish to emit particles onto.
	/// </summary>
	public PlaygroundTrails trails;
	/// <summary>
	/// Particle emit velocity.
	/// </summary>
	public Vector3 velocity;
	/// <summary>
	/// Particle emit color.
	/// </summary>
	public Color32 color = Color.white;

	void OnEnable ()
	{
		if (trails != null)
			trails.trailPointEvent += OnTrailPoint;
	}

	void OnDisable ()
	{
		if (trails != null)
			trails.trailPointEvent -= OnTrailPoint;
	}

	void OnTrailPoint (PlaygroundEventTrailPoint trailPoint)
	{
		Vector3 position = trailPoint.position;

		particles.Emit (position, velocity, color);
	}
}
