using System.Collections.Generic;
using BomberCrewCommon;
using UnityEngine;

public class RecruitmentScreenController : MonoBehaviour
{
	[SerializeField]
	private GameObject m_crewRoleIndicatorPrefab;

	[SerializeField]
	private LayoutGrid m_crewRoleLayout;

	[SerializeField]
	private GameObject m_recruitableCrewmanPrefab;

	[SerializeField]
	private GameObject m_recruitmentInfoPanelPrefab;

	[SerializeField]
	private Transform[] m_recruitableCrewmanNodes;

	[SerializeField]
	private Transform m_recruitmentSpawnNode;

	[SerializeField]
	private Transform m_panelTransform;

	[SerializeField]
	private int m_maxOfEachType;

	[SerializeField]
	private int m_maxRecruitmentLineup;

	[SerializeField]
	private UISelectFinder m_recruitSelectFinder;

	[SerializeField]
	private Transform m_hireTablePosition;

	[SerializeField]
	private Transform m_leavePosition;

	[SerializeField]
	private GameObject m_nobodyToRecruitHierarchy;

	[SerializeField]
	private AirbasePersistentCrew m_persistentCrew;

	[SerializeField]
	private GameObject m_recruitmentRecruitButton;

	[SerializeField]
	private Transform[] m_recruitButtonPositions;

	[SerializeField]
	private Transform[] m_recruitmentMidForwardPosition;

	[SerializeField]
	private AirbaseCameraNode m_selectedCamera;

	[SerializeField]
	private AirbaseCameraNode m_noneSelectedCamera;

	private List<RecruitmentSkillRequirementDisplay> m_recruitmentSkillsRequired = new List<RecruitmentSkillRequirementDisplay>();

	private List<Crewman> m_crewmenToHire = new List<Crewman>();

	private List<GameObject> m_crewmenAvatars = new List<GameObject>();

	private List<GameObject> m_createdRecruitObjects = new List<GameObject>();

	private List<SelectableFilterButton> m_createdRecruitButtons = new List<SelectableFilterButton>();

	private List<Crewman.SpecialisationSkill> m_uniquePreviousGeneration = new List<Crewman.SpecialisationSkill>();

	private GameObject m_currentPanel;

	private void Awake()
	{
		GetComponent<AirbaseAreaScreen>().OnAcceptButton += OnAcceptButton;
		GetComponent<AirbaseAreaScreen>().OnBackButton += OnBackButton;
	}

	private void Start()
	{
		SpawnRoleIndicators();
	}

	private void SpawnRoleIndicators()
	{
		Crewman.SpecialisationSkill[] crewRequirements = Singleton<GameFlow>.Instance.GetGameMode().GetCrewRequirements();
		Dictionary<Crewman.SpecialisationSkill, int> dictionary = new Dictionary<Crewman.SpecialisationSkill, int>();
		for (int num = crewRequirements.Length - 1; num >= 0; num--)
		{
			Crewman.SpecialisationSkill specialisationSkill = crewRequirements[num];
			GameObject gameObject = Object.Instantiate(m_crewRoleIndicatorPrefab);
			gameObject.transform.parent = m_crewRoleLayout.transform;
			gameObject.transform.localPosition = Vector3.zero;
			int value = 0;
			dictionary.TryGetValue(specialisationSkill, out value);
			value = (dictionary[specialisationSkill] = value + 1);
			RecruitmentSkillRequirementDisplay component = gameObject.GetComponent<RecruitmentSkillRequirementDisplay>();
			component.SetUp(specialisationSkill, value);
			m_recruitmentSkillsRequired.Add(component);
		}
		m_crewRoleLayout.RepositionChildren();
	}

	private void OnEnable()
	{
		Refresh();
		ShowPanel(null);
		m_persistentCrew.DoFreeWalk();
	}

	private void Refresh()
	{
		foreach (RecruitmentSkillRequirementDisplay item in m_recruitmentSkillsRequired)
		{
			item.Refresh();
		}
		GenerateCrewmenToHire();
		SpawnHireableAvatars();
	}

