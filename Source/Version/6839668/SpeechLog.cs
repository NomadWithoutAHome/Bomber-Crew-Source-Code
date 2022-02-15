using System.Collections.Generic;
using UnityEngine;

public class SpeechLog : MonoBehaviour
{
	public class RecentTexts
	{
		public string m_text;

		public float m_timeAlive;
	}

	[SerializeField]
	private tk2dTextMesh[] m_textMesh;

	[SerializeField]
	private float m_maxDisplayLen;

	private List<RecentTexts> m_recentTexts = new List<RecentTexts>();

	private void Update()
	{
		List<RecentTexts> list = new List<RecentTexts>();
		foreach (RecentTexts recentText in m_recentTexts)
		{
			recentText.m_timeAlive += Time.deltaTime;
			if (recentText.m_timeAlive > m_maxDisplayLen)
			{
				list.Add(recentText);
			}
		}
		foreach (RecentTexts item in list)
		{
			m_recentTexts.Remove(item);
		}
		while (m_recentTexts.Count > 4)
		{
			m_recentTexts.RemoveAt(0);
		}
		string[] array = new string[4];
		int num = 0;
		for (int num2 = m_recentTexts.Count - 1; num2 >= 0; num2--)
		{
			array[num] = m_recentTexts[num2].m_text;
			num++;
		}
		int num3 = 0;
		tk2dTextMesh[] textMesh = m_textMesh;
		foreach (tk2dTextMesh tk2dTextMesh2 in textMesh)
		{
			tk2dTextMesh2.text = ((array[num3] != null) ? array[num3] : string.Empty);
			num3++;
		}
	}

	public void Log(string text, Crewman crewman)
	{
		string text2 = crewman.GetSurname() + ": " + text;
		RecentTexts recentTexts = new RecentTexts();
		recentTexts.m_timeAlive = 0f;
		recentTexts.m_text = text2;
		m_recentTexts.Add(recentTexts);
	}
}
