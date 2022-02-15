using BomberCrewCommon;
using UnityEngine;

public class MemorialNavigation : MonoBehaviour
{
	[SerializeField]
	private UIScreen m_startScreen;

	[SerializeField]
	private UIScreen m_startScreenDemoMode;

	private void Start()
	{
		Singleton<UIScreenManager>.Instance.ShowScreen(m_startScreen.name, showNavBarButtons: false);
	}
}
