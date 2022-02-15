using BomberCrewCommon;
using UnityEngine;

public class ControlPromptDisplay : MonoBehaviour
{
	[SerializeField]
	private GameObject m_controllerHierarchy;

	[SerializeField]
	private GameObject m_mouseHierarchy;

	private void Awake()
	{
		Update();
	}

	public void Refresh()
	{
		Update();
	}

	private void Update()
	{
		if (m_controllerHierarchy != null)
		{
			m_controllerHierarchy.SetActive(Singleton<UISelector>.Instance.IsPrimary());
		}
		if (m_mouseHierarchy != null)
		{
			m_mouseHierarchy.SetActive(!Singleton<UISelector>.Instance.IsPrimary());
		}
	}
}
