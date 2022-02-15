using BomberCrewCommon;
using UnityEngine;

public class SideTransparencySet : MonoBehaviour
{
	[SerializeField]
	private SharedMaterialHolder m_materialHolder;

	[SerializeField]
	private Material m_startingMaterial;

	[SerializeField]
	private float m_alphaWhenHidden = 0.25f;

	private Material m_correctMaterial;

	private float m_currentAlpha;

	private void Start()
	{
		m_correctMaterial = m_materialHolder.GetFor(m_startingMaterial);
		m_currentAlpha = 1f;
	}

	private void Update()
	{
		float num = ((!Singleton<BomberCamera>.Instance.ShouldShowSides()) ? m_alphaWhenHidden : 1f);
		float num2 = num - m_currentAlpha;
		m_currentAlpha += num2 * Mathf.Clamp01(Time.deltaTime * 10f);
		m_correctMaterial.SetFloat("_CutMultiplier", m_currentAlpha);
	}
}
