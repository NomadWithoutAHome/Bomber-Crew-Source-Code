using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace WingroveAudio;

[AddComponentMenu("WingroveAudio/Wingrove Root")]
public class WingroveRoot : MonoBehaviour
{
	public class AudioSourcePoolItem
	{
		public AudioSource m_audioSource;

		public PooledAudioSource m_pooledAudioSource;

		public ActiveCue m_user;

		public int m_useCount;
	}

	public class CachedParameterValue
	{
		public float m_valueNull;

		public Dictionary<int, float> m_valueObject = new Dictionary<int, float>();

		public Dictionary<int, GameObject> m_nullCheckDictionary = new Dictionary<int, GameObject>();

		public bool m_isGlobalValue;

		public long m_lastNonGlobalValueN;
	}

	private class ParameterValues
	{
		public Dictionary<int, CachedParameterValue> m_parameterValues = new Dictionary<int, CachedParameterValue>();

		public List<CachedParameterValue> m_parameterValuesFast = new List<CachedParameterValue>();

		public Dictionary<string, int> m_parameterToIntValues = new Dictionary<string, int>();

		public Dictionary<int, string> m_intToParameterValues = new Dictionary<int, string>();
	}

	private static WingroveRoot s_instance;

	[SerializeField]
	private bool m_useDecibelScale = true;

	[SerializeField]
	private bool m_allowMultipleListeners;

	[SerializeField]
	private bool m_dontDestroyOnLoad;

	private GUISkin m_editorSkin;

	private static bool s_hasInstance;

	[SerializeField]
	private int m_audioSourcePoolSize = 32;

	[SerializeField]
	private int m_calculateRMSIntervalFrames = 1;

	[SerializeField]
	private bool m_useDSPTime = true;

	[SerializeField]
	private bool m_debugGUI;

	[SerializeField]
	private Vector3 m_listenerOffset;

	[SerializeField]
	private Audio3DSetting m_default3DAudioSettings;

	private int m_rmsFrame;

	private int m_frameCtr;

	private static double m_lastDSPTime;

	private static double m_dspDeltaTime;

	private List<AudioSourcePoolItem> m_audioSourcePool = new List<AudioSourcePoolItem>(128);

	private HashSet<AudioSourcePoolItem> m_freeAudioSources = new HashSet<AudioSourcePoolItem>();

	private Dictionary<string, List<BaseEventReceiveAction>> m_eventReceivers = new Dictionary<string, List<BaseEventReceiveAction>>();

	[SerializeField]
	public AudioNameGroup[] m_audioNameGroups;

	private List<WingroveListener> m_listeners = new List<WingroveListener>();

	private int m_listenerCount;

	private List<BaseWingroveAudioSource> m_allRegisteredSources = new List<BaseWingroveAudioSource>();

	private List<WingroveMixBus> m_allMixBuses = new List<WingroveMixBus>();

	private List<InstanceLimiter> m_allInstanceLimiters = new List<InstanceLimiter>();

	private ParameterValues m_values = new ParameterValues();

	private GameObject m_thisListener;

	private int m_cachedCurrentVoices;

	private GameObject m_pool;

	private int m_slowConstantCtr;

	private List<int> m_toRemoveParamsIntCached = new List<int>(32);

	public static WingroveRoot Instance
	{
		get
		{
			if (!s_hasInstance)
			{
				s_instance = (WingroveRoot)Object.FindObjectOfType(typeof(WingroveRoot));
				if (s_instance != null)
				{
					s_hasInstance = true;
				}
			}
			return s_instance;
		}
	}

	public static WingroveRoot InstanceEditor
	{
		get
		{
			if (s_instance == null)
			{
				s_instance = (WingroveRoot)Object.FindObjectOfType(typeof(WingroveRoot));
			}
			return s_instance;
		}
	}

	public bool UseDBScale
	{
		get
		{
			return m_useDecibelScale;
		}
		set
		{
			m_useDecibelScale = value;
		}
	}

