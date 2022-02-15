using UnityEngine;

public class AnimateFilmstripTexture : MonoBehaviour
{
	[SerializeField]
	private Renderer m_renderer;

	[SerializeField]
	private string m_materialProperty;

	[SerializeField]
	private int m_framesX = 8;

	[SerializeField]
	private int m_framesY = 8;

	[SerializeField]
	private float m_framerate = 30f;

	private Vector2 m_offset;

	private float m_counter;

	private float m_interval;

	private Material m_material;

	private void Start()
	{
		m_material = m_renderer.material;
		m_interval = 1f / m_framerate;
		m_offset = new Vector2(0f, 0f);
	}

	private void Update()
	{
		m_interval = 1f / m_framerate;
		if (m_counter >= m_interval)
		{
			m_offset.x += 1f / (float)m_framesX;
			if (m_offset.x >= 1f)
			{
				m_offset.x = 0f;
				m_offset.y -= 1f / (float)m_framesY;
				if (m_offset.y <= -1f)
				{
					m_offset.y = 0f;
				}
			}
			m_material.SetTextureOffset(m_materialProperty, m_offset);
			m_counter = 0f;
		}
		m_counter += Time.deltaTime;
	}
}
