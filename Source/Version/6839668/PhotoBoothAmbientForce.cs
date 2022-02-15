using UnityEngine;

public class PhotoBoothAmbientForce : MonoBehaviour
{
	[SerializeField]
	private Color m_ambientToForce;

	private Color m_prevAmbient;

	private void OnPreRender()
	{
		m_prevAmbient = RenderSettings.ambientLight;
		RenderSettings.ambientLight = m_ambientToForce;
	}

	private void OnPostRender()
	{
		RenderSettings.ambientLight = m_prevAmbient;
	}
}
