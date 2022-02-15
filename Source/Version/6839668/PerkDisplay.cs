using BomberCrewCommon;
using UnityEngine;

public class PerkDisplay : MonoBehaviour
{
	[SerializeField]
	private TextSetter m_descriptionText;

	[SerializeField]
	private TextSetter m_missionsRemaingText;

	public void SetUp(SaveData.PerkType pt, int forMissions, float factor)
	{
		string empty = string.Empty;
		switch (pt)
		{
		case SaveData.PerkType.EnemyDamageDown:
			m_descriptionText.SetTextFromLanguageString("ui_perk_enemy_damage");
			break;
		case SaveData.PerkType.EnemyHealthDown:
			m_descriptionText.SetTextFromLanguageString("ui_perk_enemy_armour");
			break;
		case SaveData.PerkType.FlakInterruptions:
			m_descriptionText.SetTextFromLanguageString("ui_perk_flak_operation");
			break;
		}
		m_missionsRemaingText.SetText(string.Format(Singleton<LanguageProvider>.Instance.GetNamedTextImmediate("ui_debrief_briefing_fornextxmissions"), forMissions.ToString()));
	}
}
