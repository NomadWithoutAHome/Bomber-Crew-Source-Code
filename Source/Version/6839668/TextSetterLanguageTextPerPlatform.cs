using BomberCrewCommon;
using UnityEngine;

public class TextSetterLanguageTextPerPlatform : MonoBehaviour
{
	[SerializeField]
	[NamedText]
	private string m_keySwitch;

	[SerializeField]
	[NamedText]
	private string m_keyXboxOne;

	[SerializeField]
	[NamedText]
	private string m_keyPS4;

	[SerializeField]
	[NamedText]
	private string m_keyPC;

	[SerializeField]
	private TextSetter m_textMesh;

	private void Awake()
	{
		if (m_textMesh == null)
		{
			m_textMesh = GetComponent<tk2dTextMesh>();
		}
		Singleton<LanguageProvider>.Instance.GetNamedText(m_keyPC, GotText);
	}

	private void GotText(string result)
	{
		if (this != null && m_textMesh != null)
		{
			m_textMesh.SetText(result);
		}
	}
}
