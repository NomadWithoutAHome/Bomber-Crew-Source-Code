using BomberCrewCommon;
using Rewired;
using UnityEngine;

public class SelectableFilterTabGroup : MonoBehaviour
{
	[SerializeField]
	private string m_moveLeftButton;

	[SerializeField]
	private string m_moveRightButton;

	[SerializeField]
	private string m_axis;

	private void Update()
	{
		if (!Singleton<UISelector>.Instance.IsPrimary() || Singleton<UISelector>.Instance.IsPaused())
		{
			return;
		}
		int num = 0;
		if (ReInput.players.GetPlayer(0).GetButtonUp(m_moveRightButton))
		{
			num = 1;
		}
		else if (ReInput.players.GetPlayer(0).GetButtonUp(m_moveLeftButton))
		{
			num = -1;
		}
		if (num == 0)
		{
			return;
		}
		int num2 = -1;
		SelectableFilterButton[] componentsInChildren = GetComponentsInChildren<SelectableFilterButton>();
		int num3 = 0;
		SelectableFilterButton[] array = componentsInChildren;
		foreach (SelectableFilterButton selectableFilterButton in array)
		{
			if (selectableFilterButton.IsSelected())
			{
				num2 = num3;
			}
			num3++;
		}
		int num4 = num2 + num;
		if (num2 == -1)
		{
			num4 = ((num != 1) ? (componentsInChildren.Length - 1) : 0);
		}
		if (num4 < 0)
		{
			num4 = 0;
		}
		else if (num4 > componentsInChildren.Length - 1)
		{
			num4 = componentsInChildren.Length - 1;
		}
		if (num4 == num2)
		{
			return;
		}
		num3 = 0;
		SelectableFilterButton[] array2 = componentsInChildren;
		foreach (SelectableFilterButton selectableFilterButton2 in array2)
		{
			if (num3 == num4)
			{
				selectableFilterButton2.FakeClick();
			}
			num3++;
		}
	}
}
