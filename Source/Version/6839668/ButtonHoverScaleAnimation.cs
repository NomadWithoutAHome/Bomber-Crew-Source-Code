using UnityEngine;

public class ButtonHoverScaleAnimation : MonoBehaviour
{
	[SerializeField]
	private tk2dSlicedSprite m_sprite;

	[SerializeField]
	private float m_scaleAmount = 10f;

	[SerializeField]
	private float m_scaleSpeed = 8f;

	private Vector3 m_startScale = Vector3.one;

	private Vector3 m_newScale = Vector3.one;

	private float m_transformScaleAmount;

	private void Start()
	{
		m_startScale = m_sprite.transform.localScale;
		m_transformScaleAmount = m_scaleAmount / m_sprite.dimensions.x;
		m_startScale.x -= m_transformScaleAmount * 0.25f;
		m_newScale = m_startScale;
	}

	private void Update()
	{
		float num = m_transformScaleAmount * Mathf.Sin(Time.unscaledTime * m_scaleSpeed);
		m_newScale.x = m_startScale.x + num;
		m_sprite.transform.localScale = m_newScale;
	}
}
