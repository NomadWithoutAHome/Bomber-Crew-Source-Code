using BomberCrewCommon;
using UnityEngine;

public class MissionTargetDam : SmoothDamageable
{
	[SerializeField]
	private TaggableItem m_taggableItem;

	[SerializeField]
	private float m_startHP;

	[SerializeField]
	private int m_xpForDestroy;

	[SerializeField]
	private Collider m_collider;

	[SerializeField]
	private GameObject m_model;

	[SerializeField]
	private GameObject m_modelDestroyed;

	[SerializeField]
	private Texture2D m_icon;

	[SerializeField]
	private MissionPlaceableObject m_placeable;

	[SerializeField]
	private Transform[] m_bomberApproachPath;

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

	private void Start()
	{
		m_taggableItem.OnTagComplete += OnTagComplete;
		m_bomberNavigation = Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBomberNavigation();
		m_hp = m_startHP;
		m_model.SetActive(value: true);
		m_modelDestroyed.SetActive(value: false);
		m_collider.enabled = true;
		m_isOptionalTarget = m_placeable.GetParameter("optionalTarget") == "true";
		m_noObjective = m_placeable.GetParameter("noObjective") == "true";
		m_noTagging = m_placeable.GetParameter("noTagging") == "true";
		if (!m_isOptionalTarget)
		{
			if (!m_noObjective)
			{
				ObjectiveManager.Objective objective = new ObjectiveManager.Objective();
				objective.m_countToComplete = 1;
				objective.m_objectiveTitle = "mission_objective_destroy_dam";
				objective.m_primary = true;
				objective.m_objectiveGroup = "TARG_" + base.name;
				objective.m_objectiveType = ObjectiveManager.ObjectiveType.BombTarget;
				m_thisObjective = Singleton<ObjectiveManager>.Instance.RegisterObjective(objective);
				m_thisLogObjective = new MissionLog.LogObjective();
				Singleton<GameFlow>.Instance.GetCurrentMissionInfo().GetMissionLog().LogObjectiveNew(m_thisLogObjective);
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
		BomberNavigation.NavigationPoint navigationPoint = null;
		BomberNavigation.NavigationPoint navigationPoint2 = null;
		for (int i = 0; i < m_bomberApproachPath.Length; i++)
		{
			BomberNavigation.NavigationPoint navigationPoint3 = new BomberNavigation.NavigationPoint();
			navigationPoint3.m_position = new Vector3d(base.transform.localToWorldMatrix.MultiplyVector(m_bomberApproachPath[i].localPosition)) + base.gameObject.btransform().position;
			navigationPoint3.m_hasDirection = false;
			navigationPoint3.m_switchToLandingState = false;
			navigationPoint3.m_holdSteady = false;
			navigationPoint3.m_navPointType = BomberNavigation.NavigationPointType.FunctionLandingBombing;
			navigationPoint3.m_associatedObject = base.gameObject;
			if (i == 0)
			{
				navigationPoint = navigationPoint3;
			}
			else if (i != m_bomberApproachPath.Length - 1)
			{
				navigationPoint3.m_forceUltraLowAltitude = true;
			}
			if (navigationPoint2 != null)
			{
				navigationPoint2.m_next = navigationPoint3;
			}
			navigationPoint2 = navigationPoint3;
		}
		m_bomberNavigation.SetNavigationPoint(navigationPoint);
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
			m_model.SetActive(value: false);
			m_modelDestroyed.SetActive(value: true);
			m_collider.enabled = false;
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
			if (!Singleton<GameFlow>.Instance.GetCurrentMissionInfo().GetCurrentlySelectedMissionDetails().m_isTrainingMission)
			{
				if (m_targetDestroyedStat != null)
				{
					m_targetDestroyedStat.ModifyValue(1);
				}
				if (m_submarineDestroyedStat != null && GetComponent<Submarine>() != null)
				{
					m_submarineDestroyedStat.ModifyValue(1);
				}
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

	public override float GetHealthNormalised()
	{
		return Mathf.Clamp01(m_hp / m_startHP);
	}
}
