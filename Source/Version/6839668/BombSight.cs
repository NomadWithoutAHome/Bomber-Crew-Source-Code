using BomberCrewCommon;
using UnityEngine;

public class BombSight : MonoBehaviour
{
	[SerializeField]
	private BomberState m_bomberState;

	[SerializeField]
	private Transform m_forwardTransform;

	[SerializeField]
	private Transform m_cameraPosition;

	[SerializeField]
	private StationBombAimer m_bombAimerStation;

	[SerializeField]
	private float m_maxInaccuracy;

	[SerializeField]
	private float m_minInaccuracy;

	[SerializeField]
	private CrewmanSkillAbilityFloat m_aimingSkill;

	[SerializeField]
	private LayerMask m_wouldHitLayerMask;

	private Vector3 m_positionAverage;

	private float m_t;

	private string m_aimingSkillId;

	private void Start()
	{
		m_aimingSkillId = m_aimingSkill.GetCachedName();
	}

	private void Update()
	{
		float num = Mathf.Min(m_bomberState.GetAltitudeAboveGround(), m_bomberState.GetAltitude());
		Vector3 velocity = m_bomberState.GetPhysicsModel().GetVelocity();
		Vector3 vector = Vector3.forward * Vector3.Dot(m_forwardTransform.forward, velocity);
		m_t += Time.deltaTime;
		float t = 0f;
		CrewmanAvatar currentCrewman = m_bombAimerStation.GetCurrentCrewman();
		if (currentCrewman != null)
		{
			t = Singleton<CrewmanSkillUpgradeInfo>.Instance.GetProficiency(m_aimingSkillId, currentCrewman.GetCrewman());
		}
		float num2 = Mathf.Lerp(m_maxInaccuracy, m_minInaccuracy, t);
		if (num < 100f)
		{
			float num3 = Mathf.Clamp01((num - 50f) / 50f);
			num2 *= num3;
		}
		if (num < 0f)
		{
			num = 0f;
		}
		float num4 = Mathf.Sqrt(2f * num / 98.1f);
		Vector3 b = num4 * vector + new Vector3(Mathf.Sin(m_t), 0f, Mathf.Cos(m_t * 0.7f)) * num2;
		m_positionAverage = Vector3.Lerp(m_positionAverage, b, Mathf.Clamp01(Time.deltaTime * 10f));
		m_cameraPosition.localPosition = m_positionAverage;
	}

	public Transform GetCameraPosition()
	{
		return m_cameraPosition;
	}

	public bool WouldHitTargetStrict()
	{
		if (Physics.Raycast(m_cameraPosition.position, -Vector3.up, out var hitInfo, 3000f, m_wouldHitLayerMask) && hitInfo.collider.GetComponent<MissionTarget>() != null)
		{
			return true;
		}
		return false;
	}

	public bool WouldHitTarget()
	{
		Vector3 velocity = m_bomberState.GetPhysicsModel().GetVelocity();
		if (Physics.Raycast(m_cameraPosition.position + velocity * 1.5f, -Vector3.up, out var hitInfo, 3000f, m_wouldHitLayerMask) && hitInfo.collider.GetComponent<MissionTarget>() != null)
		{
			return true;
		}
		if (Physics.Raycast(m_cameraPosition.position + velocity * 1f, -Vector3.up, out hitInfo, 3000f, m_wouldHitLayerMask) && hitInfo.collider.GetComponent<MissionTarget>() != null)
		{
			return true;
		}
		if (Physics.Raycast(m_cameraPosition.position + velocity * 0.5f, -Vector3.up, out hitInfo, 3000f, m_wouldHitLayerMask) && hitInfo.collider.GetComponent<MissionTarget>() != null)
		{
			return true;
		}
		if (Physics.Raycast(m_cameraPosition.position + velocity * -0.5f, -Vector3.up, out hitInfo, 3000f, m_wouldHitLayerMask) && hitInfo.collider.GetComponent<MissionTarget>() != null)
		{
			return true;
		}
		if (Physics.Raycast(m_cameraPosition.position + velocity * -1f, -Vector3.up, out hitInfo, 3000f, m_wouldHitLayerMask) && hitInfo.collider.GetComponent<MissionTarget>() != null)
		{
			return true;
		}
		return false;
	}
}
