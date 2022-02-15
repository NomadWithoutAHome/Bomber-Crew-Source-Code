using UnityEngine;

public class DestroyedSectionMovement : MonoBehaviour
{
	private bool m_isActive;

	[SerializeField]
	private Transform m_movementTransform;

	[SerializeField]
	private GameObject[] m_hierarchiesToEnable;

	private Vector3 m_currentSpeed;

	private float m_currentRotationSpeed;

	private float m_countDown = 5f;

	public void SetFallingActive()
	{
		if (!m_isActive)
		{
			GameObject[] hierarchiesToEnable = m_hierarchiesToEnable;
			foreach (GameObject gameObject in hierarchiesToEnable)
			{
				gameObject.SetActive(value: true);
			}
			m_isActive = true;
			m_currentRotationSpeed = Random.Range(-80, 80);
			m_currentSpeed = new Vector3(Random.Range(-80, 80), Random.Range(0, 300), 0f);
		}
	}

	private void Update()
	{
		if (!m_isActive)
		{
		}
	}
}
