using System.Collections.Generic;
using BomberCrewCommon;
using UnityEngine;

public class PauseMenuHowToPlayPage : MonoBehaviour
{
	[SerializeField]
	private GameObject m_categoryGroupTitlePrefab;

	[SerializeField]
	private GameObject m_categoryTitlePrefab;

	[SerializeField]
	private LayoutGrid m_categoryScrollArea;

	[SerializeField]
	private tk2dUIScrollableArea m_scrollableAreaCategory;

	[SerializeField]
	private GameObject m_sectionItemImage;

	[SerializeField]
	private GameObject m_sectionItemTitle;

	[SerializeField]
	private GameObject m_sectionItemParagraph;

	[SerializeField]
	private GameObject m_sectionItemBulletPoint;

	[SerializeField]
	private GameObject m_sectionItemIconPoint;

	[SerializeField]
	private GameObject m_sectionItemLink;

	[SerializeField]
	private LayoutGrid m_sectionItemLayoutGrid;

	[SerializeField]
	private tk2dUIScrollableArea m_scrollableAreaSectionItems;

	[SerializeField]
	private PauseMenuHowToPlayAllPages m_allPages;

	[SerializeField]
	private TextSetter m_titleText;

	private List<SelectableFilterButton> m_allButtons = new List<SelectableFilterButton>();

	private List<GameObject> m_currentlyCreatedPageItems = new List<GameObject>();

	private int m_startCategory;

	private int m_startPage;

	private Dictionary<SelectableFilterButton, int> m_allButtonRefs = new Dictionary<SelectableFilterButton, int>();

	private int m_totalNum;

	[SerializeField]
	private UISelectFinder m_finder;

	[SerializeField]
	private UISelectorMovementType m_movementType;

	private HowToPlayInfoSection m_currentlyShowingPage;

	private void OnEnable()
	{
		Singleton<UISelector>.Instance.SetFinder(m_finder);
	}

	private void Start()
	{
		int num = 0;
		m_totalNum = 0;
		PauseMenuHowToPlayAllPages.CategoryHeading[] allCategories = m_allPages.m_allCategories;
		foreach (PauseMenuHowToPlayAllPages.CategoryHeading categoryHeading in allCategories)
		{
			if (categoryHeading.m_hasHeader)
			{
				GameObject gameObject = Object.Instantiate(m_categoryGroupTitlePrefab);
				gameObject.transform.parent = m_categoryScrollArea.transform;
				gameObject.transform.localPosition = Vector3.zero;
				gameObject.GetComponentInChildren<TextSetter>().SetTextFromLanguageString(categoryHeading.m_categoryName);
			}
			int num2 = 0;
			HowToPlayInfoSection[] allPages = categoryHeading.m_allPages;
			foreach (HowToPlayInfoSection howToPlayInfoSection in allPages)
			{
				GameObject gameObject2 = Object.Instantiate(m_categoryTitlePrefab);
				gameObject2.transform.parent = m_categoryScrollArea.transform;
				gameObject2.transform.localPosition = Vector3.zero;
				gameObject2.GetComponent<SelectableFilterButton>().SetText(Singleton<LanguageProvider>.Instance.GetNamedTextImmediate(howToPlayInfoSection.m_titleText));
				int index = num2;
				int ci = num;
				gameObject2.GetComponent<SelectableFilterButton>().SetRelatedObject(howToPlayInfoSection);
				gameObject2.GetComponent<SelectableFilterButton>().OnClick += delegate
				{
					ShowPage(ci, index, jumpScroller: false);
				};
				m_allButtons.Add(gameObject2.GetComponent<SelectableFilterButton>());
				m_allButtonRefs[gameObject2.GetComponent<SelectableFilterButton>()] = m_totalNum;
				num2++;
				m_totalNum++;
			}
			num++;
		}
		m_categoryScrollArea.RepositionChildren();
		m_scrollableAreaCategory.ContentLength = m_scrollableAreaCategory.MeasureContentLength();
		ShowPage(m_startCategory, m_startPage, jumpScroller: true);
	}

	public void SetStartPage(string pageRef)
	{
		int num = 0;
		PauseMenuHowToPlayAllPages.CategoryHeading[] allCategories = m_allPages.m_allCategories;
		foreach (PauseMenuHowToPlayAllPages.CategoryHeading categoryHeading in allCategories)
		{
			int num2 = 0;
			HowToPlayInfoSection[] allPages = categoryHeading.m_allPages;
			foreach (HowToPlayInfoSection howToPlayInfoSection in allPages)
			{
				if (howToPlayInfoSection.m_jumpToReference == pageRef)
				{
					m_startCategory = num;
					m_startPage = num2;
				}
				num2++;
			}
			num++;
		}
	}

