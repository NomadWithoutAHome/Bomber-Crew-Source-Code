using System.Collections.Generic;
using BomberCrewCommon;
using UnityEngine;

public class BomberSystemUniqueId : MonoBehaviour
{
	[SerializeField]
	private string m_bomberSystemUniqueId;

	[SerializeField]
	private bool m_doSystemPrefabSpawn;

	[SerializeField]
	private bool m_matchParentLayerOnSpawn;

	[SerializeField]
	private bool m_doSystemEnable;

	[SerializeField]
	private bool m_doMeshReplace;

	[SerializeField]
	private Transform m_systemPrefabSpawnLocation;

	[SerializeField]
	private GameObject m_enabledHierarchy;

	[SerializeField]
	private GameObject[] m_additionalEnabledHierarchies;

	[SerializeField]
	private int m_previewLayer = 1;

	[SerializeField]
	private MeshFilter[] m_meshesToReplace;

	[SerializeField]
	private bool m_skipTransparentLayerStuff;

	[SerializeField]
	private GameObject m_ammoFeedHierarchy;

	[SerializeField]
	private bool m_alwaysRefresh;

	[SerializeField]
	private bool m_doMaterialCopy;

	[SerializeField]
	private Material m_defaultMaterialReference;

	[SerializeField]
	private Material[] m_materialsToUpdate;

	[SerializeField]
	private SharedMaterialHolder m_sharedMaterials;

	private EquipmentUpgradeFittableBase m_fittedUpgrade;

	private EquipmentUpgradeFittableBase m_refreshedWith;

	private bool m_everRefreshed;

	private bool m_refreshedWithPreview;

	private GameObject m_prefabSpawnedRoot;

	private int[] m_originalLayers;

	private void Awake()
	{
		BomberSystems componentInParent = GetComponentInParent<BomberSystems>();
		if (componentInParent != null)
		{
			componentInParent.RegisterSystem(m_bomberSystemUniqueId, this);
		}
		m_originalLayers = new int[m_meshesToReplace.Length];
		for (int i = 0; i < m_meshesToReplace.Length; i++)
		{
			m_originalLayers[i] = m_meshesToReplace[i].gameObject.layer;
		}
		RefreshPart();
	}

	public void RefreshPart()
	{
		BomberUpgradeConfig currentConfig = Singleton<BomberContainer>.Instance.GetCurrentConfig();
		string upgradeFor = currentConfig.GetUpgradeFor(m_bomberSystemUniqueId);
		if (!string.IsNullOrEmpty(upgradeFor))
		{
			m_fittedUpgrade = Singleton<BomberUpgradeCatalogueLoader>.Instance.GetCatalogue().GetByName(upgradeFor);
		}
		else
		{
			m_fittedUpgrade = null;
		}
		RefreshPartInternal(m_fittedUpgrade, forPreview: false);
	}

	public void RefreshForPreview(EquipmentUpgradeFittableBase eufb)
	{
		BomberUpgradeConfig currentConfig = Singleton<BomberContainer>.Instance.GetCurrentConfig();
		string upgradeFor = currentConfig.GetUpgradeFor(m_bomberSystemUniqueId);
		if (!string.IsNullOrEmpty(upgradeFor))
		{
			m_fittedUpgrade = Singleton<BomberUpgradeCatalogueLoader>.Instance.GetCatalogue().GetByName(upgradeFor);
		}
		else
		{
			m_fittedUpgrade = null;
		}
		RefreshPartInternal(eufb, m_fittedUpgrade != eufb);
	}

