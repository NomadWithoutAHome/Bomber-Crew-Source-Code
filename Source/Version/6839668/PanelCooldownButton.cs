using System;
using Common;
using UnityEngine;

public class PanelCooldownButton : MonoBehaviour
{
	[SerializeField]
	private GameObject m_readyHierarchy;

	[SerializeField]
	private GameObject m_chargingHierarchy;

	[SerializeField]
	private tk2dUIProgressBar m_expendFillDisplay;

	[SerializeField]
	private tk2dUIProgressBar m_refillDisplay;

	[SerializeField]
	private GameObject m_activeHierarchy;

	[SerializeField]
	private GameObject m_disabledHierarchy;

	[SerializeField]
	private tk2dUIItem m_button;

	private bool m_isActive;

	private bool m_isReady = true;

	private bool m_isDisabled;

	private bool m_isCharging;

	private bool m_hasEverSet;

	public event Action OnClick;

	private void Awake()
	{
		m_button.OnClick += OnButtonClick;
		m_activeHierarchy.SetActive(value: false);
		m_readyHierarchy.SetActive(value: true);
		m_disabledHierarchy.SetActive(value: false);
		m_chargingHierarchy.SetActive(value: false);
		m_hasEverSet = false;
	}

	private void OnButtonClick()
	{
		if (this.OnClick != null)
		{
			this.OnClick();
		}
	}

	public void SetStatus(bool isActive, bool isReady, bool isDisabled, float refill, float useFill)
	{
		bool flag = !isActive && !isReady;
		if (flag != m_isCharging || !m_hasEverSet)
		{
			m_isCharging = flag;
			m_chargingHierarchy.CustomActivate(flag);
		}
		if (isActive != m_isActive || !m_hasEverSet)
		{
			m_isActive = isActive;
			m_activeHierarchy.CustomActivate(isActive);
		}
		if (isReady != m_isReady || !m_hasEverSet)
		{
			m_isReady = isReady;
			m_readyHierarchy.CustomActivate(isReady);
		}
		if (isDisabled != m_isDisabled || !m_hasEverSet)
		{
			m_isDisabled = isDisabled;
			m_disabledHierarchy.CustomActivate(isDisabled);
		}
		m_expendFillDisplay.Value = Mathf.Clamp01(useFill);
		m_refillDisplay.Value = Mathf.Clamp01(refill);
		m_hasEverSet = true;
	}
}
