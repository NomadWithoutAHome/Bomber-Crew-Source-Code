using System;
using BomberCrewCommon;
using UnityEngine;

public class EndlessBriefingScreen : MonoBehaviour
{
	[SerializeField]
	private tk2dUIItem m_toMissionButton;

	[SerializeField]
	private GameObject m_endlessModePackSelectPrefab;

	[SerializeField]
	private tk2dUIItem m_selectPackButton;

	[SerializeField]
	private AirbasePersistentCrew m_persistentCrew;

	[SerializeField]
	private AirbasePersistentCrewTarget m_persistentCrewTarget;

	[SerializeField]
	private EndlessBriefingBoardPersistent m_briefingBoard;

	[SerializeField]
	private GameObject m_packSelectHierarchy;

	private void Start()
	{
		m_toMissionButton.OnClick += GoToEndlessMission;
		m_selectPackButton.OnClick += SelectPack;
	}

	private void GoToEndlessMission()
	{
		EndlessModeVariant currentEndlessMode = Singleton<EndlessModeGameFlow>.Instance.GetCurrentEndlessMode();
		int currentCrewCount = Singleton<CrewContainer>.Instance.GetCurrentCrewCount();
		Crewman[] array = new Crewman[currentCrewCount];
		for (int i = 0; i < currentCrewCount; i++)
		{
			array[i] = Singleton<CrewContainer>.Instance.GetCrewman(i);
		}
		EndlessModeVariant.LoadoutJS loadout = new EndlessModeVariant.LoadoutJS(Singleton<BomberContainer>.Instance.GetCurrentConfig(), array, Singleton<BomberContainer>.Instance.GetCurrentConfig().GetName());
		Singleton<SaveDataContainer>.Instance.Get().SetLastSeenEndlessLoadout(currentEndlessMode.name, loadout);
		Singleton<EndlessModeGameFlow>.Instance.ClearEndlessModeScore();
		CampaignStructure.CampaignMission campaignMission = Singleton<EndlessModeGameFlow>.Instance.GetCurrentEndlessMode().GetCampaignMission();
		Singleton<GameFlow>.Instance.GoToMission(campaignMission);
	}

	private void SelectPack()
	{
		UIPopupData uIPopupData = new UIPopupData();
		uIPopupData.PopupDismissedCallback = (Action<UIPopUp>)Delegate.Combine(uIPopupData.PopupDismissedCallback, (Action<UIPopUp>)delegate
		{
			m_briefingBoard.RefreshBriefingContents();
		});
		GameMode curGameMode = Singleton<GameFlow>.Instance.GetGameMode();
		uIPopupData.PopupStartCallback = (Action<UIPopUp>)Delegate.Combine(uIPopupData.PopupStartCallback, (Action<UIPopUp>)delegate(UIPopUp uip)
		{
			uip.GetComponent<EndlessModePackSelect>().OnConfirm += delegate
			{
				if (Singleton<GameFlow>.Instance.GetGameMode() != curGameMode)
				{
					Singleton<GameFlow>.Instance.GoToAirbaseBasic();
				}
			};
		});
		Singleton<UIPopupManager>.Instance.DisplayPopup(m_endlessModePackSelectPrefab, uIPopupData);
	}

	private void OnEnable()
	{
		EndlessModeData endlessModeData = Singleton<SystemDataContainer>.Instance.Get().GetEndlessModeData();
		int highScoreForMode = endlessModeData.GetHighScoreForMode(Singleton<EndlessModeGameFlow>.Instance.GetCurrentEndlessMode().name);
		m_briefingBoard.UpdateHighscoreDisplay(highScoreForMode);
		m_briefingBoard.ShowBriefingContents();
		m_persistentCrew.DoTargetedWalk(m_persistentCrewTarget);
		Transform[] endTransforms = m_persistentCrewTarget.GetEndTransforms();
		for (int i = 0; i < Singleton<CrewContainer>.Instance.GetCurrentCrewCount(); i++)
		{
			GameObject crewmanAvatar = m_persistentCrew.GetCrewmanAvatar(i);
			AirbaseCrewmanAvatarBehaviour crab = crewmanAvatar.GetComponent<AirbaseCrewmanAvatarBehaviour>();
			crab.WalkTo(endTransforms[i].position, endTransforms[i], 1f, 25f, delegate
			{
				crab.SetSitting();
			}, 1f);
		}
	}
}
