using UnityEngine;

public class ShowStatusOfDamageable : MonoBehaviour
{
	[SerializeField]
	private GameObject m_prefabToShow;

	[SerializeField]
	private Damageable m_damageable;

	[SerializeField]
	private Transform m_transformToTrack;

	[SerializeField]
	private tk2dUIItem m_hoverItem;

	[SerializeField]
	private Collider m_collider;

	private GameObject m_createdPrefab;

	private void Awake()
	{
		m_hoverItem.OnHoverOver += m_hoverItem_OnHoverOver;
		m_hoverItem.OnHoverOut += m_hoverItem_OnHoverOut;
	}

	private void Update()
	{
	}

	private void OnDisable()
	{
		m_hoverItem_OnHoverOut();
	}

	private void m_hoverItem_OnHoverOut()
	{
		if (m_createdPrefab != null)
		{
			Object.Destroy(m_createdPrefab);
		}
	}

	private void m_hoverItem_OnHoverOver()
	{
		if (m_createdPrefab != null)
		{
			Object.Destroy(m_createdPrefab);
		}
		m_createdPrefab = Object.Instantiate(m_prefabToShow);
		m_createdPrefab.GetComponent<UISystemOverlay>().SetUp(m_damageable, m_transformToTrack);
	}
}
