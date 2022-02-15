using System.Collections.Generic;
using BomberCrewCommon;
using PigeonCoopToolkit.Effects.Trails;
using UnityEngine;

public class BigTransformCoordinator : Singleton<BigTransformCoordinator>
{
	[SerializeField]
	private int m_pageSize = 10000;

	[SerializeField]
	private MonoBehaviour m_postprocessToDisableOnWarp;

	private List<BigTransform> m_allTransforms = new List<BigTransform>(256);

	private List<BigTransform> m_allNonStaticTransforms = new List<BigTransform>(256);

	private List<ParticleSystem> m_particleSystems = new List<ParticleSystem>(64);

	private List<ParticleSystem> m_particleSystemsInverted = new List<ParticleSystem>(64);

	private List<Trail> m_allTrails = new List<Trail>(32);

	private BigTransform m_primaryTransform;

	private Vector3d m_currentOffset;

	private int m_currentPageX;

	private int m_currentPageY;

	private int m_currentPageZ;

	private bool m_cameraWarpedThisFrame;

	private bool m_postprocessPreviousValue;

	private bool m_hasChangedPostProcess;

	public Vector3d RegisterBigTransform(BigTransform bt)
	{
		m_allTransforms.Add(bt);
		if (!bt.IsStatic())
		{
			m_allNonStaticTransforms.Add(bt);
		}
		if (bt.IsPrimary())
		{
			m_primaryTransform = bt;
		}
		return m_currentOffset;
	}

	public void DeRegisterBigTransform(BigTransform bt)
	{
		m_allTransforms.Remove(bt);
		m_allNonStaticTransforms.Remove(bt);
	}

	public void RegisterParticleSystem(ParticleSystem pr)
	{
		m_particleSystems.Add(pr);
	}

	public void DeRegisterParticleSystem(ParticleSystem pr)
	{
		m_particleSystems.Remove(pr);
	}

	public void RegisterTrail(Trail trail)
	{
		m_allTrails.Add(trail);
	}

	public void DeRegisterTrail(Trail trail)
	{
		m_allTrails.Remove(trail);
	}

	public void RegisterParticleSystemI(ParticleSystem pr)
	{
		m_particleSystemsInverted.Add(pr);
	}

	public void DeRegisterParticleSystemI(ParticleSystem pr)
	{
		m_particleSystemsInverted.Remove(pr);
	}

	public void UpdateAll(Vector3d newOffset, Vector3d change)
	{
		foreach (BigTransform allTransform in m_allTransforms)
		{
			allTransform.InstantForcedUpdate(newOffset);
		}
		Vector3 vector = (Vector3)change;
		foreach (ParticleSystem particleSystem in m_particleSystems)
		{
			int particleCount = particleSystem.particleCount;
			ParticleSystem.Particle[] array = new ParticleSystem.Particle[particleCount];
			particleSystem.GetParticles(array);
			for (int i = 0; i < particleCount; i++)
			{
				array[i].position -= vector;
			}
			particleSystem.SetParticles(array, particleCount);
		}
		foreach (ParticleSystem item in m_particleSystemsInverted)
		{
			int particleCount2 = item.particleCount;
			ParticleSystem.Particle[] array2 = new ParticleSystem.Particle[particleCount2];
			item.GetParticles(array2);
			for (int j = 0; j < particleCount2; j++)
			{
				array2[j].position += vector;
			}
			item.SetParticles(array2, particleCount2);
		}
		foreach (Trail allTrail in m_allTrails)
		{
			allTrail.MoveBy(vector);
		}
		m_currentOffset = newOffset;
	}

	private void ChangeOffset(int x, int z)
	{
		m_currentPageX = x;
		m_currentPageZ = z;
		Vector3d vector3d = new Vector3d(-x * m_pageSize, 0f, -z * m_pageSize);
		Vector3d change = vector3d - m_currentOffset;
		UpdateAll(vector3d, change);
	}

	public Vector3d GetCurrentOffset()
	{
		return m_currentOffset;
	}

	public void SetCameraWarp()
	{
		m_cameraWarpedThisFrame = true;
	}

	private void Update()
	{
		bool flag = false;
		if (m_primaryTransform != null)
		{
			List<BigTransform>.Enumerator enumerator = m_allNonStaticTransforms.GetEnumerator();
			while (enumerator.MoveNext())
			{
				BigTransform current = enumerator.Current;
				if (current.IsEnabled())
				{
					Vector3 position = current.transform.position;
					if (position.x != current.m_lastPos.x || position.y != current.m_lastPos.y || position.z != current.m_lastPos.z)
					{
						current.m_lastPos = position;
						current.SoftSetPosition(new Vector3d(position) - m_currentOffset);
					}
				}
			}
			Vector3d position2 = m_primaryTransform.position;
			int num = Mathd.RoundToInt(position2.x / (double)m_pageSize);
			int num2 = Mathd.RoundToInt(position2.z / (double)m_pageSize);
			if (num != m_currentPageX || num2 != m_currentPageZ)
			{
				flag = true;
				ChangeOffset(num, num2);
			}
		}
		if (flag || m_cameraWarpedThisFrame)
		{
			m_postprocessPreviousValue = m_postprocessToDisableOnWarp.enabled;
			m_postprocessToDisableOnWarp.enabled = false;
			m_hasChangedPostProcess = true;
		}
		else if (m_hasChangedPostProcess)
		{
			m_postprocessToDisableOnWarp.enabled = m_postprocessPreviousValue;
			m_hasChangedPostProcess = false;
		}
		m_cameraWarpedThisFrame = false;
	}
}
