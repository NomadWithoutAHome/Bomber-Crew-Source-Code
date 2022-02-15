using UnityEngine;
using WingroveAudio;

public class AmbientSoundInfrequent : MonoBehaviour
{
	[SerializeField]
	private float m_timeMin;

	[SerializeField]
	private float m_timeMax;

	[SerializeField]
	[AudioEventName]
	private string m_audioEvent;

	private float m_counter;

	private void Awake()
	{
		m_counter = Random.Range(0f, m_timeMax);
	}

	private void Update()
	{
		m_counter -= Time.deltaTime;
		if (m_counter < 0f)
		{
			WingroveRoot.Instance.PostEventGO(m_audioEvent, base.gameObject);
			m_counter = Random.Range(m_timeMin, m_timeMax);
		}
	}
}
