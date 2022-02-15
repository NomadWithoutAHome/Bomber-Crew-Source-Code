using UnityEngine;
using WingroveAudio;

public class UICounterDisplay : MonoBehaviour
{
	[SerializeField]
	private TextSetter m_textSetter;

	[SerializeField]
	private GameObject m_addEffect;

	[SerializeField]
	private GameObject m_deductEffect;

	[SerializeField]
	private tk2dBaseSprite[] m_spritesToUpdate;

	[SerializeField]
	private string m_format = "{0}";

	private int m_currentValue;

	private bool m_initialValueSet;

	private void Start()
	{
		m_addEffect.SetActive(value: false);
		m_deductEffect.SetActive(value: false);
	}

	private void Update()
	{
		tk2dBaseSprite[] spritesToUpdate = m_spritesToUpdate;
		foreach (tk2dBaseSprite tk2dBaseSprite2 in spritesToUpdate)
		{
			tk2dBaseSprite2.Build();
		}
	}

	public void UpdateValue(int newValue)
	{
		if (m_initialValueSet)
		{
			if (newValue < m_currentValue)
			{
				m_deductEffect.SetActive(value: false);
				m_deductEffect.SetActive(value: true);
			}
			else if (newValue > m_currentValue)
			{
				m_addEffect.SetActive(value: false);
				m_addEffect.SetActive(value: true);
				WingroveRoot.Instance.PostEvent("MONEY");
			}
		}
		m_initialValueSet = true;
		m_currentValue = newValue;
		m_textSetter.SetText(string.Format(m_format, newValue));
	}
}
