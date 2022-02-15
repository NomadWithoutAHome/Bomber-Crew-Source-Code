using System.Collections;
using BomberCrewCommon;
using UnityEngine;

public class HazardFlak : MonoBehaviour
{
	[SerializeField]
	private ParticleSystem m_particles;

	[SerializeField]
	private ExternalDamageExplicit m_damager;

	[SerializeField]
	private float m_lightTime = 0.05f;

	[SerializeField]
	private float m_lifeTime = 1.5f;

	[SerializeField]
	private Color m_lightColor;

	[SerializeField]
	private float m_lightIntensity;

	private bool m_hasExploded;

	private void Awake()
	{
		m_particles.Stop(withChildren: true);
	}

	private void OnEnable()
	{
		m_hasExploded = false;
	}

	private void Update()
	{
		if (!m_hasExploded)
		{
			Explode();
			m_hasExploded = true;
		}
	}

	public void Explode()
	{
		m_particles.Play(withChildren: true);
		if (Singleton<BomberEffectLight>.Instance != null)
		{
			Singleton<BomberEffectLight>.Instance.AddLighting(m_lightIntensity, m_lightColor, base.transform);
		}
		StartCoroutine(ActivateCollider());
	}

	private IEnumerator ActivateCollider()
	{
		yield return null;
		m_damager.DoDamage();
		yield return new WaitForSeconds(m_lightTime);
		yield return new WaitForSeconds(m_lifeTime - m_lightTime);
		Singleton<PoolManager>.Instance.ReturnToPool(base.gameObject);
	}
}
