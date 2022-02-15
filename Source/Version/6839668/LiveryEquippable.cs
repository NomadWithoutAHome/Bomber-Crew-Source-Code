using BomberCrewCommon;
using UnityEngine;

[CreateAssetMenu(menuName = "Bomber Crew/Bomber Upgrades/Livery")]
public class LiveryEquippable : EquipmentUpgradeFittableBase
{
	[SerializeField]
	private string m_textureResourcesName;

	[SerializeField]
	private Texture2D m_texture;

	[SerializeField]
	private tk2dFontData m_font;

	[SerializeField]
	private tk2dFontData m_fontSmall;

	[SerializeField]
	private string m_liveryCategoryId;

	[SerializeField]
	private string m_subVariantId;

	[SerializeField]
	private float m_alpha = 1f;

	[SerializeField]
	private bool m_maintainDirection;

	[SerializeField]
	private int m_rotate;

	[SerializeField]
	private Color m_color = Color.white;

	[SerializeField]
	private Material m_referenceMaterial;

	[SerializeField]
	private bool m_customRound;

	public string GetSubVariant()
	{
		return m_subVariantId;
	}

	public override BomberUpgradeType GetUpgradeType()
	{
		return BomberUpgradeType.Livery;
	}

	public Material GetReferenceMaterial()
	{
		return m_referenceMaterial;
	}

	public int GetRotate()
	{
		return m_rotate;
	}

	public bool UseRoundMask()
	{
		return m_customRound;
	}

	public Texture2D GetTexture()
	{
		if (m_texture == null)
		{
			if (m_textureResourcesName.Contains(","))
			{
				string[] array = m_textureResourcesName.Split(",".ToCharArray());
				return Singleton<DLCManager>.Instance.LoadAssetFromBundle<Texture2D>(array[0], array[1]);
			}
			if (m_textureResourcesName == "PROFILE")
			{
				return null;
			}
			if (m_textureResourcesName.StartsWith("CUSTOM_"))
			{
				return Singleton<CustomLiveryTextures>.Instance.GetCustomLivery(GetCustomRef(), GetCustomIndex());
			}
			return (Texture2D)Resources.Load(m_textureResourcesName);
		}
		return m_texture;
	}

	public ResourceRequest GetTextureRR(out Texture2D quickResult)
	{
		if (m_texture == null)
		{
			if (m_textureResourcesName.Contains(","))
			{
				string[] array = m_textureResourcesName.Split(",".ToCharArray());
				quickResult = Singleton<DLCManager>.Instance.LoadAssetFromBundle<Texture2D>(array[0], array[1]);
				return null;
			}
			if (m_textureResourcesName == "PROFILE")
			{
				quickResult = null;
				return null;
			}
			if (m_textureResourcesName.StartsWith("CUSTOM_"))
			{
				quickResult = Singleton<CustomLiveryTextures>.Instance.GetCustomLivery(GetCustomRef(), GetCustomIndex());
				return null;
			}
			quickResult = null;
			return Resources.LoadAsync(m_textureResourcesName);
		}
		quickResult = m_texture;
		return null;
	}

	public bool IsCustom()
	{
		return m_textureResourcesName.StartsWith("CUSTOM_");
	}

	public string GetCustomRef()
	{
		if (m_textureResourcesName.StartsWith("CUSTOM_"))
		{
			string text = m_textureResourcesName.Replace("CUSTOM_", string.Empty);
			return text.Substring(0, 1);
		}
		return string.Empty;
	}

	public int GetCustomIndex()
	{
		if (m_textureResourcesName.StartsWith("CUSTOM_"))
		{
			string text = m_textureResourcesName.Replace("CUSTOM_", string.Empty);
			string s = text.Substring(1);
			return int.Parse(s);
		}
		return 0;
	}

	public Vector3 AdjustSize(Vector3 sizeIn)
	{
		if (sizeIn.x * sizeIn.y < 0f && m_maintainDirection)
		{
			sizeIn.x *= -1f;
		}
		return sizeIn;
	}

	public Color GetColor()
	{
		return m_color;
	}

	public tk2dFontData GetFont(bool small)
	{
		if (small)
		{
			return m_fontSmall;
		}
		return m_font;
	}

	public override bool IsDisplayableFor(BomberRequirements.BomberEquipmentRequirement requirement)
	{
		if (m_textureResourcesName == "PROFILE")
		{
			return false;
		}
		return requirement.GetUniquePartId() == m_liveryCategoryId;
	}

	public float GetAlpha()
	{
		return m_alpha;
	}
}
