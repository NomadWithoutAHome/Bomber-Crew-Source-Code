using BomberCrewCommon;
using Common;
using UnityEngine;

public class UIScreenManager : Singleton<UIScreenManager>
{
	[SerializeField]
	private UIScreen[] m_displayableScreens;

	[SerializeField]
	private UIPopUp[] m_displayablePopUps;

	[SerializeField]
	private GameObject m_popUpCommon;

	private void Awake()
	{
		UIScreen[] displayableScreens = m_displayableScreens;
		foreach (UIScreen uIScreen in displayableScreens)
		{
			uIScreen.gameObject.SetActive(value: false);
		}
	}

	public GameObject ShowScreen(string screenName, bool showNavBarButtons)
	{
		GameObject result = null;
		UIScreen[] displayableScreens = m_displayableScreens;
		foreach (UIScreen uIScreen in displayableScreens)
		{
			if (uIScreen.name == screenName)
			{
				result = uIScreen.gameObject;
				uIScreen.gameObject.CustomActivate(active: true);
			}
			else
			{
				uIScreen.gameObject.CustomActivate(active: false);
			}
		}
		return result;
	}

	public bool IsScreenActive(string screenName)
	{
		UIScreen[] displayableScreens = m_displayableScreens;
		foreach (UIScreen uIScreen in displayableScreens)
		{
			if (uIScreen.name == screenName && uIScreen.gameObject.activeInHierarchy)
			{
				return true;
			}
		}
		return false;
	}
}
