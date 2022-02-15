using BomberCrewCommon;
using UnityEngine;

public class ControlPromptDisplayHelpers : MonoBehaviour
{
	public enum ControllerMapSpriteType
	{
		STEAM,
		PS4,
		XBOX,
		SWITCH
	}

	private static ControllerMapSpriteType m_controllerMapTypePC;

	public static void SetLastControllerType(ControllerMapSpriteType cmt)
	{
		m_controllerMapTypePC = cmt;
	}

	public static ControllerMapSpriteType GetMapSpriteType()
	{
		if (Singleton<UISelector>.Instance.IsPrimary())
		{
			return Singleton<UISelector>.Instance.GetControllerType();
		}
		return m_controllerMapTypePC;
	}

	public static string ConvertString(string inputString)
	{
		return inputString.Replace("[B_SL]", "‱").Replace("[B_CN]", "⁋").Replace("[B_BL]", "⁌")
			.Replace("[B_BU]", "⁍")
			.Replace("[B_LB]", "⁐")
			.Replace("[B_RB]", "⁊")
			.Replace("[B_LT]", "⁖")
			.Replace("[B_RT]", "⁘")
			.Replace("[D_ANY]", "⁛")
			.Replace("[D_LEFT]", "⁜")
			.Replace("[D_RIGHT]", "⁞")
			.Replace("[D_UP]", "⁝")
			.Replace("[D_DOWN]", "⁰")
			.Replace("[D_LR]", "ⁱ")
			.Replace("[D_UD]", "⁴")
			.Replace("[S_L]", "⁵")
			.Replace("[S_R]", "⁶")
			.Replace("[M]", "\ua712")
			.Replace("[M_LC]", "\ua713")
			.Replace("[M_RC]", "\ua714")
			.Replace("[M_MC]", "\ua715")
			.Replace("[M_MS]", "\ua716")
			.Replace("[S_LC]", "⁷")
			.Replace("[S_RC]", "⁸")
			.Replace("[B_LMB]", "⁙")
			.Replace("[B_RMB]", "⁚");
	}
}
