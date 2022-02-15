using BomberCrewCommon;
using UnityEngine;

public class EnvironmentMaterialSetup : Singleton<EnvironmentMaterialSetup>
{
	[SerializeField]
	private Material[] m_terrainMaterials;

	[SerializeField]
	private string m_texture1Property = "_MainTex";

	[SerializeField]
	private Texture2D m_texture1Snow;

	[SerializeField]
	private string m_texture2Property = "_TexR";

	[SerializeField]
	private Texture2D m_texture2Snow;

	[SerializeField]
	private Material[] m_environmentObjectsMaterials;

	[SerializeField]
	private string m_materialSnowProperty = "_SnowAmt";

	public void SetWinter(bool winter)
	{
		if (winter)
		{
			Material[] terrainMaterials = m_terrainMaterials;
			foreach (Material material in terrainMaterials)
			{
				material.SetTexture(m_texture1Property, m_texture1Snow);
				material.SetTexture(m_texture2Property, m_texture2Snow);
			}
			Material[] environmentObjectsMaterials = m_environmentObjectsMaterials;
			foreach (Material material2 in environmentObjectsMaterials)
			{
				material2.SetFloat(m_materialSnowProperty, 1f);
			}
		}
		else
		{
			Material[] terrainMaterials2 = m_terrainMaterials;
			foreach (Material material3 in terrainMaterials2)
			{
				material3.SetTexture(m_texture1Property, Singleton<GameFlow>.Instance.GetGameMode().GetEnvironmentGrassTexture());
				material3.SetTexture(m_texture2Property, Singleton<GameFlow>.Instance.GetGameMode().GetEnvironmentFieldsTexture());
			}
			Material[] environmentObjectsMaterials2 = m_environmentObjectsMaterials;
			foreach (Material material4 in environmentObjectsMaterials2)
			{
				material4.SetFloat(m_materialSnowProperty, 0f);
			}
		}
	}

	public Material GetMaterial(int index)
	{
		return m_terrainMaterials[index];
	}
}
