using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using BestHTTP;
using BomberCrewCommon;
using UnityEngine;

public class PrintCrewStatue : MonoBehaviour
{
	[SerializeField]
	private CrewStatue m_statue;

	[SerializeField]
	private Transform m_statueBaseRoot;

	[SerializeField]
	private GameObject m_showPrintStatuePopup;

	[SerializeField]
	private tk2dUIItem m_showPrintStatueButton;

	[SerializeField]
	private GameObject m_showPrintStatueEnabledButton;

	[SerializeField]
	private bool m_useCurrentCrew;

	private bool m_isProcessing;

	private string m_errorString;

	private void Awake()
	{
		m_showPrintStatueEnabledButton.SetActive(value: false);
	}

	private void Do3DPrintPopup()
	{
		UIPopupData uIPopupData = new UIPopupData();
		uIPopupData.PopupStartCallback = (Action<UIPopUp>)Delegate.Combine(uIPopupData.PopupStartCallback, (Action<UIPopUp>)delegate(UIPopUp uip)
		{
			uip.GetComponent<PrintCrewStatuePopup>().SetUp(this);
		});
		Singleton<UIPopupManager>.Instance.DisplayPopup(m_showPrintStatuePopup, uIPopupData);
	}

	public string GetErrorString()
	{
		return m_errorString;
	}

	public void DoPrint()
	{
		if (!m_isProcessing)
		{
			m_errorString = string.Empty;
			StartCoroutine(DoExport());
		}
	}

	private List<GameObject> GetAllUnderNode(Transform t)
	{
		List<GameObject> list = new List<GameObject>();
		if (t.name == "CrewNameText" || t.name.Contains("_Reflection") || t.name.Contains("Outline") || t.name.Contains("Face"))
		{
			return list;
		}
		foreach (Transform item in t)
		{
			List<GameObject> allUnderNode = GetAllUnderNode(item);
			foreach (GameObject item2 in allUnderNode)
			{
				if (!list.Contains(item2))
				{
					list.Add(item2);
				}
			}
		}
		if (!list.Contains(t.gameObject))
		{
			list.Add(t.gameObject);
		}
		return list;
	}

	public bool IsProcessing()
	{
		return m_isProcessing;
	}

	private IEnumerator DoWWWRequest(byte[] data)
	{
		string url2 = "https://i.materialise.com/upload";
		url2 = url2.Replace("<toolID>", "602b6f41-6156-458c-8c60-5db2edcb2069");
		HTTPRequest req = new HTTPRequest(new Uri(url2), HTTPMethods.Post);
		req.AddBinaryData("file", data, "BomberCrewStatue.stl");
		req.AddField("useAjax", "false");
		req.AddField("forceEmbedding", "false");
		req.AddField("plugin", "602b6f41-6156-458c-8c60-5db2edcb2069");
		req.MaxRedirects = 1;
		req.Send();
		yield return StartCoroutine(req);
		if (!string.IsNullOrEmpty(req.RedirectUri.AbsoluteUri))
		{
			m_errorString = string.Empty;
			Application.OpenURL(req.RedirectUri.AbsoluteUri);
		}
		else
		{
			m_errorString = req.Response.DataAsText;
		}
		m_isProcessing = false;
	}

	private IEnumerator DoExport()
	{
		m_isProcessing = true;
		if (m_useCurrentCrew)
		{
			int currentCrewCount = Singleton<CrewContainer>.Instance.GetCurrentCrewCount();
			List<Crewman> list = new List<Crewman>();
			for (int i = 0; i < currentCrewCount; i++)
			{
				list.Add(Singleton<CrewContainer>.Instance.GetCrewman(i));
			}
			m_statue.SetupCrewStatue(list);
		}
		else
		{
			List<Crewman> crewForStatue = Singleton<SystemDataContainer>.Instance.Get().GetCrewForStatue();
			foreach (Crewman item in crewForStatue)
			{
				item.SetEquippedFor(CrewmanGearType.Vest, Singleton<CrewmanGearCatalogueLoader>.Instance.GetCatalogue().GetByName("VestNone"));
			}
			m_statue.SetupCrewStatue(crewForStatue);
		}
		yield return new WaitForSeconds(0.1f);
		CrewmanGraphics[] cg = m_statueBaseRoot.GetComponentsInChildren<CrewmanGraphics>();
		CrewmanGraphics[] array = cg;
		foreach (CrewmanGraphics crewmanGraphics in array)
		{
			crewmanGraphics.CombineMeshes();
			crewmanGraphics.SetStatue();
		}
		yield return new WaitForSeconds(0.1f);
		List<GameObject> allToAdd = new List<GameObject>();
		allToAdd.AddRange(GetAllUnderNode(m_statueBaseRoot.transform));
		List<GameObject> confirmList = new List<GameObject>();
		foreach (GameObject item2 in allToAdd)
		{
			if (item2.GetComponent<Renderer>() != null && item2.activeInHierarchy && item2.GetComponent<Renderer>().enabled)
			{
				confirmList.Add(item2);
			}
		}
		string tmpFile = Path.GetTempFileName();
		STL.ExportBinary(confirmList.ToArray(), tmpFile, 15f, 0f);
		StartCoroutine(DoWWWRequest(File.ReadAllBytes(tmpFile)));
	}
}
