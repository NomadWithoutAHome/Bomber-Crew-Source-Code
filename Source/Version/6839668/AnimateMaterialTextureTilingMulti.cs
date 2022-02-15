using System;
using UnityEngine;

public class AnimateMaterialTextureTilingMulti : MonoBehaviour
{
	[Serializable]
	private class TextureTilingSettings
	{
		[SerializeField]
		private string m_textureProperty;

		[SerializeField]
		private Vector2 m_tilingAmount;

		[SerializeField]
		private float m_speed = 0.001f;

		private Vector2 m_tiling;

		public string TextureProperty
		{
			get
			{
				return m_textureProperty;
			}
			set
			{
				m_textureProperty = value;
			}
		}

		public Vector2 TilingAmount
		{
			get
			{
				return m_tilingAmount;
			}
			set
			{
				m_tilingAmount = value;
			}
		}

		public float Speed
		{
			get
			{
				return m_speed;
			}
			set
			{
				m_speed = value;
			}
		}

		public Vector2 Tiling
		{
			get
			{
				return m_tiling;
			}
			set
			{
				m_tiling = value;
			}
		}
	}

	[SerializeField]
	private TextureTilingSettings[] m_tilingSettings;

	[SerializeField]
	private Renderer m_renderer;

	private Material m_material;

	private void Start()
	{
		m_material = m_renderer.material;
		TextureTilingSettings[] tilingSettings = m_tilingSettings;
		foreach (TextureTilingSettings textureTilingSettings in tilingSettings)
		{
			textureTilingSettings.Tiling = m_material.GetTextureOffset(textureTilingSettings.TextureProperty);
		}
	}

	private void Update()
	{
		TextureTilingSettings[] tilingSettings = m_tilingSettings;
		foreach (TextureTilingSettings textureTilingSettings in tilingSettings)
		{
			Vector2 textureScale = m_material.GetTextureScale(textureTilingSettings.TextureProperty);
			if (textureTilingSettings.TilingAmount.x != 0f)
			{
				textureScale.x = 1f - (Mathf.Sin(Time.time * textureTilingSettings.Speed) * 0.5f + 0.5f) * textureTilingSettings.TilingAmount.x;
			}
			if (textureTilingSettings.TilingAmount.y != 0f)
			{
				textureScale.y = 1f - (Mathf.Sin(Time.time * textureTilingSettings.Speed) * 0.5f + 0.5f) * textureTilingSettings.TilingAmount.y;
			}
			m_material.SetTextureScale(textureTilingSettings.TextureProperty, textureScale);
		}
	}
}
