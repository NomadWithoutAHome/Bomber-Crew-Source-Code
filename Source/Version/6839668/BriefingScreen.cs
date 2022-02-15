using System;
using System.Collections.Generic;
using BomberCrewCommon;
using UnityEngine;

public class BriefingScreen : MonoBehaviour
{
	[SerializeField]
	private tk2dUIItem m_toMissionButton;

	[SerializeField]
	private tk2dUIItem m_toMissionAllDisplayButton;

	[SerializeField]
	private tk2dUIItem m_toEnemyFighterAcesBoardButton;

	[SerializeField]
	private AirbaseAreaScreen m_enemyAcesBoard;

	[SerializeField]
	private GameObject[] m_canGoToMissionHierarchy;

	[SerializeField]
	private GameObject m_cannotGoToMissionHierarchy;

	[SerializeField]
	private TextSetter m_missionDescriptionTitleText;

	[SerializeField]
	private Renderer m_missionTargetPhotoRenderer;

	[SerializeField]
	private GameObject m_missionMapMarkerPrefab;

	[SerializeField]
	private Transform m_missionMapMarkerHierarchy;

	[SerializeField]
	private int m_maxMissions = 3;

	[SerializeField]
	private GameObject[] m_missionSelectedHierarchies;

	[SerializeField]
	private GameObject[] m_missionNotSelectedHierarchies;

	[SerializeField]
	private CampaignMap m_campaignMap;

	[SerializeField]
	private TextSetter m_moneyReward;

	[SerializeField]
	private TextSetter m_intelReward;

	[SerializeField]
	private TextSetter m_moneyRewardSR;

	[SerializeField]
	private TextSetter m_intelRewardSR;

	[SerializeField]
	private AirbasePersistentCrew m_persistentCrew;

	[SerializeField]
	private AirbasePersistentCrewTarget m_persistentCrewTarget;

	[SerializeField]
	private PerkDisplay m_missionPerk;

	[SerializeField]
	private GameObject m_perkPrefab;

	[SerializeField]
	private LayoutGrid m_perkLayoutGrid;

	[SerializeField]
	private CampaignStructure.CampaignMission m_demoMission;

	[SerializeField]
	private TextSetter m_missionDurationText;

	[SerializeField]
	private TextSetter m_missionRiskText;

	[SerializeField]
	[NamedText]
	private string[] m_missionDurationStrings;

	[SerializeField]
	[NamedText]
	private string[] m_missionRiskStrings;

	[SerializeField]
	private GameObject m_criticalMissionHierarchy;

	[SerializeField]
	private GameObject m_enemyAceInMissionAreaHierarchy;

	[SerializeField]
	private GameObject m_isTutorialMissionHierarchy;

	[SerializeField]
	private Color[] m_durationColors;

	[SerializeField]
	private Color[] m_riskColors;

	[SerializeField]
	private int[] m_durationTimers;

	[SerializeField]
	private UISelectFinder m_missionsFinder;

	[SerializeField]
	private UISelectFinder m_selectOrBackFinder;

	[SerializeField]
	private UISelectFinder m_noneFinder;

	[SerializeField]
	private LayoutGrid m_extraContentLayoutGrid;

	[SerializeField]
	private GameObject m_extraContentTabButton;

	[SerializeField]
	private GameObject m_extraContentTabButtonMain;

	[SerializeField]
	private GameObject m_extraContentTabControlPrompt;

	[SerializeField]
	private GameObject m_confirmYesNoPopup;

	[SerializeField]
	private EnemyFighterAcesBoardPersistent m_enemyAcesBaord;

	[SerializeField]
	private CampaignProgressIndicator m_progressIndicator;

	private List<SelectableFilterButton> m_missionMapMarkerButtons = new List<SelectableFilterButton>();

	private List<GameObject> m_createdPerks = new List<GameObject>();

	private CampaignStructure.CampaignMission m_currentlySelectedMission;

	private TopBarInfoQueue.TopBarRequest m_currentTopBarRequest;

	private int m_seed;

	private List<DLCExpansionCampaign> m_additionalCampaigns = new List<DLCExpansionCampaign>();

	private DLCExpansionCampaign m_currentlySelectedCampaign;

