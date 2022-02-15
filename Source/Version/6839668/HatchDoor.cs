using System.Collections;
using UnityEngine;
using WingroveAudio;

public class HatchDoor : MonoBehaviour
{
	[SerializeField]
	private Animation m_animation;

	[SerializeField]
	private float m_openCloseTime = 0.5f;

	[SerializeField]
	private BomberDestroyableSection m_destroyableSection;

	[SerializeField]
	[AudioEventName]
	private string m_openAudioEvent;

	[SerializeField]
	[AudioEventName]
	private string m_closeAudioEvent;

	private bool m_isOpen;

	private int m_openCount;

	private float m_openness;

	public void RequestOpen()
	{
		m_openCount++;
		if (m_openCount == 1)
		{
			WingroveRoot.Instance.PostEvent(m_closeAudioEvent);
			m_isOpen = true;
		}
	}

	public void RequestOpen(float closeInSeconds)
	{
		RequestOpen();
		StartCoroutine(CloseInSeconds(closeInSeconds));
	}

	private IEnumerator CloseInSeconds(float seconds)
	{
		yield return new WaitForSeconds(seconds);
		RequestClose();
	}

	public void RequestClose()
	{
		m_openCount--;
		if (m_openCount == 0)
		{
			WingroveRoot.Instance.PostEvent(m_closeAudioEvent);
			m_isOpen = false;
		}
	}

	private void Update()
	{
		if (m_destroyableSection == null || !m_destroyableSection.IsDestroyed())
		{
			if (m_animation != null)
			{
				string animation = m_animation.clip.name;
				m_animation.Play(animation);
				m_animation[animation].enabled = true;
				m_animation[animation].speed = 0f;
				m_animation[animation].normalizedTime = m_openness;
			}
			if (m_isOpen)
			{
				m_openness = Mathf.Clamp01(m_openness + Time.deltaTime / m_openCloseTime);
			}
			else
			{
				m_openness = Mathf.Clamp01(m_openness - Time.deltaTime / m_openCloseTime);
			}
		}
	}
}
