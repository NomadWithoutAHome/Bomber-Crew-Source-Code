using System.Collections.Generic;
using UnityEngine;

namespace WingroveAudio;

public abstract class BaseWingroveAudioSource : MonoBehaviour
{
	public enum RetriggerOnSameObject
	{
		PlayAnother,
		DontPlay,
		Restart
	}

	[SerializeField]
	private bool m_looping;

	[SerializeField]
	private int m_importance;

	[SerializeField]
	[FaderInterface]
	private float m_clipMixVolume = 1f;

	[SerializeField]
	private bool m_beatSynchronizeOnStart;

	[SerializeField]
	private int m_preCacheCount = 1;

	[SerializeField]
	private bool m_is3DSound;

	[SerializeField]
	private bool m_instantRejectOnTooDistant;

	[SerializeField]
	private bool m_instantRejectHalfDistanceFewVoices;

	[SerializeField]
	private Audio3DSetting m_specify3DSettings;

	[SerializeField]
	private float m_randomVariationPitchMin = 1f;

	[SerializeField]
	private float m_randomVariationPitchMax = 1f;

	[SerializeField]
	private RetriggerOnSameObject m_retriggerOnSameObjectBehaviour;

	[SerializeField]
	private int m_parameterCurveUpdateFrequencyBase = 1;

	[SerializeField]
	private int m_parameterCurveUpdateFrequencyOffset;

	protected List<ActiveCue> m_currentActiveCues = new List<ActiveCue>(32);

	protected int m_currentActiveCuesCount;

	protected List<ActiveCue> m_toRemove = new List<ActiveCue>(32);

	protected bool m_toRemoveDirty;

	protected WingroveMixBus m_mixBus;

	protected bool m_hasMixBus;

	protected InstanceLimiter m_instanceLimiter;

	protected List<ParameterModifierBase> m_parameterModifiersLive = new List<ParameterModifierBase>();

	protected List<ParameterModifierBase> m_parameterModifiersGlobalOpt = new List<ParameterModifierBase>();

	protected List<FilterApplicationBase> m_filterApplications = new List<FilterApplicationBase>();

	private int m_frameCtr;

	private bool m_hasCachedGlobalVolume;

	private bool m_hasCachedGlobalPitch;

	private float m_cachedGlobalVolume;

	private float m_cachedGlobalPitch;

	public Audio3DSetting Get3DSettings()
	{
		if (m_is3DSound)
		{
			if (m_specify3DSettings != null)
			{
				return m_specify3DSettings;
			}
			return WingroveRoot.Instance.GetDefault3DSettings();
		}
		return null;
	}

	public List<ActiveCue> GetActiveCuesDebug()
	{
		return m_currentActiveCues;
	}

	public bool ShouldUpdateParameterCurves()
	{
		if (m_parameterCurveUpdateFrequencyBase <= 1)
		{
			return true;
		}
		int num = (m_frameCtr + m_parameterCurveUpdateFrequencyOffset) % m_parameterCurveUpdateFrequencyBase;
		return num == 0;
	}

	private void Awake()
	{
		m_mixBus = WingroveMixBus.FindParentMixBus(base.transform);
		m_instanceLimiter = WingroveMixBus.FindParentLimiter(base.transform);
		if (m_mixBus != null)
		{
			m_hasMixBus = true;
			m_mixBus.RegisterSource(this);
		}
		FindParameterModifiers(base.transform);
		FindFilterApplications(base.transform);
		WingroveRoot.Instance.RegisterAudioSource(this);
		Initialise();
	}

	private void FindParameterModifiers(Transform t)
	{
		if (t == null)
		{
			return;
		}
		ParameterModifierBase[] components = t.gameObject.GetComponents<ParameterModifierBase>();
		ParameterModifierBase[] array = components;
		foreach (ParameterModifierBase parameterModifierBase in array)
		{
			if (parameterModifierBase.IsGlobalOptimised())
			{
				m_parameterModifiersGlobalOpt.Add(parameterModifierBase);
			}
			else
			{
				m_parameterModifiersLive.Add(parameterModifierBase);
			}
		}
		FindParameterModifiers(t.parent);
	}

	private void FindFilterApplications(Transform t)
	{
		if (!(t == null))
		{
			FilterApplicationBase[] components = t.gameObject.GetComponents<FilterApplicationBase>();
			FilterApplicationBase[] array = components;
			foreach (FilterApplicationBase item in array)
			{
				m_filterApplications.Add(item);
			}
			FindFilterApplications(t.parent);
		}
	}

	public float GetPitchModifier(int goId)
	{
		if (!m_hasCachedGlobalPitch)
		{
			m_cachedGlobalPitch = 1f;
			List<ParameterModifierBase>.Enumerator enumerator = m_parameterModifiersGlobalOpt.GetEnumerator();
			while (enumerator.MoveNext())
			{
				ParameterModifierBase current = enumerator.Current;
				m_cachedGlobalPitch *= current.GetPitchMultiplier(goId);
			}
			m_hasCachedGlobalPitch = true;
		}
		float num = m_cachedGlobalPitch;
		List<ParameterModifierBase>.Enumerator enumerator2 = m_parameterModifiersLive.GetEnumerator();
		while (enumerator2.MoveNext())
		{
			ParameterModifierBase current2 = enumerator2.Current;
			num *= current2.GetPitchMultiplier(goId);
		}
		return num;
	}

