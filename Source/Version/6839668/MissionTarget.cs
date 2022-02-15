using System;
using BomberCrewCommon;
using dbox;
using UnityEngine;

public class MissionTarget : SmoothDamageable
{
	[SerializeField]
	private TaggableItem m_taggableItem;

	[SerializeField]
	private float m_startHP;

	[SerializeField]
	private int m_xpForDestroy;

	[SerializeField]
	private GameObject m_model;

	[SerializeField]
	private GameObject m_modelDestroyed;

	[SerializeField]
	private bool m_useDestroyAnimInstead;

	[SerializeField]
	private Animator m_animator;

	[SerializeField]
	private string m_animDestroyBoolName;

	[SerializeField]
	private MissionPlaceableObject m_placeable;

	[SerializeField]
	[NamedText]
	private string m_objectiveText = "mission_objective_destroy_target";

	[SerializeField]
	private GameObject m_extraExplosionPrefab;

	[SerializeField]
	private bool m_isSubmarine;

	[SerializeField]
	private bool m_useNewFollowTaggingNavPoint;

	private BomberNavigation m_bomberNavigation;

	private float m_hp;

	private bool m_destroyed;

	private ObjectiveManager.Objective m_thisObjective;

	private MissionLog.LogObjective m_thisLogObjective;

	private bool m_isOptionalTarget;

	private bool m_noObjective;

	private bool m_noTagging;

	private AchievementSystem.UserStat m_targetDestroyedStat;

	private AchievementSystem.UserStat m_submarineDestroyedStat;

	private BomberNavigation.NavigationPoint m_myNavigationPoint;

	private void Start()
	{
		m_myNavigationPoint = new BomberNavigation.NavigationPoint();
		m_myNavigationPoint.m_hasDirection = false;
		m_myNavigationPoint.m_switchToLandingState = false;
		m_myNavigationPoint.m_holdSteady = false;
		m_myNavigationPoint.m_navPointType = BomberNavigation.NavigationPointType.FunctionLandingBombing;
		m_myNavigationPoint.m_associatedObject = base.gameObject;
		m_taggableItem.OnTagComplete += OnTagComplete;
		m_bomberNavigation = Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBomberNavigation();
		m_hp = m_startHP;
		if (m_useDestroyAnimInstead)
		{
			m_animator.SetBool(m_animDestroyBoolName, value: false);
		}
		else
		{
			m_model.SetActive(value: true);
			m_modelDestroyed.SetActive(value: false);
		}
		m_isOptionalTarget = m_placeable.GetParameter("optionalTarget") == "true";
		m_noObjective = m_placeable.GetParameter("noObjective") == "true";
		m_noTagging = m_placeable.GetParameter("noTagging") == "true";
		if (!m_isOptionalTarget)
		{
			if (!m_noObjective)
			{
				ObjectiveManager.Objective newObj = new ObjectiveManager.Objective();
				newObj.m_countToComplete = 1;
				newObj.m_objectiveTitle = ((!string.IsNullOrEmpty(m_objectiveText)) ? m_objectiveText : "mission_objective_destroy_target");
				newObj.m_primary = true;
				newObj.m_objectiveGroup = "TARG_" + base.name;
				newObj.m_objectiveType = ObjectiveManager.ObjectiveType.BombTarget;
				m_thisLogObjective = new MissionLog.LogObjective();
				string addObjectiveOn = m_placeable.GetParameter("objectiveOnTrigger");
				if (string.IsNullOrEmpty(addObjectiveOn))
				{
					Singleton<GameFlow>.Instance.GetCurrentMissionInfo().GetMissionLog().LogObjectiveNew(m_thisLogObjective);
					m_thisObjective = Singleton<ObjectiveManager>.Instance.RegisterObjective(newObj);
				}
				else
				{
					MissionCoordinator instance = Singleton<MissionCoordinator>.Instance;
					instance.OnTrigger = (Action<string>)Delegate.Combine(instance.OnTrigger, (Action<string>)delegate(string st)
					{
						if (st == addObjectiveOn)
						{
							Singleton<GameFlow>.Instance.GetCurrentMissionInfo().GetMissionLog().LogObjectiveNew(m_thisLogObjective);
							m_thisObjective = Singleton<ObjectiveManager>.Instance.RegisterObjective(newObj);
							if (m_placeable.GetParameter("failIfNoBombs") == "true" && Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBombBay().GetBombLoad() != null && Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBombBay().GetBombLoad()
								.HasDropped())
							{
								Singleton<ObjectiveManager>.Instance.FailObjective(m_thisObjective);
							}
						}
					});
				}
			}
		}
		else
		{
			m_taggableItem.SetTagType(TaggableItem.TaggableType.BombTargetOptional);
		}
		if (m_noTagging && GetComponent<MapNavigatorDisplayable>() != null && GetComponent<MapNavigatorDisplayable>().GetDescriptionType() != 0)
		{
			GetComponent<MapNavigatorDisplayable>().SetShouldDisplay(shouldDisplay: false);
		}
		m_targetDestroyedStat = Singleton<AchievementSystem>.Instance.GetUserStat("TargetsDestroyed");
		m_submarineDestroyedStat = Singleton<AchievementSystem>.Instance.GetUserStat("SubmarinesDestroyed");
	}

