using UnityEngine;

namespace WingroveAudio;

public class SetParameterFromOcclusion : MonoBehaviour
{
	[SerializeField]
	[AudioParameterName]
	private string m_parameterToSet;

	[SerializeField]
	private float m_distanceContributionPerUnit;

	[SerializeField]
	private float m_fadeSpeedLinear = 1f;

	[SerializeField]
	private float m_fadeSpeedRelative;

	[SerializeField]
	private float m_contributionPerCollider = 1f;

	[SerializeField]
	private LayerMask m_layersToRaycast;

	[SerializeField]
	private float m_ignoreDistanceNear;

	[SerializeField]
	private float m_ignoreDistanceFar;

	[SerializeField]
	private bool m_forObject = true;

	private float m_occlusion;

	private bool m_hasRun;

	private int m_cachedGameObjectId;

	private int m_parameterId;

	private void Update()
	{
		float num = 0f;
		WingroveListener singleListener = WingroveRoot.Instance.GetSingleListener();
		if (singleListener != null)
		{
			Vector3 vector = singleListener.transform.position - base.transform.position;
			Vector3 origin = base.transform.position + vector.normalized * m_ignoreDistanceNear;
			num += vector.magnitude * m_distanceContributionPerUnit;
			RaycastHit[] array = Physics.RaycastAll(origin, vector.normalized, vector.magnitude - (m_ignoreDistanceFar + m_ignoreDistanceNear), m_layersToRaycast, QueryTriggerInteraction.Ignore);
			RaycastHit[] array2 = array;
			foreach (RaycastHit raycastHit in array2)
			{
				num += m_contributionPerCollider;
			}
		}
		if (!m_hasRun)
		{
			m_occlusion = num;
			m_hasRun = true;
		}
		else
		{
			if (num > m_occlusion)
			{
				m_occlusion += (num - m_occlusion) * m_fadeSpeedRelative * Time.deltaTime;
				m_occlusion += m_fadeSpeedLinear * Time.deltaTime;
				if (m_occlusion > num)
				{
					m_occlusion = num;
				}
			}
			if (num < m_occlusion)
			{
				m_occlusion += (num - m_occlusion) * m_fadeSpeedRelative * Time.deltaTime;
				m_occlusion -= m_fadeSpeedLinear * Time.deltaTime;
				if (m_occlusion < num)
				{
					m_occlusion = num;
				}
			}
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
			WingroveRoot.Instance.SetParameterForObject(m_parameterId, m_cachedGameObjectId, base.gameObject, m_occlusion);
		}
		else
		{
			WingroveRoot.Instance.SetParameterGlobal(m_parameterId, m_occlusion);
		}
	}
}
