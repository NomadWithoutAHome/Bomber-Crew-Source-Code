using System;
using System.Collections;
using System.Collections.Generic;
using BomberCrewCommon;
using UnityEngine;

public class CustomLiveryTextures : Singleton<CustomLiveryTextures>, LoadableSystem
{
	[Serializable]
	public class CustomLiveryClass
	{
		[SerializeField]
		public int m_x;

		[SerializeField]
		public int m_y;

		[SerializeField]
		public int m_numLiveriesOfType;

		[SerializeField]
		public string m_filenameFormat;

		[SerializeField]
		public string m_customRef;
	}

	[SerializeField]
	private CustomLiveryClass[] m_liveryClasses;

	private Dictionary<string, List<Texture2D>> m_loadedCustomLiveries = new Dictionary<string, List<Texture2D>>();

	private bool m_isLoaded;

	private void Awake()
	{
		Singleton<SystemLoader>.Instance.RegisterLoadableSystem(this);
	}

	private void ReloadNewSave()
	{
		m_isLoaded = false;
		StopAllCoroutines();
		StartCoroutine(Refresh());
	}

	public IEnumerator Refresh()
	{
		foreach (CustomLiveryTextures.CustomLiveryClass clc in this.m_liveryClasses)
		{
			int numPix = clc.m_x * clc.m_y;
			Color[] c = new Color[numPix];
			for (int k = 0; k < numPix; k++)
			{
				c[k] = new Color(0f, 0f, 0f, 0f);
			}
			yield return null;
			this.m_loadedCustomLiveries[clc.m_customRef] = new List<Texture2D>();
			for (int i = 0; i < clc.m_numLiveriesOfType; i++)
			{
				Texture2D t = new Texture2D(clc.m_x, clc.m_y);
				string filename = string.Format(clc.m_filenameFormat, i);
				if (FileSystem.Exists(filename))
				{
					try
					{
						byte[] data = FileSystem.ReadAllBytes(filename);
						t.LoadImage(data);
						if (t.width > 512 || t.height > 512)
						{
							t = new Texture2D(clc.m_x, clc.m_y);
						}
					}
					catch
					{
						t = new Texture2D(clc.m_x, clc.m_y);
						t.SetPixels(c);
					}
				}
				else
				{
					t.SetPixels(c);
				}
				this.m_loadedCustomLiveries[clc.m_customRef].Add(t);
				yield return null;
			}
		}
		this.m_isLoaded = true;
		yield break;
	}

	public Texture2D GetCustomLivery(string customRef, int index)
	{
		return m_loadedCustomLiveries[customRef][index];
	}

	public Vector2 GetSize(string customRef)
	{
		Vector2 result = new Vector2(64f, 64f);
		CustomLiveryClass[] liveryClasses = m_liveryClasses;
		foreach (CustomLiveryClass customLiveryClass in liveryClasses)
		{
			if (customLiveryClass.m_customRef == customRef)
			{
				result.x = customLiveryClass.m_x;
				result.y = customLiveryClass.m_y;
			}
		}
		return result;
	}

	public void SetCustomLivery(Texture2D t, string customRef, int index)
	{
		string format = string.Empty;
		CustomLiveryClass[] liveryClasses = m_liveryClasses;
		foreach (CustomLiveryClass customLiveryClass in liveryClasses)
		{
			if (customLiveryClass.m_customRef == customRef)
			{
				format = customLiveryClass.m_filenameFormat;
			}
		}
		m_loadedCustomLiveries[customRef][index] = t;
		byte[] bytes = t.EncodeToPNG();
		string filename = string.Format(format, index);
		FileSystem.WriteAllBytes(filename, bytes);
	}

	public bool IsLoadComplete()
	{
		return m_isLoaded;
	}

	public void StartLoad()
	{
		StopAllCoroutines();
		StartCoroutine(Refresh());
	}

	public void ContinueLoad()
	{
	}

	public string GetName()
	{
		return "CustomLiveryTextures";
	}

	public LoadableSystem[] GetDependencies()
	{
		return null;
	}
}
