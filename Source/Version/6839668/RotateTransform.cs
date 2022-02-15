using UnityEngine;

public class RotateTransform : MonoBehaviour
{
	[SerializeField]
	private Vector3 m_speed;

	[SerializeField]
	private bool m_useUnscaledTime;

	private void Update()
	{
		if (m_useUnscaledTime)
		{
			base.transform.localEulerAngles += m_speed * Time.unscaledDeltaTime;
		}
		else
		{
			base.transform.localEulerAngles += m_speed * Time.deltaTime;
		}
	}
}
