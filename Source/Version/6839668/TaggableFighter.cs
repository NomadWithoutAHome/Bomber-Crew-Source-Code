using BomberCrewCommon;
using UnityEngine;

public class TaggableFighter : MonoBehaviour
{
	[SerializeField]
	private TaggableItem m_taggableItem;

	[SerializeField]
	private FighterPlane m_fighter;

	private float m_taggableRemoveTimer;

	private bool m_isTaggable;

	private bool m_shouldBlockTagging;

	public bool IsTaggable()
	{
		return m_isTaggable;
	}

	public void BlockTagging()
	{
		m_shouldBlockTagging = true;
		m_taggableRemoveTimer = 0f;
		m_taggableItem.SetTagIncomplete();
		m_taggableItem.SetTaggable(taggable: false);
	}

	private void Update()
	{
		bool flag = false;
		if (!m_shouldBlockTagging)
		{
			BomberSystems bomberSystems = Singleton<BomberSpawn>.Instance.GetBomberSystems();
			CrewmanAvatar currentCrewman = bomberSystems.GetStationFor(BomberSystems.StationType.RadioOperator).GetCurrentCrewman();
			bool flag2 = currentCrewman != null;
			flag = !m_fighter.IsDestroyed() && Singleton<VisibilityHelpers>.Instance.IsVisibleHumanPlayer(bomberSystems.GetBomberState().transform.position, base.transform.position, bomberSystems, isRadarObject: true, isNavigationObject: false);
		}
		if (flag)
		{
			m_taggableItem.SetTaggable(taggable: true);
			m_taggableRemoveTimer = 1f;
		}
		else
		{
			m_taggableRemoveTimer -= Time.deltaTime;
			if (m_taggableRemoveTimer < 0f)
			{
				m_taggableItem.SetTaggable(taggable: false);
				m_taggableItem.SetTagIncomplete();
			}
		}
		m_isTaggable = flag;
	}
}
