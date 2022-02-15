using System.Collections.Generic;
using BomberCrewCommon;
using UnityEngine;
using WingroveAudio;

public class TopBarInfoQueue : Singleton<TopBarInfoQueue>
{
	public enum TopBarRequestType
	{
		Speech,
		Alert,
		Normal,
		Briefing
	}

	public class PointerHint
	{
		public GameObject m_toPointAt;

		public string m_toPointAtReference;

		public Vector3 m_offset2D;

		public bool m_downArrow;

		public bool m_worldSpace;
	}

	public class TopBarRequest
	{
		private string m_namedTextPC;

		private string m_namedTextController;

		private string m_namedText;

		private TopBarRequestType m_requestType;

		private List<PointerHint> m_pointerHints = new List<PointerHint>();

		private bool m_isGoodGuy;

		private string m_jabberAudio;

		private string m_seeMoreChapter;

		private Jabberer.JabberSettings m_jabberSettings;

		public bool m_isTexture;

		public bool m_isAlert;

		public string m_interactionIcon;

		public Texture2D m_hintTexture;

		public float m_preShowTime;

		public float m_expiryTime;

		public float m_reshowAllowedTime;

		public int m_priority;

		public string m_titleName;

		public TopBarRequest()
		{
		}

		public TopBarRequest(string text, string textController, bool isAlert, string interactionIcon, float preShow, float showTime, float noRepeat, int priority)
		{
			m_isAlert = isAlert;
			m_interactionIcon = interactionIcon;
			if (string.IsNullOrEmpty(textController))
			{
				SetString(text);
			}
			else
			{
				SetString(text, textController);
			}
			m_requestType = (isAlert ? TopBarRequestType.Alert : TopBarRequestType.Normal);
			m_preShowTime = preShow;
			m_expiryTime = showTime;
			m_reshowAllowedTime = noRepeat;
			m_priority = priority;
		}

		public void SetJabberSettings(Jabberer.JabberSettings js)
		{
			m_jabberSettings = js;
		}

		public string GetJabberAudioEvent()
		{
			return m_jabberAudio;
		}

		public Jabberer.JabberSettings GetJabberAudioSettings()
		{
			return m_jabberSettings;
		}

		public void AddPointerHint(GameObject go, Vector3 offset)
		{
			AddPointerHint(go, offset, downArrow: false, worldSpace: true);
		}

		public void AddPointerHint(GameObject go, Vector3 offset, bool downArrow, bool worldSpace)
		{
			PointerHint pointerHint = new PointerHint();
			pointerHint.m_downArrow = downArrow;
			pointerHint.m_toPointAt = go;
			pointerHint.m_offset2D = offset;
			pointerHint.m_worldSpace = worldSpace;
			m_pointerHints.Add(pointerHint);
		}

		public void AddPointerHint(string ptrRef, Vector3 offset, bool downArrow, bool worldSpace)
		{
			PointerHint pointerHint = new PointerHint();
			pointerHint.m_downArrow = downArrow;
			pointerHint.m_toPointAtReference = ptrRef;
			pointerHint.m_offset2D = offset;
			pointerHint.m_worldSpace = worldSpace;
			m_pointerHints.Add(pointerHint);
		}

		public List<PointerHint> GetPointerHints()
		{
			return m_pointerHints;
		}

		public static TopBarRequest Speech(string text, string fromCharacterName, Texture2D fromCharacterIcon, string jabberIntro, bool isGoodGuy)
		{
			TopBarRequest topBarRequest = new TopBarRequest();
			topBarRequest.SetString(text);
			topBarRequest.m_expiryTime = 15f;
			topBarRequest.m_priority = 999;
			topBarRequest.m_reshowAllowedTime = 0f;
			topBarRequest.m_preShowTime = 0f;
			topBarRequest.m_titleName = fromCharacterName;
			topBarRequest.m_isTexture = true;
			topBarRequest.m_hintTexture = fromCharacterIcon;
			topBarRequest.m_isGoodGuy = isGoodGuy;
			topBarRequest.m_jabberAudio = jabberIntro;
			topBarRequest.m_requestType = TopBarRequestType.Speech;
			return topBarRequest;
		}

		public static TopBarRequest Standard(string text, string textController, string interactionIcon, float preShow, float showTime, float noRepeat, int priority)
		{
			TopBarRequest topBarRequest = new TopBarRequest();
			if (!string.IsNullOrEmpty(textController))
			{
				topBarRequest.SetString(text, textController);
			}
			else
			{
				topBarRequest.SetString(text);
			}
			topBarRequest.m_interactionIcon = interactionIcon;
			topBarRequest.m_preShowTime = preShow;
			topBarRequest.m_expiryTime = showTime;
			topBarRequest.m_reshowAllowedTime = noRepeat;
			topBarRequest.m_priority = priority;
			topBarRequest.m_requestType = TopBarRequestType.Normal;
			return topBarRequest;
		}

