using BomberCrewCommon;
using UnityEngine;

public class FighterMusicTrigger : MonoBehaviour
{
	[SerializeField]
	private FighterAI m_fighterAI;

	[SerializeField]
	private MusicSelectionRules.MusicTriggerEvents m_toTrigger;

	private bool m_isTriggered;

	public void Update()
	{
		bool flag = m_fighterAI.IsEngaged();
		if (flag != m_isTriggered)
		{
			if (flag)
			{
				Singleton<MusicSelectionRules>.Instance.Trigger(m_toTrigger);
			}
			else
			{
				Singleton<MusicSelectionRules>.Instance.Untrigger(m_toTrigger);
			}
			m_isTriggered = flag;
		}
	}

	private void OnDestroy()
	{
		if (m_isTriggered)
		{
			Singleton<MusicSelectionRules>.Instance.Untrigger(m_toTrigger);
			m_isTriggered = false;
		}
	}
}
