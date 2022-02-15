using System;
using System.Collections.Generic;
using BomberCrewCommon;
using UnityEngine;

public class MED_Terrain : LevelItem
{
	[SerializeField]
	private int m_chunkSize = 4096;

	[SerializeField]
	[TextArea]
	private string m_chunkMap;

	[SerializeField]
	private GameObject[] m_previewObjects;

	public override Dictionary<string, string> GetParameters(Func<LevelItem, string> resolverFunc)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary["chunkSize"] = m_chunkSize.ToString();
		dictionary["chunkMap"] = m_chunkMap;
		return dictionary;
	}

	public override string GetName()
	{
		return "FlakVolume";
	}

	public override bool HasSize()
	{
		return true;
	}

	public void UpdatePreview()
	{
		List<GameObject> list = new List<GameObject>();
		foreach (Transform item in base.transform)
		{
			list.Add(item.gameObject);
		}
		foreach (GameObject item2 in list)
		{
			UnityEngine.Object.DestroyImmediate(item2);
		}
		string[] array = m_chunkMap.Split('\n');
		int num = 0;
		string[] array2 = array;
		foreach (string text in array2)
		{
			int num2 = 0;
			string text2 = text;
			foreach (char c in text2)
			{
				int num3 = c - 48;
				if (num3 >= 0 && num3 < m_previewObjects.Length)
				{
					GameObject gameObject = UnityEngine.Object.Instantiate(m_previewObjects[num3]);
					gameObject.transform.parent = base.transform;
					Vector3 vector = new Vector3(num2 * m_chunkSize, 0f, num * m_chunkSize);
					float num4 = 1f / Singleton<SceneSetUp>.Instance.GetExportSettings().m_worldScaleUp;
					gameObject.transform.localPosition = vector * num4;
					gameObject.transform.localScale = new Vector3((float)m_chunkSize * num4, 0.01f, (float)m_chunkSize * num4);
				}
				num2++;
			}
			num++;
		}
	}

	public void OnDrawGizmos()
	{
		string[] array = m_chunkMap.Split('\n');
		int num = array.Length;
		int num2 = 0;
		string[] array2 = array;
		foreach (string text in array2)
		{
			int num3 = 0;
			string text2 = text;
			foreach (char c in text2)
			{
				if (c != '\n')
				{
					num2 = Mathf.Max(num2, num3);
				}
				num3++;
			}
		}
		float num4 = 1f / Singleton<SceneSetUp>.Instance.GetExportSettings().m_worldScaleUp;
		float num5 = (float)m_chunkSize * num4;
		Gizmos.DrawWireCube(base.transform.position + new Vector3(((float)num2 * 0.5f - 0.5f) * num5, 0f, ((float)num * 0.5f - 0.5f) * num5), new Vector3((float)num2 * num5, 0.01f, (float)num * num5));
	}

	public void OnDrawGizmosSelected()
	{
		string[] array = m_chunkMap.Split('\n');
		int num = array.Length;
		int num2 = 0;
		string[] array2 = array;
		foreach (string text in array2)
		{
			int num3 = 0;
			string text2 = text;
			foreach (char c in text2)
			{
				if (c != '\n')
				{
					num2 = Mathf.Max(num2, num3);
				}
				num3++;
			}
		}
		float num4 = 1f / Singleton<SceneSetUp>.Instance.GetExportSettings().m_worldScaleUp;
		float num5 = (float)m_chunkSize * num4;
		Gizmos.color = Color.green;
		Gizmos.DrawWireCube(base.transform.position + new Vector3(((float)num2 * 0.5f - 0.5f) * num5, 0f, ((float)num * 0.5f - 0.5f) * num5), new Vector3((float)num2 * num5, 0.01f, (float)num * num5));
	}
}
