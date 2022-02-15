using System;
using BomberCrewCommon;
using Steamworks;
using UnityEngine;

public class EndlessModePackSelect : MonoBehaviour
{
	[SerializeField]
	private EndlessModeVariant m_defaultVariant;

	[SerializeField]
	private GameMode m_defaultGameMode;

	[SerializeField]
	private tk2dUIItem m_defaultVariantButton;

	[SerializeField]
	private tk2dUIItem m_dlcVariantButton;

	[SerializeField]
	private GameObject m_dlcNotInstalledHierarchy;

	[SerializeField]
	private tk2dUIItem m_dlc2VariantButton;

	[SerializeField]
	private GameObject m_dlc2NotInstalledHierarchy;

	[SerializeField]
	private string m_dlcToLookFor;

	[SerializeField]
	private string m_dlcToLookFor2;

	[SerializeField]
	private UISelectFinder m_finder;

	[SerializeField]
	private tk2dUIItem m_cancelButton;

	[SerializeField]
	private GameObject m_dlcEnabledButton;

	[SerializeField]
	private GameObject m_dlcNotEnabledButton;

	[SerializeField]
	private GameObject m_dlc2EnabledButton;

	[SerializeField]
	private GameObject m_dlc2NotEnabledButton;

	[SerializeField]
	private int m_dlcId;

	[SerializeField]
	private int m_dlc2Id;

	[SerializeField]
	private UISelectorPointingHint m_topButtonPh;

	[SerializeField]
	private UISelectorPointingHint m_secondButtonPh;

	[SerializeField]
	private UISelectorPointingHint m_thirdButtonPh;

	[SerializeField]
	private UISelectorPointingHint m_cancelButtonPh;

	[SerializeField]
	private string m_xboxProductId;

	[SerializeField]
	private string m_ps4ProductLabelSCEE;

	[SerializeField]
	private string m_ps4ProductLabelSCEA;

	[SerializeField]
	private string m_ps4ProductLabelSCEE2;

	[SerializeField]
	private string m_ps4ProductLabelSCEA2;

	private UISelectFinder m_previousFinder;

	private tk2dUIItem m_previouslyPointedAt;

	public event Action OnConfirm;

	private void OnEnable()
	{
		Singleton<MainActionButtonMonitor>.Instance.AddListener(StartButtons);
		m_previousFinder = Singleton<UISelector>.Instance.GetCurrentFinder();
		m_previouslyPointedAt = Singleton<UISelector>.Instance.GetCurrentMovementType().GetCurrentlyPointedAtItem();
		Singleton<UISelector>.Instance.SetFinder(m_finder);
		m_cancelButton.OnClick += Close;
	}

	private void OnDisable()
	{
		Singleton<MainActionButtonMonitor>.Instance.RemoveListener(StartButtons, invalidateCurrentPress: true);
		Singleton<UISelector>.Instance.SetFinder(m_previousFinder);
		if (m_previouslyPointedAt != null)
		{
			Singleton<UISelector>.Instance.ForcePointAt(m_previouslyPointedAt);
		}
		m_cancelButton.OnClick -= Close;
	}

	private bool StartButtons(MainActionButtonMonitor.ButtonPress bp)
	{
		return true;
	}

	private void Awake()
	{
		m_defaultVariantButton.OnClick += SelectDefault;
		m_dlcVariantButton.OnClick += SelectDLC;
		m_dlc2VariantButton.OnClick += SelectDLC2;
		Refresh();
	}

	private void Refresh()
	{
		bool flag = false;
		foreach (string currentInstalledDLC in Singleton<DLCManager>.Instance.GetCurrentInstalledDLCs())
		{
			if (currentInstalledDLC == m_dlcToLookFor)
			{
				flag = true;
			}
		}
		bool flag2 = false;
		foreach (string currentInstalledDLC2 in Singleton<DLCManager>.Instance.GetCurrentInstalledDLCs())
		{
			if (currentInstalledDLC2 == m_dlcToLookFor2)
			{
				flag2 = true;
			}
		}
		m_dlcNotInstalledHierarchy.SetActive(!flag);
		m_dlc2NotInstalledHierarchy.SetActive(!flag2);
		m_dlcEnabledButton.SetActive(value: true);
		m_dlcNotEnabledButton.SetActive(value: false);
		m_dlc2EnabledButton.SetActive(value: true);
		m_dlc2NotEnabledButton.SetActive(value: false);
	}

