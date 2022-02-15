using System.Collections;
using System.Collections.Generic;
using BomberCrewCommon;
using UnityEngine;

public class CrewmanPhotoBooth : Singleton<CrewmanPhotoBooth>
{
	[SerializeField]
	private GameObject m_graphicsPrefabM;

	[SerializeField]
	private GameObject m_graphicsPrefabF;

	[SerializeField]
	private Transform m_graphicsNode;

	[SerializeField]
	private Camera m_camera;

	private CrewmanGraphics m_crewmanGraphics;

	private List<Crewman> m_renderQueue = new List<Crewman>();

	private List<RenderTexture> m_renderTextures = new List<RenderTexture>();

	private bool m_isProcessing;

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

	public RenderTexture RenderForCrewman(Crewman c, RenderTexture current)
	{
		if (current == null)
		{
			current = new RenderTexture(128, 128, 24, RenderTextureFormat.ARGB32);
			current.antiAliasing = 8;
		}
		m_renderQueue.Add(c);
		m_renderTextures.Add(current);
		StartCoroutine(ProcessQueue());
		return current;
	}

	public bool IsProcessing()
	{
		return m_isProcessing;
	}

	private IEnumerator ProcessQueue()
	{
		if (m_isProcessing)
		{
			yield break;
		}
		m_isProcessing = true;
		while (m_renderQueue.Count > 0)
		{
			Crewman c = m_renderQueue[0];
			RenderTexture current = m_renderTextures[0];
			m_renderQueue.RemoveAt(0);
			m_renderTextures.RemoveAt(0);
			if (m_crewmanGraphics != null)
			{
				Object.DestroyImmediate(m_crewmanGraphics.gameObject);
			}
			m_crewmanGraphics = Object.Instantiate(m_graphicsPrefabM).GetComponent<CrewmanGraphics>();
			m_crewmanGraphics.transform.parent = m_graphicsNode.transform;
			m_crewmanGraphics.transform.localPosition = Vector3.zero;
			m_crewmanGraphics.transform.localRotation = Quaternion.identity;
			m_crewmanGraphics.transform.localScale = Vector3.one;
			m_crewmanGraphics.SetFromCrewman(c);
			m_crewmanGraphics.SetEquipmentFromCrewman(c);
			SetLayerRecursively(m_crewmanGraphics.gameObject, base.gameObject.layer);
			m_camera.targetTexture = current;
			yield return null;
			yield return new WaitForEndOfFrame();
			m_camera.Render();
			m_camera.targetTexture = null;
		}
		m_isProcessing = false;
	}
}
