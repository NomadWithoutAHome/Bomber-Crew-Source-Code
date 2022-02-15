using System;
using System.Collections.Generic;
using BomberCrewCommon;
using UnityEngine;

public class FishpondRadar : Singleton<FishpondRadar>
{
	private class RadarBlipTracked
	{
		public GameObject m_createdObject;

		public RadarBlip m_displayComponent;

		public TaggableItem m_taggableItem;
	}

	[SerializeField]
	private GameObject m_blipPrefab;

	[SerializeField]
	private float m_rimDistanceMult = 0.95f;

	[SerializeField]
	private float m_arrowHeightThreshold = 300f;

	[SerializeField]
	private Transform m_blipHierarchy;

	[SerializeField]
	private float m_radarScale;

	[SerializeField]
	private GameObject m_activeHierarchy;

	[SerializeField]
	private GameObject m_statusLightElectricsOut;

	[SerializeField]
	private Transform m_rotateHierarchy;

	[SerializeField]
	private Transform m_bomberRotationHierarchy;

	[SerializeField]
	private float m_sweepAngleThickness;

	[SerializeField]
	private Transform m_sweepRotationNode;

	private List<RadarBlipTracked> m_allRadarBlips = new List<RadarBlipTracked>();

	private StationRadioOperator m_radioOpStation;

	private float m_currentSweepAngle;

	private int m_currentFighters;

	private int m_currentLandTargets;

	private bool m_currentlyActive;

	private bool m_electricsOutActive;

	private int m_blipPrefabInstanceId;

	private void Start()
	{
		m_blipPrefabInstanceId = m_blipPrefab.GetInstanceID();
		m_radioOpStation = (StationRadioOperator)Singleton<BomberSpawn>.Instance.GetBomberSystems().GetStationFor(BomberSystems.StationType.RadioOperator);
	}

	private RadarBlipTracked GetTrackerFor(TaggableItem ti, bool allowCreate)
	{
		foreach (RadarBlipTracked allRadarBlip in m_allRadarBlips)
		{
			if (allRadarBlip.m_taggableItem == ti)
			{
				return allRadarBlip;
			}
		}
		if (allowCreate)
		{
			RadarBlipTracked radarBlipTracked = new RadarBlipTracked();
			radarBlipTracked.m_taggableItem = ti;
			radarBlipTracked.m_createdObject = Singleton<PoolManager>.Instance.GetFromPoolNoReparent(m_blipPrefab, m_blipPrefabInstanceId);
			radarBlipTracked.m_createdObject.SetActive(value: false);
			radarBlipTracked.m_createdObject.transform.parent = m_blipHierarchy;
			radarBlipTracked.m_displayComponent = radarBlipTracked.m_createdObject.GetComponent<RadarBlip>();
			m_allRadarBlips.Add(radarBlipTracked);
			return radarBlipTracked;
		}
		return null;
	}

	public float GetBlipPingAmountFor(TaggableItem ti)
	{
		if (m_radioOpStation.IsRadarActive())
		{
			return GetTrackerFor(ti, allowCreate: false)?.m_displayComponent.GetCurrentPingAmount() ?? 0f;
		}
		return 0f;
	}

	public bool ShouldPlayPingAnimation(TaggableItem ti)
	{
		if (m_radioOpStation.IsRadarActive())
		{
			return GetTrackerFor(ti, allowCreate: false)?.m_displayComponent.ShouldPlayAnimation() ?? false;
		}
		return false;
	}

	public void MarkAnimationPlayed(TaggableItem ti)
	{
		if (m_radioOpStation.IsRadarActive())
		{
			GetTrackerFor(ti, allowCreate: false)?.m_displayComponent.MarkAnimationPlayed();
		}
	}

