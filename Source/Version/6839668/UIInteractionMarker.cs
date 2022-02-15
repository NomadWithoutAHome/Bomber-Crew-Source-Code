using BomberCrewCommon;
using Rewired;
using UnityEngine;

public class UIInteractionMarker : MonoBehaviour
{
	[SerializeField]
	private tk2dUIItem m_teardropInteractionClick;

	[SerializeField]
	private tk2dBaseSprite[] m_sprites;

	[SerializeField]
	private GameObject m_teardropInteractionEnable;

	[SerializeField]
	private OutlineMesh m_teardropOutline;

	[SerializeField]
	private Material[] m_materials;

	[SerializeField]
	private Renderer m_teardropRenderer;

	[SerializeField]
	private GameObject m_hotkeyDisplayPrefab;

	[SerializeField]
	private Collider m_teardropCollider;

	private InteractiveItem m_interactiveItem;

	private CrewmanAvatar m_crewman;

	private bool m_crewmanIsNull;

	private GameObject m_currentHotkeyDisplay;

	private int m_hovered;

	private bool m_hasEverSet;

	private bool m_tearDropInteractionMarkerShown;

	private EnumToIconMapping.InteractionOrAlertType m_shownIcon;

	private bool m_colliderIsEnabled = true;

	private void Awake()
	{
		m_teardropInteractionClick.OnClick += MainAreaClick_OnClick;
		m_teardropInteractionClick.OnHoverOver += MainAreaClick_OnHoverOver;
		m_teardropInteractionClick.OnHoverOut += MainAreaClick_OnHoverOut;
		m_teardropInteractionEnable.SetActive(value: false);
		m_crewmanIsNull = true;
	}

	private void MainAreaClick_OnHoverOut()
	{
		if (base.gameObject.activeInHierarchy)
		{
			m_hovered--;
			if (m_hovered == 0)
			{
				m_interactiveItem.HoverOver(hover: false);
				m_teardropOutline.GetClonedRenderer().enabled = false;
			}
		}
		else
		{
			m_hovered = 0;
			m_interactiveItem.HoverOver(hover: false);
			m_teardropOutline.GetClonedRenderer().enabled = false;
		}
	}

	private void MainAreaClick_OnHoverOver()
	{
		if (base.gameObject.activeInHierarchy)
		{
			m_hovered++;
			if (GetInteraction() != null)
			{
				m_interactiveItem.HoverOver(hover: true);
				m_teardropOutline.GetClonedRenderer().enabled = true;
			}
		}
	}

	private InteractiveItem.Interaction GetInteraction()
	{
		if (m_interactiveItem != null)
		{
			if (m_crewmanIsNull)
			{
				return null;
			}
			return m_interactiveItem.GetInteractionOptionsPublic(m_crewman, skipNullCheck: true);
		}
		return null;
	}

	private void DoInteraction(InteractiveItem.Interaction ii)
	{
		Singleton<ContextControl>.Instance.DoInteraction(ii, m_interactiveItem);
	}

	private void OnEnable()
	{
		Singleton<ContextControl>.Instance.RegisterUIInteractionMarker(this);
	}

	private void OnDisable()
	{
		if (Singleton<ContextControl>.Instance != null)
		{
			Singleton<ContextControl>.Instance.DeRegisterUIInteractionMarker(this);
		}
		if (m_currentHotkeyDisplay != null)
		{
			Object.Destroy(m_currentHotkeyDisplay);
		}
	}

	private void OnDestroy()
	{
		m_interactiveItem.HoverOver(hover: false);
		if (m_interactiveItem.GetInteractionItem() != null)
		{
			m_interactiveItem.GetInteractionItem().OnClick -= MainAreaClick_OnClick;
			m_interactiveItem.GetInteractionItem().OnHoverOver -= MainAreaClick_OnHoverOver;
			m_interactiveItem.GetInteractionItem().OnHoverOut -= MainAreaClick_OnHoverOut;
		}
		if (m_currentHotkeyDisplay != null)
		{
			Object.Destroy(m_currentHotkeyDisplay);
		}
	}

	private void MainAreaClick_OnClick()
	{
		InteractiveItem.Interaction interaction = GetInteraction();
		if (interaction != null)
		{
			DoInteraction(interaction);
		}
	}

	public void SetInitial(InteractiveItem item)
	{
		m_interactiveItem = item;
		tk2dUIItem interactionItem = m_interactiveItem.GetInteractionItem();
		if (interactionItem != null)
		{
			interactionItem.OnClick += MainAreaClick_OnClick;
			interactionItem.OnHoverOver += MainAreaClick_OnHoverOver;
			interactionItem.OnHoverOut += MainAreaClick_OnHoverOut;
		}
	}

