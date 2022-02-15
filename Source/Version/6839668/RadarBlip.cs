using UnityEngine;

public class RadarBlip : MonoBehaviour
{
	[SerializeField]
	private GameObject m_upArrow;

	[SerializeField]
	private GameObject m_downArrow;

	[SerializeField]
	private GameObject m_groundedIndicator;

	[SerializeField]
	private AnimateDetailAlpha[] m_alphaAnimation;

	[SerializeField]
	private float m_fadeAwaySpeed = 1f;

	[SerializeField]
	private tk2dSpriteAnimator m_blipPingAnimator;

	private float m_currentPingAmount;

	private bool m_shouldPlayAnimation;

	public void SetArrow(bool up, bool down, bool grounded)
	{
		m_upArrow.SetActive(up && !grounded);
		m_downArrow.SetActive(down && !grounded);
		m_groundedIndicator.SetActive(grounded);
	}

	public void PingBlip()
	{
		if (m_currentPingAmount < 0.5f)
		{
			m_shouldPlayAnimation = true;
			m_blipPingAnimator.Play();
		}
		m_currentPingAmount = 1f;
	}

	public bool ShouldPlayAnimation()
	{
		return m_shouldPlayAnimation && m_currentPingAmount > 0.9f;
	}

	public void MarkAnimationPlayed()
	{
		m_shouldPlayAnimation = false;
	}

	private void Update()
	{
		m_currentPingAmount -= m_fadeAwaySpeed * Time.deltaTime;
		if (m_currentPingAmount < 0f)
		{
			m_shouldPlayAnimation = false;
			m_currentPingAmount = 0f;
		}
		AnimateDetailAlpha[] alphaAnimation = m_alphaAnimation;
		foreach (AnimateDetailAlpha animateDetailAlpha in alphaAnimation)
		{
			animateDetailAlpha.SetDetailAlpha(m_currentPingAmount);
		}
	}

	public float GetCurrentPingAmount()
	{
		return m_currentPingAmount;
	}
}
