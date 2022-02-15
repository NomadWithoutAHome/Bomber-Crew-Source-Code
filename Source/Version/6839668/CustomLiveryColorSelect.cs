using UnityEngine;

public class CustomLiveryColorSelect : MonoBehaviour
{
	[SerializeField]
	private tk2dBaseSprite m_tint;

	public void SetUp(Color c)
	{
		m_tint.color = c;
	}
}
