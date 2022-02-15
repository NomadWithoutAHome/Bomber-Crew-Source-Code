using UnityEngine;

public class ScrollTexture : MonoBehaviour
{
	[SerializeField]
	private Vector2 m_scrollSpeed = Vector2.zero;

	[SerializeField]
	private float m_multiplier = 1f;

	private Vector2 m_offset = Vector2.zero;

	private void Start()
	{
		m_offset = GetComponent<Renderer>().material.mainTextureOffset;
	}

	public void SetMultiplier(float multiplier)
	{
		m_multiplier = multiplier;
	}

	private void Update()
	{
		GetComponent<Renderer>().material.mainTextureOffset = m_offset;
		m_offset.x += m_scrollSpeed.x * Time.deltaTime * m_multiplier;
		m_offset.y += m_scrollSpeed.y * Time.deltaTime * m_multiplier;
		if (m_scrollSpeed.x > 0f)
		{
			if (m_offset.x >= 1f)
			{
				m_offset.x -= 1f;
			}
		}
		else if (m_offset.x <= -1f)
		{
			m_offset.x += 1f;
		}
		if (m_scrollSpeed.y > 0f)
		{
			if (m_offset.y >= 1f)
			{
				m_offset.y -= 1f;
			}
		}
		else if (m_offset.y <= -1f)
		{
			m_offset.y += 1f;
		}
	}
}
