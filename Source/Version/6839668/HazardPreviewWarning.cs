using BomberCrewCommon;
using UnityEngine;

public class HazardPreviewWarning : MonoBehaviour
{
	[SerializeField]
	private WorldToScreenTracker m_worldToScreenTracker;

	[SerializeField]
	private GameObject m_previewSphere;

	[SerializeField]
	private tk2dTextMesh[] m_counterTextMeshes;

	[SerializeField]
	private string m_countdownFormat = "{0:00}:{1:00}";

	[SerializeField]
	private GameObject m_enabledHierarchy;

	[SerializeField]
	private float m_maxDistance;

	private GameObject m_toTrack;

	public void SetTracker(tk2dCamera uiCamera, GameObject toTrack, float radius)
	{
		m_worldToScreenTracker.SetTracking(m_previewSphere.transform, uiCamera);
		m_previewSphere.transform.localScale = Vector3.one * radius;
		m_toTrack = toTrack;
		m_enabledHierarchy.SetActive(value: false);
		tk2dTextMesh[] counterTextMeshes = m_counterTextMeshes;
		foreach (tk2dTextMesh tk2dTextMesh2 in counterTextMeshes)
		{
			tk2dTextMesh2.SetText(string.Empty);
		}
	}

	public void SetCountdown(float countdownNormalised, float countdownSeconds)
	{
		if (countdownSeconds < 0f)
		{
			countdownSeconds = 0f;
		}
		int num = Mathf.FloorToInt(countdownSeconds);
		float num2 = Mathf.Clamp((countdownSeconds - (float)num) * 100f, 0f, 99f);
		tk2dTextMesh[] counterTextMeshes = m_counterTextMeshes;
		foreach (tk2dTextMesh tk2dTextMesh2 in counterTextMeshes)
		{
			tk2dTextMesh2.SetText(string.Format(m_countdownFormat, num, num2));
		}
	}

	public void SetNoCountdown()
	{
		tk2dTextMesh[] counterTextMeshes = m_counterTextMeshes;
		foreach (tk2dTextMesh tk2dTextMesh2 in counterTextMeshes)
		{
			tk2dTextMesh2.SetText(string.Empty);
		}
	}

	private void LateUpdate()
	{
		Vector3 vector = Singleton<BomberSpawn>.Instance.GetBomberSystems().transform.position - m_toTrack.transform.position;
		vector.y = 0f;
		m_previewSphere.transform.position = m_toTrack.transform.position;
		m_enabledHierarchy.SetActive(vector.magnitude < m_maxDistance);
	}
}