	public float GetVolumeModifier(int goId)
	{
		if (!m_hasCachedGlobalVolume)
		{
			m_cachedGlobalVolume = 1f;
			List<ParameterModifierBase>.Enumerator enumerator = m_parameterModifiersGlobalOpt.GetEnumerator();
			while (enumerator.MoveNext())
			{
				ParameterModifierBase current = enumerator.Current;
				m_cachedGlobalVolume *= current.GetVolumeMultiplier(goId);
			}
			m_hasCachedGlobalVolume = true;
		}
		float num = m_clipMixVolume * m_cachedGlobalVolume;
		List<ParameterModifierBase>.Enumerator enumerator2 = m_parameterModifiersLive.GetEnumerator();
		while (enumerator2.MoveNext())
		{
			ParameterModifierBase current2 = enumerator2.Current;
			num *= current2.GetVolumeMultiplier(goId);
		}
		return num;
	}

	public void UpdateFilters(PooledAudioSource targetPlayer, int goId)
	{
		targetPlayer.ResetFiltersForFrame();
		List<FilterApplicationBase>.Enumerator enumerator = m_filterApplications.GetEnumerator();
		while (enumerator.MoveNext())
		{
			FilterApplicationBase current = enumerator.Current;
			current.UpdateFor(targetPlayer, goId);
		}
		targetPlayer.CommitFiltersForFrame();
	}

	public bool IsPlaying()
	{
		return m_currentActiveCuesCount > 0;
	}

	public float GetCurrentTime()
	{
		float num = 0f;
		foreach (ActiveCue currentActiveCue in m_currentActiveCues)
		{
			num = Mathf.Max(currentActiveCue.GetTime(), num);
		}
		return num;
	}

	public float GetTimeUntilFinished(WingroveGroupInformation.HandleRepeatingAudio handleRepeats)
	{
		float num = 0f;
		foreach (ActiveCue currentActiveCue in m_currentActiveCues)
		{
			num = Mathf.Max(currentActiveCue.GetTimeUntilFinished(handleRepeats), num);
		}
		return num;
	}

	public void DoUpdate(int frameCtr)
	{
		m_frameCtr = frameCtr;
		m_hasCachedGlobalPitch = false;
		m_hasCachedGlobalVolume = false;
		if (m_currentActiveCuesCount > 0)
		{
			UpdateInternal();
		}
	}

	protected void UpdateInternal()
	{
		List<ActiveCue>.Enumerator enumerator = m_currentActiveCues.GetEnumerator();
		while (enumerator.MoveNext())
		{
			ActiveCue current = enumerator.Current;
			current.Update();
		}
		if (!m_toRemoveDirty)
		{
			return;
		}
		foreach (ActiveCue item in m_toRemove)
		{
			m_currentActiveCues.Remove(item);
			if (m_instanceLimiter != null)
			{
				m_instanceLimiter.RemoveCue(item);
			}
			m_currentActiveCuesCount--;
		}
		m_toRemove.Clear();
		m_toRemoveDirty = false;
	}

	private void OnDestroy()
	{
		if (m_hasMixBus)
		{
			m_mixBus.RemoveSource(this);
		}
	}

	public float GetClipMixVolume()
	{
		return m_clipMixVolume;
	}

	public int GetImportance()
	{
		if (!m_hasMixBus)
		{
			return m_importance;
		}
		return m_importance + m_mixBus.GetImportance();
	}

	public ActiveCue GetCueForGameObject(GameObject go)
	{
		int instanceID = go.GetInstanceID();
		foreach (ActiveCue currentActiveCue in m_currentActiveCues)
		{
			if (currentActiveCue.GetTargetObjectId() == instanceID && currentActiveCue.GetState() != ActiveCue.CueState.PlayingFadeOut && currentActiveCue.GetState() != ActiveCue.CueState.Stopped)
			{
				return currentActiveCue;
			}
		}
		foreach (ActiveCue currentActiveCue2 in m_currentActiveCues)
		{
			if (currentActiveCue2.GetTargetObjectId() == instanceID)
			{
				return currentActiveCue2;
			}
		}
		return null;
	}

	public float GetRMS()
	{
		float num = 0f;
		foreach (ActiveCue currentActiveCue in m_currentActiveCues)
		{
			float rMS = currentActiveCue.GetRMS();
			num += rMS * rMS;
		}
		return Mathf.Sqrt(num);
	}

	public float GetMixBusLevel()
	{
		if (m_hasMixBus)
		{
			return m_mixBus.GetMixedVol();
		}
		return 1f;
	}

	public float GetNewPitch()
	{
		return Random.Range(m_randomVariationPitchMin, m_randomVariationPitchMax);
	}

	public float GetPitchMax()
	{
		return m_randomVariationPitchMax;
	}

