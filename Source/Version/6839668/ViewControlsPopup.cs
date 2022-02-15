using BomberCrewCommon;
using UnityEngine;

public class ViewControlsPopup : MonoBehaviour
{
	[SerializeField]
	private tk2dUIItem m_dismissButton;

	[SerializeField]
	private GameObject m_pauseMenuPrefab;

	[SerializeField]
	private GameObject m_pcControlsHierarchy;

	[SerializeField]
	private GameObject m_consoleControlsHierarchy;

	[SerializeField]
	private SelectableFilterButton m_pcControlsButton;

	[SerializeField]
	private SelectableFilterButton m_consoleControlsButton;

	[SerializeField]
	private GameObject m_tabSelectHierarchy;

	private UISelectFinder m_oldFinder;

	private UISelectorMovementType m_oldMovementType;

	private void Start()
	{
		m_dismissButton.OnClick += ReturnToPauseMenu;
		if (true)
		{
			m_tabSelectHierarchy.SetActive(value: true);
			SetControls(Singleton<UISelector>.Instance.IsPrimary());
		}
		else
		{
			m_tabSelectHierarchy.SetActive(value: false);
			m_consoleControlsHierarchy.SetActive(value: true);
			m_pcControlsHierarchy.SetActive(value: false);
		}
		m_pcControlsButton.OnClick += delegate
		{
			SetControls(controller: false);
		};
		m_consoleControlsButton.OnClick += delegate
		{
			SetControls(controller: true);
		};
	}

	private void SetControls(bool controller)
	{
		m_pcControlsButton.SetSelected(!controller);
		m_pcControlsHierarchy.SetActive(!controller);
		m_consoleControlsButton.SetSelected(controller);
		m_consoleControlsHierarchy.SetActive(controller);
	}

	private void ReturnToPauseMenu()
	{
		Singleton<UIPopupManager>.Instance.DismissCurrentPopup();
		Singleton<UIPopupManager>.Instance.DisplayPopup(m_pauseMenuPrefab);
	}

	private void OnEnable()
	{
		Singleton<GameFlow>.Instance.SetPaused(paused: true);
		m_oldFinder = Singleton<UISelector>.Instance.GetCurrentFinder();
		m_oldMovementType = Singleton<UISelector>.Instance.GetCurrentMovementType();
	}

	private void OnDisable()
	{
		Singleton<GameFlow>.Instance.SetPaused(paused: false);
		Singleton<UISelector>.Instance.SetFinder(m_oldFinder);
	}
}
