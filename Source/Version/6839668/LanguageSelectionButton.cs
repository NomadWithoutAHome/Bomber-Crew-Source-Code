using UnityEngine;

public class LanguageSelectionButton : MonoBehaviour
{
	[SerializeField]
	private TextSetter m_languageText;

	public void SetLanguageText(string text, int languageToken)
	{
		tk2dTextMesh tk2dTextMesh2 = (tk2dTextMesh)m_languageText;
		if (tk2dTextMesh2 != null)
		{
			tk2dTextMesh2.SetOverrideLangFlag(languageToken);
			tk2dTextMesh2.UpdateFontType();
		}
		m_languageText.SetText(text);
	}
}
