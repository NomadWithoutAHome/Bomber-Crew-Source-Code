using System;
using BomberCrewCommon;
using UnityEngine;

public class PopupEditCharacter : MonoBehaviour
{
	public enum ChangeType
	{
		HairStyle,
		HairColour,
		Body,
		SkinColour,
		Eyes,
		Mouth,
		FacialHair
	}

	[SerializeField]
	private tk2dUIItem m_hairStyleNext;

	[SerializeField]
	private tk2dUIItem m_hairStylePrevious;

	[SerializeField]
	private tk2dUIItem m_hairColourNext;

	[SerializeField]
	private tk2dUIItem m_hairColourPrev;

	[SerializeField]
	private tk2dUIItem m_skinColourNext;

	[SerializeField]
	private tk2dUIItem m_skinColourPrev;

	[SerializeField]
	private tk2dUIItem m_eyesNext;

	[SerializeField]
	private tk2dUIItem m_eyesPrev;

	[SerializeField]
	private tk2dUIItem m_mouthNext;

	[SerializeField]
	private tk2dUIItem m_mouthPrev;

	[SerializeField]
	private tk2dUIItem m_facialHairNext;

	[SerializeField]
	private tk2dUIItem m_facialHairPrev;

	[SerializeField]
	private tk2dUIItem m_bodyNext;

	[SerializeField]
	private tk2dUIItem m_bodyPrev;

	[SerializeField]
	private GameObject m_facialHairHierarchy;

	[SerializeField]
	private GameObject[] m_consoleOnlyHierarchy;

	[SerializeField]
	private GameObject[] m_pcOnlyHierarchy;

	[SerializeField]
	private tk2dUITextInput m_textInputFieldFirst;

	[SerializeField]
	private tk2dUITextInput m_textInputFieldSecond;

	[SerializeField]
	private TextSetter m_lengthTextFirst;

	[SerializeField]
	private TextSetter m_lengthTextSecond;

	[SerializeField]
	private tk2dUIItem m_renameButtonFirst;

	[SerializeField]
	private tk2dUIItem m_renameButtonSecond;

	[SerializeField]
	private TextSetter m_renameButtonTextNameFirst;

	[SerializeField]
	private TextSetter m_renameButtonTextNameSecond;

	[SerializeField]
	private UISelectFinder m_finder;

	[SerializeField]
	private CrewmanGraphics m_crewmanRenderer;

	[SerializeField]
	private tk2dUIItem m_closeButton;

	[SerializeField]
	private int m_maxNameLength = 13;

	[SerializeField]
	private UISelectorPointingHint m_pointingHintLower;

	[SerializeField]
	private UISelectorPointingHint[] m_pointingHintUpper;

	[SerializeField]
	private UISelectFinder m_selectFinderNone;

	private Crewman m_crewman;

	private UISelectFinder m_previousFinder;

	private tk2dUIItem m_previouslyPointedAt;

	private string m_newFirstName;

	private string m_newSurname;

	private bool m_textBoxIsFocus;

