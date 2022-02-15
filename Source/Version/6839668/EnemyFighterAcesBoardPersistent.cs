using System.Collections.Generic;
using BomberCrewCommon;
using UnityEngine;

public class EnemyFighterAcesBoardPersistent : MonoBehaviour
{
	[SerializeField]
	private GameObject m_fighterAceItemPrefab;

	[SerializeField]
	private LayoutGrid m_fighterAcesLayoutGrid;

	private List<EnemyFighterAcesBoardItem> m_aceFighterItems = new List<EnemyFighterAcesBoardItem>();

	private List<EnemyFighterAcesBoardItem> m_aceFighterItemsDLC = new List<EnemyFighterAcesBoardItem>();

	private int m_numDefeated;

	private void OnEnable()
	{
		SetupAceFightersBoard();
	}

	public int GetNumAcesTotal()
	{
		return m_aceFighterItems.Count + m_aceFighterItemsDLC.Count;
	}

	public void Refresh()
	{
		int num = 0;
		EnemyFighterAce[] enemyFighterAces = Singleton<EnemyFighterAcesCatalogueLoader>.Instance.GetCatalogue().GetEnemyFighterAces();
		bool flag = Singleton<GameFlow>.Instance.GetGameMode().ShouldOverrideAceCatalogue();
		if (flag)
		{
			enemyFighterAces = Singleton<GameFlow>.Instance.GetGameMode().GetAces().GetEnemyFighterAces();
		}
		int num2 = 2;
		foreach (EnemyFighterAcesBoardItem aceFighterItem in m_aceFighterItems)
		{
			aceFighterItem.SetUp(enemyFighterAces[num], num2 > 0);
			num2--;
			if (Singleton<SaveDataContainer>.Instance.Get().IsAceDefeated(enemyFighterAces[num].name))
			{
				aceFighterItem.SetDowned(downed: true);
				num2++;
			}
			else
			{
				aceFighterItem.SetDowned(downed: false);
			}
			num++;
		}
		if (flag)
		{
			return;
		}
		int num3 = 0;
		foreach (string currentInstalledDLC in Singleton<DLCManager>.Instance.GetCurrentInstalledDLCs())
		{
			DLCExpansionCampaign dLCExpansionCampaign = Singleton<DLCManager>.Instance.LoadAssetFromBundle<DLCExpansionCampaign>(currentInstalledDLC, "CAMPAIGN_" + currentInstalledDLC);
			if (!(dLCExpansionCampaign != null) || !(dLCExpansionCampaign.GetAceCatalogue() != null))
			{
				continue;
			}
			EnemyFighterAce[] enemyFighterAces2 = dLCExpansionCampaign.GetAceCatalogue().GetEnemyFighterAces();
			for (int i = 0; i < enemyFighterAces2.Length; i++)
			{
				if (Singleton<SaveDataContainer>.Instance.Get().IsAceDefeated(enemyFighterAces2[i].name))
				{
					m_aceFighterItemsDLC[num3].SetDowned(downed: true);
				}
				else
				{
					m_aceFighterItemsDLC[num3].SetDowned(downed: false);
				}
				num3++;
			}
		}
	}

	private void SetupAceFightersBoard()
	{
		foreach (EnemyFighterAcesBoardItem aceFighterItem in m_aceFighterItems)
		{
			Object.DestroyImmediate(aceFighterItem.gameObject);
		}
		foreach (EnemyFighterAcesBoardItem item in m_aceFighterItemsDLC)
		{
			Object.DestroyImmediate(item.gameObject);
		}
		m_aceFighterItems.Clear();
		m_aceFighterItemsDLC.Clear();
		EnemyFighterAce[] enemyFighterAces = Singleton<EnemyFighterAcesCatalogueLoader>.Instance.GetCatalogue().GetEnemyFighterAces();
		bool flag = Singleton<GameFlow>.Instance.GetGameMode().ShouldOverrideAceCatalogue();
		if (flag)
		{
			enemyFighterAces = Singleton<GameFlow>.Instance.GetGameMode().GetAces().GetEnemyFighterAces();
		}
		int num = 2;
		for (int i = 0; i < enemyFighterAces.Length; i++)
		{
			EnemyFighterAcesBoardItem component = Object.Instantiate(m_fighterAceItemPrefab).GetComponent<EnemyFighterAcesBoardItem>();
			component.transform.parent = m_fighterAcesLayoutGrid.transform;
			component.transform.rotation = m_fighterAcesLayoutGrid.transform.rotation;
			component.transform.localPosition = Vector3.zero;
			component.SetUp(enemyFighterAces[i], num > 0);
			m_aceFighterItems.Add(component);
			num--;
			if (Singleton<SaveDataContainer>.Instance.Get().IsAceDefeated(enemyFighterAces[i].name) && (Singleton<GameFlow>.Instance.GetCurrentMissionInfo().GetMissionLog() == null || Singleton<GameFlow>.Instance.GetCurrentMissionInfo().GetMissionLog().GetAceEncountered() != enemyFighterAces[i]))
			{
				component.SetDowned(downed: true);
				num++;
			}
			else
			{
				component.SetDowned(downed: false);
			}
		}
		if (!flag)
		{
			foreach (string currentInstalledDLC in Singleton<DLCManager>.Instance.GetCurrentInstalledDLCs())
			{
				DLCExpansionCampaign dLCExpansionCampaign = Singleton<DLCManager>.Instance.LoadAssetFromBundle<DLCExpansionCampaign>(currentInstalledDLC, "CAMPAIGN_" + currentInstalledDLC);
				if (!(dLCExpansionCampaign != null) || !(dLCExpansionCampaign.GetAceCatalogue() != null))
				{
					continue;
				}
				EnemyFighterAce[] enemyFighterAces2 = dLCExpansionCampaign.GetAceCatalogue().GetEnemyFighterAces();
				for (int j = 0; j < enemyFighterAces2.Length; j++)
				{
					EnemyFighterAcesBoardItem component2 = Object.Instantiate(m_fighterAceItemPrefab).GetComponent<EnemyFighterAcesBoardItem>();
					component2.transform.parent = m_fighterAcesLayoutGrid.transform;
					component2.transform.rotation = m_fighterAcesLayoutGrid.transform.rotation;
					component2.transform.localPosition = Vector3.zero;
					component2.SetUp(enemyFighterAces2[j], identityKnown: true);
					m_aceFighterItemsDLC.Add(component2);
					if (Singleton<SaveDataContainer>.Instance.Get().IsAceDefeated(enemyFighterAces2[j].name) && (Singleton<GameFlow>.Instance.GetCurrentMissionInfo().GetMissionLog() == null || Singleton<GameFlow>.Instance.GetCurrentMissionInfo().GetMissionLog().GetAceEncountered() != enemyFighterAces2[j]))
					{
						component2.SetDowned(downed: true);
						num++;
					}
					else
					{
						component2.SetDowned(downed: false);
					}
				}
			}
		}
		m_fighterAcesLayoutGrid.RepositionChildren();
	}
}
