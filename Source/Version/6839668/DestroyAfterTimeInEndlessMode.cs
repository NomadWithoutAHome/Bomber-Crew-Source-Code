using System.Collections;
using BomberCrewCommon;
using UnityEngine;

public class DestroyAfterTimeInEndlessMode : MonoBehaviour
{
	[SerializeField]
	private float m_destroyTime = 25f;

	[SerializeField]
	private bool m_repool;

	private void OnEnable()
	{
		if (Singleton<GameFlow>.Instance.GetGameMode().UseEndlessDifficulty())
		{
			StartCoroutine(DestroyInTime());
		}
	}

	private IEnumerator DestroyInTime()
	{
		yield return new WaitForSeconds(m_destroyTime);
		if (m_repool)
		{
			Singleton<PoolManager>.Instance.ReturnToPool(base.gameObject);
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
	}
}
