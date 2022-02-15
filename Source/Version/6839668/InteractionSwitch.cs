using System;
using BomberCrewCommon;
using UnityEngine;

public class InteractionSwitch : MonoBehaviour
{
	[SerializeField]
	private InteractiveItem m_item;

	[SerializeField]
	private GameObject m_hideWhenCrewmanSelected;

	private GameObject m_interactiveItemCached;

	private void OnEnable()
	{
		if (Singleton<GameFlow>.Instance.GetIsInMissionProbable())
		{
			if (Singleton<ContextControl>.Instance != null)
			{
				m_interactiveItemCached = UnityEngine.Object.Instantiate(Singleton<EnumToIconMapping>.Instance.GetInteractionPrefab());
				m_interactiveItemCached.transform.parent = base.transform;
				m_interactiveItemCached.GetComponent<UIInteractionMarker>().SetInitial(m_item);
				m_interactiveItemCached.SetActive(value: false);
			}
			if (Singleton<ContextControl>.Instance != null)
			{
				ContextControl instance = Singleton<ContextControl>.Instance;
				instance.OnContextChange = (Action<CrewmanAvatar>)Delegate.Combine(instance.OnContextChange, new Action<CrewmanAvatar>(ContextChange));
			}
		}
	}

	private void OnDisable()
	{
		if (Singleton<GameFlow>.Instance != null && Singleton<GameFlow>.Instance.GetIsInMissionProbable() && m_interactiveItemCached != null)
		{
			UnityEngine.Object.Destroy(m_interactiveItemCached);
			m_interactiveItemCached = null;
			if (Singleton<ContextControl>.Instance != null)
			{
				ContextControl instance = Singleton<ContextControl>.Instance;
				instance.OnContextChange = (Action<CrewmanAvatar>)Delegate.Remove(instance.OnContextChange, new Action<CrewmanAvatar>(ContextChange));
			}
		}
	}

	private void ContextChange(CrewmanAvatar selectedCrewman)
	{
		if (m_hideWhenCrewmanSelected != null)
		{
			if (selectedCrewman == null)
			{
				m_hideWhenCrewmanSelected.SetActive(value: true);
			}
			else
			{
				m_hideWhenCrewmanSelected.SetActive(value: false);
			}
		}
		if (selectedCrewman != null)
		{
			m_interactiveItemCached.SetActive(value: true);
			m_interactiveItemCached.GetComponent<UIInteractionMarker>().SetCrewman(selectedCrewman);
		}
		else
		{
			m_interactiveItemCached.SetActive(value: false);
		}
	}
}
