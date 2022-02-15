using Common;
using UnityEngine;

public class CrewWalkArrowPreview : MonoBehaviour
{
	[SerializeField]
	private GameObject m_enableNode;

	private bool m_enabled;

	public void SetVisible(bool visible)
	{
		if (m_enabled != visible)
		{
			m_enableNode.CustomActivate(visible);
			m_enabled = visible;
		}
	}
}
