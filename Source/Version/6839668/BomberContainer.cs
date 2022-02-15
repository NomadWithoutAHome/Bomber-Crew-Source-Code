using BomberCrewCommon;
using UnityEngine;

public class BomberContainer : Singleton<BomberContainer>
{
	[SerializeField]
	private BomberLivery m_livery;

	public BomberUpgradeConfig GetCurrentConfig()
	{
		return Singleton<SaveDataContainer>.Instance.Get().GetCurrentBomber();
	}

	public BomberUpgradeConfig GetNewBomber(SaveData sd)
	{
		BomberUpgradeConfig bomberUpgradeConfig = new BomberUpgradeConfig();
		BomberRequirements bomberRequirements = Singleton<GameFlow>.Instance.GetGameMode().GetBomberRequirements();
		bomberUpgradeConfig.BuildFrom(bomberRequirements, sd);
		return bomberUpgradeConfig;
	}

	public void UpgradeWithDefaults(BomberDefaults bd, int maxValue)
	{
		BomberUpgradeConfig currentConfig = GetCurrentConfig();
		int num = 0;
		BomberDefaults.RequirementDefault[] requirement = bd.m_requirement;
		foreach (BomberDefaults.RequirementDefault requirementDefault in requirement)
		{
			if (num < maxValue)
			{
				currentConfig.SetUpgrade(requirementDefault.m_requirementName, requirementDefault.m_equipment);
				num += requirementDefault.m_equipment.GetCost();
			}
		}
	}

	public void PatchUp(BomberUpgradeConfig buc, SaveData sd)
	{
		BomberRequirements bomberRequirements = Singleton<GameFlow>.Instance.GetGameMode().GetBomberRequirements();
		buc.PatchFrom(bomberRequirements, sd);
	}

	public BomberLivery GetLivery()
	{
		return m_livery;
	}

	public BomberRequirements GetRequirements()
	{
		return Singleton<GameFlow>.Instance.GetGameMode().GetBomberRequirements();
	}
}
