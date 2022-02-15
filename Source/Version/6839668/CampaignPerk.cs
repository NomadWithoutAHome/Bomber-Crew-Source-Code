using UnityEngine;

[CreateAssetMenu(menuName = "Bomber Crew/Campaign Perk")]
public class CampaignPerk : ScriptableObject
{
	[SerializeField]
	private SaveData.PerkType m_perkType;

	[SerializeField]
	private float m_factor = 0.25f;

	[SerializeField]
	private int m_forMissions = 2;

	public SaveData.PerkType GetPerk()
	{
		return m_perkType;
	}

	public int GetMissions()
	{
		return m_forMissions;
	}

	public float GetFactor()
	{
		return m_factor;
	}
}
