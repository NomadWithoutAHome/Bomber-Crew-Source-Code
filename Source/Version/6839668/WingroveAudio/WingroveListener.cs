using UnityEngine;

namespace WingroveAudio;

[AddComponentMenu("WingroveAudio/Wingrove Listeners")]
public class WingroveListener : MonoBehaviour
{
	private Vector3 m_position;

	public void UpdatePosition()
	{
		m_position = base.transform.position;
	}

	public Vector3 GetPosition()
	{
		return m_position;
	}

	private void OnEnable()
	{
		if (WingroveRoot.Instance != null)
		{
			WingroveRoot.Instance.RegisterListener(this);
		}
	}

	private void OnDisable()
	{
		if (WingroveRoot.Instance != null)
		{
			WingroveRoot.Instance.UnregisterListener(this);
		}
	}
}
