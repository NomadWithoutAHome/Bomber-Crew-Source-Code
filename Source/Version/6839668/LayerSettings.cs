using UnityEngine;

[CreateAssetMenu(menuName = "Runner Duck/Layer Settings")]
public class LayerSettings : ScriptableObject
{
	[SerializeField]
	private LayerMask m_layerMask;

	public LayerMask GetLayerMask()
	{
		return m_layerMask;
	}
}
