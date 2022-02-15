using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using BomberCrewCommon;
using UnityEngine;

public class AceFighterSpawner : MonoBehaviour
{
	[SerializeField]
	private MissionPlaceableObject m_placeableObject;

	private bool m_initialised;

	private bool m_willSpawn;

	private EnemyFighterAce m_selectedAce;

	private float m_chanceToSetOnEncounter;

	private void Start()
	{
		Init();
	}

	private void Init()
	{
		if (m_initialised)
		{
			return;
		}
		float num = (float)Convert.ToDouble(m_placeableObject.GetParameter("chanceOfAppear"), CultureInfo.InvariantCulture);
		float aceChanceCounter = Singleton<SaveDataContainer>.Instance.Get().GetAceChanceCounter();
		aceChanceCounter += num;
		string spawnTrigger = m_placeableObject.GetParameter("spawnTrigger");
		string parameter = m_placeableObject.GetParameter("fromDLC");
		float num2 = (m_chanceToSetOnEncounter = ((!(aceChanceCounter >= 1f)) ? aceChanceCounter : (aceChanceCounter - 1f)));
		if (aceChanceCounter >= 1f)
		{
			m_willSpawn = true;
			List<EnemyFighterAce> list = new List<EnemyFighterAce>();
			if (!Singleton<GameFlow>.Instance.GetGameMode().ShouldOverrideAceCatalogue())
			{
				if (!string.IsNullOrEmpty(parameter))
				{
					foreach (string currentInstalledDLC in Singleton<DLCManager>.Instance.GetCurrentInstalledDLCs())
					{
						DLCExpansionCampaign dLCExpansionCampaign = Singleton<DLCManager>.Instance.LoadAssetFromBundle<DLCExpansionCampaign>(currentInstalledDLC, "CAMPAIGN_" + currentInstalledDLC);
						if (dLCExpansionCampaign != null && dLCExpansionCampaign.GetInternalReference() == parameter && dLCExpansionCampaign.GetAceCatalogue() != null)
						{
							list.AddRange(dLCExpansionCampaign.GetAceCatalogue().GetEnemyFighterAces());
						}
					}
				}
				else
				{
					list.AddRange(Singleton<EnemyFighterAcesCatalogueLoader>.Instance.GetCatalogue().GetEnemyFighterAces());
					foreach (string currentInstalledDLC2 in Singleton<DLCManager>.Instance.GetCurrentInstalledDLCs())
					{
						DLCExpansionCampaign dLCExpansionCampaign2 = Singleton<DLCManager>.Instance.LoadAssetFromBundle<DLCExpansionCampaign>(currentInstalledDLC2, "CAMPAIGN_" + currentInstalledDLC2);
						if (dLCExpansionCampaign2 != null && dLCExpansionCampaign2.GetAceCatalogue() != null)
						{
							list.AddRange(dLCExpansionCampaign2.GetAceCatalogue().GetEnemyFighterAces());
						}
					}
				}
			}
			else
			{
				list.AddRange(Singleton<GameFlow>.Instance.GetGameMode().GetAces().GetEnemyFighterAces());
			}
			List<EnemyFighterAce> list2 = new List<EnemyFighterAce>();
			string parameter2 = m_placeableObject.GetParameter("forceId");
			int num3 = 0;
			if (!string.IsNullOrEmpty(parameter2))
			{
				num3 = Convert.ToInt32(parameter2);
			}
			int num4 = 0;
			bool flag = false;
			if (num3 == 0)
			{
				foreach (EnemyFighterAce item in list)
				{
					if (!Singleton<SaveDataContainer>.Instance.Get().IsAceDefeated(item.name))
					{
						list2.Add(item);
						if (!flag)
						{
							num4++;
						}
					}
					else
					{
						flag = true;
					}
				}
			}
			else
			{
				list2.Add(list[num3 - 1]);
			}
			int count = list2.Count;
			int num5 = list.Count - list2.Count;
			if (count > 0)
			{
				int index = ((!(UnityEngine.Random.Range(0f, 10f) < 7f)) ? 1 : 0);
				if (num5 == 0 || num5 == 1 || count == 1 || num4 == 7)
				{
					index = 0;
				}
				m_selectedAce = list2[index];
				if (string.IsNullOrEmpty(spawnTrigger))
				{
					DoSpawn();
				}
				else
				{
					MissionCoordinator instance = Singleton<MissionCoordinator>.Instance;
					instance.OnTrigger = (Action<string>)Delegate.Combine(instance.OnTrigger, (Action<string>)delegate(string tr)
					{
						if (tr == spawnTrigger)
						{
							DoSpawn();
						}
					});
				}
				StartCoroutine(WarnOfAcePilot());
			}
		}
		else
		{
			Singleton<SaveDataContainer>.Instance.Get().SetAceChanceCounter(m_chanceToSetOnEncounter);
		}
		m_initialised = true;
	}

	public void OnAceEncounter()
	{
		Singleton<SaveDataContainer>.Instance.Get().SetAceChanceCounter(m_chanceToSetOnEncounter);
	}

	private IEnumerator WarnOfAcePilot()
	{
		yield return new WaitForSeconds(45f);
		string aceEncounterText = "speech_mission_command_ace_encounter_likely";
		if (m_selectedAce.IsPluralAce())
		{
			aceEncounterText = "speech_mission_command_ace_encounter_likely_plural";
		}
		TopBarInfoQueue.TopBarRequest speech1 = TopBarInfoQueue.TopBarRequest.Speech(aceEncounterText, Singleton<LanguageProvider>.Instance.GetNamedTextImmediate(Singleton<TopBarInfoQueue>.Instance.GetCommandNamedText()), Singleton<TopBarInfoQueue>.Instance.GetCommandPortrait(), Singleton<TopBarInfoQueue>.Instance.GetCommandJabberText(), isGoodGuy: true);
		string text = string.Format(Singleton<LanguageProvider>.Instance.GetNamedTextImmediate("speech_mission_command_ace_encounter_likely"), m_selectedAce.GetFirstName(), m_selectedAce.GetSurname());
		speech1.SetStringLiteral(text);
		Singleton<TopBarInfoQueue>.Instance.RegisterRequest(speech1);
	}

	private void DoSpawn()
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(m_selectedAce.GetInMissionPrefab());
		gameObject.btransform().position = base.gameObject.btransform().position;
		gameObject.GetComponent<AceFighterInMission>().SetUp(m_selectedAce, this);
	}

	public bool GetWillSpawn()
	{
		Init();
		return m_willSpawn;
	}
}
