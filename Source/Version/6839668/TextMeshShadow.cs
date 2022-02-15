using System;
using UnityEngine;

[RequireComponent(typeof(tk2dTextMesh))]
public class TextMeshShadow : MonoBehaviour
{
	[SerializeField]
	private Vector3 m_offset = new Vector3(2f, -2f, 0f);

	[SerializeField]
	private Color m_tint = new Color(0f, 0f, 0f, 0.5f);

	[SerializeField]
	private GameObject m_createdObject;

	private tk2dTextMesh m_copyFromTextObject;

	private MeshFilter m_copyFromMeshFilter;

	private MeshRenderer m_copyFromMeshRenderer;

	private void Awake()
	{
		m_copyFromTextObject = GetComponent<tk2dTextMesh>();
		m_copyFromMeshRenderer = GetComponent<MeshRenderer>();
		m_copyFromMeshFilter = GetComponent<MeshFilter>();
		tk2dTextMesh copyFromTextObject = m_copyFromTextObject;
		copyFromTextObject.OnRefresh = (Action)Delegate.Combine(copyFromTextObject.OnRefresh, new Action(Refresh));
		Refresh();
	}

	private void OnDestroy()
	{
		tk2dTextMesh copyFromTextObject = m_copyFromTextObject;
		copyFromTextObject.OnRefresh = (Action)Delegate.Remove(copyFromTextObject.OnRefresh, new Action(Refresh));
	}

	public void Refresh()
	{
		if (m_copyFromTextObject == null)
		{
			m_copyFromTextObject = GetComponent<tk2dTextMesh>();
			m_copyFromMeshRenderer = GetComponent<MeshRenderer>();
			m_copyFromMeshFilter = GetComponent<MeshFilter>();
		}
		if (m_createdObject == null)
		{
			m_createdObject = new GameObject("ShadowMesh");
			m_createdObject.transform.parent = base.transform;
			m_createdObject.transform.localScale = Vector3.one;
			m_createdObject.transform.localRotation = Quaternion.identity;
			m_createdObject.AddComponent<MeshFilter>();
			m_createdObject.AddComponent<MeshRenderer>();
		}
		m_createdObject.layer = base.gameObject.layer;
		m_createdObject.transform.localPosition = m_offset;
		m_createdObject.transform.localRotation = Quaternion.identity;
		if (m_copyFromMeshFilter.sharedMesh != null)
		{
			Mesh mesh = UnityEngine.Object.Instantiate(m_copyFromMeshFilter.sharedMesh);
			Color[] colors = mesh.colors;
			for (int i = 0; i < colors.Length; i++)
			{
				ref Color reference = ref colors[i];
				reference = m_tint;
			}
			mesh.colors = colors;
			MeshRenderer component = m_createdObject.GetComponent<MeshRenderer>();
			m_createdObject.GetComponent<MeshFilter>().sharedMesh = mesh;
			component.sharedMaterial = m_copyFromMeshRenderer.sharedMaterial;
			component.sortingLayerName = m_copyFromMeshRenderer.sortingLayerName;
			component.sortingOrder = m_copyFromMeshRenderer.sortingOrder - 1;
		}
	}
}
