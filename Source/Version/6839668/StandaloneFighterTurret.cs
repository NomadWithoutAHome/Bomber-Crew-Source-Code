using BomberCrewCommon;
using UnityEngine;

public class StandaloneFighterTurret : MonoBehaviour
{
	[SerializeField]
	private Transform m_rotateXHierarchy;

	[SerializeField]
	private Transform m_rotateYHierarchy;

	[SerializeField]
	private Transform m_gunForwardNode;

	[SerializeField]
	private RaycastGunTurret[] m_guns;

	[SerializeField]
	private float m_range;

	[SerializeField]
	private float m_aimRequirement = 0.9f;

	[SerializeField]
	private FighterDynamics m_dynamics;

	[SerializeField]
	private float m_minRotationX;

	[SerializeField]
	private float m_maxRotationX;

	[SerializeField]
	private float m_minRotationY;

	[SerializeField]
	private float m_maxRotationY;

	[SerializeField]
	private float m_aimAcceleration;

	[SerializeField]
	private float m_aimSpeedDegrees;

	[SerializeField]
	private float m_damping;

	[SerializeField]
	private FighterPlane m_fighterPlane;

	[SerializeField]
	private float m_aimInaccuracyFrequency = 0.2f;

	[SerializeField]
	private bool m_isAce;

	[SerializeField]
	private float m_inaccuracyScaleMultiplier = 1f;

	private BomberState m_bomber;

	private float m_angleX;

	private float m_angleY;

	private float m_yRotateVelocity;

	private float m_xRotateVelocity;

	private float m_aimPhase;

	private void Start()
	{
		m_aimPhase = Random.Range(0f, 1f);
		m_bomber = Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBomberState();
		float num = Random.Range(0f, 10f);
	}

	private void UpdateAim(float aimXTarget, float aimYTarget)
	{
		float num = aimYTarget - m_angleY;
		if (num > 180f)
		{
			num -= 360f;
		}
		else if (num < -180f)
		{
			num += 360f;
		}
		m_yRotateVelocity += Time.deltaTime * m_aimAcceleration * (num / 90f);
		m_yRotateVelocity = Mathf.Clamp(m_yRotateVelocity, 0f - m_aimSpeedDegrees, m_aimSpeedDegrees);
		m_angleY += m_yRotateVelocity * Time.deltaTime;
		float num2 = aimXTarget - m_angleX;
		if (num2 > 180f)
		{
			num2 -= 360f;
		}
		else if (num2 < -180f)
		{
			num2 += 360f;
		}
		m_xRotateVelocity += Time.deltaTime * m_aimAcceleration * (num2 / 90f);
		m_xRotateVelocity = Mathf.Clamp(m_xRotateVelocity, 0f - m_aimSpeedDegrees, m_aimSpeedDegrees);
		m_angleX += m_xRotateVelocity * Time.deltaTime;
		m_xRotateVelocity -= Time.deltaTime * m_xRotateVelocity * m_damping;
		m_yRotateVelocity -= Time.deltaTime * m_yRotateVelocity * m_damping;
		if (m_angleY > 180f)
		{
			m_angleY -= 360f;
		}
		if (m_angleY < -180f)
		{
			m_angleY += 360f;
		}
		if (m_angleX > 180f)
		{
			m_angleX -= 360f;
		}
		if (m_angleX < -180f)
		{
			m_angleX += 360f;
		}
		float num3 = 5f;
		float num4 = aimXTarget - m_angleX;
		float num5 = aimYTarget - m_angleY;
		if (num4 > 180f)
		{
			num4 -= 360f;
		}
		else if (num4 < -180f)
		{
			num4 += 360f;
		}
		if (num5 > 180f)
		{
			num5 -= 360f;
		}
		else if (num5 < -180f)
		{
			num5 += 360f;
		}
		m_angleX += num4 * Mathf.Clamp01(num3 * Time.deltaTime);
		m_angleY += num5 * Mathf.Clamp01(num3 * Time.deltaTime);
		m_angleX = Mathf.Clamp(m_angleX, m_minRotationX, m_maxRotationX);
		m_angleY = Mathf.Clamp(m_angleY, m_minRotationY, m_maxRotationY);
		if (m_rotateYHierarchy != null)
		{
			m_rotateYHierarchy.localRotation = Quaternion.Euler(m_angleY, 0f, 0f);
		}
		if (m_rotateXHierarchy != null)
		{
			m_rotateXHierarchy.localRotation = Quaternion.Euler(0f, m_angleX, 0f);
		}
	}

	private void Update()
	{
		m_aimPhase += Time.deltaTime * m_aimInaccuracyFrequency;
		if (m_aimPhase > 1f)
		{
			m_aimPhase -= 1f;
		}
		Vector3 position = m_bomber.transform.position;
		float inaccuracyFactor = ((!m_isAce) ? (AimingUtils.GetInaccuracyFighter(Singleton<VisibilityHelpers>.Instance.GetNightFactor(), m_bomber.IsEvasive(), m_bomber.IsLitUp(), m_aimPhase) * m_inaccuracyScaleMultiplier) : 0f);
		Vector3 aimAheadTarget = AimingUtils.GetAimAheadTarget(base.transform.position, position, m_bomber.GetPhysicsModel().GetVelocity() - m_fighterPlane.GetCachedVelocity(), m_guns[0].GetMuzzleVelocity(), inaccuracyFactor, isGroundBasedOrFriendly: false);
		Vector3 vector = aimAheadTarget - m_gunForwardNode.transform.position;
		Vector3 normalized = vector.normalized;
		Vector3 vector2 = base.transform.worldToLocalMatrix.MultiplyVector(normalized);
		float aimYTarget = Mathf.Atan2(0f - vector2.y, new Vector3(vector2.x, 0f, vector2.z).magnitude) * 57.29578f;
		float aimXTarget = Mathf.Atan2(0f - vector2.z, vector2.x) * 57.29578f;
		UpdateAim(aimXTarget, aimYTarget);
		float num = Vector3.Dot(vector.normalized, m_gunForwardNode.forward);
		if (num > m_aimRequirement && vector.magnitude < m_range)
		{
			RaycastGunTurret[] guns = m_guns;
			foreach (RaycastGunTurret raycastGunTurret in guns)
			{
				raycastGunTurret.SetFiring(firing: true, m_dynamics.GetVelocity());
			}
		}
		else
		{
			RaycastGunTurret[] guns2 = m_guns;
			foreach (RaycastGunTurret raycastGunTurret2 in guns2)
			{
				raycastGunTurret2.SetFiring(firing: false, Vector3.zero);
			}
		}
	}
}
