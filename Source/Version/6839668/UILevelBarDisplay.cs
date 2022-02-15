using UnityEngine;

public class UILevelBarDisplay : MonoBehaviour
{
	[SerializeField]
	private tk2dClippedSprite m_levelSprite;

	private Rect m_clippedRect;

	private void Start()
	{
		m_clippedRect = new Rect(m_levelSprite.ClipRect.x, m_levelSprite.ClipRect.y, m_levelSprite.ClipRect.width, m_levelSprite.ClipRect.height);
	}

	public void DisplayValue(float value)
	{
		m_clippedRect.height = value;
		m_levelSprite.ClipRect = m_clippedRect;
	}
}
