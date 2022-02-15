using UnityEngine;

public class ControlPromptIcon : MonoBehaviour
{
	public enum ControllerSpriteButton
	{
		PLATFORM_BumperL,
		PLATFORM_BumperR,
		PLATFORM_ButtonBack,
		PLATFORM_ButtonConfirm,
		PLATFORM_ButtonL,
		PLATFORM_ButtonSelect,
		PLATFORM_ButtonStart,
		PLATFORM_ButtonUp,
		PLATFORM_DPad,
		PLATFORM_DPad_Down,
		PLATFORM_DPad_Left,
		PLATFORM_DPad_LeftRight,
		PLATFORM_DPad_Right,
		PLATFORM_DPad_Up,
		PLATFORM_DPad_UpDown,
		PLATFORM_StickL,
		PLATFORM_StickR,
		PLATFORM_TriggerL,
		PLATFORM_TriggerR,
		MOUSE_ClickLeft,
		MOUSE_ClickRight,
		MOUSE_Mouse,
		MOUSE_MouseWheelClick,
		MOUSE_MouseWheelScroll,
		PLATFORM_StickL_LeftRight,
		PLATFORM_StickR_LeftRight,
		PLATFORM_StickL_UpDown,
		PLATFORM_StickR_UpDown
	}

	[SerializeField]
	private tk2dSprite m_controllerSprite;

	[SerializeField]
	private ControllerSpriteButton m_spriteButton;

	private void Awake()
	{
		Refresh();
	}

	public void SetIcon(ControllerSpriteButton buttonType)
	{
		m_spriteButton = buttonType;
		Refresh();
	}

	private void Refresh()
	{
		m_controllerSprite.SetSprite(m_spriteButton.ToString().Replace("PLATFORM", ControlPromptDisplayHelpers.GetMapSpriteType().ToString()));
	}
}
