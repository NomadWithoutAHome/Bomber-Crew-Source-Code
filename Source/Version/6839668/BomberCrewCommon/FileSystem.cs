using System;
using UnityEngine;

namespace BomberCrewCommon;

public class FileSystem : MonoBehaviour
{
	[SerializeField]
	private FileSystemBase[] m_fileSystems;

	private FileSystemBase m_thisFileSystem;

	private static FileSystem s_staticInstance;

	public static event Action OnSaveStart;

	public static event Action OnSaveFinish;

	public static event Action OnSaveReset;

	private void Awake()
	{
		s_staticInstance = this;
		FileSystemBase[] fileSystems = m_fileSystems;
		foreach (FileSystemBase fileSystemBase in fileSystems)
		{
			if (fileSystemBase.IsSupportedOnPlatform())
			{
				m_thisFileSystem = fileSystemBase;
			}
		}
		m_thisFileSystem.SetHooks(SaveStart, SaveFinish, SaveReset);
		m_thisFileSystem.SetUp();
	}

	private void SaveStart()
	{
		if (FileSystem.OnSaveStart != null)
		{
			FileSystem.OnSaveStart();
		}
	}

	private void SaveReset()
	{
		if (FileSystem.OnSaveReset != null)
		{
			FileSystem.OnSaveReset();
		}
	}

	private void SaveFinish()
	{
		if (FileSystem.OnSaveFinish != null)
		{
			FileSystem.OnSaveFinish();
		}
	}

	public static bool Exists(string filename)
	{
		return s_staticInstance.m_thisFileSystem.Exists(filename);
	}

	public static byte[] ReadAllBytes(string filename)
	{
		return s_staticInstance.m_thisFileSystem.ReadAllBytes(filename);
	}

	public static void WriteAllBytes(string filename, byte[] bytes)
	{
		s_staticInstance.m_thisFileSystem.WriteAllBytes(filename, bytes);
	}

	public static string ReadAllText(string filename)
	{
		return s_staticInstance.m_thisFileSystem.ReadAllText(filename);
	}

	public static void WriteAllText(string filename, string contents)
	{
		s_staticInstance.m_thisFileSystem.WriteAllText(filename, contents);
	}

	public static bool IsReady()
	{
		return s_staticInstance.m_thisFileSystem.IsReady();
	}
}