	private List<SelectableFilterButton> m_allFilterButtons = new List<SelectableFilterButton>();

	private SelectableFilterButton m_mainFilterButton;

	private int m_debugMissionPage;

	private void GoToMissionDebug(int offset)
	{
		CampaignStructure campaignStructure = ((!(m_currentlySelectedCampaign != null)) ? Singleton<GameFlow>.Instance.GetCampaign() : m_currentlySelectedCampaign.GetCampaign());
		CampaignStructure.CampaignMission[] allMissions = campaignStructure.GetAllMissions();
		int num = m_debugMissionPage * 10;
		int num2 = (offset + num) % allMissions.Length;
		bool flag = Singleton<CrewContainer>.Instance.GetCurrentCrewCount() == Singleton<GameFlow>.Instance.GetGameMode().GetMaxCrew();
		SelectMission(allMissions[num2]);
	}

	private void Awake()
	{
		m_toMissionButton.OnClick += ClickToMission;
		m_toMissionAllDisplayButton.OnClick += ClickBackToAll;
		m_toEnemyFighterAcesBoardButton.OnClick += ShowEnemyFighterAcesBoard;
		m_seed = UnityEngine.Random.Range(0, 99999);
		AirbaseAreaScreen component = GetComponent<AirbaseAreaScreen>();
		component.OnAcceptButton += OnAcceptButton;
		component.OnBackButton += OnBackButton;
		if (Singleton<GameFlow>.Instance.GetGameMode().UseAddOnCampaignsTab())
		{
			foreach (string currentInstalledDLC in Singleton<DLCManager>.Instance.GetCurrentInstalledDLCs())
			{
				DLCExpansionCampaign dLCExpansionCampaign = Singleton<DLCManager>.Instance.LoadAssetFromBundle<DLCExpansionCampaign>(currentInstalledDLC, "CAMPAIGN_" + currentInstalledDLC);
				if (dLCExpansionCampaign != null && dLCExpansionCampaign.GetCampaign() != null)
				{
					m_additionalCampaigns.Add(dLCExpansionCampaign);
				}
			}
		}
		if (m_additionalCampaigns.Count > 0)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(m_extraContentTabButtonMain);
			gameObject.transform.parent = m_extraContentLayoutGrid.transform;
			SelectableFilterButton sfbMain = gameObject.GetComponent<SelectableFilterButton>();
			sfbMain.SetSelected(selected: true);
			sfbMain.SetText(Singleton<LanguageProvider>.Instance.GetNamedTextImmediate("campaign_name_base"));
			sfbMain.OnClick += delegate
			{
				sfbMain.SetSelected(selected: true);
				foreach (SelectableFilterButton allFilterButton in m_allFilterButtons)
				{
					allFilterButton.SetSelected(selected: false);
				}
				SetCampaign(null);
			};
			m_mainFilterButton = sfbMain;
			foreach (DLCExpansionCampaign dec in m_additionalCampaigns)
			{
				GameObject gameObject2 = UnityEngine.Object.Instantiate(m_extraContentTabButton);
				gameObject2.transform.parent = m_extraContentLayoutGrid.transform;
				SelectableFilterButton sfbThis = gameObject2.GetComponent<SelectableFilterButton>();
				m_allFilterButtons.Add(sfbThis);
				sfbThis.SetText(Singleton<LanguageProvider>.Instance.GetNamedTextImmediate(dec.GetNamedTextReference()));
				sfbThis.OnClick += delegate
				{
					sfbMain.SetSelected(selected: false);
					foreach (SelectableFilterButton allFilterButton2 in m_allFilterButtons)
					{
						allFilterButton2.SetSelected(allFilterButton2 == sfbThis);
					}
					SetCampaign(dec);
				};
			}
			m_extraContentLayoutGrid.RepositionChildren();
			m_extraContentTabControlPrompt.SetActive(value: true);
		}
		else
		{
			m_extraContentTabControlPrompt.SetActive(value: false);
		}
	}

	private void SetCampaign(DLCExpansionCampaign expansionCampaign)
	{
		m_currentlySelectedCampaign = expansionCampaign;
		Singleton<SaveDataContainer>.Instance.Get().SetLastDLCCampaignSelected((!(expansionCampaign == null)) ? expansionCampaign.GetInternalReference() : null);
		SelectMission(null);
		UpdateMissionNames();
	}

	public bool IsShowingMission()
	{
		return m_currentlySelectedMission != null;
	}

	private void OnDisable()
	{
		if (m_currentTopBarRequest != null)
		{
			Singleton<TopBarInfoQueue>.Instance.RemoveRequest(m_currentTopBarRequest);
			m_currentTopBarRequest = null;
		}
		Singleton<MainActionButtonMonitor>.Instance.RemoveListener(MainActionButtonPress, invalidateCurrentPress: false);
	}

	private void UpdateMissionNames()
	{
		CampaignStructure campaignStructure = ((!(m_currentlySelectedCampaign != null)) ? Singleton<GameFlow>.Instance.GetCampaign() : m_currentlySelectedCampaign.GetCampaign());
		CampaignStructure.CampaignMission[] allMissions = campaignStructure.GetAllMissions();
		int num = allMissions.Length / 10;
		if (m_debugMissionPage < 0)
		{
			m_debugMissionPage = num;
		}
		if (m_debugMissionPage > num)
		{
			m_debugMissionPage = 0;
		}
		int num2 = m_debugMissionPage * 10;
		int num3 = 0;
	}

	private void OnEnable()
	{
		UpdateMissionNames();
		Singleton<MainActionButtonMonitor>.Instance.AddListener(MainActionButtonPress);
		string lastDLCCampaignSelected = Singleton<SaveDataContainer>.Instance.Get().GetLastDLCCampaignSelected();
		if (lastDLCCampaignSelected == null)
		{
			SetCampaign(null);
		}
		else
		{
			int num = 0;
			foreach (DLCExpansionCampaign additionalCampaign in m_additionalCampaigns)
			{
				if (additionalCampaign.GetInternalReference() == lastDLCCampaignSelected)
				{
					m_mainFilterButton.SetSelected(selected: false);
					for (int i = 0; i < m_allFilterButtons.Count; i++)
					{
						m_allFilterButtons[num].SetSelected(i == num);
					}
					SetCampaign(additionalCampaign);
					break;
				}
				num++;
			}
		}
		SelectMission(null);
		bool flag = Singleton<CrewContainer>.Instance.GetCurrentCrewCount() == Singleton<GameFlow>.Instance.GetGameMode().GetMaxCrew();
		GameObject[] canGoToMissionHierarchy = m_canGoToMissionHierarchy;
		foreach (GameObject gameObject in canGoToMissionHierarchy)
		{
			gameObject.SetActive(flag);
		}
		m_cannotGoToMissionHierarchy.SetActive(!flag);
		if (!flag)
		{
			Singleton<UISelector>.Instance.SetFinder(m_noneFinder);
		}
		m_persistentCrew.DoTargetedWalk(m_persistentCrewTarget);
		Transform[] endTransforms = m_persistentCrewTarget.GetEndTransforms();
		for (int k = 0; k < Singleton<CrewContainer>.Instance.GetCurrentCrewCount(); k++)
		{
			GameObject crewmanAvatar = m_persistentCrew.GetCrewmanAvatar(k);
			AirbaseCrewmanAvatarBehaviour crab = crewmanAvatar.GetComponent<AirbaseCrewmanAvatarBehaviour>();
			crab.WalkTo(endTransforms[k].position, endTransforms[k], 1f, 25f, delegate
			{
				crab.SetSitting();
			}, 1f);
		}
		Singleton<AirbaseNavigation>.Instance.RefreshCrewPhoto();
		CreatePerks();
	}

	private bool MainActionButtonPress(MainActionButtonMonitor.ButtonPress bp)
	{
		if (bp == MainActionButtonMonitor.ButtonPress.TopAction || bp == MainActionButtonMonitor.ButtonPress.TopActionDown)
		{
			if (m_allFilterButtons.Count > 0)
			{
				if (m_mainFilterButton.IsSelected())
				{
					if (bp == MainActionButtonMonitor.ButtonPress.TopActionDown)
					{
						m_allFilterButtons[0].GetUIItem().SimulateDown();
					}
					else
					{
						m_allFilterButtons[0].GetUIItem().SimulateUpClick();
					}
				}
				else
				{
					int num = 0;
					int num2 = 0;
					foreach (SelectableFilterButton allFilterButton in m_allFilterButtons)
					{
						if (allFilterButton.IsSelected())
						{
							num = num2;
						}
						num2++;
					}
					int num3 = num + 1;
					if (num3 == m_allFilterButtons.Count)
					{
						if (bp == MainActionButtonMonitor.ButtonPress.TopActionDown)
						{
							m_mainFilterButton.GetUIItem().SimulateDown();
						}
						else
						{
							m_mainFilterButton.GetUIItem().SimulateUpClick();
						}
					}
					else if (bp == MainActionButtonMonitor.ButtonPress.TopActionDown)
					{
						m_allFilterButtons[num3 % m_allFilterButtons.Count].GetUIItem().SimulateDown();
					}
					else
					{
						m_allFilterButtons[num3 % m_allFilterButtons.Count].GetUIItem().SimulateUpClick();
					}
				}
			}
			return true;
		}
		return false;
	}

	private void CreatePerks()
	{
		foreach (GameObject createdPerk in m_createdPerks)
		{
			UnityEngine.Object.DestroyImmediate(createdPerk);
		}
		m_createdPerks.Clear();
		List<SaveData.ActivePerk> currentPerks = Singleton<SaveDataContainer>.Instance.Get().GetCurrentPerks();
		foreach (SaveData.ActivePerk item in currentPerks)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(m_perkPrefab);
			gameObject.transform.parent = m_perkLayoutGrid.transform;
			gameObject.GetComponent<PerkDisplay>().SetUp(item.m_perkType, item.m_numMissionRemaining, item.m_reduceAmountFactor);
			m_createdPerks.Add(gameObject);
		}
		m_perkLayoutGrid.RepositionChildren();
	}

	private void ClickBackToAll()
	{
		SelectMission(null);
	}

	private void SpawnMissionButtons()
	{
		m_campaignMap.Clear();
		m_missionMapMarkerButtons.Clear();
		List<CampaignStructure.CampaignMission> list = ((!(m_currentlySelectedCampaign == null)) ? m_currentlySelectedCampaign.GetCampaign().GetCurrentAvailableMissions(m_maxMissions, m_seed) : Singleton<GameFlow>.Instance.GetCampaign().GetCurrentAvailableMissions(m_maxMissions, m_seed));
		if (Singleton<GameFlow>.Instance.IsDemoMode())
		{
			list.Clear();
			list.Add(m_demoMission);
		}
		foreach (CampaignStructure.CampaignMission cm in list)
		{
			bool isComplete = Singleton<SaveDataContainer>.Instance.Get().HasCompletedMission(cm.m_missionReferenceName);
			GameObject gameObject = m_campaignMap.SetMissionTargetMarker(cm.GetMissionTargetArea(), interactive: true, null, cm.GetRiskBalanced(), cm.m_isKeyMission && cm.m_showKeyMissionTitle, cm.m_orderingNumber, isComplete);
			BriefingMissionButton component = gameObject.GetComponent<BriefingMissionButton>();
			if (component != null)
			{
				component.SetUp(cm, isComplete, cm.m_orderingNumber);
			}
			SelectableFilterButton component2 = gameObject.GetComponent<SelectableFilterButton>();
			component2.OnClick += delegate
			{
				SelectMission(cm);
				Singleton<TopBarInfoQueue>.Instance.RemoveRequest(Singleton<TopBarInfoQueue>.Instance.GetCurrentlyShownRequest());
			};
			component2.SetRelatedObject(cm);
			m_missionMapMarkerButtons.Add(component2);
		}
		m_missionMapMarkerHierarchy.GetComponent<LayoutGrid>().RepositionChildren();
	}

	private void SpawnMissionInfo()
	{
		m_campaignMap.Clear();
		m_missionMapMarkerButtons.Clear();
		m_campaignMap.SetMissionTargetMarker(m_currentlySelectedMission.GetMissionTargetArea(), interactive: false, m_currentlySelectedMission.GetMissionPath(), m_currentlySelectedMission.GetRiskBalanced(), m_currentlySelectedMission.m_isKeyMission && m_currentlySelectedMission.m_showKeyMissionTitle, 0, isComplete: false);
	}

	private void SelectMission(CampaignStructure.CampaignMission cm)
	{
		CampaignStructure.CampaignMission currentlySelectedMission = m_currentlySelectedMission;
		m_currentlySelectedMission = cm;
		if (cm != null)
		{
			Singleton<UISelector>.Instance.SetFinder(m_selectOrBackFinder);
			SpawnMissionInfo();
			m_missionDescriptionTitleText.SetTextFromLanguageString(cm.m_titleNamedText);
			m_moneyReward.SetText($"{Mathf.RoundToInt((float)cm.m_totalMoneyReward * 0.6f):N0}");
			m_intelReward.SetText($"{Mathf.RoundToInt((float)cm.m_totalIntelReward * 0.6f)}");
			m_moneyRewardSR.SetText($"{Mathf.RoundToInt((float)cm.m_totalMoneyReward * 0.4f):N0}");
			m_intelRewardSR.SetText($"{Mathf.RoundToInt((float)cm.m_totalIntelReward * 0.4f)}");
			m_criticalMissionHierarchy.SetActive(cm.m_isKeyMission && cm.m_showKeyMissionTitle);
			m_isTutorialMissionHierarchy.SetActive(cm.m_isTrainingMission);
			float aceChance = cm.GetAceChance();
			float num = Singleton<SaveDataContainer>.Instance.Get().GetAceChanceCounter() + aceChance;
			m_enemyAceInMissionAreaHierarchy.SetActive(num >= 1f && Singleton<SaveDataContainer>.Instance.Get().GetNumDefeatedAces() < m_enemyAcesBaord.GetNumAcesTotal());
			if (cm.GetAceIsForced())
			{
				m_enemyAceInMissionAreaHierarchy.SetActive(value: true);
			}
			m_missionTargetPhotoRenderer.material.mainTexture = cm.m_targetPhotoTexture;
			float estimatedTimeSeconds = CampaignStructure.GetEstimatedTimeSeconds(cm.m_level);
			int num2 = 0;
			int[] durationTimers = m_durationTimers;
			foreach (int num3 in durationTimers)
			{
				if (estimatedTimeSeconds >= (float)num3)
				{
					num2++;
				}
			}
			m_missionDurationText.SetTextFromLanguageString(m_missionDurationStrings[Mathf.Min(num2, m_missionDurationStrings.Length - 1)]);
			m_missionDurationText.SetColor(m_durationColors[Mathf.Min(num2, m_durationColors.Length - 1)]);
			int riskBalanced = cm.GetRiskBalanced();
			m_missionRiskText.SetTextFromLanguageString(m_missionRiskStrings[Mathf.Min(riskBalanced, m_missionRiskStrings.Length - 1)]);
			m_missionRiskText.SetColor(m_riskColors[Mathf.Min(riskBalanced, m_riskColors.Length - 1)]);
			if (cm.m_campaignPerk != null)
			{
				m_missionPerk.gameObject.SetActive(value: true);
				m_missionPerk.SetUp(cm.m_campaignPerk.GetPerk(), cm.m_campaignPerk.GetMissions(), cm.m_campaignPerk.GetFactor());
			}
			else
			{
				m_missionPerk.gameObject.SetActive(value: false);
			}
			GameObject[] missionSelectedHierarchies = m_missionSelectedHierarchies;
			foreach (GameObject gameObject in missionSelectedHierarchies)
			{
				gameObject.SetActive(value: true);
			}
			GameObject[] missionNotSelectedHierarchies = m_missionNotSelectedHierarchies;
			foreach (GameObject gameObject2 in missionNotSelectedHierarchies)
			{
				gameObject2.SetActive(value: false);
			}
			if (m_currentTopBarRequest != null)
			{
				Singleton<TopBarInfoQueue>.Instance.RemoveRequest(m_currentTopBarRequest);
				m_currentTopBarRequest = null;
			}
			m_currentTopBarRequest = TopBarInfoQueue.TopBarRequest.Briefing(cm.m_descriptionNamedText, Singleton<LanguageProvider>.Instance.GetNamedTextImmediate(Singleton<TopBarInfoQueue>.Instance.GetCommandNamedText()), Singleton<TopBarInfoQueue>.Instance.GetCommandPortrait(), Singleton<TopBarInfoQueue>.Instance.GetCommandJabberText(), isGoodGuy: true);
			m_currentTopBarRequest.m_expiryTime = 0f;
			Singleton<TopBarInfoQueue>.Instance.RegisterRequest(m_currentTopBarRequest);
			return;
		}
		if (Singleton<UISelector>.Instance.GetCurrentFinder() == m_selectOrBackFinder)
		{
			Singleton<UISelector>.Instance.SetFinder(m_missionsFinder);
		}
		if (m_currentTopBarRequest != null)
		{
			Singleton<TopBarInfoQueue>.Instance.RemoveRequest(m_currentTopBarRequest);
			m_currentTopBarRequest = null;
		}
		if (Singleton<CrewContainer>.Instance.GetCurrentCrewCount() == Singleton<GameFlow>.Instance.GetGameMode().GetMaxCrew())
		{
			SpawnMissionButtons();
		}
		else
		{
			m_campaignMap.Clear();
		}
		GameObject[] missionSelectedHierarchies2 = m_missionSelectedHierarchies;
		foreach (GameObject gameObject3 in missionSelectedHierarchies2)
		{
			gameObject3.SetActive(value: false);
		}
		GameObject[] missionNotSelectedHierarchies2 = m_missionNotSelectedHierarchies;
		foreach (GameObject gameObject4 in missionNotSelectedHierarchies2)
		{
			gameObject4.SetActive(value: true);
		}
		foreach (SelectableFilterButton missionMapMarkerButton in m_missionMapMarkerButtons)
		{
			if (missionMapMarkerButton.GetRelatedObject() == currentlySelectedMission)
			{
				Singleton<UISelector>.Instance.ForcePointAt(missionMapMarkerButton.GetUIItem());
			}
		}
	}

	private void ClickToMission()
	{
		if (m_currentlySelectedMission == null)
		{
			return;
		}
		if (m_currentlySelectedCampaign != null)
		{
			if (m_currentlySelectedCampaign.GetRecommendedIntel() <= Singleton<SaveDataContainer>.Instance.Get().GetIntel())
			{
				Singleton<GameFlow>.Instance.GoToMission(m_currentlySelectedMission);
				return;
			}
			UIPopupData uIPopupData = new UIPopupData();
			uIPopupData.PopupStartCallback = (Action<UIPopUp>)Delegate.Combine(uIPopupData.PopupStartCallback, (Action<UIPopUp>)delegate(UIPopUp uip)
			{
				string namedTextImmediate = Singleton<LanguageProvider>.Instance.GetNamedTextImmediate("ui_briefing_dlc_difficulty_warning_title");
				uip.GetComponent<UIPopUpConfirm>().SetUp(Singleton<LanguageProvider>.Instance.GetNamedTextImmediate("ui_briefing_dlc_difficulty_warning_title"), Singleton<LanguageProvider>.Instance.GetNamedTextImmediate("ui_briefing_dlc_difficulty_warning"), Singleton<LanguageProvider>.Instance.GetNamedTextImmediate("ui_briefing_dlc_difficulty_warning_yes"), Singleton<LanguageProvider>.Instance.GetNamedTextImmediate("ui_briefing_dlc_difficulty_warning_no"));
				uip.GetComponent<UIPopUpConfirm>().OnConfirm += delegate
				{
					Singleton<GameFlow>.Instance.GoToMission(m_currentlySelectedMission);
				};
			});
			Singleton<UIPopupManager>.Instance.DisplayPopup(m_confirmYesNoPopup, uIPopupData);
		}
		else
		{
			Singleton<GameFlow>.Instance.GoToMission(m_currentlySelectedMission);
		}
	}

	private void ShowEnemyFighterAcesBoard()
	{
		Singleton<AirbaseNavigation>.Instance.SetSelectingArea(m_enemyAcesBoard);
	}

	private void OnAcceptButton(bool down)
	{
	}

	private void OnBackButton()
	{
		if (Singleton<UISelector>.Instance.GetCurrentFinder() == m_selectOrBackFinder)
		{
			SelectMission(null);
		}
	}

	private void Update()
	{
	}
}
