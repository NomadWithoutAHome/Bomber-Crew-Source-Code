using System;
using BomberCrewCommon;
using UnityEngine;

public class UIButtonStateDefinitions : Singleton<UIButtonStateDefinitions>
{
	[Serializable]
	public class ButtonDefinitionState
	{
		public string m_backgroundSprite;

		public Color m_textColour;

		public Color m_iconColor;

		public Vector3 m_offset = Vector3.zero;

		public bool m_enableUiItem = true;
	}

	public enum ButtonType
	{
		Default,
		StoreItem,
		StoreItemTypeSelection,
		Purchase,
		SkillAbility,
		LiveryColorSwatch,
		MultiLevelMenu
	}

	public enum ButtonState
	{
		Default,
		Disabled,
		Selected,
		FilteredOut,
		FilteredOutSelected,
		FilteredOutSelectedLowKey
	}

	[Serializable]
	public class ButtonDefinition
	{
		[SerializeField]
		private ButtonDefinitionState m_default;

		[SerializeField]
		private ButtonDefinitionState m_disabled;

		[SerializeField]
		private ButtonDefinitionState m_selected;

		[SerializeField]
		private ButtonDefinitionState m_filteredOut;

		[SerializeField]
		private ButtonDefinitionState m_filteredOutSelected;

		[SerializeField]
		private ButtonDefinitionState m_filteredOutSelectedLowKey;

		public ButtonDefinitionState GetStateDefinition(ButtonState buttonState)
		{
			return buttonState switch
			{
				ButtonState.Default => m_default, 
				ButtonState.Selected => m_selected, 
				ButtonState.Disabled => m_disabled, 
				ButtonState.FilteredOut => m_filteredOut, 
				ButtonState.FilteredOutSelected => m_filteredOutSelected, 
				ButtonState.FilteredOutSelectedLowKey => m_filteredOutSelectedLowKey, 
				_ => m_default, 
			};
		}
	}

	[SerializeField]
	private ButtonDefinition m_buttonDefault;

	[SerializeField]
	private ButtonDefinition m_buttonStoreItem;

	[SerializeField]
	private ButtonDefinition m_buttonStoreItemTypeSelection;

	[SerializeField]
	private ButtonDefinition m_buttonPurchase;

	[SerializeField]
	private ButtonDefinition m_buttonSkillAbility;

	[SerializeField]
	private ButtonDefinition m_buttonLiveryColorSwatch;

	[SerializeField]
	private ButtonDefinition m_buttonMultiLevelMenu;

	public ButtonDefinitionState GetState(ButtonType buttonType, ButtonState buttonState)
	{
		return buttonType switch
		{
			ButtonType.Default => m_buttonDefault.GetStateDefinition(buttonState), 
			ButtonType.StoreItem => m_buttonStoreItem.GetStateDefinition(buttonState), 
			ButtonType.StoreItemTypeSelection => m_buttonStoreItemTypeSelection.GetStateDefinition(buttonState), 
			ButtonType.Purchase => m_buttonPurchase.GetStateDefinition(buttonState), 
			ButtonType.SkillAbility => m_buttonSkillAbility.GetStateDefinition(buttonState), 
			ButtonType.LiveryColorSwatch => m_buttonLiveryColorSwatch.GetStateDefinition(buttonState), 
			ButtonType.MultiLevelMenu => m_buttonMultiLevelMenu.GetStateDefinition(buttonState), 
			_ => m_buttonDefault.GetStateDefinition(buttonState), 
		};
	}
}
