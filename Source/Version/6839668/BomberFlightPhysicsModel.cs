using System.Collections.Generic;
using System.Linq;
using AudioNames;
using BomberCrewCommon;
using UnityEngine;
using WingroveAudio;

public class BomberFlightPhysicsModel : MonoBehaviour
{
	[SerializeField]
	private Rigidbody m_landingMesh;

	[SerializeField]
	private Transform m_baseTransform;

	[SerializeField]
	private Transform[] m_landingGearCopy;

	[SerializeField]
	private float m_yokeAngleEffectivenessPerVelocity;

	[SerializeField]
	private float m_yokeAngleEffectivenessPerEngineRate;

	[SerializeField]
	private float m_bankAngleEffectivenessPerVelocity;

	[SerializeField]
	private float m_rollToYawEffectivenessPerVelocity;

	[SerializeField]
	private float m_acceleration;

	[SerializeField]
	private float m_accelerationVertical;

	[SerializeField]
	private float m_horizontalResistanceAir;

	[SerializeField]
	private float m_verticalResistanceAir;

	[SerializeField]
	private float m_horizontalResistanceAirSpeedReq;

	[SerializeField]
	private float m_horizontalResistanceAirSpeedReqMin;

	[SerializeField]
	private float m_angularResistanceAir;

	[SerializeField]
	private float m_horiziontalResistanceBrakes;

	[SerializeField]
	private float m_horiziontalResistanceWheels;

	[SerializeField]
	private float m_horizontalResistanceScrape;

	[SerializeField]
	private float m_angularDeadZone = 3f;

	[SerializeField]
	private float m_minVelocityForLift;

	[SerializeField]
	private float m_maxVelocityForLift;

	[SerializeField]
	private Transform m_liftTransform;

	[SerializeField]
	private Transform m_centreOfMass;

	[SerializeField]
	private AnimationCurve m_liftEffectivenessCurve;

	[SerializeField]
	private AnimationCurve m_engineEfficiencyCurve;

	[SerializeField]
	private AnimationCurve m_fuelEfficiencyCurve;

	[SerializeField]
	private AnimationCurve m_fuelVerticalUsage;

	[SerializeField]
	private float m_frontHeavyRate;

	[SerializeField]
	private float m_damagePerVelocity;

	[SerializeField]
	private float m_groundCollisionDamageRadius;

	[SerializeField]
	private float[] m_fuelMixesForce;

	[SerializeField]
	private float[] m_fuelMixesUsage;

	[SerializeField]
	private BomberSystems m_bomberSystems;

	[SerializeField]
	private GameObject[] m_liftGeneratingTransformsLeft;

	[SerializeField]
	private GameObject[] m_liftGeneratingTransformsRight;

	[SerializeField]
	private GameObject[] m_stabilityGeneratingTransforms;

	[SerializeField]
	private GameObject[] m_criticalTransformsAdditional;

	[SerializeField]
	private AeroSurfacesEfficiency m_aeroSurfaces;

	[SerializeField]
	private Collider[] m_landingGearColliders;

	[SerializeField]
	private float m_velocityRedirectionalise = 0.2f;

	[SerializeField]
	private float m_baseAngularDrag = 0.05f;

	[SerializeField]
	private float m_landingDownAngularDrag = 1f;

	private float m_yokeAngle;

	private float m_landingBrake;

	private HashSet<Collider> m_groundCollisionColliders = new HashSet<Collider>();

	private float m_bank;

	private int m_fuelMix = 1;

	private float m_fuelMixTracked = 1f;

	private float m_lastFuelUsage;

	private float m_bankEffectMultiplierTarget = 1f;

	private float m_bankEffectMultiplier = 1f;

	private float m_doingManeuvreCountdown;

	private int m_fuelUsageAverageFrameDelay;

	private int m_fuelUsageCurFrame;

	private float[] m_fuelUsageTotals = new float[60];

	private bool m_simplifiedModel;

	private bool m_allWheelsOnGround;

	private bool m_doLandingFX;

	private bool m_hasDoneBadDescendTrigger;

	private float m_previousEngineForce;

	private Dictionary<Collider, BomberPhysicsCollisionDamage> m_collisionDamage = new Dictionary<Collider, BomberPhysicsCollisionDamage>();

	public bool IsSimplified()
	{
		return m_simplifiedModel;
	}

	public float GetPreviousEngineForce()
	{
		return m_previousEngineForce;
	}

