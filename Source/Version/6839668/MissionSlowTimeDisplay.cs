using BomberCrewCommon;
using UnityEngine;

public class MissionSlowTimeDisplay : MonoBehaviour
{
	private enum DisplayState
	{
		Ready,
		Active,
		Charging
	}

	[SerializeField]
	private tk2dRadialSprite[] m_fillSprites;

	[SerializeField]
	private GameObject m_effectCharge;

	[SerializeField]
	private GameObject m_effectActive;

	[SerializeField]
	private GameObject m_effectReady;

	[SerializeField]
	private GameObject m_enabledHierarchy;

	private bool m_activeStateSet;

	private bool m_chargingStateSet;

	private bool m_readyStateSet = true;

	private DisplayState m_state;

	private void Update()
	{
		m_enabledHierarchy.SetActive(!Singleton<SystemDataContainer>.Instance.Get().BlockSlowDown());
		if (Singleton<MissionSpeedControls>.Instance.IsSlowdownActive())
		{
			m_state = DisplayState.Active;
		}
		else if (Singleton<MissionSpeedControls>.Instance.GetSlowdownAmountNormalised() < 1f && !Singleton<MissionSpeedControls>.Instance.IsSlowdownActive())
		{
			m_state = DisplayState.Charging;
		}
		else
		{
			m_state = DisplayState.Ready;
		}
		switch (m_state)
		{
		case DisplayState.Active:
			if (!m_activeStateSet)
			{
				m_effectActive.SetActive(value: true);
				m_effectCharge.SetActive(value: false);
				m_effectReady.SetActive(value: false);
				m_activeStateSet = true;
				m_chargingStateSet = false;
				m_readyStateSet = false;
			}
			UpdateFillSprites();
			break;
		case DisplayState.Charging:
			if (!m_chargingStateSet)
			{
				m_effectActive.SetActive(value: false);
				m_effectCharge.SetActive(value: true);
				m_effectReady.SetActive(value: false);
				m_activeStateSet = false;
				m_chargingStateSet = true;
				m_readyStateSet = false;
			}
			UpdateFillSprites();
			break;
		case DisplayState.Ready:
			if (!m_readyStateSet)
			{
				m_effectActive.SetActive(value: false);
				m_effectCharge.SetActive(value: false);
				m_effectReady.SetActive(value: true);
				m_activeStateSet = false;
				m_chargingStateSet = false;
				m_readyStateSet = true;
			}
			break;
		}
	}

	private void UpdateFillSprites()
	{
		tk2dRadialSprite[] fillSprites = m_fillSprites;
		foreach (tk2dRadialSprite tk2dRadialSprite2 in fillSprites)
		{
			if (tk2dRadialSprite2.enabled && tk2dRadialSprite2.gameObject.activeInHierarchy)
			{
				tk2dRadialSprite2.SetValue(1f - Singleton<MissionSpeedControls>.Instance.GetSlowdownAmountNormalised());
			}
		}
	}
}
