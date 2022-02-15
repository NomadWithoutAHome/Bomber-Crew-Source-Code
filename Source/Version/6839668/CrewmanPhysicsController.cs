using System.Collections.Generic;
using BomberCrewCommon;
using UnityEngine;

public class CrewmanPhysicsController : MonoBehaviour
{
	[SerializeField]
	private float m_castRayFromAbove;

	[SerializeField]
	private float m_castRayFromAboveWalk;

	[SerializeField]
	private LayerMask m_groundCollisionLayers;

	[SerializeField]
	private float m_gravityForce;

	[SerializeField]
	private float m_walkRadius;

	[SerializeField]
	private float m_slipRate;

	[SerializeField]
	private float m_slipNormNormalDoubleCheck;

	[SerializeField]
	private float m_slipNormNormal;

	[SerializeField]
	private float m_slipNormBraced;

	[SerializeField]
	private CrewmanAvatar m_crewmanAvatar;

	[SerializeField]
	private float m_shiftRadius = 0.5f;

	[SerializeField]
	private float m_maxShiftSpeed = 0.1f;

	private float m_currentGravityVelocity;

	private Vector3 m_groundNormal;

	private bool m_isGrounded;

	private bool m_isLocked;

	private bool m_canBrace;

	private bool m_didBrace;

	private bool m_didSlip;

	private bool m_didShift;

	private bool m_canShift;

	private float m_slipCtr;

	public void CanBrace(bool brace)
	{
		m_canBrace = brace;
	}

	public bool IsBracing()
	{
		return m_didBrace;
	}

	public bool DidSlip()
	{
		return m_didSlip;
	}

	public void SetCanShift()
	{
		m_canShift = true;
	}

	public void TryToCentralise(BomberWalkZone bwz, Vector3 up, bool andRotate)
	{
		Vector3 nearestPlanarPointLocalFromWorld = bwz.GetNearestPlanarPointLocalFromWorld(base.transform.position);
		nearestPlanarPointLocalFromWorld.z = 0f;
		Vector3 nearestPlanarPointLocalFastSafe = bwz.GetNearestPlanarPointLocalFastSafe(nearestPlanarPointLocalFromWorld);
		if (andRotate)
		{
			Quaternion rotation = base.transform.rotation;
			rotation = ((!(Vector3.Dot(base.transform.forward, bwz.transform.right) > 0f)) ? GetGoodRotation(-bwz.transform.right, up, rotation) : GetGoodRotation(bwz.transform.right, up, rotation));
			base.transform.SetPositionAndRotation(Vector3.Lerp(base.transform.position, bwz.GetPointFromLocal(nearestPlanarPointLocalFastSafe), 0.5f * Time.deltaTime), Quaternion.RotateTowards(base.transform.rotation, rotation, 10f * Time.deltaTime));
		}
		else
		{
			base.transform.position = Vector3.Lerp(base.transform.position, bwz.GetPointFromLocal(nearestPlanarPointLocalFastSafe), 0.5f * Time.deltaTime);
		}
	}

	private Quaternion GetGoodRotation(Vector3 wd, Vector3 up, Quaternion def)
	{
		Vector3 normalized = wd.normalized;
		Vector3 vector = normalized - Vector3.Dot(up, normalized) * up;
		if (vector.magnitude > 0f)
		{
			return Quaternion.LookRotation(vector.normalized, up);
		}
		return def;
	}

	public void SetRotationFrom(Vector3 wd, Vector3 up)
	{
		Vector3 normalized = wd.normalized;
		Vector3 vector = normalized - Vector3.Dot(up, normalized) * up;
		if (vector.magnitude > 0f)
		{
			base.transform.rotation = Quaternion.LookRotation(vector.normalized, up);
		}
	}

