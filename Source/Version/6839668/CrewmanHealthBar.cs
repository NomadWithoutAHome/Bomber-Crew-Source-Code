using Common;
using UnityEngine;

public class CrewmanHealthBar : MonoBehaviour
{
	[SerializeField]
	private tk2dUIProgressBar m_healthBar;

	[SerializeField]
	private tk2dUIProgressBar m_healthBarRedFill;

	[SerializeField]
	private tk2dUIProgressBar m_oxygenBar;

	[SerializeField]
	private tk2dUIProgressBar m_oxygenTemperatureBar;

	[SerializeField]
	private GameObject m_countingDownHierarchy;

	[SerializeField]
	private tk2dClippedSprite m_deathClippedSprite;

	[SerializeField]
	private tk2dTextMesh m_deathCountdownText;

	[SerializeField]
	private GameObject m_isDeadHierarchy;

	private CrewmanAvatar m_crewmanAvatar;

	private CrewmanLifeStatus m_lifeStatus;

	private float m_lastHealthN = -1f;

	private float m_lastHealthRed = -1f;

	private float m_lastHealthOxT = -1f;

	private float m_lastOx = -1f;

	public void SetUp(CrewmanAvatar crewman)
	{
		m_crewmanAvatar = crewman;
		m_lifeStatus = crewman.GetHealthState();
		Refresh();
	}

	private void Update()
	{
		Refresh();
	}

	private void Refresh()
	{
		if (!(m_lifeStatus != null))
		{
			return;
		}
		float totalHealthN = m_lifeStatus.GetTotalHealthN();
		if (m_lastHealthN != totalHealthN)
		{
			m_healthBar.Value = totalHealthN;
			m_lastHealthN = totalHealthN;
		}
		float num = totalHealthN + m_lifeStatus.GetRedDisplayN();
		if (m_lastHealthRed != num)
		{
			m_healthBarRedFill.Value = num;
			m_lastHealthRed = num;
		}
		float num2 = totalHealthN + m_lifeStatus.GetOxygenTemperatureN();
		if (m_lastHealthOxT != num2)
		{
			m_oxygenTemperatureBar.Value = num2;
			m_lastHealthOxT = num2;
		}
		float num3 = totalHealthN + m_lifeStatus.GetOxygenN();
		if (m_lastOx != num3)
		{
			m_oxygenBar.Value = num3;
			m_lastOx = num3;
		}
		if (m_lifeStatus.IsCountingDown())
		{
			if (m_countingDownHierarchy != null)
			{
				m_countingDownHierarchy.SetActive(value: true);
				if (m_deathClippedSprite != null)
				{
					m_deathClippedSprite.clipTopRight = new Vector2(m_lifeStatus.GetHealthCountdownNormalised(), 1f);
				}
				if (m_deathCountdownText != null)
				{
					float healthCountdown = m_lifeStatus.GetHealthCountdown();
					int num4 = Mathf.FloorToInt(healthCountdown / 60f);
					int num5 = Mathf.FloorToInt(healthCountdown - (float)(num4 * 60));
					string text = $"{num4:0}:{num5:00}";
					m_deathCountdownText.text = text;
				}
			}
		}
		else if (m_lifeStatus.IsDead())
		{
			if (m_deathClippedSprite != null)
			{
				m_deathClippedSprite.clipTopRight = new Vector2(0f, 1f);
			}
			if (m_countingDownHierarchy != null)
			{
				m_countingDownHierarchy.SetActive(value: false);
			}
			if (m_isDeadHierarchy != null)
			{
				m_isDeadHierarchy.CustomActivate(active: true);
			}
		}
		else if (m_countingDownHierarchy != null)
		{
			m_countingDownHierarchy.SetActive(value: false);
		}
	}
}
