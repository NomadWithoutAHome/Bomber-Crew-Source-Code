using BomberCrewCommon;
using UnityEngine;

public class EngagementScreen : MonoBehaviour
{
	private bool m_hasTransferred;

	[SerializeField]
	private Animation m_cameraAnimation;

	[SerializeField]
	private GameObject m_pressAToStartHierarchy;

	private int m_numGamepads = -1;

	private void OnDisable()
	{
	}

	private void OnEnable()
	{
		m_hasTransferred = false;
		m_pressAToStartHierarchy.SetActive(value: true);
		Singleton<CrewStatue>.Instance.ResetForNoProfile();
	}

	private void HidePressAToStart()
	{
		m_pressAToStartHierarchy.SetActive(value: false);
	}

	private void Update()
	{
	}

	private void GoToNextScreen()
	{
		m_hasTransferred = true;
		if (Singleton<SystemDataContainer>.Instance.Get().HasEverSetLanguage())
		{
			if (m_cameraAnimation != null)
			{
				m_cameraAnimation.Stop();
				m_cameraAnimation.Play();
			}
			Singleton<UIScreenManager>.Instance.ShowScreen("StartScreen", showNavBarButtons: true);
		}
		else
		{
			Singleton<UIScreenManager>.Instance.ShowScreen("LanguageSelectionScreen", showNavBarButtons: true);
		}
	}
}
