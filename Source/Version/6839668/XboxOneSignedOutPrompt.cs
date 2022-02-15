using BomberCrewCommon;
using Rewired;
using UnityEngine;
using WingroveAudio;

public class XboxOneSignedOutPrompt : MonoBehaviour
{
	[SerializeField]
	private GameObject m_mainWindowHierarchy;

	[SerializeField]
	private GameObject m_secondPromptHierarchy;

	[SerializeField]
	private TextSetter m_mainWindowTextSetter;

	private float m_prevTimeScale;

	private int m_stage;

	private bool m_pausedOnEnable;

	private UISelector m_uiSelectorOnEnable;

	private InputLayerInterface m_inputLayerInterfaceOnEnable;

	private void OnEnable()
	{
		m_mainWindowHierarchy.SetActive(value: true);
		m_secondPromptHierarchy.SetActive(value: false);
		m_stage = 0;
		m_prevTimeScale = Time.timeScale;
		WingroveRoot.Instance.PostEvent("PAUSE_GAME");
		m_pausedOnEnable = Singleton<GameFlow>.Instance.IsPaused();
		Singleton<GameFlow>.Instance.SetPaused(paused: true);
		Time.timeScale = 0f;
		Singleton<InputLayerInterface>.Instance.DisableAllLayers();
		Singleton<UISelector>.Instance.Pause();
		m_uiSelectorOnEnable = Singleton<UISelector>.Instance;
		m_inputLayerInterfaceOnEnable = Singleton<InputLayerInterface>.Instance;
		Singleton<MainActionButtonMonitor>.Instance.AddListener(ButtonListener);
	}

	private void OnDisable()
	{
		Singleton<GameFlow>.Instance.SetPaused(m_pausedOnEnable);
		Time.timeScale = m_prevTimeScale;
		WingroveRoot.Instance.PostEvent("UNPAUSE_GAME");
		Singleton<InputLayerInterface>.Instance.EnableAllLayers();
		Singleton<UISelector>.Instance.Resume();
		Singleton<MainActionButtonMonitor>.Instance.RemoveListener(ButtonListener, invalidateCurrentPress: true);
	}

	private bool ButtonListener(MainActionButtonMonitor.ButtonPress bp)
	{
		return true;
	}

	private void Update()
	{
		if (Singleton<UISelector>.Instance != m_uiSelectorOnEnable && Singleton<UISelector>.Instance != null)
		{
			Singleton<UISelector>.Instance.Pause();
			m_uiSelectorOnEnable = Singleton<UISelector>.Instance;
		}
		if (Singleton<InputLayerInterface>.Instance != m_inputLayerInterfaceOnEnable && Singleton<InputLayerInterface>.Instance != null)
		{
			Singleton<InputLayerInterface>.Instance.DisableAllLayers();
			m_inputLayerInterfaceOnEnable = Singleton<InputLayerInterface>.Instance;
		}
		if (ReInput.players.GetPlayer(0).GetButtonUp(2))
		{
			if (m_stage == 0)
			{
				m_stage = 1;
				m_mainWindowHierarchy.SetActive(value: false);
				m_secondPromptHierarchy.SetActive(value: true);
			}
			else
			{
				Singleton<GameFlow>.Instance.ResetToMainMenu();
				base.gameObject.SetActive(value: false);
			}
		}
		if (ReInput.players.GetPlayer(0).GetButtonUp(3) && m_stage == 1)
		{
			m_stage = 0;
			m_mainWindowHierarchy.SetActive(value: true);
			m_secondPromptHierarchy.SetActive(value: false);
		}
	}
}
