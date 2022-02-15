using System.Collections;
using BomberCrewCommon;
using UnityEngine;

public class UIFader : Singleton<UIFader>
{
	[SerializeField]
	private tk2dBaseSprite m_faderSprite;

	[SerializeField]
	private Collider m_faderCollider;

	private Color m_faderColour;

	private float m_fadeTime = 1f;

	private float m_pauseTime = 1f;

	private bool m_fadeInProgress;

	private bool m_fadeIn;

	private bool m_fadeOut;

	private void Awake()
	{
		base.gameObject.SetActive(value: true);
		m_faderColour = new Color(m_faderSprite.color.r, m_faderSprite.color.g, m_faderSprite.color.b, 0f);
		m_faderSprite.GetComponent<Renderer>().enabled = false;
		m_faderCollider.enabled = false;
	}

	public void ShowFadeInOut(float fadeTime, float pauseTime, bool fadeIn, bool fadeOut)
	{
		if (!m_fadeInProgress)
		{
			m_fadeTime = fadeTime;
			m_pauseTime = pauseTime;
			m_fadeIn = fadeIn;
			m_fadeOut = fadeOut;
			base.gameObject.SetActive(value: true);
			StartCoroutine(ShowFade());
		}
	}

	private IEnumerator ShowFade()
	{
		m_fadeInProgress = true;
		float timePassed2 = 0f;
		if (m_fadeOut)
		{
			m_faderSprite.GetComponent<Renderer>().enabled = true;
			m_faderCollider.enabled = true;
			while (timePassed2 <= m_fadeTime)
			{
				timePassed2 += Time.deltaTime;
				m_faderColour.a = timePassed2 / m_fadeTime;
				m_faderSprite.color = m_faderColour;
				yield return null;
			}
		}
		m_faderColour.a = 1f;
		m_faderSprite.color = m_faderColour;
		yield return new WaitForSeconds(m_pauseTime);
		if (m_fadeIn)
		{
			m_faderSprite.GetComponent<Renderer>().enabled = true;
			m_faderCollider.enabled = true;
			timePassed2 = 0f;
			while (timePassed2 <= m_fadeTime)
			{
				timePassed2 += Time.deltaTime;
				m_faderColour.a = 1f - timePassed2 / m_fadeTime;
				m_faderSprite.color = m_faderColour;
				yield return null;
			}
			m_faderColour.a = 0f;
			m_faderSprite.GetComponent<Renderer>().enabled = false;
			m_faderCollider.enabled = false;
		}
		m_fadeInProgress = false;
		yield return null;
	}
}
