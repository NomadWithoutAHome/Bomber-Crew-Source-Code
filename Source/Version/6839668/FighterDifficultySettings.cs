using System;
using BomberCrewCommon;
using UnityEngine;

[CreateAssetMenu(menuName = "Bomber Crew/Fighter Difficulty Settings")]
public class FighterDifficultySettings : ScriptableObject
{
	[Serializable]
	public class DifficultySetting
	{
		[SerializeField]
		public int m_intelRequired;

		[SerializeField]
		public float m_damageMultiplier;

		[SerializeField]
		public float m_firePhaseMultiplier;

		[SerializeField]
		public float m_healthMultiplier;

		[SerializeField]
		public bool m_doEngineMagic;

		[SerializeField]
		public bool m_doFuselageMagic;

		[SerializeField]
		public bool m_doSystemsMagic;

		[SerializeField]
		public bool m_doCrewMagic;

		[SerializeField]
		public int m_gangUpMaximum;

		[SerializeField]
		public bool m_doAceMagic;
	}

	[SerializeField]
	private DifficultySetting[] m_allDifficulties;

	[SerializeField]
	private DifficultySetting m_demoDifficulty;

	public DifficultySetting GetCurrentDifficultySetting()
	{
		int intel = Singleton<SaveDataContainer>.Instance.Get().GetIntel();
		int num = 0;
		int num2 = 0;
		DifficultySetting[] allDifficulties = m_allDifficulties;
		foreach (DifficultySetting difficultySetting in allDifficulties)
		{
			if (intel >= difficultySetting.m_intelRequired)
			{
				num = num2;
			}
			num2++;
		}
		int difficultyBump = Singleton<SaveDataContainer>.Instance.Get().GetDifficultyBump();
		return m_allDifficulties[Mathf.Clamp(num + difficultyBump, 0, m_allDifficulties.Length - 1)];
	}
}
