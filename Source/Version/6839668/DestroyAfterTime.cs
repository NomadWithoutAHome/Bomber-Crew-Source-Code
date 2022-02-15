using System.Collections;
using BomberCrewCommon;
using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
	[SerializeField]
	private float m_duration;

	[SerializeField]
	private bool m_returnToPool;

	private IEnumerator Start()
	{
		yield return new WaitForSeconds(m_duration);
		if (m_returnToPool)
		{
			Singleton<PoolManager>.Instance.ReturnToPool(base.gameObject);
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
	}
}
