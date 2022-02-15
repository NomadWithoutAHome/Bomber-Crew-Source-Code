using BomberCrewCommon;
using UnityEngine;

public class UISelectorMovementTypeSnap : UISelectorMovementType
{
	[SerializeField]
	private bool m_useScreenSpacePosition;

	[SerializeField]
	private bool m_hasMemory;

	private tk2dUIItem m_currentlyPointedAtItem;

	private UISelectorPointingHint m_currentlyPointedAtHint;

	private Vector3 m_currentScreenSpacePosition;

	private UISelectFinder m_currentFinder;

	private tk2dUIItem m_lastOnDeselect;

	private void Start()
	{
		m_currentScreenSpacePosition = new Vector3(0f, 5000f, 0f);
	}

	public void FindBest()
	{
		if (!(m_currentFinder != null))
		{
			return;
		}
		tk2dUIItem[] allItems = m_currentFinder.GetAllItems();
		tk2dUIItem tk2dUIItem2 = null;
		tk2dUIItem[] array = allItems;
		foreach (tk2dUIItem tk2dUIItem3 in array)
		{
			if (!IsValid(tk2dUIItem3))
			{
				continue;
			}
			if (tk2dUIItem2 == null)
			{
				tk2dUIItem2 = tk2dUIItem3;
			}
			else if (tk2dUIItem3.GetComponent<UISelectorPointingHint>() != null)
			{
				if (tk2dUIItem3.GetComponent<UISelectorPointingHint>().IsDefault())
				{
					tk2dUIItem2 = tk2dUIItem3;
				}
			}
			else if (tk2dUIItem3.GetComponent<SelectableFilterButton>() != null && tk2dUIItem3.GetComponent<SelectableFilterButton>().IsSelected())
			{
				tk2dUIItem2 = tk2dUIItem3;
			}
			if (tk2dUIItem3 == m_currentlyPointedAtItem)
			{
				tk2dUIItem2 = tk2dUIItem3;
				break;
			}
		}
		if (m_currentlyPointedAtItem != null)
		{
			m_currentlyPointedAtItem.HoverOut(null);
		}
		if (tk2dUIItem2 != null)
		{
			tk2dUIItem2.HoverOver(m_currentlyPointedAtItem);
			m_currentlyPointedAtHint = tk2dUIItem2.GetComponent<UISelectorPointingHint>();
		}
		m_currentlyPointedAtItem = tk2dUIItem2;
	}

	public override void SetUp(UISelectFinder finder)
	{
		m_currentFinder = finder;
		if (m_hasMemory && m_lastOnDeselect != null)
		{
			ForcePointAt(m_lastOnDeselect);
		}
	}

	public override void ForcePointAt(GameObject go, int cameraLayer)
	{
		tk2dCamera tk2dCamera2 = tk2dCamera.CameraForLayer(cameraLayer);
		if (tk2dCamera2 != null)
		{
			Vector3 currentScreenSpacePosition = tk2dCamera2.ScreenCamera.WorldToScreenPoint(go.transform.position);
			currentScreenSpacePosition.z = 0f;
			m_currentScreenSpacePosition = currentScreenSpacePosition;
		}
	}

	public override void DeSelect()
	{
		m_lastOnDeselect = m_currentlyPointedAtItem;
		if (m_currentlyPointedAtItem != null)
		{
			m_currentlyPointedAtItem.HoverOut(null);
		}
		m_currentlyPointedAtItem = null;
		m_currentlyPointedAtHint = null;
	}

	public override void ForcePointAt(tk2dUIItem target)
	{
		if (m_currentlyPointedAtItem != null)
		{
			m_currentlyPointedAtItem.HoverOut(null);
		}
		if (Singleton<UISelector>.Instance.GetCurrentMovementType() == this)
		{
			target.HoverOver(m_currentlyPointedAtItem);
		}
		m_currentlyPointedAtItem = target;
		m_currentlyPointedAtHint = target.GetComponent<UISelectorPointingHint>();
	}

