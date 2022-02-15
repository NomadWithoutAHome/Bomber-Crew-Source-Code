using System;
using System.Collections.Generic;
using BomberCrewCommon;
using UnityEngine;

public class MissionMapController : Singleton<MissionMapController>
{
	public class CreatedMapObject
	{
		public MapNavigatorDisplayable m_linkedItem;

		public MissionMapAreaDisplay m_mmad;

		public bool m_hasMMad;

		public GameObject m_linkedMapDisplayPrefab;

		public bool m_everBeenSeen;

		public bool m_addedByRecon;

		public bool m_isShowing;

		public float m_lastMMadSize;
	}

	[SerializeField]
	private Transform m_cameraPositionTransform;

	[SerializeField]
	private Camera m_camera;

	[SerializeField]
	private float m_cameraZoomInSize;

	[SerializeField]
	private Vector3 m_cameraZoomOutCentre;

	[SerializeField]
	private float m_cameraZoomOutSize;

	[SerializeField]
	private Transform m_bomberPosition;

	[SerializeField]
	private float m_scale = 100f;

	[SerializeField]
	private Transform m_mapObjectHierarchy;

	[SerializeField]
	private Renderer m_mapFogRenderer;

	[SerializeField]
	private Material m_missionMapMaterial;

	private bool m_zoomedIn = true;

	private float m_fogSetting;

	private float m_timeSinceOptionalReconAdded;

	private List<CreatedMapObject> m_mapObjects = new List<CreatedMapObject>();

	public void RegisterMapObject(MapNavigatorDisplayable mnd)
	{
		CreatedMapObject createdMapObject = new CreatedMapObject();
		createdMapObject.m_linkedItem = mnd;
		createdMapObject.m_linkedMapDisplayPrefab = UnityEngine.Object.Instantiate(mnd.GetDisplayPrefab());
		createdMapObject.m_linkedMapDisplayPrefab.transform.parent = m_mapObjectHierarchy;
		createdMapObject.m_linkedMapDisplayPrefab.transform.localPosition = ToMapPosition(mnd.gameObject.btransform().position);
		MissionMapAreaDisplay component = createdMapObject.m_linkedMapDisplayPrefab.GetComponent<MissionMapAreaDisplay>();
		if (component != null)
		{
			component.SetUp(mnd.GetRadius() / m_scale);
			createdMapObject.m_hasMMad = true;
			createdMapObject.m_mmad = component;
		}
		createdMapObject.m_linkedMapDisplayPrefab.SetActive(value: false);
		m_mapObjects.Add(createdMapObject);
	}

	public void DeRegisterMapObject(MapNavigatorDisplayable mnd)
	{
		CreatedMapObject createdMapObject = null;
		foreach (CreatedMapObject mapObject in m_mapObjects)
		{
			if (mapObject.m_linkedItem == mnd)
			{
				UnityEngine.Object.Destroy(mapObject.m_linkedMapDisplayPrefab);
				createdMapObject = mapObject;
			}
		}
		if (createdMapObject != null)
		{
			m_mapObjects.Remove(createdMapObject);
		}
	}

	private void Awake()
	{
		m_zoomedIn = true;
		m_mapFogRenderer.sharedMaterial = UnityEngine.Object.Instantiate(m_mapFogRenderer.sharedMaterial);
		m_mapFogRenderer.sharedMaterial.SetColor("_Tint", new Color(1f, 1f, 1f, 0f));
		m_timeSinceOptionalReconAdded = 1000f;
	}

	public Material GetMissionMapMaterial()
	{
		return m_missionMapMaterial;
	}

	public void ToggleZoom()
	{
		m_zoomedIn = !m_zoomedIn;
	}

	public void SetZoom(bool zoomedIn)
	{
		m_zoomedIn = zoomedIn;
	}

	public bool GetIsMapZoomedIn()
	{
		return m_zoomedIn;
	}

	public int SpotFurther(float distance, float angle, int max)
	{
		List<CreatedMapObject> list = new List<CreatedMapObject>();
		foreach (CreatedMapObject mapObject in m_mapObjects)
		{
			Vector3 vector = mapObject.m_linkedItem.transform.position - Singleton<BomberSpawn>.Instance.GetBomberSystems().transform.position;
			if (!mapObject.m_everBeenSeen && !mapObject.m_addedByRecon && vector.magnitude < distance && (Vector3.Dot(vector.normalized, Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBomberState().GetPhysicsModel()
				.GetHeading()
				.normalized) < Mathf.Sin(angle * ((float)Math.PI / 180f)) || vector.magnitude < distance * 0.3f))
				{
					list.Add(mapObject);
				}
			}
			int num = 0;
			while (list.Count > 0)
			{
				int index = UnityEngine.Random.Range(0, list.Count);
				list[index].m_addedByRecon = true;
				list.RemoveAt(index);
				num++;
				if (num == max)
				{
					break;
				}
			}
			return num;
		}

