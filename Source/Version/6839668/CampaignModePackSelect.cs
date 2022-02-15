using System;
using BomberCrewCommon;
using Steamworks;
using UnityEngine;

public class CampaignModePackSelect : MonoBehaviour
{
	[SerializeField]
	private GameMode m_defaultGameMode;

	[SerializeField]
	private tk2dUIItem m_defaultVariantButton;

	[SerializeField]
	private tk2dUIItem m_dlcVariantButton;

	[SerializeField]
	private GameObject m_dlcNotInstalledHierarchy;

	[SerializeField]
	private string m_dlcToLookFor;

	[SerializeField]
	private UISelectFinder m_finder;

	[SerializeField]
	private tk2dUIItem m_cancelButton;

	[SerializeField]
	private GameObject m_dlcEnabledButton;

	[SerializeField]
	private GameObject m_dlcNotEnabledButton;

	[SerializeField]
	private int m_dlcId;

	[SerializeField]
	private string m_xboxProductId;

	[SerializeField]
	private string m_ps4ProductLabelSCEE;

	[SerializeField]
	private string m_ps4ProductLabelSCEA;

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
		if (Singleton<UISelector>.Instance.GetCurrentFinder() == m_finder)
		{
			Singleton<UISelector>.Instance.SetFinder(m_previousFinder);
			if (m_previouslyPointedAt != null)
			{
				Singleton<UISelector>.Instance.ForcePointAt(m_previouslyPointedAt);
			}
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
		m_dlcNotInstalledHierarchy.SetActive(!flag);
		m_dlcEnabledButton.SetActive(value: true);
		m_dlcNotEnabledButton.SetActive(value: false);
	}

	private void SelectDefault()
	{
		Singleton<GameFlow>.Instance.SetGameMode(m_defaultGameMode);
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
		Singleton<GameFlow>.Instance.SetGameMode(dLCExpansionCampaign.GetGameMode());
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
