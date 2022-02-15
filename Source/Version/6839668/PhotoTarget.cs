using System;
using BomberCrewCommon;
using UnityEngine;

public class PhotoTarget : MonoBehaviour
{
	[Serializable]
	public class MoneyReward
	{
		[SerializeField]
		public int m_intel;

		[SerializeField]
		public int m_moneyReward;
	}

	[SerializeField]
	private TaggableItem m_taggableItem;

	[SerializeField]
	private Collider m_boundsCollider;

	[SerializeField]
	private int m_xpReward;

	[SerializeField]
	private int m_intelReward;

	[SerializeField]
	private MoneyReward[] m_moneyRewards;

	[SerializeField]
	private MissionPlaceableObject m_placeable;

	[SerializeField]
	private GameObject m_photoSiteOptionalMapMarker;

	[SerializeField]
	private MapNavigatorDisplayable m_mapDisplayable;

	[SerializeField]
	private SmoothDamageable m_damageableToMonitor;

	private bool m_hasBeenPhotographed;

	private BomberNavigation m_bomberNavigation;

	private string m_triggerOnPhoto;

	private bool m_tagAllowed = true;

	private bool m_isOptional;

	private bool m_hasDamageable;

	private void Start()
	{
		m_taggableItem.OnTagComplete += OnTagComplete;
		m_bomberNavigation = Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBomberNavigation();
		m_triggerOnPhoto = m_placeable.GetParameter("triggerOnPhoto");
		m_isOptional = m_placeable.GetParameter("optional") == "true";
		if (m_mapDisplayable != null && m_isOptional)
		{
			m_mapDisplayable.SetDisplayablePrefab(m_photoSiteOptionalMapMarker, MapNavigatorDisplayable.NavigatorDescriptionType.PhotoSiteOptional);
		}
		if (m_isOptional)
		{
			m_taggableItem.SetIsFunctional(functional: false);
		}
		string allowTag = m_placeable.GetParameter("enableTagTrigger");
		if (!string.IsNullOrEmpty(allowTag))
		{
			m_tagAllowed = false;
			MissionCoordinator instance = Singleton<MissionCoordinator>.Instance;
			instance.OnTrigger = (Action<string>)Delegate.Combine(instance.OnTrigger, (Action<string>)delegate(string st)
			{
				if (st == allowTag)
				{
					m_tagAllowed = true;
				}
			});
		}
		else
		{
			m_tagAllowed = true;
		}
		Singleton<BomberSpawn>.Instance.GetBomberSystems().GetPhotoCamera().RegisterPhotoTarget(this);
		m_hasDamageable = m_damageableToMonitor != null;
	}

	private void OnTagComplete()
	{
		Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBomberNavigation().SetNavigationPoint(base.gameObject.btransform().position, BomberNavigation.NavigationPointType.Detour, base.gameObject);
	}

	public bool GetHasBeenPhotographed()
	{
		return m_hasBeenPhotographed;
	}

	public int GetMoneyReward()
	{
		int intel = Singleton<SaveDataContainer>.Instance.Get().GetIntel();
		int num = 0;
		int result = 0;
		MoneyReward[] moneyRewards = m_moneyRewards;
		foreach (MoneyReward moneyReward in moneyRewards)
		{
			if (intel >= moneyReward.m_intel && moneyReward.m_intel >= num)
			{
				num = moneyReward.m_intel;
				result = moneyReward.m_moneyReward;
			}
		}
		return result;
	}

	public void SetHasBeenPhotographed()
	{
		if (!m_hasBeenPhotographed && base.enabled)
		{
			AchievementSystem.Achievement achievement = Singleton<AchievementSystem>.Instance.GetAchievement("TakePhoto");
			if (achievement != null)
			{
				Singleton<AchievementSystem>.Instance.AwardAchievement(achievement);
			}
			if (m_mapDisplayable != null)
			{
				m_mapDisplayable.SetShouldDisplay(shouldDisplay: false);
			}
			m_hasBeenPhotographed = true;
			if (m_isOptional)
			{
				Singleton<GameFlow>.Instance.GetCurrentMissionInfo().GetMissionLog().AddValidPhotoTaken(m_xpReward, m_intelReward, GetMoneyReward());
			}
			else
			{
				Singleton<GameFlow>.Instance.GetCurrentMissionInfo().GetMissionLog().AddValidPhotoTaken(m_xpReward, 0, 0);
			}
			Singleton<MissionXPCounter>.Instance.AddXP(m_xpReward, base.transform, base.transform.position);
			if (!string.IsNullOrEmpty(m_triggerOnPhoto))
			{
				Singleton<MissionCoordinator>.Instance.FireTrigger(m_triggerOnPhoto);
			}
		}
	}

	public Bounds GetBounds()
	{
		return m_boundsCollider.bounds;
	}

	private void Update()
	{
		if (m_hasDamageable && m_damageableToMonitor.GetHealthNormalised() <= 0f && !m_hasBeenPhotographed)
		{
			m_hasBeenPhotographed = true;
			if (!m_isOptional)
			{
				Singleton<MissionCoordinator>.Instance.FireTrigger("NONOPTIONAL_PHOTO_DESTROY");
			}
		}
		if (!m_hasBeenPhotographed)
		{
			m_taggableItem.SetTaggable(m_tagAllowed);
			if (m_bomberNavigation.GetAssociatedObject() != base.gameObject)
			{
				m_taggableItem.SetTagIncomplete();
			}
		}
		else
		{
			m_taggableItem.SetTagIncomplete();
			m_taggableItem.SetTaggable(taggable: false);
		}
	}
}
