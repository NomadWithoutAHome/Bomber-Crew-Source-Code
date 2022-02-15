using BomberCrewCommon;
using UnityEngine;
using WingroveAudio;

public class BomberMissileLauncher : MonoBehaviour
{
	[SerializeField]
	private BomberSystemUniqueId m_upgradeSystem;

	[SerializeField]
	private Transform[] m_missilePositions;

	[SerializeField]
	private LayerMask m_collisionLayerMask;

	[SerializeField]
	private BomberDestroyableSection[] m_requiredSections;

	[SerializeField]
	private GameObject[] m_missileHierarchies;

	private MissilesUpgrade m_upgrade;

	private int m_numMissiles;

	private int m_currentMissileIndex;

	private bool m_showMissileInfo;

	private float m_rechargeTime;

	private void Start()
	{
		m_upgrade = (MissilesUpgrade)m_upgradeSystem.GetUpgrade();
		if (m_upgrade != null)
		{
			m_showMissileInfo = true;
			m_numMissiles = m_upgrade.GetNumMissiles();
			m_rechargeTime = m_upgrade.GetRechargeTime();
		}
		for (int i = 0; i < m_missileHierarchies.Length; i++)
		{
			m_missileHierarchies[i].SetActive(i < m_numMissiles);
		}
	}

	public float GetRechargeTime()
	{
		return m_rechargeTime;
	}

	public int GetNumMissilesRemaining()
	{
		BomberDestroyableSection[] requiredSections = m_requiredSections;
		foreach (BomberDestroyableSection bomberDestroyableSection in requiredSections)
		{
			if (bomberDestroyableSection.IsDestroyed())
			{
				return 0;
			}
		}
		return m_numMissiles;
	}

	public bool ShouldShowMissileLauncherSettings()
	{
		return m_showMissileInfo;
	}

	public bool FireMissile(StationGunner fromGunner, Vector3 inheritedVelocity, FighterPlane lastTarget)
	{
		bool result = false;
		if (GetNumMissilesRemaining() > 0 && !Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBomberState().HasNotCompletedTakeOff())
		{
			m_numMissiles--;
			Transform transform = m_missilePositions[m_currentMissileIndex];
			GameObject gameObject = Singleton<CommonEffectManager>.Instance.ProjectileEffect(m_upgrade.GetProjectileType(), transform.position, transform.position + transform.forward * 2000f, 1f, m_collisionLayerMask, inheritedVelocity, fromGunner.GetCurrentCrewman(), isDouble: false, isQuad: false, isFake: false);
			WingroveRoot.Instance.PostEventGO(m_upgrade.GetProjectileType().GetFireAudioHook(), base.gameObject);
			gameObject.GetComponent<ProjectileEffectRocketHomingPlayer>().SetTarget(lastTarget);
			m_missileHierarchies[m_currentMissileIndex].SetActive(value: false);
			m_currentMissileIndex++;
			result = true;
		}
		return result;
	}
}
