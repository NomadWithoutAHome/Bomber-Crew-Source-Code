using System;
using System.Collections.Generic;
using BomberCrewCommon;
using Newtonsoft.Json;
using UnityEngine;

public class SaveDataContainer : Singleton<SaveDataContainer>
{
	[SerializeField]
	private int m_maxSlots;

	[SerializeField]
	private Texture2D m_defaultCrewPhoto;

	private SaveData m_saveData;

	private int m_currentSlotIndex;

	private Dictionary<string, Texture2D> m_cachedSaveSlotPictures = new Dictionary<string, Texture2D>();

	private void Awake()
	{
	}

	private void DoSaveDebug()
	{
		if (m_saveData != null)
		{
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("+£100"))
			{
				m_saveData.AddBalance(100);
			}
			if (GUILayout.Button("-£100"))
			{
				m_saveData.AddBalance(-100);
			}
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("+£1000"))
			{
				m_saveData.AddBalance(1000);
			}
			if (GUILayout.Button("-£1000"))
			{
				m_saveData.AddBalance(-1000);
			}
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("+£10000"))
			{
				m_saveData.AddBalance(10000);
			}
			if (GUILayout.Button("-£10000"))
			{
				m_saveData.AddBalance(-10000);
			}
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("+I 100"))
			{
				m_saveData.AddIntel(100);
			}
			GUILayout.EndHorizontal();
		}
	}

	public void New(int slotIndex, bool isCampaign, bool skippedTutorial)
	{
		Singleton<BomberContainer>.Instance.GetLivery().Reset();
		m_currentSlotIndex = slotIndex;
		m_saveData = new SaveData();
		m_saveData.SetUp();
		if (isCampaign)
		{
			m_saveData.SetIsCampaign(skippedTutorial);
		}
	}

	public bool Exists(int slotIndex)
	{
		string path = GetPath(slotIndex);
		return FileSystem.Exists(path);
	}

	private void Update()
	{
		if (m_saveData != null)
		{
			m_saveData.AddTime(Time.deltaTime);
		}
	}

	public bool Exists()
	{
		for (int i = 0; i < m_maxSlots; i++)
		{
			if (Exists(i))
			{
				return true;
			}
		}
		return false;
	}

	public int GetMaxSlots()
	{
		return m_maxSlots;
	}

	public SaveData TempLoadSlotBackup(int slotIndex)
	{
		SaveData result = null;
		string filename = GetPath(slotIndex) + ".backup";
		try
		{
			string text = FileSystem.ReadAllText(filename);
			if (text != null)
			{
				result = JsonConvert.DeserializeObject<SaveData>(text);
				return result;
			}
			DebugLogWrapper.LogError("LOADING BACKUP RETURNED NULL!");
			return result;
		}
		catch (Exception ex)
		{
			if (ex.InnerException != null)
			{
				DebugLogWrapper.LogError(ex.InnerException.ToString());
			}
			DebugLogWrapper.LogError("LOADING BACKUP FAILED!!! " + ex.ToString());
			return result;
		}
	}

	public SaveData TempLoadSlot(int slotIndex)
	{
		SaveData saveData = null;
		string path = GetPath(slotIndex);
		try
		{
			string text = FileSystem.ReadAllText(path);
			if (text != null)
			{
				saveData = JsonConvert.DeserializeObject<SaveData>(text);
			}
			else
			{
				DebugLogWrapper.LogError("LOADING RETURNED NULL!");
			}
		}
		catch (Exception ex)
		{
			if (ex.InnerException != null)
			{
				DebugLogWrapper.LogError(ex.InnerException.ToString());
			}
			DebugLogWrapper.LogError("LOADING FAILED!!! " + ex.ToString());
		}
		if (saveData == null)
		{
			saveData = TempLoadSlotBackup(slotIndex);
		}
		return saveData;
	}

	private string GetPath(int slotIndex)
	{
		return Singleton<GameFlow>.Instance.GetGameMode().GetSaveDataPrefix() + slotIndex + ".dat";
	}

	private string GetPath()
	{
		return Singleton<GameFlow>.Instance.GetGameMode().GetSaveDataPrefix() + m_currentSlotIndex + ".dat";
	}

	private string GetPhotoFilename(int slotIndex)
	{
		return Singleton<GameFlow>.Instance.GetGameMode().GetSaveDataPrefix() + "Photo_" + slotIndex + ".png";
	}

	public void SaveCrewPhoto(Texture2D photo)
	{
		byte[] bytes = photo.EncodeToPNG();
		string photoFilename = GetPhotoFilename(m_currentSlotIndex);
		FileSystem.WriteAllBytes(photoFilename, bytes);
		m_cachedSaveSlotPictures[photoFilename] = photo;
	}

	public Texture2D LoadCrewPhoto(int saveSlot)
	{
		string photoFilename = GetPhotoFilename(saveSlot);
		Texture2D value = m_defaultCrewPhoto;
		if (!m_cachedSaveSlotPictures.TryGetValue(photoFilename, out value))
		{
			if (FileSystem.Exists(photoFilename))
			{
				try
				{
					byte[] data = FileSystem.ReadAllBytes(photoFilename);
					Texture2D texture2D = new Texture2D(4, 4);
					texture2D.LoadImage(data);
					value = texture2D;
					m_cachedSaveSlotPictures[photoFilename] = value;
					return value;
				}
				catch
				{
					return m_defaultCrewPhoto;
				}
			}
			value = m_defaultCrewPhoto;
		}
		return value;
	}

	public int GetCurrentSlot()
	{
		return m_currentSlotIndex;
	}

	public bool Load(int slotIndex)
	{
		Singleton<BomberContainer>.Instance.GetLivery().Reset();
		m_currentSlotIndex = slotIndex;
		m_saveData = TempLoadSlot(slotIndex);
		return m_saveData != null;
	}

	public void Save()
	{
		m_saveData.SetDLC(Singleton<DLCManager>.Instance.GetCurrentInstalledDLCs());
		string path = GetPath();
		try
		{
			string contents = JsonConvert.SerializeObject(m_saveData, Formatting.Indented);
			FileSystem.WriteAllText(path, contents);
		}
		catch (Exception)
		{
		}
	}

	public SaveData Get()
	{
		return m_saveData;
	}
}
