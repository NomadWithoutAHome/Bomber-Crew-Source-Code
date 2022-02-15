using System;
using UnityEngine;

public class AnimateMaterialTextureOffsetMulti : MonoBehaviour
{
	[Serializable]
	private class TextureOffsetSettings
	{
		[SerializeField]
		private string m_textureProperty;

		[SerializeField]
		private Vector2 m_offsetAmount;

		private Vector2 m_offset;

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

		public Vector2 OffsetAmount
		{
			get
			{
				return m_offsetAmount;
			}
			set
			{
				m_offsetAmount = value;
			}
		}

		public Vector2 Offset
		{
			get
			{
				return m_offset;
			}
			set
			{
				m_offset = value;
			}
		}
	}

	[SerializeField]
	private TextureOffsetSettings[] m_offsetSettings;

	[SerializeField]
	private Renderer m_renderer;

	private Material m_material;

	private void Start()
	{
		m_material = m_renderer.material;
		TextureOffsetSettings[] offsetSettings = m_offsetSettings;
		foreach (TextureOffsetSettings textureOffsetSettings in offsetSettings)
		{
			textureOffsetSettings.Offset = m_material.GetTextureOffset(textureOffsetSettings.TextureProperty);
		}
	}

	private void Update()
	{
		TextureOffsetSettings[] offsetSettings = m_offsetSettings;
		foreach (TextureOffsetSettings textureOffsetSettings in offsetSettings)
		{
			Vector2 textureOffset = m_material.GetTextureOffset(textureOffsetSettings.TextureProperty);
			Vector2 offset = new Vector2(textureOffset.x + textureOffsetSettings.OffsetAmount.x, textureOffset.y + textureOffsetSettings.OffsetAmount.y);
			m_material.SetTextureOffset(textureOffsetSettings.TextureProperty, WrapOffsetValues(offset));
		}
	}

	private Vector2 WrapOffsetValues(Vector2 offset)
	{
		Vector2 result = new Vector2(offset.x, offset.y);
		if (result.x >= 1f)
		{
			result.x -= 1f;
		}
		else if (result.x <= -1f)
		{
			result.x += 1f;
		}
		if (result.y >= 1f)
		{
			result.y -= 1f;
		}
		else if (result.y <= -1f)
		{
			result.y += 1f;
		}
		return result;
	}
}
