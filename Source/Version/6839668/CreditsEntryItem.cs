using UnityEngine;

public class CreditsEntryItem : MonoBehaviour
{
	[SerializeField]
	private TextSetter m_titleText;

	[SerializeField]
	private TextSetter m_mainText;

	[SerializeField]
	private TextSetter m_smallText;

	[SerializeField]
	private tk2dBaseSprite m_iconSprite;

	[SerializeField]
	private LayoutGrid m_layoutGrid;

	public void SetUp(CreditsPopulator.CreditsItem ci)
	{
		if (m_titleText != null)
		{
			m_titleText.SetText(ci.m_title);
			m_titleText.GetComponent<tk2dTextMesh>().ForceBuild();
			m_titleText.GetComponent<TextMeshShadow>().Refresh();
			if (string.IsNullOrEmpty(ci.m_title))
			{
				m_titleText.gameObject.SetActive(value: false);
			}
		}
		if (m_smallText != null)
		{
			m_smallText.GetComponent<tk2dTextMesh>().SetForceWesternLineBreaks();
			m_smallText.SetText(ci.m_smallText);
			m_smallText.GetComponent<tk2dTextMesh>().ForceBuild();
			m_smallText.GetComponent<TextMeshShadow>().Refresh();
			if (string.IsNullOrEmpty(ci.m_smallText))
			{
				m_smallText.gameObject.SetActive(value: false);
			}
		}
		if (m_mainText != null)
		{
			m_mainText.SetText(ci.m_text);
			m_mainText.GetComponent<tk2dTextMesh>().ForceBuild();
			m_mainText.GetComponent<TextMeshShadow>().Refresh();
			if (string.IsNullOrEmpty(ci.m_text))
			{
				m_mainText.gameObject.SetActive(value: false);
			}
		}
		if (m_iconSprite != null)
		{
			m_iconSprite.SetSprite(ci.m_iconName);
		}
		if (m_layoutGrid != null)
		{
			m_layoutGrid.RepositionChildren();
		}
	}
}
