using UnityEngine;

namespace PigeonCoopToolkit.Effects.Trails;

[AddComponentMenu("Pigeon Coop Toolkit/Effects/Smoke Plume")]
public class SmokePlume : TrailRenderer_Base
{
	public float TimeBetweenPoints = 0.1f;

	public Vector3 ConstantForce = Vector3.up * 0.5f;

	public float RandomForceScale = 0.05f;

	public int MaxNumberOfPoints = 50;

	private float _timeSincePoint;

	protected override void Start()
	{
		base.Start();
		_timeSincePoint = 0f;
	}

	protected override void OnStartEmit()
	{
		_timeSincePoint = 0f;
	}

	protected override void Reset()
	{
		base.Reset();
		TrailData.SizeOverLife = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.5f, 0.2f), new Keyframe(1f, 0.2f));
		TrailData.Lifetime = 6f;
		ConstantForce = Vector3.up * 0.5f;
		TimeBetweenPoints = 0.1f;
		RandomForceScale = 0.05f;
		MaxNumberOfPoints = 50;
	}

	protected override void Update()
	{
		if (_emit)
		{
			_timeSincePoint += ((!_noDecay) ? Time.deltaTime : 0f);
			if (_timeSincePoint >= TimeBetweenPoints)
			{
				AddPoint(new SmokeTrailPoint(), _t.position);
				_timeSincePoint = 0f;
			}
		}
		base.Update();
	}

	protected override void InitialiseNewPoint(PCTrailPoint newPoint)
	{
		((SmokeTrailPoint)newPoint).RandomVec = Random.onUnitSphere * RandomForceScale;
	}

	protected override void UpdateTrail(PCTrail trail, float deltaTime)
	{
		if (_noDecay)
		{
			return;
		}
		foreach (PCTrailPoint point in trail.Points)
		{
			point.Position += ConstantForce * deltaTime;
		}
	}

	protected override int GetMaxNumberOfPoints()
	{
		return MaxNumberOfPoints;
	}
}
