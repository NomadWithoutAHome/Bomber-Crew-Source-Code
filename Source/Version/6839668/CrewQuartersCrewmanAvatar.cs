using UnityEngine;

public class CrewQuartersCrewmanAvatar : MonoBehaviour
{
	[SerializeField]
	private CrewmanGraphicsInstantiate m_crewmanGraphicsInstantiate;

	private CrewmanGraphics m_crewmanGraphics;

	private Crewman m_crewman;

	public void SetUp(Crewman crewman)
	{
		m_crewman = crewman;
		m_crewmanGraphicsInstantiate.Init(m_crewman);
		m_crewmanGraphics = m_crewmanGraphicsInstantiate.GetCrewmanGraphics();
		RefreshEquipment();
		m_crewmanGraphics.SetFromCrewman(m_crewman);
	}

	public void RefreshEquipment()
	{
		foreach (CrewmanEquipmentBase item in m_crewman.GetAllEquipment())
		{
			m_crewmanGraphics.SetupGearItemGraphics(item, m_crewman, asPreviewOnly: false);
		}
	}

	public void ShowEquipment(CrewmanEquipmentBase ceb)
	{
		m_crewmanGraphics.SetupGearItemGraphics(ceb, m_crewman, !m_crewman.GetAllEquipment().Contains(ceb));
	}
}
