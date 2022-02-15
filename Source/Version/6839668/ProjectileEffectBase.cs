using UnityEngine;

public abstract class ProjectileEffectBase : MonoBehaviour
{
	public abstract void TriggerEffect(Vector3 startPos, Vector3 endPos, float muzzleVelocity, float damage, LayerMask targetLayermask, Vector3 inheritedVelocity, object relatedObject, ProjectileType projectileType);

	public virtual void SetFake(bool fake)
	{
	}
}