		public static TopBarRequest Alert(string text, string textController, string interactionIcon, float preShow, float showTime, float noRepeat, int priority)
		{
			TopBarRequest topBarRequest = new TopBarRequest();
			if (!string.IsNullOrEmpty(textController))
			{
				topBarRequest.SetString(text, textController);
			}
			else
			{
				topBarRequest.SetString(text);
			}
			topBarRequest.m_interactionIcon = interactionIcon;
			topBarRequest.m_preShowTime = preShow;
			topBarRequest.m_expiryTime = showTime;
			topBarRequest.m_reshowAllowedTime = noRepeat;
			topBarRequest.m_priority = priority;
			topBarRequest.m_isAlert = true;
			topBarRequest.m_requestType = TopBarRequestType.Alert;
			return topBarRequest;
		}

		public static TopBarRequest Briefing(string text, string fromCharacterName, Texture2D fromCharacterIcon, string jabberIntro, bool isGoodGuy)
		{
			TopBarRequest topBarRequest = new TopBarRequest();
			topBarRequest.SetString(text);
			topBarRequest.m_expiryTime = 15f;
			topBarRequest.m_priority = 999;
			topBarRequest.m_reshowAllowedTime = 0f;
			topBarRequest.m_preShowTime = 0f;
			topBarRequest.m_titleName = fromCharacterName;
			topBarRequest.m_isTexture = true;
			topBarRequest.m_hintTexture = fromCharacterIcon;
			topBarRequest.m_isGoodGuy = isGoodGuy;
			topBarRequest.m_jabberAudio = jabberIntro;
			topBarRequest.m_requestType = TopBarRequestType.Briefing;
			return topBarRequest;
		}

		public void SetString(string text)
		{
			m_namedText = ControlPromptDisplayHelpers.ConvertString(Singleton<LanguageProvider>.Instance.GetTextGroupOrTextImmediate(text));
		}

		public void SetStringLiteral(string literalText)
		{
			m_namedText = literalText;
		}

		public void SetString(string textPc, string textController)
		{
			m_namedTextPC = ControlPromptDisplayHelpers.ConvertString(Singleton<LanguageProvider>.Instance.GetNamedTextImmediate(textPc));
			m_namedTextController = ControlPromptDisplayHelpers.ConvertString(Singleton<LanguageProvider>.Instance.GetNamedTextImmediate(textController));
		}

		public void SetTitleName(string titleName)
		{
			m_titleName = titleName;
		}

		public TopBarRequestType GetRequestType()
		{
			return m_requestType;
		}

		public string GetText()
		{
			if (!string.IsNullOrEmpty(m_namedText))
			{
				return m_namedText;
			}
			if (Singleton<UISelector>.Instance.IsPrimary())
			{
				return m_namedTextController;
			}
			return m_namedTextPC;
		}

		public TopBarRequest AddSeeMore(string chapter)
		{
			m_seeMoreChapter = chapter;
			return this;
		}

		public string GetSeeMore()
		{
			return m_seeMoreChapter;
		}
	}

	public class TopBarActive
	{
		public float m_inQueueTimer;

		public float m_shownForTimer;

		public float m_timeSinceShown;

		public TopBarRequest m_linkedRequest;

		public bool m_currentlyRequested;
	}

	private List<TopBarActive> m_currentQueue = new List<TopBarActive>();

	[SerializeField]
	private Transform m_location;

	[SerializeField]
	private GameObject m_normalPrefab;

	[SerializeField]
	private GameObject m_speechPrefab;

	[SerializeField]
	private GameObject m_speechAirbasePrefab;

	[SerializeField]
	private GameObject m_alertPrefab;

	[SerializeField]
	private GameObject m_briefingPrefab;

	[SerializeField]
	private GameObject m_pointerPrefab;

	[SerializeField]
	private Transform m_pointerParentTransform;

	[SerializeField]
	private Texture2D m_commandPortrait;

	[SerializeField]
	[NamedText]
	private string m_commandNamedText;

	[SerializeField]
	[AudioEventName]
	private string m_commandJabberText;

	private TopBarRequest m_currentlyShownRequest;

	private GameObject m_currentlySpawnedDisplay;

	private bool m_fullyDisabled;

	private bool m_hintsDisabled;

	private bool m_isInAirBase;

	private void OnEnable()
	{
		m_isInAirBase = Singleton<MissionCoordinator>.Instance == null;
	}

	private void OnDisable()
	{
	}

	private void DoDebugPanel()
	{
		m_hintsDisabled = GUILayout.Toggle(m_hintsDisabled, "Disable TOP BAR HINTS");
		m_fullyDisabled = GUILayout.Toggle(m_fullyDisabled, "Disable TOP BAR ALL");
	}

	public string GetCommandJabberText()
	{
		return m_commandJabberText;
	}

	public string GetCommandNamedText()
	{
		return m_commandNamedText;
	}

