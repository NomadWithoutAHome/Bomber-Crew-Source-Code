using System.Collections.Generic;
using BomberCrewCommon;
using UnityEngine;

public class CrewStatue : Singleton<CrewStatue>
{
	[SerializeField]
	private Transform[] m_crewStatuePositions;

	[SerializeField]
	private GameObject m_statueFigures;

	[SerializeField]
	private GameObject m_crewGraphicsMale;

	[SerializeField]
	private CrewStatuePlaqueItem[] m_plaques;

	[SerializeField]
	private GameObject m_statueBaseRoot;

	[SerializeField]
	private GameObject m_plaqueStandard;

	[SerializeField]
	private GameObject m_plaqueJa;

	[SerializeField]
	private GameObject m_plaqueZh;

	[SerializeField]
	private GameObject m_plaqueKo;

	private List<GameObject> m_memorialCrew = new List<GameObject>();

	private GameObject m_currentPlaques;

	private void Start()
	{
		m_currentPlaques = m_plaqueStandard;
		SetupCrewStatue();
		Singleton<DLCManager>.Instance.OnDLCUpdate += SetupCrewStatueDLC;
		Singleton<LanguageLoader>.Instance.OnLanguageReload += SetupCrewStatue;
	}

	private void SetPlaqueLanguage()
	{
		GameObject gameObject = m_plaqueStandard;
		switch (Singleton<SystemDataContainer>.Instance.GetCurrentLanguage())
		{
		case "ja":
			gameObject = m_plaqueJa;
			break;
		case "zh":
			gameObject = m_plaqueZh;
			break;
		case "ko":
			gameObject = m_plaqueKo;
			break;
		}
		if (gameObject != m_currentPlaques)
		{
			List<CrewStatuePlaqueItem> list = new List<CrewStatuePlaqueItem>();
			CrewStatuePlaqueItem[] plaques = m_plaques;
			foreach (CrewStatuePlaqueItem crewStatuePlaqueItem in plaques)
			{
				GameObject gameObject2 = Object.Instantiate(gameObject);
				gameObject2.transform.position = crewStatuePlaqueItem.transform.position;
				list.Add(gameObject2.GetComponent<CrewStatuePlaqueItem>());
				Object.Destroy(crewStatuePlaqueItem.gameObject);
			}
			m_plaques = list.ToArray();
			m_currentPlaques = gameObject;
		}
	}

	private void SetupCrewStatueDLC(string newDLC)
	{
		SetupCrewStatue();
	}

	private void OnDestroy()
	{
		Singleton<DLCManager>.Instance.OnDLCUpdate -= SetupCrewStatueDLC;
		Singleton<LanguageLoader>.Instance.OnLanguageReload -= SetupCrewStatue;
	}

	private void StatueDebug()
	{
	}

	private void ClearExisting()
	{
		foreach (GameObject item in m_memorialCrew)
		{
			Object.DestroyImmediate(item);
		}
		m_memorialCrew.Clear();
	}

	public void ResetForNoProfile()
	{
		DisablePlaques();
		ClearExisting();
		m_statueFigures.SetActive(value: true);
	}

	public void SetupCrewStatue()
	{
		List<Crewman> crewForStatue = Singleton<SystemDataContainer>.Instance.Get().GetCrewForStatue();
		SetupCrewStatue(crewForStatue);
	}

	public void SetupCrewStatue(List<Crewman> memorialCrew)
	{
		SetPlaqueLanguage();
		DisablePlaques();
		ClearExisting();
		if (memorialCrew != null && memorialCrew.Count > 0)
		{
			m_statueFigures.SetActive(value: false);
			int num = 0;
			{
				foreach (Crewman item in memorialCrew)
				{
					item.ClearEquipmentCache();
					List<CrewmanEquipmentBase> allEquipment = item.GetAllEquipment();
					GameObject crewGraphicsMale = m_crewGraphicsMale;
					GameObject gameObject = null;
					gameObject = Object.Instantiate(crewGraphicsMale);
					m_memorialCrew.Add(gameObject);
					gameObject.transform.parent = m_crewStatuePositions[num];
					gameObject.transform.localPosition = Vector3.zero;
					gameObject.transform.rotation = m_crewStatuePositions[num].rotation;
					CrewmanGraphics component = gameObject.GetComponent<CrewmanGraphics>();
					component.SetFromCrewman(item);
					component.SetEquipmentFromCrewman(item);
					component.SetStatue();
					SetLayerRecursively(gameObject, m_statueFigures.layer);
					m_plaques[num].gameObject.SetActive(value: true);
					m_plaques[num].SetUp(item.GetCrewmanRankTranslated() + " " + item.GetFirstName() + "\n" + item.GetSurname());
					num++;
				}
				return;
			}
		}
		m_statueFigures.SetActive(value: true);
	}

	private void DisablePlaques()
	{
		CrewStatuePlaqueItem[] plaques = m_plaques;
		foreach (CrewStatuePlaqueItem crewStatuePlaqueItem in plaques)
		{
			crewStatuePlaqueItem.gameObject.SetActive(value: false);
		}
	}

	private void SetLayerRecursively(GameObject obj, int newLayer)
	{
		if (obj == null)
		{
			return;
		}
		obj.layer = newLayer;
		foreach (Transform item in obj.transform)
		{
			if (!(item == null))
			{
				SetLayerRecursively(item.gameObject, newLayer);
			}
		}
	}
}
