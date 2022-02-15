using System.Collections;
using BomberCrewCommon;
using UnityEngine;
using WingroveAudio;

public class KeyMissionDisplay : MonoBehaviour
{
	[SerializeField]
	private TextSetter m_titleSetter;

	[SerializeField]
	private Animator m_animator;

	[SerializeField]
	private float m_delayAudio;

	[SerializeField]
	[AudioEventName]
	private string m_audioHookStart;

	[SerializeField]
	[AudioEventName]
	private string m_audioHookEnd;

	[SerializeField]
	private float m_displayTime;

	public void SetUp(CampaignStructure.CampaignMission cm)
	{
		m_titleSetter.SetTextFromLanguageString(cm.m_titleNamedText);
	}

	private IEnumerator Start()
	{
		m_animator.SetBool("Show", value: true);
		yield return new WaitForSeconds(m_delayAudio);
		WingroveRoot.Instance.PostEvent(m_audioHookStart);
		yield return new WaitForSeconds(m_displayTime);
		WingroveRoot.Instance.PostEvent(m_audioHookEnd);
		m_animator.SetBool("Show", value: false);
		yield return new WaitForSeconds(5f);
		while (Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBomberState().HasNotCompletedTakeOff())
		{
			yield return null;
		}
		Singleton<MusicSelectionRules>.Instance.TriggerTimed(MusicSelectionRules.MusicTriggerEvents.KeyMissionBegin, 10f);
	}
}
