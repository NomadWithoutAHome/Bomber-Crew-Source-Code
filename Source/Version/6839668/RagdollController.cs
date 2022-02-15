using UnityEngine;

public class RagdollController : MonoBehaviour
{
	public class StoredRigidBodyInfo
	{
		public Transform m_transform;

		public Vector3 m_storedPosition;

		public Quaternion m_storedRotation;
	}

	[SerializeField]
	private float m_gravity = 9.81f;

	[SerializeField]
	private bool m_enabled = true;

	[SerializeField]
	private float m_blendTimeTotal = 0.5f;

	private Vector3 m_forceVector;

	private Rigidbody[] m_rigidbodies;

	private Collider[] m_ragdollColliders;

	[SerializeField]
	private Animator m_animator;

	[SerializeField]
	private Transform m_pelvisTransform;

	[SerializeField]
	private Transform m_feetTransform;

	private StoredRigidBodyInfo[] m_rbi;

	private float m_blendTime;

	private Transform m_root;

	private void Awake()
	{
		m_forceVector = new Vector3(0f, 0f - m_gravity, 0f);
		GetPhysicsComponents();
		SetEnabled(enabled: false, null);
	}

	private void FixedUpdate()
	{
		if (m_enabled)
		{
			Rigidbody[] rigidbodies = m_rigidbodies;
			foreach (Rigidbody rigidbody in rigidbodies)
			{
				rigidbody.AddForce(m_forceVector, ForceMode.Acceleration);
			}
		}
	}

	private void LateUpdate()
	{
		if (m_enabled || !(m_blendTime > 0f))
		{
			return;
		}
		float t = Mathf.Clamp01(m_blendTime / m_blendTimeTotal);
		for (int i = 0; i < m_rigidbodies.Length; i++)
		{
			StoredRigidBodyInfo storedRigidBodyInfo = m_rbi[i];
			Transform transform = storedRigidBodyInfo.m_transform;
			if (transform != base.transform)
			{
				if (transform == m_pelvisTransform)
				{
					transform.localPosition = Vector3.Lerp(transform.localPosition, storedRigidBodyInfo.m_storedPosition, t);
				}
				transform.rotation = Quaternion.Slerp(transform.rotation, base.transform.rotation * storedRigidBodyInfo.m_storedRotation, t);
			}
		}
		m_blendTime -= Time.deltaTime;
	}

	private void GetPhysicsComponents()
	{
		m_rigidbodies = m_pelvisTransform.GetComponentsInChildren<Rigidbody>(includeInactive: true);
		m_ragdollColliders = m_pelvisTransform.GetComponentsInChildren<Collider>(includeInactive: true);
		m_rbi = new StoredRigidBodyInfo[m_rigidbodies.Length];
		for (int i = 0; i < m_rigidbodies.Length; i++)
		{
			m_rbi[i] = new StoredRigidBodyInfo();
			m_rbi[i].m_transform = m_rigidbodies[i].transform;
		}
	}

	public void SetEnabled(bool enabled, Transform root)
	{
		m_animator.enabled = !enabled;
		Collider[] ragdollColliders = m_ragdollColliders;
		foreach (Collider collider in ragdollColliders)
		{
			collider.enabled = enabled;
		}
		Rigidbody[] rigidbodies = m_rigidbodies;
		foreach (Rigidbody rigidbody in rigidbodies)
		{
			rigidbody.isKinematic = !enabled;
			rigidbody.velocity = Vector3.zero;
		}
		if (root != null)
		{
			m_root = root;
		}
		if (m_enabled != enabled)
		{
			if (enabled)
			{
				m_blendTime = 0f;
			}
			else
			{
				for (int k = 0; k < m_rigidbodies.Length; k++)
				{
					m_rbi[k].m_storedPosition = m_rbi[k].m_transform.localPosition;
					m_rbi[k].m_storedRotation = Quaternion.Inverse(base.transform.rotation) * m_rbi[k].m_transform.rotation;
				}
				m_blendTime = m_blendTimeTotal;
			}
		}
		m_enabled = enabled;
	}
}
