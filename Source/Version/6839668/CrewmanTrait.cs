using UnityEngine;

[CreateAssetMenu(menuName = "Bomber Crew/Crewman Trait")]
public class CrewmanTrait : ScriptableObject
{
	[SerializeField]
	[NamedText]
	private string m_traitName;

	[SerializeField]
	private bool m_lockedToCertainSkills;

	[SerializeField]
	private int m_defenseBoost;

	[SerializeField]
	private int m_temperatureResistBoost;

	[SerializeField]
	private int m_oxygenTimeBoost;

	[SerializeField]
	private float m_movementSpeedMultiplier;

	[SerializeField]
	private int m_survivalLandBoost;

	[SerializeField]
	private int m_survivalSeaBoost;

	public string GetNamedText()
	{
		return m_traitName;
	}

	public int GetDefenseBoost()
	{
		return m_defenseBoost;
	}

	public void SetTraitName(string name)
	{
		m_traitName = name;
	}

	public int GetTemperatureResistBoost()
	{
		return m_temperatureResistBoost;
	}

	public int GetOxygenTimerBoost()
	{
		return m_oxygenTimeBoost;
	}

	public int GetSurvivalLandBoost()
	{
		return m_survivalLandBoost;
	}

	public int GetSurvivalSeaBoost()
	{
		return m_survivalSeaBoost;
	}

	public float GetMovementSpeedMultiplier()
	{
		if (m_movementSpeedMultiplier == 0f)
		{
			return 1f;
		}
		return m_movementSpeedMultiplier;
	}
}
