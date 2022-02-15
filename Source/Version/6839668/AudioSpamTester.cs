using UnityEngine;
using WingroveAudio;

public class AudioSpamTester : MonoBehaviour
{
	[SerializeField]
	private float m_timer;

	[SerializeField]
	private string m_event;

	private float m_t;

	private void Update()
	{
		m_t += Time.deltaTime;
		if (m_timer > 0f)
		{
			while (m_t > m_timer)
			{
				WingroveRoot.Instance.PostEventGO(m_event, base.gameObject);
				m_t -= m_timer;
			}
		}
	}
}