	private void Update()
	{
		List<TaggableItem> allOfType = Singleton<TagManager>.Instance.GetAllOfType(TaggableItem.VisibilityType.RadarAssisted);
		BomberState bomberState = Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBomberState();
		RadarSystemUpgrade radarSystem = m_radioOpStation.GetRadarSystem();
		m_currentSweepAngle += 360f / radarSystem.GetSweepTime() * Time.deltaTime;
		while (m_currentSweepAngle >= 360f)
		{
			m_currentSweepAngle -= 360f;
		}
		m_sweepRotationNode.localRotation = Quaternion.Euler(0f, 0f, 0f - m_currentSweepAngle);
		bool flag = true;
		if (!m_radioOpStation.IsRadarManned())
		{
			flag = false;
		}
		if (m_radioOpStation.IsRadarElectricsWorking())
		{
			if (m_electricsOutActive)
			{
				m_statusLightElectricsOut.SetActive(value: false);
				m_electricsOutActive = false;
			}
		}
		else
		{
			if (!m_electricsOutActive)
			{
				m_statusLightElectricsOut.SetActive(value: true);
				m_electricsOutActive = true;
			}
			flag = false;
		}
		if (m_currentlyActive != flag)
		{
			m_activeHierarchy.SetActive(flag);
			m_currentlyActive = flag;
		}
		m_currentFighters = 0;
		m_currentLandTargets = 0;
		int num = 0;
		float range = radarSystem.GetRange();
		float num2 = range * m_rimDistanceMult;
		foreach (TaggableItem item in allOfType)
		{
			Vector3d vector3d = item.gameObject.btransform().position - bomberState.gameObject.btransform().position;
			Vector3 vector = (Vector3)vector3d;
			Vector3 vector2 = new Vector3(vector.x, 0f, vector.z);
			RadarBlipTracked trackerFor = GetTrackerFor(item, allowCreate: true);
			if (vector.magnitude < range)
			{
				Vector3 vector3 = new Vector3(0f - vector.z, vector.x, 0f).normalized * vector2.magnitude;
				Vector3 vector4 = vector3.normalized * Mathf.Clamp01(vector3.magnitude / num2);
				trackerFor.m_createdObject.transform.localPosition = vector4 * m_radarScale;
				trackerFor.m_createdObject.transform.rotation = Quaternion.identity;
				trackerFor.m_displayComponent.SetArrow(vector.y > m_arrowHeightThreshold, vector.y < 0f - m_arrowHeightThreshold, item.IsRadarOnGround());
				trackerFor.m_createdObject.SetActive(value: true);
				if (Vector3.Dot(m_blipHierarchy.worldToLocalMatrix.MultiplyVector(m_sweepRotationNode.up), vector3.normalized) > Mathf.Cos(m_sweepAngleThickness * ((float)Math.PI / 180f)))
				{
					trackerFor.m_displayComponent.PingBlip();
				}
				if (!item.DontAnnounceOnRadar())
				{
					if (item.IsRadarOnGround())
					{
						m_currentLandTargets++;
					}
					else
					{
						m_currentFighters++;
					}
				}
			}
			else
			{
				trackerFor.m_createdObject.SetActive(value: false);
			}
		}
		List<RadarBlipTracked> list = new List<RadarBlipTracked>();
		foreach (RadarBlipTracked allRadarBlip in m_allRadarBlips)
		{
			if (!allOfType.Contains(allRadarBlip.m_taggableItem) || allRadarBlip.m_taggableItem == null)
			{
				list.Add(allRadarBlip);
			}
		}
		foreach (RadarBlipTracked item2 in list)
		{
			UnityEngine.Object.Destroy(item2.m_createdObject);
			m_allRadarBlips.Remove(item2);
		}
		m_rotateHierarchy.localRotation = Quaternion.Inverse(RotationTo2DRotation(Camera.main.transform.rotation));
		m_bomberRotationHierarchy.localRotation = Quaternion.Inverse(RotationTo2DRotation(bomberState.transform.rotation));
	}

	public int GetNumFighters()
	{
		return m_currentFighters;
	}

	public int GetNumGroundTargets()
	{
		return m_currentLandTargets;
	}

	private Quaternion RotationTo2DRotation(Quaternion rotationIn)
	{
		return Quaternion.Euler(0f, rotationIn.eulerAngles.y, 0f);
	}
}
