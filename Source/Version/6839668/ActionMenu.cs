using System;
using BomberCrewCommon;
using UnityEngine;

public class ActionMenu : MonoBehaviour
{
	[SerializeField]
	private GameObject[] m_optionHierarchies;

	[SerializeField]
	private tk2dTextMesh[] m_labels;

	[SerializeField]
	private tk2dUIItem[] m_items;

	[SerializeField]
	private HighlightOnOver[] m_highlightAnimators;

	[SerializeField]
	private tk2dSpriteAnimator[] m_progressAnimators;

	[SerializeField]
	private tk2dUIProgressBar[] m_progressBars;

	private InteractiveItem.Interaction[] m_options;

	private InteractiveItem m_item;

	private CrewmanAvatar m_currentCrewman;

	public void SetUp(InteractiveItem.Interaction[] options, InteractiveItem item, CrewmanAvatar crewmanAvatar)
	{
		m_currentCrewman = crewmanAvatar;
		tk2dSpriteAnimator[] progressAnimators = m_progressAnimators;
		foreach (tk2dSpriteAnimator tk2dSpriteAnimator2 in progressAnimators)
		{
			tk2dSpriteAnimator2.Play($"Progress{crewmanAvatar.GetIndex() + 1:00}");
		}
		HighlightOnOver[] highlightAnimators = m_highlightAnimators;
		foreach (HighlightOnOver highlightOnOver in highlightAnimators)
		{
			highlightOnOver.SetHoverColour(m_currentCrewman.GetIndex() + 1);
		}
		m_options = options;
		m_item = item;
		GameObject[] optionHierarchies = m_optionHierarchies;
		foreach (GameObject gameObject in optionHierarchies)
		{
			gameObject.SetActive(value: false);
		}
		int num = 0;
		foreach (InteractiveItem.Interaction interaction in options)
		{
			m_optionHierarchies[num].SetActive(value: true);
			m_labels[num].text = interaction.m_description;
			m_items[num].OnClickUIItem += ActionMenu_OnClickUIItem;
			num++;
		}
	}

	private void SetDormant()
	{
		Collider[] componentsInChildren = GetComponentsInChildren<Collider>(includeInactive: true);
		foreach (Collider collider in componentsInChildren)
		{
			collider.enabled = false;
		}
	}

	private void OnContractFinished(InteractiveItem.InteractionContract ic)
	{
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private void OnContractUpdated(InteractiveItem.InteractionContract ic)
	{
		tk2dUIProgressBar[] progressBars = m_progressBars;
		foreach (tk2dUIProgressBar tk2dUIProgressBar2 in progressBars)
		{
			tk2dUIProgressBar2.Value = ic.GetValue();
		}
	}

	public void DoAction(int index)
	{
		InteractiveItem.InteractionContract interactionContract = null;
		interactionContract = Singleton<ContextControl>.Instance.DoInteraction(m_options[index], m_item);
		if (interactionContract != null)
		{
			HighlightOnOver[] highlightAnimators = m_highlightAnimators;
			foreach (HighlightOnOver highlightOnOver in highlightAnimators)
			{
				highlightOnOver.SetOn();
			}
			InteractiveItem.InteractionContract interactionContract2 = interactionContract;
			interactionContract2.OnContractFinish = (Action<InteractiveItem.InteractionContract>)Delegate.Combine(interactionContract2.OnContractFinish, new Action<InteractiveItem.InteractionContract>(OnContractFinished));
			InteractiveItem.InteractionContract interactionContract3 = interactionContract;
			interactionContract3.OnContractUpdate = (Action<InteractiveItem.InteractionContract>)Delegate.Combine(interactionContract3.OnContractUpdate, new Action<InteractiveItem.InteractionContract>(OnContractUpdated));
			SetDormant();
			for (int j = 0; j < m_items.Length; j++)
			{
				m_optionHierarchies[j].SetActive(j == index);
			}
		}
		else
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private void ActionMenu_OnClickUIItem(tk2dUIItem obj)
	{
		int num = 0;
		InteractiveItem.InteractionContract interactionContract = null;
		InteractiveItem.Interaction[] options = m_options;
		foreach (InteractiveItem.Interaction interaction in options)
		{
			if (m_items[num] == obj)
			{
				DoAction(num);
				break;
			}
			num++;
		}
	}
}
