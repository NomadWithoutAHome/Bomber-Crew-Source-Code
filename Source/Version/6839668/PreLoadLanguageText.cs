using System;
using UnityEngine;

public class PreLoadLanguageText : MonoBehaviour
{
	[Serializable]
	private class PreLoadText
	{
		[SerializeField]
		public string m_languageRef;

		[SerializeField]
		public string m_text;

		[SerializeField]
		public tk2dFontData m_fontData;
	}

	[SerializeField]
	private PreLoadText[] m_preloadText;

	[SerializeField]
	private LanguageSelectionTable m_selectionTable;

	[SerializeField]
	private tk2dTextMesh m_textMesh;

	private void Start()
	{
		string localeString = null;
		string text = m_selectionTable.GetLanguageFor(Application.systemLanguage, localeString);
		if (string.IsNullOrEmpty(text))
		{
			text = "default";
		}
		PreLoadText[] preloadText = m_preloadText;
		foreach (PreLoadText preLoadText in preloadText)
		{
			if (preLoadText.m_languageRef == text)
			{
				m_textMesh.text = preLoadText.m_text;
				if (preLoadText.m_fontData != null)
				{
					m_textMesh.font = preLoadText.m_fontData;
				}
			}
		}
	}
}
