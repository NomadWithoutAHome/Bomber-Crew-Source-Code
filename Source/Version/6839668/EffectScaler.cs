using System.Collections;
using UnityEngine;

public class EffectScaler : MonoBehaviour
{
	[SerializeField]
	private EffectScalerHelper[] m_helpers;

	private void OnEnable()
	{
		for (int i = 0; i < m_helpers.Length; i++)
		{
			if (m_helpers[i] != null)
			{
				m_helpers[i].SetUp();
			}
		}
		SetScale(0f);
	}

	public void SetScale(float scale)
	{
		for (int i = 0; i < m_helpers.Length; i++)
		{
			if (m_helpers[i] != null)
			{
				m_helpers[i].SetEffectScale(scale);
			}
		}
	}

	public void DestroyIn(float time)
	{
		StartCoroutine(DestroyInCo(time));
	}

	private IEnumerator DestroyInCo(float time)
	{
		yield return new WaitForSeconds(time);
		Object.Destroy(base.gameObject);
	}
}
