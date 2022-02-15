using UnityEngine;

public class AssignDamageMaskTexture : MonoBehaviour
{
	[SerializeField]
	private Renderer[] m_shadowMeshRenderers;

	[SerializeField]
	private string m_shaderPropertyName = "_MainTex";

	[SerializeField]
	private SharedMaterialHolder m_sharedMask;

	private void Start()
	{
		Renderer[] shadowMeshRenderers = m_shadowMeshRenderers;
		foreach (Renderer renderer in shadowMeshRenderers)
		{
			Material material = (renderer.sharedMaterial = m_sharedMask.GetFor(renderer.sharedMaterial));
		}
	}
}
