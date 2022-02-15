using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using BomberCrewCommon;
using UnityEngine;

public class EndlessModeController : Singleton<EndlessModeController>
{
	[Serializable]
	public class WaveDefinition
	{
		[SerializeField]
		private WaveType[] m_possibleWaveTypes;

		public WaveType SelectWaveType()
		{
			return m_possibleWaveTypes[UnityEngine.Random.Range(0, m_possibleWaveTypes.Length)];
		}
	}

	[Serializable]
	public class FighterGroupDefinition
	{
		[SerializeField]
		public int m_numPoints;

		[SerializeField]
		public GameObject[] m_fighterPrefab;
	}

	[Serializable]
	public class WaveDifficultyDefinition
	{
		[SerializeField]
		public int m_maxPointsPerGroup;

		[SerializeField]
		public int m_minPointPerGroup;

		[SerializeField]
		public int m_maxPointsTotal;

		[SerializeField]
		public float m_maxTime;

		[SerializeField]
		public int m_maxBonusHeightIndex;

		[SerializeField]
		public int m_minHazards;

		[SerializeField]
		public int m_maxHazards;
	}

	public enum WaveType
	{
		FightersOnly,
		BombTargetLand,
		BombSubmarines,
		HealAndRefuel,
		BombTargetTimed,
		EnemyAceWave,
		BombTargetLandMulti,
		BombTargetLandGrandSlam
	}

	[SerializeField]
	private MissionPlaceableObject m_placeable;

	[SerializeField]
	private WaveDefinition[] m_waveCycle;

	[SerializeField]
	private WaveDifficultyDefinition[] m_waveDifficulty;

	[SerializeField]
	private GameObject m_waveAnnounceTitlePrefab;

	[SerializeField]
	private FighterGroupDefinition[] m_fighterGroupDefs;

	[SerializeField]
	private GameObject[] m_standardTargets;

	[SerializeField]
	private GameObject[] m_seaTargets;

	[SerializeField]
	private GameObject[] m_grandSlamTargets;

	[SerializeField]
	private GameObject m_missionTargetMarker;

	[SerializeField]
	private Transform m_scaleTransform;

	[SerializeField]
	private GameObject m_upgradeItemPrefab;

	[SerializeField]
	private int m_maxRegularItems;

	[SerializeField]
	private float m_timeBetweenDrops;

	[SerializeField]
	private EndlessModeUpgrade.UpgradeType[] m_upgradeCycle;

	[SerializeField]
	private GameObject m_pickUpMessagePrefab;

	[SerializeField]
	private int m_waveLoopPoint = 5;

	[SerializeField]
	private GameObject m_flakHazard;

	[SerializeField]
	private GameObject m_aaHazard;

	[SerializeField]
	private int m_maxBonusesPerWave;

	[SerializeField]
	private int m_completeWaveScoreStandard;

	[SerializeField]
	private int m_completeWaveScoreIncreasePerWave;

	[SerializeField]
	private int m_timeBonusMax;

	[SerializeField]
	private float m_timeBonusTimeForMax;

	[SerializeField]
	private float m_timeBonusTimeForMin;

	[SerializeField]
	private GameObject m_spawnPrefab;

	[SerializeField]
	private bool m_snowInsteadOfRain;

	[SerializeField]
	private float[] m_areaScaleSettings;

	[SerializeField]
	private GameObject m_areaExpandingPrefab;

	[SerializeField]
	private GameObject m_getReadyPrefab;

	[SerializeField]
	private BombLoadConfig m_standardBombLoadConfig;

	[SerializeField]
	private BombLoadConfig m_threeTargetBombLoadConfig;

	[SerializeField]
	private BombLoadConfig m_grandSlamConfig;

	[SerializeField]
	private EnemyFighterAce[] m_overrideAceOrder;

	[SerializeField]
	private int[] m_pointsValues;

	[SerializeField]
	private int m_multiTargetCount = 3;

	[SerializeField]
	private int m_maxSubs = 5;

	private MissionLog.LogObjective m_objective;

	private float m_squareRadius;

	private List<EndlessModeArea> m_allAreas = new List<EndlessModeArea>();

	private bool m_waveFailed;

	private int m_aceIndex;

	private int m_maxBonusesToSpawn;

	private int m_maxBonusAltitude;

	private float m_waveTimer;

	private List<GameObject> m_currentHazards = new List<GameObject>();

	private List<EndlessModeArea> m_currentlyUsedAreas = new List<EndlessModeArea>();

	private List<EndlessModeArea> m_hazardAreas = new List<EndlessModeArea>();

	private int m_spawnPrefabInstanceId;

	private float m_currentAreaSizeScale;

	private float m_finalAreaSize;

	private bool m_crewReachedMaxLevel;

	private int m_numLevelUpCollections;

	private bool m_nextWaveForced;

	private WaveType m_forcedWave;

	public bool CrewAreAtMaxLevel()
	{
		return m_crewReachedMaxLevel;
	}

