using Rewired.Dev;

namespace RewiredConsts;

public static class Action
{
	[ActionIdFieldInfo(categoryName = "In Mission", friendlyName = "Move Selection Y")]
	public const int Move_Selection_Y = 0;

	[ActionIdFieldInfo(categoryName = "In Mission", friendlyName = "Move Selection Y Analogue")]
	public const int Move_Selection_Y_Analogue = 40;

	[ActionIdFieldInfo(categoryName = "In Mission", friendlyName = "Move Selection Y Digital")]
	public const int Move_Selection_Y_Digital = 41;

	[ActionIdFieldInfo(categoryName = "In Mission", friendlyName = "Move Selection X")]
	public const int Move_Selection_X = 1;

	[ActionIdFieldInfo(categoryName = "In Mission", friendlyName = "Move Selection X Analogue")]
	public const int Move_Selection_X_Analogue = 42;

	[ActionIdFieldInfo(categoryName = "In Mission", friendlyName = "Move Selection X Digital")]
	public const int Move_Selection_X_Digital = 43;

	[ActionIdFieldInfo(categoryName = "In Mission", friendlyName = "Select")]
	public const int Select = 2;

	[ActionIdFieldInfo(categoryName = "In Mission", friendlyName = "Cancel selection")]
	public const int Cancel_selection = 3;

	[ActionIdFieldInfo(categoryName = "In Mission", friendlyName = "Select Crew Right")]
	public const int Select_Crew_Right = 4;

	[ActionIdFieldInfo(categoryName = "In Mission", friendlyName = "Select Crew Left")]
	public const int Select_Crew_Left = 24;

	[ActionIdFieldInfo(categoryName = "In Mission", friendlyName = "Select Crew Up")]
	public const int Select_Crew_Up = 5;

	[ActionIdFieldInfo(categoryName = "In Mission", friendlyName = "Select Crew Down")]
	public const int Select_Crew_Down = 25;

	[ActionIdFieldInfo(categoryName = "In Mission", friendlyName = "Camera rotate X")]
	public const int Camera_rotate_X = 6;

	[ActionIdFieldInfo(categoryName = "In Mission", friendlyName = "Camera rotate Y")]
	public const int Camera_rotate_Y = 7;

	[ActionIdFieldInfo(categoryName = "In Mission", friendlyName = "Camera zoom")]
	public const int Camera_zoom = 8;

	[ActionIdFieldInfo(categoryName = "In Mission", friendlyName = "Toggle Targeting Mode")]
	public const int Toggle_Targeting_Mode = 12;

	[ActionIdFieldInfo(categoryName = "In Mission", friendlyName = "Hold Targeting Mode")]
	public const int Hold_Targeting_Mode = 16;

	[ActionIdFieldInfo(categoryName = "In Mission", friendlyName = "Hold Movement Mode")]
	public const int Hold_Movement_Mode = 17;

	[ActionIdFieldInfo(categoryName = "In Mission", friendlyName = "Quick Select Pilot")]
	public const int Quick_Select_Pilot = 20;

	[ActionIdFieldInfo(categoryName = "In Mission", friendlyName = "Pause")]
	public const int Pause = 21;

	[ActionIdFieldInfo(categoryName = "In Mission", friendlyName = "Fast Forward")]
	public const int Fast_Forward = 22;

	[ActionIdFieldInfo(categoryName = "In Mission", friendlyName = "Mouse Move Camera")]
	public const int Mouse_Move_Camera = 33;

	[ActionIdFieldInfo(categoryName = "In Mission", friendlyName = "Jump to internal zoom")]
	public const int Jump_to_internal_zoom = 34;

	[ActionIdFieldInfo(categoryName = "In Mission", friendlyName = "Camera Rotate Y TM")]
	public const int Camera_Rotate_Y_TM = 35;

	[ActionIdFieldInfo(categoryName = "In Mission", friendlyName = "Slow T ime")]
	public const int Slow_Time = 36;

	[ActionIdFieldInfo(categoryName = "In Mission", friendlyName = "Get Ammo")]
	public const int Get_Ammo = 37;

	[ActionIdFieldInfo(categoryName = "In Mission", friendlyName = "To Med Bay")]
	public const int To_Med_Bay = 38;

	[ActionIdFieldInfo(categoryName = "In Mission", friendlyName = "To Pilot Station")]
	public const int To_Pilot_Station = 39;

	[ActionIdFieldInfo(categoryName = "In Mission", friendlyName = "Context Action 1")]
	public const int Context_Action_1 = 44;

	[ActionIdFieldInfo(categoryName = "In Mission", friendlyName = "Context Action 2")]
	public const int Context_Action_2 = 46;

	[ActionIdFieldInfo(categoryName = "In Mission", friendlyName = "Context Action 3")]
	public const int Context_Action_3 = 47;

	[ActionIdFieldInfo(categoryName = "In Mission", friendlyName = "Toggle internal zoom")]
	public const int Toggle_internal_zoom = 45;

	[ActionIdFieldInfo(categoryName = "In Mission", friendlyName = "Zoom Hold or Toggle")]
	public const int Zoom_Hold_or_Toggle = 48;

	[ActionIdFieldInfo(categoryName = "Airbase", friendlyName = "Select Top Bar L")]
	public const int Select_Top_Bar_L = 27;

	[ActionIdFieldInfo(categoryName = "Airbase", friendlyName = "Select Top Bar R")]
	public const int Select_Top_Bar_R = 28;

	[ActionIdFieldInfo(categoryName = "Airbase", friendlyName = "Select Med Bar L")]
	public const int Select_Med_Bar_L = 30;

	[ActionIdFieldInfo(categoryName = "Airbase", friendlyName = "Select Med Bar R")]
	public const int Select_Med_Bar_R = 29;

	[ActionIdFieldInfo(categoryName = "Airbase", friendlyName = "Select Third Category L")]
	public const int Select_Third_Category_L = 31;

	[ActionIdFieldInfo(categoryName = "Airbase", friendlyName = "Select Third Category R")]
	public const int Select_Third_Category_R = 32;
}
