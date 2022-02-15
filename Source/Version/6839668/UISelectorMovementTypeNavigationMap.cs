using BomberCrewCommon;
using Rewired;
using UnityEngine;

public class UISelectorMovementTypeNavigationMap : UISelectorMovementType
{
	[SerializeField]
	private Transform m_ctrItem;

	[SerializeField]
	private tk2dUIItem m_mainUIItem;

	[SerializeField]
	private float m_hoverRadius = 10f;

	private Vector3 m_offsetPosition;

	private bool m_isHovered;

	public override tk2dUIItem GetCurrentlyPointedAtItem()
	{
		return m_mainUIItem;
	}

	public override Vector2 GetCurrentScreenSpacePointerPosition()
	{
		Vector3 position = m_ctrItem.transform.position + m_offsetPosition;
		Vector3 vector = Singleton<ContextControl>.Instance.GetUICamera().ScreenCamera.WorldToScreenPoint(position);
		return vector;
	}

	public override bool UseScreenSpacePointerPosition()
	{
		return true;
	}

	public override void ForcePointAt(tk2dUIItem target)
	{
	}

	public override void ForcePointAt(GameObject go, int cameraLayer)
	{
	}

	public override void DoMovement(Vector2 absMove, Vector2 tickMove)
	{
		Vector2 axis2D = ReInput.players.GetPlayer(0).GetAxis2D(1, 0);
		if (axis2D.magnitude > 0.2f)
		{
			Vector3 vector = axis2D.normalized;
			m_offsetPosition = new Vector3(vector.x, vector.y, 0f) * m_hoverRadius;
			m_isHovered = true;
		}
	}

	public bool IsDirectionPressed()
	{
		return m_isHovered;
	}

	public override void UpdateLogic()
	{
	}

	public override void SetUp(UISelectFinder finder)
	{
		m_isHovered = false;
	}

	public override void DeSelect()
	{
		m_isHovered = false;
	}
}