	private void Awake()
	{
		if (m_dontDestroyOnLoad)
		{
			Object.DontDestroyOnLoad(this);
		}
		s_instance = this;
		m_pool = new GameObject("AudioSourcePool");
		m_pool.transform.parent = base.transform;
		for (int i = 0; i < m_audioSourcePoolSize; i++)
		{
			CreateAudioSource(usingInstantly: false);
		}
		BaseEventReceiveAction[] componentsInChildren = GetComponentsInChildren<BaseEventReceiveAction>();
		BaseEventReceiveAction[] array = componentsInChildren;
		foreach (BaseEventReceiveAction baseEventReceiveAction in array)
		{
			string[] events = baseEventReceiveAction.GetEvents();
			if (events == null)
			{
				continue;
			}
			string[] array2 = events;
			foreach (string key in array2)
			{
				if (!m_eventReceivers.ContainsKey(key))
				{
					m_eventReceivers[key] = new List<BaseEventReceiveAction>();
				}
				m_eventReceivers[key].Add(baseEventReceiveAction);
			}
		}
		if (m_thisListener == null)
		{
			m_thisListener = new GameObject("Listener");
			m_thisListener.transform.parent = base.transform;
			m_thisListener.AddComponent<AudioListener>();
			m_thisListener.transform.localPosition = m_listenerOffset;
		}
		base.transform.position = Vector3.zero;
		m_lastDSPTime = AudioSettings.dspTime;
		SceneManager.activeSceneChanged += SceneChanged;
	}

	private void ClearParamSlowConstant(int count)
	{
		int num = 0;
		if (m_values.m_parameterValuesFast.Count != 0)
		{
			CachedParameterValue cachedParameterValue = m_values.m_parameterValuesFast[m_slowConstantCtr % m_values.m_parameterValuesFast.Count];
			if (!cachedParameterValue.m_isGlobalValue)
			{
				foreach (KeyValuePair<int, GameObject> item in cachedParameterValue.m_nullCheckDictionary)
				{
					if (item.Value == null)
					{
						m_toRemoveParamsIntCached.Add(item.Key);
						num++;
						if (count != 0 && num == count)
						{
							break;
						}
					}
				}
				if (num > 0)
				{
					foreach (int item2 in m_toRemoveParamsIntCached)
					{
						cachedParameterValue.m_nullCheckDictionary.Remove(item2);
						cachedParameterValue.m_valueObject.Remove(item2);
					}
					m_toRemoveParamsIntCached.Clear();
				}
			}
		}
		m_slowConstantCtr++;
	}

	public void RegisterIL(InstanceLimiter il)
	{
		m_allInstanceLimiters.Add(il);
	}

	private void ClearParams(int count)
	{
		int num = 0;
		foreach (KeyValuePair<int, CachedParameterValue> parameterValue in m_values.m_parameterValues)
		{
			if (!parameterValue.Value.m_isGlobalValue)
			{
				foreach (KeyValuePair<int, GameObject> item in parameterValue.Value.m_nullCheckDictionary)
				{
					if (item.Value == null)
					{
						m_toRemoveParamsIntCached.Add(item.Key);
						num++;
						if (count != 0 && num == count)
						{
							break;
						}
					}
				}
				if (num > 0)
				{
					foreach (int item2 in m_toRemoveParamsIntCached)
					{
						parameterValue.Value.m_nullCheckDictionary.Remove(item2);
						parameterValue.Value.m_valueObject.Remove(item2);
					}
					m_toRemoveParamsIntCached.Clear();
				}
			}
			if (count != 0 && num >= count)
			{
				break;
			}
		}
	}

	public int GetParameterId(string fromParameter)
	{
		int value = 0;
		if (!m_values.m_parameterToIntValues.TryGetValue(fromParameter, out value))
		{
			value = m_values.m_parameterToIntValues.Count + 1;
			m_values.m_parameterToIntValues[fromParameter] = value;
			m_values.m_intToParameterValues[value] = fromParameter;
		}
		return value;
	}

	private void SceneChanged(Scene sca, Scene scb)
	{
		s_hasInstance = false;
		ClearParams(0);
	}

	public Audio3DSetting GetDefault3DSettings()
	{
		return m_default3DAudioSettings;
	}

	public void RegisterAudioSource(BaseWingroveAudioSource bas)
	{
		m_allRegisteredSources.Add(bas);
	}

	public void RegisterMixBus(WingroveMixBus wmb)
	{
		m_allMixBuses.Add(wmb);
	}

	public float EvaluteDefault3D(float distance)
	{
		if (m_default3DAudioSettings != null)
		{
			return m_default3DAudioSettings.EvaluateStandard(distance);
		}
		return 1f / distance;
	}

