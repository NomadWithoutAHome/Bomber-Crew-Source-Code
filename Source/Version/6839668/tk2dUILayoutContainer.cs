using System;
using UnityEngine;

public abstract class tk2dUILayoutContainer : tk2dUILayout
{
	protected Vector2 innerSize = Vector2.zero;

	public event Action OnChangeContent;

	public Vector2 GetInnerSize()
	{
		return innerSize;
	}

	protected abstract void DoChildLayout();

	public override void Reshape(Vector3 dMin, Vector3 dMax, bool updateChildren)
	{
		bMin += dMin;
		bMax += dMax;
		Vector3 vector = new Vector3(bMin.x, bMax.y);
		base.transform.position += vector;
		bMin -= vector;
		bMax -= vector;
		DoChildLayout();
		if (this.OnChangeContent != null)
		{
			this.OnChangeContent();
		}
	}

	public void AddLayout(tk2dUILayout layout, tk2dUILayoutItem item)
	{
		item.gameObj = layout.gameObject;
		item.layout = layout;
		layoutItems.Add(item);
		layout.gameObject.transform.parent = base.transform;
		Refresh();
	}

	public void AddLayoutAtIndex(tk2dUILayout layout, tk2dUILayoutItem item, int index)
	{
		item.gameObj = layout.gameObject;
		item.layout = layout;
		layoutItems.Insert(index, item);
		layout.gameObject.transform.parent = base.transform;
		Refresh();
	}

	public void RemoveLayout(tk2dUILayout layout)
	{
		foreach (tk2dUILayoutItem layoutItem in layoutItems)
		{
			if (layoutItem.layout == layout)
			{
				layoutItems.Remove(layoutItem);
				layout.gameObject.transform.parent = null;
				break;
			}
		}
		Refresh();
	}
}
