using UnityEngine;

public class TestOpenParachute : MonoBehaviour
{
	[SerializeField]
	private GameObject m_parachute;

	[SerializeField]
	private Rigidbody m_dragRigidbody;

	[SerializeField]
	private float m_dragValue;

	[SerializeField]
	private float m_decellerationRate = 10f;

	private bool m_chuteOpen;

	private void Start()
	{
		m_parachute.SetActive(value: false);
		m_dragRigidbody.drag = 0f;
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			m_parachute.SetActive(value: true);
			m_chuteOpen = true;
		}
		if (m_chuteOpen && m_dragRigidbody.drag < m_dragValue)
		{
			m_dragRigidbody.drag += Time.deltaTime * m_decellerationRate;
		}
	}
}
