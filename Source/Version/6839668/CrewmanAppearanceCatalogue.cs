using System;
using BomberCrewCommon;
using UnityEngine;

public class CrewmanAppearanceCatalogue : Singleton<CrewmanAppearanceCatalogue>
{
	[Serializable]
	private struct TextureAreaBounds
	{
		public int x;

		public int y;

		public int width;

		public int height;

		public TextureAreaBounds(int x, int y, int width, int height)
		{
			this.x = x;
			this.y = y;
			this.width = width;
			this.height = height;
		}
	}

	[SerializeField]
	private Texture2D m_crewmanTexture;

	[SerializeField]
	private TextureAreaBounds m_clothingTextureArea;

	[SerializeField]
	private int m_indexCountClothing = 16;

	[SerializeField]
	private int m_maxCivviesClothingIndex = 4;

	[SerializeField]
	private TextureAreaBounds m_skinTextureArea;

	[SerializeField]
	private int m_indexCountSkin = 8;

	[SerializeField]
	private int m_indexCountHair = 8;

	[SerializeField]
	private int m_indexCountEyes = 32;

	[SerializeField]
	private int m_indexCountMouth = 32;

	[SerializeField]
	private Mesh[] m_maleHairVariation;

	[SerializeField]
	private Mesh[] m_femaleHairVariation;

	[SerializeField]
	private Mesh[] m_maleFacialHairVariation;

	[SerializeField]
	private string[] m_voiceJabbersMale;

	[SerializeField]
	private string[] m_voiceJabbersFemale;

	[SerializeField]
	private string[] m_voiceJabbersMaleUS;

	[SerializeField]
	private string[] m_voiceJabbersFemaleUS;

	[SerializeField]
	private GameObject m_crewmanEditPrefab;

	public GameObject GetEditPrefab()
	{
		return m_crewmanEditPrefab;
	}

	public int GetIndexCountClothing()
	{
		return m_indexCountClothing;
	}

	public int GetMaxCivviesIndex()
	{
		return m_maxCivviesClothingIndex;
	}

	public int GetIndexCountSkin()
	{
		return m_indexCountSkin;
	}

	public int GetIndexCountHair()
	{
		return m_indexCountHair;
	}

	public int GetIndexCountEyes()
	{
		return m_indexCountEyes;
	}

	public int GetIndexCountMouth()
	{
		return m_indexCountMouth;
	}

	public int GetMaxJabbers(Crewman.ModelType mt)
	{
		if (mt == Crewman.ModelType.Female)
		{
			return m_voiceJabbersFemale.Length;
		}
		return m_voiceJabbersMale.Length;
	}

	public string GetJabberEvent(Crewman.ModelType mt, int index)
	{
		if (Singleton<GameFlow>.Instance.GetGameMode().GetUseUSNaming())
		{
			if (mt == Crewman.ModelType.Female)
			{
				return m_voiceJabbersFemaleUS[index % m_voiceJabbersFemale.Length];
			}
			return m_voiceJabbersMaleUS[index % m_voiceJabbersMale.Length];
		}
		if (mt == Crewman.ModelType.Female)
		{
			return m_voiceJabbersFemale[index % m_voiceJabbersFemale.Length];
		}
		return m_voiceJabbersMale[index % m_voiceJabbersMale.Length];
	}

	public int GetNumHairVariation(Crewman.ModelType mt)
	{
		if (mt == Crewman.ModelType.Female)
		{
			return m_femaleHairVariation.Length;
		}
		return m_maleHairVariation.Length;
	}

	public int GetNumFacialHairVariation()
	{
		return m_maleFacialHairVariation.Length;
	}

	public Mesh GetHairMesh(Crewman.ModelType mt, int index)
	{
		if (mt == Crewman.ModelType.Female)
		{
			return m_femaleHairVariation[index % m_femaleHairVariation.Length];
		}
		return m_maleHairVariation[index % m_maleHairVariation.Length];
	}

	public Mesh GetFacialHairMesh(int index)
	{
		return m_maleFacialHairVariation[index % m_maleFacialHairVariation.Length];
	}

	public Vector2 GetOffsetForClothingIndex(int index)
	{
		Vector2 result = new Vector2(0f, 0f);
		float num = (result.y = 0f - 1f / (float)GetIndexCountClothing() * (float)index);
		return result;
	}

	public Vector2 GetOffsetForSkinIndex(int index)
	{
		Vector2 result = new Vector2(0f, 0f);
		float num = (result.x = 0.125f * (float)index);
		return result;
	}

	public Vector2 GetOffsetForHairIndex(int index)
	{
		Vector2 result = new Vector2(0f, 0f);
		float num = (result.y = 0f - 0.0625f * (float)index);
		return result;
	}
}
