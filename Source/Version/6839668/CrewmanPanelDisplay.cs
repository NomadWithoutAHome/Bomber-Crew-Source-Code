using BomberCrewCommon;
using Common;
using UnityEngine;

public class CrewmanPanelDisplay : MonoBehaviour
{
	[SerializeField]
	private tk2dTextMesh m_surnameText;

	[SerializeField]
	private tk2dUIItem m_uiItem;

	[SerializeField]
	private GameObject m_itemIndicatorParachute;

	[SerializeField]
	private GameObject m_itemIndicatorFirstAidKit;

	[SerializeField]
	private GameObject m_itemIndicatorExtinguisher;

	[SerializeField]
	private GameObject m_itemIndicatorAmmoBelt;

	[SerializeField]
	private GameObject m_selectHighlight;

	[SerializeField]
	private GameObject m_selectUpDownPrompt;

	[SerializeField]
	private GameObject m_selectArrow;

	[SerializeField]
	private GameObject m_hideAllHierarchy;

	[SerializeField]
	private GameObject m_speechHierarchy;

	[SerializeField]
	private tk2dTextMesh m_speechTextMesh;

	[SerializeField]
	private tk2dSprite m_stationSprite;

	[SerializeField]
	private GameObject m_alertIcon;

	[SerializeField]
	private CrewmanDualSkillDisplay m_skillDisplay;

	[SerializeField]
	private CrewmenIconsDisplay m_iconsDisplay;

	[SerializeField]
	private Renderer m_portraitRenderer;

	[SerializeField]
	private CrewmanHealthBar m_healthBarDisplay;

	[SerializeField]
	private GameObject m_bailedOutHierarchy;

	[SerializeField]
	private GameObject m_fellOffHierarchy;

	[SerializeField]
	private GameObject m_activityHierarchy;

	[SerializeField]
	private tk2dSprite m_activitySprite;

	[SerializeField]
	private tk2dRadialSprite m_activityRadial;

	[SerializeField]
	private GameObject m_noactivityHierarchy;

	[SerializeField]
	private GameObject m_selectIfSelectionModeActive;

	[SerializeField]
	private GameObject m_parachuteIndicator;

	[SerializeField]
	private GameObject m_firstAidIndicator;

	[SerializeField]
	private GameObject m_extinguisherIndicator;

	[SerializeField]
	private GameObject m_ammoBeltIndicator;

	private TextureRenderCamera m_renderCamera;

	private int m_index;

	private Crewman m_crewman;

	private CrewmanAvatar m_crewmanAvatar;

	private SpeechPrioritiser.SpeechRequestActive m_currentlyShowingSpeechReq;

	private float m_safeSpeechDisable;

	private int m_primarySkillLevelSet;

	private Material[] m_rendererMaterials;

	private bool m_hasSetConstants;

	private Color m_previousFlashColor;

	private bool m_showingParachute;

	private bool m_showingExtinguisher;

	private bool m_showingAmmoBelt;

	private bool m_showingFirstAidKit;

	private void Start()
	{
		m_skillDisplay.Refresh();
	}

	public void SetUpForCrewman(int index)
	{
		m_index = index;
		m_crewman = Singleton<CrewContainer>.Instance.GetCrewman(index);
		m_crewmanAvatar = Singleton<ContextControl>.Instance.GetAvatarFor(m_crewman);
		if (m_skillDisplay != null)
		{
			m_primarySkillLevelSet = m_crewman.GetPrimarySkill().GetLevel();
			m_skillDisplay.SetUp(m_crewman);
		}
		if (m_iconsDisplay != null)
		{
			m_iconsDisplay.SetUp(m_crewmanAvatar);
		}
		if (m_uiItem != null)
		{
			m_uiItem.OnClick += OnClick;
			m_uiItem.OnHoverOver += m_uiItem_OnHoverOver;
			m_uiItem.OnHoverOut += m_uiItem_OnHoverOut;
		}
		m_portraitRenderer.sharedMaterial = Object.Instantiate(m_portraitRenderer.sharedMaterial);
		m_portraitRenderer.sharedMaterial.mainTexture = m_crewman.GetPortraitPictureTexture();
		m_rendererMaterials = m_portraitRenderer.sharedMaterials;
		if (m_healthBarDisplay != null)
		{
			m_healthBarDisplay.SetUp(m_crewmanAvatar);
		}
	}

