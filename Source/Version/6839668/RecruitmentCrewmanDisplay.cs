using System;
using BomberCrewCommon;
using UnityEngine;

public class RecruitmentCrewmanDisplay : MonoBehaviour
{
	[SerializeField]
	private tk2dTextMesh m_nameTextField;

	[SerializeField]
	private tk2dTextMesh m_detailsText;

	[SerializeField]
	[NamedText]
	private string m_ageTextFieldPrefix;

	[SerializeField]
	[NamedText]
	private string m_traitsTextFieldPrefix;

	[SerializeField]
	[NamedText]
	private string m_hometownTextFieldPrefix;

	[SerializeField]
	private CrewmanDualSkillDisplay m_skillDisplay;

	[SerializeField]
	private tk2dUIItem m_recruitButton;

	[SerializeField]
	private tk2dUIItem m_cancelButton;

	[SerializeField]
	private tk2dUIItem m_renameButton;

	[SerializeField]
	private TextSetter m_armourStat;

	[SerializeField]
	private TextSetter m_speedStat;

	[SerializeField]
	private TextSetter m_temperatureStat;

	[SerializeField]
	private TextSetter m_oxygenStat;

	[SerializeField]
	private TextSetter m_survivalLandStat;

	[SerializeField]
	private TextSetter m_survivalSeaStat;

	[SerializeField]
	private UISelectFinder m_finder;

	private Crewman m_crewman;

	private GameObject m_avatar;

	public event Action OnClick;

	public event Action OnClickCancel;

	private void Start()
	{
		m_recruitButton.OnClick += OnRecruitClick;
		m_cancelButton.OnClick += OnCancelClick;
		m_renameButton.OnClick += DoRename;
	}

	public UISelectFinder GetFinder()
	{
		return m_finder;
	}

	private void OnRecruitClick()
	{
		if (this.OnClick != null)
		{
			this.OnClick();
		}
	}

	public void FakeClick()
	{
		OnRecruitClick();
	}

	private void OnCancelClick()
	{
		if (this.OnClickCancel != null)
		{
			this.OnClickCancel();
		}
	}

	private void DoRename()
	{
		m_crewman.DoEdit(delegate
		{
			m_avatar.GetComponentInChildren<CrewmanGraphics>().SetFromCrewman(m_crewman);
			SetUp(m_crewman, m_avatar);
		});
	}

	public Crewman GetCrewman()
	{
		return m_crewman;
	}

	public void SetUp(Crewman crewman, GameObject avatar)
	{
		m_avatar = avatar;
		m_crewman = crewman;
		m_nameTextField.text = $"{crewman.GetFirstName()} {crewman.GetSurname()}";
		string empty = string.Empty;
		empty += $"{Singleton<LanguageProvider>.Instance.GetNamedTextImmediate(m_ageTextFieldPrefix)} {crewman.GetAge()}";
		empty = empty + "\n" + $"{Singleton<LanguageProvider>.Instance.GetNamedTextImmediate(m_traitsTextFieldPrefix)} {Singleton<LanguageProvider>.Instance.GetNamedTextImmediate(crewman.GetTraitsNamedText())}";
		empty = empty + "\n" + $"{Singleton<LanguageProvider>.Instance.GetNamedTextImmediate(m_hometownTextFieldPrefix)} {crewman.GetHomeTown()}";
		m_detailsText.text = empty;
		m_skillDisplay.SetUp(crewman);
		string statString = Singleton<StatHelper>.Instance.GetStatString(StatHelper.StatInfo.CreateTime(crewman.GetOxygenTime(), biggerIsBetter: true, null), includePlus: false);
		m_oxygenStat.SetText(statString);
		string statString2 = Singleton<StatHelper>.Instance.GetStatString(StatHelper.StatInfo.Create(crewman.GetArmourTotal(), null), includePlus: false);
		m_armourStat.SetText(statString2);
		string statString3 = Singleton<StatHelper>.Instance.GetStatString(StatHelper.StatInfo.Create(crewman.GetTemperatureResistance(), null), includePlus: false);
		m_temperatureStat.SetText(statString3);
		string statString4 = Singleton<StatHelper>.Instance.GetStatString(StatHelper.StatInfo.CreatePercent(crewman.GetMovementFactor(), biggerIsBetter: true, null), includePlus: false);
		m_speedStat.SetText(statString4);
		string statString5 = Singleton<StatHelper>.Instance.GetStatString(StatHelper.StatInfo.Create(crewman.GetSurvivalLand(), null), includePlus: false);
		m_survivalLandStat.SetText(statString5);
		string statString6 = Singleton<StatHelper>.Instance.GetStatString(StatHelper.StatInfo.Create(crewman.GetSurvivalSea(), null), includePlus: false);
		m_survivalSeaStat.SetText(statString6);
	}
}
