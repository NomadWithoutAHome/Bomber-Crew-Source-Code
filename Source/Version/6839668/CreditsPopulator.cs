using System;
using UnityEngine;
using WingroveAudio;

public class CreditsPopulator : MonoBehaviour
{
	public enum CreditItemType
	{
		Logo,
		Entry
	}

	[Serializable]
	public class CreditsItem
	{
		[SerializeField]
		public CreditItemType m_itemType;

		[SerializeField]
		public GameObject m_prefabOverride;

		[SerializeField]
		public string m_title;

		[SerializeField]
		[TextArea]
		public string m_text;

		[SerializeField]
		[TextArea]
		public string m_smallText;

		[SerializeField]
		public string m_iconName;
	}

	[SerializeField]
	private CreditsItem[] m_allCredits;

	[SerializeField]
	private GameObject m_logoItemPrefab;

	[SerializeField]
	private GameObject m_entryItemPrefab;

	[SerializeField]
	private LayoutGrid m_layoutGrid;

	[SerializeField]
	private GameObject m_fader;

	[SerializeField]
	private float m_yPosFaderEnable;

	private Transform m_finalTransform;

	private bool m_faderIsStarted;

	private void Start()
	{
		CreditsItem[] allCredits = m_allCredits;
		foreach (CreditsItem creditsItem in allCredits)
		{
			GameObject gameObject = creditsItem.m_prefabOverride;
			if (gameObject == null)
			{
				gameObject = ((creditsItem.m_itemType != CreditItemType.Entry) ? m_logoItemPrefab : m_entryItemPrefab);
			}
			GameObject gameObject2 = UnityEngine.Object.Instantiate(gameObject);
			gameObject2.GetComponent<CreditsEntryItem>().SetUp(creditsItem);
			gameObject2.transform.parent = m_layoutGrid.transform;
			m_finalTransform = gameObject2.transform;
		}
		m_layoutGrid.RepositionChildren();
	}

	private void Update()
	{
		if (!m_faderIsStarted && m_finalTransform.position.y > m_yPosFaderEnable)
		{
			WingroveRoot.Instance.PostEvent("MUSIC_STOP");
			m_fader.SetActive(value: true);
			m_faderIsStarted = true;
		}
	}

	public bool IsDone()
	{
		if (m_finalTransform == null)
		{
			return false;
		}
		if (m_finalTransform.position.y > 1500f)
		{
			return true;
		}
		return false;
	}
}
