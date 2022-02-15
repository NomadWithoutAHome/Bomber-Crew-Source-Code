using BomberCrewCommon;
using UnityEngine;

public class ShowGameCompleteCrewmanInfo : MonoBehaviour
{
	[SerializeField]
	private TextSetter m_firstNameText;

	[SerializeField]
	private TextSetter m_secondNameText;

	[SerializeField]
	private CrewmanSkillDisplay m_skillDisplay;

	[SerializeField]
	private Renderer m_portraitRenderer;

	public void SetUp(Crewman cm)
	{
		m_firstNameText.SetText(cm.GetFirstName());
		m_secondNameText.SetText(cm.GetSurname().ToUpper());
		m_skillDisplay.SetUp(cm, 0);
		m_portraitRenderer.material.mainTexture = Singleton<CrewmanPhotoBooth>.Instance.RenderForCrewman(cm, null);
	}
}