	public bool GetLooping()
	{
		return m_looping;
	}

	public ActiveCue Stop(ActiveCue cue, float fade)
	{
		if (cue == null)
		{
			foreach (ActiveCue currentActiveCue in m_currentActiveCues)
			{
				currentActiveCue.Stop(fade);
			}
			return cue;
		}
		cue.Stop(fade);
		return cue;
	}

	private ActiveCue GetNextCue()
	{
		return new ActiveCue();
	}

	public void RePool(ActiveCue cue)
	{
		m_toRemove.Add(cue);
		m_toRemoveDirty = true;
	}

	public ActiveCue Play(ActiveCue cue, float fade, GameObject target, AudioArea aa)
	{
		if (m_instanceLimiter == null || m_instanceLimiter.CanPlay(target))
		{
			if (cue == null || m_retriggerOnSameObjectBehaviour == RetriggerOnSameObject.PlayAnother)
			{
				bool flag = false;
				if (m_is3DSound && m_instantRejectOnTooDistant && base.gameObject != null)
				{
					Vector3 relativeListeningPosition = WingroveRoot.Instance.GetRelativeListeningPosition(target.transform.position);
					float magnitude = (WingroveRoot.Instance.GetSingleListener().transform.position - relativeListeningPosition).magnitude;
					float maxDistance = Get3DSettings().GetMaxDistance();
					if (magnitude > maxDistance)
					{
						flag = true;
					}
					else if (m_instantRejectHalfDistanceFewVoices && magnitude > maxDistance * 0.5f && WingroveRoot.Instance.IsCloseToMax())
					{
						flag = true;
					}
				}
				if (!flag)
				{
					cue = GetNextCue();
					cue.Initialise(base.gameObject, this, target, aa);
					m_currentActiveCues.Add(cue);
					m_currentActiveCuesCount++;
					if (m_beatSynchronizeOnStart)
					{
						BeatSyncSource current = BeatSyncSource.GetCurrent();
						if (current != null)
						{
							cue.Play(fade, current.GetNextBeatTime());
						}
						else
						{
							cue.Play(fade);
						}
					}
					else
					{
						cue.Play(fade);
					}
					if (m_instanceLimiter != null)
					{
						m_instanceLimiter.AddCue(cue, target);
					}
				}
			}
			else if (m_retriggerOnSameObjectBehaviour != RetriggerOnSameObject.DontPlay)
			{
				cue.Play(fade);
			}
		}
		return cue;
	}

	public ActiveCue Play(ActiveCue cue, float fade, GameObject target)
	{
		if (m_instanceLimiter == null || m_instanceLimiter.CanPlay(target))
		{
			if (cue == null || m_retriggerOnSameObjectBehaviour == RetriggerOnSameObject.PlayAnother)
			{
				bool flag = false;
				if (m_is3DSound && m_instantRejectOnTooDistant && base.gameObject != null)
				{
					Vector3 relativeListeningPosition = WingroveRoot.Instance.GetRelativeListeningPosition(target.transform.position);
					float magnitude = (WingroveRoot.Instance.GetSingleListener().transform.position - relativeListeningPosition).magnitude;
					float maxDistance = Get3DSettings().GetMaxDistance();
					if (magnitude > maxDistance)
					{
						flag = true;
					}
					else if (m_instantRejectHalfDistanceFewVoices && magnitude > maxDistance * 0.5f && WingroveRoot.Instance.IsCloseToMax())
					{
						flag = true;
					}
				}
				if (!flag)
				{
					cue = GetNextCue();
					cue.Initialise(base.gameObject, this, target);
					m_currentActiveCues.Add(cue);
					m_currentActiveCuesCount++;
					if (m_beatSynchronizeOnStart)
					{
						BeatSyncSource current = BeatSyncSource.GetCurrent();
						if (current != null)
						{
							cue.Play(fade, current.GetNextBeatTime());
						}
						else
						{
							cue.Play(fade);
						}
					}
					else
					{
						cue.Play(fade);
					}
					if (m_instanceLimiter != null)
					{
						m_instanceLimiter.AddCue(cue, target);
					}
				}
			}
			else if (m_retriggerOnSameObjectBehaviour != RetriggerOnSameObject.DontPlay)
			{
				cue.Play(fade);
			}
		}
		return cue;
	}

	public ActiveCue Pause(ActiveCue cue)
	{
		if (cue == null)
		{
			foreach (ActiveCue currentActiveCue in m_currentActiveCues)
			{
				currentActiveCue.Pause();
			}
			return cue;
		}
		cue.Pause();
		return cue;
	}

	public ActiveCue Unpause(ActiveCue cue)
	{
		if (cue == null)
		{
			foreach (ActiveCue currentActiveCue in m_currentActiveCues)
			{
				currentActiveCue.Unpause();
			}
			return cue;
		}
		cue.Unpause();
		return cue;
	}

	public virtual AudioClip GetAudioClip()
	{
		Debug.LogError("Using null implementation");
		return null;
	}

	public abstract void RemoveUsage();

	public abstract void AddUsage();

	public virtual void Initialise()
	{
	}
}