	private void OnTagComplete()
	{
		Singleton<CrewMiscSpeechTriggers>.Instance.DoExternalTrigger(CrewMiscSpeechTriggers.SpeechExternalTrigger.BombTargetTagged, null);
		if (m_useNewFollowTaggingNavPoint)
		{
			m_bomberNavigation.SetNavigationPoint(m_myNavigationPoint);
		}
		else
		{
			m_bomberNavigation.SetNavigationPoint(base.gameObject.btransform().position, BomberNavigation.NavigationPointType.FunctionLandingBombing, base.gameObject);
		}
	}

	public override bool IsDamageBlocker()
	{
		return false;
	}

	public override float DamageGetPassthrough(float amt, DamageSource ds)
	{
		m_hp -= amt;
		if (!m_destroyed && m_hp <= 0f)
		{
			m_destroyed = true;
			if (m_useDestroyAnimInstead)
			{
				m_animator.SetBool(m_animDestroyBoolName, value: true);
			}
			else
			{
				m_model.SetActive(value: false);
				m_modelDestroyed.SetActive(value: true);
			}
			if (m_extraExplosionPrefab != null)
			{
				DboxInMissionController.DBoxCall(DboxSdkWrapper.PostExplosion, base.transform.position);
				GameObject fromPoolSlow = Singleton<PoolManager>.Instance.GetFromPoolSlow(m_extraExplosionPrefab);
				fromPoolSlow.btransform().SetFromCurrentPage(base.transform.position);
			}
			Singleton<GameFlow>.Instance.GetCurrentMissionInfo().GetMissionLog().SetTargetDestroyed(m_xpForDestroy);
			Singleton<EndlessModeGameFlow>.Instance.GetEndlessModeResult().m_totalScore += m_xpForDestroy;
			Singleton<MissionXPCounter>.Instance.AddXP(m_xpForDestroy, base.transform, base.transform.position);
			if (m_thisLogObjective != null)
			{
				m_thisLogObjective.m_isComplete = true;
			}
			if (!m_isOptionalTarget && !m_noObjective)
			{
				Singleton<ObjectiveManager>.Instance.CompleteObjective(m_thisObjective);
			}
			if (m_targetDestroyedStat != null)
			{
				m_targetDestroyedStat.ModifyValue(1);
			}
			if (m_submarineDestroyedStat != null && m_isSubmarine)
			{
				m_submarineDestroyedStat.ModifyValue(1);
			}
			Singleton<CrewMiscSpeechTriggers>.Instance.DoExternalTrigger(CrewMiscSpeechTriggers.SpeechExternalTrigger.BombTargetDestroyed, null);
			if (Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBombLoad().IsGrandSlam() && Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBombLoad().HasDropped())
			{
				AchievementSystem.Achievement achievement = Singleton<AchievementSystem>.Instance.GetAchievement("GrandSlam");
				if (achievement != null)
				{
					Singleton<AchievementSystem>.Instance.AwardAchievement(achievement);
				}
			}
			string parameter = m_placeable.GetParameter("destroyedTrigger");
			if (!string.IsNullOrEmpty(parameter))
			{
				Singleton<MissionCoordinator>.Instance.FireTrigger(parameter);
			}
		}
		return 0f;
	}

	private void Update()
	{
		if (m_useNewFollowTaggingNavPoint)
		{
			m_myNavigationPoint.m_position = base.gameObject.btransform().position;
		}
		if (m_noTagging)
		{
			m_taggableItem.SetTaggable(taggable: false);
		}
		else if (Singleton<MissionCoordinator>.Instance.IsOutwardJourney() && !m_destroyed)
		{
			m_taggableItem.SetTaggable(taggable: true);
			if (m_bomberNavigation.GetAssociatedObject() != base.gameObject)
			{
				m_taggableItem.SetTagIncomplete();
			}
		}
		else
		{
			m_taggableItem.SetTaggable(taggable: false);
		}
	}

	public float GetTotalHealth()
	{
		return m_startHP;
	}

	public override float GetHealthNormalised()
	{
		return Mathf.Clamp01(m_hp / m_startHP);
	}
}
