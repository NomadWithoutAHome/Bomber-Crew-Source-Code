using UnityEngine;

public class RandomPos : MonoBehaviour
{
	public float m_interval = 10f;

	public float m_width = 20f;

	public float m_height = 10f;

	private float m_counter;

	private void Update()
	{
		m_counter += Time.deltaTime;
		if (m_counter >= m_interval)
		{
			base.transform.position = new Vector3(Random.Range((0f - m_width) / 2f, m_width / 2f), Random.Range((0f - m_height) / 2f, m_height / 2f), 0f);
			m_counter = 0f;
		}
	}
}