	public override void DoMovement(Vector2 absMove, Vector2 tickMove)
	{
		if (!(tickMove.magnitude > 0f) || !(m_currentFinder != null))
		{
			return;
		}
		Vector3 vector = m_currentScreenSpacePosition;
		if (m_currentlyPointedAtItem != null)
		{
			tk2dCamera tk2dCamera2 = tk2dCamera.CameraForLayer(m_currentlyPointedAtItem.gameObject.layer);
			vector = ((!(tk2dCamera2 != null)) ? Camera.main.WorldToScreenPoint(m_currentlyPointedAtItem.transform.position) : tk2dCamera2.ScreenCamera.WorldToScreenPoint(m_currentlyPointedAtItem.transform.position));
		}
		vector.z = 0f;
		tk2dUIItem[] allItems = m_currentFinder.GetAllItems();
		float num = ((!(m_currentlyPointedAtItem == null)) ? 0.75f : (-1.1f));
		float num2 = float.MaxValue;
		tk2dUIItem tk2dUIItem2 = null;
		if (m_currentlyPointedAtHint != null)
		{
			float num3 = Vector3.Dot(tickMove, Vector3.up);
			float num4 = Vector3.Dot(tickMove, Vector3.right);
			if (num3 > 0.85f)
			{
				tk2dUIItem2 = m_currentlyPointedAtHint.GetUpLink();
			}
			else if (num3 < -0.85f)
			{
				tk2dUIItem2 = m_currentlyPointedAtHint.GetDownLink();
			}
			else if (num4 > 0.85f)
			{
				tk2dUIItem2 = m_currentlyPointedAtHint.GetRightLink();
			}
			else if (num4 < -0.85f)
			{
				tk2dUIItem2 = m_currentlyPointedAtHint.GetLeftLink();
			}
		}
		if (tk2dUIItem2 == null)
		{
			tk2dUIItem[] array = allItems;
			foreach (tk2dUIItem tk2dUIItem3 in array)
			{
				if (!(tk2dUIItem3 != m_currentlyPointedAtItem) || !IsValid(tk2dUIItem3))
				{
					continue;
				}
				tk2dCamera tk2dCamera3 = tk2dCamera.CameraForLayer(tk2dUIItem3.gameObject.layer);
				if (tk2dCamera3 != null)
				{
					Vector3 vector2 = tk2dCamera3.ScreenCamera.WorldToScreenPoint(tk2dUIItem3.transform.position);
					vector2.z = 0f;
					Vector3 vector3 = vector2 - vector;
					if (Vector3.Dot(vector3.normalized, tickMove) > num && vector3.magnitude < num2)
					{
						num2 = vector3.magnitude;
						tk2dUIItem2 = tk2dUIItem3;
					}
				}
				else
				{
					Vector3 vector4 = Camera.main.WorldToScreenPoint(tk2dUIItem3.transform.position);
					Vector3 vector5 = vector4 - vector;
					if (Vector3.Dot(vector5.normalized, tickMove) > num && vector5.magnitude < num2)
					{
						num2 = vector5.magnitude;
						tk2dUIItem2 = tk2dUIItem3;
					}
				}
			}
		}
		if (tk2dUIItem2 != null)
		{
			if (m_currentlyPointedAtItem != null)
			{
				m_currentlyPointedAtItem.HoverOut(null);
			}
			tk2dUIItem2.HoverOver(m_currentlyPointedAtItem);
			m_currentlyPointedAtItem = tk2dUIItem2;
			m_currentlyPointedAtHint = tk2dUIItem2.GetComponent<UISelectorPointingHint>();
		}
	}

	public override void UpdateLogic()
	{
		if (m_currentlyPointedAtItem == null || !IsValid(m_currentlyPointedAtItem) || !m_currentFinder.DoesItemMatch(m_currentlyPointedAtItem))
		{
			FindBest();
		}
		if (m_currentlyPointedAtItem != null)
		{
			Transform transform = ((!(m_currentlyPointedAtHint == null)) ? m_currentlyPointedAtHint.GetPointingHint() : m_currentlyPointedAtItem.transform);
			tk2dCamera tk2dCamera2 = tk2dCamera.CameraForLayer(m_currentlyPointedAtItem.gameObject.layer);
			if (tk2dCamera2 != null)
			{
				m_currentScreenSpacePosition = tk2dCamera2.ScreenCamera.WorldToScreenPoint(transform.position);
			}
			if (!IsValid(m_currentlyPointedAtItem))
			{
				m_currentlyPointedAtItem.HoverOut(null);
				m_currentlyPointedAtItem = null;
				m_currentlyPointedAtHint = null;
			}
		}
	}

	public override tk2dUIItem GetCurrentlyPointedAtItem()
	{
		return m_currentlyPointedAtItem;
	}

	public override Vector2 GetCurrentScreenSpacePointerPosition()
	{
		return m_currentScreenSpacePosition;
	}

	public override bool UseScreenSpacePointerPosition()
	{
		return m_useScreenSpacePosition;
	}
}