	public HowToPlayInfoSection GetPage(string pageRef)
	{
		PauseMenuHowToPlayAllPages.CategoryHeading[] allCategories = m_allPages.m_allCategories;
		foreach (PauseMenuHowToPlayAllPages.CategoryHeading categoryHeading in allCategories)
		{
			HowToPlayInfoSection[] allPages = categoryHeading.m_allPages;
			foreach (HowToPlayInfoSection howToPlayInfoSection in allPages)
			{
				if (howToPlayInfoSection.m_jumpToReference == pageRef)
				{
					return howToPlayInfoSection;
				}
			}
		}
		return null;
	}

	public void GoToPage(string pageRef)
	{
		int num = 0;
		PauseMenuHowToPlayAllPages.CategoryHeading[] allCategories = m_allPages.m_allCategories;
		foreach (PauseMenuHowToPlayAllPages.CategoryHeading categoryHeading in allCategories)
		{
			int num2 = 0;
			HowToPlayInfoSection[] allPages = categoryHeading.m_allPages;
			foreach (HowToPlayInfoSection howToPlayInfoSection in allPages)
			{
				if (howToPlayInfoSection.m_jumpToReference == pageRef)
				{
					ShowPage(num, num2, jumpScroller: true);
				}
				num2++;
			}
			num++;
		}
	}

	public void ShowPage(int category, int page, bool jumpScroller)
	{
		HowToPlayInfoSection howToPlayInfoSection = m_allPages.m_allCategories[category].m_allPages[page];
		foreach (SelectableFilterButton allButton in m_allButtons)
		{
			bool flag = howToPlayInfoSection == (HowToPlayInfoSection)allButton.GetRelatedObject();
			allButton.SetSelected(flag);
			if (flag)
			{
				if (jumpScroller)
				{
					int num = m_allButtonRefs[allButton];
					float value = Mathf.Clamp01((float)num / (float)(m_totalNum - 1));
					m_scrollableAreaCategory.Value = value;
				}
				Singleton<UISelector>.Instance.ForcePointAt(allButton.GetUIItem());
			}
		}
		if (!(howToPlayInfoSection != m_currentlyShowingPage))
		{
			return;
		}
		m_currentlyShowingPage = howToPlayInfoSection;
		foreach (GameObject currentlyCreatedPageItem in m_currentlyCreatedPageItems)
		{
			Object.DestroyImmediate(currentlyCreatedPageItem);
		}
		if (m_allPages.m_allCategories[category].m_hasHeader)
		{
			m_titleText.SetText($"{Singleton<LanguageProvider>.Instance.GetNamedTextImmediate(m_allPages.m_allCategories[category].m_categoryName)}: {Singleton<LanguageProvider>.Instance.GetNamedTextImmediate(howToPlayInfoSection.m_titleText)}");
		}
		else
		{
			m_titleText.SetTextFromLanguageString(howToPlayInfoSection.m_titleText);
		}
		HowToPlayInfoSection.HowToPlaySectionItem[] allItems = howToPlayInfoSection.m_allItems;
		foreach (HowToPlayInfoSection.HowToPlaySectionItem howToPlaySectionItem in allItems)
		{
			GameObject gameObject = null;
			switch (howToPlaySectionItem.m_itemType)
			{
			case HowToPlayInfoSection.ItemType.BulletPoint:
				gameObject = m_sectionItemBulletPoint;
				break;
			case HowToPlayInfoSection.ItemType.Image:
				gameObject = m_sectionItemImage;
				break;
			case HowToPlayInfoSection.ItemType.Paragraph:
				gameObject = m_sectionItemParagraph;
				break;
			case HowToPlayInfoSection.ItemType.Title:
				gameObject = m_sectionItemTitle;
				break;
			case HowToPlayInfoSection.ItemType.IconPoint:
				gameObject = m_sectionItemIconPoint;
				break;
			case HowToPlayInfoSection.ItemType.LinkButton:
				gameObject = m_sectionItemLink;
				break;
			}
			if (gameObject != null)
			{
				GameObject gameObject2 = Object.Instantiate(gameObject);
				gameObject2.transform.parent = m_sectionItemLayoutGrid.transform;
				gameObject2.transform.localPosition = Vector3.zero;
				gameObject2.GetComponent<HowToPlayItemDisplay>().SetUp(howToPlaySectionItem, this);
				m_currentlyCreatedPageItems.Add(gameObject2);
			}
		}
		m_sectionItemLayoutGrid.RepositionChildren();
		m_scrollableAreaSectionItems.ContentLength = m_scrollableAreaSectionItems.MeasureContentLength() + 32f;
		m_scrollableAreaSectionItems.Value = 0f;
	}
}
