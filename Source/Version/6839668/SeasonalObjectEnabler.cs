using System;
using UnityEngine;

public class SeasonalObjectEnabler : MonoBehaviour
{
	[Serializable]
	public class SeasonalObjectGroup
	{
		[SerializeField]
		private GameObject[] m_objectsToEnable;

		[SerializeField]
		private GameObject[] m_objectsToDisable;

		[SerializeField]
		private int m_monthStart;

		[SerializeField]
		private int m_dayStart;

		[SerializeField]
		private int m_monthEnd;

		[SerializeField]
		private int m_dayEnd;

		private bool IsValid()
		{
			DateTime utcNow = DateTime.UtcNow;
			bool flag = false;
			bool flag2 = false;
			int month = utcNow.Month;
			int day = utcNow.Day;
			if (month > m_monthStart)
			{
				flag = true;
			}
			else if (month == m_monthStart && day >= m_dayStart)
			{
				flag = true;
			}
			if (month < m_monthEnd)
			{
				flag2 = true;
			}
			else if (month == m_monthEnd && day <= m_dayEnd)
			{
				flag2 = true;
			}
			if (m_monthEnd >= m_monthStart)
			{
				return flag && flag2;
			}
			return flag || flag2;
		}

		public void Set()
		{
			if (IsValid())
			{
				GameObject[] objectsToEnable = m_objectsToEnable;
				foreach (GameObject gameObject in objectsToEnable)
				{
					gameObject.SetActive(value: true);
				}
				GameObject[] objectsToDisable = m_objectsToDisable;
				foreach (GameObject gameObject2 in objectsToDisable)
				{
					gameObject2.SetActive(value: false);
				}
			}
		}
	}

	[SerializeField]
	private SeasonalObjectGroup[] m_seasonalObjectGroups;

	private void Start()
	{
		SeasonalObjectGroup[] seasonalObjectGroups = m_seasonalObjectGroups;
		foreach (SeasonalObjectGroup seasonalObjectGroup in seasonalObjectGroups)
		{
			seasonalObjectGroup.Set();
		}
	}
}
