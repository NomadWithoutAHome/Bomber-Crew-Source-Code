using System;
using System.Collections;
using System.Collections.Generic;
using BomberCrewCommon;
using dbox;
using UnityEngine;

public class BombLoad : MonoBehaviour
{
	public enum BombRackState
	{
		Unarmed,
		Armed,
		Dropped
	}

	[Serializable]
	private class BombRack
	{
		[SerializeField]
		private Bomb[] m_bombs;

		[SerializeField]
		private bool m_useOrderingImportant;

		[SerializeField]
		private int m_reliesOnIndex;

		public Bomb[] GetBombs()
		{
			return m_bombs;
		}

		public int GetBombCount()
		{
			return m_bombs.Length;
		}

		public bool UseOrdering()
		{
			return m_useOrderingImportant;
		}

		public int GetOrderingIndex()
		{
			return m_reliesOnIndex;
		}
	}

	[SerializeField]
	[NamedText]
	private string m_namedTextReference;

	[SerializeField]
	private BombRack[] m_bombRacks;

	[SerializeField]
	private bool m_removeBombBayDoors;

	[SerializeField]
	private bool m_doesntRequireBombBayDoors;

	[SerializeField]
	private bool m_isGrandSlam;

	private bool m_dropped;

	private List<Bomb>[] m_bombs;

	private List<BombRackState> m_bombRackState;

	private bool m_isDropping;

	private void Awake()
	{
		m_bombs = new List<Bomb>[m_bombRacks.Length];
		m_bombRackState = new List<BombRackState>();
		for (int i = 0; i < m_bombRacks.Length; i++)
		{
			m_bombRackState.Add(BombRackState.Unarmed);
			Bomb[] bombs = m_bombRacks[i].GetBombs();
			m_bombs[i] = new List<Bomb>();
			for (int j = 0; j < m_bombRacks[i].GetBombCount(); j++)
			{
				m_bombs[i].Add(bombs[j]);
			}
		}
	}

	public void DropBombs()
	{
		if (!HasDropped())
		{
			StartCoroutine(BombReleaseSequence());
		}
	}

	public bool IsGrandSlam()
	{
		return m_isGrandSlam;
	}

	public bool ShouldRemoveBayDoors()
	{
		return m_removeBombBayDoors;
	}

	public bool IgnoresBombBayDoors()
	{
		return m_doesntRequireBombBayDoors;
	}

	public List<BombRackState> GetBombRackStates()
	{
		return m_bombRackState;
	}

	private void VerifyState()
	{
		int num = 0;
		BombRack[] bombRacks = m_bombRacks;
		foreach (BombRack bombRack in bombRacks)
		{
			if (bombRack.UseOrdering())
			{
				int orderingIndex = bombRack.GetOrderingIndex();
				if (m_bombRackState[num] == BombRackState.Armed && m_bombRackState[orderingIndex] == BombRackState.Unarmed)
				{
					m_bombRackState[num] = BombRackState.Unarmed;
					m_bombRackState[orderingIndex] = BombRackState.Armed;
				}
			}
			num++;
		}
	}

	private IEnumerator BombReleaseSequence()
	{
		while (m_isDropping)
		{
			yield return null;
		}
		if (HasDropped())
		{
			yield break;
		}
		VerifyState();
		m_isDropping = true;
		BombRackState[] cachedState = new BombRackState[m_bombRackState.Count];
		for (int j = 0; j < cachedState.Length; j++)
		{
			cachedState[j] = m_bombRackState[j];
		}
		for (int i = 0; i < cachedState.Length; i++)
		{
			if (cachedState[i] != BombRackState.Armed)
			{
				continue;
			}
			for (int bc = 0; bc < m_bombRacks[i].GetBombCount(); bc++)
			{
				if (m_bombs[i][bc] != null)
				{
					yield return new WaitForSeconds(0.2f);
					if (m_bombs[i][bc] != null && !m_bombs[i][bc].IsReleased())
					{
						DboxInMissionController.DBoxCall(DboxSdkWrapper.PostReleaseBombs);
						m_bombs[i][bc].Release();
					}
				}
			}
			m_bombRackState[i] = BombRackState.Dropped;
		}
		m_isDropping = false;
		if (HasDropped())
		{
			yield return new WaitForSeconds(15f);
			if (HasDropped())
			{
				Singleton<ObjectiveManager>.Instance.FailObjectivesOfType(ObjectiveManager.ObjectiveType.BombTarget);
				Singleton<ObjectiveManager>.Instance.FailObjectivesOfType(ObjectiveManager.ObjectiveType.NonBombTarget);
				Singleton<MissionCoordinator>.Instance.FireTrigger("ALL_BOMBS_DROPPED");
			}
		}
	}

	public void ForceGone()
	{
		for (int i = 0; i < m_bombRackState.Count; i++)
		{
			if (m_bombRackState[i] == BombRackState.Armed)
			{
				for (int j = 0; j < m_bombs[i].Count; j++)
				{
					m_bombs[i][j].transform.parent = null;
					m_bombs[i][j].Release();
					UnityEngine.Object.Destroy(m_bombs[i][j]);
					m_bombRackState[i] = BombRackState.Dropped;
				}
			}
		}
	}

	public void ToggleArm(int index)
	{
		if (m_bombRackState[index] == BombRackState.Unarmed)
		{
			m_bombRackState[index] = BombRackState.Armed;
		}
		else if (m_bombRackState[index] == BombRackState.Armed)
		{
			m_bombRackState[index] = BombRackState.Unarmed;
		}
	}

	public void ArmEnoughFor(int damage)
	{
		float num = 0f;
		for (int i = 0; i < m_bombRackState.Count; i++)
		{
			if (m_bombRackState[i] == BombRackState.Armed)
			{
				for (int j = 0; j < m_bombs[i].Count; j++)
				{
					float damageAmt = m_bombs[i][j].GetDamageAmt();
					num += damageAmt;
				}
			}
		}
		if (num >= (float)damage)
		{
			return;
		}
		for (int k = 0; k < m_bombRackState.Count; k++)
		{
			if (m_bombRackState[k] == BombRackState.Unarmed)
			{
				float num2 = 0f;
				for (int l = 0; l < m_bombs[k].Count; l++)
				{
					float damageAmt2 = m_bombs[k][l].GetDamageAmt();
					num2 += damageAmt2;
				}
				m_bombRackState[k] = BombRackState.Armed;
				num += num2;
				if (num >= (float)damage)
				{
					break;
				}
			}
		}
	}

	public int GetNumBombRacks()
	{
		return m_bombRackState.Count;
	}

	public int GetNumBombsRacksArmed()
	{
		int num = 0;
		for (int i = 0; i < m_bombRacks.Length; i++)
		{
			if (m_bombRackState[i] == BombRackState.Armed)
			{
				num++;
			}
		}
		return num;
	}

	public bool HasDroppedAny()
	{
		bool result = false;
		for (int i = 0; i < m_bombRackState.Count; i++)
		{
			if (m_bombRackState[i] == BombRackState.Dropped)
			{
				result = true;
			}
		}
		return result;
	}

	public bool HasDropped()
	{
		bool result = true;
		for (int i = 0; i < m_bombRackState.Count; i++)
		{
			if (m_bombRackState[i] != BombRackState.Dropped)
			{
				result = false;
			}
		}
		return result;
	}
}
