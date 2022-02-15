using UnityEngine;

public class MissionMapAreaDisplay : MonoBehaviour
{
	[SerializeField]
	private Transform m_circleDisplay;

	public void SetUp(float radius)
	{
		m_circleDisplay.transform.localScale = new Vector3(radius * 2f, 0.1f, radius * 2f);
	}
}
