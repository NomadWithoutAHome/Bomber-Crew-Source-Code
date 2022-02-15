using UnityEngine;

public class UniqifyMaterialOnAwake : MonoBehaviour
{
	[SerializeField]
	private Renderer[] m_renderers;

	private void Awake()
	{
		Renderer[] renderers = m_renderers;
		foreach (Renderer renderer in renderers)
		{
			Material[] sharedMaterials = renderer.sharedMaterials;
			for (int j = 0; j < sharedMaterials.Length; j++)
			{
				sharedMaterials[j] = Object.Instantiate(sharedMaterials[j]);
			}
			renderer.sharedMaterials = sharedMaterials;
		}
	}
}
