using System;
using BomberCrewCommon;
using UnityEngine;

[CreateAssetMenu(menuName = "Bomber Crew/Font Group")]
public class FontGroup : ScriptableObject
{
	[Serializable]
	public class ControllerTypeReplacement
	{
		[SerializeField]
		public ControlPromptDisplayHelpers.ControllerMapSpriteType m_spriteType;

		[SerializeField]
		public tk2dFontData m_font;

		[SerializeField]
		public Texture2D m_replacementTexture;
	}

	[Serializable]
	public class FontTextureReplacement
	{
		[SerializeField]
		private tk2dFontData m_font;

		[SerializeField]
		private ControllerTypeReplacement[] m_replacement;

		[SerializeField]
		private int m_forLanguageFlags;

		public int GetLanguageFlag()
		{
			return m_forLanguageFlags;
		}

		public tk2dFontData GetFont(ControlPromptDisplayHelpers.ControllerMapSpriteType cType)
		{
			tk2dFontData font = m_font;
			ControllerTypeReplacement[] replacement = m_replacement;
			foreach (ControllerTypeReplacement controllerTypeReplacement in replacement)
			{
				if (controllerTypeReplacement.m_spriteType == cType)
				{
					if (controllerTypeReplacement.m_font != null)
					{
						font = controllerTypeReplacement.m_font;
					}
					break;
				}
			}
			return font;
		}

		public void SetControllerTexture(tk2dFontData font, ControlPromptDisplayHelpers.ControllerMapSpriteType cType)
		{
			ControllerTypeReplacement[] replacement = m_replacement;
			foreach (ControllerTypeReplacement controllerTypeReplacement in replacement)
			{
				if (controllerTypeReplacement.m_spriteType == cType)
				{
					if (controllerTypeReplacement.m_replacementTexture != null)
					{
						font.materialInst.mainTexture = controllerTypeReplacement.m_replacementTexture;
					}
					break;
				}
			}
		}
	}

	[SerializeField]
	private FontTextureReplacement[] m_replacements;

	[SerializeField]
	private tk2dFontData m_baseFont;

	private int m_lastLanguageFlag = -1;

	private ControlPromptDisplayHelpers.ControllerMapSpriteType m_lastControllerType;

	private bool m_hasCached;

	private tk2dFontData m_cached;

	public static event Action OnFontGroupLikelyChanged;

	public static void UpdateFontGroup()
	{
		if (FontGroup.OnFontGroupLikelyChanged != null)
		{
			FontGroup.OnFontGroupLikelyChanged();
		}
	}

	public tk2dFontData GetReplacementData(int overrideLangFlag)
	{
		int num = 1;
		if (Application.isPlaying)
		{
			num = Singleton<LanguageLoader>.Instance.GetLanguageFontReference();
		}
		if (overrideLangFlag != 0)
		{
			num = overrideLangFlag;
		}
		ControlPromptDisplayHelpers.ControllerMapSpriteType mapSpriteType = ControlPromptDisplayHelpers.GetMapSpriteType();
		if (m_hasCached && (m_lastLanguageFlag != num || mapSpriteType != m_lastControllerType))
		{
			m_hasCached = false;
		}
		if (!m_hasCached)
		{
			m_cached = m_baseFont;
			bool flag = false;
			FontTextureReplacement[] replacements = m_replacements;
			foreach (FontTextureReplacement fontTextureReplacement in replacements)
			{
				if ((fontTextureReplacement.GetLanguageFlag() & num) != 0)
				{
					m_cached = fontTextureReplacement.GetFont(mapSpriteType);
					fontTextureReplacement.SetControllerTexture(m_cached, mapSpriteType);
					flag = true;
				}
			}
			if (!flag)
			{
				FontTextureReplacement[] replacements2 = m_replacements;
				foreach (FontTextureReplacement fontTextureReplacement2 in replacements2)
				{
					m_cached = fontTextureReplacement2.GetFont(mapSpriteType);
					fontTextureReplacement2.SetControllerTexture(m_cached, mapSpriteType);
				}
			}
			m_hasCached = true;
			m_lastLanguageFlag = num;
			m_lastControllerType = mapSpriteType;
		}
		return m_cached;
	}
}
