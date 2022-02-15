using Common;
using Rewired;
using UnityEngine;

public class MoveFromAxis : MonoBehaviour
{
	[SerializeField]
	private GameObject m_toShow;

	[SerializeField]
	private float m_radius;

	[SerializeField]
	private string m_buttonRefX;

	[SerializeField]
	private string m_buttonRefY;

	private void Update()
	{
		Vector2 axis2D = ReInput.players.GetPlayer(0).GetAxis2D(m_buttonRefX, m_buttonRefY);
		if (axis2D.magnitude > 0f)
		{
			if (!m_toShow.IsActivated())
			{
				m_toShow.CustomActivate(active: true);
			}
			m_toShow.transform.localPosition = axis2D * m_radius;
		}
		else if (m_toShow.IsActivated())
		{
			m_toShow.CustomActivate(active: false);
		}
	}
}
