using UnityEngine;

public class CrewmanStatusAlert : MonoBehaviour
{
	[SerializeField]
	private tk2dSprite m_iconSprite;

	public void SetUp(string sprite)
	{
		m_iconSprite.SetSprite(sprite);
	}
}
