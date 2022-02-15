using UnityEngine;

public class CustomLiveryBrushSelect : MonoBehaviour
{
	[SerializeField]
	private tk2dSprite m_spriteToSet;

	public void SetUp(string spriteName)
	{
		m_spriteToSet.SetSprite(spriteName);
	}
}
