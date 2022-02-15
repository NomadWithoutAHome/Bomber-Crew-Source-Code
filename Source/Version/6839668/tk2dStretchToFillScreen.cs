using UnityEngine;

[ExecuteInEditMode]
public class tk2dStretchToFillScreen : MonoBehaviour
{
	private enum Side
	{
		Width,
		Height,
		Both,
		None
	}

	[SerializeField]
	private tk2dSlicedSprite m_slicedSprite;

	private tk2dCamera m_camera;

	[SerializeField]
	private Vector2 m_extraBorder = new Vector2(4f, 4f);

	[SerializeField]
	private Side m_side = Side.None;

	private void Update()
	{
		if (m_camera == null)
		{
			m_camera = tk2dCamera.CameraForLayer(base.gameObject.layer);
		}
		if (m_slicedSprite == null)
		{
			m_slicedSprite = base.gameObject.GetComponent<tk2dSlicedSprite>();
		}
		switch (m_side)
		{
		case Side.Width:
			m_slicedSprite.dimensions = new Vector2(m_camera.ScreenExtents.size.x + m_extraBorder.x, m_slicedSprite.dimensions.y);
			break;
		case Side.Height:
			m_slicedSprite.dimensions = new Vector2(m_slicedSprite.dimensions.x, m_camera.ScreenExtents.size.y + m_extraBorder.y);
			break;
		case Side.Both:
			m_slicedSprite.dimensions = m_camera.ScreenExtents.size + m_extraBorder;
			break;
		}
	}
}
