using UnityEngine;

public class PropellorSpinJitter : MonoBehaviour
{
	private float m_maxRotationJitter = 12f;

	private void Update()
	{
		base.transform.localEulerAngles = new Vector3(0f, 0f, Random.Range(0f - m_maxRotationJitter, m_maxRotationJitter));
	}
}