	private void SpawnHireableAvatars()
	{
		while (m_crewmenAvatars.Count < m_maxRecruitmentLineup)
		{
			m_crewmenAvatars.Add(null);
			m_createdRecruitObjects.Add(null);
			m_createdRecruitButtons.Add(null);
		}
		for (int i = 0; i < m_maxRecruitmentLineup; i++)
		{
			bool flag = false;
			bool flag2 = false;
			if (m_crewmenToHire[i] != null)
			{
				if (m_crewmenAvatars[i] == null || m_crewmenAvatars[i].GetComponent<RecruitmentCrewmanAvatar>().GetCrewman() != m_crewmenToHire[i])
				{
					flag = true;
					flag2 = true;
				}
			}
			else
			{
				flag2 = true;
			}
			if (flag2)
			{
				if (m_crewmenAvatars[i] != null)
				{
					AirbaseCrewmanAvatarBehaviour thisCrewmanBehaviour = m_crewmenAvatars[i].GetComponent<AirbaseCrewmanAvatarBehaviour>();
					m_crewmenAvatars[i].GetComponent<RecruitmentCrewmanAvatar>().SetInteractive(interactive: false);
					thisCrewmanBehaviour.GetCrewmanGraphics().FacialAnimController.Scowl(2f);
					thisCrewmanBehaviour.WalkTo(m_recruitmentSpawnNode.position, null, 0f, 4f, delegate
					{
						Object.Destroy(thisCrewmanBehaviour.gameObject);
					}, 1f);
					m_crewmenAvatars[i] = null;
				}
				if (m_createdRecruitObjects[i] != null)
				{
					Object.Destroy(m_createdRecruitObjects[i]);
					m_createdRecruitObjects[i] = null;
					m_createdRecruitButtons[i] = null;
				}
			}
			if (!flag)
			{
				continue;
			}
			GameObject gameObject = Object.Instantiate(m_recruitableCrewmanPrefab);
			gameObject.transform.parent = m_recruitmentSpawnNode;
			gameObject.transform.localPosition = Vector3.zero;
			Transform transform = m_recruitableCrewmanNodes[i];
			RecruitmentCrewmanAvatar rca = gameObject.GetComponent<RecruitmentCrewmanAvatar>();
			AirbaseCrewmanAvatarBehaviour component = gameObject.GetComponent<AirbaseCrewmanAvatarBehaviour>();
			component.WalkTo(transform.position, transform, 1f, 15f, delegate
			{
				rca.SetInteractive(interactive: true);
			}, 1f);
			m_crewmenAvatars[i] = gameObject;
			Crewman toHire = m_crewmenToHire[i];
			component.SetCrewman(toHire);
			rca.SetUp(toHire);
			rca.SetInteractive(interactive: false);
			rca.OnClick += delegate
			{
				if (m_currentPanel == null)
				{
					ShowPanel(toHire);
				}
			};
			GameObject gameObject2 = Object.Instantiate(m_recruitmentRecruitButton);
			gameObject2.transform.parent = m_recruitButtonPositions[i];
			gameObject2.transform.localPosition = Vector3.zero;
			gameObject2.GetComponent<CrewQuartersCrewmanNameButton>().SetUp(toHire);
			gameObject2.GetComponent<SelectableFilterButton>().OnClick += delegate
			{
				ShowPanel(toHire);
			};
			m_createdRecruitButtons[i] = gameObject2.GetComponent<SelectableFilterButton>();
			m_createdRecruitObjects[i] = gameObject2;
		}
		bool active = true;
		for (int j = 0; j < m_maxRecruitmentLineup; j++)
		{
			if (m_crewmenAvatars[j] != null)
			{
				active = false;
			}
		}
		m_nobodyToRecruitHierarchy.SetActive(active);
	}

