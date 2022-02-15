using System;
using UnityEngine;

namespace BomberCrewCommon;

public abstract class FileSystemBase : MonoBehaviour
{
	private Action m_saveStart;

	private Action m_saveFinish;

	private Action m_saveReset;

	public virtual bool IsSupportedOnPlatform()
	{
		return false;
	}

	public abstract bool Exists(string filename);

	public abstract byte[] ReadAllBytes(string filename);

	public abstract void WriteAllBytes(string filename, byte[] bytes);

	public abstract string ReadAllText(string filename);

	public abstract void WriteAllText(string filename, string contents);

	public virtual bool IsReady()
	{
		return true;
	}

	public void SetHooks(Action saveStart, Action saveFinish, Action saveReset)
	{
		m_saveStart = saveStart;
		m_saveFinish = saveFinish;
		m_saveReset = saveReset;
	}

	protected void SaveReset()
	{
		if (m_saveReset != null)
		{
			m_saveReset();
		}
	}

	protected void SaveStart()
	{
		if (m_saveStart != null)
		{
			m_saveStart();
		}
	}

	protected void SaveFinish()
	{
		if (m_saveFinish != null)
		{
			m_saveFinish();
		}
	}

	public virtual void SetUp()
	{
	}
}
