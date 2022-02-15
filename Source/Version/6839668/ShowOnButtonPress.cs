using Common;
using Rewired;
using UnityEngine;

public class ShowOnButtonPress : MonoBehaviour
{
	[SerializeField]
	private GameObject m_toShow;

	[SerializeField]
	private string m_buttonRef;

	private void Start()
	{
		m_toShow.SetActive(value: false);
	}

	private void Update()
	{
		if (ReInput.players.GetPlayer(0).GetButton(m_buttonRef))
		{
			if (!m_toShow.IsActivated())
			{
				m_toShow.CustomActivate(active: true);
			}
		}
		else if (m_toShow.IsActivated())
		{
			m_toShow.CustomActivate(active: false);
		}
	}
}