	private void ShowPanel(Crewman forCrewman)
	{
		if (m_currentPanel != null)
		{
			Object.Destroy(m_currentPanel);
		}
		GameObject avatar = null;
		for (int i = 0; i < m_maxRecruitmentLineup; i++)
		{
			if (!(m_crewmenAvatars[i] != null))
			{
				continue;
			}
			bool flag = m_crewmenAvatars[i].GetComponent<RecruitmentCrewmanAvatar>().GetCrewman() == forCrewman;
			m_crewmenAvatars[i].GetComponent<RecruitmentCrewmanAvatar>().SetSelected(flag);
			if (flag)
			{
				avatar = m_crewmenAvatars[i];
			}
			RecruitmentCrewmanAvatar rca = m_crewmenAvatars[i].GetComponent<RecruitmentCrewmanAvatar>();
			AirbaseCrewmanAvatarBehaviour component = m_crewmenAvatars[i].GetComponent<AirbaseCrewmanAvatarBehaviour>();
			if (flag)
			{
				m_crewmenAvatars[i].GetComponent<AirbaseCrewmanAvatarBehaviour>().Talk();
				Transform transform = m_recruitmentMidForwardPosition[i];
				m_crewmenAvatars[i].GetComponent<AirbaseCrewmanAvatarBehaviour>().WalkTo(transform.position, transform, 1f, 15f, delegate
				{
					rca.SetInteractive(interactive: true);
				}, 1f);
			}
			else
			{
				Transform transform2 = m_recruitableCrewmanNodes[i];
				m_crewmenAvatars[i].GetComponent<AirbaseCrewmanAvatarBehaviour>().WalkTo(transform2.position, transform2, 1f, 15f, delegate
				{
					rca.SetInteractive(interactive: true);
				}, 1f);
			}
			if (m_createdRecruitButtons[i] != null)
			{
				m_createdRecruitButtons[i].SetSelected(flag);
			}
		}
		if (forCrewman != null)
		{
			m_currentPanel = Object.Instantiate(m_recruitmentInfoPanelPrefab);
			m_currentPanel.transform.parent = m_panelTransform;
			m_currentPanel.transform.localPosition = Vector3.zero;
			Singleton<UISelector>.Instance.SetFinder(m_currentPanel.GetComponent<RecruitmentCrewmanDisplay>().GetFinder());
			m_currentPanel.GetComponent<RecruitmentCrewmanDisplay>().SetUp(forCrewman, avatar);
			m_currentPanel.GetComponent<RecruitmentCrewmanDisplay>().OnClickCancel += delegate
			{
				ShowPanel(null);
			};
			m_currentPanel.GetComponent<RecruitmentCrewmanDisplay>().OnClick += delegate
			{
				Singleton<CrewContainer>.Instance.HireNewCrewman(forCrewman);
				Singleton<AirbaseNavigation>.Instance.Refresh();
				for (int j = 0; j < m_maxRecruitmentLineup; j++)
				{
					if (m_crewmenAvatars[j] != null && m_crewmenAvatars[j].GetComponent<RecruitmentCrewmanAvatar>().GetCrewman() == forCrewman)
					{
						AirbaseCrewmanAvatarBehaviour avatar2 = m_crewmenAvatars[j].GetComponent<AirbaseCrewmanAvatarBehaviour>();
						m_crewmenAvatars[j].GetComponent<RecruitmentCrewmanAvatar>().SetInteractive(interactive: false);
						avatar2.GetCrewmanGraphics().FacialAnimController.Smile(1.5f);
						avatar2.WalkTo(m_hireTablePosition.position, m_hireTablePosition, 1f, 4f, delegate
						{
							avatar2.GetComponent<RecruitmentCrewmanAvatar>().SetSelected(isSelected: false);
							avatar2.Talk();
							avatar2.Wait(1.5f, delegate
							{
								avatar2.WalkTo(m_leavePosition.position, null, 0f, 4f, delegate
								{
									Object.Destroy(avatar2.gameObject);
								}, 1f);
							});
						}, 1f);
						m_crewmenAvatars[j] = null;
					}
				}
				for (int k = 0; k < m_maxRecruitmentLineup; k++)
				{
					if (m_crewmenToHire[k] == forCrewman)
					{
						m_crewmenToHire[k] = null;
					}
				}
				Refresh();
				ShowPanel(null);
				Singleton<AirbaseNavigation>.Instance.RefreshCrewPhoto();
			};
			Singleton<AirbaseCameraController>.Instance.MoveCameraToLocation(m_selectedCamera);
		}
		else
		{
			Singleton<UISelector>.Instance.SetFinder(m_recruitSelectFinder);
			Singleton<AirbaseCameraController>.Instance.MoveCameraToLocation(m_noneSelectedCamera);
		}
	}

