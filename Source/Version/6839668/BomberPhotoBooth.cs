using System.Collections;
using BomberCrewCommon;
using UnityEngine;

public class BomberPhotoBooth : Singleton<BomberPhotoBooth>
{
	[SerializeField]
	private Transform m_graphicsNode;

	[SerializeField]
	private Camera m_camera;

	[SerializeField]
	private BomberLivery m_bomberLivery;

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

	public RenderTexture RenderForBomber(BomberUpgradeConfig buc)
	{
		if (m_airbaseBomber != null)
		{
			Object.DestroyImmediate(m_airbaseBomber.gameObject);
		}
		m_airbaseBomber = Object.Instantiate(Singleton<GameFlow>.Instance.GetGameMode().GetAirbaseBomberPrefab());
		m_airbaseBomber.transform.parent = m_graphicsNode.transform;
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
		m_renderTexture = new RenderTexture(512, 256, 24, RenderTextureFormat.ARGB32);
		m_renderTexture.antiAliasing = 8;
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
