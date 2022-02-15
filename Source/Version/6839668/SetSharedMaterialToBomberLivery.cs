using BomberCrewCommon;
using UnityEngine;

public class SetSharedMaterialToBomberLivery : MonoBehaviour
{
	[SerializeField]
	private SharedMaterialHolder m_sharedMask;

	[SerializeField]
	private Material[] m_allMaterials;

	private BomberLivery m_overrideLivery;

	private void Start()
	{
		Texture2D mainTexture = ((!(m_overrideLivery == null)) ? m_overrideLivery.GetCurrentLiveryTexture() : Singleton<BomberContainer>.Instance.GetLivery().GetCurrentLiveryTexture());
		Material[] allMaterials = m_allMaterials;
		foreach (Material inMat in allMaterials)
		{
			m_sharedMask.GetFor(inMat).mainTexture = mainTexture;
		}
	}

	public void SetOverrideLivery(BomberLivery bl)
	{
		m_overrideLivery = bl;
	}
}
