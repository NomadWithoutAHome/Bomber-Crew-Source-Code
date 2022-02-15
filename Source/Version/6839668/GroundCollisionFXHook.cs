using BomberCrewCommon;
using UnityEngine;

public class GroundCollisionFXHook : MonoBehaviour
{
	[SerializeField]
	private float m_velocityMagnitudeThreshold = 4f;

	[SerializeField]
	private LayerMask m_environmentLayer;

	private void OnCollisionEnter(Collision c)
	{
		if (((1 << c.collider.gameObject.layer) & (int)m_environmentLayer) == 0 || !(c.relativeVelocity.magnitude > m_velocityMagnitudeThreshold) || !(Singleton<GlobalEffects>.Instance != null))
		{
			return;
		}
		ContactPoint[] contacts = c.contacts;
		foreach (ContactPoint contactPoint in contacts)
		{
			GroundCollisionType.GroundCollisionEffectType effectType = GroundCollisionType.GroundCollisionEffectType.Default;
			GroundCollisionType component = c.collider.GetComponent<GroundCollisionType>();
			if (component != null)
			{
				effectType = component.GetEffectType();
			}
			Singleton<GlobalEffects>.Instance.SpawnImpactEffects(effectType, contactPoint.point);
		}
	}
}
