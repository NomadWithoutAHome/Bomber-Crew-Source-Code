using BomberCrewCommon;
using UnityEngine;

public class CampaignMenuScreen : MonoBehaviour
{
	[SerializeField]
	private tk2dUIItem m_startNewButton;

	[SerializeField]
	private tk2dUIItem m_startNewSkipButton;

	[SerializeField]
	private tk2dUIItem m_loadOldButton;

	[SerializeField]
	private tk2dUIItem m_toMemorialButton;

	[SerializeField]
	private tk2dUIItem m_backToMainMenuButton;

	[SerializeField]
	private GameObject m_loadHierarchy;

	[SerializeField]
	private AirbaseCameraNode m_thisCameraNode;

	[SerializeField]
	private SlotSelectScreen m_slotSelectionScreen;

	[SerializeField]
	private UISelectFinder m_finder;

	[SerializeField]
	private GameMode m_gameModeStandard;

	[SerializeField]
	private GameMode m_gameModeEndless;

	private void Awake()
	{
		m_startNewButton.OnClick += StartNew;
		m_startNewSkipButton.OnClick += StartNewSkip;
		m_loadOldButton.OnClick += Load;
		m_toMemorialButton.OnClick += ToMemorial;
		m_backToMainMenuButton.OnClick += BackToMainMenu;
	}

	private void BackToMainMenu()
	{
		Singleton<UIScreenManager>.Instance.ShowScreen("StartScreen", showNavBarButtons: true);
	}

	private void StartNew()
	{
		if (Singleton<SaveDataContainer>.Instance.Exists())
		{
			m_slotSelectionScreen.SetUp(isLoading: false, skipIntro: false);
			Singleton<UIScreenManager>.Instance.ShowScreen("SlotSelectionScreen", showNavBarButtons: true);
		}
		else if (Singleton<GameFlow>.Instance.GetGameMode().HasTutorial())
		{
			Singleton<GameFlow>.Instance.StartNewGame(0);
		}
		else
		{
			Singleton<GameFlow>.Instance.StartNewGameSkip(0);
		}
	}

	private void DoDemo()
	{
		Singleton<GameFlow>.Instance.SetGameMode(m_gameModeStandard);
		Singleton<UIScreenManager>.Instance.ShowScreen("StartScreenDemo", showNavBarButtons: true);
	}

	private void StartNewSkip()
	{
		if (Singleton<SaveDataContainer>.Instance.Exists())
		{
			m_slotSelectionScreen.SetUp(isLoading: false, skipIntro: true);
			Singleton<UIScreenManager>.Instance.ShowScreen("SlotSelectionScreen", showNavBarButtons: true);
		}
		else
		{
			Singleton<GameFlow>.Instance.StartNewGameSkip(0);
		}
	}

	private void Load()
	{
		m_slotSelectionScreen.SetUp(isLoading: true, skipIntro: false);
		Singleton<UIScreenManager>.Instance.ShowScreen("SlotSelectionScreen", showNavBarButtons: true);
	}

	private void OnEnable()
	{
		m_loadHierarchy.SetActive(Singleton<SaveDataContainer>.Instance.Exists());
		Singleton<AirbaseCameraController>.Instance.MoveCameraToLocation(m_thisCameraNode);
		if (!Singleton<SaveDataContainer>.Instance.Exists() || !Singleton<GameFlow>.Instance.GetGameMode().HasTutorial())
		{
			m_startNewSkipButton.gameObject.SetActive(value: false);
		}
		Singleton<UISelector>.Instance.SetFinder(m_finder);
	}

	private void ToMemorial()
	{
		Singleton<UIScreenManager>.Instance.ShowScreen("NameWall", showNavBarButtons: true);
	}
}