	public void CollectedLevelUp()
	{
		m_numLevelUpCollections++;
		if (m_numLevelUpCollections >= 11)
		{
			m_crewReachedMaxLevel = true;
		}
	}

	private void DoSpawnEffect(GameObject go)
	{
		GameObject fromPool = Singleton<PoolManager>.Instance.GetFromPool(m_spawnPrefab, m_spawnPrefabInstanceId);
		fromPool.btransform().SetFromCurrentPage(go.transform.position);
	}

	public void SetAreaSize(float scale)
	{
		m_currentAreaSizeScale = scale;
		m_squareRadius = m_finalAreaSize * scale;
		Vector3 vector = (Vector3)base.gameObject.btransform().position;
		Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBomberState().SetBounds(vector - new Vector3(m_squareRadius, 0f, m_squareRadius), vector + new Vector3(m_squareRadius, 0f, m_squareRadius));
		Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBomberState().SetBoundsStrict();
		m_scaleTransform.transform.localScale = new Vector3(m_squareRadius + 600f, 5000f, m_squareRadius + 600f);
		m_scaleTransform.transform.localPosition = new Vector3(0f, 2500f, 0f);
	}

	private void Start()
	{
		m_spawnPrefabInstanceId = m_spawnPrefab.GetInstanceID();
		Singleton<PoolManager>.Instance.PoolPrefab(m_spawnPrefab, 10);
		Singleton<EndlessModeUI>.Instance.SetEndlessModeActive();
		Singleton<EndlessModeUI>.Instance.SetWave(0);
		m_objective = new MissionLog.LogObjective();
		Singleton<GameFlow>.Instance.GetCurrentMissionInfo().GetMissionLog().LogObjectiveNew(m_objective);
		ObjectiveManager.Objective objective = new ObjectiveManager.Objective();
		objective.m_countToComplete = 1;
		objective.m_objectiveTitle = "endlessmode_objective";
		objective.m_primary = true;
		objective.m_objectiveType = ObjectiveManager.ObjectiveType.None;
		Singleton<ObjectiveManager>.Instance.RegisterObjective(objective);
		m_finalAreaSize = (float)Convert.ToDouble(m_placeable.GetParameter("radius"), CultureInfo.InvariantCulture);
		SetAreaSize(m_areaScaleSettings[0]);
		Singleton<CloudGrid>.Instance.SetAlwaysSlow();
		StartCoroutine(DoEndlessMode());
		StartCoroutine(DoItemDrops());
		int staticEngineCount = Singleton<BomberSpawn>.Instance.GetBomberSystems().GetStaticEngineCount();
		for (int i = 0; i < staticEngineCount; i++)
		{
			Engine engineOrdered = Singleton<BomberSpawn>.Instance.GetBomberSystems().GetEngineOrdered(i);
			if (engineOrdered != null)
			{
				engineOrdered.GetComponent<BomberDestroyableSection>().SetLives(4);
			}
		}
	}

	private void EndlessDebug()
	{
		foreach (Enum value in Enum.GetValues(typeof(WaveType)))
		{
			if (GUILayout.Button(value.ToString()))
			{
				m_nextWaveForced = true;
				m_forcedWave = (WaveType)(object)value;
			}
		}
	}

	private void OnDestroy()
	{
	}

	private void Update()
	{
		Singleton<EndlessModeUI>.Instance.SetScore(Singleton<EndlessModeGameFlow>.Instance.GetEndlessModeResult().m_totalScore);
		m_waveTimer += Time.deltaTime;
	}

	private IEnumerator DoItemDrops()
	{
		List<GameObject> m_currentItemDrops = new List<GameObject>();
		int upgradeIndex = 0;
		float timer = 0f;
		int droppedEver = 0;
		while (true)
		{
			timer -= Time.deltaTime;
			if (timer < 0f)
			{
				while (m_currentItemDrops.Contains(null))
				{
					m_currentItemDrops.Remove(null);
				}
				if (m_currentItemDrops.Count < m_maxRegularItems && droppedEver < m_maxBonusesToSpawn)
				{
					timer = m_timeBetweenDrops;
					GameObject gameObject = UnityEngine.Object.Instantiate(m_upgradeItemPrefab);
					gameObject.transform.parent = base.transform;
					gameObject.transform.localPosition = new Vector3(UnityEngine.Random.Range(0f - m_squareRadius, m_squareRadius), 0f, UnityEngine.Random.Range(0f - m_squareRadius, m_squareRadius));
					EndlessModeUpgrade.UpgradeType upgradeType = m_upgradeCycle[upgradeIndex % m_upgradeCycle.Length];
					if (upgradeType == EndlessModeUpgrade.UpgradeType.XPUpgrade && CrewAreAtMaxLevel())
					{
						upgradeType = EndlessModeUpgrade.UpgradeType.Points;
					}
					int num = UnityEngine.Random.Range(0, m_maxBonusAltitude + 1);
					gameObject.GetComponent<EndlessModeUpgrade>().SetUpgrade(upgradeType, num);
					if (upgradeType == EndlessModeUpgrade.UpgradeType.Points)
					{
						int isPoints = m_pointsValues[num];
						gameObject.GetComponent<EndlessModeUpgrade>().SetIsPoints(isPoints);
					}
					upgradeIndex++;
					droppedEver++;
				}
			}
			yield return null;
		}
	}

