using BomberCrewCommon;
using UnityEngine;

public class TagUIProgress : MonoBehaviour
{
	[SerializeField]
	private tk2dRadialSprite m_progressItem;

	[SerializeField]
	private WorldToScreenTracker m_tracker;

	[SerializeField]
	private GameObject m_visibleHierarchy;

	[SerializeField]
	private bool m_onlyVisibleInTargetingMode;

	[SerializeField]
	private float m_targetingModePreTimeShow;

	[SerializeField]
	private float m_targetingModeFadeTime;

	[SerializeField]
	private tk2dBaseSprite[] m_spritesToFade;

	[SerializeField]
	private float m_nonTargetingModeAlpha;

	[SerializeField]
	private float m_targetingModeCentreSize = 640f;

	[SerializeField]
	private Transform m_tagPositionTransform;

	[SerializeField]
	private Animation m_taggedAnim;

	[SerializeField]
	private AnimationCurve m_progressCurve;

	[SerializeField]
	private TagOffScreenMarker m_offscreenMarker;

	private float m_tagProgress;

	private float m_distance;

	private bool m_visible;

	private float m_targetingModeTimer;

	private float m_previouslySetTagProgress;

	private bool m_hasEverSetTagProgress;

	private Transform m_target;

	private TaggableItem m_taggableItem;

	public void SetUp(TaggableItem taggableItem, Transform target, tk2dCamera uiCamera)
	{
		m_taggableItem = taggableItem;
		if (m_tracker != null)
		{
			m_tracker.SetTracking(target, uiCamera);
		}
		Update();
	}

	public TaggableItem GetTaggableItem()
	{
		return m_taggableItem;
	}

	public void SetVisible(bool isVisible)
	{
		m_visible = isVisible;
	}

	private void OnDisable()
	{
		Object.Destroy(base.gameObject);
	}

	public bool IsVisible()
	{
		if (m_onlyVisibleInTargetingMode)
		{
			return m_visible && m_targetingModeTimer > m_targetingModeFadeTime + m_targetingModePreTimeShow;
		}
		return m_visible;
	}

	public bool IsOffscreenMarkerVisible()
	{
		if ((m_offscreenMarker != null && m_offscreenMarker.IsDisplayedCurrently()) || m_nonTargetingModeAlpha > 0f)
		{
			return true;
		}
		return false;
	}

	private void Update()
	{
		m_distance = m_tagPositionTransform.position.magnitude;
		if (m_progressItem != null && (!m_hasEverSetTagProgress || m_tagProgress != m_previouslySetTagProgress))
		{
			m_progressItem.SetValue(m_progressCurve.Evaluate(1f - m_tagProgress));
			m_hasEverSetTagProgress = true;
			m_previouslySetTagProgress = m_tagProgress;
		}
		m_visibleHierarchy.SetActive(m_visible);
		if (m_onlyVisibleInTargetingMode)
		{
			if (Singleton<ContextControl>.Instance.IsTargetingMode() && m_distance < m_targetingModeCentreSize)
			{
				m_targetingModeTimer += Time.deltaTime;
			}
			else
			{
				m_targetingModeTimer = 0f;
			}
			float t = Mathf.Clamp01((m_targetingModeTimer - m_targetingModePreTimeShow) / m_targetingModeFadeTime);
			tk2dBaseSprite[] spritesToFade = m_spritesToFade;
			foreach (tk2dBaseSprite tk2dBaseSprite2 in spritesToFade)
			{
				Color color = tk2dBaseSprite2.color;
				color.a = Mathf.Lerp(m_nonTargetingModeAlpha, 1f, t);
				tk2dBaseSprite2.color = color;
			}
		}
	}

	public float GetProgress()
	{
		return Mathf.Clamp01(m_tagProgress);
	}

	public float GetCentreDistance()
	{
		return m_distance;
	}

	public void Progress(float amt)
	{
		m_tagProgress = Mathf.Clamp01(m_tagProgress + amt);
		if (!m_hasEverSetTagProgress || m_tagProgress != m_previouslySetTagProgress)
		{
			if (m_progressItem != null)
			{
				m_progressItem.SetValue(m_progressCurve.Evaluate(1f - m_tagProgress));
			}
			m_hasEverSetTagProgress = true;
			m_previouslySetTagProgress = m_tagProgress;
		}
	}

	public bool ShouldTag()
	{
		return m_tagProgress == 1f;
	}
}
