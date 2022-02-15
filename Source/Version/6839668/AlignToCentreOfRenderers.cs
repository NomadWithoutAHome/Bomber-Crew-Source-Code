using UnityEngine;

[ExecuteInEditMode]
public class AlignToCentreOfRenderers : MonoBehaviour
{
	[SerializeField]
	private Renderer[] m_renderers;

	[ContextMenu("Align")]
	public void Align()
	{
		Bounds bounds = m_renderers[0].bounds;
		for (int i = 1; i < m_renderers.Length; i++)
		{
			bounds.Encapsulate(m_renderers[i].bounds);
		}
		Renderer[] renderers = m_renderers;
		foreach (Renderer renderer in renderers)
		{
			if (renderer.gameObject.activeInHierarchy)
			{
				renderer.transform.localPosition = new Vector3(renderer.transform.localPosition.x - bounds.center.x, renderer.transform.localPosition.y, renderer.transform.localPosition.z);
			}
		}
	}
}
