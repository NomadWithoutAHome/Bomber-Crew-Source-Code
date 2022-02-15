using System.Collections.Generic;
using BomberCrewCommon;
using UnityEngine;

public class BomberUpgradeCatalogueLoader : Singleton<BomberUpgradeCatalogueLoader>, LoadableSystem
{
	public class BomberUpgradeCatalogueInternal
	{
		private List<EquipmentUpgradeFittableBase> m_allUpgrades = new List<EquipmentUpgradeFittableBase>();

		private List<BomberDefaults> m_bomberDefaults = new List<BomberDefaults>();

		private Dictionary<string, EquipmentUpgradeFittableBase> m_allUpgradesDictionary = new Dictionary<string, EquipmentUpgradeFittableBase>();

		public void Add(BomberUpgradeCatalogue buc)
		{
			m_allUpgrades.AddRange(buc.GetAll());
			m_bomberDefaults.AddRange(buc.GetDefaultUpgrades());
			EquipmentUpgradeFittableBase[] all = buc.GetAll();
			foreach (EquipmentUpgradeFittableBase equipmentUpgradeFittableBase in all)
			{
				m_allUpgradesDictionary[equipmentUpgradeFittableBase.name] = equipmentUpgradeFittableBase;
			}
		}

		public List<EquipmentUpgradeFittableBase> GetAllOfType(BomberUpgradeType upgradeType)
		{
			List<EquipmentUpgradeFittableBase> list = new List<EquipmentUpgradeFittableBase>();
			foreach (EquipmentUpgradeFittableBase allUpgrade in m_allUpgrades)
			{
				if (allUpgrade.GetUpgradeType() == upgradeType)
				{
					list.Add(allUpgrade);
				}
			}
			return list;
		}

		public List<EquipmentUpgradeFittableBase> GetAll()
		{
			return m_allUpgrades;
		}

		public EquipmentUpgradeFittableBase GetByName(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				return null;
			}
			EquipmentUpgradeFittableBase value = null;
			m_allUpgradesDictionary.TryGetValue(name, out value);
			return value;
		}

		public List<BomberDefaults> GetDefaultUpgrades()
		{
			return m_bomberDefaults;
		}
	}

	[SerializeField]
	private BomberUpgradeCatalogue m_catalogue;

	private bool m_isLoaded;

	private BomberUpgradeCatalogueInternal m_internalCatalogue;

	private void Awake()
	{
		Singleton<SystemLoader>.Instance.RegisterLoadableSystem(this);
		Singleton<DLCManager>.Instance.OnDLCUpdate += OnDLCUpdated;
	}

	private void OnDLCUpdated(string assetBundle)
	{
		BomberUpgradeCatalogue bomberUpgradeCatalogue = Singleton<DLCManager>.Instance.LoadAssetFromBundle<BomberUpgradeCatalogue>(assetBundle, "BUC_" + assetBundle);
		if (bomberUpgradeCatalogue != null)
		{
			m_internalCatalogue.Add(bomberUpgradeCatalogue);
		}
	}

	public void ContinueLoad()
	{
	}

	public BomberUpgradeCatalogueInternal GetCatalogue()
	{
		return m_internalCatalogue;
	}

	public LoadableSystem[] GetDependencies()
	{
		return null;
	}

	public string GetName()
	{
		return "BomberUpgradeCatalogue";
	}

	public bool IsLoadComplete()
	{
		return m_isLoaded;
	}

	public void StartLoad()
	{
		m_internalCatalogue = new BomberUpgradeCatalogueInternal();
		m_internalCatalogue.Add(m_catalogue);
		m_isLoaded = true;
	}
}
