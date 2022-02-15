using UnityEngine;

public class DamageEffectsSpawner : MonoBehaviour
{
	[SerializeField]
	private ParticleSystemHelper[] m_particleSystemHelpers;

	public void SpawnEffects(Vector3 pos)
	{
		ParticleSystemHelper[] particleSystemHelpers = m_particleSystemHelpers;
		foreach (ParticleSystemHelper particleSystemHelper in particleSystemHelpers)
		{
			particleSystemHelper.EmitParticlesAtPos(pos);
		}
	}
}
