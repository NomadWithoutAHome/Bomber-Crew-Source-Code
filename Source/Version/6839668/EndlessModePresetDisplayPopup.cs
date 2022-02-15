using System;
using System.Collections;
using BomberCrewCommon;
using UnityEngine;

public class EndlessModePresetDisplayPopup : MonoBehaviour
{
	[SerializeField]
	private TextSetter m_loadSaveButtonText;

	[SerializeField]
	private TextSetter m_presetTitleText;

	[SerializeField]
	private GameObject[] m_toLoadHierarchies;

	[SerializeField]
	private GameObject[] m_toSaveHierarchies;

	[SerializeField]
	private Renderer m_imageRenderer;

	[SerializeField]
	private GameObject m_buttonsHierarchy;

	[SerializeField]
	private tk2dUIItem m_mainButton;

	[SerializeField]
	private tk2dUIItem m_cancelButton;

	[SerializeField]
	private UISelectFinder m_finder;

	[SerializeField]
	private tk2dUITextInput m_textInput;

	[SerializeField]
	private tk2dUIItem m_textInputButton;

	[SerializeField]
	private TextSetter m_lengthTextSetter;

	[SerializeField]
	private int m_maxLength = 30;

	[SerializeField]
	private GameObject m_consoleLayout;

	[SerializeField]
	private GameObject m_pcLayout;

	[SerializeField]
	private UISelectorPointingHint[] m_pointingHintUp;

	private UISelectFinder m_previousFinder;

	private tk2dUIItem m_previouslyPointedAt;

	private RenderTexture m_waitForImage;

	private bool m_imageIsGenerated;

	private string m_saveName;

	public event Action<Texture2D, string> OnSave;

	public event Action OnLoad;

	private void Awake()
	{
		m_consoleLayout.SetActive(value: false);
		m_pcLayout.SetActive(value: true);
		m_saveName = string.Empty;
	}

	private void OnEnable()
	{
		Singleton<MainActionButtonMonitor>.Instance.AddListener(StartButtons);
		m_previousFinder = Singleton<UISelector>.Instance.GetCurrentFinder();
		m_previouslyPointedAt = Singleton<UISelector>.Instance.GetCurrentMovementType().GetCurrentlyPointedAtItem();
		Singleton<UISelector>.Instance.SetFinder(m_finder);
		if (Singleton<UISelector>.Instance.UseVirtualKeyboard())
		{
			UISelectorPointingHint[] pointingHintUp = m_pointingHintUp;
			foreach (UISelectorPointingHint uISelectorPointingHint in pointingHintUp)
			{
				uISelectorPointingHint.SetUpLink(m_textInput.GetComponentInChildren<tk2dUIItem>());
			}
		}
	}

	private void OnDisable()
	{
		Singleton<MainActionButtonMonitor>.Instance.RemoveListener(StartButtons, invalidateCurrentPress: true);
		Singleton<UISelector>.Instance.SetFinder(m_previousFinder);
		if (m_previouslyPointedAt != null)
		{
			Singleton<UISelector>.Instance.ForcePointAt(m_previouslyPointedAt);
		}
	}

	private bool StartButtons(MainActionButtonMonitor.ButtonPress bp)
	{
		if (bp == MainActionButtonMonitor.ButtonPress.Back)
		{
			Cancel();
		}
		return true;
	}

	private void SetMode(bool loading)
	{
		GameObject[] toLoadHierarchies = m_toLoadHierarchies;
		foreach (GameObject gameObject in toLoadHierarchies)
		{
			gameObject.SetActive(loading);
		}
		GameObject[] toSaveHierarchies = m_toSaveHierarchies;
		foreach (GameObject gameObject2 in toSaveHierarchies)
		{
			gameObject2.SetActive(!loading);
		}
		m_cancelButton.OnClick += Cancel;
		if (loading)
		{
			m_mainButton.OnClick += Load;
			m_loadSaveButtonText.SetTextFromLanguageString("endless_mode_presetchange_button_load");
		}
		else
		{
			m_mainButton.OnClick += Save;
			m_loadSaveButtonText.SetTextFromLanguageString("endless_mode_presetchange_button_save");
		}
	}

	private void Load()
	{
		if (this.OnLoad != null)
		{
			this.OnLoad();
		}
		Singleton<UIPopupManager>.Instance.DismissCurrentPopup();
	}