	public void SpawnPickUpMessage(string spriteName, string pickUpName)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(m_pickUpMessagePrefab);
		gameObject.GetComponent<EndlessModePickUpMessage>().SetUp(spriteName, pickUpName);
	}

	public void SpawnPickUpMessage(string pickUpName, int score)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(m_pickUpMessagePrefab);
		gameObject.GetComponent<EndlessModePickUpMessage>().SetUp(pickUpName, score);
	}

	private IEnumerator DoEndlessMode()
	{
		while (Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBomberState().HasNotStartedTakeOff())
		{
			yield return null;
		}
		GameObject getReadyObject = UnityEngine.Object.Instantiate(m_getReadyPrefab);
		while (Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBomberState().HasNotCompletedTakeOff())
		{
			yield return null;
		}
		int waveNumber = 0;
		int waveLoopNumber = 0;
		while (true)
		{
			WaveDefinition wd = m_waveCycle[waveLoopNumber];
			WaveDifficultyDefinition wdd = m_waveDifficulty[Mathf.Min(m_waveDifficulty.Length - 1, waveNumber)];
			yield return StartCoroutine(DoWave(waveNumber, wd, wdd));
			yield return null;
			waveNumber++;
			waveLoopNumber++;
			if (waveLoopNumber == m_waveCycle.Length)
			{
				waveLoopNumber = m_waveLoopPoint;
			}
			float newScale = m_areaScaleSettings[Mathf.Min(waveNumber, m_areaScaleSettings.Length - 1)];
			if (newScale > m_currentAreaSizeScale)
			{
				GameObject expandObject = UnityEngine.Object.Instantiate(m_areaExpandingPrefab);
				float t = 0f;
				float startScale = m_currentAreaSizeScale;
				while (t < 1f)
				{
					t += Time.deltaTime * 0.125f;
					SetAreaSize(Mathf.Lerp(startScale, newScale, t));
					yield return null;
				}
				SetAreaSize(newScale);
			}
		}
	}

	private void ClearHazards()
	{
		foreach (GameObject currentHazard in m_currentHazards)
		{
			DoSpawnEffect(currentHazard);
			UnityEngine.Object.Destroy(currentHazard);
		}
		m_currentHazards.Clear();
		foreach (EndlessModeArea hazardArea in m_hazardAreas)
		{
			DeregisterArea(hazardArea);
		}
		m_hazardAreas.Clear();
	}

	private void SpawnFlak(EndlessModeArea area)
	{
		Vector3d position = area.gameObject.btransform().position;
		float squareRadius = area.GetSquareRadius();
		GameObject gameObject = UnityEngine.Object.Instantiate(m_flakHazard);
		Vector3d position2 = position + new Vector3d(UnityEngine.Random.Range(0f - squareRadius, squareRadius), 0f, UnityEngine.Random.Range(0f - squareRadius, squareRadius));
		List<LevelDescription.LevelParameter> list = new List<LevelDescription.LevelParameter>();
		LevelDescription.LevelParameter levelParameter = new LevelDescription.LevelParameter();
		levelParameter.m_key = "density";
		levelParameter.m_value = "1";
		list.Add(levelParameter);
		levelParameter = new LevelDescription.LevelParameter();
		levelParameter.m_key = "radius";
		levelParameter.m_value = "3000";
		list.Add(levelParameter);
		if (UnityEngine.Random.Range(0, 2) == 0)
		{
			levelParameter = new LevelDescription.LevelParameter();
			levelParameter.m_key = "height";
			levelParameter.m_value = "425";
			list.Add(levelParameter);
			levelParameter = new LevelDescription.LevelParameter();
			levelParameter.m_key = "altitude";
			levelParameter.m_value = "525";
			list.Add(levelParameter);
		}
		else
		{
			levelParameter = new LevelDescription.LevelParameter();
			levelParameter.m_key = "height";
			levelParameter.m_value = "320";
			list.Add(levelParameter);
			levelParameter = new LevelDescription.LevelParameter();
			levelParameter.m_key = "altitude";
			levelParameter.m_value = "0";
			list.Add(levelParameter);
		}
		gameObject.GetComponent<MissionPlaceableObject>().SetPosition(position2, Vector3.one, list, Quaternion.identity);
		DoSpawnEffect(gameObject);
		m_currentHazards.Add(gameObject);
	}

	private void SpawnAA(EndlessModeArea area)
	{
		Vector3d position = area.gameObject.btransform().position;
		float squareRadius = area.GetSquareRadius();
		for (int i = 0; i < 4; i++)
		{
			float num = (float)i / 9f * (float)Math.PI;
			GameObject gameObject = UnityEngine.Object.Instantiate(m_aaHazard);
			Vector3d position2 = position + new Vector3d(Mathf.Sin(num * (float)Math.PI) * squareRadius * 0.9f, 0f, Mathf.Cos(num * (float)Math.PI) * squareRadius * 0.9f);
			List<LevelDescription.LevelParameter> parameters = new List<LevelDescription.LevelParameter>();
			gameObject.GetComponent<MissionPlaceableObject>().SetPosition(position2, Vector3.one, parameters, Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f));
			DoSpawnEffect(gameObject);
			m_currentHazards.Add(gameObject);
		}
	}

	private void SpawnHazards(int numHazards)
	{
		ClearHazards();
		for (int i = 0; i < numHazards; i++)
		{
			EndlessModeArea endlessModeArea = RegisterArea(land: true);
			m_hazardAreas.Add(endlessModeArea);
			if (UnityEngine.Random.Range(0, 2) == 0)
			{
				SpawnFlak(endlessModeArea);
			}
			else
			{
				SpawnAA(endlessModeArea);
			}
		}
	}

	private void ChangeWeather(bool allowCloudy)
	{
		Singleton<CloudGrid>.Instance.Clear();
		if (allowCloudy)
		{
			int num = UnityEngine.Random.Range(0, 10);
			if (num < 4)
			{
				float density = UnityEngine.Random.Range(0.1f, 0.9f);
				Singleton<CloudGrid>.Instance.RegisterOverall(density, rain: false, snow: false);
			}
			else if (num >= 8)
			{
				float density2 = UnityEngine.Random.Range(0.1f, 0.8f);
				if (m_snowInsteadOfRain)
				{
					Singleton<CloudGrid>.Instance.RegisterOverall(density2, rain: false, snow: true);
				}
				else
				{
					Singleton<CloudGrid>.Instance.RegisterOverall(density2, rain: true, snow: false);
				}
			}
		}
		else
		{
			float density3 = UnityEngine.Random.Range(0.1f, 0.25f);
			Singleton<CloudGrid>.Instance.RegisterOverall(density3, rain: false, snow: false);
		}
	}

	private bool AreSeaAreasAvailable()
	{
		Vector3 position = base.transform.position;
		foreach (EndlessModeArea allArea in m_allAreas)
		{
			if (!allArea.IsLand())
			{
				Vector3 vector = allArea.transform.position - position;
				float num = Mathf.Sqrt(allArea.GetSquareRadius() * allArea.GetSquareRadius() * 2f);
				if (Mathf.Abs(vector.x) + num < m_squareRadius && Mathf.Abs(vector.z) + num < m_squareRadius)
				{
					return true;
				}
			}
		}
		return false;
	}

	private EndlessModeArea RegisterArea(bool land)
	{
		List<EndlessModeArea> list = new List<EndlessModeArea>();
		Vector3 position = base.transform.position;
		foreach (EndlessModeArea allArea in m_allAreas)
		{
			if (allArea.IsLand() == land && !m_currentlyUsedAreas.Contains(allArea))
			{
				Vector3 vector = allArea.transform.position - position;
				float num = Mathf.Sqrt(allArea.GetSquareRadius() * allArea.GetSquareRadius() * 2f);
				if (Mathf.Abs(vector.x) + num < m_squareRadius && Mathf.Abs(vector.z) + num < m_squareRadius)
				{
					list.Add(allArea);
				}
			}
		}
		if (list.Count > 0)
		{
			EndlessModeArea endlessModeArea = list[UnityEngine.Random.Range(0, list.Count)];
			m_currentlyUsedAreas.Add(endlessModeArea);
			return endlessModeArea;
		}
		return null;
	}

	private void DeregisterArea(EndlessModeArea ema)
	{
		m_currentlyUsedAreas.Remove(ema);
	}

	private IEnumerator DoWave(int index, WaveDefinition wd, WaveDifficultyDefinition wdd)
	{
		WaveType wt = wd.SelectWaveType();
		if (wt == WaveType.BombSubmarines && !AreSeaAreasAvailable())
		{
			wt = WaveType.BombTargetLand;
		}
		if (m_nextWaveForced)
		{
			wt = m_forcedWave;
			m_nextWaveForced = false;
		}
		GameObject announcement = UnityEngine.Object.Instantiate(m_waveAnnounceTitlePrefab);
		announcement.GetComponent<EndlessModeWaveDisplay>().SetUp(string.Format(Singleton<LanguageProvider>.Instance.GetNamedTextImmediate("endlessmode_wave_title"), index + 1), Singleton<LanguageProvider>.Instance.GetNamedTextImmediate("endlessmode_wave_type_" + wt));
		Singleton<EndlessModeUI>.Instance.SetWave(index);
		Singleton<EndlessModeGameFlow>.Instance.GetEndlessModeResult().m_waveReached++;
		m_waveFailed = false;
		m_maxBonusAltitude = wdd.m_maxBonusHeightIndex;
		SpawnHazards(UnityEngine.Random.Range(wdd.m_minHazards, wdd.m_maxHazards + 1));
		if (wt == WaveType.BombTargetLandGrandSlam)
		{
			ChangeWeather(allowCloudy: false);
		}
		else
		{
			ChangeWeather(allowCloudy: true);
		}
		m_maxBonusesToSpawn += m_maxBonusesPerWave;
		m_waveTimer = 0f;
		switch (wt)
		{
		case WaveType.FightersOnly:
			Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBombBay().SetBombLoadType(m_standardBombLoadConfig);
			yield return StartCoroutine(DoFightersOnlyWave(wdd, spawnAce: false));
			break;
		case WaveType.BombTargetLand:
			yield return StartCoroutine(DoBombTargetWave(wdd, 1, onLand: true, 0f, grandSlam: false));
			break;
		case WaveType.BombTargetTimed:
			yield return StartCoroutine(DoBombTargetWave(wdd, 1, onLand: true, 180f, grandSlam: false));
			break;
		case WaveType.EnemyAceWave:
			Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBombBay().SetBombLoadType(m_standardBombLoadConfig);
			yield return StartCoroutine(DoFightersOnlyWave(wdd, spawnAce: true));
			break;
		case WaveType.HealAndRefuel:
			Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBombBay().SetBombLoadType(m_standardBombLoadConfig);
			yield return StartCoroutine(DoRefuelWave(120f));
			break;
		case WaveType.BombSubmarines:
			yield return StartCoroutine(DoBombTargetWave(wdd, UnityEngine.Random.Range(2, m_maxSubs), onLand: false, 0f, grandSlam: false));
			break;
		case WaveType.BombTargetLandMulti:
			yield return StartCoroutine(DoBombTargetWave(wdd, m_multiTargetCount, onLand: true, 0f, grandSlam: false));
			break;
		case WaveType.BombTargetLandGrandSlam:
			yield return StartCoroutine(DoBombTargetWave(wdd, 1, onLand: true, 0f, grandSlam: true));
			break;
		}
		if (m_waveFailed)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(m_waveAnnounceTitlePrefab);
			gameObject.GetComponent<EndlessModeWaveDisplay>().SetUpFail(string.Format(Singleton<LanguageProvider>.Instance.GetNamedTextImmediate("endlessmode_wave_title"), index + 1), Singleton<LanguageProvider>.Instance.GetNamedTextImmediate("endlessmode_wave_type_" + wt));
		}
		else
		{
			int mainScore = m_completeWaveScoreStandard + m_completeWaveScoreIncreasePerWave * index;
			float value = Mathf.InverseLerp(m_timeBonusTimeForMax, m_timeBonusTimeForMin, m_waveTimer);
			float f = Mathf.Lerp(0f, m_timeBonusMax, 1f - Mathf.Clamp01(value));
			int timeBonusScore = Mathf.RoundToInt(f);
			GameObject gameObject2 = UnityEngine.Object.Instantiate(m_waveAnnounceTitlePrefab);
			gameObject2.GetComponent<EndlessModeWaveDisplay>().SetUpComplete(string.Format(Singleton<LanguageProvider>.Instance.GetNamedTextImmediate("endlessmode_wave_title"), index + 1), Singleton<LanguageProvider>.Instance.GetNamedTextImmediate("endlessmode_wave_type_" + wt), mainScore, timeBonusScore);
			Singleton<EndlessModeGameFlow>.Instance.GetEndlessModeResult().m_wavesActuallyCompleted++;
		}
		yield return new WaitForSeconds(15f);
		ClearHazards();
	}

	private IEnumerator DoRefuelWave(float timer)
	{
		float countdown = timer;
		List<GameObject> toDestroy = new List<GameObject>();
		ObjectiveManager.Objective timerObjective = new ObjectiveManager.Objective
		{
			m_primary = true,
			m_countToComplete = 1,
			m_objectiveType = ObjectiveManager.ObjectiveType.Other,
			m_objectiveTitle = "mission_objective_complete_before"
		};
		int tSecs2 = (int)Mathf.Max(countdown, 0f);
		int mins2 = Mathf.FloorToInt(countdown / 60f);
		int seconds2 = tSecs2 - mins2 * 60;
		timerObjective.m_substitutionString = $"{mins2}:{seconds2:00}";
		Singleton<ObjectiveManager>.Instance.RegisterObjective(timerObjective);
		for (int i = 0; i < 20; i++)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(m_upgradeItemPrefab);
			gameObject.transform.parent = base.transform;
			gameObject.transform.localPosition = new Vector3(UnityEngine.Random.Range(0f - m_squareRadius, m_squareRadius), 0f, UnityEngine.Random.Range(0f - m_squareRadius, m_squareRadius));
			gameObject.GetComponent<EndlessModeUpgrade>().SetUpgrade(EndlessModeUpgrade.UpgradeType.FuelRefill, UnityEngine.Random.Range(0, m_maxBonusAltitude + 1));
			gameObject.GetComponent<EndlessModeUpgrade>().SetTimer(countdown - 1f);
		}
		for (int j = 0; j < 10; j++)
		{
			GameObject gameObject2 = UnityEngine.Object.Instantiate(m_upgradeItemPrefab);
			gameObject2.transform.parent = base.transform;
			gameObject2.transform.localPosition = new Vector3(UnityEngine.Random.Range(0f - m_squareRadius, m_squareRadius), 0f, UnityEngine.Random.Range(0f - m_squareRadius, m_squareRadius));
			gameObject2.GetComponent<EndlessModeUpgrade>().SetUpgrade(EndlessModeUpgrade.UpgradeType.Repair, UnityEngine.Random.Range(0, m_maxBonusAltitude + 1));
			gameObject2.GetComponent<EndlessModeUpgrade>().SetTimer(countdown - 1f);
		}
		for (int k = 0; k < 5; k++)
		{
			GameObject gameObject3 = UnityEngine.Object.Instantiate(m_upgradeItemPrefab);
			gameObject3.transform.parent = base.transform;
			gameObject3.transform.localPosition = new Vector3(UnityEngine.Random.Range(0f - m_squareRadius, m_squareRadius), 0f, UnityEngine.Random.Range(0f - m_squareRadius, m_squareRadius));
			gameObject3.GetComponent<EndlessModeUpgrade>().SetUpgrade(EndlessModeUpgrade.UpgradeType.HealCrew, UnityEngine.Random.Range(0, m_maxBonusAltitude + 1));
			gameObject3.GetComponent<EndlessModeUpgrade>().SetTimer(countdown - 1f);
		}
		while (countdown > 0f)
		{
			countdown -= Time.deltaTime;
			tSecs2 = (int)Mathf.Max(countdown, 0f);
			mins2 = Mathf.FloorToInt(countdown / 60f);
			seconds2 = tSecs2 - mins2 * 60;
			if (mins2 < 0)
			{
				mins2 = 0;
				seconds2 = 0;
			}
			timerObjective.m_substitutionString = $"{mins2}:{seconds2:00}";
			yield return null;
		}
		Singleton<ObjectiveManager>.Instance.CompleteObjective(timerObjective);
		yield return new WaitForSeconds(2f);
		Singleton<ObjectiveManager>.Instance.RemoveObjective(timerObjective);
	}

	private IEnumerator DoBombTargetWave(WaveDifficultyDefinition wdd, int numTargets, bool onLand, float timer, bool grandSlam)
	{
		EndlessModeArea selectedArea = RegisterArea(onLand);
		float sqR = selectedArea.GetSquareRadius();
		Vector3d ctrPosition = selectedArea.gameObject.btransform().position;
		List<GameObject> toDestroy = new List<GameObject>();
		List<LevelDescription.LevelParameter> standardParameters = new List<LevelDescription.LevelParameter>();
		GameObject wayPoint = UnityEngine.Object.Instantiate(m_missionTargetMarker);
		wayPoint.GetComponent<MissionPlaceableObject>().SetPosition(ctrPosition, Vector3.one, standardParameters, Quaternion.identity);
		Singleton<MissionCoordinator>.Instance.RegisterExternallySpawnedMissionPlaceableObject(wayPoint, "MissionTarget");
		toDestroy.Add(wayPoint);
		float startAngle = UnityEngine.Random.Range(0f, (float)Math.PI * 2f);
		GameObject[] targets = m_standardTargets;
		if (!onLand)
		{
			targets = m_seaTargets;
		}
		if (grandSlam)
		{
			targets = m_grandSlamTargets;
		}
		int targetSelection = UnityEngine.Random.Range(0, targets.Length);
		for (int i = 0; i < numTargets; i++)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(targets[targetSelection]);
			Vector3d vector3d = new Vector3d(Mathf.Sin(startAngle), 0f, Mathf.Cos(startAngle)) * sqR * 0.800000011920929;
			Vector3d position = ctrPosition + vector3d;
			gameObject.GetComponent<MissionPlaceableObject>().SetPosition(position, Vector3.one, standardParameters, Quaternion.identity);
			DoSpawnEffect(gameObject);
			toDestroy.Add(gameObject);
			startAngle += (float)Math.PI / 2f;
		}
		yield return null;
		if (grandSlam)
		{
			Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBombBay().SetBombLoadType(m_grandSlamConfig);
			TopBarInfoQueue.TopBarRequest tbr = TopBarInfoQueue.TopBarRequest.Standard("mission_speech_grandslam_neartarget", null, "Icon_BombAiming", 0f, 10f, 0f, 5);
			Singleton<TopBarInfoQueue>.Instance.RegisterRequest(tbr);
		}
		else if (onLand)
		{
			if (numTargets == 3)
			{
				Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBombBay().SetBombLoadType(m_threeTargetBombLoadConfig);
			}
			else
			{
				Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBombBay().SetBombLoadType(m_standardBombLoadConfig);
			}
		}
		else
		{
			Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBombBay().SetBombLoadType(m_standardBombLoadConfig);
		}
		List<ObjectiveManager.Objective> allObjectives = Singleton<ObjectiveManager>.Instance.GetCurrentObjectives();
		List<ObjectiveManager.Objective> allTargetObjectives = new List<ObjectiveManager.Objective>();
		foreach (ObjectiveManager.Objective item in allObjectives)
		{
			if (item.m_objectiveType == ObjectiveManager.ObjectiveType.BombTarget)
			{
				allTargetObjectives.Add(item);
			}
		}
		float timeCountdown = timer;
		ObjectiveManager.Objective timerObjective = null;
		if (timer > 0f)
		{
			timerObjective = new ObjectiveManager.Objective
			{
				m_primary = true,
				m_countToComplete = 1,
				m_objectiveType = ObjectiveManager.ObjectiveType.Other,
				m_objectiveTitle = "mission_objective_complete_before"
			};
			int num = (int)Mathf.Max(timeCountdown, 0f);
			int num2 = Mathf.FloorToInt(timeCountdown / 60f);
			int num3 = num - num2 * 60;
			timerObjective.m_substitutionString = $"{num2}:{num3:00}";
			Singleton<ObjectiveManager>.Instance.RegisterObjective(timerObjective);
		}
		float waveSpawnTimer = 0f;
		int numPointsUsed = 0;
		bool allComplete;
		bool allFailed;
		do
		{
			allComplete = true;
			allFailed = true;
			foreach (ObjectiveManager.Objective item2 in allTargetObjectives)
			{
				if (!item2.m_complete)
				{
					allComplete = false;
				}
				if (!item2.m_failed)
				{
					allFailed = false;
				}
			}
			waveSpawnTimer -= Time.deltaTime;
			if (waveSpawnTimer < 0f)
			{
				waveSpawnTimer = 60f;
				int num4 = SpawnWaveOfEnemies(wdd, numPointsUsed);
				if (num4 != -1)
				{
					numPointsUsed += num4;
				}
				else
				{
					waveSpawnTimer = 180f;
					numPointsUsed = 0;
				}
			}
			if (timerObjective != null)
			{
				timeCountdown -= Time.deltaTime;
				int num5 = (int)Mathf.Max(timeCountdown, 0f);
				int num6 = Mathf.FloorToInt(timeCountdown / 60f);
				int num7 = num5 - num6 * 60;
				if (num6 < 0)
				{
					num6 = 0;
					num7 = 0;
				}
				timerObjective.m_substitutionString = $"{num6}:{num7:00}";
				if (timeCountdown < 0f)
				{
					Singleton<ObjectiveManager>.Instance.FailObjectivesOfType(ObjectiveManager.ObjectiveType.BombTarget);
					Singleton<ObjectiveManager>.Instance.FailObjective(timerObjective);
					allFailed = true;
				}
			}
			yield return null;
		}
		while (!allComplete && !allFailed);
		if (allFailed)
		{
			m_waveFailed = true;
			if (timerObjective != null)
			{
				Singleton<ObjectiveManager>.Instance.FailObjective(timerObjective);
			}
		}
		else if (timerObjective != null)
		{
			Singleton<ObjectiveManager>.Instance.CompleteObjective(timerObjective);
		}
		yield return new WaitForSeconds(2f);
		foreach (ObjectiveManager.Objective item3 in allTargetObjectives)
		{
			Singleton<ObjectiveManager>.Instance.RemoveObjective(item3);
		}
		if (timerObjective != null)
		{
			Singleton<ObjectiveManager>.Instance.RemoveObjective(timerObjective);
		}
		int index = 0;
		foreach (GameObject item4 in toDestroy)
		{
			if (index != 0)
			{
				DoSpawnEffect(item4);
			}
			UnityEngine.Object.Destroy(item4);
			index++;
		}
		DeregisterArea(selectedArea);
	}

	private int SpawnWaveOfEnemies(WaveDifficultyDefinition wdd, int numPointsUsed)
	{
		List<FighterGroupDefinition> list = new List<FighterGroupDefinition>();
		List<FighterGroupDefinition> list2 = new List<FighterGroupDefinition>();
		FighterGroupDefinition[] fighterGroupDefs = m_fighterGroupDefs;
		foreach (FighterGroupDefinition fighterGroupDefinition in fighterGroupDefs)
		{
			if (fighterGroupDefinition.m_numPoints <= wdd.m_maxPointsPerGroup && fighterGroupDefinition.m_numPoints + numPointsUsed <= wdd.m_maxPointsTotal)
			{
				if (fighterGroupDefinition.m_numPoints >= wdd.m_minPointPerGroup)
				{
					list.Add(fighterGroupDefinition);
				}
				else
				{
					list2.Add(fighterGroupDefinition);
				}
			}
		}
		if (list.Count == 0)
		{
			if (list2.Count == 0)
			{
				return -1;
			}
			list = list2;
		}
		FighterGroupDefinition fighterGroupDefinition2 = list[UnityEngine.Random.Range(0, list.Count)];
		SpawnFighterWave(fighterGroupDefinition2);
		return fighterGroupDefinition2.m_numPoints;
	}

	private IEnumerator DoFightersOnlyWave(WaveDifficultyDefinition wdd, bool spawnAce)
	{
		if (spawnAce)
		{
			EnemyFighterAce[] array = Singleton<EnemyFighterAcesCatalogueLoader>.Instance.GetCatalogue().GetEnemyFighterAces();
			if (m_overrideAceOrder != null && m_overrideAceOrder.Length != 0)
			{
				array = m_overrideAceOrder;
			}
			int num = Mathf.Clamp(m_aceIndex / array.Length + 1, 1, 4);
			for (int i = 0; i < num; i++)
			{
				Vector3d position = Singleton<BomberSpawn>.Instance.GetBomberSystems().gameObject.btransform().position;
				Vector3d vector3d = new Vector3d(UnityEngine.Random.Range(-1f, 1f), 0f, UnityEngine.Random.Range(-1f, 1f));
				if (vector3d.x == 0.0 && vector3d.y == 0.0)
				{
					vector3d.x = 1.0;
				}
				Vector3d position2 = position + vector3d.normalized * 2500.0;
				if (position2.y < 250.0)
				{
					position2.y = 250.0;
				}
				EnemyFighterAce enemyFighterAce = array[m_aceIndex % array.Length];
				GameObject gameObject = UnityEngine.Object.Instantiate(enemyFighterAce.GetInMissionPrefab());
				gameObject.btransform().position = position2;
				gameObject.GetComponent<AceFighterInMission>().SetUp(enemyFighterAce, null);
				m_aceIndex++;
			}
		}
		int numPointsUsed = 0;
		while (numPointsUsed < wdd.m_maxPointsTotal)
		{
			int numThisWave = SpawnWaveOfEnemies(wdd, numPointsUsed);
			if (numThisWave == -1)
			{
				break;
			}
			numPointsUsed += numThisWave;
			yield return new WaitForSeconds(30f);
		}
		while (Singleton<FighterCoordinator>.Instance.AreAnyFightersEngaged())
		{
			yield return null;
		}
		yield return new WaitForSeconds(2f);
		List<ObjectiveManager.Objective> allObjectives = Singleton<ObjectiveManager>.Instance.GetCurrentObjectives();
		List<ObjectiveManager.Objective> toRemove = new List<ObjectiveManager.Objective>();
		foreach (ObjectiveManager.Objective item in allObjectives)
		{
			if (item.m_objectiveType == ObjectiveManager.ObjectiveType.Ace)
			{
				toRemove.Add(item);
			}
		}
		foreach (ObjectiveManager.Objective item2 in toRemove)
		{
			Singleton<ObjectiveManager>.Instance.RemoveObjective(item2);
		}
	}

	private void SpawnFighterWave(FighterGroupDefinition fgd)
	{
		FighterWing fighterWing = new FighterWing();
		int num = fgd.m_fighterPrefab.Length;
		Vector3d position = Singleton<BomberSpawn>.Instance.GetBomberSystems().gameObject.btransform().position;
		Vector3d vector3d = new Vector3d(UnityEngine.Random.Range(-1f, 1f), 0f, UnityEngine.Random.Range(-1f, 1f));
		if (vector3d.x == 0.0 && vector3d.y == 0.0)
		{
			vector3d.x = 1.0;
		}
		Vector3d vector3d2 = position + vector3d.normalized * 2500.0;
		if (vector3d2.y < 250.0)
		{
			vector3d2.y = 250.0;
		}
		for (int i = 0; i < num; i++)
		{
			GameObject gameObject = fgd.m_fighterPrefab[i];
			if (gameObject != null)
			{
				GameObject gameObject2 = UnityEngine.Object.Instantiate(gameObject);
				gameObject2.transform.parent = null;
				gameObject2.transform.localScale = gameObject.transform.localScale;
				gameObject2.btransform().position = vector3d2 + new Vector3d(i * 50, 0f, 0f);
				gameObject2.GetComponent<FighterPlane>().SetFromArea(fighterWing, shouldHunt: true);
			}
		}
		Singleton<FighterCoordinator>.Instance.RegisterWing(fighterWing);
	}

	public void RegisterArea(EndlessModeArea ema)
	{
		m_allAreas.Add(ema);
	}
}
