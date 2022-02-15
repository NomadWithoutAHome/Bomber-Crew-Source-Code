using System;
using UnityEngine;

public class RecruitmentCrewmanAvatar : MonoBehaviour
{
	[SerializeField]
	private tk2dUIItem m_uiItem;

	[SerializeField]
	private Collider m_interactionCollider;

	[SerializeField]
	private CrewmanGraphicsInstantiate m_crewmanGraphicsInstantiate;

	private CrewmanGraphics m_crewmanGraphics;

	private Crewman m_crewman;

	private FlashManager.ActiveFlash m_selectedFlash;

	public event Action OnClick;

	private void Awake()
	{
		m_uiItem.OnClick += OnCrewmanClick;
	}

	public void SetSelected(bool isSelected)
	{
	}

	private void OnCrewmanClick()
	{
		if (this.OnClick != null)
		{
			this.OnClick();
		}
	}

	public Crewman GetCrewman()
	{
		return m_crewman;
	}

	public void SetUp(Crewman crewman)
	{
		m_crewman = crewman;
		m_crewmanGraphicsInstantiate.Init(m_crewman);
		m_crewmanGraphics = m_crewmanGraphicsInstantiate.GetCrewmanGraphics();
		m_crewmanGraphics.SetFromCrewman(m_crewman);
		m_crewmanGraphics.SetUpClothingGraphics(m_crewman.GetCivilianClothing());
	}

	public void SetInteractive(bool interactive)
	{
		m_interactionCollider.enabled = interactive;
	}
}
