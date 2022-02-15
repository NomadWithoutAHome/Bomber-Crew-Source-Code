using BomberCrewCommon;
using UnityEngine;

public class NonBombDroppableTargetArea : MonoBehaviour
{
	public enum ExpectedDropType
	{
		Flyers,
		RescueKit
	}

	[SerializeField]
	private ExpectedDropType m_expectedDrop;

	[SerializeField]
	private float m_radius;

	[SerializeField]
	private TaggableItem m_taggableItem;

	[SerializeField]
	private MissionPlaceableObject m_placeable;

	[SerializeField]
	private bool m_requiresTagOnly;

	[SerializeField]
	private bool m_searchAndRescueHint;

	private bool m_isSupplied;

	private BomberNavigation m_bomberNavigation;

	private bool m_noObjective;

	private MissionLog.LogObjective m_thisLogObjective;

	private ObjectiveManager.Objective m_thisObjective;

	[SerializeField]
	[NamedText]
	private string m_objectiveTitle = "mission_objective_drop_supplies";

	private void Start()
	{
		m_taggableItem.OnTagComplete += OnTagComplete;
		m_bomberNavigation = Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBomberNavigation();
		m_noObjective = m_placeable.GetParameter("noObjective") == "true";
		if (!m_noObjective)
		{
			ObjectiveManager.Objective objective = new ObjectiveManager.Objective();
			objective.m_countToComplete = 1;
			objective.m_objectiveTitle = m_objectiveTitle;
			objective.m_primary = true;
			objective.m_objectiveGroup = "TARG_" + base.name;
			if (m_requiresTagOnly)
			{
				objective.m_objectiveType = ObjectiveManager.ObjectiveType.TagOnly;
			}
			else
			{
				objective.m_objectiveType = ObjectiveManager.ObjectiveType.NonBombTarget;
			}
			m_thisObjective = Singleton<ObjectiveManager>.Instance.RegisterObjective(objective);
			m_thisLogObjective = new MissionLog.LogObjective();
			Singleton<GameFlow>.Instance.GetCurrentMissionInfo().GetMissionLog().LogObjectiveNew(m_thisLogObjective);
		}
		else
		{
			m_taggableItem.SetTagType(TaggableItem.TaggableType.BombTargetOptional);
		}
	}

	private void OnTagComplete()
	{
		if (m_requiresTagOnly)
		{
			Complete();
		}
		else
		{
			m_bomberNavigation.SetNavigationPoint(base.gameObject.btransform().position, BomberNavigation.NavigationPointType.FunctionLandingBombing, base.gameObject);
		}
	}

	private void Update()
	{
		if (Singleton<MissionCoordinator>.Instance.IsOutwardJourney() && !m_isSupplied)
		{
			Vector3 vector = base.transform.position - Singleton<BomberSpawn>.Instance.GetBomberSystems().transform.position;
			m_taggableItem.SetTaggable(vector.magnitude < 2200f);
			if (m_bomberNavigation.GetAssociatedObject() != base.gameObject)
			{
				m_taggableItem.SetTagIncomplete();
			}
			if (m_searchAndRescueHint && vector.magnitude < 4000f)
			{
				Singleton<TopBarMissionTipsConstant>.Instance.SetSearchAndRescueRelevant(relevant: true);
			}
		}
		else
		{
			Singleton<TopBarMissionTipsConstant>.Instance.SetSearchAndRescueRelevant(relevant: false);
			m_taggableItem.SetTaggable(taggable: false);
		}
	}

	private void Complete()
	{
		Singleton<TopBarMissionTipsConstant>.Instance.SetSearchAndRescueRelevant(relevant: false);
		m_isSupplied = true;
		if (m_thisObjective != null)
		{
			Singleton<ObjectiveManager>.Instance.CompleteObjective(m_thisObjective);
		}
		if (m_thisLogObjective != null)
		{
			m_thisLogObjective.m_isComplete = true;
		}
	}

	public bool TrySupplyIsSuccessful(NonBombDroppable nba, ExpectedDropType dropType)
	{
		if (!m_requiresTagOnly && !m_isSupplied && dropType == m_expectedDrop && (nba.transform.position - base.transform.position).magnitude < m_radius)
		{
			Complete();
		}
		return false;
	}
}
