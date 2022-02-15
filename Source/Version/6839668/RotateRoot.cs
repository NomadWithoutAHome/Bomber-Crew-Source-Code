using UnityEngine;

public class RotateRoot : MonoBehaviour
{
	public float m_rotationAmount = 1f;

	private Vector3 m_rotationVector = Vector3.zero;

	private void Start()
	{
		m_rotationVector = new Vector3(0f, 0f, m_rotationAmount);
	}

	private void Update()
	{
		base.transform.Rotate(m_rotationVector);
		foreach (Transform item in base.transform)
		{
			item.Rotate(-m_rotationVector);
		}
	}
}
