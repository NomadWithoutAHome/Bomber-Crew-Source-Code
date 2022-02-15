using BomberCrewCommon;
using UnityEngine;

public class UICrewmanOverlay : MonoBehaviour
{
	[SerializeField]
	private WorldToScreenTrackerCustom m_tracker;

	[SerializeField]
	private tk2dTextMesh m_nameText;

	[SerializeField]
	private CrewmanSkillDisplay m_skillDisplay;

	[SerializeField]
	private GameObject m_enableNode;

	[SerializeField]
	private GameObject m_highlightedNode;

	[SerializeField]
	private GameObject m_selectedNode;

	[SerializeField]
	private GameObject m_defaultNode;

	[SerializeField]
	private CrewmenIconsDisplay m_iconsDisplay;

	[SerializeField]
	private CrewmanHealthBar m_healthBar;

	[SerializeField]
	private GameObject m_selectionModeActive;

	private CrewmanAvatar m_crewman;

	public void SetUp(CrewmanAvatar crewman)
	{
		m_crewman = crewman;
		m_tracker.SetTracking(new Transform[2]
		{
			crewman.GetCrewmanGraphics().GetHeadTransform(),
			crewman.GetCrewmanGraphics().GetPelvisTransform()
		}, new Transform[1] { crewman.GetCrewmanGraphics().GetHeadTransform() }, tk2dCamera.CameraForLayer(base.gameObject.layer));
		m_nameText.text = crewman.GetCrewman().GetSurname();
		m_skillDisplay.SetUp(crewman.GetCrewman(), 0);
		if (m_iconsDisplay != null)
		{
			m_iconsDisplay.SetUp(m_crewman);
		}
		if (m_healthBar != null)
		{
			m_healthBar.SetUp(m_crewman);
		}
		Refresh();
	}

	private void Update()
	{
		Refresh();
	}

	private void Refresh()
	{
		m_enableNode.SetActive(Singleton<BomberCamera>.Instance.ShouldShowCrewmenOverlays() && !m_crewman.GetHealthState().IsDead());
		m_highlightedNode.SetActive(m_crewman.IsOutlined());
		m_selectedNode.SetActive(Singleton<ContextControl>.Instance.GetCurrentlySelected() == m_crewman);
		m_defaultNode.SetActive(Singleton<ContextControl>.Instance.GetCurrentlySelected() != m_crewman && !m_crewman.IsOutlined());
		m_selectionModeActive.SetActive(Singleton<ContextControl>.Instance.IsCrewSelectMode());
	}
}
