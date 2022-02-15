using Newtonsoft.Json;

[JsonObject(MemberSerialization.OptIn)]
public class CrewmanTraitJS
{
	[JsonProperty]
	private string m_traitName;

	[JsonProperty]
	private int m_defenseBoost;

	[JsonProperty]
	private int m_temperatureResistBoost;

	[JsonProperty]
	private int m_oxygenTimeBoost;

	[JsonProperty]
	private float m_movementSpeedMultiplier;

	[JsonProperty]
	private int m_survivalLandBoost;

	[JsonProperty]
	private int m_survivalSeaBoost;

	public CrewmanTraitJS()
	{
	}

	public CrewmanTraitJS(CrewmanTrait ctI)
	{
		m_traitName = ctI.GetNamedText();
		m_defenseBoost = ctI.GetDefenseBoost();
		m_temperatureResistBoost = ctI.GetTemperatureResistBoost();
		m_oxygenTimeBoost = ctI.GetOxygenTimerBoost();
		m_movementSpeedMultiplier = ctI.GetMovementSpeedMultiplier();
		m_survivalLandBoost = ctI.GetSurvivalLandBoost();
		m_survivalSeaBoost = ctI.GetSurvivalSeaBoost();
	}

	public string GetNamedText()
	{
		return m_traitName;
	}

	public int GetDefenseBoost()
	{
		return m_defenseBoost;
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
