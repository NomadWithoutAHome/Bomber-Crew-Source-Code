using UnityEngine;

public class HoverHighlightAnimation : MonoBehaviour
{
	[SerializeField]
	private tk2dBaseSprite m_sprite;

	[SerializeField]
	private float m_flashRate = 4f;

	[SerializeField]
	private bool m_useUnscaledTime;

	private Color m_spriteColor;

	private float m_spriteStartAlpha;

	private float m_alphaMod;

	private float m_time;

	private void OnEnable()
	{
		if (m_sprite == null)
		{
			m_sprite = base.gameObject.GetComponent<tk2dBaseSprite>();
		}
		m_spriteColor = m_sprite.color;
		m_spriteColor.a = 1f;
		m_time = 0f;
		Update();
	}

	private void Update()
	{
		if (m_useUnscaledTime)
		{
			m_alphaMod = 0.5f + Mathf.Sin(m_time * m_flashRate) * 0.5f * 0.7f;
			m_time += Time.unscaledDeltaTime;
		}
		else
		{
			m_alphaMod = 0.5f + Mathf.Sin(m_time * m_flashRate) * 0.5f * 0.7f;
			m_time += Time.deltaTime;
		}
		m_spriteColor.a = m_alphaMod;
		m_sprite.color = m_spriteColor;
	}
}
