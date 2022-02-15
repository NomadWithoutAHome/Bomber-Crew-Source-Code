using System;
using BomberCrewCommon;
using UnityEngine;

public class BouncingBombTimingLights : MonoBehaviour
{
	[SerializeField]
	private float m_timingSpeed;

	[SerializeField]
	private GameObject[] m_displayQuads;

	[SerializeField]
	private GameObject m_quadsEndabledHierarchy;

	[SerializeField]
	private Transform m_raycastDownFromPos;

	[SerializeField]
	private float m_maxAltitude = 70f;

	[SerializeField]
	private LayerMask m_layerMaskRaycast;

	[SerializeField]
	private float m_lightSeparationMax;

	private float m_timingT;

	private bool m_aboveBounceWater;

	private void Update()
	{
		m_timingT += m_timingSpeed * Time.deltaTime;
		if (m_timingT > 1f)
		{
			m_timingT -= 1f;
		}
		BomberSystems bomberSystems = Singleton<BomberSpawn>.Instance.GetBomberSystems();
		BomberState bomberState = bomberSystems.GetBomberState();
		if (bomberState.GetAltitude() < m_maxAltitude)
		{
			float num = Mathf.Min(bomberState.GetAltitudeAboveGround(), bomberState.GetAltitude());
			Vector3 velocity = bomberState.GetPhysicsModel().GetVelocity();
			Vector3 vector = velocity;
			if (num < 0f)
			{
				num = 0f;
			}
			float num2 = Mathf.Sqrt(2f * num / 98.1f);
			Vector3 vector2 = num2 * vector;
			m_aboveBounceWater = false;
			if (Physics.Raycast(m_raycastDownFromPos.position + vector2, Vector3.down, out var hitInfo, m_maxAltitude, m_layerMaskRaycast) && hitInfo.collider.GetComponent<BounceWater>() != null)
			{
				m_aboveBounceWater = true;
				Vector3 vector3 = hitInfo.point + Vector3.up * 0.1f;
				Vector3 forward = m_raycastDownFromPos.forward;
				forward.y = 0f;
				forward = forward.normalized;
				float num3 = Mathf.Sin(m_timingT * (float)Math.PI * 2f) * m_lightSeparationMax * 0.5f;
				m_displayQuads[0].transform.SetPositionAndRotation(vector3 + forward * num3, Quaternion.identity);
				m_displayQuads[1].transform.SetPositionAndRotation(vector3 - forward * num3, Quaternion.identity);
			}
			m_quadsEndabledHierarchy.SetActive(m_aboveBounceWater);
		}
		else
		{
			m_quadsEndabledHierarchy.SetActive(value: false);
		}
	}

	public bool AboveBounceWater()
	{
		return m_aboveBounceWater;
	}

	public bool IsGoodTiming()
	{
		if (Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBomberState().GetAltitude() < m_maxAltitude)
		{
			return (m_timingT > 0.4f && m_timingT < 0.6f) || m_timingT < 0.1f || m_timingT > 0.9f;
		}
		return false;
	}
}
