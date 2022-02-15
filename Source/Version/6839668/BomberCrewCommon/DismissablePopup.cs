using UnityEngine;

namespace BomberCrewCommon;

[RequireComponent(typeof(UIPopUp))]
public class DismissablePopup : MonoBehaviour
{
	[SerializeField]
	private tk2dUIItem m_dismissButton;

	[SerializeField]
	private bool m_dismissOnAppClose;

	private void Start()
	{
		m_dismissButton.OnClick += HandleCloseButtonClick;
	}

	private void HandleCloseButtonClick()
	{
		GetComponent<UIPopUp>().DismissPopup();
	}

	private void OnDestroy()
	{
		m_dismissButton.OnClick -= HandleCloseButtonClick;
	}

	private void OnApplicationPause(bool paused)
	{
		if (m_dismissOnAppClose && paused)
		{
			GetComponent<UIPopUp>().DismissPopup();
		}
	}
}
