using UnityEngine;

public class TrackPosition : MonoBehaviour
{
	[SerializeField]
	private Transform m_toCopy;

	private void LateUpdate()
	{
		base.transform.position = m_toCopy.position;
	}
}
