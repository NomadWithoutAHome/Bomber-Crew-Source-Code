using System.Collections.Generic;
using BomberCrewCommon;
using UnityEngine;

public class CloudGrid : Singleton<CloudGrid>
{
	public class CloudArea
	{
		public int m_x;

		public int m_y;

		public int m_numCloudsCurrently;

		public List<GameObject> m_currentClouds = new List<GameObject>();

		public float m_density;

		public Vector3d m_cloudNodeOffset;

		public bool m_rain;

		public bool m_snow;
	}

	[SerializeField]
	private float m_unitsPerGridCell;

	[SerializeField]
	private Vector3 m_initialOffset;

	[SerializeField]
	private int m_gridSize;

	[SerializeField]
	private GameObject[] m_cloudPrefabs;

	[SerializeField]
	private int m_cloudsPerDensity;

	[SerializeField]
	private float[] m_minScales;

	[SerializeField]
	private float[] m_maxScales;

	[SerializeField]
	private CloudLayers m_cloudLayers;

	[SerializeField]
	private int m_outOfRangeDistance = 3;

	private List<CloudArea> m_requiringUpdates = new List<CloudArea>();

	private CloudArea[] m_allCloudAreas;

	private int m_prevX;

	private int m_prevY;

	private bool m_alwaysSlow;

	private int[] m_cloudPrefabInstanceIds;

	private void Awake()
	{
		m_cloudPrefabInstanceIds = new int[m_cloudPrefabs.Length];
		for (int i = 0; i < m_cloudPrefabs.Length; i++)
		{
			m_cloudPrefabInstanceIds[i] = m_cloudPrefabs[i].GetInstanceID();
		}
		m_allCloudAreas = new CloudArea[m_gridSize * m_gridSize];
		for (int j = 0; j < m_gridSize; j++)
		{
			for (int k = 0; k < m_gridSize; k++)
			{
				m_allCloudAreas[j + k * m_gridSize] = new CloudArea();
				m_allCloudAreas[j + k * m_gridSize].m_x = j;
				m_allCloudAreas[j + k * m_gridSize].m_y = k;
				m_allCloudAreas[j + k * m_gridSize].m_cloudNodeOffset = new Vector3d(GridToPosition(j, k));
			}
		}
	}

	public void RegisterOverall(float density, bool rain, bool snow)
	{
		CloudArea[] allCloudAreas = m_allCloudAreas;
		foreach (CloudArea cloudArea in allCloudAreas)
		{
			cloudArea.m_rain |= rain;
			cloudArea.m_snow |= snow;
			cloudArea.m_density = Mathf.Max(density, cloudArea.m_density);
			if (NumClouds(m_allCloudAreas[cloudArea.m_x + cloudArea.m_y * m_gridSize].m_density) != m_allCloudAreas[cloudArea.m_x + cloudArea.m_y * m_gridSize].m_numCloudsCurrently)
			{
				m_requiringUpdates.Add(cloudArea);
			}
		}
	}

	public Vector2 PositionToGrid(Vector3 inPos)
	{
		Vector3 vector = inPos - m_initialOffset;
		return new Vector2(Mathf.Round(vector.x / m_unitsPerGridCell), Mathf.Round(vector.z / m_unitsPerGridCell));
	}

	public Vector3 GridToPosition(int x, int y)
	{
		return m_initialOffset + new Vector3((float)x * m_unitsPerGridCell, base.transform.position.y, (float)y * m_unitsPerGridCell);
	}

	private int NumClouds(float density)
	{
		return Mathf.RoundToInt(density * (float)m_cloudsPerDensity);
	}

	public void Clear()
	{
		CloudArea[] allCloudAreas = m_allCloudAreas;
		foreach (CloudArea cloudArea in allCloudAreas)
		{
			cloudArea.m_density = 0f;
			cloudArea.m_rain = false;
			cloudArea.m_snow = false;
		}
	}

	public void RegisterArea(Vector3 ctr, float radius, float density, bool rain, bool snow)
	{
		Vector2 vector = PositionToGrid(ctr + new Vector3(0f - radius, 0f, 0f - radius));
		Vector2 vector2 = PositionToGrid(ctr + new Vector3(radius, 0f, radius));
		for (int i = (int)vector.x; i <= (int)vector2.x; i++)
		{
			for (int j = (int)vector.y; j <= (int)vector2.y; j++)
			{
				if (i < 0 || i >= m_gridSize || j < 0 || j >= m_gridSize)
				{
					continue;
				}
				Vector3 vector3 = GridToPosition(i, j);
				if ((vector3 - ctr).magnitude < radius)
				{
					m_allCloudAreas[i + j * m_gridSize].m_rain |= rain;
					m_allCloudAreas[i + j * m_gridSize].m_snow |= snow;
					m_allCloudAreas[i + j * m_gridSize].m_density = Mathf.Max(density, m_allCloudAreas[i + j * m_gridSize].m_density);
					if (NumClouds(m_allCloudAreas[i + j * m_gridSize].m_density) != m_allCloudAreas[i + j * m_gridSize].m_numCloudsCurrently)
					{
						m_requiringUpdates.Add(m_allCloudAreas[i + j * m_gridSize]);
					}
				}
			}
		}
	}