	private void Save()
	{
		m_buttonsHierarchy.SetActive(value: false);
		StartCoroutine(WaitAndSave());
	}

	private IEnumerator WaitAndSave()
	{
		while (!m_imageIsGenerated)
		{
			yield return null;
		}
		if (this.OnSave != null)
		{
			Singleton<UIPopupManager>.Instance.DismissCurrentPopup();
			RenderTexture.active = m_waitForImage;
			Texture2D texture2D = new Texture2D(m_waitForImage.width, m_waitForImage.height, TextureFormat.RGB24, mipmap: false);
			texture2D.ReadPixels(new Rect(0f, 0f, m_waitForImage.width, m_waitForImage.height), 0, 0);
			texture2D.Apply();
			if (string.IsNullOrEmpty(m_saveName))
			{
				m_saveName = Singleton<BomberContainer>.Instance.GetCurrentConfig().GetName();
			}
			this.OnSave(texture2D, m_saveName);
		}
	}

	private void Cancel()
	{
		Singleton<UIPopupManager>.Instance.DismissCurrentPopup();
	}

	public void SetUp(EndlessModeVariant.Loadout loadout)
	{
		SetMode(loading: true);
		m_presetTitleText.SetTextFromLanguageString(loadout.m_title);
		m_imageRenderer.sharedMaterial = UnityEngine.Object.Instantiate(m_imageRenderer.material);
		m_imageRenderer.sharedMaterial.mainTexture = loadout.GetTexture();
	}

	public void SetUp(EndlessModeVariant.LoadoutJS loadout, string mode, int index)
	{
		bool flag = loadout != null;
		SetMode(flag);
		if (!flag)
		{
			int currentCrewCount = Singleton<CrewContainer>.Instance.GetCurrentCrewCount();
			Crewman[] array = new Crewman[currentCrewCount];
			for (int i = 0; i < currentCrewCount; i++)
			{
				array[i] = Singleton<CrewContainer>.Instance.GetCrewman(i);
			}
			m_saveName = Singleton<BomberContainer>.Instance.GetCurrentConfig().GetName();
			m_presetTitleText.SetText(m_saveName);
			m_imageIsGenerated = false;
			m_waitForImage = Singleton<CrewAndBomberPhotoBooth>.Instance.RenderForCrewAndBomber(array, Singleton<BomberContainer>.Instance.GetCurrentConfig());
			m_lengthTextSetter.SetText($"{m_saveName.Length} / {m_maxLength}");
			m_textInput.Text = m_saveName;
			m_textInput.maxCharacterLength = m_maxLength;
			tk2dUITextInput textInput = m_textInput;
			textInput.OnTextChange = (Action<tk2dUITextInput>)Delegate.Combine(textInput.OnTextChange, new Action<tk2dUITextInput>(ChangeText));
			m_textInputButton.OnClick += RenameText;
			StartCoroutine(WaitForImage());
		}
		else
		{
			m_presetTitleText.SetText(loadout.m_title);
			m_imageRenderer.sharedMaterial = UnityEngine.Object.Instantiate(m_imageRenderer.material);
			m_imageRenderer.sharedMaterial.mainTexture = loadout.GetTexture(mode, index);
		}
	}

	private void RenameText()
	{
		Singleton<RenameStringUsingPlatform>.Instance.Rename(m_saveName, Singleton<LanguageProvider>.Instance.GetNamedTextImmediate("endlessmode_rename_text_header"), m_maxLength, delegate(string result)
		{
			m_saveName = result;
			m_presetTitleText.SetText(m_saveName);
		}, null);
	}

	private void ChangeText(tk2dUITextInput textInput)
	{
		m_saveName = m_textInput.Text;
		m_presetTitleText.SetText(m_saveName);
	}

	private void Update()
	{
		m_lengthTextSetter.SetText($"{m_saveName.Length} / {m_maxLength}");
	}

	private IEnumerator WaitForImage()
	{
		while (Singleton<CrewAndBomberPhotoBooth>.Instance.IsProcessing())
		{
			yield return null;
		}
		m_imageRenderer.sharedMaterial = UnityEngine.Object.Instantiate(m_imageRenderer.material);
		m_imageRenderer.sharedMaterial.mainTexture = m_waitForImage;
		m_imageIsGenerated = true;
	}
}