	public void DoUpdate(Vector3 bomberUp, BomberWalkZone bwz)
	{
		m_didBrace = false;
		m_didSlip = false;
		m_didShift = false;
		if (!m_isLocked)
		{
			if (bwz == null)
			{
				m_isGrounded = false;
				m_didSlip = true;
				return;
			}
			SetRotationFrom(base.transform.forward, bomberUp);
			Vector3 origin = base.transform.position + base.transform.up * m_castRayFromAbove;
			if (Physics.Raycast(origin, -base.transform.up, out var hitInfo, 100f, m_groundCollisionLayers))
			{
				float num = hitInfo.distance - m_castRayFromAbove;
				if (num > 0f)
				{
					if (num < 0.1f)
					{
						m_currentGravityVelocity = 0f;
						base.transform.position += num * -base.transform.up;
						m_groundNormal = hitInfo.normal;
						m_isGrounded = true;
					}
					else
					{
						m_currentGravityVelocity += m_gravityForce * Time.deltaTime;
						float num2 = Vector3.Dot(base.transform.up, Vector3.up) * num;
						base.transform.position += Mathf.Max(num2, m_currentGravityVelocity * Time.deltaTime) * -Vector3.up;
						if (m_currentGravityVelocity * Time.deltaTime >= num2)
						{
							m_groundNormal = hitInfo.normal;
							m_isGrounded = true;
						}
					}
				}
				else
				{
					m_currentGravityVelocity = 0f;
					base.transform.position += (0f - num) * base.transform.up;
					m_groundNormal = hitInfo.normal;
					m_isGrounded = true;
				}
				if (m_isGrounded)
				{
					float num3 = Vector3.Dot(bomberUp, Vector3.up);
					bool flag = false;
					if (num3 < m_slipNormNormal)
					{
						if ((m_canBrace && num3 > m_slipNormBraced) || !bwz.IsExternal())
						{
							m_slipCtr -= Time.deltaTime * 2f;
							flag = true;
						}
						else
						{
							m_slipCtr += Time.deltaTime * 2f;
						}
					}
					else
					{
						m_slipCtr -= Time.deltaTime * 2f;
					}
					m_slipCtr = Mathf.Clamp01(m_slipCtr);
					if (m_slipCtr > 0.5f && num3 < m_slipNormNormalDoubleCheck && bwz.IsExternal())
					{
						Vector3 normalized = (m_groundNormal - Vector3.Dot(m_groundNormal, Vector3.up) * Vector3.up).normalized;
						DoMove(normalized * m_slipRate * Time.deltaTime, isAWalk: true, isDirectionWalk: false, bomberUp);
						m_didSlip = true;
						m_didBrace = false;
					}
					else
					{
						m_didBrace = flag;
						if (flag)
						{
							TryToCentralise(bwz, bomberUp, andRotate: false);
						}
					}
				}
			}
			else
			{
				m_currentGravityVelocity += m_gravityForce * Time.deltaTime;
				base.transform.position += m_currentGravityVelocity * Time.deltaTime * -Vector3.up;
				m_isGrounded = false;
				m_didSlip = true;
			}
			if (m_isGrounded && !m_didSlip && !m_didBrace && m_canShift)
			{
				List<CrewSpawner.CrewmanAvatarPairing> allCrew = Singleton<CrewSpawner>.Instance.GetAllCrew();
				CrewSpawner.CrewmanAvatarPairing crewmanAvatarPairing = null;
				float num4 = m_shiftRadius;
				Vector3 vector = Vector3.zero;
				foreach (CrewSpawner.CrewmanAvatarPairing item in allCrew)
				{
					if (item.m_spawnedAvatar != m_crewmanAvatar)
					{
						Vector3 vector2 = item.m_spawnedAvatar.transform.position - base.transform.position;
						float magnitude = vector2.magnitude;
						if (magnitude < num4)
						{
							crewmanAvatarPairing = item;
							num4 = magnitude;
							vector = vector2;
						}
					}
				}
				if (crewmanAvatarPairing != null)
				{
					float num5 = m_shiftRadius * 1.1f - num4;
					Vector3 normalized2 = vector.normalized;
					Vector3 pn = base.transform.position + -normalized2 * num5;
					Vector3 nearestPlanarPoint = bwz.GetNearestPlanarPoint(pn);
					Vector3 vector3 = nearestPlanarPoint - base.transform.position;
					DoMove((vector3 - Vector3.Dot(bomberUp, vector3) * bomberUp).normalized * Mathf.Min(num5, m_maxShiftSpeed * Time.deltaTime), isAWalk: true, isDirectionWalk: false, bomberUp);
					m_didShift = true;
				}
			}
		}
		m_canShift = false;
	}

	public bool DidShift()
	{
		return m_didShift;
	}

	public void SetLocked(bool locked)
	{
		m_isLocked = locked;
		if (locked)
		{
			m_currentGravityVelocity = 0f;
		}
	}

	public bool IsGrounded()
	{
		return m_isGrounded || m_isLocked;
	}

	public bool IsLocked()
	{
		return m_isLocked;
	}

	public Vector3 GetGroundNormal()
	{
		return m_groundNormal;
	}

	public void Teleport(Vector3 newPos, bool groundSnap)
	{
		base.transform.position = newPos;
		if (groundSnap)
		{
			GroundSnap();
		}
	}

	private void GroundSnap()
	{
		if (m_isLocked)
		{
			return;
		}
		Vector3 origin = base.transform.position + base.transform.up * m_castRayFromAbove;
		if (!Physics.Raycast(origin, -base.transform.up, out var hitInfo, 100f, m_groundCollisionLayers))
		{
			return;
		}
		float num = hitInfo.distance - m_castRayFromAbove;
		if (num > 0f)
		{
			if (num < 0.1f)
			{
				m_currentGravityVelocity = 0f;
				base.transform.position += num * -base.transform.up;
				m_groundNormal = hitInfo.normal;
				m_isGrounded = true;
			}
		}
		else
		{
			m_currentGravityVelocity = 0f;
			base.transform.position += (0f - num) * base.transform.up;
			m_groundNormal = hitInfo.normal;
			m_isGrounded = true;
		}
	}

	public void DoMove(Vector3 moveDelta, bool isAWalk, bool isDirectionWalk, Vector3 bomberUp)
	{
		if (m_isLocked || (isAWalk && !m_isGrounded))
		{
			return;
		}
		if (isDirectionWalk)
		{
			SetRotationFrom(moveDelta, bomberUp);
		}
		Vector3 origin = base.transform.position + base.transform.up * m_castRayFromAboveWalk;
		if (!Physics.Raycast(origin, moveDelta.normalized, out var _, moveDelta.magnitude + m_walkRadius, m_groundCollisionLayers))
		{
			base.transform.position += moveDelta;
			if (isAWalk)
			{
				GroundSnap();
			}
		}
	}
}
