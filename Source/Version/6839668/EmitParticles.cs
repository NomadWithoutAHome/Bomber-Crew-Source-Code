using UnityEngine;

public class EmitParticles : MonoBehaviour
{
	[SerializeField]
	private int m_particleCount;

	[SerializeField]
	private ParticleSystem m_particleSystem;

	public void Emit()
	{
		m_particleSystem.Emit(m_particleCount);
	}
}
