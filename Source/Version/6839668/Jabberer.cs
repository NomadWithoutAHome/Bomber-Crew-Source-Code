using System.Collections.Generic;
using BomberCrewCommon;
using UnityEngine;
using WingroveAudio;

public class Jabberer : Singleton<Jabberer>
{
	public class ActiveJabber
	{
		public float m_countdown;

		public int m_syllablesRemaining;

		public int m_syllablesDone;

		public string m_baseEventName;

		public string m_forceFirstEvent;

		public GameObject m_relatedGameObject;

		public JabberSettings m_jabberSettings;
	}

	public enum JabberSettings
	{
		Normal,
		Twins
	}

	[SerializeField]
	private string m_pitchParameter;

	[SerializeField]
	private float m_jabberTimeMin;

	[SerializeField]
	private float m_jabberTimeMax;

	[SerializeField]
	private int m_jabberSyllsMin = 8;

	[SerializeField]
	private int m_jabberSyllsMax = 12;

	private List<ActiveJabber> m_allJabbers = new List<ActiveJabber>();

	public void StartJabber(string baseEvent, string firstEvent, GameObject relatedObject, JabberSettings settings)
	{
		ActiveJabber activeJabber = new ActiveJabber();
		activeJabber.m_syllablesRemaining = Random.Range(m_jabberSyllsMin, m_jabberSyllsMax);
		activeJabber.m_baseEventName = baseEvent;
		activeJabber.m_forceFirstEvent = firstEvent;
		activeJabber.m_relatedGameObject = relatedObject;
		activeJabber.m_jabberSettings = settings;
		if (settings == JabberSettings.Twins)
		{
			activeJabber.m_syllablesRemaining = 12;
		}
		m_allJabbers.Add(activeJabber);
	}

	private void Update()
	{
		List<ActiveJabber> list = new List<ActiveJabber>();
		foreach (ActiveJabber allJabber in m_allJabbers)
		{
			float num = ((Time.deltaTime != 0f) ? Time.unscaledDeltaTime : 0f);
			allJabber.m_countdown -= num;
			if (allJabber.m_countdown < 0f)
			{
				allJabber.m_countdown = Random.Range(m_jabberTimeMin, m_jabberTimeMax);
				if (allJabber.m_syllablesDone == 0 && !string.IsNullOrEmpty(allJabber.m_forceFirstEvent))
				{
					WingroveRoot.Instance.PostEventGO(allJabber.m_forceFirstEvent, base.gameObject);
				}
				else if (allJabber.m_jabberSettings == JabberSettings.Normal)
				{
					WingroveRoot.Instance.PostEventGO(allJabber.m_baseEventName, base.gameObject);
				}
				else if (allJabber.m_syllablesDone <= 5)
				{
					allJabber.m_countdown *= 1.5f;
					WingroveRoot.Instance.PostEventGO(allJabber.m_baseEventName + "_A", base.gameObject);
				}
				else if (allJabber.m_syllablesDone >= 7 && allJabber.m_syllablesDone <= 12)
				{
					allJabber.m_countdown *= 1.5f;
					WingroveRoot.Instance.PostEventGO(allJabber.m_baseEventName + "_B", base.gameObject);
				}
				allJabber.m_syllablesDone++;
				allJabber.m_syllablesRemaining--;
				if (allJabber.m_syllablesRemaining <= 0)
				{
					list.Add(allJabber);
				}
			}
		}
		foreach (ActiveJabber item in list)
		{
			m_allJabbers.Remove(item);
		}
	}
}
