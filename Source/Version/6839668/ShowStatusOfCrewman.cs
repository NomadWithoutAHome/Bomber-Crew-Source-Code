using UnityEngine;

public class ShowStatusOfCrewman : MonoBehaviour
{
	[SerializeField]
	private GameObject m_prefabToShow;

	[SerializeField]
	private tk2dUIItem m_hoverItem;

	[SerializeField]
	private CrewmanAvatar m_crewmanAvatar;

	private GameObject m_createdPrefab;

	private void Start()
	{
		m_createdPrefab = Object.Instantiate(m_prefabToShow);
		m_createdPrefab.GetComponent<UICrewmanOverlay>().SetUp(m_crewmanAvatar);
	}

	private void OnDisable()
	{
		if (m_createdPrefab != null)
		{
			Object.Destroy(m_createdPrefab);
		}
	}
}
