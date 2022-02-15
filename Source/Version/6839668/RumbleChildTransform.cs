using BomberCrewCommon;
using UnityEngine;

public class RumbleChildTransform : MonoBehaviour
{
	[SerializeField]
	private Transform m_transformToRumble;

	[SerializeField]
	private float m_factor;

	[SerializeField]
	private BomberCamera m_bomberCamera;

	private bool m_isDisabled;

	private bool m_hasNaN;

	public void SetDisabled(bool disabled)
	{
		m_isDisabled = disabled;
	}

	private void Update()
	{
		if (m_isDisabled)
		{
			return;
		}
		m_factor = m_bomberCamera.GetRumbleFactor();
		if (!m_hasNaN)
		{
			Vector3 vector = Singleton<RumbleMixer>.Instance.GetCurrentRumble() * m_factor;
			if (float.IsNaN(vector.magnitude) || float.IsNaN(vector.x) || float.IsNaN(vector.y) || float.IsNaN(vector.z))
			{
				DebugLogWrapper.LogError("[RUMBLER] There are still NaNs in the rumbler :(");
				m_hasNaN = true;
			}
			if (!m_hasNaN)
			{
				m_transformToRumble.localPosition = Singleton<RumbleMixer>.Instance.GetCurrentRumble() * m_factor;
			}
			else
			{
				m_transformToRumble.localPosition = Vector3.zero;
			}
		}
		else
		{
			m_transformToRumble.localPosition = Vector3.zero;
		}
	}
}