	public void SetUp(Crewman c)
	{
		m_crewman = c;
		m_lengthTextFirst.SetText($"{c.GetFirstName().Length} / {m_maxNameLength}");
		m_lengthTextSecond.SetText($"{c.GetSurname().Length} / {m_maxNameLength}");
		if (Singleton<UISelector>.Instance.UseVirtualKeyboard())
		{
			m_renameButtonTextNameSecond.SetText(c.GetSurname());
			m_renameButtonTextNameFirst.SetText(c.GetFirstName());
			GameObject[] consoleOnlyHierarchy = m_consoleOnlyHierarchy;
			foreach (GameObject gameObject in consoleOnlyHierarchy)
			{
				gameObject.SetActive(value: true);
			}
			GameObject[] pcOnlyHierarchy = m_pcOnlyHierarchy;
			foreach (GameObject gameObject2 in pcOnlyHierarchy)
			{
				gameObject2.SetActive(value: false);
			}
			m_renameButtonSecond.OnClick += RenameSurname;
			m_renameButtonFirst.OnClick += RenameFirstName;
		}
		else
		{
			m_textInputFieldFirst.m_deselectOnEnter = true;
			m_textInputFieldSecond.m_deselectOnEnter = true;
			m_textInputFieldFirst.Text = c.GetFirstName();
			m_textInputFieldFirst.maxCharacterLength = m_maxNameLength;
			m_textInputFieldSecond.Text = c.GetSurname();
			m_textInputFieldSecond.maxCharacterLength = m_maxNameLength;
			tk2dUITextInput textInputFieldFirst = m_textInputFieldFirst;
			textInputFieldFirst.OnPreFocus = (Action)Delegate.Combine(textInputFieldFirst.OnPreFocus, new Action(ChangeSelector));
			tk2dUITextInput textInputFieldSecond = m_textInputFieldSecond;
			textInputFieldSecond.OnPreFocus = (Action)Delegate.Combine(textInputFieldSecond.OnPreFocus, new Action(ChangeSelector));
			tk2dUITextInput textInputFieldFirst2 = m_textInputFieldFirst;
			textInputFieldFirst2.OnExitFocus = (Action)Delegate.Combine(textInputFieldFirst2.OnExitFocus, new Action(ResetSelector));
			tk2dUITextInput textInputFieldSecond2 = m_textInputFieldSecond;
			textInputFieldSecond2.OnExitFocus = (Action)Delegate.Combine(textInputFieldSecond2.OnExitFocus, new Action(ResetSelector));
			GameObject[] consoleOnlyHierarchy2 = m_consoleOnlyHierarchy;
			foreach (GameObject gameObject3 in consoleOnlyHierarchy2)
			{
				gameObject3.SetActive(value: false);
			}
			GameObject[] pcOnlyHierarchy2 = m_pcOnlyHierarchy;
			foreach (GameObject gameObject4 in pcOnlyHierarchy2)
			{
				gameObject4.SetActive(value: true);
			}
			m_pointingHintLower.SetUpLink(m_textInputFieldSecond.GetComponentInChildren<tk2dUIItem>());
			UISelectorPointingHint[] pointingHintUpper = m_pointingHintUpper;
			foreach (UISelectorPointingHint uISelectorPointingHint in pointingHintUpper)
			{
				uISelectorPointingHint.SetDownLink(m_textInputFieldFirst.GetComponentInChildren<tk2dUIItem>());
			}
		}
		RefreshCrewman();
		Refresh();
	}

	private void ChangeSelector()
	{
		Singleton<UISelector>.Instance.SetFinder(m_selectFinderNone);
		Singleton<MainActionButtonMonitor>.Instance.InvalidateCurrentFrame();
		m_textBoxIsFocus = true;
	}

	private void ResetSelector()
	{
		Singleton<UISelector>.Instance.SetFinder(m_finder);
		m_textBoxIsFocus = false;
	}

	private void RenameFirstName()
	{
		string startingText = ((!string.IsNullOrEmpty(m_newFirstName)) ? m_newFirstName : m_crewman.GetFirstName());
		Singleton<RenameStringUsingPlatform>.Instance.SoftwareKeyboard(startingText, Singleton<LanguageProvider>.Instance.GetNamedTextImmediate("ui_rename_firstname"), m_maxNameLength, delegate(string res)
		{
			if (res.Length > 0)
			{
				m_newFirstName = res;
				m_renameButtonTextNameFirst.SetText(res);
				m_lengthTextFirst.SetText($"{res.Length} / {m_maxNameLength}");
			}
		}, null);
	}

	private void RenameSurname()
	{
		string startingText = ((!string.IsNullOrEmpty(m_newSurname)) ? m_newSurname : m_crewman.GetSurname());
		Singleton<RenameStringUsingPlatform>.Instance.SoftwareKeyboard(startingText, Singleton<LanguageProvider>.Instance.GetNamedTextImmediate("ui_rename_secondname"), m_maxNameLength, delegate(string res)
		{
			if (res.Length > 0)
			{
				m_newSurname = res;
				m_renameButtonTextNameSecond.SetText(res);
				m_lengthTextSecond.SetText($"{res.Length} / {m_maxNameLength}");
			}
		}, null);
	}