	private void Awake()
	{
		m_fuelMix = 1;
		GetComponent<Rigidbody>().centerOfMass = m_centreOfMass.localPosition;
		Collider[] componentsInChildren = GetComponentsInChildren<Collider>();
		Collider[] array = componentsInChildren;
		foreach (Collider collider in array)
		{
			m_collisionDamage[collider] = collider.GetComponent<BomberPhysicsCollisionDamage>();
		}
		m_landingMesh.transform.parent = null;
	}

	public void SetBankEffectMultiplier(float amt)
	{
		m_bankEffectMultiplierTarget = amt;
	}

	public void SetDoingManeuvre()
	{
		m_doingManeuvreCountdown = 1f;
	}

	private void OnDestroy()
	{
	}

	public void ForceVelocity(float v)
	{
		m_landingMesh.velocity = m_liftTransform.forward * v;
	}

	public float GetLastFuelUsage()
	{
		return m_lastFuelUsage;
	}

	public void SetHorizontalForces(int fuelMix, float brakes)
	{
		m_fuelMix = fuelMix;
		m_landingBrake = brakes;
	}

	public void SetYoke(float normalisedSteer)
	{
		m_yokeAngle = Mathf.Clamp(normalisedSteer, -1f, 1f);
	}

	public void SetYokeBank(float normalisedBank)
	{
		m_bank = Mathf.Clamp(normalisedBank, -1f, 1f);
	}

	public Vector3 GetVelocity()
	{
		return m_landingMesh.velocity;
	}

	public Vector3 GetHeading()
	{
		return m_liftTransform.forward;
	}

	public float GetBanking()
	{
		return GetEulerCentered().x;
	}

	public float GetBankingInput()
	{
		return m_bank;
	}

	public float GetPitchInput()
	{
		return m_yokeAngle;
	}

	public float GetTurningVelocity()
	{
		return 0f - m_landingMesh.angularVelocity.y;
	}

	public float GetPitch()
	{
		return GetEulerCentered().z;
	}

	public float GetAngularVelocityTotal()
	{
		return m_landingMesh.angularVelocity.magnitude;
	}

	private void OnCollisionStay(Collision c)
	{
		float damagePerUnit = m_damagePerVelocity / (float)c.contacts.Length;
		ContactPoint[] contacts = c.contacts;
		for (int i = 0; i < contacts.Length; i++)
		{
			ContactPoint contactPoint = contacts[i];
			Collider thisCollider = contactPoint.thisCollider;
			BomberPhysicsCollisionDamage value = null;
			m_collisionDamage.TryGetValue(thisCollider, out value);
			if (value != null)
			{
				GroundCollisionType.GroundCollisionEffectType gct = GroundCollisionType.GroundCollisionEffectType.Default;
				GroundCollisionType component = c.collider.GetComponent<GroundCollisionType>();
				if (component != null)
				{
					gct = component.GetEffectType();
				}
				value.DoDamage(GetVelocity(), contactPoint.normal, contactPoint.point, damagePerUnit, gct);
			}
		}
		ContactPoint[] contacts2 = c.contacts;
		foreach (ContactPoint contactPoint2 in contacts2)
		{
			m_groundCollisionColliders.Add(contactPoint2.thisCollider);
		}
	}

	private void OnCollisionEnter(Collision c)
	{
		float damagePerUnit = m_damagePerVelocity / (float)c.contacts.Length;
		ContactPoint[] contacts = c.contacts;
		for (int i = 0; i < contacts.Length; i++)
		{
			ContactPoint contactPoint = contacts[i];
			Collider thisCollider = contactPoint.thisCollider;
			BomberPhysicsCollisionDamage value = null;
			m_collisionDamage.TryGetValue(thisCollider, out value);
			if (value != null)
			{
				GroundCollisionType.GroundCollisionEffectType gct = GroundCollisionType.GroundCollisionEffectType.Default;
				GroundCollisionType component = c.collider.GetComponent<GroundCollisionType>();
				if (component != null)
				{
					gct = component.GetEffectType();
				}
				value.DoDamage(GetVelocity(), contactPoint.normal, contactPoint.point, damagePerUnit, gct);
			}
		}
		ContactPoint[] contacts2 = c.contacts;
		foreach (ContactPoint contactPoint2 in contacts2)
		{
			m_groundCollisionColliders.Add(contactPoint2.thisCollider);
		}
	}

	private void OnCollisionExit(Collision c)
	{
		m_groundCollisionColliders.Clear();
		ContactPoint[] contacts = c.contacts;
		foreach (ContactPoint contactPoint in contacts)
		{
			m_groundCollisionColliders.Add(contactPoint.thisCollider);
		}
	}

