using System.Collections.Generic;
using UnityEngine;

public class LayoutGrid : LayoutController
{
	private enum HorizontalAlignment
	{
		Left,
		Right,
		Centre
	}

	[SerializeField]
	private HorizontalAlignment m_horizontalAlignment;

	[SerializeField]
	private int m_numberOfColumns;

	[SerializeField]
	private int m_paddingBetweenColumns;

	[SerializeField]
	private float m_paddingBetweenRows;

	[SerializeField]
	private bool m_useFixedSpacing;

	[SerializeField]
	private float m_fixedSpacingX;

	[SerializeField]
	private float m_fixedSpacingY;

	[SerializeField]
	private bool m_useFixedSpacingX;

	[SerializeField]
	private bool m_useFixedSpacingY;

	[SerializeField]
	private bool m_onlyEnabled;

	[SerializeField]
	private bool m_roundPosition;

	[SerializeField]
	private bool m_repositionOnEnable;

	public int NumberOfColumns
	{
		get
		{
			return m_numberOfColumns;
		}
		set
		{
			m_numberOfColumns = value;
		}
	}

	public float PaddingBetweenRows
	{
		get
		{
			return m_paddingBetweenRows;
		}
		set
		{
			m_paddingBetweenRows = value;
		}
	}

	private void OnEnable()
	{
		if (m_repositionOnEnable)
		{
			RepositionChildren();
		}
	}

	public override void RepositionChildren()
	{
		float num = 0f;
		float num2 = 0f;
		float num3 = num;
		float num4 = num2;
		int num5 = 0;
		float num6 = 0f;
		int num7 = 0;
		foreach (Transform item in base.transform)
		{
			if (item.gameObject.activeInHierarchy || !m_onlyEnabled)
			{
				Bounds bounds = default(Bounds);
				bounds = DisplayUtils.GetRendererBoundsOfGameObjectAndAllChildren(item.gameObject);
				num3 = ((m_useFixedSpacing || m_useFixedSpacingX) ? (num3 + m_fixedSpacingX / 2f) : (num3 + bounds.extents.x));
				item.localPosition = new Vector3(num3, num4, item.localPosition.z);
				if (num5 < m_numberOfColumns - 1)
				{
					num3 = ((m_useFixedSpacing || m_useFixedSpacingX) ? (num3 + m_fixedSpacingX / 2f) : (num3 + (bounds.extents.x + (float)m_paddingBetweenColumns)));
					num5++;
				}
				else
				{
					num3 = num;
					num4 = ((m_useFixedSpacing || m_useFixedSpacingY) ? (num4 - m_fixedSpacingY) : (num4 - (bounds.size.y + m_paddingBetweenRows)));
					num5 = 0;
				}
				num7++;
			}
		}
		num6 = ((m_useFixedSpacing || m_useFixedSpacingX) ? ((float)Mathf.Min(num7, NumberOfColumns) * m_fixedSpacingX) : DisplayUtils.GetRendererBoundsOfGameObjectAndAllChildren(base.gameObject).size.x);
		switch (m_horizontalAlignment)
		{
		case HorizontalAlignment.Centre:
		{
			foreach (Transform item2 in base.transform)
			{
				float num9 = num6 / 2f;
				Vector3 vector2 = new Vector3(item2.localPosition.x - num9, item2.localPosition.y, item2.localPosition.z);
				if (m_roundPosition)
				{
					vector2 = RoundPosition(vector2);
				}
				item2.localPosition = vector2;
			}
			break;
		}
		case HorizontalAlignment.Right:
		{
			foreach (Transform item3 in base.transform)
			{
				float num8 = num6;
				Vector3 vector = new Vector3(item3.localPosition.x - num8, item3.localPosition.y, item3.localPosition.z);
				if (m_roundPosition)
				{
					vector = RoundPosition(vector);
				}
				item3.localPosition = vector;
			}
			break;
		}
		case HorizontalAlignment.Left:
			break;
		}
	}

	public void ClearChildren()
	{
		foreach (Transform item in base.transform)
		{
			Object.Destroy(item.gameObject);
		}
	}

	public void ClearChildrenImmediate()
	{
		List<Transform> list = new List<Transform>();
		foreach (Transform item in base.transform)
		{
			list.Add(item);
		}
		foreach (Transform item2 in list)
		{
			Object.DestroyImmediate(item2.gameObject);
		}
	}

	public float GetFixedSpacingY()
	{
		return m_fixedSpacingY;
	}

	private Vector3 RoundPosition(Vector3 pos)
	{
		return new Vector3(Mathf.Round(pos.x), Mathf.Round(pos.y), Mathf.Round(pos.z));
	}
}