	public void SetCrewman(CrewmanAvatar crewman)
	{
		m_crewman = crewman;
		if (m_crewman == null)
		{
			m_crewmanIsNull = true;
		}
		else
		{
			m_crewmanIsNull = false;
		}
		m_hovered = 0;
		m_interactiveItem.HoverOver(hover: false);
		m_teardropOutline.GetClonedRenderer().enabled = false;
	}

	public InteractiveItem GetRelatedItem()
	{
		return m_interactiveItem;
	}

	public tk2dUIItem GetTeardrop()
	{
		return m_teardropInteractionClick;
	}

	private void Update()
	{
		if (m_interactiveItem == null)
		{
			base.gameObject.SetActive(value: false);
			if (m_currentHotkeyDisplay != null)
			{
				Object.Destroy(m_currentHotkeyDisplay);
			}
			return;
		}
		base.transform.SetPositionAndRotation(m_interactiveItem.GetMovementArrowTransform().position, Quaternion.identity);
		InteractiveItem.Interaction interaction = null;
		if (!m_crewmanIsNull)
		{
			interaction = m_interactiveItem.GetInteractionOptionsPublic(m_crewman, skipNullCheck: true);
		}
		EnumToIconMapping.InteractionOrAlertType interactionOrAlertType = EnumToIconMapping.InteractionOrAlertType.Main;
		bool flag = true;
		if (interaction != null)
		{
			interactionOrAlertType = interaction.m_iconType;
			if (interaction.m_hasExtraHoverIconType && m_hovered != 0)
			{
				interactionOrAlertType = interaction.m_iconTypeHovered;
				flag = false;
			}
		}
		if (interaction != null && interactionOrAlertType != 0 && !m_interactiveItem.IsAnyInteractionInProgress(m_crewman))
		{
			if (!m_hasEverSet || !m_tearDropInteractionMarkerShown)
			{
				m_tearDropInteractionMarkerShown = true;
				m_teardropInteractionEnable.SetActive(value: true);
			}
			m_teardropRenderer.sharedMaterial = m_materials[interaction.m_teardropMaterialIndex];
			if (!m_hasEverSet || interactionOrAlertType != m_shownIcon)
			{
				string iconName = Singleton<EnumToIconMapping>.Instance.GetIconName(interactionOrAlertType);
				m_shownIcon = interactionOrAlertType;
				tk2dBaseSprite[] sprites = m_sprites;
				for (int i = 0; i < sprites.Length; i++)
				{
					tk2dSprite tk2dSprite2 = (tk2dSprite)sprites[i];
					tk2dSprite2.SetSprite(iconName);
				}
			}
			m_hasEverSet = true;
			if (m_colliderIsEnabled != flag)
			{
				m_colliderIsEnabled = flag;
				m_teardropCollider.enabled = m_colliderIsEnabled;
			}
		}
		else
		{
			if (!m_hasEverSet || m_tearDropInteractionMarkerShown)
			{
				m_tearDropInteractionMarkerShown = false;
				m_teardropInteractionEnable.SetActive(value: false);
			}
			m_hasEverSet = true;
		}
		int inputId = m_interactiveItem.GetInputId();
		if (inputId != 0 && (!Singleton<UISelector>.Instance.IsPrimary() || Singleton<ContextControl>.Instance.IsMovementMode()) && ReInput.players.GetPlayer(0).GetButtonDown(inputId))
		{
			MainAreaClick_OnClick();
		}
		if (interaction != null)
		{
			string hotkeyText = m_interactiveItem.GetHotkeyText();
			string hotkeyControllerText = m_interactiveItem.GetHotkeyControllerText();
			if (!string.IsNullOrEmpty(hotkeyText) && m_currentHotkeyDisplay == null)
			{
				m_currentHotkeyDisplay = Object.Instantiate(m_hotkeyDisplayPrefab);
				m_currentHotkeyDisplay.GetComponent<InteractiveItemHotkeyDisplay>().SetUp(hotkeyText, hotkeyControllerText, m_interactiveItem.GetMovementArrowTransform(), Singleton<ContextControl>.Instance.GetUICamera(), m_interactiveItem.GetHotkeyIsCritical());
			}
		}
		else if (m_currentHotkeyDisplay != null)
		{
			Object.Destroy(m_currentHotkeyDisplay);
		}
	}
}
