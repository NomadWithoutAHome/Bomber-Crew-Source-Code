using Rewired;
using UnityEngine;

public class ControlScrollAreaWithAxis : MonoBehaviour
{
	[SerializeField]
	private tk2dUIScrollableArea m_scrollArea;

	[SerializeField]
	private string m_axis;

	[SerializeField]
	private float m_speedPix;

	private void Update()
	{
		if (m_scrollArea.ContentLength > 0f)
		{
			float axis = ReInput.players.GetPlayer(0).GetAxis(m_axis);
			float num = m_speedPix * Time.unscaledDeltaTime / m_scrollArea.ContentLength;
			m_scrollArea.Value = Mathf.Clamp01(num * axis + m_scrollArea.Value);
		}
	}
}
