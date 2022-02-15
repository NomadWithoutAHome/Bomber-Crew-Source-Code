using UnityEngine;

[ExecuteInEditMode]
public class TransformStretchToFillScreen : MonoBehaviour
{
	private enum Side
	{
		Width,
		Height,
		Both,
		None
	}

	[SerializeField]
	private Transform m_transformToStretch;

	private tk2dCamera m_camera;

	[SerializeField]
	private Vector2 m_extraBorder = new Vector2(4f, 4f);

	[SerializeField]
	private Side m_side = Side.None;

	[SerializeField]
	private bool m_matchScaleBothAxis;

	private void Update()
	{
		if (m_camera == null)
		{
			m_camera = tk2dCamera.CameraForLayer(base.gameObject.layer);
		}
		switch (m_side)
		{
		case Side.Width:
			if (m_matchScaleBothAxis)
			{
				m_transformToStretch.localScale = new Vector3(m_camera.ScreenExtents.size.x + m_extraBorder.x, m_camera.ScreenExtents.size.x + m_extraBorder.x, 1f);
			}
			else
			{
				m_transformToStretch.localScale = new Vector3(m_camera.ScreenExtents.size.x + m_extraBorder.x, m_transformToStretch.localScale.y, 1f);
			}
			break;
		case Side.Height:
			if (m_matchScaleBothAxis)
			{
				m_transformToStretch.localScale = new Vector3(m_camera.ScreenExtents.size.y + m_extraBorder.y, m_camera.ScreenExtents.size.y + m_extraBorder.y, 1f);
			}
			else
			{
				m_transformToStretch.localScale = new Vector3(m_transformToStretch.localScale.x, m_camera.ScreenExtents.size.y + m_extraBorder.y, 1f);
			}
			break;
		case Side.Both:
			m_transformToStretch.localScale = new Vector3(m_camera.ScreenExtents.size.x + m_extraBorder.x, m_camera.ScreenExtents.size.y + m_extraBorder.y, 1f);
			break;
		}
	}
}
