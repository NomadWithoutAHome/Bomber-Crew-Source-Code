using System.Collections;
using UnityEngine;

public class DelayedDamageTrigger : MonoBehaviour
{
	[SerializeField]
	private float m_doDamageFor;

	[SerializeField]
	private float m_startTime;

	[SerializeField]
	private ExternalDamageExplicit m_damager;

	private void OnEnable()
	{
		StartCoroutine(DoDelayedDamageTrigger());
	}

	private IEnumerator DoDelayedDamageTrigger()
	{
		yield return new WaitForSeconds(m_startTime);
		float t = m_doDamageFor;
		while (t > 0f)
		{
			yield return null;
			t -= Time.deltaTime;
			m_damager.DoDamage(Time.deltaTime);
		}
	}
}