	private void OnEnable()
	{
		RewiredKeyboardDisable.SetKeyboardDisable(disable: true);
		Singleton<MainActionButtonMonitor>.Instance.AddListener(StartButtons);
		m_previousFinder = Singleton<UISelector>.Instance.GetCurrentFinder();
		m_previouslyPointedAt = Singleton<UISelector>.Instance.GetCurrentMovementType().GetCurrentlyPointedAtItem();
		Singleton<UISelector>.Instance.SetFinder(m_finder);
	}

	private void OnDisable()
	{
		RewiredKeyboardDisable.SetKeyboardDisable(disable: false);
		Singleton<MainActionButtonMonitor>.Instance.RemoveListener(StartButtons, invalidateCurrentPress: true);
		Singleton<UISelector>.Instance.SetFinder(m_previousFinder);
		if (m_previouslyPointedAt != null)
		{
			Singleton<UISelector>.Instance.ForcePointAt(m_previouslyPointedAt);
		}
	}

	private bool StartButtons(MainActionButtonMonitor.ButtonPress bp)
	{
		if (m_textBoxIsFocus && (bp == MainActionButtonMonitor.ButtonPress.Confirm || bp == MainActionButtonMonitor.ButtonPress.Back))
		{
			if (m_textInputFieldFirst.IsFocus)
			{
				m_textInputFieldFirst.SetFocus(focus: false);
			}
			if (m_textInputFieldSecond.IsFocus)
			{
				m_textInputFieldSecond.SetFocus(focus: false);
			}
		}
		return true;
	}

	private void Awake()
	{
		m_hairStyleNext.OnClick += delegate
		{
			Change(ChangeType.HairStyle, 1);
		};
		m_hairStylePrevious.OnClick += delegate
		{
			Change(ChangeType.HairStyle, -1);
		};
		m_skinColourNext.OnClick += delegate
		{
			Change(ChangeType.SkinColour, 1);
		};
		m_skinColourPrev.OnClick += delegate
		{
			Change(ChangeType.SkinColour, -1);
		};
		m_hairColourNext.OnClick += delegate
		{
			Change(ChangeType.HairColour, 1);
		};
		m_hairColourPrev.OnClick += delegate
		{
			Change(ChangeType.HairColour, -1);
		};
		m_bodyNext.OnClick += delegate
		{
			Change(ChangeType.Body, 1);
		};
		m_bodyPrev.OnClick += delegate
		{
			Change(ChangeType.Body, -1);
		};
		m_eyesNext.OnClick += delegate
		{
			Change(ChangeType.Eyes, 1);
		};
		m_eyesPrev.OnClick += delegate
		{
			Change(ChangeType.Eyes, -1);
		};
		m_mouthNext.OnClick += delegate
		{
			Change(ChangeType.Mouth, 1);
		};
		m_mouthPrev.OnClick += delegate
		{
			Change(ChangeType.Mouth, -1);
		};
		m_facialHairNext.OnClick += delegate
		{
			Change(ChangeType.FacialHair, 1);
		};
		m_facialHairPrev.OnClick += delegate
		{
			Change(ChangeType.FacialHair, -1);
		};
		m_closeButton.OnClick += ClosePopup;
	}

	private void ClosePopup()
	{
		if (Singleton<UISelector>.Instance.UseVirtualKeyboard())
		{
			if (!string.IsNullOrEmpty(m_newFirstName))
			{
				m_crewman.SetFirstName(m_newFirstName);
			}
			if (!string.IsNullOrEmpty(m_newSurname))
			{
				m_crewman.SetSurname(m_newSurname);
			}
		}
		else
		{
			if (!string.IsNullOrEmpty(m_textInputFieldFirst.Text))
			{
				m_crewman.SetFirstName(m_textInputFieldFirst.Text);
			}
			if (!string.IsNullOrEmpty(m_textInputFieldSecond.Text))
			{
				m_crewman.SetSurname(m_textInputFieldSecond.Text);
			}
		}
		m_crewman.RefreshPortrait();
		Singleton<UIPopupManager>.Instance.DismissCurrentPopup();
	}

