using UnityEngine;

[CreateAssetMenu(menuName = "Bomber Crew/Tutorial Prompt")]
public class TutorialPromptSetUp : ScriptableObject
{
	[SerializeField]
	private ControlPromptIcon.ControllerSpriteButton[] m_pcButtons;

	[SerializeField]
	private ControlPromptIcon.ControllerSpriteButton[] m_controllerButtons;

	public ControlPromptIcon.ControllerSpriteButton[] GetPCButtons()
	{
		return m_pcButtons;
	}

	public ControlPromptIcon.ControllerSpriteButton[] GetControllerButtons()
	{
		return m_controllerButtons;
	}
}
