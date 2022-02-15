using BomberCrewCommon;
using UnityEngine;

public class SelectableScrollListButton : MonoBehaviour
{
	[SerializeField]
	private SelectableFilterButton m_filterButton;

	[SerializeField]
	private float m_heightUp;

	[SerializeField]
	private float m_heightDown;

	private tk2dUIScrollableArea m_scrollableArea;

	public void SetScrollList(tk2dUIScrollableArea scrollableArea)
	{
		m_scrollableArea = scrollableArea;
	}

	private void FindScrollList(Transform t)
	{
		tk2dUIScrollableArea componentInChildren = t.GetComponentInChildren<tk2dUIScrollableArea>();
		if (componentInChildren != null)
		{
			SetScrollList(componentInChildren);
		}
		else if (t.parent != null)
		{
			FindScrollList(t.parent);
		}
	}

	private void Start()
	{
		m_filterButton.OnClick += CheckScrollList;
		FindScrollList(base.transform);
		if (m_filterButton.IsSelected())
		{
			CheckScrollList();
		}
	}

	private void OnDestroy()
	{
		m_filterButton.OnClick -= CheckScrollList;
	}

	private void CheckScrollList()
	{
		if (Singleton<UISelector>.Instance.IsPrimary())
		{
			float num = m_scrollableArea.MeasureContentLength();
			Vector3 vector = -(base.transform.position - m_scrollableArea.transform.position);
			if (vector.y < m_heightUp)
			{
				float num2 = vector.y - m_heightUp;
				m_scrollableArea.Value += num2 / (num - m_scrollableArea.VisibleAreaLength);
			}
			else if (vector.y > m_scrollableArea.VisibleAreaLength - m_heightDown)
			{
				float num3 = vector.y - (m_scrollableArea.VisibleAreaLength - m_heightDown);
				m_scrollableArea.Value += num3 / (num - m_scrollableArea.VisibleAreaLength);
			}
		}
	}
}
