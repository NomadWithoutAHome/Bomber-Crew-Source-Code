using BomberCrewCommon;
using UnityEngine;

public class HowToPlayItemDisplay : MonoBehaviour
{
	[SerializeField]
	private Renderer m_imageRenderer;

	[SerializeField]
	private TextSetter m_textSetter;

	[SerializeField]
	private tk2dUIItem m_linkButton;

	[SerializeField]
	private tk2dBaseSprite m_iconSprite;

	[SerializeField]
	private TextSetter m_linkButtonText;

	private HowToPlayInfoSection.HowToPlaySectionItem m_sectionItem;

	private PauseMenuHowToPlayPage m_controllerPage;

	private void Start()
	{
		if (m_linkButton != null)
		{
			m_linkButton.OnClick += ClickButton;
		}
	}

	public void SetUp(HowToPlayInfoSection.HowToPlaySectionItem item, PauseMenuHowToPlayPage controllerPage)
	{
		m_controllerPage = controllerPage;
		m_sectionItem = item;
		if (m_imageRenderer != null && item.m_image != null)
		{
			m_imageRenderer.sharedMaterial = Object.Instantiate(m_imageRenderer.sharedMaterial);
			m_imageRenderer.sharedMaterial.mainTexture = item.m_image;
			float num = m_imageRenderer.transform.localScale.x / (float)item.m_image.width;
			float y = (float)item.m_image.height * num;
			m_imageRenderer.transform.localScale = new Vector3(m_imageRenderer.transform.localScale.x, y, 1f);
		}
		if (m_textSetter != null)
		{
			string text = item.m_text;
			if (Singleton<UISelector>.Instance.IsPrimary() && !string.IsNullOrEmpty(item.m_textController) && item.m_textController != "UNSET")
			{
				text = item.m_textController;
			}
			if (text == "UNSET")
			{
				m_textSetter.SetText(string.Empty);
			}
			else
			{
				m_textSetter.SetText(ControlPromptDisplayHelpers.ConvertString(Singleton<LanguageProvider>.Instance.GetNamedTextImmediate(text)));
			}
			if (m_textSetter is tk2dTextMesh)
			{
				((tk2dTextMesh)m_textSetter).ForceBuild();
			}
		}
		if (m_iconSprite != null)
		{
			m_iconSprite.SetSprite(item.m_iconReference);
		}
		if (m_linkButton != null)
		{
			HowToPlayInfoSection page = m_controllerPage.GetPage(item.m_linkReference);
			if (page != null)
			{
				m_linkButtonText.SetText(string.Format("{0} {1}", Singleton<LanguageProvider>.Instance.GetNamedTextImmediate("misc_htp_seemore"), Singleton<LanguageProvider>.Instance.GetNamedTextImmediate(page.m_titleText)));
			}
		}
	}

	private void ClickButton()
	{
		if (m_controllerPage != null)
		{
			m_controllerPage.GoToPage(m_sectionItem.m_linkReference);
		}
	}
}