	public void SetAlwaysSlow()
	{
		m_alwaysSlow = true;
	}

	private void Update()
	{
		Vector3d position = Singleton<BomberSpawn>.Instance.GetBomberSystems().gameObject.btransform().position;
		Vector2 vector = PositionToGrid((Vector3)position);
		int num = (int)vector.x;
		int num2 = (int)vector.y;
		bool flag = false;
		if (m_prevX != num || m_prevY != num2)
		{
			flag = true;
		}
		m_prevX = num;
		m_prevY = num2;
		if (flag)
		{
			for (int i = num - (m_outOfRangeDistance + 1); i < num + (m_outOfRangeDistance + 1); i++)
			{
				for (int j = num2 - (m_outOfRangeDistance + 1); j < num2 + (m_outOfRangeDistance + 1); j++)
				{
					if (i >= 0 && i < m_gridSize && j >= 0 && j < m_gridSize)
					{
						m_requiringUpdates.Add(m_allCloudAreas[i + j * m_gridSize]);
					}
				}
			}
		}
		float num3 = 0f;
		bool flag2 = false;
		bool snow = false;
		if (num >= 0 && num < m_gridSize && num2 >= 0 && num2 < m_gridSize)
		{
			num3 = m_allCloudAreas[num + num2 * m_gridSize].m_density;
			flag2 = m_allCloudAreas[num + num2 * m_gridSize].m_rain;
			snow = m_allCloudAreas[num + num2 * m_gridSize].m_snow;
		}
		if (flag2)
		{
			num3 *= 4f;
		}
		m_cloudLayers.SetLowerDensityTarget(num3);
		Singleton<BomberSpawn>.Instance.GetBomberSystems().GetWeatherSensor().SetFromCloudGrid(snow, flag2, lightning: false);
		if (m_requiringUpdates.Count <= 0)
		{
			return;
		}
		List<CloudArea> list = new List<CloudArea>();
		int num4 = 0;
		foreach (CloudArea requiringUpdate in m_requiringUpdates)
		{
			int value = requiringUpdate.m_x - num;
			int value2 = requiringUpdate.m_y - num2;
			int num5 = Mathf.Abs(value) + Mathf.Abs(value2);
			int num6 = 0;
			if (num5 < m_outOfRangeDistance)
			{
				num6 = NumClouds(requiringUpdate.m_density);
			}
			bool flag3 = false;
			int num7 = 0;
			while (requiringUpdate.m_numCloudsCurrently < num6)
			{
				if (num4 > 1 && m_alwaysSlow)
				{
					flag3 = true;
					break;
				}
				if (num7 > 1 && num5 > 1)
				{
					flag3 = true;
					break;
				}
				int num8 = Random.Range(0, m_cloudPrefabs.Length);
				GameObject fromPoolNoReparent = Singleton<PoolManager>.Instance.GetFromPoolNoReparent(m_cloudPrefabs[num8], m_cloudPrefabInstanceIds[num8]);
				fromPoolNoReparent.transform.parent = base.transform;
				fromPoolNoReparent.transform.localPosition = new Vector3(Random.Range((0f - m_unitsPerGridCell) * 0.5f, m_unitsPerGridCell * 0.5f), 0f, Random.Range((0f - m_unitsPerGridCell) * 0.5f, m_unitsPerGridCell * 0.5f)) + (Vector3)requiringUpdate.m_cloudNodeOffset;
				fromPoolNoReparent.transform.localScale = Vector3.one * Random.Range(m_minScales[num8], m_maxScales[num8]);
				fromPoolNoReparent.transform.localRotation = Quaternion.Euler(0f, Random.Range(0, 360), 0f);
				requiringUpdate.m_numCloudsCurrently++;
				num7++;
				num4++;
				requiringUpdate.m_currentClouds.Add(fromPoolNoReparent);
			}
			int num9 = 0;
			while (requiringUpdate.m_numCloudsCurrently > num6)
			{
				if (num9 > 2 && (num5 > 1 || m_alwaysSlow))
				{
					flag3 = true;
					break;
				}
				GameObject pooledObject = requiringUpdate.m_currentClouds[0];
				requiringUpdate.m_currentClouds.RemoveAt(0);
				requiringUpdate.m_numCloudsCurrently--;
				Singleton<PoolManager>.Instance.ReturnToPool(pooledObject);
				num9++;
				num4++;
			}
			if (!flag3)
			{
				list.Add(requiringUpdate);
			}
			if (m_alwaysSlow && num4 > 1)
			{
				break;
			}
		}
		foreach (CloudArea item in list)
		{
			m_requiringUpdates.Remove(item);
		}
	}
}
