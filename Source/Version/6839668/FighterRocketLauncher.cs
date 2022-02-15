using System.Collections;
using BomberCrewCommon;
using UnityEngine;
using WingroveAudio;

public class FighterRocketLauncher : MonoBehaviour
{
	[SerializeField]
	private ProjectileType m_projectileToFire;

	[SerializeField]
	private float m_fireCountdown;

	[SerializeField]
	private Transform m_effectScalerNode;

	[SerializeField]
	private int m_ammoMax;

	[SerializeField]
	private Transform[] m_fireOrigins;

	[SerializeField]
	private int m_salvoRocketCount = 4;

	[SerializeField]
	private float m_salvoInterval = 0.25f;

	[SerializeField]
	private float m_range = 2000f;

	[SerializeField]
	private LayerMask m_collisionLayerMask;

	[SerializeField]
	private GameObject m_hazardPreviewIndicatorPrefab;

	private int m_ammo;

	private bool m_isChargingToFire;

	private float m_countdown;

	private bool m_didFire;

	private Vector3 m_inheritedVelocity;

	private GameObject m_chargeIndicator;

	private void Start()
	{
		m_ammo = m_ammoMax;
		m_countdown = m_fireCountdown;
		m_chargeIndicator = Object.Instantiate(m_hazardPreviewIndicatorPrefab);
		m_chargeIndicator.GetComponent<HazardPreviewWarning>().SetTracker(Singleton<TagManager>.Instance.GetUICamera(), base.gameObject, 15f);
		m_chargeIndicator.SetActive(value: false);
	}

	private void OnDestroy()
	{
		if (m_chargeIndicator != null)
		{
			Object.Destroy(m_chargeIndicator);
		}
	}

	public void SetCharging(bool charging)
	{
		if (m_isChargingToFire != charging)
		{
			m_countdown = m_fireCountdown;
			m_isChargingToFire = charging;
		}
	}

	public void SetInheritedVelocity(Vector3 ihv)
	{
		m_inheritedVelocity = ihv;
	}

	public bool DidFire()
	{
		return m_didFire;
	}

	public void ResetDidFireTrigger()
	{
		m_didFire = false;
	}

	public bool HasAmmo()
	{
		return m_ammo > 0;
	}

	private void Update()
	{
		if (m_isChargingToFire)
		{
			m_countdown -= Time.deltaTime;
			if (m_countdown < m_fireCountdown * 0.8f)
			{
				m_chargeIndicator.SetActive(value: true);
				m_chargeIndicator.GetComponent<HazardPreviewWarning>().SetCountdown(m_countdown / m_fireCountdown, m_countdown);
			}
			else
			{
				m_chargeIndicator.SetActive(value: false);
			}
			if (m_countdown < 0f)
			{
				m_didFire = true;
				m_isChargingToFire = false;
				m_countdown = m_fireCountdown;
				m_chargeIndicator.SetActive(value: false);
				StopAllCoroutines();
				StartCoroutine("DoRocketSalvo");
			}
		}
		else
		{
			m_chargeIndicator.SetActive(value: false);
		}
	}

	private IEnumerator DoRocketSalvo()
	{
		int fireOriginIndex = 0;
		for (int i = 0; i < m_salvoRocketCount; i++)
		{
			FireRocket(m_fireOrigins[fireOriginIndex]);
			fireOriginIndex++;
			if (fireOriginIndex >= m_fireOrigins.Length)
			{
				fireOriginIndex = 0;
			}
			yield return new WaitForSeconds(m_salvoInterval);
		}
		yield return null;
	}

	private void FireRocket(Transform fireOrigin)
	{
		Singleton<CommonEffectManager>.Instance.ProjectileEffect(m_projectileToFire, fireOrigin.position, fireOrigin.position + fireOrigin.forward * m_range, 1f, m_collisionLayerMask, m_inheritedVelocity, null, isDouble: false, isQuad: false, isFake: false);
		WingroveRoot.Instance.PostEventGO(m_projectileToFire.GetFireAudioHook(), base.gameObject);
		m_ammo--;
	}
}
