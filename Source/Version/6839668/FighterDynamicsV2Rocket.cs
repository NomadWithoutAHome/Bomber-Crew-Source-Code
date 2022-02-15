using BomberCrewCommon;
using UnityEngine;

public class FighterDynamicsV2Rocket : FighterDynamics
{
	[SerializeField]
	private float m_movementSpeedStart;

	[SerializeField]
	private float m_movementSpeedEnd;

	[SerializeField]
	private float m_movementSpeedTime;

	[SerializeField]
	private float m_maxHeight;

	private float m_t;

	public override Vector3 GetVelocity()
	{
		float t = Mathf.Clamp01(m_t / m_movementSpeedTime);
		return new Vector3(0f, Mathf.Lerp(m_movementSpeedStart, m_movementSpeedEnd, t), 0f);
	}

	public override void FixedUpdate()
	{
		if (!m_isDestroyed)
		{
			m_t += Time.deltaTime;
			base.gameObject.btransform().position += Time.deltaTime * new Vector3d(GetVelocity());
			base.transform.rotation = Quaternion.identity;
			if (base.gameObject.btransform().position.y > (double)m_maxHeight)
			{
				Singleton<MissionCoordinator>.Instance.FireTrigger("V2_ESCAPED");
				Object.Destroy(base.gameObject);
			}
		}
	}
}
