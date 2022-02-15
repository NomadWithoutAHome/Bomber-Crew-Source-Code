using System;
using BomberCrewCommon;
using UnityEngine;

public class DLCShowArea : MonoBehaviour
{
	[Serializable]
	public class ExpectedDLC
	{
		[SerializeField]
		public string m_dlcPackName;

		[SerializeField]
		public string m_dlcNamedText;

		[SerializeField]
		public Texture2D m_dlcSmallTexture;

		[SerializeField]
		public Texture2D m_dlcLargeTexture;

		[SerializeField]
		public uint m_appId;

		[SerializeField]
		public bool m_showOnConsole;
	}

	[SerializeField]
	private LayoutGrid m_layoutGrid;

	[SerializeField]
	private GameObject m_dlcButton;

	[SerializeField]
	private ExpectedDLC[] m_dlcPacksToDisplay;

	[SerializeField]
	private GameObject m_dlcPopup;

	private void Start()
	{
		ExpectedDLC[] dlcPacksToDisplay = m_dlcPacksToDisplay;
		foreach (ExpectedDLC expectedDLC in dlcPacksToDisplay)
		{
			if (true)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(m_dlcButton);
				gameObject.transform.parent = m_layoutGrid.transform;
				gameObject.transform.position = Vector3.zero;
				ExpectedDLC toShow = expectedDLC;
				gameObject.GetComponent<DLCShowDetailsButton>().SetUp(expectedDLC, delegate
				{
					ShowDLCPopup(toShow);
				});
			}
		}
		m_layoutGrid.RepositionChildren();
	}

	private void ShowDLCPopup(ExpectedDLC ed)
	{
		UIPopupData uIPopupData = new UIPopupData();
		uIPopupData.PopupStartCallback = (Action<UIPopUp>)Delegate.Combine(uIPopupData.PopupStartCallback, (Action<UIPopUp>)delegate(UIPopUp uip)
		{
			uip.GetComponent<DLCShowDetailsPopup>().SetUp(ed);
		});
		Singleton<UIPopupManager>.Instance.DisplayPopup(m_dlcPopup, uIPopupData);
	}
}
