using BomberCrewCommon;
using UnityEngine;

public abstract class TextSetter : MonoBehaviour
{
	public abstract void SetText(string text);

	public virtual void SetColor(Color c)
	{
	}

	public void SetTextFromLanguageString(string languageKey)
	{
		SetText(Singleton<LanguageProvider>.Instance.GetNamedTextImmediate(languageKey));
	}
}
