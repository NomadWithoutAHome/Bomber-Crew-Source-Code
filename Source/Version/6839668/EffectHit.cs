using System.Collections;
using BomberCrewCommon;
using UnityEngine;

public class EffectHit : MonoBehaviour
{
	[SerializeField]
	private ParticleSystem m_rootParticleSystem;

	[SerializeField]
	private float m_effectDuration;

	private float m_effectTime;

	public void Reset()
	{
		StopAllCoroutines();
		m_rootParticleSystem.Stop(withChildren: true);
	}

	public void TriggerEffect(Vector3 hitPoint, Quaternion hitRotation, Transform trackingTransform)
	{
		Reset();
		base.transform.parent = trackingTransform;
		base.transform.position = hitPoint;
		base.transform.rotation = hitRotation;
		StartCoroutine(Effect());
	}

	private IEnumerator Effect()
	{
		m_effectTime = 0f;
		m_rootParticleSystem.Play(withChildren: true);
		while (m_effectTime <= m_effectDuration)
		{
			m_effectTime += Time.deltaTime;
			yield return null;
		}
		Singleton<PoolManager>.Instance.ReturnToPool(base.gameObject);
	}
}
