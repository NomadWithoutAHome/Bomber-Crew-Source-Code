using UnityEngine;

public class AnimateMaterialTextureOffset : MonoBehaviour
{
	[SerializeField]
	private Vector2 m_offset;

	[SerializeField]
	private Renderer m_renderer;

	private void Start()
	{
		m_offset = m_renderer.material.mainTextureOffset;
	}

	private void Update()
	{
		m_renderer.material.mainTextureOffset = m_offset;
	}
}
