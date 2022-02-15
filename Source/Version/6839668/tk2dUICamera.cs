using UnityEngine;

[AddComponentMenu("2D Toolkit/UI/Core/tk2dUICamera")]
public class tk2dUICamera : MonoBehaviour
{
	public enum tk2dRaycastType
	{
		Physics3D,
		Physics2D
	}

	[SerializeField]
	private LayerMask raycastLayerMask = -1;

	[SerializeField]
	private tk2dRaycastType raycastType;

	public tk2dRaycastType RaycastType => raycastType;

	public LayerMask FilteredMask => (int)raycastLayerMask & GetComponent<Camera>().cullingMask;

	public Camera HostCamera => GetComponent<Camera>();

	public void AssignRaycastLayerMask(LayerMask mask)
	{
		raycastLayerMask = mask;
	}

	private void OnEnable()
	{
		if (GetComponent<Camera>() == null)
		{
			DebugLogWrapper.LogError("tk2dUICamera should only be attached to a camera.");
			base.enabled = false;
		}
		else if (!GetComponent<Camera>().orthographic && raycastType == tk2dRaycastType.Physics2D)
		{
			DebugLogWrapper.LogError("tk2dUICamera - Physics2D raycast only works with orthographic cameras.");
			base.enabled = false;
		}
		else
		{
			tk2dUIManager.RegisterCamera(this);
		}
	}

	private void OnDisable()
	{
		tk2dUIManager.UnregisterCamera(this);
	}
}
