using BomberCrewCommon;
using Common;
using Rewired;
using UnityEngine;

public class NameWallScreen : MonoBehaviour
{
	[SerializeField]
	private tk2dUIItem m_toCrewStatueButton;

	[SerializeField]
	private UIScreen m_crewStatueScreen;

	[SerializeField]
	private AirbaseCameraNode m_thisCameraNode;

	[SerializeField]
	private AirbaseCameraNode[] m_cameraNodes;

	[SerializeField]
	private tk2dUIItem m_nextCameraButton;

	[SerializeField]
	private tk2dUIItem m_previousCameraButton;

	[SerializeField]
	private UISelectFinder m_finder;

	[SerializeField]
	private GameObject m_payRespectsHierarchy;

	private int m_targetCameraIndex;

	private float m_respectsHeldFor;

	private bool m_respectsPaid;

	private void Awake()
	{
		m_toCrewStatueButton.OnClick += ClickToCrewStatue;
		m_nextCameraButton.OnClick += ClickToNextCamera;
		m_previousCameraButton.OnClick += ClickToPreviousCamera;
	}

	private void ClickToCrewStatue()
	{
		Singleton<UIScreenManager>.Instance.ShowScreen(m_crewStatueScreen.name, showNavBarButtons: true);
	}

	private void ClickToNextCamera()
	{
		if (m_targetCameraIndex + 1 < m_cameraNodes.Length)
		{
			m_targetCameraIndex++;
			GoToTargetCamera();
		}
	}

	private void ClickToPreviousCamera()
	{
		if (m_targetCameraIndex - 1 >= 0)
		{
			m_targetCameraIndex--;
			GoToTargetCamera();
		}
	}

	private void Update()
	{
		if (ReInput.players.GetPlayer(0).GetButtonDown(27))
		{
			ClickToPreviousCamera();
		}
		if (ReInput.players.GetPlayer(0).GetButtonDown(28))
		{
			ClickToNextCamera();
		}
		if (Input.GetKeyDown(KeyCode.F))
		{
			m_payRespectsHierarchy.SetActive(value: false);
			m_payRespectsHierarchy.CustomActivate(active: true);
		}
		if (ReInput.players.GetPlayer(0).GetButton(48))
		{
			m_respectsHeldFor += Time.deltaTime;
			if (m_respectsHeldFor > 1.5f && !m_respectsPaid)
			{
				m_respectsPaid = true;
				m_payRespectsHierarchy.SetActive(value: false);
				m_payRespectsHierarchy.CustomActivate(active: true);
			}
		}
		else
		{
			m_respectsHeldFor = 0f;
		}
	}

	private void OnEnable()
	{
		m_targetCameraIndex = 0;
		Singleton<UISelector>.Instance.SetFinder(m_finder);
		GoToTargetCamera();
		Singleton<MainActionButtonMonitor>.Instance.AddListener(ButtonListener);
		m_payRespectsHierarchy.SetActive(value: false);
	}

	private void OnDisable()
	{
		Singleton<MainActionButtonMonitor>.Instance.RemoveListener(ButtonListener, invalidateCurrentPress: false);
		m_payRespectsHierarchy.SetActive(value: false);
	}

	private bool ButtonListener(MainActionButtonMonitor.ButtonPress bp)
	{
		if (bp == MainActionButtonMonitor.ButtonPress.Back)
		{
			ClickToCrewStatue();
		}
		return true;
	}

	private void GoToTargetCamera()
	{
		if (m_targetCameraIndex == 0)
		{
			m_previousCameraButton.gameObject.SetActive(value: false);
		}
		else if (!m_previousCameraButton.gameObject.activeInHierarchy)
		{
			m_previousCameraButton.gameObject.SetActive(value: true);
		}
		if (m_targetCameraIndex == m_cameraNodes.Length - 1)
		{
			m_nextCameraButton.gameObject.SetActive(value: false);
		}
		else if (!m_nextCameraButton.gameObject.activeInHierarchy)
		{
			m_nextCameraButton.gameObject.SetActive(value: true);
		}
		Singleton<AirbaseCameraController>.Instance.MoveCameraToLocation(m_cameraNodes[m_targetCameraIndex]);
	}
}
