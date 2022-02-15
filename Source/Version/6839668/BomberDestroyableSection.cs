using System;
using BomberCrewCommon;
using UnityEngine;
using WingroveAudio;

public class BomberDestroyableSection : MonoBehaviour
{
	[SerializeField]
	private SmoothDamageable m_damageableToMonitor;

	[SerializeField]
	private float m_chanceOfInstantDetachOnDestroy;

	[SerializeField]
	private float m_chanceOfDelayedDetachOnDestroy;

	[SerializeField]
	private float m_minDetachDelay;

	[SerializeField]
	private float m_maxDetachDelay;

	[SerializeField]
	private GameObject m_primaryHierarchyToDetach;

	[SerializeField]
	private GameObject[] m_additionalHierachies;

	[SerializeField]
	private BomberDestroyableSection[] m_childDestroyableSections;

	[SerializeField]
	private Mesh m_primaryCollisionMesh;

	[SerializeField]
	private GameObject m_toEnableOnDestroyed;

	private bool m_isDestroyed;

	private bool m_detached;

	private bool m_preparingToDetach;

	private float m_detachCountdown;

	private BomberSystems m_bomberSystems;

	private int m_lives;

	private bool m_useLives;

	private bool m_hasUsedLife;

	public event Action OnSectionDestroy;

	private void Start()
	{
		m_bomberSystems = Singleton<BomberSpawn>.Instance.GetBomberSystems();
		if (m_toEnableOnDestroyed != null)
		{
			m_toEnableOnDestroyed.SetActive(value: false);
		}
	}

	public static BomberDestroyableSection FindDestroyableSectionFor(Transform t)
	{
		BomberDestroyableSection component = t.GetComponent<BomberDestroyableSection>();
		if (component != null)
		{
			return component;
		}
		if (t.parent != null)
		{
			return FindDestroyableSectionFor(t.parent);
		}
		return null;
	}

	public bool IsDestroyed()
	{
		return m_detached;
	}

	public void SetLives(int lives)
	{
		m_chanceOfInstantDetachOnDestroy = 0f;
		m_useLives = true;
		m_lives = lives;
	}

	public void Detach(Transform fromDetachBase)
	{
		if (!m_detached)
		{
			if (m_toEnableOnDestroyed != null)
			{
				m_toEnableOnDestroyed.SetActive(value: true);
			}
			Transform transform = fromDetachBase;
			if (transform == null || (m_primaryCollisionMesh != null && UnityEngine.Random.Range(0f, 1f) > 0.5f))
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(m_bomberSystems.GetDestroyedSectionPrefab());
				gameObject.transform.SetPositionAndRotation(m_primaryHierarchyToDetach.transform.position, m_primaryHierarchyToDetach.transform.rotation);
				transform = gameObject.transform;
				Vector3 velocity = m_bomberSystems.GetBomberState().GetPhysicsModel().GetVelocity();
				gameObject.GetComponent<Rigidbody>().velocity = velocity + new Vector3(0f, 8f, 0f);
				gameObject.GetComponent<Rigidbody>().angularVelocity = UnityEngine.Random.insideUnitSphere * 3f;
				WingroveRoot.Instance.PostEventGO("BOMBER_PIECE_DISCONNECT", gameObject);
			}
			m_primaryHierarchyToDetach.transform.parent = transform;
			BomberDestroyableSection[] childDestroyableSections = m_childDestroyableSections;
			foreach (BomberDestroyableSection bomberDestroyableSection in childDestroyableSections)
			{
				bomberDestroyableSection.Detach(transform);
			}
			m_detached = true;
			if (this.OnSectionDestroy != null)
			{
				this.OnSectionDestroy();
			}
			GameObject[] additionalHierachies = m_additionalHierachies;
			foreach (GameObject gameObject2 in additionalHierachies)
			{
				gameObject2.transform.parent = transform;
			}
			Collider[] componentsInChildren = m_primaryHierarchyToDetach.GetComponentsInChildren<Collider>();
			Collider[] array = componentsInChildren;
			foreach (Collider collider in array)
			{
				collider.enabled = false;
			}
			GameObject gameObject3 = new GameObject("Collider_" + base.name);
			gameObject3.transform.parent = transform;
			gameObject3.transform.SetPositionAndRotation(base.transform.position, base.transform.rotation);
			MeshCollider meshCollider = gameObject3.AddComponent<MeshCollider>();
			meshCollider.sharedMesh = m_primaryCollisionMesh;
			meshCollider.convex = true;
			meshCollider.inflateMesh = true;
		}
	}

	private void Update()
	{
		if (!m_isDestroyed)
		{
			if (m_damageableToMonitor != null && m_damageableToMonitor.GetHealthNormalised() == 0f)
			{
				m_isDestroyed = true;
				if (m_useLives)
				{
					if (!m_hasUsedLife)
					{
						m_hasUsedLife = true;
						m_lives--;
						if (m_lives <= 0)
						{
							Detach(null);
						}
					}
				}
				else if (UnityEngine.Random.Range(0f, 1f) <= m_chanceOfInstantDetachOnDestroy)
				{
					Detach(null);
				}
				else if (UnityEngine.Random.Range(0f, 1f) <= m_chanceOfDelayedDetachOnDestroy)
				{
					m_preparingToDetach = true;
					m_detachCountdown = UnityEngine.Random.Range(m_minDetachDelay, m_maxDetachDelay);
				}
			}
			else
			{
				m_hasUsedLife = false;
			}
		}
		else
		{
			if (m_detached)
			{
				return;
			}
			if (m_damageableToMonitor.GetHealthNormalised() != 0f)
			{
				m_preparingToDetach = false;
				m_isDestroyed = false;
				m_hasUsedLife = false;
			}
			if (m_preparingToDetach)
			{
				m_detachCountdown -= Time.deltaTime;
				if (m_detachCountdown < 0f)
				{
					Detach(null);
				}
			}
		}
	}
}
