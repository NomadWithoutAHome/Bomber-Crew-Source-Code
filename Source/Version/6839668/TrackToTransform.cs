using UnityEngine;

public class TrackToTransform : MonoBehaviour
{
	[SerializeField]
	private Transform m_transformToTrackTo;

	private void Update()
	{
		base.transform.rotation = m_transformToTrackTo.rotation;
		base.transform.position = m_transformToTrackTo.position;
		base.transform.localScale = m_transformToTrackTo.localScale;
	}
}
