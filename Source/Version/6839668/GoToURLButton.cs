using Steamworks;
using UnityEngine;

public class GoToURLButton : MonoBehaviour
{
	[SerializeField]
	private tk2dUIItem m_button;

	[SerializeField]
	private string m_url;

	[SerializeField]
	private bool m_showInSteam;

	private void Start()
	{
		m_button.OnClick += LaunchURL;
	}

	private void LaunchURL()
	{
		bool flag = m_showInSteam;
		if (!SteamManager.Initialized)
		{
			flag = false;
		}
		if (flag)
		{
			SteamFriends.ActivateGameOverlayToWebPage(m_url);
		}
		else
		{
			Application.OpenURL(m_url);
		}
	}
}