		private Vector3 ToMapPosition(Vector3d worldPosition)
		{
			Vector3d vector3d = worldPosition / m_scale;
			return new Vector3((float)vector3d.z, 0f - (float)vector3d.x, 0f);
		}

		private Quaternion ToMapRotation(Vector3 forward)
		{
			Vector3 vector = new Vector3(forward.z, 0f - forward.x, 0f);
			float num = Mathf.Atan2(0f - vector.x, vector.y);
			return Quaternion.Euler(0f, 0f, num * 57.29578f);
		}

		public void SetFogSetting(float amt)
		{
			m_fogSetting = amt;
		}

		private void Update()
		{
			Vector3d position = Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBigTransform().position;
			if (m_zoomedIn)
			{
				m_cameraPositionTransform.localPosition = ToMapPosition(position);
				m_camera.orthographicSize = m_cameraZoomInSize;
			}
			else
			{
				m_cameraPositionTransform.localPosition = m_cameraZoomOutCentre;
				m_camera.orthographicSize = m_cameraZoomOutSize;
			}
			m_bomberPosition.localPosition = ToMapPosition(position);
			m_bomberPosition.rotation = ToMapRotation(Singleton<BomberSpawn>.Instance.GetBomberSystems().transform.forward);
			m_timeSinceOptionalReconAdded += Time.deltaTime;
			m_mapFogRenderer.sharedMaterial.SetColor("_Tint", new Color(1f, 1f, 1f, m_fogSetting));
			foreach (CreatedMapObject mapObject in m_mapObjects)
			{
				bool flag = false;
				if (!mapObject.m_linkedItem.RequiresVisibility())
				{
					flag = true;
				}
				else if (mapObject.m_everBeenSeen)
				{
					flag = true;
				}
				else if (Singleton<BomberSpawn>.Instance.GetBomberSystems().GetStationFor(BomberSystems.StationType.Navigation).GetCurrentCrewman() != null && (Singleton<VisibilityHelpers>.Instance.IsVisibleHumanPlayer(Singleton<BomberSpawn>.Instance.GetBomberSystems().transform.position, mapObject.m_linkedItem.transform.position, Singleton<BomberSpawn>.Instance.GetBomberSystems(), isRadarObject: false, isNavigationObject: true) || mapObject.m_addedByRecon))
				{
					mapObject.m_everBeenSeen = true;
					flag = true;
					switch (mapObject.m_linkedItem.GetDescriptionType())
					{
					case MapNavigatorDisplayable.NavigatorDescriptionType.Hazard:
						Singleton<SpeechPrioritiser>.Instance.RequestNewSpeech(StaticSpeechSets.AddedToMap_Hazard, Singleton<BomberSpawn>.Instance.GetBomberSystems().GetStationFor(BomberSystems.StationType.Navigation).GetCurrentCrewman());
						break;
					case MapNavigatorDisplayable.NavigatorDescriptionType.PhotoSite:
						Singleton<SpeechPrioritiser>.Instance.RequestNewSpeech(StaticSpeechSets.AddedToMap_PhotoSite, Singleton<BomberSpawn>.Instance.GetBomberSystems().GetStationFor(BomberSystems.StationType.Navigation).GetCurrentCrewman());
						break;
					case MapNavigatorDisplayable.NavigatorDescriptionType.PhotoSiteOptional:
						Singleton<SpeechPrioritiser>.Instance.RequestNewSpeech(StaticSpeechSets.AddedToMap_PhotoSiteOptional, Singleton<BomberSpawn>.Instance.GetBomberSystems().GetStationFor(BomberSystems.StationType.Navigation).GetCurrentCrewman());
						m_timeSinceOptionalReconAdded = 0f;
						break;
					case MapNavigatorDisplayable.NavigatorDescriptionType.Target:
						Singleton<SpeechPrioritiser>.Instance.RequestNewSpeech(StaticSpeechSets.AddedToMap_Target, Singleton<BomberSpawn>.Instance.GetBomberSystems().GetStationFor(BomberSystems.StationType.Navigation).GetCurrentCrewman());
						break;
					case MapNavigatorDisplayable.NavigatorDescriptionType.Rescue:
						Singleton<SpeechPrioritiser>.Instance.RequestNewSpeech(StaticSpeechSets.AddedToMap_Rescue, Singleton<BomberSpawn>.Instance.GetBomberSystems().GetStationFor(BomberSystems.StationType.Navigation).GetCurrentCrewman());
						break;
					}
				}
				if (mapObject.m_isShowing != flag)
				{
					mapObject.m_linkedMapDisplayPrefab.SetActive(flag);
					mapObject.m_isShowing = flag;
				}
				if (mapObject.m_hasMMad && mapObject.m_isShowing)
				{
					float num = mapObject.m_linkedItem.GetRadius() / m_scale;
					if (num != mapObject.m_lastMMadSize)
					{
						mapObject.m_mmad.SetUp(num);
						mapObject.m_lastMMadSize = num;
					}
				}
			}
		}

		public bool HasSpottedOptionalReconRecently()
		{
			return m_timeSinceOptionalReconAdded < 60f;
		}
	}
