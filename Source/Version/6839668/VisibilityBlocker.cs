using UnityEngine;

public class VisibilityBlocker : MonoBehaviour
{
	[SerializeField]
	private float m_blockOverDistance;

	[SerializeField]
	private float m_blockUnderDistance;

	public bool IsVisible(float atDistance)
	{
		if (m_blockUnderDistance < m_blockOverDistance)
		{
			if (atDistance < m_blockUnderDistance || atDistance > m_blockOverDistance)
			{
				return true;
			}
		}
		else if (atDistance > m_blockUnderDistance && atDistance < m_blockOverDistance)
		{
			return true;
		}
		return false;
	}
}