	public bool IsOnGround()
	{
		return m_groundCollisionColliders.Count > 0;
	}

	private Vector3 GetEulerCentered()
	{
		Vector3 eulerAngles = m_landingMesh.transform.rotation.eulerAngles;
		if (eulerAngles.x > 180f)
		{
			eulerAngles.x -= 360f;
		}
		else if (eulerAngles.x < -180f)
		{
			eulerAngles.x += 360f;
		}
		if (eulerAngles.y > 180f)
		{
			eulerAngles.y -= 360f;
		}
		else if (eulerAngles.y < -180f)
		{
			eulerAngles.y += 360f;
		}
		if (eulerAngles.z > 180f)
		{
			eulerAngles.z -= 360f;
		}
		else if (eulerAngles.z < -180f)
		{
			eulerAngles.z += 360f;
		}
		return eulerAngles;
	}

	public float GetFuelMixForce()
	{
		int num = Mathf.Clamp(Mathf.FloorToInt(m_fuelMixTracked), 0, m_fuelMixesUsage.Length - 2);
		int num2 = num + 1;
		float t = Mathf.Clamp(m_fuelMixTracked - (float)num, 0f, 1f);
		return Mathf.Lerp(m_fuelMixesForce[num], m_fuelMixesForce[num2], t);
	}

	public float GetFuelMixUsage()
	{
		int num = Mathf.Clamp(Mathf.FloorToInt(m_fuelMixTracked), 0, m_fuelMixesUsage.Length - 2);
		int num2 = num + 1;
		float t = Mathf.Clamp(m_fuelMixTracked - (float)num, 0f, 1f);
		return Mathf.Lerp(m_fuelMixesUsage[num], m_fuelMixesUsage[num2], t);
	}

	public bool ShouldUseSimplifiedModel()
	{
		if (m_simplifiedModel && base.transform.position.y < 30f && !m_bomberSystems.GetBomberState().HasNotCompletedTakeOff())
		{
			return true;
		}
		return false;
	}

	public void SetSimplifiedModel()
	{
		if (!m_simplifiedModel)
		{
			Vector3 velocity = m_landingMesh.velocity;
			velocity.y = 0f;
			velocity = velocity.normalized;
			velocity.y += Random.Range(0.25f, 0.4f);
			m_landingMesh.drag = 0.4f;
			m_landingMesh.angularDrag = 0.2f;
			m_landingMesh.velocity += velocity * 30f;
			m_landingMesh.angularVelocity += Random.onUnitSphere * 40f;
			Singleton<DifficultyMagic>.Instance.SetDisabled();
		}
		m_simplifiedModel = true;
	}

	public void SetDoLandingFX()
	{
		m_doLandingFX = true;
	}

	public bool AllWheelsOnGround()
	{
		return m_allWheelsOnGround;
	}

