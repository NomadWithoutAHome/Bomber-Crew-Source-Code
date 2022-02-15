using UnityEngine;

public class NotificationDisplay : MonoBehaviour
{
	[SerializeField]
	private tk2dTextMesh m_textMesh;

	public void SetUp(InMissionNotification sn)
	{
		m_textMesh.text = sn.m_text;
	}
}
