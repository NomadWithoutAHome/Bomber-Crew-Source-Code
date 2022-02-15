using System;
using UnityEngine;

public abstract class Damageable : MonoBehaviour
{
	private int m_objectId;

	public Action<DamageSource, float> OnDamage;

	public int GetObjectIdCached()
	{
		if (m_objectId == 0)
		{
			m_objectId = base.gameObject.GetInstanceID();
		}
		return m_objectId;
	}

	public abstract bool IsDamageBlocker();

	public abstract float DamageGetPassthrough(float amt, DamageSource damageSource);

	public virtual bool DirectDamageOnly()
	{
		return false;
	}

	public virtual Vector3 GetDamagePosition(Vector3 nearPos)
	{
		return base.transform.position;
	}
}