	public Texture2D GetCommandPortrait()
	{
		return m_commandPortrait;
	}

	public void RegisterRequest(TopBarRequest tbr)
	{
		foreach (TopBarActive item in m_currentQueue)
		{
			if (item.m_linkedRequest == tbr)
			{
				item.m_currentlyRequested = true;
				return;
			}
		}
		TopBarActive topBarActive = new TopBarActive();
		topBarActive.m_linkedRequest = tbr;
		topBarActive.m_currentlyRequested = true;
		m_currentQueue.Add(topBarActive);
	}

	public void RemoveRequest(TopBarRequest tbr)
	{
		TopBarActive topBarActive = null;
		foreach (TopBarActive item in m_currentQueue)
		{
			if (item.m_linkedRequest == tbr)
			{
				topBarActive = item;
				break;
			}
		}
		if (topBarActive != null)
		{
			topBarActive.m_currentlyRequested = false;
			if (topBarActive.m_timeSinceShown >= topBarActive.m_linkedRequest.m_reshowAllowedTime)
			{
				m_currentQueue.Remove(topBarActive);
			}
		}
	}

	private void Update()
	{
		List<TopBarActive> list = new List<TopBarActive>();
		TopBarActive topBarActive = null;
		foreach (TopBarActive item in m_currentQueue)
		{
			if ((topBarActive == null || item.m_linkedRequest.m_priority > topBarActive.m_linkedRequest.m_priority) && item.m_currentlyRequested && (item.m_shownForTimer < item.m_linkedRequest.m_expiryTime || item.m_linkedRequest.m_expiryTime == 0f) && item.m_inQueueTimer >= item.m_linkedRequest.m_preShowTime)
			{
				bool flag = false;
				if (item.m_linkedRequest.GetRequestType() != 0 && m_hintsDisabled)
				{
					flag = true;
				}
				if (m_fullyDisabled)
				{
					flag = true;
				}
				if (!flag)
				{
					topBarActive = item;
				}
			}
		}
		foreach (TopBarActive item2 in m_currentQueue)
		{
			if (item2 != topBarActive)
			{
				if (item2.m_currentlyRequested)
				{
					item2.m_inQueueTimer += Time.deltaTime;
				}
				else
				{
					item2.m_inQueueTimer = 0f;
				}
				item2.m_timeSinceShown += Time.deltaTime;
				if (item2.m_timeSinceShown >= item2.m_linkedRequest.m_reshowAllowedTime && item2.m_linkedRequest.m_reshowAllowedTime != 0f)
				{
					item2.m_shownForTimer = 0f;
				}
			}
			else
			{
				item2.m_timeSinceShown = 0f;
				item2.m_shownForTimer += Time.deltaTime;
			}
		}
		if (topBarActive != null)
		{
			if (m_currentlyShownRequest == topBarActive.m_linkedRequest)
			{
				return;
			}
			if (m_currentlySpawnedDisplay != null)
			{
				m_currentlySpawnedDisplay.GetComponent<TopBarInfoDisplayer>().Remove();
				m_currentlySpawnedDisplay = null;
			}
			switch (topBarActive.m_linkedRequest.GetRequestType())
			{
			case TopBarRequestType.Alert:
				m_currentlySpawnedDisplay = Singleton<PoolManager>.Instance.GetFromPoolSlowNoReparent(m_alertPrefab);
				break;
			case TopBarRequestType.Speech:
				if (m_isInAirBase)
				{
					m_currentlySpawnedDisplay = Singleton<PoolManager>.Instance.GetFromPoolSlowNoReparent(m_speechAirbasePrefab);
				}
				else
				{
					m_currentlySpawnedDisplay = Singleton<PoolManager>.Instance.GetFromPoolSlowNoReparent(m_speechPrefab);
				}
				break;
			case TopBarRequestType.Briefing:
				m_currentlySpawnedDisplay = Singleton<PoolManager>.Instance.GetFromPoolSlowNoReparent(m_briefingPrefab);
				break;
			default:
				m_currentlySpawnedDisplay = Singleton<PoolManager>.Instance.GetFromPoolSlowNoReparent(m_normalPrefab);
				break;
			}
			m_currentlySpawnedDisplay.transform.parent = m_location;
			m_currentlySpawnedDisplay.transform.localPosition = Vector3.zero;
			m_currentlySpawnedDisplay.GetComponent<TopBarInfoDisplayer>().SetUp(topBarActive.m_linkedRequest, m_pointerPrefab, m_pointerParentTransform);
			m_currentlyShownRequest = topBarActive.m_linkedRequest;
		}
		else if (m_currentlySpawnedDisplay != null)
		{
			m_currentlySpawnedDisplay.GetComponent<TopBarInfoDisplayer>().Remove();
			m_currentlySpawnedDisplay = null;
		}
	}

	public TopBarRequest GetCurrentlyShownRequest()
	{
		return m_currentlyShownRequest;
	}
}
