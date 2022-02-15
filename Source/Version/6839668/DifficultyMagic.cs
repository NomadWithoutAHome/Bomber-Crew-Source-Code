using System.Collections;
using System.Collections.Generic;
using BomberCrewCommon;
using UnityEngine;

public class DifficultyMagic : Singleton<DifficultyMagic>
{
	private bool m_disabled;

	private BomberSystems m_bomberSystems;

	private bool m_aceShouldGo;

	private bool m_aceIsPresent;

	private void Start()
	{
		if (!m_disabled)
		{
			FighterDifficultySettings.DifficultySetting currentDifficulty = Singleton<FighterCoordinator>.Instance.GetCurrentDifficulty();
			m_bomberSystems = Singleton<BomberSpawn>.Instance.GetBomberSystems();
			if (currentDifficulty.m_doCrewMagic)
			{
				StartCoroutine(CrewMagic());
			}
			if (currentDifficulty.m_doEngineMagic)
			{
				StartCoroutine(DoEngineMagic());
			}
			if (currentDifficulty.m_doFuselageMagic)
			{
				StartCoroutine(DoFuselageMagic());
			}
			if (currentDifficulty.m_doSystemsMagic)
			{
				StartCoroutine(DoSystemsMagicHyd());
				StartCoroutine(DoSystemsMagicElec());
			}
			if (currentDifficulty.m_doAceMagic)
			{
				StartCoroutine(DoAceMagic());
			}
		}
	}

	public void SetAcePresent()
	{
		m_aceIsPresent = true;
	}

	public bool GetAceShouldGo()
	{
		return m_aceShouldGo && !m_disabled;
	}

	private IEnumerator DoSystemsMagicHyd()
	{
		while (true)
		{
			if (!m_bomberSystems.GetHydraulics().IsBroken())
			{
				yield return null;
				continue;
			}
			if (!m_bomberSystems.GetElectricalSystem().IsBroken())
			{
				m_bomberSystems.GetElectricalSystem().SetInvincible();
			}
			yield return new WaitForSeconds(30f);
			m_bomberSystems.GetElectricalSystem().SetNoLongerInvincible();
			yield return null;
		}
	}

	private IEnumerator DoSystemsMagicElec()
	{
		while (true)
		{
			if (!m_bomberSystems.GetElectricalSystem().IsBroken())
			{
				yield return null;
				continue;
			}
			if (!m_bomberSystems.GetHydraulics().IsBroken())
			{
				m_bomberSystems.GetHydraulics().SetInvincible();
			}
			yield return new WaitForSeconds(30f);
			m_bomberSystems.GetHydraulics().SetNoLongerInvincible();
			yield return null;
		}
	}

	private IEnumerator DoFuselageMagic()
	{
		while (!m_bomberSystems.GetCriticalFlash().IsFlashing())
		{
			yield return null;
		}
		BomberFuselageSection[] bfs = m_bomberSystems.GetComponentsInChildren<BomberFuselageSection>();
		BomberFuselageSection[] array = bfs;
		foreach (BomberFuselageSection bomberFuselageSection in array)
		{
			bomberFuselageSection.SetInvincible(invincible: true);
		}
		yield return new WaitForSeconds(30f);
		BomberFuselageSection[] array2 = bfs;
		foreach (BomberFuselageSection bomberFuselageSection2 in array2)
		{
			bomberFuselageSection2.SetInvincible(invincible: false);
		}
	}

	private IEnumerator DoAceMagic()
	{
		int prevNumOut = 0;
		List<CrewSpawner.CrewmanAvatarPairing> all = Singleton<CrewSpawner>.Instance.GetAllCrew();
		while (true)
		{
			if (m_aceIsPresent)
			{
				if (m_bomberSystems.GetCriticalFlash().IsFlashing())
				{
					break;
				}
				int num = 0;
				for (int i = 0; i < m_bomberSystems.GetEngineCount(); i++)
				{
					Engine engine = m_bomberSystems.GetEngine(i);
					if (engine != null && !engine.IsBroken() && !engine.IsDestroyed() && !engine.IsOnFire())
					{
						num++;
					}
				}
				if (num == 1 && Singleton<FighterCoordinator>.Instance.GetNumTaggableCurrently() + Singleton<FighterCoordinator>.Instance.GetNumTaggedCurrently() > 2)
				{
					break;
				}
				int num2 = 0;
				int num3 = 0;
				foreach (CrewSpawner.CrewmanAvatarPairing item in all)
				{
					if (item.m_crewman.IsDead())
					{
						num2++;
					}
					else if (item.m_spawnedAvatar.GetHealthState().IsCountingDown())
					{
						num3++;
					}
				}
				if ((num2 > prevNumOut || num2 + num3 > prevNumOut + 1) && (num2 >= 1 || num2 + num3 >= 3))
				{
					break;
				}
			}
			else
			{
				int num4 = 0;
				foreach (CrewSpawner.CrewmanAvatarPairing item2 in all)
				{
					if (item2.m_crewman.IsDead())
					{
						num4++;
					}
				}
				prevNumOut = num4;
			}
			yield return null;
		}
		m_aceShouldGo = true;
	}

