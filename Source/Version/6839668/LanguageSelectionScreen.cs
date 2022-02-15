using System.Collections.Generic;
using BomberCrewCommon;
using UnityEngine;

public class LanguageSelectionScreen : MonoBehaviour
{
	[SerializeField]
	private GameObject m_languageSelectionPrefab;

	[SerializeField]
	private TextSetter m_confirmButtonText;

	[SerializeField]
	private LayoutGrid m_confirmButtonLayoutGrid;

	[SerializeField]
	private tk2dUIItem m_confirmButtonButton;

	[SerializeField]
	private LayoutGrid m_layoutGrid;

	[SerializeField]
	private UISelectFinder m_finder;

	private Dictionary<SystemDataContainer.SupportedLanguage, GameObject> m_languageButtons = new Dictionary<SystemDataContainer.SupportedLanguage, GameObject>();

	private string m_currentlySelectedLanguage;

	private void OnEnable()
	{
		Singleton<UISelector>.Instance.SetFinder(m_finder);
	}

	private void OnDisable()
	{
	}

	private void ShowLangDebug()
	{
		if (GUILayout.Button("LANG: Load Longest Language"))
		{
			Singleton<LanguageLoader>.Instance.LoadLongestLanguage();
			Singleton<UIScreenManager>.Instance.ShowScreen("StartScreen", showNavBarButtons: true);
		}
	}

	private void Start()
	{
		m_confirmButtonButton.OnClick += Confirm;
		m_currentlySelectedLanguage = Singleton<SystemDataContainer>.Instance.GetCurrentLanguage();
		SystemDataContainer.SupportedLanguage[] allLanguages = Singleton<SystemDataContainer>.Instance.GetAllLanguages();
		foreach (SystemDataContainer.SupportedLanguage sl in allLanguages)
		{
			GameObject gameObject = Object.Instantiate(m_languageSelectionPrefab);
			gameObject.transform.parent = m_layoutGrid.transform;
			gameObject.GetComponent<LanguageSelectionButton>().SetLanguageText(sl.m_localisedName, Singleton<LanguageLoader>.Instance.GetLanguageFontReference(sl.m_languageToken));
			m_languageButtons[sl] = gameObject;
			SelectableFilterButton sfb = gameObject.GetComponent<SelectableFilterButton>();
			if (sl.m_languageToken == m_currentlySelectedLanguage)
			{
				sfb.SetSelected(selected: true);
				tk2dTextMesh tk2dTextMesh2 = (tk2dTextMesh)m_confirmButtonText;
				if (tk2dTextMesh2 != null)
				{
					tk2dTextMesh2.SetOverrideLangFlag(Singleton<LanguageLoader>.Instance.GetLanguageFontReference(m_currentlySelectedLanguage));
					tk2dTextMesh2.UpdateFontType();
				}
				m_confirmButtonText.SetText(sl.m_confirmButtonText);
				m_confirmButtonLayoutGrid.RepositionChildren();
			}
			sfb.OnClick += delegate
			{
				m_currentlySelectedLanguage = sl.m_languageToken;
				tk2dTextMesh tk2dTextMesh3 = (tk2dTextMesh)m_confirmButtonText;
				if (tk2dTextMesh3 != null)
				{
					tk2dTextMesh3.SetOverrideLangFlag(Singleton<LanguageLoader>.Instance.GetLanguageFontReference(m_currentlySelectedLanguage));
					tk2dTextMesh3.UpdateFontType();
				}
				m_confirmButtonText.SetText(sl.m_confirmButtonText);
				m_confirmButtonLayoutGrid.RepositionChildren();
				foreach (GameObject value in m_languageButtons.Values)
				{
					SelectableFilterButton component = value.GetComponent<SelectableFilterButton>();
					component.SetSelected(sfb == component);
				}
			};
		}
		m_layoutGrid.RepositionChildren();
	}

	private void Confirm()
	{
		Singleton<SystemDataContainer>.Instance.Get().SetCurrentLanguage(m_currentlySelectedLanguage);
		Singleton<SystemDataContainer>.Instance.Get().SetHasEverSetLanguage();
		Singleton<SystemDataContainer>.Instance.Save();
		Singleton<LanguageLoader>.Instance.ReLoadLanguage();
		Singleton<UIScreenManager>.Instance.ShowScreen("StartScreen", showNavBarButtons: true);
	}
}