	private void OnGUI()
	{
		if (!m_debugGUI)
		{
			return;
		}
		Vector3 vector = new Vector3(0f, 0f);
		GUI.BeginGroup(new Rect(0f, 0f, 600f, 1000f), string.Empty, "box");
		GUI.Label(new Rect(vector.x, vector.y, 300f, 20f), "Number of sources: " + m_audioSourcePool.Count);
		vector.y += 20f;
		foreach (AudioSourcePoolItem item in m_audioSourcePool)
		{
			if (item.m_user != null)
			{
				GUI.Label(new Rect(vector.x, vector.y, 500f, 20f), string.Concat(item.m_audioSource.clip.name, " @ ", item.m_user.GetState(), " ", (int)(100f * item.m_audioSource.time / item.m_audioSource.clip.length), " ", item.m_useCount));
			}
			else
			{
				GUI.Label(new Rect(vector.x, vector.y, 500f, 20f), "Unused : " + item.m_useCount);
			}
			vector.y += 20f;
			if (vector.y >= 980f)
			{
				vector.y = 40f;
				vector.x += 300f;
			}
		}
		GUI.EndGroup();
	}

	public CachedParameterValue GetParameter(int parameter)
	{
		m_values.m_parameterValues.TryGetValue(parameter, out var value);
		return value;
	}

	public float GetParameterForGameObject(int parameter, int gameObjectId)
	{
		m_values.m_parameterValues.TryGetValue(parameter, out var value);
		if (value == null)
		{
			return 0f;
		}
		if (value.m_isGlobalValue)
		{
			return value.m_valueNull;
		}
		float value2 = 0f;
		value.m_valueObject.TryGetValue(gameObjectId, out value2);
		return value2;
	}

	public void SetParameterGlobal(int parameter, float setValue)
	{
		CachedParameterValue value = null;
		m_values.m_parameterValues.TryGetValue(parameter, out value);
		if (value == null)
		{
			value = new CachedParameterValue();
			m_values.m_parameterValues[parameter] = value;
			m_values.m_parameterValuesFast.Add(value);
		}
		value.m_valueNull = setValue;
		value.m_isGlobalValue = true;
	}

	public void SetParameterForObject(int parameter, int gameObjectId, GameObject go, float setValue)
	{
		CachedParameterValue value = null;
		m_values.m_parameterValues.TryGetValue(parameter, out value);
		if (value == null)
		{
			value = new CachedParameterValue();
			m_values.m_parameterValues[parameter] = value;
			m_values.m_parameterValuesFast.Add(value);
		}
		value.m_valueObject[gameObjectId] = setValue;
		value.m_nullCheckDictionary[gameObjectId] = go;
		value.m_lastNonGlobalValueN++;
		value.m_isGlobalValue = false;
	}

	public Dictionary<int, CachedParameterValue> GetAllParams()
	{
		return m_values.m_parameterValues;
	}

	public string GetParamName(int val)
	{
		return m_values.m_intToParameterValues[val];
	}

	public int FindEvent(string eventName)
	{
		int num = 0;
		AudioNameGroup[] audioNameGroups = m_audioNameGroups;
		foreach (AudioNameGroup audioNameGroup in audioNameGroups)
		{
			if (audioNameGroup != null && audioNameGroup.GetEvents() != null)
			{
				string[] events = audioNameGroup.GetEvents();
				foreach (string text in events)
				{
					if (text == eventName)
					{
						return num;
					}
				}
			}
			num++;
		}
		return -1;
	}

	public int FindParameter(string parameterName)
	{
		int num = 0;
		AudioNameGroup[] audioNameGroups = m_audioNameGroups;
		foreach (AudioNameGroup audioNameGroup in audioNameGroups)
		{
			if (audioNameGroup != null && audioNameGroup.GetParameters() != null)
			{
				string[] parameters = audioNameGroup.GetParameters();
				foreach (string text in parameters)
				{
					if (text == parameterName)
					{
						return num;
					}
				}
			}
			num++;
		}
		return -1;
	}

	public GUISkin GetSkin()
	{
		if (m_editorSkin == null)
		{
			m_editorSkin = (GUISkin)Resources.Load("WingroveAudioSkin");
		}
		return m_editorSkin;
	}

	public void PostEvent(string eventName)
	{
		PostEventCL(eventName, null, null);
	}

	public void PostEventCL(string eventName, List<ActiveCue> cuesIn)
	{
		PostEventCL(eventName, cuesIn, null);
	}

	public void PostEventGO(string eventName, GameObject targetObject)
	{
		PostEventGO(eventName, targetObject, null);
	}

	public void PostEventGOAA(string eventName, GameObject targetObject, AudioArea aa)
	{
		PostEventGOAA(eventName, targetObject, null, aa);
	}

