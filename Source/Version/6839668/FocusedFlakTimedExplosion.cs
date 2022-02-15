using BomberCrewCommon;
using dbox;
using UnityEngine;

public class FocusedFlakTimedExplosion : MonoBehaviour
{
	[SerializeField]
	private float m_explosionDuration;

	[SerializeField]
	private int m_numExplosions;

	[SerializeField]
	private float m_radius;

	[SerializeField]
	private GameObject m_flakExplosionPrefab;

	[SerializeField]
	private GameObject m_preExplodePrefab;

	private float m_timerCountdownStart;

	private float m_timerCountdown;

	private float m_secondCountdown;

	private GameObject m_preExplodeGameObject;

	private float m_frameCountdown;

	private int m_flakPrefabInstanceId;

	private void Awake()
	{
		m_flakPrefabInstanceId = m_flakExplosionPrefab.GetInstanceID();
		m_preExplodeGameObject = Object.Instantiate(m_preExplodePrefab);
		m_preExplodeGameObject.GetComponent<HazardPreviewWarning>().SetTracker(Singleton<TagManager>.Instance.GetUICamera(), base.gameObject, m_radius);
		m_preExplodeGameObject.SetActive(value: false);
	}

	public void SetUp(float timeToExplode)
	{
		m_timerCountdownStart = (m_timerCountdown = timeToExplode - m_explosionDuration * 0.5f);
		m_secondCountdown = m_explosionDuration;
	}

	private void OnDestroy()
	{
		if (m_preExplodeGameObject != null)
		{
			Object.Destroy(m_preExplodeGameObject);
		}
	}

	private void Update()
	{
		m_timerCountdown -= Time.deltaTime;
		if (m_timerCountdown < 0f)
		{
			m_preExplodeGameObject.SetActive(value: false);
			m_secondCountdown -= Time.deltaTime;
			m_frameCountdown -= Time.deltaTime;
			while (m_frameCountdown <= 0f)
			{
				if (m_frameCountdown < -1f / 6f)
				{
					m_frameCountdown = -2f / 15f;
				}
				Vector3 insideUnitSphere = Random.insideUnitSphere;
				Vector3 vector = base.transform.position + insideUnitSphere * Random.Range(0f, m_radius) * Random.Range(0.75f, 1f);
				GameObject fromPool = Singleton<PoolManager>.Instance.GetFromPool(m_flakExplosionPrefab, m_flakPrefabInstanceId);
				fromPool.transform.localScale = m_flakExplosionPrefab.transform.localScale;
				fromPool.gameObject.btransform().SetFromCurrentPage(vector);
				DboxInMissionController.DBoxCall(DboxSdkWrapper.PostFlak, vector);
				m_frameCountdown += 1f / 30f;
			}
			if (m_secondCountdown < 0f)
			{
				Object.Destroy(base.gameObject);
			}
		}
		else
		{
			m_preExplodeGameObject.SetActive(value: true);
			m_preExplodeGameObject.GetComponent<HazardPreviewWarning>().SetCountdown(m_timerCountdown / m_timerCountdownStart, m_timerCountdown);
		}
	}
}
