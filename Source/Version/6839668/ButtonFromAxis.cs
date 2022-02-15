using Rewired;
using UnityEngine;

public class ButtonFromAxis : MonoBehaviour
{
	[SerializeField]
	private GameObject m_toShowPositive;

	[SerializeField]
	private GameObject m_toShowNegative;

	[SerializeField]
	private string m_buttonRef;

	private void Update()
	{
		m_toShowPositive.SetActive(ReInput.players.GetPlayer(0).GetAxis(m_buttonRef) > 0f);
		m_toShowNegative.SetActive(ReInput.players.GetPlayer(0).GetAxis(m_buttonRef) < 0f);
	}
}
