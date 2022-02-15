using System.Collections.Generic;
using BomberCrewCommon;
using UnityEngine;

public class CrewSpawner : Singleton<CrewSpawner>
{
	public class CrewmanAvatarPairing
	{
		public CrewmanAvatar m_spawnedAvatar;

		public Crewman m_crewman;
	}

	[SerializeField]
	private GameObject m_crewmanPrefab;

	[SerializeField]
	private Transform m_spawnerNodes;

	[SerializeField]
	private Transform m_spawnMarkersUnder;

	[SerializeField]
	private tk2dCamera m_uiCamera;

	[SerializeField]
	private Transform m_walkFromLeft;

	[SerializeField]
	private Transform m_walkFromLeftRight;

	[SerializeField]
	private int m_cameraShares;

	private int m_cameraIndex;

	private bool m_debugInvincible;

	private List<CrewmanAvatarPairing> m_spawnedCrewAvatars = new List<CrewmanAvatarPairing>();

	private void OnEnable()
	{
		Singleton<CrewContainer>.Instance.OnNewCrewman += Refresh;
		Refresh();
	}

	private void OnDisable()
	{
		if (Singleton<CrewContainer>.Instance != null)
		{
			Singleton<CrewContainer>.Instance.OnNewCrewman -= Refresh;
		}
	}

	private void SetInvincible(bool invincible)
	{
		if (invincible == m_debugInvincible)
		{
			return;
		}
		m_debugInvincible = invincible;
		foreach (CrewmanAvatarPairing spawnedCrewAvatar in m_spawnedCrewAvatars)
		{
			spawnedCrewAvatar.m_spawnedAvatar.SetInvincible(m_debugInvincible);
		}
	}

	private void DebugPanelFunc()
	{
		bool invincible = GUILayout.Toggle(m_debugInvincible, "Crew mostly invincible");
		SetInvincible(invincible);
	}

	public CrewmanAvatar Find(Crewman crewman)
	{
		foreach (CrewmanAvatarPairing spawnedCrewAvatar in m_spawnedCrewAvatars)
		{
			if (spawnedCrewAvatar.m_crewman == crewman)
			{
				return spawnedCrewAvatar.m_spawnedAvatar;
			}
		}
		return null;
	}

	public int GetIndexFromCrewman(CrewmanAvatar ca)
	{
		int result = -1;
		for (int i = 0; i < m_spawnedCrewAvatars.Count; i++)
		{
			if (m_spawnedCrewAvatars[i].m_spawnedAvatar == ca)
			{
				result = i;
			}
		}
		return result;
	}

	public CrewmanAvatar Find(int index)
	{
		return m_spawnedCrewAvatars[index].m_spawnedAvatar;
	}

	public List<CrewmanAvatarPairing> GetAllCrew()
	{
		return m_spawnedCrewAvatars;
	}

	private void Refresh()
	{
		if (!(Singleton<CrewContainer>.Instance != null))
		{
			return;
		}
		int maxCrew = Singleton<GameFlow>.Instance.GetGameMode().GetMaxCrew();
		BomberSystems bomberSystems = Singleton<BomberSpawn>.Instance.GetBomberSystems();
		Transform crewHierarchy = bomberSystems.GetCrewHierarchy();
		List<Station> list = new List<Station>();
		for (int i = 0; i < maxCrew; i++)
		{
			Crewman crewman = Singleton<CrewContainer>.Instance.GetCrewman(i);
			if (crewman != null && !crewman.IsDead())
			{
				GameObject gameObject = Object.Instantiate(m_crewmanPrefab);
				gameObject.transform.parent = crewHierarchy;
				gameObject.transform.localScale = Vector3.one;
				gameObject.GetComponent<CrewmanAvatar>().SetUp(Singleton<CrewContainer>.Instance.GetCrewman(i), i, null);
				Station stationForPrimarySkill = bomberSystems.GetStationForPrimarySkill(crewman.GetPrimarySkill().GetSkill(), list);
				list.Add(stationForPrimarySkill);
				gameObject.transform.position = stationForPrimarySkill.transform.position;
				stationForPrimarySkill.SetCrewman(gameObject.GetComponent<CrewmanAvatar>());
				CrewmanAvatarPairing crewmanAvatarPairing = new CrewmanAvatarPairing();
				crewmanAvatarPairing.m_crewman = crewman;
				crewmanAvatarPairing.m_spawnedAvatar = gameObject.GetComponent<CrewmanAvatar>();
				m_spawnedCrewAvatars.Add(crewmanAvatarPairing);
			}
		}
	}

	private void Update()
	{
		m_cameraIndex++;
		if (m_cameraIndex == m_cameraShares)
		{
			m_cameraIndex = 0;
		}
	}

	public bool ShouldAllowCamera(int index)
	{
		return m_cameraIndex % m_cameraShares == index % m_cameraShares;
	}
}
