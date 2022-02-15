using System.Collections.Generic;
using BomberCrewCommon;
using UnityEngine;

public class DemoAirbasePrompter : MonoBehaviour
{
	[SerializeField]
	private UIScreen[] m_screensToVisit;

	[SerializeField]
	private AirbaseAreaScreen[] m_screensToVisitArea;

	[SerializeField]
	private AirbaseNavigation m_navigationTabs;

	[SerializeField]
	private float m_timeInEachScreen;

	[SerializeField]
	[NamedText]
	private string[] m_namedTextOnScreen;

	[SerializeField]
	private GameObject m_pointerPrefab;

	[SerializeField]
	private BriefingScreen m_briefingScreen;

	private float m_currentTimer;

	private int m_currentIndex;

	private List<AirbaseAreaScreen> m_scenesVisited = new List<AirbaseAreaScreen>();

	private bool m_showingPointer;

	private GameObject m_currentPointer;

	private TopBarInfoQueue.TopBarRequest m_currentRequest;

	private void Start()
	{
		m_currentPointer = Object.Instantiate(m_pointerPrefab);
		m_currentPointer.SetActive(value: false);
	}

	private void Update()
	{
		if (!Singleton<GameFlow>.Instance.IsDemoMode())
		{
			Object.Destroy(base.gameObject);
			return;
		}
		if (Singleton<UIScreenManager>.Instance.IsScreenActive("DeBriefing"))
		{
			m_currentPointer.SetActive(value: false);
			return;
		}
		bool flag = false;
		for (int i = 0; i < m_screensToVisit.Length; i++)
		{
			if (!Singleton<UIScreenManager>.Instance.IsScreenActive(m_screensToVisit[i].name))
			{
				continue;
			}
			flag = true;
			if (!m_scenesVisited.Contains(m_screensToVisitArea[i]))
			{
				if (m_currentRequest != null)
				{
					Singleton<TopBarInfoQueue>.Instance.RemoveRequest(m_currentRequest);
				}
				m_scenesVisited.Add(m_screensToVisitArea[i]);
				TopBarInfoQueue.TopBarRequest topBarRequest = TopBarInfoQueue.TopBarRequest.Speech(m_namedTextOnScreen[i], Singleton<LanguageProvider>.Instance.GetNamedTextImmediate(Singleton<TopBarInfoQueue>.Instance.GetCommandNamedText()), Singleton<TopBarInfoQueue>.Instance.GetCommandPortrait(), Singleton<TopBarInfoQueue>.Instance.GetCommandJabberText(), isGoodGuy: true);
				Singleton<TopBarInfoQueue>.Instance.RegisterRequest(topBarRequest);
				m_currentRequest = topBarRequest;
			}
		}
		if (m_briefingScreen.IsShowingMission() && Singleton<UIScreenManager>.Instance.IsScreenActive(m_briefingScreen.name) && m_currentRequest != null)
		{
			Singleton<TopBarInfoQueue>.Instance.RemoveRequest(m_currentRequest);
		}
		if (!flag && m_currentRequest != null)
		{
			Singleton<TopBarInfoQueue>.Instance.RemoveRequest(m_currentRequest);
		}
		if (m_currentIndex >= m_screensToVisit.Length)
		{
			return;
		}
		bool flag2 = m_scenesVisited.Contains(m_screensToVisitArea[m_currentIndex]);
		if (!flag2 && m_navigationTabs.GetButtonFor(m_screensToVisitArea[m_currentIndex]).IsSelected())
		{
			flag2 = true;
		}
		if (flag2 == m_showingPointer)
		{
			if (!flag2)
			{
				m_showingPointer = true;
				m_currentPointer.SetActive(value: true);
				m_currentPointer.transform.position = m_navigationTabs.GetButtonFor(m_screensToVisitArea[m_currentIndex]).transform.position;
			}
			else
			{
				m_currentPointer.SetActive(value: false);
				m_showingPointer = false;
			}
		}
		m_currentTimer += Time.deltaTime;
		if (m_currentTimer > m_timeInEachScreen)
		{
			m_currentIndex++;
			if (m_currentIndex == m_screensToVisit.Length)
			{
				m_currentIndex--;
			}
			m_currentTimer = 0f;
		}
	}
}
