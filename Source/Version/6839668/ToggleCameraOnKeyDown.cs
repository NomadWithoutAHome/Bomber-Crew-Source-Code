using UnityEngine;

public class ToggleCameraOnKeyDown : MonoBehaviour
{
	[SerializeField]
	private KeyCode m_key;

	[SerializeField]
	private Camera m_camera;

	private void Update()
	{
		if (Input.GetKeyDown(m_key))
		{
			if (m_camera.enabled)
			{
				m_camera.enabled = false;
			}
			else
			{
				m_camera.enabled = true;
			}
		}
	}
}