	private void SelectDefault()
	{
		Singleton<GameFlow>.Instance.SetGameMode(m_defaultGameMode);
		if (Singleton<SaveDataContainer>.Instance.Exists(0))
		{
			Singleton<SaveDataContainer>.Instance.Load(0);
		}
		else
		{
			Singleton<SaveDataContainer>.Instance.New(0, isCampaign: true, skippedTutorial: true);
		}
		Singleton<EndlessModeGameFlow>.Instance.SetEndlessModeVariant(m_defaultVariant);
		Singleton<EndlessModeGameFlow>.Instance.ResetEndlessMode();
		if (this.OnConfirm != null)
		{
			this.OnConfirm();
		}
		Singleton<UIPopupManager>.Instance.DismissCurrentPopup();
	}

	private void SelectDLC()
	{
		DLCExpansionCampaign dLCExpansionCampaign = Singleton<DLCManager>.Instance.LoadAssetFromBundle<DLCExpansionCampaign>(m_dlcToLookFor, "CAMPAIGN_" + m_dlcToLookFor);
		if (dLCExpansionCampaign == null)
		{
			if (SteamManager.Initialized)
			{
				SteamFriends.ActivateGameOverlayToStore(new AppId_t((uint)m_dlcId), EOverlayToStoreFlag.k_EOverlayToStoreFlag_None);
			}
			return;
		}
		Singleton<GameFlow>.Instance.SetGameMode(m_defaultGameMode);
		if (Singleton<SaveDataContainer>.Instance.Exists(0))
		{
			Singleton<SaveDataContainer>.Instance.Load(0);
		}
		else
		{
			Singleton<SaveDataContainer>.Instance.New(0, isCampaign: true, skippedTutorial: true);
		}
		Singleton<EndlessModeGameFlow>.Instance.SetEndlessModeVariant(dLCExpansionCampaign.GetEndlessModeVariant());
		Singleton<EndlessModeGameFlow>.Instance.ResetEndlessMode();
		if (this.OnConfirm != null)
		{
			this.OnConfirm();
		}
		Singleton<UIPopupManager>.Instance.DismissCurrentPopup();
	}

	private void SelectDLC2()
	{
		DLCExpansionCampaign dLCExpansionCampaign = Singleton<DLCManager>.Instance.LoadAssetFromBundle<DLCExpansionCampaign>(m_dlcToLookFor2, "CAMPAIGN_" + m_dlcToLookFor2);
		if (dLCExpansionCampaign == null)
		{
			if (SteamManager.Initialized)
			{
				SteamFriends.ActivateGameOverlayToStore(new AppId_t((uint)m_dlc2Id), EOverlayToStoreFlag.k_EOverlayToStoreFlag_None);
			}
			return;
		}
		Singleton<GameFlow>.Instance.SetGameMode(dLCExpansionCampaign.GetEndlessGameMode());
		if (Singleton<SaveDataContainer>.Instance.Exists(0))
		{
			Singleton<SaveDataContainer>.Instance.Load(0);
		}
		else
		{
			Singleton<SaveDataContainer>.Instance.New(0, isCampaign: true, skippedTutorial: true);
		}
		Singleton<EndlessModeGameFlow>.Instance.SetEndlessModeVariant(dLCExpansionCampaign.GetEndlessModeVariant());
		Singleton<EndlessModeGameFlow>.Instance.ResetEndlessMode();
		if (this.OnConfirm != null)
		{
			this.OnConfirm();
		}
		Singleton<UIPopupManager>.Instance.DismissCurrentPopup();
	}

	private void Close()
	{
		Singleton<UIPopupManager>.Instance.DismissCurrentPopup();
	}
}
