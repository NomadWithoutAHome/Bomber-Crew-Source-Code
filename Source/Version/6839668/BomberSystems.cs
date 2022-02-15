using System;
using System.Collections.Generic;
using UnityEngine;

public class BomberSystems : MonoBehaviour
{
	public enum StationType
	{
		Pilot,
		Navigation,
		RadioOperator,
		BombAimer,
		GunsNose,
		GunsTail,
		GunsUpperDeck,
		GunsLowerDeck,
		FlightEngineer,
		MedicalBay,
		GunsWaistLeft,
		GunsWaistRight,
		GunsCheekLeft,
		GunsCheekRight
	}

	public enum FuselageType
	{
		Tail,
		MidStern,
		MidAft,
		Cockpit,
		Nose,
		BayDoors
	}

	[Serializable]
	public class StationListItem
	{
		[SerializeField]
		public StationType m_stationType;

		[SerializeField]
		public Station m_station;

		[SerializeField]
		public Crewman.SpecialisationSkill m_skill;
	}

	[Serializable]
	public class FuselageListItem
	{
		[SerializeField]
		public FuselageType m_fuselageType;

		[SerializeField]
		public BomberFuselageSection m_section;
	}

	[SerializeField]
	private Transform m_crewHierarchy;

	[SerializeField]
	private BomberState m_bomberState;

	[SerializeField]
	private BombBayDoors m_bombBayDoors;

	[SerializeField]
	private BombBay m_bombBay;

	[SerializeField]
	private Transform[] m_targetableAreas;

	[SerializeField]
	private BomberNavigation m_bomberNavigation;

	[SerializeField]
	private BomberCameraTrackNode m_trackingNode;

	[SerializeField]
	private FireOverview m_fireOverview;

	[SerializeField]
	private PhotoCamera m_photoCamera;

	[SerializeField]
	private BomberAllWalkZones m_walkZones;

	[SerializeField]
	private GameObject m_destroyedPartRigidBodyPrefab;

	[SerializeField]
	private WeatherSensor m_weatherSensor;

	[SerializeField]
	private AmmoFeed[] m_ammoFeeds;

	[SerializeField]
	private BombSight m_bombSight;

	[SerializeField]
	private AmmoBeltBox m_ammoBox;

	[SerializeField]
	private DamageableMeshSharedMask m_damageMask;

	[SerializeField]
	private StationListItem[] m_allStations;

	[SerializeField]
	private FuelTank[] m_fuelTanks;

	[SerializeField]
	private ElectricalSystem m_electricalSystem;

	[SerializeField]
	private BomberTemperatureOxygen m_temperatureOxygen;

	[SerializeField]
	private LandingGear m_landingGear;

	[SerializeField]
	private LandingGearSingle[] m_landingGearSeparates;

	[SerializeField]
	private EngineExtinguisher[] m_engineExtinguishers;

	[SerializeField]
	private Engine[] m_enginesOrdered;

	[SerializeField]
	private HydraulicsTank m_hydraulics;

	[SerializeField]
	private OxygenTank m_oxygenTank;

	[SerializeField]
	private Transform m_floorUpTransform;

	[SerializeField]
	private BomberCriticalFlashAll m_criticalFlashInfo;

	[SerializeField]
	private BigTransform m_bigTransform;

	private Dictionary<string, BomberSystemUniqueId> m_registeredSystems = new Dictionary<string, BomberSystemUniqueId>();

	private Dictionary<Type, List<InteractiveItem>> m_searchableInteractives = new Dictionary<Type, List<InteractiveItem>>();

	private int m_initialEngineCount;

	private List<Engine> m_engines = new List<Engine>();

	private int m_litUpCounter;

	private List<Transform> m_bomberTargetableAreas = new List<Transform>();

	private void Start()
	{
		Transform[] targetableAreas = m_targetableAreas;
		foreach (Transform t in targetableAreas)
		{
			m_bomberTargetableAreas.Add(t);
			BomberDestroyableSection bomberDestroyableSection = BomberDestroyableSection.FindDestroyableSectionFor(t);
			if (bomberDestroyableSection != null)
			{
				bomberDestroyableSection.OnSectionDestroy += delegate
				{
					m_bomberTargetableAreas.Remove(t);
				};
			}
		}
	}

	public BigTransform GetBigTransform()
	{
		return m_bigTransform;
	}

	public DamageableMeshSharedMask GetDamageMask()
	{
		return m_damageMask;
	}

	public void RegisterSystem(string sysName, BomberSystemUniqueId bud)
	{
		if (m_registeredSystems.ContainsKey(sysName))
		{
		}
		m_registeredSystems[sysName] = bud;
	}

	public void AddSearchLight()
	{
		m_litUpCounter++;
	}

	public int GetLitUpCounter()
	{
		return m_litUpCounter;
	}

	public AmmoBeltBox GetAmmoBox()
	{
		return m_ammoBox;
	}

	public BombSight GetBombSight()
	{
		return m_bombSight;
	}

	public BomberCriticalFlashAll GetCriticalFlash()
	{
		return m_criticalFlashInfo;
	}

