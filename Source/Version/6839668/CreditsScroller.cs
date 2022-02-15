using UnityEngine;

public class CreditsScroller : MonoBehaviour
{
	[SerializeField]
	private Transform m_nodeToScroll;

	[SerializeField]
	private float m_speedToScrollAt;

	[SerializeField]
	private float m_preScrollDelay;

	private void Start()
	{
		m_nodeToScroll.transform.localPosition = new Vector3(0f, 0f - m_preScrollDelay * m_speedToScrollAt, 0f);
	}

	private void Update()
	{
		m_nodeToScroll.transform.localPosition += new Vector3(0f, m_speedToScrollAt * Time.deltaTime, 0f);
	}
}
