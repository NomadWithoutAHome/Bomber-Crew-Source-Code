using UnityEngine;

public class BomberMovingPartRotation : MonoBehaviour
{
	[SerializeField]
	private BomberDestroyableSection m_destroyableSeciton;

	[SerializeField]
	private BomberFlightPhysicsModel m_physicsModel;

	[SerializeField]
	private float m_maxRotation;

	[SerializeField]
	private float m_degreesPerBank;

	[SerializeField]
	private float m_degreesPerPitch;

	[SerializeField]
	private float m_xMultiplier = 1f;

	[SerializeField]
	private float m_yMultiplier;

	private bool m_enabled = true;

	private float m_currentRotation;

	private Quaternion m_startRotation;

	private void Start()
	{
		m_enabled = true;
		m_destroyableSeciton.OnSectionDestroy += DestroyThis;
		m_startRotation = base.transform.localRotation;
	}

	private void DestroyThis()
	{
		m_enabled = false;
	}

	private void Update()
	{
		if (m_enabled)
		{
			float bankingInput = m_physicsModel.GetBankingInput();
			float pitchInput = m_physicsModel.GetPitchInput();
			float num = Mathf.Clamp(m_degreesPerBank * bankingInput + m_degreesPerPitch * pitchInput, 0f - m_maxRotation, m_maxRotation);
			float num2 = num - m_currentRotation;
			m_currentRotation += num2 * Mathf.Clamp01(Time.deltaTime * 4f);
			m_currentRotation = Mathf.Clamp(m_currentRotation, 0f - m_maxRotation, m_maxRotation);
			base.transform.localRotation = m_startRotation * Quaternion.Euler(m_currentRotation * m_xMultiplier, m_currentRotation * m_yMultiplier, 0f);
		}
	}
}