	public void RemoveSearchLight()
	{
		m_litUpCounter--;
	}

	public Transform GetFloorUp()
	{
		return m_floorUpTransform;
	}

	public bool IsLitUp()
	{
		return m_litUpCounter > 0;
	}

	public WeatherSensor GetWeatherSensor()
	{
		return m_weatherSensor;
	}

	public AmmoFeed[] GetAmmoFeeds()
	{
		return m_ammoFeeds;
	}

	public EngineExtinguisher GetEngineExtinguisher(int index)
	{
		return m_engineExtinguishers[index];
	}

	public HydraulicsTank GetHydraulics()
	{
		return m_hydraulics;
	}

	public OxygenTank GetOxygenTank()
	{
		return m_oxygenTank;
	}

	public void RegisterInteractiveSearchable(Type forType, InteractiveItem ii)
	{
		List<InteractiveItem> value = null;
		m_searchableInteractives.TryGetValue(forType, out value);
		if (value == null)
		{
			value = new List<InteractiveItem>();
			m_searchableInteractives[forType] = value;
		}
		value.Add(ii);
	}

	public void DeRegisterInteractiveSearchable(Type forType, InteractiveItem ii)
	{
		List<InteractiveItem> value = null;
		m_searchableInteractives.TryGetValue(forType, out value);
		value?.Remove(ii);
	}

	public List<InteractiveItem> GetSearchableInteractives(Type forType)
	{
		List<InteractiveItem> value = null;
		m_searchableInteractives.TryGetValue(forType, out value);
		if (value == null)
		{
			value = new List<InteractiveItem>();
			m_searchableInteractives[forType] = value;
		}
		return value;
	}

	public GameObject GetDestroyedSectionPrefab()
	{
		return m_destroyedPartRigidBodyPrefab;
	}

	public BomberSystemUniqueId GetSystemByName(string sysName)
	{
		BomberSystemUniqueId value = null;
		m_registeredSystems.TryGetValue(sysName, out value);
		return value;
	}

	public LandingGearSingle[] GetIndividualLandingGears()
	{
		return m_landingGearSeparates;
	}

	public BomberAllWalkZones GetAllWalkZones()
	{
		return m_walkZones;
	}

	public FuelTank GetFuelTank(int index)
	{
		if (index < m_fuelTanks.Length)
		{
			return m_fuelTanks[index];
		}
		return null;
	}

	public void RemoveEngine(Engine e)
	{
		m_engines.Remove(e);
	}

	public void RegisterEngine(Engine e)
	{
		m_engines.Add(e);
		m_initialEngineCount++;
	}

	public FuelTank[] GetFuelTanks()
	{
		return m_fuelTanks;
	}

	public FireOverview GetFireOverview()
	{
		return m_fireOverview;
	}

	public LandingGear GetLandingGear()
	{
		return m_landingGear;
	}

	public List<Transform> GetTargetableAreas()
	{
		return m_bomberTargetableAreas;
	}

	public BomberNavigation GetBomberNavigation()
	{
		return m_bomberNavigation;
	}

	public BomberTemperatureOxygen GetTemperatureOxygen()
	{
		return m_temperatureOxygen;
	}

	public PhotoCamera GetPhotoCamera()
	{
		return m_photoCamera;
	}

	public Engine GetEngine(int index)
	{
		if (index < m_engines.Count)
		{
			return m_engines[index];
		}
		return null;
	}

	public Engine GetEngineOrdered(int index)
	{
		return m_enginesOrdered[index];
	}

	public Station GetStationForPrimarySkill(Crewman.SpecialisationSkill skill, List<Station> excludeList)
	{
		StationListItem[] allStations = m_allStations;
		foreach (StationListItem stationListItem in allStations)
		{
			if (stationListItem.m_skill == skill && !excludeList.Contains(stationListItem.m_station))
			{
				return stationListItem.m_station;
			}
		}
		return null;
	}

	public BomberCameraTrackNode GetCameraTrackingNode()
	{
		return m_trackingNode;
	}

	public int GetEngineCount()
	{
		return m_initialEngineCount;
	}

	public int GetStaticEngineCount()
	{
		return m_enginesOrdered.Length;
	}

	public ElectricalSystem GetElectricalSystem()
	{
		return m_electricalSystem;
	}

	public Station GetStationFor(StationType station)
	{
		StationListItem[] allStations = m_allStations;
		foreach (StationListItem stationListItem in allStations)
		{
			if (stationListItem.m_stationType == station)
			{
				return stationListItem.m_station;
			}
		}
		return null;
	}

	public Transform GetCrewHierarchy()
	{
		return m_crewHierarchy;
	}

	public BomberState GetBomberState()
	{
		return m_bomberState;
	}

	public BombBayDoors GetBombBayDoors()
	{
		return m_bombBayDoors;
	}

	public BombBay GetBombBay()
	{
		return m_bombBay;
	}

	public BombLoad GetBombLoad()
	{
		return m_bombBay.GetBombLoad();
	}
}