	private void FixedUpdate()
	{
		m_doingManeuvreCountdown -= Time.deltaTime * 0.2f;
		if (m_doingManeuvreCountdown < 0f)
		{
			m_doingManeuvreCountdown = 0f;
		}
		int num = m_fuelMix;
		if (m_doingManeuvreCountdown > 0f)
		{
			num = ((!(m_doingManeuvreCountdown > 0.9f)) ? 2 : 3);
		}
		if (m_fuelMixTracked > (float)num)
		{
			m_fuelMixTracked -= Time.deltaTime;
			if (m_fuelMixTracked < (float)num)
			{
				m_fuelMixTracked = num;
			}
		}
		else if (m_fuelMixTracked < (float)num)
		{
			m_fuelMixTracked += Time.deltaTime;
			if (m_fuelMixTracked > (float)num)
			{
				m_fuelMixTracked = num;
			}
		}
		if (m_bankEffectMultiplierTarget > m_bankEffectMultiplier)
		{
			m_bankEffectMultiplier += Time.deltaTime * 3f;
			if (m_bankEffectMultiplierTarget < m_bankEffectMultiplier)
			{
				m_bankEffectMultiplier = m_bankEffectMultiplierTarget;
			}
		}
		else if (m_bankEffectMultiplierTarget < m_bankEffectMultiplier)
		{
			m_bankEffectMultiplier -= Time.deltaTime;
			if (m_bankEffectMultiplierTarget > m_bankEffectMultiplier)
			{
				m_bankEffectMultiplier = m_bankEffectMultiplierTarget;
			}
		}
		int num2 = 0;
		GameObject[] stabilityGeneratingTransforms = m_stabilityGeneratingTransforms;
		foreach (GameObject gameObject in stabilityGeneratingTransforms)
		{
			if (gameObject != null)
			{
				num2++;
			}
		}
		float num3 = (float)num2 / (float)m_stabilityGeneratingTransforms.Length;
		float num4 = 0.5f + (float)num2 / (float)m_stabilityGeneratingTransforms.Length * 0.5f;
		int engineCount = m_bomberSystems.GetEngineCount();
		float num5 = 0f;
		for (int j = 0; j < engineCount; j++)
		{
			Engine engine = m_bomberSystems.GetEngine(j);
			if (m_doingManeuvreCountdown > 0f)
			{
				num5 += 1f;
			}
			else if (engine != null && engine.IsOn())
			{
				num5 += engine.GetCapability();
			}
		}
		float value = num5 / (float)engineCount;
		if (m_simplifiedModel)
		{
			value = 0f;
		}
		float num6 = Vector3.Dot(m_liftTransform.forward, Vector3.up);
		float num7 = ((!(m_doingManeuvreCountdown > 0f)) ? Mathf.Pow(1f - Mathf.Abs(num6), 0.5f) : 1f);
		float num8 = (m_previousEngineForce = GetFuelMixForce() * m_engineEfficiencyCurve.Evaluate(Mathf.Clamp01(value)));
		float num9 = Mathf.Max(num8, (m_groundCollisionColliders.Count != 0) ? 0f : (GetVelocity().magnitude * Mathf.Clamp(num7, 0f, 0.5f)));
		float num10 = Mathf.Max(num9 - num8, 0f);
		float num11 = m_fuelEfficiencyCurve.Evaluate(Mathf.Clamp01(value)) * m_engineEfficiencyCurve.Evaluate(Mathf.Clamp01(value)) * GetFuelMixUsage();
		Vector3 vector = m_landingMesh.transform.position - m_baseTransform.position;
		m_baseTransform.SetPositionAndRotation(m_landingMesh.transform.position, m_landingMesh.transform.rotation);
		if (!m_simplifiedModel)
		{
			m_landingMesh.velocity += num9 * Time.deltaTime * m_acceleration * (1f - Mathf.Abs(Vector3.Dot(m_liftTransform.forward, Vector3.up))) * m_liftTransform.transform.forward;
		}
		float magnitude = (m_landingMesh.velocity - Vector3.up * Vector3.Dot(m_landingMesh.velocity, Vector3.up)).magnitude;
		float num12 = Mathf.Clamp01(magnitude - m_horizontalResistanceAirSpeedReqMin / (m_horizontalResistanceAirSpeedReq - m_horizontalResistanceAirSpeedReqMin)) * num7;
		float num13 = 0f;
		GameObject[] liftGeneratingTransformsLeft = m_liftGeneratingTransformsLeft;
		foreach (GameObject gameObject2 in liftGeneratingTransformsLeft)
		{
			if (gameObject2 != null || m_doingManeuvreCountdown > 0f)
			{
				num13 = Mathf.Clamp01(num13 + 0.5f);
			}
		}
		float num14 = 0f;
		GameObject[] liftGeneratingTransformsRight = m_liftGeneratingTransformsRight;
		foreach (GameObject gameObject3 in liftGeneratingTransformsRight)
		{
			if (gameObject3 != null || m_doingManeuvreCountdown > 0f)
			{
				num14 = Mathf.Clamp01(num14 + 0.5f);
			}
		}
		Vector3 vector2 = m_landingMesh.velocity - Vector3.up * Vector3.Dot(m_landingMesh.velocity, Vector3.up);
		if (!m_simplifiedModel)
		{
			m_landingMesh.velocity -= Time.deltaTime * m_horizontalResistanceAir * num12 * vector2;
			vector2 = m_landingMesh.velocity - Vector3.up * Vector3.Dot(m_landingMesh.velocity, Vector3.up);
		}
		if (!m_simplifiedModel)
		{
			if (m_groundCollisionColliders.Count > 0)
			{
				bool flag = true;
				bool flag2 = true;
				Collider[] landingGearColliders = m_landingGearColliders;
				foreach (Collider collider in landingGearColliders)
				{
					if (collider == null || !m_groundCollisionColliders.Contains(collider))
					{
						flag = false;
					}
				}
				foreach (Collider groundCollisionCollider in m_groundCollisionColliders)
				{
					if (!m_landingGearColliders.Contains(groundCollisionCollider))
					{
						flag2 = false;
					}
				}
				if (flag2)
				{
					m_landingMesh.velocity -= Time.deltaTime * m_horiziontalResistanceBrakes * m_landingBrake * vector2;
					vector2 = m_landingMesh.velocity - Vector3.up * Vector3.Dot(m_landingMesh.velocity, Vector3.up);
					m_landingMesh.velocity -= Time.deltaTime * m_horiziontalResistanceWheels * vector2;
					if (m_landingMesh.velocity.magnitude > 4f && m_doLandingFX)
					{
						foreach (Collider groundCollisionCollider2 in m_groundCollisionColliders)
						{
						}
					}
					m_landingMesh.angularDrag = m_landingDownAngularDrag;
				}
				else
				{
					m_landingMesh.angularDrag = m_baseAngularDrag;
					m_landingMesh.velocity -= Time.deltaTime * m_horizontalResistanceScrape * vector2;
				}
				m_allWheelsOnGround = flag && flag2;
			}
			else
			{
				m_landingMesh.angularDrag = m_baseAngularDrag;
				m_allWheelsOnGround = false;
			}
		}
		else
		{
			m_allWheelsOnGround = false;
		}
		float num15 = ((!(m_doingManeuvreCountdown > 0f)) ? (1f - Mathf.Clamp01(num6)) : 1f);
		float num16 = ((!(m_doingManeuvreCountdown > 0f)) ? (1f - Mathf.Abs(num6)) : 1f);
		float num17 = m_fuelVerticalUsage.Evaluate(num6);
		float num18 = (1f - m_landingBrake) * m_liftEffectivenessCurve.Evaluate(Mathf.Clamp01((magnitude - m_minVelocityForLift) / (m_maxVelocityForLift - m_minVelocityForLift)));
		int num19 = 0;
		GameObject[] criticalTransformsAdditional = m_criticalTransformsAdditional;
		foreach (GameObject gameObject4 in criticalTransformsAdditional)
		{
			if (gameObject4 == null)
			{
				num19++;
				num13 *= 0.3f;
				num14 *= 0.3f;
				num18 *= 0.3f;
			}
		}
		Vector3 eulerCentered = GetEulerCentered();
		if (!m_simplifiedModel)
		{
			m_landingMesh.AddRelativeTorque(0f, 0f, num18 * m_yokeAngle * m_yokeAngleEffectivenessPerVelocity * Time.deltaTime + num8 * m_yokeAngle * m_yokeAngleEffectivenessPerEngineRate * Time.deltaTime * num16, ForceMode.Impulse);
			m_landingMesh.AddRelativeTorque(0f, 0f, (0f - num10) * Time.deltaTime * num16 * 0.05f + (0f - num10) * Time.deltaTime * num16 * num15 * 0.2f, ForceMode.Impulse);
			m_landingMesh.AddRelativeTorque(0f, 0f, m_frontHeavyRate * Time.deltaTime, ForceMode.Impulse);
			m_landingMesh.AddRelativeTorque(num18 * m_bank * m_bankAngleEffectivenessPerVelocity * Time.deltaTime * num16 * num4 * m_bankEffectMultiplier, 0f, 0f, ForceMode.Impulse);
			m_landingMesh.AddRelativeTorque(eulerCentered.x * num18 * 0.01f * Time.deltaTime * num3, 0f, 0f, ForceMode.Impulse);
			float num20 = Mathf.Max(Mathf.Abs(eulerCentered.x / m_bankEffectMultiplier) - m_angularDeadZone, 0f) * Mathf.Sign(eulerCentered.x);
			m_landingMesh.AddRelativeTorque(0f, (0f - num20) * num18 * m_rollToYawEffectivenessPerVelocity * Time.deltaTime * num16, 0f, ForceMode.Impulse);
			m_landingMesh.AddForce(new Vector3(0f, num18 * 50f * Time.deltaTime, 0f), ForceMode.VelocityChange);
			m_landingMesh.AddForce(new Vector3(0f, num9 * Time.deltaTime * m_accelerationVertical * m_liftTransform.forward.y * num15, 0f) * num13 * num14, ForceMode.Impulse);
			float num21 = Mathf.Abs(Mathf.Pow(num14 - num13, 4f)) * Mathf.Sign(num14 - num13);
			m_landingMesh.AddRelativeTorque(num18 * num21 * 5f * m_bankAngleEffectivenessPerVelocity * Time.deltaTime, 0f, 0f, ForceMode.Impulse);
			m_landingMesh.velocity -= Vector3.up * Vector3.Dot(m_landingMesh.velocity, Vector3.up) * num7 * m_verticalResistanceAir * Time.deltaTime * ((num13 + num14) / 2f);
			m_landingMesh.angularVelocity -= m_landingMesh.angularVelocity * m_angularResistanceAir * Time.deltaTime * Mathf.Max((num13 + num14) / 2f, 0.2f);
		}
		LandingGearSingle[] individualLandingGears = m_bomberSystems.GetIndividualLandingGears();
		int num22 = 0;
		LandingGearSingle[] array = individualLandingGears;
		foreach (LandingGearSingle landingGearSingle in array)
		{
			if (landingGearSingle != null && m_landingGearCopy[num22] != null)
			{
				m_landingGearCopy[num22].position = landingGearSingle.GetTrackingTransform().position;
			}
			num22++;
		}
		FuelTank[] fuelTanks = m_bomberSystems.GetFuelTanks();
		float inverseEfficiency = m_aeroSurfaces.GetInverseEfficiency();
		int num24 = 0;
		FuelTank[] array2 = fuelTanks;
		foreach (FuelTank fuelTank in array2)
		{
			if (fuelTank != null && fuelTank.GetFuel() > 0f)
			{
				num24++;
			}
		}
		FuelTank[] array3 = fuelTanks;
		foreach (FuelTank fuelTank2 in array3)
		{
			if (fuelTank2 != null && num24 != 0)
			{
				fuelTank2.UseFuel(num11 * num17 * inverseEfficiency / (float)num24);
			}
		}
		float setValue = Mathf.Clamp01((Mathf.Abs(num6 * 0.25f) + Mathf.Clamp(0f - num6, 0f, 1f) * 0.6f) * Mathf.Clamp01(GetVelocity().magnitude) + Mathf.Abs(GetVelocity().magnitude / 90f) * 0.5f);
		float setValue2 = Mathf.Pow(num11 * num17, 1.5f) * 0.3f + m_engineEfficiencyCurve.Evaluate(Mathf.Clamp01(value)) * 0.3f;
		WingroveRoot.Instance.SetParameterGlobal(GameEvents.Parameters.CacheVal_BomberSpeed(), setValue);
		WingroveRoot.Instance.SetParameterGlobal(GameEvents.Parameters.CacheVal_EngineSpeed(), setValue2);
		m_lastFuelUsage = num11 * num17 * inverseEfficiency;
		m_fuelUsageAverageFrameDelay--;
		if (m_fuelUsageAverageFrameDelay < 0)
		{
			m_fuelUsageAverageFrameDelay = 4;
			m_fuelUsageTotals[m_fuelUsageCurFrame] = m_lastFuelUsage;
			m_fuelUsageCurFrame = (m_fuelUsageCurFrame + 1) % 60;
		}
		if (!m_simplifiedModel)
		{
			float magnitude2 = m_landingMesh.velocity.magnitude;
			Vector3 vector3 = m_liftTransform.forward * magnitude2;
			m_landingMesh.velocity -= m_landingMesh.velocity * m_velocityRedirectionalise * Time.deltaTime;
			m_landingMesh.velocity += vector3 * m_velocityRedirectionalise * Time.deltaTime;
		}
		if ((num5 == 0f || num13 == 0f || num14 == 0f || num19 > 0) && !m_bomberSystems.GetBomberState().HasNotCompletedTakeOff() && !m_bomberSystems.GetBomberState().IsLanding() && base.transform.position.magnitude > 200f && base.transform.position.y > 100f && !m_hasDoneBadDescendTrigger)
		{
			m_hasDoneBadDescendTrigger = true;
			Singleton<MusicSelectionRules>.Instance.TriggerTimed(MusicSelectionRules.MusicTriggerEvents.DescendingBad, 30f);
		}
	}

	public float GetFuelUsage()
	{
		return m_lastFuelUsage;
	}

	public float GetFuelUsageAverage()
	{
		float num = 0f;
		for (int i = 0; i < 60; i++)
		{
			num += m_fuelUsageTotals[i];
		}
		return num / 60f;
	}

	private void DebugInfo()
	{
		GUILayout.Label(string.Concat("VEL: ", GetVelocity(), " HORZVEL: ", (m_landingMesh.velocity - Vector3.up * Vector3.Dot(m_landingMesh.velocity, Vector3.up)).magnitude));
	}
}
