using System;
using BomberCrewCommon;
using UnityEngine;
using WingroveAudio;

public class TaggableItem : MonoBehaviour
{
	public enum VisibilityType
	{
		RadarAssisted,
		NavigatorAssisted,
		None,
		AlwaysVisibleIfVisible
	}

	public enum TaggableType
	{
		Fighter,
		NavigationPoint,
		BombTarget,
		AirBase,
		PhotoSite,
		FighterAce,
		NavigationCustom,
		BombTargetOptional,
		Defend,
		SearchRescue,
		SearchSpot,
		EndlessUpgrade
	}

	[SerializeField]
	private TaggableType m_taggableType;

	[SerializeField]
	private bool m_autoStart;

	[SerializeField]
	private bool m_useFullVisibility;

	[SerializeField]
	private VisibilityType m_visibilityType;

	[SerializeField]
	private bool m_isRadarOnGround;

	[SerializeField]
	private Transform m_toTrackTransform;

	[SerializeField]
	private bool m_isFunctional;

	[SerializeField]
	private bool m_dontAnnounceOnRadar;

	private bool m_taggable;

	private bool m_taggableAndVisible;

	private bool m_isTagComplete;

	private BomberSystems m_currentBomber;

	public event Action OnTagComplete;

	private void OnEnable()
	{
		if (m_autoStart)
		{
			SetTaggable(taggable: true);
		}
	}

	private void OnDisable()
	{
		SetTagIncomplete();
		SetTaggable(taggable: false);
	}

	public bool IsFunctional()
	{
		return m_isFunctional;
	}

	public void SetTagType(TaggableType newTag)
	{
		m_taggableType = newTag;
	}

	public void SetIsFunctional(bool functional)
	{
		m_isFunctional = functional;
	}

	public void SetTaggable(bool taggable)
	{
		if (m_taggable != taggable)
		{
			m_taggable = taggable;
			if (!m_taggable)
			{
				SetTagIncomplete();
			}
			if (m_taggableAndVisible && !m_taggable)
			{
				ChangeTagState(taggableAndVisible: false);
			}
		}
	}

	public bool IsRadarOnGround()
	{
		return m_isRadarOnGround;
	}

	public bool DontAnnounceOnRadar()
	{
		return m_dontAnnounceOnRadar;
	}

	public VisibilityType GetVisibilityType()
	{
		return m_visibilityType;
	}

	private void ChangeTagState(bool taggableAndVisible)
	{
		if (taggableAndVisible != m_taggableAndVisible)
		{
			m_taggableAndVisible = taggableAndVisible;
			if (m_taggableAndVisible)
			{
				Singleton<TagManager>.Instance.RegisterTaggable(this);
			}
			else if (Singleton<TagManager>.Instance != null)
			{
				Singleton<TagManager>.Instance.DeRegisterTaggable(this);
			}
		}
	}

	public bool IsTaggableAndVisible()
	{
		return m_taggableAndVisible;
	}

	private void Update()
	{
		if (m_currentBomber == null)
		{
			m_currentBomber = Singleton<BomberSpawn>.Instance.GetBomberSystems();
		}
		if (m_taggable)
		{
			bool flag = Singleton<VisibilityHelpers>.Instance.IsVisibleHumanPlayer(m_currentBomber.transform.position, base.transform.position, m_currentBomber, m_visibilityType == VisibilityType.RadarAssisted, m_visibilityType == VisibilityType.NavigatorAssisted);
			if (m_visibilityType == VisibilityType.AlwaysVisibleIfVisible)
			{
				flag = (m_currentBomber.transform.position - base.transform.position).magnitude < 4000f;
			}
			if (m_taggableAndVisible != flag)
			{
				ChangeTagState(flag);
			}
		}
		else if (m_taggableAndVisible)
		{
			ChangeTagState(taggableAndVisible: false);
		}
	}

	public void SetTagIncomplete()
	{
		m_isTagComplete = false;
	}

	public void SetTagComplete()
	{
		WingroveRoot.Instance.PostEvent("TARGET_LOCKED");
		m_isTagComplete = true;
		if (this.OnTagComplete != null)
		{
			this.OnTagComplete();
		}
	}

	public bool IsFullyTagged()
	{
		return m_isTagComplete;
	}

	public TaggableType GetTaggableType()
	{
		return m_taggableType;
	}

	public Transform GetTrackingTramsform()
	{
		return (!(m_toTrackTransform == null)) ? m_toTrackTransform : base.transform;
	}
}
