using UnityEngine;

public class UISystemOverlay : MonoBehaviour
{
	[SerializeField]
	private tk2dUIProgressBar m_progressBar;

	[SerializeField]
	private tk2dTextMesh m_labelText;

	[SerializeField]
	private tk2dTextMesh m_hpText;

	[SerializeField]
	private WorldToScreenTracker m_tracker;

	private Damageable m_system;

	public void SetUp(Damageable damageableSystem, Transform transformToTrack)
	{
		m_tracker.SetTracking(transformToTrack, tk2dCamera.CameraForLayer(base.gameObject.layer));
		m_system = damageableSystem;
		Refresh();
	}

	private void Refresh()
	{
		if (m_system != null)
		{
			m_labelText.text = m_system.name;
		}
	}

	private void Update()
	{
		Refresh();
	}
}
