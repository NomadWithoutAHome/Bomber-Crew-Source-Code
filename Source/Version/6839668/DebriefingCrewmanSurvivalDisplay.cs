using System.Collections;
using BomberCrewCommon;
using UnityEngine;
using WingroveAudio;

public class DebriefingCrewmanSurvivalDisplay : MonoBehaviour
{
	[SerializeField]
	private GameObject m_statsDisplay;

	[SerializeField]
	private tk2dRadialSprite m_radialSprite;

	[SerializeField]
	private Transform m_rotateDisplay;

	[SerializeField]
	private GameObject m_wheelDisplay;

	[SerializeField]
	private TextSetter m_landedLandSea;

	[SerializeField]
	private TextSetter m_nearOrAwayFromBomber;

	[SerializeField]
	private TextSetter m_survivalValue;

	[SerializeField]
	private tk2dBaseSprite[] m_landSeaSprites;

	[SerializeField]
	private TextSetter m_hostilityValue;

	[SerializeField]
	private string m_seaSurvivalSpriteName;

	[SerializeField]
	private string m_landSurvivalSpriteName;

	[SerializeField]
	[AudioEventName]
	private string m_drumrollStartEvent;

	[SerializeField]
	[AudioEventName]
	private string m_drumrollEndEvent;

	[SerializeField]
	[AudioEventName]
	private string m_drumrollGoodEnd;

	[SerializeField]
	[AudioEventName]
	private string m_drumrollBadEnd;

	[SerializeField]
	[NamedText]
	private string m_atSeaText;

	[SerializeField]
	[NamedText]
	private string m_onLandText;

	[SerializeField]
	[NamedText]
	private string m_withBomberText;

	[SerializeField]
	[NamedText]
	private string m_awayFromBomberText;

	private bool m_isDone;

	private bool m_isMIA;

	private Vector3 m_myPosition;

	private Vector3 m_bomberPosition;

	private DebriefingScreen m_debriefingScreen;

	private void Awake()
	{
	}

	public void SetUp(Crewman cm, MissionLog.LandingState ls, bool isBailedOut, Vector3 bomberPosition, Vector3 inPosition, DebriefingScreen db)
	{
		m_debriefingScreen = db;
		m_wheelDisplay.SetActive(value: false);
		m_survivalValue.SetText("0");
		m_hostilityValue.SetText("0");
		if (ls == MissionLog.LandingState.InSea)
		{
			m_landedLandSea.SetTextFromLanguageString(m_atSeaText);
			tk2dBaseSprite[] landSeaSprites = m_landSeaSprites;
			foreach (tk2dBaseSprite tk2dBaseSprite2 in landSeaSprites)
			{
				tk2dBaseSprite2.SetSprite(m_seaSurvivalSpriteName);
			}
		}
		else
		{
			m_landedLandSea.SetTextFromLanguageString(m_onLandText);
			tk2dBaseSprite[] landSeaSprites2 = m_landSeaSprites;
			foreach (tk2dBaseSprite tk2dBaseSprite3 in landSeaSprites2)
			{
				tk2dBaseSprite3.SetSprite(m_landSurvivalSpriteName);
			}
		}
		m_bomberPosition = bomberPosition;
		if (!isBailedOut)
		{
			m_myPosition = bomberPosition;
		}
		else
		{
			m_myPosition = inPosition;
		}
		if ((m_bomberPosition - m_myPosition).magnitude < 2500f)
		{
			m_nearOrAwayFromBomber.SetTextFromLanguageString(m_withBomberText);
		}
		else
		{
			m_nearOrAwayFromBomber.SetTextFromLanguageString(m_awayFromBomberText);
		}
		StartCoroutine(DoSequence(cm, ls, isBailedOut));
	}

	public bool IsDone()
	{
		return m_isDone;
	}

	public bool IsMIA()
	{
		return m_isMIA;
	}

