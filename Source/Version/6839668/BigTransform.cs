using BomberCrewCommon;
using UnityEngine;

public class BigTransform : MonoBehaviour
{
	private Vector3d m_position;

	[SerializeField]
	private bool m_isPrimary;

	[SerializeField]
	private bool m_takeOnAwake;

	[SerializeField]
	private bool m_isStatic;

	private static BigTransform s_primaryTransform;

	private Vector3d m_currentOffset;

	public Vector3 m_lastPos;

	private bool m_enabledFlag;

	public Vector3d position
	{
		get
		{
			return m_position;
		}
		set
		{
			m_position = value;
			InstantForcedUpdate();
		}
	}

	private void Awake()
	{
		if (m_takeOnAwake || m_isStatic)
		{
			m_position = new Vector3d(base.transform.position);
		}
		Singleton<BigTransformCoordinator>.Instance.RegisterBigTransform(this);
		InstantForcedUpdate();
	}

	private void OnEnable()
	{
		m_enabledFlag = true;
	}

	private void OnDisable()
	{
		m_enabledFlag = false;
	}

	public bool IsEnabled()
	{
		return m_enabledFlag;
	}

	private void OnDestroy()
	{
		if (Singleton<BigTransformCoordinator>.Instance != null)
		{
			Singleton<BigTransformCoordinator>.Instance.DeRegisterBigTransform(this);
		}
	}

	public bool IsStatic()
	{
		return m_isStatic;
	}

	public void MarkAsStatic()
	{
		m_isStatic = true;
		Singleton<BigTransformCoordinator>.Instance.DeRegisterBigTransform(this);
		Singleton<BigTransformCoordinator>.Instance.RegisterBigTransform(this);
	}

	public bool IsPrimary()
	{
		return m_isPrimary;
	}

	public void SetFromCurrentPage(Vector3 f)
	{
		m_currentOffset = Singleton<BigTransformCoordinator>.Instance.GetCurrentOffset();
		m_position = -m_currentOffset + new Vector3d(f);
		InstantForcedUpdate(m_currentOffset);
	}

	public void SoftSetFromCurrentPage(Vector3 f)
	{
		base.transform.position = f;
		m_currentOffset = Singleton<BigTransformCoordinator>.Instance.GetCurrentOffset();
		m_position = new Vector3d(f) - m_currentOffset;
	}

	private void InstantForcedUpdate()
	{
		InstantForcedUpdate(Singleton<BigTransformCoordinator>.Instance.GetCurrentOffset());
	}

	public void InstantForcedUpdate(Vector3d offset)
	{
		m_currentOffset = offset;
		Vector3 lastPos = (Vector3)(m_position + m_currentOffset);
		base.transform.position = lastPos;
		m_lastPos = lastPos;
	}

	public void SoftSetPosition(Vector3d sp)
	{
		m_position = sp;
	}
}
