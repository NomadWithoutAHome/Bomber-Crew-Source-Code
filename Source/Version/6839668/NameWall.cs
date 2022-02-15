using System.Collections.Generic;
using BomberCrewCommon;
using UnityEngine;

public class NameWall : MonoBehaviour
{
	[SerializeField]
	private GameObject m_nameWallItemPrefab;

	[SerializeField]
	private GameObject m_nameWallItemPrefabJa;

	[SerializeField]
	private GameObject m_nameWallItemPrefabZh;

	[SerializeField]
	private GameObject m_nameWallItemPrefabKo;

	[SerializeField]
	private LayoutGrid[] m_nameWallLayoutGrids;

	[SerializeField]
	private int m_nameCountPerColumn = 18;

	[SerializeField]
	private int m_columnCountPerPage = 3;

	private List<MemorialNameWallItem> m_allWallItems = new List<MemorialNameWallItem>();

	private void Start()
	{
		SetUpNameWall();
		Singleton<LanguageLoader>.Instance.OnLanguageReload += SetUpNameWall;
	}

	private void OnDestroy()
	{
		Singleton<LanguageLoader>.Instance.OnLanguageReload -= SetUpNameWall;
	}

	private void SetUpNameWall()
	{
		ClearNameWall();
		List<SystemData.CrewmanLite> crewForWallNames = Singleton<SystemDataContainer>.Instance.Get().GetCrewForWallNames();
		if (crewForWallNames == null)
		{
			return;
		}
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		string text = string.Empty;
		List<string> list = new List<string>();
		int num4 = crewForWallNames.Count;
		int num5 = m_nameWallLayoutGrids.Length * m_nameCountPerColumn * m_columnCountPerPage;
		if (num4 > num5)
		{
			num4 = num5;
		}
		for (int i = 0; i < num4; i++)
		{
			SystemData.CrewmanLite crewmanLite = crewForWallNames[i];
			string text2 = text;
			text = text2 + crewmanLite.GetCrewmanRankTranslated() + ". " + crewmanLite.GetFullName() + "\n";
			num2++;
			if (num2 == m_nameCountPerColumn || i == num4 - 1)
			{
				CreateNewColumnTextObject(num3);
				list.Add(text);
				num++;
				num2 = 0;
				text = string.Empty;
				if (num == m_columnCountPerPage)
				{
					num3++;
					num = 0;
				}
			}
		}
		for (int j = 0; j < list.Count; j++)
		{
			m_allWallItems[j].SetUp(list[j]);
		}
		LayoutGrid[] nameWallLayoutGrids = m_nameWallLayoutGrids;
		foreach (LayoutGrid layoutGrid in nameWallLayoutGrids)
		{
			layoutGrid.RepositionChildren();
		}
	}

	private void CreateNewColumnTextObject(int pageIndex)
	{
		GameObject original = m_nameWallItemPrefab;
		switch (Singleton<SystemDataContainer>.Instance.GetCurrentLanguage())
		{
		case "ja":
			original = m_nameWallItemPrefabJa;
			break;
		case "zh":
			original = m_nameWallItemPrefabZh;
			break;
		case "ko":
			original = m_nameWallItemPrefabKo;
			break;
		}
		GameObject gameObject = Object.Instantiate(original);
		gameObject.transform.parent = m_nameWallLayoutGrids[pageIndex].transform;
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.rotation = m_nameWallLayoutGrids[pageIndex].transform.rotation;
		m_allWallItems.Add(gameObject.GetComponent<MemorialNameWallItem>());
	}

	private void ClearNameWall()
	{
		LayoutGrid[] nameWallLayoutGrids = m_nameWallLayoutGrids;
		foreach (LayoutGrid layoutGrid in nameWallLayoutGrids)
		{
			for (int j = 0; j < layoutGrid.transform.childCount; j++)
			{
				Object.DestroyImmediate(layoutGrid.transform.GetChild(j).gameObject);
			}
		}
		m_allWallItems.Clear();
	}
}
