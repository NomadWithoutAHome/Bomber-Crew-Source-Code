using BomberCrewCommon;
using Steamworks;
using UnityEngine;

public class DLCShowDetailsPopup : MonoBehaviour
{
	[SerializeField]
	private GameObject m_installedHierarchy;

	[SerializeField]
	private GameObject m_notInstalledHierarchy;

	[SerializeField]
	private tk2dUIItem m_closeButton;

	[SerializeField]
	private tk2dUIItem m_closeButtonInstaleld;

	[SerializeField]
	private tk2dUIItem m_toStoreButton;

	[SerializeField]
	private Renderer m_mainSprite;

	[SerializeField]
	private TextSetter m_titleText;

	[SerializeField]
	private UISelectFinder m_selectFinder;

	private DLCShowArea.ExpectedDLC m_dlc;

	private UISelectFinder m_previousFinder;

	public void SetUp(DLCShowArea.ExpectedDLC forDLC)
	{
		m_dlc = forDLC;
		m_mainSprite.sharedMaterial = Object.Instantiate(m_mainSprite.sharedMaterial);
		m_mainSprite.sharedMaterial.mainTexture = forDLC.m_dlcLargeTexture;
		m_titleText.SetTextFromLanguageString(forDLC.m_dlcNamedText);
		bool flag = Singleton<DLCManager>.Instance.GetCurrentInstalledDLCs().Contains(forDLC.m_dlcPackName);
		m_installedHierarchy.SetActive(flag);
		m_notInstalledHierarchy.SetActive(!flag);
		m_closeButton.OnClick += Close;
		m_closeButtonInstaleld.OnClick += Close;
		m_toStoreButton.OnClick += ToStore;
	}

	private void OnEnable()
	{
		if (m_previousFinder == null)
		{
			m_previousFinder = Singleton<UISelector>.Instance.GetCurrentFinder();
		}
		Singleton<UISelector>.Instance.SetFinder(m_selectFinder);
	}

	private void Close()
	{
		Singleton<UISelector>.Instance.SetFinder(m_previousFinder);
		Singleton<UIPopupManager>.Instance.DismissCurrentPopup();
	}

	private void ToStore()
	{
		if (SteamManager.Initialized)
		{
			SteamFriends.ActivateGameOverlayToStore(new AppId_t(m_dlc.m_appId), EOverlayToStoreFlag.k_EOverlayToStoreFlag_None);
		}
	}
}
