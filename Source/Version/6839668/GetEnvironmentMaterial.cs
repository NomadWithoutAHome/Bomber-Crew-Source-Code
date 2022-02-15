using BomberCrewCommon;
using UnityEngine;

public class GetEnvironmentMaterial : MonoBehaviour
{
	[SerializeField]
	private Renderer m_renderer;

	[SerializeField]
	private int m_materialIndex;

	private void Start()
	{
		m_renderer.sharedMaterial = Singleton<EnvironmentMaterialSetup>.Instance.GetMaterial(m_materialIndex);
	}
}
