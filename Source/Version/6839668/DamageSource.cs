using UnityEngine;

public class DamageSource
{
	public enum DamageShape
	{
		Line,
		Sphere,
		None
	}

	public enum DamageType
	{
		Impact,
		Fire,
		GroundImpact,
		SelfFire
	}

	public DamageShape m_damageShapeEffect;

	public DamageType m_damageType;

	public Vector3 m_position;

	public object m_relatedObject;

	public RaycastHit m_raycastInfo;

	public Vector3 m_trajectory;

	public float m_radius;

	public bool m_incendiaryEffect;

	public ProjectileType m_fromProjectile;
}
