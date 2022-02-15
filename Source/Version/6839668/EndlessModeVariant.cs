using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using BomberCrewCommon;
using Newtonsoft.Json;
using UnityEngine;

[CreateAssetMenu(menuName = "Bomber Crew/Endless Mode Variant")]
public class EndlessModeVariant : ScriptableObject
{
	[Serializable]
	public class Loadout
	{
		[SerializeField]
		public BomberDefaults m_bomber;

		[SerializeField]
		[NamedText]
		public string m_title;

		[SerializeField]
		public CrewmanEquipmentPreset[] m_crewmanEquipmentPresets;

		[SerializeField]
		private Texture2D m_texture;

		public Texture2D GetTexture()
		{
			return m_texture;
		}
	}

	[Serializable]
	[JsonObject(MemberSerialization.OptIn)]
	public class LoadoutJS
	{
		[SerializeField]
		[JsonProperty]
		public BomberUpgradeConfig m_bomber;

		[SerializeField]
		[JsonProperty]
		public string m_title;

		[SerializeField]
		[JsonProperty]
		public List<List<string>> m_crewmanEquipmentPresetDictionary;

		private Texture2D m_cachedTexture;

		public LoadoutJS()
		{
		}

		public LoadoutJS(BomberUpgradeConfig buc, Crewman[] crewmanEquipmentPreset, string bomberName)
		{
			m_bomber = new BomberUpgradeConfig(buc);
			m_crewmanEquipmentPresetDictionary = new List<List<string>>();
			foreach (Crewman crewman in crewmanEquipmentPreset)
			{
				List<string> list = new List<string>();
				foreach (CrewmanEquipmentBase item in crewman.GetAllEquipment())
				{
					list.Add(item.name);
				}
				m_crewmanEquipmentPresetDictionary.Add(list);
			}
			m_title = bomberName;
		}

		public LoadoutJS(BomberUpgradeConfig buc, Crewman[] crewmanEquipmentPreset, string bomberName, Texture2D photo, string mode, int index)
		{
			byte[] bytes = photo.EncodeToPNG();
			FileSystem.WriteAllBytes(GetFilename(mode, index), bytes);
			m_bomber = new BomberUpgradeConfig(buc);
			m_crewmanEquipmentPresetDictionary = new List<List<string>>();
			foreach (Crewman crewman in crewmanEquipmentPreset)
			{
				List<string> list = new List<string>();
				foreach (CrewmanEquipmentBase item in crewman.GetAllEquipment())
				{
					list.Add(item.name);
				}
				m_crewmanEquipmentPresetDictionary.Add(list);
			}
			m_title = bomberName;
		}

		private string GetFilename(string mode, int index)
		{
			return "BC_C_" + mode + index + ".png";
		}

		public string GetUniqueString()
		{
			string s = JsonConvert.SerializeObject(this).ToLower();
			MD5 mD = MD5.Create();
			byte[] bytes = Encoding.ASCII.GetBytes(s);
			byte[] buffer = Cryptography.Encrypt(bytes, "flfiflfr");
			byte[] inArray = mD.ComputeHash(buffer);
			return Convert.ToBase64String(inArray);
		}

		public Texture2D GetTexture(string mode, int index)
		{
			if (m_cachedTexture == null)
			{
				m_cachedTexture = new Texture2D(1, 1);
				string filename = GetFilename(mode, index);
				if (FileSystem.Exists(filename))
				{
					try
					{
						byte[] data = FileSystem.ReadAllBytes(filename);
						m_cachedTexture.LoadImage(data);
					}
					catch
					{
						m_cachedTexture.SetPixel(0, 0, Color.white);
					}
				}
			}
			return m_cachedTexture;
		}
	}

	[SerializeField]
	private CampaignStructure.CampaignMission m_campaignMission;

	[SerializeField]
	private bool m_allowDLCItems;

	[SerializeField]
	private string m_description;

	[SerializeField]
	private string m_title;

	[SerializeField]
	[NamedText]
	private string m_briefingText;

	[SerializeField]
	private Texture2D m_briefingBoardHeaderImage;

	[SerializeField]
	private Loadout[] m_presets;

	[SerializeField]
	private FighterDifficultySettings.DifficultySetting m_difficultySettings;

	[SerializeField]
	private HiScoreIdentifier m_leaderboardIds;

	public FighterDifficultySettings.DifficultySetting GetDifficulty()
	{
		return m_difficultySettings;
	}

	public bool AllowDLCItems()
	{
		return m_allowDLCItems;
	}

	public HiScoreIdentifier GetLeaderboardIds()
	{
		return m_leaderboardIds;
	}

	public int GetNumberOfPresets()
	{
		return m_presets.Length;
	}

	public Loadout GetPreset(int index)
	{
		return m_presets[index];
	}

	public CampaignStructure.CampaignMission GetCampaignMission()
	{
		return m_campaignMission;
	}

	public string GetNamedTextTitle()
	{
		return m_title;
	}

	public string GetNamedTextBriefing()
	{
		return m_briefingText;
	}

	public Texture2D GetBriefingBoardHeaderImage()
	{
		return m_briefingBoardHeaderImage;
	}
}
