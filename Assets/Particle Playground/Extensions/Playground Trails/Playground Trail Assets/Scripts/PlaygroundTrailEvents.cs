using UnityEngine;
using System.Collections;

namespace ParticlePlayground {

	public class PlaygroundEventTrailPoint {

		/// <summary>
		/// The identifier of which trail this trail point event belongs to.
		/// </summary>
		[HideInInspector] public int trailId;
		/// <summary>
		/// The identifier of which trail point index this trail point event belongs to.
		/// </summary>
		[HideInInspector] public int pointId;
		/// <summary>
		/// The position of the trail point.
		/// </summary>
		[HideInInspector] public Vector3 position;
		/// <summary>
		/// The width of the trail point.
		/// </summary>
		[HideInInspector] public float width;
		/// <summary>
		/// The lifetime of the trail point.
		/// </summary>
		[HideInInspector] public float lifetime;
		/// <summary>
		/// The time the trail point was created.
		/// </summary>
		[HideInInspector] public float timeCreated;


		public void Update (int trailId, int pointId, Vector3 position, float width, float lifetime, float timeCreated)
		{
			this.trailId = trailId;
			this.pointId = pointId;
			this.position = position;
			this.width = width;
			this.lifetime = lifetime;
			this.timeCreated = timeCreated;
		}

		/// <summary>
		/// Returns a copy of this PlaygroundEventTrailPoint instance.
		/// </summary>
		public PlaygroundEventTrailPoint Clone ()
		{
			PlaygroundEventTrailPoint trailPointEventClone = new PlaygroundEventTrailPoint();
			trailPointEventClone.Update(
				trailId,
				pointId,
				position,
				width,
				lifetime,
				timeCreated
			);
			return trailPointEventClone;
		}
	}

	/// <summary>
	/// Event delegate for sending a PlaygroundEventTrailPoint to any event listeners.
	/// </summary>
	public delegate void OnPlaygroundTrailPoint(PlaygroundEventTrailPoint trailPoint);
}