using BomberCrewCommon;
using UnityEngine;

public class MedKit : MonoBehaviour
{
	[SerializeField]
	private int m_maxUses = 1;

	private int m_numUsesRemaining;

	private void Awake()
	{
		m_numUsesRemaining = m_maxUses;
	}

	private void Start()
	{
		if (Singleton<BomberSpawn>.Instance != null)
		{
			Singleton<BomberSpawn>.Instance.GetBomberSystems().RegisterInteractiveSearchable(typeof(MedKit), GetComponent<InteractiveItem>());
		}
	}

	public void Use()
	{
		m_numUsesRemaining--;
		if (m_numUsesRemaining == 0)
		{
			CrewmanAvatar carriedBy = GetComponent<CarryableItem>().GetCarriedBy();
			if (carriedBy != null)
			{
				carriedBy.DropItem();
				Object.Destroy(base.gameObject);
			}
		}
	}
}
