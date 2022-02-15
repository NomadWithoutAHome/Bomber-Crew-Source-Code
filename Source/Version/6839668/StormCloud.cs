using UnityEngine;

public class StormCloud : MissionPlaceableObject
{
	[SerializeField]
	private float m_maxScale = 1.25f;

	[SerializeField]
	private Material m_closeMaterial;

	[SerializeField]
	private Material m_farMaterial;

	[SerializeField]
	private float m_materialSwitchRadius = 2500f;

	[SerializeField]
	private Transform m_centreTransform;

	[SerializeField]
	private Renderer m_rendererToSwitch;

	private Camera m_mainCamera;

	private bool m_isCloseMaterial;

	public void Start()
	{
		base.transform.localScale = new Vector3(Random.Range(1f, m_maxScale), 1f, Random.Range(1f, m_maxScale));
		m_mainCamera = Camera.main;
	}

	private void Update()
	{
		bool flag = (m_centreTransform.position - m_mainCamera.transform.position).magnitude < m_materialSwitchRadius;
		if (m_isCloseMaterial != flag)
		{
			m_isCloseMaterial = flag;
			if (flag)
			{
				m_rendererToSwitch.sharedMaterial = m_closeMaterial;
			}
			else
			{
				m_rendererToSwitch.sharedMaterial = m_farMaterial;
			}
		}
	}
}
