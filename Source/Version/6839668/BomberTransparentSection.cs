using UnityEngine;

public class BomberTransparentSection : MonoBehaviour
{
	[SerializeField]
	private GameObject[] m_layerObjects;

	[SerializeField]
	private string m_layerNameNormal;

	[SerializeField]
	private string m_layerNameTransparent;

	[SerializeField]
	private Material m_transparentMaterial;

	[SerializeField]
	private Material m_regularMaterial;

	[SerializeField]
	private Renderer[] m_layerRenderers;

	[SerializeField]
	private SharedMaterialHolder m_sharedMaterialHolder;

	private int m_layerNormal;

	private int m_layerTransparent;

	private TransparentPostProcessEffect m_postEffect;

	private bool m_enabled;

	private bool m_hidden = true;

	private Material m_transparentMaterialShared;

	private Material m_regularMaterialShared;

	private void Start()
	{
		m_postEffect = Object.FindObjectOfType<TransparentPostProcessEffect>();
		if (m_postEffect != null)
		{
			m_enabled = true;
		}
		m_layerNormal = LayerMask.NameToLayer(m_layerNameNormal);
		m_layerTransparent = LayerMask.NameToLayer(m_layerNameTransparent);
		m_transparentMaterialShared = m_sharedMaterialHolder.GetFor(m_transparentMaterial);
		m_regularMaterialShared = m_sharedMaterialHolder.GetFor(m_regularMaterial);
	}

	private void Update()
	{
		if (!m_enabled)
		{
			return;
		}
		bool flag = m_postEffect.GetCurrentT() < 1f;
		if (flag != m_hidden)
		{
			GameObject[] layerObjects = m_layerObjects;
			foreach (GameObject gameObject in layerObjects)
			{
				gameObject.layer = ((!flag) ? m_layerNormal : m_layerTransparent);
			}
			Renderer[] layerRenderers = m_layerRenderers;
			foreach (Renderer renderer in layerRenderers)
			{
				renderer.material = ((!flag) ? m_regularMaterialShared : m_transparentMaterialShared);
			}
			m_hidden = flag;
		}
	}
}