	private IEnumerator DoSequence(Crewman cm, MissionLog.LandingState ls, bool isBailedOut)
	{
		int hostilityPoints = Mathf.Clamp(Mathf.RoundToInt((m_myPosition.magnitude - 5000f) / 250f), 10, 100);
		int survivalPoints = 0;
		survivalPoints = ((ls != MissionLog.LandingState.InSea) ? (survivalPoints + cm.GetSurvivalLand()) : (survivalPoints + cm.GetSurvivalSea()));
		if ((m_bomberPosition - m_myPosition).magnitude < 150f)
		{
			BomberUpgradeConfig currentConfig = Singleton<BomberContainer>.Instance.GetCurrentConfig();
			BomberRequirements requirements = Singleton<BomberContainer>.Instance.GetRequirements();
			BomberRequirements.BomberEquipmentRequirement[] requirements2 = requirements.GetRequirements();
			foreach (BomberRequirements.BomberEquipmentRequirement req in requirements2)
			{
				EquipmentUpgradeFittableBase upgradeFor = currentConfig.GetUpgradeFor(req);
				if (upgradeFor != null)
				{
					survivalPoints = ((ls != MissionLog.LandingState.InSea) ? (survivalPoints + upgradeFor.GetSurvivalPointsLand()) : (survivalPoints + upgradeFor.GetSurvivalPointsSea()));
				}
			}
		}
		float t3 = 0f;
		while (t3 < 1f)
		{
			m_survivalValue.SetText(((int)((float)survivalPoints * t3)).ToString());
			t3 += Time.deltaTime * ((!m_debriefingScreen.ShouldFastForward()) ? 2f : 10f);
			yield return null;
		}
		m_survivalValue.SetText(survivalPoints.ToString());
		t3 = 0f;
		while (t3 < 1f)
		{
			m_hostilityValue.SetText(((int)((float)hostilityPoints * t3)).ToString());
			t3 += Time.deltaTime * ((!m_debriefingScreen.ShouldFastForward()) ? 2f : 10f);
			yield return null;
		}
		m_hostilityValue.SetText(hostilityPoints.ToString());
		m_radialSprite.SetValue(1f);
		float ang = 10f;
		m_rotateDisplay.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, ang));
		m_wheelDisplay.SetActive(value: true);
		float proportion = (float)survivalPoints / (float)(survivalPoints + hostilityPoints);
		t3 = 0f;
		while (t3 < 1f)
		{
			m_radialSprite.SetValue(1f - t3 * proportion);
			t3 += Time.deltaTime * ((!m_debriefingScreen.ShouldFastForward()) ? 2f : 10f);
			yield return null;
		}
		m_radialSprite.SetValue(1f - proportion);
		if (proportion > 0f)
		{
			WingroveRoot.Instance.PostEvent(m_drumrollStartEvent);
			int angFinalResult2 = Mathf.RoundToInt(Mathf.Pow(Random.Range(0f, 1f), 0.6f) * 360f) % 360;
			angFinalResult2 += 720;
			while (true)
			{
				float delta = (float)angFinalResult2 - ang;
				ang += Mathf.Min(delta * 2f, 360f) * Time.deltaTime * ((!m_debriefingScreen.ShouldFastForward()) ? 1f : 5f);
				m_rotateDisplay.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, ang));
				if (Mathf.Abs(delta) < 1f)
				{
					break;
				}
				yield return null;
			}
			WingroveRoot.Instance.PostEvent(m_drumrollEndEvent);
			int intA = (int)ang % 360;
			if (intA >= 360 - Mathf.RoundToInt(proportion * 360f) || intA < 2)
			{
				WingroveRoot.Instance.PostEvent(m_drumrollGoodEnd);
				m_isMIA = false;
			}
			else
			{
				WingroveRoot.Instance.PostEvent(m_drumrollBadEnd);
				m_isMIA = true;
			}
		}
		else
		{
			WingroveRoot.Instance.PostEvent(m_drumrollBadEnd);
			m_isMIA = true;
		}
		yield return StartCoroutine(m_debriefingScreen.DoSkipWait(1.5f));
		m_isDone = true;
		yield return null;
	}
}