	private int Wrap(int val, int max, int change)
	{
		val %= max;
		val += change;
		if (val < 0)
		{
			val = max - 1;
		}
		else if (val >= max)
		{
			val = 0;
		}
		return val;
	}

	private int Wrap(int val, int min, int max, int change)
	{
		val %= max;
		val += change;
		if (val < min)
		{
			val = max - 1;
		}
		else if (val >= max)
		{
			val = min;
		}
		return val;
	}

	private void Change(ChangeType ct, int change)
	{
		switch (ct)
		{
		case ChangeType.HairStyle:
		{
			int hairVariation = m_crewman.GetHairVariation();
			int hairVariation2 = Wrap(hairVariation, Singleton<CrewmanAppearanceCatalogue>.Instance.GetNumHairVariation(m_crewman.GetModelType()), change);
			m_crewman.SetHairVariation(hairVariation2);
			break;
		}
		case ChangeType.Body:
			m_crewman.ChangeBodyType();
			break;
		case ChangeType.Eyes:
		{
			int eyeColor = m_crewman.GetEyeColor();
			int eyes = Wrap(eyeColor, Singleton<CrewmanAppearanceCatalogue>.Instance.GetIndexCountEyes(), change);
			m_crewman.SetEyes(eyes);
			break;
		}
		case ChangeType.HairColour:
		{
			int hairColor = m_crewman.GetHairColor();
			int min = 0;
			if (Singleton<CrewmanAppearanceCatalogue>.Instance.GetHairMesh(m_crewman.GetModelType(), m_crewman.GetHairVariation()) != null)
			{
				min = 1;
			}
			int hairColor2 = Wrap(hairColor, min, Singleton<CrewmanAppearanceCatalogue>.Instance.GetIndexCountHair(), change);
			m_crewman.SetHairColor(hairColor2);
			break;
		}
		case ChangeType.Mouth:
		{
			int mouthType = m_crewman.GetMouthType();
			int mouthType2 = Wrap(mouthType, Singleton<CrewmanAppearanceCatalogue>.Instance.GetIndexCountMouth(), change);
			m_crewman.SetMouthType(mouthType2);
			break;
		}
		case ChangeType.SkinColour:
		{
			int skinTone = m_crewman.GetSkinTone();
			int skinTone2 = Wrap(skinTone, Singleton<CrewmanAppearanceCatalogue>.Instance.GetIndexCountSkin(), change);
			m_crewman.SetSkinTone(skinTone2);
			break;
		}
		case ChangeType.FacialHair:
		{
			int facialHairVariation = m_crewman.GetFacialHairVariation();
			int facialHairVariation2 = Wrap(facialHairVariation, Singleton<CrewmanAppearanceCatalogue>.Instance.GetNumFacialHairVariation(), change);
			m_crewman.SetFacialHairVariation(facialHairVariation2);
			break;
		}
		}
		RefreshCrewman();
	}

	private void RefreshCrewman()
	{
		m_crewmanRenderer.SetFromCrewman(m_crewman);
		m_facialHairHierarchy.SetActive(m_crewman.GetModelType() == Crewman.ModelType.Male);
	}

	private void Refresh()
	{
		if (!Singleton<UISelector>.Instance.UseVirtualKeyboard())
		{
			m_lengthTextFirst.SetText($"{m_textInputFieldFirst.Text.Length} / {m_maxNameLength}");
			m_lengthTextSecond.SetText($"{m_textInputFieldSecond.Text.Length} / {m_maxNameLength}");
		}
	}

	private void Update()
	{
		Refresh();
		if (Input.GetKeyUp(KeyCode.Return) && m_textInputFieldFirst.IsFocus)
		{
			m_textInputFieldFirst.SetFocus(focus: false);
			m_textInputFieldSecond.SetFocus(focus: true);
		}
	}
}
