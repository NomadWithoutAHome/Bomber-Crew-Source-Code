using UnityEngine;
using UnityEngine.Rendering;

public class OutlineMesh : MonoBehaviour
{
	[SerializeField]
	private Material m_outlineMaterial;

	[SerializeField]
	private Renderer m_rendererToClone;

	private bool m_isInitialised;

	private Renderer m_clonedRenderer;

	private Material[] m_sharedMaterials;

	private GameObject m_clonedObject;

	public Renderer GetClonedRenderer()
	{
		Init();
		return m_clonedRenderer;
	}

	public Material[] GetSharedMaterials()
	{
		Init();
		return m_sharedMaterials;
	}

	private void OnDestroy()
	{
		if (m_isInitialised && m_clonedObject != null)
		{
			Object.Destroy(m_clonedObject);
		}
	}

	private void Init()
	{
		if (!m_isInitialised)
		{
			m_isInitialised = true;
			GameObject gameObject = new GameObject("OutlineClone");
			gameObject.transform.parent = base.transform.parent;
			gameObject.layer = LayerMask.NameToLayer("CrewmanOutlines");
			m_clonedObject = gameObject;
			Renderer renderer = ((!(m_rendererToClone != null)) ? GetComponent<Renderer>() : m_rendererToClone);
			Renderer renderer2 = null;
			if (renderer is SkinnedMeshRenderer)
			{
				SkinnedMeshRenderer skinnedMeshRenderer = (SkinnedMeshRenderer)renderer;
				SkinnedMeshRenderer skinnedMeshRenderer2 = gameObject.AddComponent<SkinnedMeshRenderer>();
				skinnedMeshRenderer2.rootBone = skinnedMeshRenderer.rootBone;
				skinnedMeshRenderer2.localBounds = skinnedMeshRenderer.localBounds;
				skinnedMeshRenderer2.bones = skinnedMeshRenderer.bones;
				skinnedMeshRenderer2.sharedMesh = skinnedMeshRenderer.sharedMesh;
				renderer2 = skinnedMeshRenderer2;
			}
			else
			{
				MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
				MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
				meshFilter.sharedMesh = GetComponent<MeshFilter>().sharedMesh;
				renderer2 = meshRenderer;
			}
			m_sharedMaterials = new Material[renderer.sharedMaterials.Length];
			Material material = Object.Instantiate(m_outlineMaterial);
			for (int i = 0; i < renderer.sharedMaterials.Length; i++)
			{
				m_sharedMaterials[i] = material;
			}
			renderer2.shadowCastingMode = ShadowCastingMode.Off;
			renderer2.receiveShadows = false;
			renderer2.sharedMaterials = m_sharedMaterials;
			renderer2.enabled = false;
			m_clonedRenderer = renderer2;
			gameObject.transform.localPosition = base.transform.localPosition;
			gameObject.transform.localScale = base.transform.localScale;
			gameObject.transform.localRotation = base.transform.localRotation;
		}
	}

	private void Start()
	{
		Init();
	}
}