	public void PostEventGOAA(string eventName, GameObject targetObject, List<ActiveCue> cuesOut, AudioArea aa)
	{
		List<BaseEventReceiveAction> value = null;
		if (!m_eventReceivers.TryGetValue(eventName, out value))
		{
			return;
		}
		foreach (BaseEventReceiveAction item in value)
		{
			item.PerformAction(eventName, targetObject, aa, cuesOut);
		}
	}

	public void PostEventGO(string eventName, GameObject targetObject, List<ActiveCue> cuesOut)
	{
		List<BaseEventReceiveAction> value = null;
		if (!m_eventReceivers.TryGetValue(eventName, out value))
		{
			return;
		}
		foreach (BaseEventReceiveAction item in value)
		{
			item.PerformAction(eventName, targetObject, cuesOut);
		}
	}

	public void PostEventCL(string eventName, List<ActiveCue> cuesIn, List<ActiveCue> cuesOut)
	{
		List<BaseEventReceiveAction> value = null;
		if (!m_eventReceivers.TryGetValue(eventName, out value))
		{
			return;
		}
		foreach (BaseEventReceiveAction item in value)
		{
			item.PerformAction(eventName, cuesIn, cuesOut);
		}
	}

	public AudioSourcePoolItem TryClaimPoolSource(ActiveCue cue)
	{
		AudioSourcePoolItem audioSourcePoolItem = null;
		foreach (AudioSourcePoolItem freeAudioSource in m_freeAudioSources)
		{
			if (freeAudioSource.m_user == null || freeAudioSource.m_user.GetState() == ActiveCue.CueState.Stopped)
			{
				audioSourcePoolItem = freeAudioSource;
				break;
			}
		}
		if (audioSourcePoolItem != null)
		{
			if (audioSourcePoolItem.m_user != null)
			{
				audioSourcePoolItem.m_user.Virtualise();
			}
			m_freeAudioSources.Remove(audioSourcePoolItem);
			audioSourcePoolItem.m_user = cue;
			return audioSourcePoolItem;
		}
		AudioSourcePoolItem audioSourcePoolItem2 = null;
		int importance = cue.GetImportance();
		float num = 1f;
		foreach (AudioSourcePoolItem item in m_audioSourcePool)
		{
			if (item.m_user == null || item.m_user.GetState() == ActiveCue.CueState.Stopped)
			{
				if (item.m_user != null)
				{
					item.m_user.Virtualise();
				}
				item.m_user = cue;
				m_freeAudioSources.Remove(item);
				return item;
			}
			int importance2 = item.m_user.GetImportance();
			if (importance2 < cue.GetImportance())
			{
				if (importance2 < importance)
				{
					importance = item.m_user.GetImportance();
					audioSourcePoolItem2 = item;
				}
				else if (importance2 == importance && item.m_user.GetState() == ActiveCue.CueState.PlayingFadeOut)
				{
					audioSourcePoolItem2 = item;
				}
			}
			else if (importance2 == importance && (item.m_user.GetState() == ActiveCue.CueState.PlayingFadeOut || item.m_user.GetTheoreticalVolumeCached() < num))
			{
				num = item.m_user.GetTheoreticalVolumeCached();
				audioSourcePoolItem2 = item;
			}
		}
		if (audioSourcePoolItem2 != null)
		{
			audioSourcePoolItem2.m_user.Virtualise();
			audioSourcePoolItem2.m_pooledAudioSource.ResetFiltersForFrame();
			audioSourcePoolItem2.m_user = cue;
			m_freeAudioSources.Remove(audioSourcePoolItem2);
			return audioSourcePoolItem2;
		}
		if (m_audioSourcePool.Count < m_audioSourcePoolSize)
		{
			AudioSourcePoolItem audioSourcePoolItem3 = CreateAudioSource(usingInstantly: true);
			audioSourcePoolItem3.m_user = cue;
			return audioSourcePoolItem3;
		}
		return null;
	}

	private AudioSourcePoolItem CreateAudioSource(bool usingInstantly)
	{
		GameObject gameObject = new GameObject("PooledAudioSource_" + m_audioSourcePool.Count);
		gameObject.transform.parent = m_pool.transform;
		AudioSource audioSource = gameObject.AddComponent<AudioSource>();
		AudioSourcePoolItem audioSourcePoolItem = new AudioSourcePoolItem();
		audioSourcePoolItem.m_audioSource = audioSource;
		audioSourcePoolItem.m_pooledAudioSource = gameObject.AddComponent<PooledAudioSource>();
		audioSource.enabled = false;
		m_audioSourcePool.Add(audioSourcePoolItem);
		if (!usingInstantly)
		{
			m_freeAudioSources.Add(audioSourcePoolItem);
		}
		return audioSourcePoolItem;
	}

