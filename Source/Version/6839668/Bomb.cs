using System;
using System.Collections;
using BomberCrewCommon;
using dbox;
using UnityEngine;
using WingroveAudio;

public class Bomb : Damageable
{
	protected Rigidbody m_rigidBody;

	protected bool m_released;

	[SerializeField]
	protected float m_rotateSpeed;

	[SerializeField]
	protected ExternalDamageExplicit m_hitGroundDamage;

	[SerializeField]
	protected float m_health;

	[SerializeField]
	protected GameObject m_explosionPrefab;

	[SerializeField]
	protected Transform m_centreOfMass;

	[SerializeField]
	protected Rigidbody m_tailFinRigidbody;

	[SerializeField]
	protected DamageFlash m_damageFlash;

	[SerializeField]
	protected GameObject m_releaseEffect;

	[SerializeField]
	protected GameObject m_craterPrefab;

	[SerializeField]
	private bool m_isGrandSlamBomb;

	[SerializeField]
	private float m_grandSlamMinAltitude;

	[SerializeField]
	private bool m_ignoreFireDamage;

	[SerializeField]
	private float m_groundDamageMultiplier = 1f;

	protected LayerMask m_groundHitLayerMask;

	protected FlashManager.ActiveFlash m_currentFlash;

	protected bool m_destroyed;

	protected float m_currentHealth;

	private float m_damageMultiplier = 1f;

	private BomberSystems m_bomberSystems;

	private int m_explosionPrefabInstanceId;

	public event Action OnExplode;

	private void Start()
	{
		m_explosionPrefabInstanceId = m_explosionPrefab.GetInstanceID();
		if (m_rigidBody == null)
		{
			m_rigidBody = base.gameObject.GetComponent<Rigidbody>();
		}
		m_currentHealth = m_health;
		m_groundHitLayerMask = 1 << LayerMask.NameToLayer("Environment");
		if (m_releaseEffect != null)
		{
			m_releaseEffect.SetActive(value: false);
		}
		m_bomberSystems = Singleton<BomberSpawn>.Instance.GetBomberSystems();
	}

	public void SetDamageMultiplier(float amt)
	{
		m_damageMultiplier = amt;
	}

	public float GetDamageAmt()
	{
		return m_hitGroundDamage.GetDamage();
	}

	public virtual void Release()
	{
		m_bomberSystems = Singleton<BomberSpawn>.Instance.GetBomberSystems();
		Vector3 position = base.transform.position;
		m_released = true;
		m_rigidBody.isKinematic = false;
		if (m_tailFinRigidbody != null)
		{
			m_tailFinRigidbody.isKinematic = false;
		}
		m_rigidBody.centerOfMass = m_centreOfMass.localPosition;
		m_rigidBody.velocity = m_bomberSystems.GetBomberState().GetPhysicsModel().GetVelocity();
		WingroveRoot.Instance.PostEventGO("BOMB_DROP", base.gameObject);
		BigTransform bigTransform = base.gameObject.AddComponent<BigTransform>();
		base.transform.parent = null;
		bigTransform.SetFromCurrentPage(position);
		m_damageFlash.ReturnToNormal();
		if (m_isGrandSlamBomb && base.transform.position.y < m_grandSlamMinAltitude)
		{
			SetDamageMultiplier(0.1f);
		}
		if (m_releaseEffect != null)
		{
			m_releaseEffect.SetActive(value: true);
		}
	}

	public bool IsReleased()
	{
		return m_released;
	}

	private void OnCollisionEnter(Collision c)
	{
		if (m_released)
		{
			CreateCrater();
			Explode();
		}
	}

	private void FixedUpdate()
	{
		if (m_released && base.transform.position.y < 0f && !m_destroyed)
		{
			Explode();
		}
	}

	public void Explode()
	{
		if (!m_destroyed)
		{
			if (this.OnExplode != null)
			{
				this.OnExplode();
			}
			m_destroyed = true;
			DboxInMissionController.DBoxCall(DboxSdkWrapper.PostExplosion, base.transform.position);
			WingroveRoot.Instance.PostEventGO("BOMB_DROP_STOP", base.gameObject);
			GameObject fromPool = Singleton<PoolManager>.Instance.GetFromPool(m_explosionPrefab, m_explosionPrefabInstanceId);
			fromPool.btransform().SetFromCurrentPage(base.transform.position);
			StartCoroutine(DoExplosionDamage());
		}
	}

	private IEnumerator DoExplosionDamage()
	{
		yield return null;
		WingroveRoot.Instance.PostEventGO("BOMB_DROP_STOP", base.gameObject);
		m_hitGroundDamage.DoDamage(m_damageMultiplier);
		UnityEngine.Object.Destroy(base.gameObject);
	}

	public void CreateCrater()
	{
		if (m_craterPrefab != null)
		{
			Ray ray = new Ray(new Vector3(base.transform.position.x, 150f, base.transform.position.z), Vector3.down);
			if (Physics.Raycast(ray, out var hitInfo, 150f, m_groundHitLayerMask) && hitInfo.point.y > 0.5f)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(m_craterPrefab);
				gameObject.btransform().SetFromCurrentPage(hitInfo.point);
				gameObject.transform.localEulerAngles = new Vector3(0f, UnityEngine.Random.Range(-180f, 180f), 0f);
				float num = UnityEngine.Random.Range(0.85f, 1.2f);
				gameObject.transform.localScale = new Vector3(num, num, num);
			}
		}
	}

	public override bool IsDamageBlocker()
	{
		return false;
	}

	public override float DamageGetPassthrough(float amt, DamageSource damageSource)
	{
		if (!m_destroyed && !m_released)
		{
			if (m_ignoreFireDamage && damageSource.m_damageType == DamageSource.DamageType.Fire)
			{
				return 0f;
			}
			if (m_groundDamageMultiplier != 0f && damageSource.m_damageType == DamageSource.DamageType.GroundImpact)
			{
				amt *= m_groundDamageMultiplier;
			}
			if (m_damageFlash != null)
			{
				m_damageFlash.DoFlash();
			}
			m_currentHealth -= amt;
			if (m_currentHealth <= 0f)
			{
				Explode();
			}
			if (m_currentHealth < m_health / 2f && m_damageFlash != null)
			{
				m_damageFlash.DoLowHealth(m_currentHealth < m_health / 4f);
			}
		}
		return 0f;
	}

	public override bool DirectDamageOnly()
	{
		return true;
	}
}