	private void OnDisable()
	{
		if (m_renderCamera != null)
		{
			m_renderCamera.DeregisterUse();
			m_renderCamera = null;
		}
	}

	private void m_uiItem_OnHoverOut()
	{
		m_crewmanAvatar.HoverOffPanel();
	}

	private void m_uiItem_OnHoverOver()
	{
		m_crewmanAvatar.HoverOnPanel();
	}

	private void Update()
	{
		if (Input.GetKeyDown((KeyCode)(49 + m_index)) && m_uiItem != null)
		{
			OnClick();
		}
		if (!m_hasSetConstants)
		{
			m_surnameText.text = m_crewman.GetSurname();
		}
		if (m_selectHighlight != null)
		{
			m_selectHighlight.SetActive(m_crewmanAvatar.IsOutlined());
		}
		if (Singleton<ContextControl>.Instance.GetCurrentlySelected() == m_crewmanAvatar)
		{
			ShowAsSelected(selected: true);
		}
		else
		{
			ShowAsSelected(selected: false);
		}
		if (m_primarySkillLevelSet != m_crewman.GetPrimarySkill().GetLevel() && m_skillDisplay != null)
		{
			m_skillDisplay.SetUp(m_crewman);
			m_primarySkillLevelSet = m_crewman.GetPrimarySkill().GetLevel();
		}
		if (m_selectIfSelectionModeActive != null)
		{
			m_selectIfSelectionModeActive.SetActive(Singleton<ContextControl>.Instance.IsCrewSelectMode());
		}
		SpeechPrioritiser.SpeechRequestActive currentSpeechFor = Singleton<SpeechPrioritiser>.Instance.GetCurrentSpeechFor(m_crewman);
		if (m_currentlyShowingSpeechReq != currentSpeechFor)
		{
			m_currentlyShowingSpeechReq = currentSpeechFor;
			if (m_currentlyShowingSpeechReq != null)
			{
				Singleton<Jabberer>.Instance.StartJabber(m_crewman.GetJabberEvent(), null, base.gameObject, Jabberer.JabberSettings.Normal);
				m_speechHierarchy.CustomActivate(active: true);
				string useText = m_currentlyShowingSpeechReq.GetUseText();
				if (string.IsNullOrEmpty(useText))
				{
					CrewmanAvatar useName = m_currentlyShowingSpeechReq.GetUseName();
					if (useName != null)
					{
						m_speechTextMesh.text = string.Format(m_currentlyShowingSpeechReq.GetText(), useName.GetCrewman().GetFirstName(), useName.GetCrewman().GetSurname());
					}
					else
					{
						m_speechTextMesh.text = m_currentlyShowingSpeechReq.GetText();
					}
				}
				else
				{
					m_speechTextMesh.text = string.Format(m_currentlyShowingSpeechReq.GetText(), useText);
				}
			}
			else
			{
				m_currentlyShowingSpeechReq = null;
				m_speechHierarchy.CustomActivate(active: false);
				m_safeSpeechDisable = 0f;
			}
		}
		if (m_currentlyShowingSpeechReq == null && m_speechHierarchy.activeInHierarchy)
		{
			m_safeSpeechDisable += Time.deltaTime;
			if (m_safeSpeechDisable > 5f)
			{
				m_safeSpeechDisable = 0f;
				m_speechHierarchy.CustomActivate(active: false);
			}
		}
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		bool flag4 = false;
		bool flag5 = false;
		if (m_crewmanAvatar != null)
		{
			Color currentColor = m_crewmanAvatar.GetFlashManager().GetCurrentColor();
			if (currentColor != m_previousFlashColor)
			{
				Material[] rendererMaterials = m_rendererMaterials;
				foreach (Material material in rendererMaterials)
				{
					material.SetColor("_FlashAddition", currentColor);
				}
				m_previousFlashColor = currentColor;
			}
			if (m_crewmanAvatar.IsBailedOut() && !m_crewman.IsDead())
			{
				if (m_crewmanAvatar.IsBailedOutNoParachute())
				{
					m_bailedOutHierarchy.SetActive(value: false);
					m_fellOffHierarchy.SetActive(value: true);
				}
				else
				{
					m_bailedOutHierarchy.SetActive(value: true);
					m_fellOffHierarchy.SetActive(value: false);
				}
			}
			else
			{
				m_bailedOutHierarchy.SetActive(value: false);
				m_fellOffHierarchy.SetActive(value: false);
			}
			float progress = 0f;
			string iconType = null;
			if (m_crewmanAvatar.IsActivityInProgress(out progress, out iconType))
			{
				m_noactivityHierarchy.SetActive(value: false);
				m_activityHierarchy.SetActive(value: true);
				if (!string.IsNullOrEmpty(iconType))
				{
					m_activitySprite.SetSprite(iconType);
				}
				else if (m_crewmanAvatar.HasStation())
				{
					Station station = m_crewmanAvatar.GetStation();
					m_activitySprite.SetSprite(station.GetUISpriteName());
				}
				m_activityRadial.SetValue(1f - progress);
				flag5 = true;
			}
			else
			{
				m_noactivityHierarchy.SetActive(value: true);
				m_activityHierarchy.SetActive(value: false);
			}
		}
		else
		{
			m_activityHierarchy.SetActive(value: false);
			Material[] rendererMaterials2 = m_rendererMaterials;
			foreach (Material material2 in rendererMaterials2)
			{
				material2.SetColor("_FlashAddition", new Color(0f, 0f, 0f, 0f));
			}
		}
		if (!m_crewmanAvatar.HasStation())
		{
			m_stationSprite.gameObject.SetActive(value: false);
			m_alertIcon.SetActive(value: false);
			if (!m_crewmanAvatar.IsBailedOut())
			{
				CarryableItem carriedItem = m_crewmanAvatar.GetCarriedItem();
				if (carriedItem != null)
				{
					switch (carriedItem.GetCarryableType())
					{
					case CarryableItem.CarryableTypeQuick.AmmoBelt:
						flag4 = true;
						break;
					case CarryableItem.CarryableTypeQuick.Extinguisher:
						flag = true;
						break;
					case CarryableItem.CarryableTypeQuick.FirstAidKit:
						flag3 = true;
						break;
					case CarryableItem.CarryableTypeQuick.Parachute:
						flag2 = true;
						break;
					}
				}
			}
		}
		else
		{
			Station station2 = m_crewmanAvatar.GetStation();
			string uISpriteName = station2.GetUISpriteName();
			if (!string.IsNullOrEmpty(uISpriteName))
			{
				m_stationSprite.gameObject.SetActive(value: true);
				m_stationSprite.SetSprite(uISpriteName);
			}
			else
			{
				m_stationSprite.gameObject.SetActive(value: false);
			}
			int num = 0;
			foreach (InMissionNotification notification in station2.GetNotifications().GetNotifications())
			{
				if (notification.m_urgency == StationNotificationUrgency.Red)
				{
					num++;
				}
			}
			m_alertIcon.SetActive(num > 0);
		}
		if (flag5)
		{
			flag4 = false;
			flag = false;
			flag3 = false;
			flag2 = false;
		}
		if (flag3 != m_showingFirstAidKit)
		{
			m_showingFirstAidKit = flag3;
			m_firstAidIndicator.SetActive(flag3);
		}
		if (flag2 != m_showingParachute)
		{
			m_showingParachute = flag2;
			m_parachuteIndicator.SetActive(flag2);
		}
		if (flag4 != m_showingAmmoBelt)
		{
			m_showingAmmoBelt = flag4;
			m_ammoBeltIndicator.SetActive(flag4);
		}
		if (flag != m_showingExtinguisher)
		{
			m_showingExtinguisher = flag;
			m_extinguisherIndicator.SetActive(flag);
		}
		m_hasSetConstants = true;
	}

	private void OnClick()
	{
		Crewman crewman = Singleton<CrewContainer>.Instance.GetCrewman(m_index);
		if (crewman != null)
		{
			Singleton<ContextControl>.Instance.SelectCrewman(crewman);
		}
	}

	private void OnDropClick()
	{
		Crewman crewman = Singleton<CrewContainer>.Instance.GetCrewman(m_index);
		if (crewman != null)
		{
			CrewmanAvatar avatarFor = Singleton<ContextControl>.Instance.GetAvatarFor(crewman);
			if (avatarFor != null)
			{
				avatarFor.DropItem();
			}
		}
	}

	private void ShowAsSelected(bool selected)
	{
		if (m_selectArrow != null)
		{
			m_selectArrow.SetActive(selected);
		}
	}
}
