using UnityEngine;

public class CopyFOV : MonoBehaviour
{
	[SerializeField]
	private Camera m_thisCamera;

	[SerializeField]
	private Camera m_otherCamera;

	private void LateUpdate()
	{
		m_thisCamera.fieldOfView = m_otherCamera.fieldOfView;
	}
}
