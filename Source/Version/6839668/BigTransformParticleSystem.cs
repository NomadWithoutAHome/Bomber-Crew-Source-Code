using BomberCrewCommon;
using UnityEngine;

public class BigTransformParticleSystem : MonoBehaviour
{
	[SerializeField]
	private ParticleSystem[] m_particleSystems;

	[SerializeField]
	private bool m_inverted;

	private void OnEnable()
	{
		ParticleSystem[] particleSystems = m_particleSystems;
		foreach (ParticleSystem pr in particleSystems)
		{
			if (m_inverted)
			{
				Singleton<BigTransformCoordinator>.Instance.RegisterParticleSystemI(pr);
			}
			else
			{
				Singleton<BigTransformCoordinator>.Instance.RegisterParticleSystem(pr);
			}
		}
	}

	private void OnDisable()
	{
		ParticleSystem[] particleSystems = m_particleSystems;
		foreach (ParticleSystem pr in particleSystems)
		{
			if (m_inverted)
			{
				Singleton<BigTransformCoordinator>.Instance.DeRegisterParticleSystemI(pr);
			}
			else
			{
				Singleton<BigTransformCoordinator>.Instance.DeRegisterParticleSystem(pr);
			}
		}
	}
}
