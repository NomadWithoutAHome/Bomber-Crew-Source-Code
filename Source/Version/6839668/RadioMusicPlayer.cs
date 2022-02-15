using System.Collections;
using UnityEngine;
using WingroveAudio;

public class RadioMusicPlayer : MonoBehaviour
{
	[SerializeField]
	[AudioEventName]
	private string m_playNormal;

	[SerializeField]
	[AudioEventName]
	private string m_playStatic;

	[SerializeField]
	[AudioEventName]
	private string m_radioJabber;

	private WingroveGroupInformation m_groupInfo;

	private bool m_isDoingRadioJabber;

	private float m_timeToWait;

	private void Start()
	{
		m_timeToWait = 1.5f;
		m_groupInfo = WingroveRoot.Instance.transform.Find("Master").Find("MusicMaster").Find("RadioPlaylist")
			.Find("Playlist")
			.GetComponent<WingroveGroupInformation>();
		WingroveRoot.Instance.PostEventGO(m_playStatic, base.gameObject);
		WingroveRoot.Instance.PostEventGO(m_playNormal, base.gameObject);
	}

	private IEnumerator DoRadioJabberNewTrack()
	{
		yield return new WaitForSeconds(1.5f);
		int sentences = Random.Range(2, 5);
		for (int i = 0; i < sentences; i++)
		{
			int sylls = Random.Range(9, 14);
			for (int j = 0; j < sylls; j++)
			{
				WingroveRoot.Instance.PostEventGO(m_radioJabber, base.gameObject);
				yield return new WaitForSeconds(Random.Range(0.2f, 0.35f));
			}
			yield return new WaitForSeconds(Random.Range(1f, 2f));
		}
		yield return new WaitForSeconds(1.5f);
		WingroveRoot.Instance.PostEventGO(m_playNormal, base.gameObject);
		yield return null;
		m_isDoingRadioJabber = false;
		m_timeToWait = 1.5f;
	}

	private void Update()
	{
		m_timeToWait -= Time.deltaTime;
		if (!m_groupInfo.IsAnyPlaying() && m_timeToWait < 0f && !m_isDoingRadioJabber)
		{
			m_isDoingRadioJabber = true;
			StartCoroutine(DoRadioJabberNewTrack());
		}
	}
}