	private void RefreshPartInternal(EquipmentUpgradeFittableBase eufb, bool forPreview)
	{
		if (!(m_refreshedWith != eufb) && forPreview == m_refreshedWithPreview && m_everRefreshed && !m_alwaysRefresh)
		{
			return;
		}
		m_everRefreshed = true;
		if (m_doSystemEnable)
		{
			m_enabledHierarchy.SetActive(eufb != null);
			if (m_additionalEnabledHierarchies.Length > 0)
			{
				GameObject[] additionalEnabledHierarchies = m_additionalEnabledHierarchies;
				foreach (GameObject gameObject in additionalEnabledHierarchies)
				{
					gameObject.SetActive(eufb != null);
				}
			}
			if (forPreview)
			{
				List<Renderer> list = new List<Renderer>();
				list.AddRange(m_enabledHierarchy.GetComponentsInChildren<Renderer>());
				GameObject[] additionalEnabledHierarchies2 = m_additionalEnabledHierarchies;
				foreach (GameObject gameObject2 in additionalEnabledHierarchies2)
				{
					if (gameObject2.GetComponent<Renderer>() != null)
					{
						list.Add(gameObject2.GetComponent<Renderer>());
					}
				}
				foreach (Renderer item in list)
				{
					if (item.gameObject.layer == 0)
					{
						item.gameObject.layer = m_previewLayer;
					}
				}
			}
			else
			{
				List<Renderer> list2 = new List<Renderer>();
				list2.AddRange(m_enabledHierarchy.GetComponentsInChildren<Renderer>());
				GameObject[] additionalEnabledHierarchies3 = m_additionalEnabledHierarchies;
				foreach (GameObject gameObject3 in additionalEnabledHierarchies3)
				{
					if (gameObject3.GetComponent<Renderer>() != null)
					{
						list2.Add(gameObject3.GetComponent<Renderer>());
					}
				}
				foreach (Renderer item2 in list2)
				{
					if (item2.gameObject.layer == m_previewLayer)
					{
						item2.gameObject.layer = 0;
					}
				}
			}
		}
		if (m_doMeshReplace && eufb != null)
		{
			Mesh[] meshes = eufb.GetMeshes(m_bomberSystemUniqueId);
			if (meshes != null)
			{
				int num = 0;
				MeshFilter[] meshesToReplace = m_meshesToReplace;
				foreach (MeshFilter meshFilter in meshesToReplace)
				{
					meshFilter.sharedMesh = meshes[num];
					if (!m_skipTransparentLayerStuff)
					{
						if (forPreview)
						{
							meshFilter.gameObject.layer = m_previewLayer;
						}
						else if (meshFilter.gameObject.layer == m_previewLayer)
						{
							meshFilter.gameObject.layer = m_originalLayers[num];
						}
					}
					num++;
				}
			}
		}
		if (m_ammoFeedHierarchy != null)
		{
			bool active = false;
			GunTurretUpgrade gunTurretUpgrade = (GunTurretUpgrade)eufb;
			if (gunTurretUpgrade != null && gunTurretUpgrade.HasAmmoFeed())
			{
				active = true;
			}
			m_ammoFeedHierarchy.SetActive(active);
			if (forPreview)
			{
				Renderer[] componentsInChildren = m_ammoFeedHierarchy.GetComponentsInChildren<Renderer>();
				Renderer[] array = componentsInChildren;
				foreach (Renderer renderer in array)
				{
					renderer.gameObject.layer = m_previewLayer;
				}
			}
			else
			{
				Renderer[] componentsInChildren2 = m_ammoFeedHierarchy.GetComponentsInChildren<Renderer>();
				Renderer[] array2 = componentsInChildren2;
				foreach (Renderer renderer2 in array2)
				{
					if (renderer2.gameObject.layer == m_previewLayer)
					{
						renderer2.gameObject.layer = 0;
					}
				}
			}
		}
		if (m_doMaterialCopy)
		{
			LiveryEquippable liveryEquippable = (LiveryEquippable)eufb;
			Material material = liveryEquippable.GetReferenceMaterial();
			if (material == null)
			{
				material = m_defaultMaterialReference;
			}
			Material[] materialsToUpdate = m_materialsToUpdate;
			foreach (Material inMat in materialsToUpdate)
			{
				Material @for = m_sharedMaterials.GetFor(inMat);
				Texture texture = @for.GetTexture("_MaskTexture");
				Texture texture2 = @for.GetTexture("_MainTex");
				Texture texture3 = @for.GetTexture("_DamageTexture");
				@for.CopyPropertiesFromMaterial(material);
				@for.shader = material.shader;
				@for.SetTexture("_MaskTexture", texture);
				@for.SetTexture("_MainTex", texture2);
				@for.SetTexture("_DamageTexture", texture3);
			}
		}
		if (m_doSystemPrefabSpawn)
		{
			if (eufb != null)
			{
				if (m_prefabSpawnedRoot != null)
				{
					Object.DestroyImmediate(m_prefabSpawnedRoot);
				}
				GameObject prefabToPlace = eufb.GetPrefabToPlace();
				if (prefabToPlace != null)
				{
					m_prefabSpawnedRoot = Object.Instantiate(prefabToPlace);
					m_prefabSpawnedRoot.transform.parent = m_systemPrefabSpawnLocation;
					m_prefabSpawnedRoot.transform.localPosition = Vector3.zero;
					m_prefabSpawnedRoot.transform.localRotation = Quaternion.identity;
					m_prefabSpawnedRoot.transform.localScale = Vector3.one;
					if (m_matchParentLayerOnSpawn)
					{
						ChangeLayersRecursively(m_prefabSpawnedRoot.transform, m_systemPrefabSpawnLocation.gameObject.layer);
					}
					if (forPreview)
					{
						Renderer[] componentsInChildren3 = m_prefabSpawnedRoot.GetComponentsInChildren<Renderer>();
						Renderer[] array3 = componentsInChildren3;
						foreach (Renderer renderer3 in array3)
						{
							renderer3.gameObject.layer = m_previewLayer;
						}
					}
				}
			}
			else if (m_prefabSpawnedRoot != null)
			{
				Object.DestroyImmediate(m_prefabSpawnedRoot);
			}
		}
		m_refreshedWith = eufb;
		m_refreshedWithPreview = forPreview;
	}

	public GameObject GetPrefabSpawendRoot()
	{
		return m_prefabSpawnedRoot;
	}

	public EquipmentUpgradeFittableBase GetUpgrade()
	{
		return m_fittedUpgrade;
	}

	public string GetUniqueId()
	{
		return m_bomberSystemUniqueId;
	}

	private void ChangeLayersRecursively(Transform t, int layer)
	{
		t.gameObject.layer = layer;
		foreach (Transform item in t)
		{
			ChangeLayersRecursively(item, layer);
		}
	}
}
