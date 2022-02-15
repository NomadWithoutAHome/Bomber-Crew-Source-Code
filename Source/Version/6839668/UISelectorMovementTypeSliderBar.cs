using Rewired;
using UnityEngine;

public class UISelectorMovementTypeSliderBar : UISelectorMovementType
{
	[SerializeField]
	private SliderBar m_sliderBar;

	[SerializeField]
	private tk2dUIItem m_uiItem;

	[SerializeField]
	private float m_analogueMoveAmount = 1f;

	[SerializeField]
	private float m_digitalMoveAmount = 0.1f;

	private Vector3 m_previousDigital;

	public override tk2dUIItem GetCurrentlyPointedAtItem()
	{
		return null;
	}

	public override Vector2 GetCurrentScreenSpacePointerPosition()
	{
		Vector3 screenPosThumb = m_sliderBar.GetScreenPosThumb();
		return screenPosThumb;
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
		Vector3 previousDigital = ReInput.players.GetPlayer(0).GetAxis2D(43, 41);
		Vector3 vector = ReInput.players.GetPlayer(0).GetAxis2D(42, 40);
		float num = m_sliderBar.GetValue();
		if (Mathf.Abs(previousDigital.x) > 0.2f && (m_previousDigital.magnitude == 0f || Vector3.Dot(previousDigital.normalized, m_previousDigital.normalized) < 0.1f))
		{
			num += m_digitalMoveAmount * Mathf.Sign(previousDigital.x);
		}
		num += vector.x * m_analogueMoveAmount * Time.unscaledDeltaTime;
		if (num != m_sliderBar.GetValue())
		{
			m_sliderBar.SetValue(Mathf.Clamp01(num));
		}
		m_previousDigital = previousDigital;
	}

	public override void UpdateLogic()
	{
	}

	public override void SetUp(UISelectFinder finder)
	{
	}

	public override void DeSelect()
	{
	}
}