	public void UnlinkSource(AudioSourcePoolItem item, bool fromVirtualise)
	{
		item.m_audioSource.Stop();
		item.m_audioSource.enabled = false;
		item.m_audioSource.clip = null;
		item.m_user = null;
		m_freeAudioSources.Add(item);
	}

	public string dbStringUtil(float amt)
	{
		string empty = string.Empty;
		float num = 20f * Mathf.Log10(amt);
		if (num == 0f)
		{
			return "-0.00 dB";
		}
		if (float.IsInfinity(num))
		{
			return "-inf dB";
		}
		return $"{num:0.00}" + " dB";
	}

	public bool ShouldCalculateMS(int index)
	{
		return true;
	}

	private void Update()
	{
		ClearParamSlowConstant(1);
		m_frameCtr = (m_frameCtr + 1) % 512;
		if (m_listenerCount > 0)
		{
			GetSingleListener().UpdatePosition();
		}
		foreach (WingroveMixBus allMixBuse in m_allMixBuses)
		{
			allMixBuse.DoUpdate();
		}
		foreach (BaseWingroveAudioSource allRegisteredSource in m_allRegisteredSources)
		{
			allRegisteredSource.DoUpdate(m_frameCtr);
		}
		foreach (InstanceLimiter allInstanceLimiter in m_allInstanceLimiters)
		{
			allInstanceLimiter.ResetFrameFlags();
		}
		m_cachedCurrentVoices = 0;
		foreach (AudioSourcePoolItem item in m_audioSourcePool)
		{
			if (item.m_user != null)
			{
				m_cachedCurrentVoices++;
			}
		}
		m_rmsFrame++;
		if (m_rmsFrame >= m_calculateRMSIntervalFrames)
		{
			m_rmsFrame = 0;
		}
		if (m_useDSPTime)
		{
			m_dspDeltaTime = AudioSettings.dspTime - m_lastDSPTime;
		}
		else
		{
			m_dspDeltaTime = Time.deltaTime;
		}
		m_lastDSPTime = AudioSettings.dspTime;
	}

	public static float GetDeltaTime()
	{
		return (float)m_dspDeltaTime;
	}

	public int GetCurrentVoices()
	{
		return m_cachedCurrentVoices;
	}

	public int GetMaxVoices()
	{
		return m_audioSourcePoolSize;
	}

	public bool IsCloseToMax()
	{
		return m_cachedCurrentVoices > m_audioSourcePoolSize - 8;
	}

	public void RegisterListener(WingroveListener listener)
	{
		m_listeners.Add(listener);
		m_listenerCount++;
		if (m_thisListener == null)
		{
			m_thisListener = new GameObject("Listener");
			m_thisListener.transform.parent = base.transform;
			m_thisListener.AddComponent<AudioListener>();
			m_thisListener.transform.localPosition = m_listenerOffset;
		}
		m_thisListener.transform.parent = listener.transform;
		m_thisListener.transform.localPosition = Vector3.zero;
		m_thisListener.transform.localRotation = Quaternion.identity;
		m_thisListener.transform.localScale = Vector3.one;
		while (m_listeners.Contains(null))
		{
			m_listeners.Remove(null);
			m_listenerCount = m_listeners.Count;
		}
	}

	public void UnregisterListener(WingroveListener listener)
	{
		m_listeners.Remove(listener);
		m_listenerCount--;
		Transform transform = null;
		if (m_listenerCount != 0)
		{
			transform = m_listeners[m_listenerCount - 1].transform;
		}
	}

	public WingroveListener GetSingleListener()
	{
		if (m_listenerCount == 1)
		{
			return m_listeners[0];
		}
		return null;
	}

	public Vector3 GetRelativeListeningPosition(Vector3 inPosition)
	{
		return inPosition;
	}

	public Vector3 GetRelativeListeningPosition(AudioArea aa, Vector3 inPosition)
	{
		if (aa != null)
		{
			inPosition = aa.GetListeningPosition(m_listeners[0].transform.position, inPosition);
			if (inPosition == m_listeners[0].transform.position)
			{
				return m_listeners[0].transform.position;
			}
			return inPosition;
		}
		return inPosition;
	}
}
