using Rewired;
using UnityEngine;

public class UISelectorMovementTypeCanvas : UISelectorMovementType
{
	[SerializeField]
	private tk2dUIItem m_canvasItem;

	[SerializeField]
	private tk2dCamera m_uiCamera;

	[SerializeField]
	private float m_worldSizeSquareRadius;

	[SerializeField]
	private float m_worldSizeSquareRadiusHeight;

	[SerializeField]
	private float m_moveSpeed;

	[SerializeField]
	private float m_nudgeSpeed;

	private Vector3 m_screenSpacePointerPosition;

	private Vector3 m_previousDigital;

	private bool m_hasSetCursorPosition;

	private float m_renudgeTime;

	private float m_renudgeFirst;

	public override tk2dUIItem GetCurrentlyPointedAtItem()
	{
		return m_canvasItem;
	}

	public override Vector2 GetCurrentScreenSpacePointerPosition()
	{
		return m_screenSpacePointerPosition;
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
		Vector3 vector = ReInput.players.GetPlayer(0).GetAxis2D(43, 41);
		Vector3 vector2 = ReInput.players.GetPlayer(0).GetAxis2D(42, 40);
		m_screenSpacePointerPosition += vector2 * m_moveSpeed * Time.deltaTime;
		if (vector.magnitude > 0.2f)
		{
			if (m_previousDigital.magnitude == 0f || Vector3.Dot(vector.normalized, m_previousDigital.normalized) < 0.1f || m_renudgeTime < 0f)
			{
				m_renudgeFirst *= 0.8f;
				if (m_renudgeFirst < 0.075f)
				{
					m_renudgeFirst = 0.075f;
				}
				m_renudgeTime = m_renudgeFirst;
				float num = (float)Screen.height / 1080f;
				Vector3 vector3 = m_uiCamera.ScreenCamera.WorldToScreenPoint(m_canvasItem.transform.position) + new Vector3(num * m_nudgeSpeed * 0.5f, num * m_nudgeSpeed * 0.5f);
				m_screenSpacePointerPosition -= vector3;
				m_screenSpacePointerPosition /= num * m_nudgeSpeed;
				m_screenSpacePointerPosition.x = Mathf.Round(m_screenSpacePointerPosition.x);
				m_screenSpacePointerPosition.y = Mathf.Round(m_screenSpacePointerPosition.y);
				m_screenSpacePointerPosition *= num * m_nudgeSpeed;
				m_screenSpacePointerPosition += vector3;
				m_screenSpacePointerPosition += vector * m_nudgeSpeed * num;
			}
		}
		else
		{
			m_renudgeFirst = 0.5f;
		}
		m_renudgeTime -= Time.deltaTime;
		Vector3 vector4 = m_uiCamera.ScreenCamera.WorldToScreenPoint(m_canvasItem.transform.position);
		float num2 = (float)Screen.height / 1080f;
		if (m_worldSizeSquareRadiusHeight == 0f)
		{
			m_worldSizeSquareRadiusHeight = m_worldSizeSquareRadius;
		}
		m_screenSpacePointerPosition.x = Mathf.Clamp(m_screenSpacePointerPosition.x, vector4.x - m_worldSizeSquareRadius * num2, vector4.x + m_worldSizeSquareRadius * num2);
		m_screenSpacePointerPosition.y = Mathf.Clamp(m_screenSpacePointerPosition.y, vector4.y - m_worldSizeSquareRadiusHeight * num2, vector4.y + m_worldSizeSquareRadiusHeight * num2);
		m_previousDigital = vector;
	}

	public override void UpdateLogic()
	{
	}

	public override void SetUp(UISelectFinder finder)
	{
		if (!m_hasSetCursorPosition)
		{
			m_screenSpacePointerPosition = m_uiCamera.ScreenCamera.WorldToScreenPoint(m_canvasItem.transform.position);
			float num = (float)Screen.height / 1080f;
			m_screenSpacePointerPosition += new Vector3(m_nudgeSpeed * num * 0.5f, m_nudgeSpeed * num * 0.5f, 0f);
			m_hasSetCursorPosition = true;
		}
	}

	public override void DeSelect()
	{
	}
}
