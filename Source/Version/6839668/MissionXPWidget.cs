using UnityEngine;

public class MissionXPWidget : MonoBehaviour
{
	[SerializeField]
	private TextSetter m_textSetter;

	[SerializeField]
	private WorldToScreenTracker m_tracker;

	[SerializeField]
	private float m_duration;

	private Transform m_trackingTransform;

	private GameObject m_mirrorTrackingTransform;

	private float m_timer;

	public void SetUp(int amt, Transform toTrack, Vector3 trackPos, tk2dCamera uiCamera)
	{
		m_trackingTransform = toTrack;
		m_mirrorTrackingTransform = new GameObject("TrackingXP");
		m_mirrorTrackingTransform.AddComponent<BigTransform>();
		if (toTrack != null)
		{
			m_mirrorTrackingTransform.btransform().SetFromCurrentPage(toTrack.position);
		}
		else
		{
			m_mirrorTrackingTransform.btransform().SetFromCurrentPage(trackPos);
		}
		m_tracker.SetTracking(m_mirrorTrackingTransform.transform, uiCamera);
		m_timer = m_duration;
		m_textSetter.SetText("+" + amt);
	}

	private void Update()
	{
		if (m_trackingTransform != null)
		{
			m_mirrorTrackingTransform.btransform().SetFromCurrentPage(m_trackingTransform.position);
		}
		m_timer -= Time.deltaTime;
		if (m_timer < 0f)
		{
			Object.Destroy(m_mirrorTrackingTransform);
			Object.Destroy(base.gameObject);
		}
	}
}
