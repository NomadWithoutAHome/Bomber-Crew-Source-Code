using System.Collections;
using BomberCrewCommon;
using UnityEngine;

public class CrewAndBomberPhotoBooth : Singleton<CrewAndBomberPhotoBooth>
{
	[SerializeField]
	private GameObject m_graphicsPrefab;

	[SerializeField]
	private Transform[] m_crewmanGraphicsNodes;

	[SerializeField]
	private GameObject[] m_blobShadows;

	[SerializeField]
	private RuntimeAnimatorController[] m_animControllers;

	[SerializeField]
	private BomberLivery m_bomberLivery;

	[SerializeField]
	private Transform m_bomberGraphicsNode;

	[SerializeField]
	private Camera m_camera;

	private Crewman[] m_crew;

	private CrewmanGraphics[] m_crewmanGraphics;

	private GameObject m_airbaseBomber;

	private bool m_isProcessing;

	private RenderTexture m_renderTexture;

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

	public RenderTexture RenderForCrewAndBomber(Crewman[] c, BomberUpgradeConfig buc)
	{
		m_crew = c;
		if (m_crewmanGraphics != null)
		{
			CrewmanGraphics[] crewmanGraphics = m_crewmanGraphics;
			foreach (CrewmanGraphics crewmanGraphics2 in crewmanGraphics)
			{
				Object.DestroyImmediate(crewmanGraphics2.gameObject);
			}
		}
		m_crewmanGraphics = new CrewmanGraphics[m_crew.Length];
		for (int j = 0; j < m_crew.Length; j++)
		{
			m_crewmanGraphics[j] = Object.Instantiate(m_graphicsPrefab).GetComponent<CrewmanGraphics>();
			m_crewmanGraphics[j].transform.parent = m_crewmanGraphicsNodes[j].transform;
			m_crewmanGraphics[j].transform.localPosition = Vector3.zero;
			m_crewmanGraphics[j].transform.localRotation = Quaternion.identity;
			m_crewmanGraphics[j].transform.localScale = Vector3.one;
			m_crewmanGraphics[j].SetFromCrewman(m_crew[j]);
			m_crewmanGraphics[j].SetEquipmentFromCrewman(m_crew[j]);
			m_crewmanGraphics[j].SetAnimatorController(m_animControllers[j]);
			SetLayerRecursively(m_crewmanGraphics[j].gameObject, base.gameObject.layer);
			m_blobShadows[j].SetActive(value: true);
		}
		if (m_airbaseBomber != null)
		{
			Object.DestroyImmediate(m_airbaseBomber.gameObject);
		}
		m_airbaseBomber = Object.Instantiate(Singleton<GameFlow>.Instance.GetGameMode().GetAirbaseBomberPrefab());
		m_airbaseBomber.transform.parent = m_bomberGraphicsNode.transform;
		m_airbaseBomber.transform.localPosition = Vector3.zero;
		m_airbaseBomber.transform.localRotation = Quaternion.identity;
		m_airbaseBomber.transform.localScale = Vector3.one;
		m_airbaseBomber.SetActive(value: false);
		SetSharedMaterialToBomberLivery[] componentsInChildren = m_airbaseBomber.GetComponentsInChildren<SetSharedMaterialToBomberLivery>(includeInactive: true);
		foreach (SetSharedMaterialToBomberLivery setSharedMaterialToBomberLivery in componentsInChildren)
		{
			setSharedMaterialToBomberLivery.SetOverrideLivery(m_bomberLivery);
		}
		m_bomberLivery.SetBomberConfig(buc);
		m_bomberLivery.Refresh();
		SetLayerRecursively(m_airbaseBomber.gameObject, base.gameObject.layer);
		m_renderTexture = new RenderTexture(512, 512, 24, RenderTextureFormat.ARGB32);
		m_renderTexture.filterMode = FilterMode.Bilinear;
		StartCoroutine(Process());
		return m_renderTexture;
	}

	public bool IsProcessing()
	{
		return m_isProcessing;
	}

	private IEnumerator Process()
	{
		if (!m_isProcessing)
		{
			m_isProcessing = true;
			while (m_bomberLivery.IsGenerating())
			{
				yield return null;
			}
			m_airbaseBomber.SetActive(value: true);
			m_camera.targetTexture = m_renderTexture;
			yield return null;
			yield return new WaitForEndOfFrame();
			while (m_bomberLivery.IsGenerating())
			{
				yield return null;
			}
			m_camera.Render();
			m_camera.targetTexture = null;
			m_isProcessing = false;
		}
	}
}
