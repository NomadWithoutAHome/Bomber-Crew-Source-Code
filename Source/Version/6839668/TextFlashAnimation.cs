using UnityEngine;

public class TextFlashAnimation : MonoBehaviour
{
	[SerializeField]
	private tk2dTextMesh m_textMesh;

	[SerializeField]
	private float m_flashRate = 4f;

	private Color m_spriteColor;

	private float m_spriteStartAlpha;

	private void OnEnable()
	{
		if (m_textMesh == null)
		{
			m_textMesh = base.gameObject.GetComponent<tk2dTextMesh>();
		}
		m_spriteColor = m_textMesh.color;
		m_spriteColor.a = 1f;
	}

	private void Update()
	{
		float a = 0.5f + Mathf.Sin(Time.time * m_flashRate) * 0.5f * 0.7f;
		m_spriteColor.a = a;
		m_textMesh.color = m_spriteColor;
	}
}
