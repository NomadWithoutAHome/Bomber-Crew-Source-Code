using UnityEngine;

public class PointingWidget : MonoBehaviour
{
	[SerializeField]
	private tk2dSlicedSprite m_slicedSpriteToSize;

	[SerializeField]
	private float m_scale;

	[SerializeField]
	private Vector2 m_padding;

	[SerializeField]
	private GameObject m_pointingAtNullHierarchy;

	[SerializeField]
	private GameObject m_pointingAtNonNullHierarchy;

	[SerializeField]
	private FlashManager m_flashManager;

	private FlashManager.ActiveFlash m_activeFlash;

	public void Flash()
	{
		m_activeFlash = m_flashManager.AddOrUpdateFlash(0.8f, 2.6f, 0f, 1, 1f, Color.white, m_activeFlash);
	}

	public void SetPointedAt(UISelectorMovementType movementTracker, tk2dCamera camera)
	{
		tk2dUIItem currentlyPointedAtItem = movementTracker.GetCurrentlyPointedAtItem();
		if (m_pointingAtNonNullHierarchy != null)
		{
			m_pointingAtNonNullHierarchy.SetActive(currentlyPointedAtItem != null);
		}
		if (m_pointingAtNullHierarchy != null)
		{
			m_pointingAtNullHierarchy.SetActive(currentlyPointedAtItem == null);
		}
		if (movementTracker.UseScreenSpacePointerPosition())
		{
			Vector3 position = movementTracker.GetCurrentScreenSpacePointerPosition();
			Vector3 position2 = camera.ScreenCamera.ScreenToWorldPoint(position);
			position2.z = 0f;
			base.transform.position = position2;
		}
		else if (currentlyPointedAtItem != null)
		{
			if (m_slicedSpriteToSize != null && currentlyPointedAtItem.GetComponent<BoxCollider>() != null)
			{
				m_slicedSpriteToSize.dimensions = currentlyPointedAtItem.GetComponent<BoxCollider>().size * m_scale;
			}
			tk2dCamera tk2dCamera2 = tk2dCamera.CameraForLayer(currentlyPointedAtItem.gameObject.layer);
			if (tk2dCamera2 != null)
			{
				UISelectorPointingHint component = currentlyPointedAtItem.GetComponent<UISelectorPointingHint>();
				Transform transform = ((!(component == null)) ? component.GetPointingHint() : currentlyPointedAtItem.transform);
				Vector3 position3 = tk2dCamera2.ScreenCamera.WorldToScreenPoint(transform.position);
				Vector3 position4 = camera.ScreenCamera.ScreenToWorldPoint(position3);
				position4.z = 0f;
				base.transform.position = position4;
			}
			else
			{
				Camera main = Camera.main;
				UISelectorPointingHint component2 = currentlyPointedAtItem.GetComponent<UISelectorPointingHint>();
				Transform transform2 = ((!(component2 == null)) ? component2.GetPointingHint() : currentlyPointedAtItem.transform);
				Vector3 position5 = main.WorldToScreenPoint(transform2.position);
				Vector3 position6 = camera.ScreenCamera.ScreenToWorldPoint(position5);
				position6.z = 0f;
				base.transform.position = position6;
			}
		}
	}
}
