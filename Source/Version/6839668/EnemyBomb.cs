using System.Collections;
using BomberCrewCommon;
using dbox;
using UnityEngine;
using WingroveAudio;

public class EnemyBomb : MonoBehaviour
{
	[SerializeField]
	protected ExternalDamageExplicit m_hitGroundDamage;

	[SerializeField]
	protected GameObject m_explosionPrefab;

	[SerializeField]
	private float m_moveSpeed;

	protected LayerMask m_groundHitLayerMask;

	private Shootable m_target;

	private bool m_destroyed;

	private Vector3 m_direction;

	private int m_explosionPrefabInstanceId;

	private float m_currentSpeed;

	private void Start()
	{
		m_explosionPrefabInstanceId = m_explosionPrefab.GetInstanceID();
		m_groundHitLayerMask = 1 << LayerMask.NameToLayer("Environment");
	}

	public float GetDamageAmt()
	{
		return m_hitGroundDamage.GetDamage();
	}

	public void Release(Shootable target, float speed)
	{
		m_target = target;
		m_currentSpeed = speed;
		WingroveRoot.Instance.PostEventGO("BOMB_DROP", base.gameObject);
		m_direction = Vector3.down;
	}

	private void OnCollisionEnter(Collision c)
	{
		Explode();
	}

	private void FixedUpdate()
	{
		if (m_target != null)
		{
			Vector3 vector = m_target.GetCentreTransform().position - base.transform.position;
			m_direction = vector.normalized;
			if (vector.magnitude < 30f)
			{
				Explode();
			}
		}
		base.transform.position += m_direction * m_currentSpeed * Time.deltaTime;
		m_currentSpeed = (m_moveSpeed + m_currentSpeed) / 2f;
		if (base.transform.position.y < 0f && !m_destroyed)
		{
			Explode();
		}
	}

	public void Explode()
	{
		if (!m_destroyed)
		{
			m_destroyed = true;
			WingroveRoot.Instance.PostEventGO("BOMB_DROP_STOP", base.gameObject);
			DboxInMissionController.DBoxCall(DboxSdkWrapper.PostExplosion, base.transform.position);
			GameObject fromPool = Singleton<PoolManager>.Instance.GetFromPool(m_explosionPrefab, m_explosionPrefabInstanceId);
			fromPool.btransform().SetFromCurrentPage(base.transform.position);
			StartCoroutine(DoExplosionDamage());
		}
	}

	private IEnumerator DoExplosionDamage()
	{
		yield return null;
		WingroveRoot.Instance.PostEventGO("BOMB_DROP_STOP", base.gameObject);
		m_hitGroundDamage.DoDamage();
		Object.Destroy(base.gameObject);
	}
}
