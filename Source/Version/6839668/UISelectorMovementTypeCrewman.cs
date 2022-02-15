using System.Collections.Generic;
using BomberCrewCommon;
using UnityEngine;

public class UISelectorMovementTypeCrewman : UISelectorMovementType
{
	private bool m_wasMoved;

	private float m_timeHeld;

	private void Start()
	{
	}

	public override void SetUp(UISelectFinder finder)
	{
		m_wasMoved = false;
		m_timeHeld = 0f;
	}

	public override void ForcePointAt(GameObject go, int cameraLayer)
	{
	}

	public override void ForcePointAt(tk2dUIItem target)
	{
	}

	public override void DeSelect()
	{
	}

	public bool WasMoved()
	{
		return m_wasMoved || m_timeHeld > 0.75f;
	}

	public override void DoMovement(Vector2 absMove, Vector2 tickMove)
	{
		int num = 0;
		bool flag = false;
		bool flag2 = false;
		float num2 = Mathf.Abs(Vector3.Dot(tickMove, Vector3.right));
		float num3 = Mathf.Abs(Vector3.Dot(tickMove, Vector3.up));
		if (num2 < 0.9f && num3 < 0.9f)
		{
			return;
		}
		if (tickMove.magnitude > 0f)
		{
			m_wasMoved = true;
			if (num2 > num3)
			{
				flag = true;
				num = Mathf.RoundToInt(Mathf.Sign(tickMove.x));
			}
			else
			{
				flag = false;
				num = -Mathf.RoundToInt(Mathf.Sign(tickMove.y));
			}
		}
		if (num == 0)
		{
			return;
		}
		List<CrewmanAvatar> list = new List<CrewmanAvatar>();
		for (int i = 0; i < Singleton<CrewContainer>.Instance.GetCurrentCrewCount(); i++)
		{
			CrewmanAvatar crewmanAvatar = Singleton<CrewSpawner>.Instance.Find(i);
			if (crewmanAvatar != null)
			{
				list.Add(crewmanAvatar);
			}
		}
		if (flag)
		{
			list.Sort(delegate(CrewmanAvatar x, CrewmanAvatar y)
			{
				float x2 = Camera.main.WorldToScreenPoint(x.transform.position).x;
				float x3 = Camera.main.WorldToScreenPoint(y.transform.position).x;
				return Mathf.RoundToInt(x2 - x3);
			});
		}
		int num4 = list.IndexOf(Singleton<ContextControl>.Instance.GetCurrentlySelected());
		if (flag2)
		{
			int num5 = num4;
			if (num4 == -1 && num == -1)
			{
				num5 = Singleton<CrewContainer>.Instance.GetCurrentCrewCount();
			}
			CrewmanAvatar crewmanAvatar2;
			do
			{
				num5 += num;
				if (num5 == -1 || num5 == Singleton<CrewContainer>.Instance.GetCurrentCrewCount())
				{
					break;
				}
				crewmanAvatar2 = list[num5];
			}
			while (!(crewmanAvatar2 != null) || !crewmanAvatar2.IsSelectable() || !Singleton<ContextControl>.Instance.ClickOnCrewman(crewmanAvatar2));
			return;
		}
		if (num4 == -1 && num == -1)
		{
			num4 = 0;
		}
		for (int j = 1; j < Singleton<CrewContainer>.Instance.GetCurrentCrewCount(); j++)
		{
			CrewmanAvatar crewmanAvatar3 = list[(num4 + j * num + list.Count) % Singleton<CrewContainer>.Instance.GetCurrentCrewCount()];
			if (crewmanAvatar3 != null && Singleton<ContextControl>.Instance.ClickOnCrewman(crewmanAvatar3))
			{
				break;
			}
		}
	}

	public override void UpdateLogic()
	{
		m_timeHeld += Time.unscaledDeltaTime;
	}

	public override tk2dUIItem GetCurrentlyPointedAtItem()
	{
		return null;
	}

	public override Vector2 GetCurrentScreenSpacePointerPosition()
	{
		return Vector2.zero;
	}

	public override bool UseScreenSpacePointerPosition()
	{
		return false;
	}
}
