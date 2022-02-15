using System.Collections.Generic;
using BomberCrewCommon;
using UnityEngine;

public class CrewmanGearCatalogueLoader : Singleton<CrewmanGearCatalogueLoader>, LoadableSystem
{
	public class CrewmanGearCatalogueInternal
	{
		private List<CrewmanEquipmentBase> m_allEquipment = new List<CrewmanEquipmentBase>();

		private List<CrewmanEquipmentBase> m_defaultGear = new List<CrewmanEquipmentBase>();

		private List<CrewmanEquipmentPreset> m_allPresets = new List<CrewmanEquipmentPreset>();

		public void Add(CrewmanGearCatalogue cgc)
		{
			m_allEquipment.AddRange(cgc.GetAll());
			m_defaultGear.AddRange(cgc.GetAllDefaults());
			m_allPresets.AddRange(cgc.GetAllPresets());
		}

		public CrewmanEquipmentBase GetByName(string byName)
		{
			foreach (CrewmanEquipmentBase item in m_allEquipment)
			{
				if (string.Equals(item.name, byName))
				{
					return item;
				}
			}
			return null;
		}

		public List<CrewmanEquipmentPreset> GetAllPresets()
		{
			return m_allPresets;
		}

		public List<CrewmanEquipmentBase> GetAll()
		{
			return m_allEquipment;
		}

		public List<CrewmanEquipmentBase> GetByType(CrewmanGearType byType)
		{
			List<CrewmanEquipmentBase> list = new List<CrewmanEquipmentBase>();
			foreach (CrewmanEquipmentBase item in m_allEquipment)
			{
				if (item.GetGearType() == byType)
				{
					list.Add(item);
				}
			}
			return list;
		}

		public CrewmanEquipmentBase GetDefaultByType(CrewmanGearType byType)
		{
			List<CrewmanEquipmentBase> list = new List<CrewmanEquipmentBase>();
			foreach (CrewmanEquipmentBase item in m_defaultGear)
			{
				if (item.GetGearType() == byType)
				{
					return item;
				}
			}
			return null;
		}
	}

	[SerializeField]
	private CrewmanGearCatalogue m_catalogue;

	private CrewmanGearCatalogueInternal m_catalogueInternal;

	private bool m_loadComplete;

	private void Awake()
	{
		Singleton<SystemLoader>.Instance.RegisterLoadableSystem(this);
		Singleton<DLCManager>.Instance.OnDLCUpdate += OnDLCUpdate;
	}

	public CrewmanGearCatalogueInternal GetCatalogue()
	{
		return m_catalogueInternal;
	}

	public bool IsLoadComplete()
	{
		return m_loadComplete;
	}

	private void OnDLCUpdate(string assetBundle)
	{
		CrewmanGearCatalogue crewmanGearCatalogue = Singleton<DLCManager>.Instance.LoadAssetFromBundle<CrewmanGearCatalogue>(assetBundle, "CGC_" + assetBundle);
		if (crewmanGearCatalogue != null)
		{
			m_catalogueInternal.Add(crewmanGearCatalogue);
		}
	}

	public void StartLoad()
	{
		m_catalogueInternal = new CrewmanGearCatalogueInternal();
		m_catalogueInternal.Add(m_catalogue);
		m_loadComplete = true;
	}

	public void ContinueLoad()
	{
	}

	public string GetName()
	{
		return "CrewmanGearCatalogue";
	}

	public LoadableSystem[] GetDependencies()
	{
		return null;
	}
}
