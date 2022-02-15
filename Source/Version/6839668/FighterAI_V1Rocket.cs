using BomberCrewCommon;
using UnityEngine;

public class FighterAI_V1Rocket : FighterAI
{
	[SerializeField]
	private float m_targetAltitude;

	private Vector3d m_offsets;

	private bool m_offsetSet;

	protected override bool CanAttack()
	{
		return true;
	}

	protected override void StopAttack()
	{
	}

	public override void UpdateWithState(FighterState fs)
	{
		if (!m_offsetSet)
		{
			m_offsets += new Vector3d(Random.Range(-200, 200), 0f, Random.Range(-200, 200));
			m_offsetSet = true;
		}
		Vector3d vector3d = new Vector3d(4800f, m_targetAltitude, 1100f) + m_offsets;
		Vector3d vector3d2 = vector3d - base.gameObject.btransform().position;
		m_controls.SetHeading((Vector3)vector3d2.normalized, 1f, 0f);
		if (vector3d2.magnitude < 200.0)
		{
			Singleton<FighterCoordinator>.Instance.SetV1HitLondon();
			m_fighterPlane.SetDestroyed(forceExplode: false, forceDontExplode: true, byPlayer: false);
		}
	}
}
