using UnityEngine;

[ExecuteInEditMode]
public class tk2dSpriteStretch : MonoBehaviour
{
	private enum Axis
	{
		Width,
		Height,
		Both,
		WidthAspect,
		HeightAspect
	}

	[SerializeField]
	private Axis m_axis;

	[SerializeField]
	private BoxCollider m_collider;

	[SerializeField]
	private float m_aspect;

	private tk2dSlicedSprite m_sprite;

	private Camera m_camera;

	private float m_cameraWidth;

	private float m_cameraHeight;

	private tk2dBaseSprite.Anchor m_lastAnchor;

	private void Awake()
	{
		StretchSprite();
	}

	private void OnEnable()
	{
		StretchSprite();
	}

	private void Update()
	{
		if (m_camera == null)
		{
			StretchSprite();
			return;
		}
		float num = m_camera.pixelWidth;
		float num2 = m_camera.pixelHeight;
		if (num != m_cameraWidth || num2 != m_cameraHeight)
		{
			StretchSprite();
		}
		if (m_sprite.anchor != m_lastAnchor)
		{
			StretchSprite();
		}
	}

	public void StretchSprite()
	{
		m_camera = base.gameObject.GetComponentInParent<Camera>();
		if (!(m_camera != null))
		{
			return;
		}
		m_cameraWidth = m_camera.pixelWidth;
		m_cameraHeight = m_camera.pixelHeight;
		if (base.gameObject.GetComponent<tk2dSlicedSprite>() != null)
		{
			m_sprite = base.gameObject.GetComponent<tk2dSlicedSprite>();
			m_lastAnchor = m_sprite.anchor;
			switch (m_axis)
			{
			case Axis.Width:
				m_sprite.dimensions = new Vector2(m_cameraWidth, m_sprite.dimensions.y);
				MatchCollider();
				break;
			case Axis.Height:
				m_sprite.dimensions = new Vector2(m_sprite.dimensions.x, m_cameraHeight);
				MatchCollider();
				break;
			case Axis.Both:
				m_sprite.dimensions = new Vector2(m_cameraWidth, m_cameraHeight);
				MatchCollider();
				break;
			case Axis.WidthAspect:
				m_sprite.dimensions = new Vector2(m_cameraWidth, m_cameraWidth / m_aspect);
				MatchCollider();
				break;
			case Axis.HeightAspect:
				m_sprite.dimensions = new Vector2(m_cameraHeight * m_aspect, m_cameraHeight);
				MatchCollider();
				break;
			}
		}
	}

	private void MatchCollider()
	{
		if (m_collider != null)
		{
			m_collider.size = new Vector3(m_sprite.dimensions.x, m_sprite.dimensions.y, m_collider.size.z);
			switch (m_sprite.anchor)
			{
			case tk2dBaseSprite.Anchor.LowerCenter:
				m_collider.center = new Vector3(0f, m_collider.size.y / 2f, 0f);
				break;
			case tk2dBaseSprite.Anchor.LowerLeft:
				m_collider.center = new Vector3(m_collider.size.x / 2f, m_collider.size.y / 2f, 0f);
				break;
			case tk2dBaseSprite.Anchor.LowerRight:
				m_collider.center = new Vector3(0f - m_collider.size.x / 2f, m_collider.size.y / 2f, 0f);
				break;
			case tk2dBaseSprite.Anchor.MiddleCenter:
				m_collider.center = Vector3.zero;
				break;
			case tk2dBaseSprite.Anchor.MiddleLeft:
				m_collider.center = new Vector3(m_collider.size.x / 2f, 0f, 0f);
				break;
			case tk2dBaseSprite.Anchor.MiddleRight:
				m_collider.center = new Vector3(0f - m_collider.size.x / 2f, 0f, 0f);
				break;
			case tk2dBaseSprite.Anchor.UpperCenter:
				m_collider.center = new Vector3(0f, 0f - m_collider.size.y / 2f, 0f);
				break;
			case tk2dBaseSprite.Anchor.UpperLeft:
				m_collider.center = new Vector3(m_collider.size.x / 2f, 0f - m_collider.size.y / 2f, 0f);
				break;
			case tk2dBaseSprite.Anchor.UpperRight:
				m_collider.center = new Vector3(0f - m_collider.size.x / 2f, 0f - m_collider.size.y / 2f, 0f);
				break;
			}
		}
	}
}
