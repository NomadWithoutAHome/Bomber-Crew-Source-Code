using System.Collections.Generic;
using BomberCrewCommon;
using UnityEngine;

public class PauseMenuCrewDetailsPage : MonoBehaviour
{
	[SerializeField]
	private GameObject m_crewmanSelectButtonPrefab;

	[SerializeField]
	private GameObject m_crewmanSelectButtonPrefabSmall;

	[SerializeField]
	private LayoutGrid m_crewmanSelectLayout;

	[SerializeField]
	private LayoutGrid m_crewmanSelectLayoutSmall;

	[SerializeField]
	private GameObject m_crewmanDetailsPrefab;

	[SerializeField]
	private Transform m_crewmanDetailsHierarchy;

	[SerializeField]
	private GameObject m_crewmanGraphicsPrefab;

	[SerializeField]
	private Transform m_crewmanGraphicsHierarchy;

	[SerializeField]
	private CrewmanHealthBar m_healthBar;

	[SerializeField]
	private CrewTrainingView m_trainingView;

	[SerializeField]
	private GameObject m_statusAvailable;

	[SerializeField]
	private GameObject m_statusBleedOut;

	[SerializeField]
	private GameObject m_statusBailedOut;

	[SerializeField]
	private GameObject m_statusFalling;

	[SerializeField]
	private GameObject m_statusDead;

	[SerializeField]
	private UISelectFinder m_finder;

	[SerializeField]
	private UISelectorMovementType m_movementType;

	[SerializeField]
	private UISelectFinder m_finderSkills;

	private List<GameObject> m_crewStatusDisplays = new List<GameObject>();

	private List<SelectableFilterButton> m_allCrewButtons = new List<SelectableFilterButton>();

	private GameObject m_crewmanGraphics;

	private GameObject m_panelDisplay;

	private void OnEnable()
	{
		Singleton<UISelector>.Instance.SetFinder(m_finder);
		Singleton<MainActionButtonMonitor>.Instance.AddListener(MainButtonPresses);
	}

	private void OnDisable()
	{
		Singleton<MainActionButtonMonitor>.Instance.RemoveListener(MainButtonPresses, invalidateCurrentPress: false);
	}

	private bool MainButtonPresses(MainActionButtonMonitor.ButtonPress bp)
	{
		switch (bp)
		{
		case MainActionButtonMonitor.ButtonPress.Confirm:
			if (Singleton<UISelector>.Instance.GetCurrentFinder() == m_finder)
			{
				Singleton<UISelector>.Instance.SetFinder(m_finderSkills);
			}
			return true;
		case MainActionButtonMonitor.ButtonPress.Back:
			if (Singleton<UISelector>.Instance.GetCurrentFinder() == m_finderSkills)
			{
				Singleton<UISelector>.Instance.SetFinder(m_finder);
			}
			else if (Singleton<UISelector>.Instance.GetCurrentFinder() == m_finder)
			{
				return false;
			}
			return true;
		default:
			return false;
		}
	}

	private void SetLayer(Transform t, int l)
	{
		t.gameObject.layer = l;
		foreach (Transform item in t)
		{
			SetLayer(item, l);
		}
	}

	private void Start()
	{
		int l = LayerMask.NameToLayer("_Popup");
		m_crewStatusDisplays.Add(m_statusAvailable);
		m_crewStatusDisplays.Add(m_statusBleedOut);
		m_crewStatusDisplays.Add(m_statusBailedOut);
		m_crewStatusDisplays.Add(m_statusFalling);
		m_crewStatusDisplays.Add(m_statusDead);
		GameObject original = ((!Singleton<GameFlow>.Instance.GetGameMode().GetUseUSNaming()) ? m_crewmanSelectButtonPrefab : m_crewmanSelectButtonPrefabSmall);
		LayoutGrid layoutGrid = ((!Singleton<GameFlow>.Instance.GetGameMode().GetUseUSNaming()) ? m_crewmanSelectLayout : m_crewmanSelectLayoutSmall);
		List<CrewSpawner.CrewmanAvatarPairing> allCrew = Singleton<CrewSpawner>.Instance.GetAllCrew();
		foreach (CrewSpawner.CrewmanAvatarPairing cap in allCrew)
		{
			GameObject gameObject = Object.Instantiate(original);
			gameObject.transform.parent = layoutGrid.transform;
			gameObject.GetComponent<CrewTrainingCrewmanNameButton>().SetUp(cap.m_crewman);
			SelectableFilterButton sfb = gameObject.GetComponent<SelectableFilterButton>();
			sfb.OnClick += delegate
			{
				foreach (SelectableFilterButton allCrewButton in m_allCrewButtons)
				{
					allCrewButton.SetSelected(sfb == allCrewButton);
				}
				SelectCrewman(cap);
			};
			m_allCrewButtons.Add(sfb);
			SetLayer(gameObject.transform, l);
		}
		layoutGrid.RepositionChildren();
		m_panelDisplay = Object.Instantiate(m_crewmanDetailsPrefab);
		m_panelDisplay.transform.parent = m_crewmanDetailsHierarchy;
		m_panelDisplay.transform.localPosition = Vector3.zero;
		SetLayer(m_panelDisplay.transform, l);
		m_allCrewButtons[0].SetSelected(selected: true);
		SelectCrewman(allCrew[0]);
	}

	private void SelectCrewman(CrewSpawner.CrewmanAvatarPairing cap)
	{
		if (m_crewmanGraphics != null)
		{
			Object.Destroy(m_crewmanGraphics);
		}
		if (cap.m_crewman.IsDead())
		{
			m_crewmanGraphics.SetActive(value: false);
			ShowStatusDisplay(m_statusDead);
		}
		else
		{
			m_crewmanGraphics = Object.Instantiate(m_crewmanGraphicsPrefab);
			m_crewmanGraphics.transform.parent = m_crewmanGraphicsHierarchy;
			m_crewmanGraphics.transform.localPosition = Vector3.zero;
			CrewmanGraphics component = m_crewmanGraphics.GetComponent<CrewmanGraphics>();
			m_panelDisplay.GetComponent<CrewQuartersCrewmanDisplayPanel>().SetUp(cap.m_crewman, null);
			component.SetFromCrewman(cap.m_crewman);
			component.SetEquipmentFromCrewman(cap.m_crewman);
			component.SetUseRealtimeForAnimation(useRealtime: true);
			SetLayer(m_crewmanGraphics.transform, LayerMask.NameToLayer("PhotoBooth"));
			if (cap.m_spawnedAvatar.GetHealthState().IsCountingDown())
			{
				ShowStatusDisplay(m_statusBleedOut);
				cap.m_spawnedAvatar.GetCrewmanGraphics().SetIsCollapsed(collapsed: true);
			}
			else if (cap.m_spawnedAvatar.IsBailedOut())
			{
				if (cap.m_spawnedAvatar.IsBailedOutNoParachute())
				{
					ShowStatusDisplay(m_statusFalling);
					m_crewmanGraphics.SetActive(value: false);
				}
				else
				{
					ShowStatusDisplay(m_statusBailedOut);
					m_crewmanGraphics.SetActive(value: false);
				}
			}
			else
			{
				ShowStatusDisplay(m_statusAvailable);
			}
		}
		m_healthBar.SetUp(cap.m_spawnedAvatar);
		m_trainingView.SetUpForCrewman(cap.m_crewman);
	}

	private void ShowStatusDisplay(GameObject statusDisplay)
	{
		foreach (GameObject crewStatusDisplay in m_crewStatusDisplays)
		{
			crewStatusDisplay.SetActive(crewStatusDisplay == statusDisplay);
		}
	}
}
