using BomberCrewCommon;
using UnityEngine;

public class InteractiveItemHotkeyDisplay : MonoBehaviour
{
	[SerializeField]
	private TextSetter m_textSetter;

	[SerializeField]
	private GameObject m_enableHierarchy;

	[SerializeField]
	private WorldToScreenTracker m_tracker;

	[SerializeField]
	private GameObject m_keyboardObject;

	[SerializeField]
	private Vector3 m_offsetMouse;

	[SerializeField]
	private Vector3 m_offsetController;

	[SerializeField]
	private Transform m_offsetTransform;

	[SerializeField]
	private GameObject m_backgroundDefault;

	[SerializeField]
	private GameObject m_backgroundCritical;

	private bool m_controllerMode;

	private bool m_hasEverBeenSet;

	private bool m_shouldBeVisible;

	private string m_textKeyMouse;

	private string m_textController;

	public void SetUp(string textKeyMouse, string textController, Transform toTrack, tk2dCamera camera, bool isCritical)
	{
		m_textKeyMouse = ControlPromptDisplayHelpers.ConvertString(textKeyMouse);
		m_textController = ControlPromptDisplayHelpers.ConvertString(textController);
		m_tracker.SetTracking(toTrack, camera);
		if (isCritical)
		{
			m_backgroundDefault.SetActive(value: false);
			m_backgroundCritical.SetActive(value: true);
		}
		else
		{
			m_backgroundDefault.SetActive(value: true);
			m_backgroundCritical.SetActive(value: false);
		}
		Refresh();
	}

	private void Refresh()
	{
		bool flag = Singleton<UISelector>.Instance.IsPrimary();
		if (!m_hasEverBeenSet || m_controllerMode != flag)
		{
			m_hasEverBeenSet = true;
			m_controllerMode = flag;
			if (!m_controllerMode)
			{
				if (!string.IsNullOrEmpty(m_textKeyMouse))
				{
					m_textSetter.SetText(m_textKeyMouse);
					m_keyboardObject.SetActive(value: true);
					m_shouldBeVisible = true;
					m_offsetTransform.localPosition = m_offsetMouse;
				}
				else
				{
					m_shouldBeVisible = false;
				}
			}
			else if (!string.IsNullOrEmpty(m_textController))
			{
				m_textSetter.SetText(m_textController);
				m_keyboardObject.SetActive(value: false);
				m_shouldBeVisible = true;
				m_offsetTransform.localPosition = m_offsetController;
			}
			else
			{
				m_shouldBeVisible = false;
			}
		}
		if (Singleton<BomberCamera>.Instance.ShouldShowCrewmenOverlays())
		{
			if (m_controllerMode)
			{
				m_enableHierarchy.SetActive(Singleton<ContextControl>.Instance.IsMovementMode() && m_shouldBeVisible);
			}
			else
			{
				m_enableHierarchy.SetActive(true && m_shouldBeVisible);
			}
		}
		else
		{
			m_enableHierarchy.SetActive(value: false);
		}
	}

	private void Update()
	{
		Refresh();
	}
}
