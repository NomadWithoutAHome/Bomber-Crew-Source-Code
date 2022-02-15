using BomberCrewCommon;
using UnityEngine;

public class TextSetterLanguageText : MonoBehaviour
{
	[SerializeField]
	[NamedText]
	private string m_key;

	[SerializeField]
	private TextSetter m_textMesh;

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
		if (this != null && m_textMesh != null)
		{
			m_textMesh.SetText(result);
		}
	}
}
