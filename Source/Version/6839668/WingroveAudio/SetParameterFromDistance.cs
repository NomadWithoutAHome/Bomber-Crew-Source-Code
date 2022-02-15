using UnityEngine;

namespace WingroveAudio;

public class SetParameterFromDistance : MonoBehaviour
{
	[SerializeField]
	[AudioParameterName]
	private string m_parameterToSet;

	[SerializeField]
	private float m_minDist;

	[SerializeField]
	private float m_maxDist;

	[SerializeField]
	private bool m_smooth;

	[SerializeField]
	private bool m_forObject;

	[SerializeField]
	private AudioArea m_useAudioArea;

	private int m_cachedGameObjectId;

	private int m_parameterId;

	private void Update()
	{
		float setValue = 0f;
		WingroveListener singleListener = WingroveRoot.Instance.GetSingleListener();
		if (singleListener != null)
		{
			float magnitude = (singleListener.transform.position - base.transform.position).magnitude;
			if (m_useAudioArea != null)
			{
				Vector3 listeningPosition = m_useAudioArea.GetListeningPosition(singleListener.transform.position, m_useAudioArea.transform.position);
				magnitude = (singleListener.transform.position - listeningPosition).magnitude;
			}
			setValue = Mathf.Clamp01((magnitude - m_minDist) / (m_maxDist - m_minDist));
		}
		if (m_parameterId == 0)
		{
			m_parameterId = WingroveRoot.Instance.GetParameterId(m_parameterToSet);
		}
		if (m_forObject)
		{
			if (m_cachedGameObjectId == 0)
			{
				m_cachedGameObjectId = base.gameObject.GetInstanceID();
			}
			WingroveRoot.Instance.SetParameterForObject(m_parameterId, m_cachedGameObjectId, base.gameObject, setValue);
		}
		else
		{
			WingroveRoot.Instance.SetParameterGlobal(m_parameterId, setValue);
		}
	}
}
