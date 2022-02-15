using BomberCrewCommon;
using UnityEngine;

public class tk2dLanguageText : MonoBehaviour
{
	[SerializeField]
	[NamedText]
	private string m_key;

	[SerializeField]
	private tk2dTextMesh m_textMesh;

	[SerializeField]
	private bool m_forceUpperCase;

	private void Awake()
	{
		if (m_textMesh == null)
		{
			m_textMesh = GetComponent<tk2dTextMesh>();
		}
		Singleton<LanguageProvider>.Instance.GetNamedText(m_key, GotText);
	}

	private void GotText(string result)
	{
		if (!(this != null) || !(m_textMesh != null))
		{
			return;
		}
		if (m_forceUpperCase)
		{
			if (result == null)
			{
				m_textMesh.SetText(string.Empty);
			}
			else
			{
				m_textMesh.SetText(result.ToUpper());
			}
		}
		else
		{
			m_textMesh.SetText(result);
		}
	}
}
