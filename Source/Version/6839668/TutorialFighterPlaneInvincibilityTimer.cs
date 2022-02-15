using UnityEngine;

public class TutorialFighterPlaneInvincibilityTimer : MonoBehaviour
{
	[SerializeField]
	private FighterPlane m_fighterPlane;

	[SerializeField]
	private FighterAI m_ai;

	private void Start()
	{
		m_fighterPlane.SetInvincible(invincible: true);
	}

	private void Update()
	{
		if (m_ai.GetAIState() == FighterAI.FighterState.ReturningToDistance)
		{
			m_fighterPlane.SetInvincible(invincible: false);
		}
	}
}