	private void GenerateCrewmenToHire()
	{
		List<string> list = new List<string>();
		while (m_crewmenToHire.Count < m_maxRecruitmentLineup)
		{
			m_crewmenToHire.Add(null);
		}
		List<Crewman.SpecialisationSkill> list2 = new List<Crewman.SpecialisationSkill>();
		list2.AddRange(Singleton<GameFlow>.Instance.GetGameMode().GetCrewRequirements());
		int currentCrewCount = Singleton<CrewContainer>.Instance.GetCurrentCrewCount();
		for (int i = 0; i < currentCrewCount; i++)
		{
			list2.Remove(Singleton<CrewContainer>.Instance.GetCrewman(i).GetPrimarySkill().GetSkill());
			list.Add(Singleton<CrewContainer>.Instance.GetCrewman(i).GetSurname());
		}
		List<Crewman.SpecialisationSkill> list3 = new List<Crewman.SpecialisationSkill>();
		foreach (Crewman.SpecialisationSkill item in list2)
		{
			list3.Add(item);
		}
		List<Crewman> list4 = new List<Crewman>();
		foreach (Crewman item2 in m_crewmenToHire)
		{
			if (item2 != null && !list3.Contains(item2.GetPrimarySkill().GetSkill()))
			{
				list4.Add(item2);
			}
		}
		foreach (Crewman item3 in list4)
		{
			m_crewmenToHire[m_crewmenToHire.IndexOf(item3)] = null;
		}
		if (list3.Count > m_maxRecruitmentLineup)
		{
			list3.Sort((Crewman.SpecialisationSkill a, Crewman.SpecialisationSkill b) => Random.Range(-1, 2));
		}
		CrewContainer.RecruitmentInfo currentRecruitmentInfo = Singleton<CrewContainer>.Instance.GetCurrentRecruitmentInfo();
		foreach (Crewman.SpecialisationSkill item4 in list3)
		{
			int num = 0;
			foreach (Crewman item5 in m_crewmenToHire)
			{
				if (item5 != null && item5.GetPrimarySkill().GetSkill() == item4)
				{
					num++;
				}
			}
			while (num < m_maxOfEachType)
			{
				Crewman crewman = new Crewman(new Crewman.Skill(item4, 1), null);
				if (currentRecruitmentInfo != null)
				{
					int num2 = Random.Range(currentRecruitmentInfo.m_minXP, currentRecruitmentInfo.m_maxXP);
					crewman.GetPrimarySkill().AddXP((int)((float)num2 * Singleton<GameFlow>.Instance.GetGameMode().GetXPMultiplier()));
				}
				int num3 = 0;
				while (list.Contains(crewman.GetSurname()))
				{
					crewman = new Crewman(new Crewman.Skill(item4, 1), null);
					num3++;
					if (num3 == 100)
					{
						break;
					}
				}
				bool flag = false;
				for (int j = 0; j < m_maxRecruitmentLineup; j++)
				{
					if (m_crewmenToHire[j] == null)
					{
						m_crewmenToHire[j] = crewman;
						list.Add(crewman.GetSurname());
						flag = true;
						num++;
						break;
					}
				}
				if (!flag)
				{
					break;
				}
			}
		}
	}

	private void OnAcceptButton(bool down)
	{
	}

	private void OnBackButton()
	{
		if (m_currentPanel != null)
		{
			ShowPanel(null);
		}
	}
}