	private IEnumerator DoEngineMagic()
	{
		while (true)
		{
			int numEngines2 = 0;
			for (int i = 0; i < m_bomberSystems.GetEngineCount(); i++)
			{
				Engine engine = m_bomberSystems.GetEngine(i);
				if (engine != null && !engine.IsBroken() && !engine.IsDestroyed() && !engine.IsOnFire())
				{
					numEngines2++;
				}
			}
			if (numEngines2 != 1)
			{
				yield return null;
				continue;
			}
			for (int j = 0; j < m_bomberSystems.GetEngineCount(); j++)
			{
				Engine engine2 = m_bomberSystems.GetEngine(j);
				if (engine2 != null && !engine2.IsBroken() && !engine2.IsDestroyed() && !engine2.IsOnFire())
				{
					engine2.SetInvincible(invincible: true);
				}
			}
			yield return new WaitForSeconds(30f);
			for (int k = 0; k < m_bomberSystems.GetEngineCount(); k++)
			{
				Engine engine3 = m_bomberSystems.GetEngine(k);
				if (engine3 != null && !engine3.IsBroken() && !engine3.IsDestroyed() && !engine3.IsOnFire())
				{
					engine3.SetInvincible(invincible: false);
				}
			}
			yield return new WaitForSeconds(240f);
			while (true)
			{
				int numEngines = 0;
				for (int l = 0; l < m_bomberSystems.GetEngineCount(); l++)
				{
					Engine engine4 = m_bomberSystems.GetEngine(l);
					if (engine4 != null && !engine4.IsBroken() && !engine4.IsDestroyed() && !engine4.IsOnFire())
					{
						numEngines++;
					}
				}
				if (numEngines > 2)
				{
					break;
				}
				yield return null;
			}
			yield return null;
		}
	}

	private IEnumerator CrewMagic()
	{
		while (m_bomberSystems.GetBomberState().GetAltitudeAboveGround() < 50f)
		{
			yield return null;
		}
		int prevNumOut = 0;
		float invincibilityTimer = 0f;
		float resetTimer = 0f;
		List<CrewSpawner.CrewmanAvatarPairing> all = Singleton<CrewSpawner>.Instance.GetAllCrew();
		while (m_bomberSystems.GetBomberState().GetAltitudeAboveGround() > 30f)
		{
			int numDead = 0;
			int numBleeding = 0;
			foreach (CrewSpawner.CrewmanAvatarPairing item in all)
			{
				if (item.m_crewman.IsDead())
				{
					numDead++;
				}
				else if (item.m_spawnedAvatar.GetHealthState().IsCountingDown())
				{
					numBleeding++;
				}
			}
			if (numDead + numBleeding >= 2 && numDead + numBleeding > prevNumOut)
			{
				prevNumOut = numDead + numBleeding;
				invincibilityTimer = 30f;
				resetTimer = 120f;
				foreach (CrewSpawner.CrewmanAvatarPairing item2 in all)
				{
					item2.m_spawnedAvatar.SetDamageMultiplier(0.1f);
				}
			}
			if (invincibilityTimer > 0f)
			{
				invincibilityTimer -= Time.deltaTime;
				if (invincibilityTimer <= 0f)
				{
					foreach (CrewSpawner.CrewmanAvatarPairing item3 in all)
					{
						item3.m_spawnedAvatar.SetDamageMultiplier(1f);
					}
				}
			}
			if (resetTimer > 0f)
			{
				resetTimer -= Time.deltaTime;
				if (resetTimer <= 0f)
				{
					prevNumOut = numDead + numBleeding;
				}
			}
			yield return null;
		}
		foreach (CrewSpawner.CrewmanAvatarPairing item4 in all)
		{
			item4.m_spawnedAvatar.SetInvincible(invincible: false);
		}
	}

	public void SetDisabled()
	{
		m_disabled = true;
		StopAllCoroutines();
	}
}
