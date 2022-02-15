using UnityEngine;

public class ShowWalkPath : MonoBehaviour
{
	[SerializeField]
	private LineRenderer m_lineRenderer;

	[SerializeField]
	private FlashManager m_flashManager;

	private FlashManager.ActiveFlash m_activeFlash;

	public void ShowPath(CrewmanAvatar fromAvatar, BomberWalkZone.WalkPath fullPath, Vector3 actualEnd)
	{
		if (fullPath.m_path.Count > 0)
		{
			m_lineRenderer.positionCount = fullPath.m_path.Count + 3;
			m_lineRenderer.SetPosition(0, fromAvatar.transform.position);
			m_lineRenderer.SetPosition(1, fullPath.m_path[0].m_zone.GetNearestPlanarPoint(fromAvatar.transform.position));
			for (int i = 0; i < fullPath.m_path.Count; i++)
			{
				Vector3 pointFromLocal = fullPath.m_path[i].m_zone.GetPointFromLocal(fullPath.m_path[i].m_zoneLocalPosition);
				m_lineRenderer.SetPosition(i + 2, pointFromLocal);
			}
			m_lineRenderer.SetPosition(fullPath.m_path.Count + 2, actualEnd);
			m_lineRenderer.enabled = true;
		}
		else
		{
			m_lineRenderer.enabled = false;
		}
	}

	public void Flash()
	{
		m_activeFlash = m_flashManager.AddOrUpdateFlash(0.8f, 2.6f, 0f, 1, 1f, Color.white, m_activeFlash);
	}

	public void HidePath()
	{
		m_lineRenderer.enabled = false;
	}
}
