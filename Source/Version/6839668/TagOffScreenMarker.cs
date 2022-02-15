using BomberCrewCommon;
using UnityEngine;

public class TagOffScreenMarker : MonoBehaviour
{
	[SerializeField]
	private TagUIProgress m_uiProgressToWatch;

	[SerializeField]
	private GameObject m_showHierarchy;

	[SerializeField]
	private WorldToScreenTracker m_tracker;

	[SerializeField]
	private int m_edgeOffset;

	[SerializeField]
	private bool m_showInTargetingMode;

	[SerializeField]
	private GameObject m_radarViewHierarchy;

	[SerializeField]
	private GameObject m_mapViewHierarchy;

	[SerializeField]
	private AnimateDetailAlpha m_animateDetailAlphaRadar;

	[SerializeField]
	private AnimateDetailAlpha m_animateDetailNavigator;

	[SerializeField]
	private tk2dSpriteAnimator m_blipAnimator;

	[SerializeField]
	private bool m_showEvenNotTargeting;

	private BomberSystems m_bomberSytems;

	private StationRadioOperator m_radioOpStation;

	private StationNavigator m_navigationStation;

	private bool m_isShowing;

	private void Start()
	{
		m_bomberSytems = Singleton<BomberSpawn>.Instance.GetBomberSystems();
		m_radioOpStation = (StationRadioOperator)m_bomberSytems.GetStationFor(BomberSystems.StationType.RadioOperator);
		m_navigationStation = (StationNavigator)m_bomberSytems.GetStationFor(BomberSystems.StationType.Navigation);
	}

	private void LateUpdate()
	{
		bool flag = true;
		bool flag2 = true;
		if (m_uiProgressToWatch == null || !m_uiProgressToWatch.IsVisible())
		{
			if (m_uiProgressToWatch != null)
			{
				switch (m_uiProgressToWatch.GetTaggableItem().GetVisibilityType())
				{
				case TaggableItem.VisibilityType.RadarAssisted:
					if (m_radarViewHierarchy != null)
					{
						m_radarViewHierarchy.SetActive(value: true);
					}
					if (m_mapViewHierarchy != null)
					{
						m_mapViewHierarchy.SetActive(value: false);
					}
					if (m_animateDetailAlphaRadar != null)
					{
						m_animateDetailAlphaRadar.SetDetailAlpha(Singleton<FishpondRadar>.Instance.GetBlipPingAmountFor(m_uiProgressToWatch.GetTaggableItem()));
					}
					if (m_blipAnimator != null && Singleton<FishpondRadar>.Instance.ShouldPlayPingAnimation(m_uiProgressToWatch.GetTaggableItem()))
					{
						m_blipAnimator.Play();
						Singleton<FishpondRadar>.Instance.MarkAnimationPlayed(m_uiProgressToWatch.GetTaggableItem());
					}
					break;
				case TaggableItem.VisibilityType.NavigatorAssisted:
				{
					if (m_mapViewHierarchy != null)
					{
						m_mapViewHierarchy.SetActive(value: true);
					}
					if (m_radarViewHierarchy != null)
					{
						m_radarViewHierarchy.SetActive(value: false);
					}
					float navigationHintVisibility = m_navigationStation.GetNavigationHintVisibility();
					if (navigationHintVisibility == 0f)
					{
						flag2 = false;
					}
					m_animateDetailNavigator.SetDetailAlpha(navigationHintVisibility);
					break;
				}
				default:
					if (m_radarViewHierarchy != null)
					{
						m_radarViewHierarchy.SetActive(value: false);
					}
					if (m_mapViewHierarchy != null)
					{
						m_mapViewHierarchy.SetActive(value: false);
					}
					if (m_animateDetailAlphaRadar != null)
					{
						m_animateDetailAlphaRadar.SetDetailAlpha(0f);
					}
					break;
				}
			}
			tk2dCamera camera = m_tracker.GetCamera();
			float num = camera.ScreenExtents.width / 2f;
			float num2 = camera.ScreenExtents.height / 2f;
			bool isBehind = false;
			Vector3 vector = m_tracker.GetPosition(out isBehind);
			if (vector.magnitude == 0f)
			{
				vector.x = 1f;
			}
			if (isBehind)
			{
				vector.y = 0f - vector.y;
				vector.x = 0f - vector.x;
			}
			if (isBehind || vector.x > num || vector.x < 0f - num || vector.y > num2 || vector.y < 0f - num2)
			{
				float num3 = num / num2;
				vector.y *= num3;
				vector = vector.normalized;
				float num4 = Mathf.Max(Mathf.Abs(vector.x), Mathf.Abs(vector.y));
				vector /= num4;
				vector.x *= num;
				vector.y *= num2;
			}
			else
			{
				flag = m_showEvenNotTargeting || (m_showInTargetingMode && Singleton<ContextControl>.Instance.IsTargetingMode());
			}
			vector.x = Mathf.Clamp(vector.x, 0f - num + (float)m_edgeOffset, num - (float)m_edgeOffset);
			vector.y = Mathf.Clamp(vector.y, 0f - num2 + (float)m_edgeOffset, num2 - (float)m_edgeOffset);
			base.transform.position = new Vector3(vector.x, vector.y, 0f);
		}
		else
		{
			flag = false;
		}
		m_isShowing = flag && flag2;
		m_showHierarchy.SetActive(flag);
	}

	public bool IsDisplayedCurrently()
	{
		return m_isShowing;
	}
}
