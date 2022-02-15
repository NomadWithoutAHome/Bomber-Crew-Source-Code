using System;
using BomberCrewCommon;
using UnityEngine;

public class GlobalEffects : Singleton<GlobalEffects>
{
	[Serializable]
	private class MinMaxInt
	{
		[SerializeField]
		private int m_min;

		[SerializeField]
		private int m_max;

		public int Min
		{
			get
			{
				return m_min;
			}
			set
			{
				m_min = value;
			}
		}

		public int Max
		{
			get
			{
				return m_max;
			}
			set
			{
				m_max = value;
			}
		}
	}

	[SerializeField]
	private ParticleSystem m_impactEffectParticlesGround;

	private ParticleSystem.EmitParams m_emitParamsGround;

	[SerializeField]
	private MinMaxInt m_minMaxParticlesGround;

	[SerializeField]
	private ParticleSystem m_impactEffectParticlesSea;

	private ParticleSystem.EmitParams m_emitParamsSea;

	[SerializeField]
	private MinMaxInt m_minMaxParticlesSea;

	[SerializeField]
	private ParticleSystem m_impactEffectParticlesWoodland;

	private ParticleSystem.EmitParams m_emitParamsWoodland;

	[SerializeField]
	private MinMaxInt m_minMaxParticlesWoodland;

	private void Awake()
	{
		ParticleSystem.EmissionModule emission = m_impactEffectParticlesGround.emission;
		emission.rateOverTime = new ParticleSystem.MinMaxCurve(0f);
		ParticleSystem.EmissionModule emission2 = m_impactEffectParticlesSea.emission;
		emission2.rateOverTime = new ParticleSystem.MinMaxCurve(0f);
		ParticleSystem.EmissionModule emission3 = m_impactEffectParticlesWoodland.emission;
		emission3.rateOverTime = new ParticleSystem.MinMaxCurve(0f);
	}

	public void SpawnImpactEffects(GroundCollisionType.GroundCollisionEffectType effectType, Vector3 position)
	{
		switch (effectType)
		{
		case GroundCollisionType.GroundCollisionEffectType.Default:
			SpawnImpactEffectsGround(position);
			break;
		case GroundCollisionType.GroundCollisionEffectType.Woodland:
			SpawnImpactEffectsWoodland(position);
			break;
		}
	}

	public void SpawnImpactEffectsGround(Vector3 position)
	{
		m_emitParamsGround.position = position;
		m_impactEffectParticlesGround.Emit(m_emitParamsGround, UnityEngine.Random.Range(m_minMaxParticlesGround.Min, m_minMaxParticlesGround.Max));
	}

	public void SpawnImpactEffectsGroundLow(Vector3 position)
	{
		m_emitParamsGround.position = position;
		m_impactEffectParticlesGround.Emit(m_emitParamsGround, m_minMaxParticlesGround.Min);
	}

	public void SpawnImpactEffectsSea(Vector3 position)
	{
		m_emitParamsSea.position = position;
		m_impactEffectParticlesSea.Emit(m_emitParamsSea, UnityEngine.Random.Range(m_minMaxParticlesSea.Min, m_minMaxParticlesSea.Max));
	}

	public void SpawnImpactEffectsWoodland(Vector3 position)
	{
		m_emitParamsWoodland.position = position;
		m_impactEffectParticlesWoodland.Emit(m_emitParamsWoodland, UnityEngine.Random.Range(m_minMaxParticlesWoodland.Min, m_minMaxParticlesWoodland.Max));
	}
}
