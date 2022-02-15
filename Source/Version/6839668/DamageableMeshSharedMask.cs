using System.Collections.Generic;
using BomberCrewCommon;
using UnityEngine;

public class DamageableMeshSharedMask : MonoBehaviour
{
	public class DamageToUpdateWith
	{
		public Vector2 m_centrePoint;

		public Texture2D m_stampTexture;

		public float m_size;

		public bool m_undoDamage;
	}

	public class MeshInfo
	{
		public int[] m_meshTris;

		public Vector3[] m_meshVerts;

		public Vector2[] m_meshUVs;
	}

	[SerializeField]
	private Vector2 m_textureSize = new Vector2(1024f, 512f);

	[SerializeField]
	private Material m_drawToBufferMaterial;

	[SerializeField]
	private float m_damageAlphaMin = 0.3f;

	[SerializeField]
	private float m_damageAlphaMax = 1f;

	[SerializeField]
	private SharedMaterialHolder m_sharedMaterialHolder;

	[SerializeField]
	private Material[] m_materialsToSetUp;

	[SerializeField]
	private string[] m_shaderPropertyNamePerMaterial;

	[SerializeField]
	private float m_worldToUVScale = 0.01f;

	private RenderTexture m_texture;

	private List<DamageToUpdateWith> m_nextFrameSplats = new List<DamageToUpdateWith>();

	public void RegisterMesh(MeshCollider mc, Mesh m)
	{
	}

	private void Awake()
	{
		m_texture = new RenderTexture((int)m_textureSize.x, (int)m_textureSize.y, 0);
		m_texture.filterMode = FilterMode.Point;
		m_texture.wrapMode = TextureWrapMode.Repeat;
		RenderTexture.active = m_texture;
		GL.Clear(clearDepth: true, clearColor: true, Color.black);
		RenderTexture.active = null;
		int num = 0;
		Material[] materialsToSetUp = m_materialsToSetUp;
		foreach (Material inMat in materialsToSetUp)
		{
			Material @for = m_sharedMaterialHolder.GetFor(inMat);
			@for.SetTexture(m_shaderPropertyNamePerMaterial[num], GetTexture());
			num++;
		}
		m_drawToBufferMaterial = Object.Instantiate(m_drawToBufferMaterial);
	}

	public void DamageFromRaycastHit(RaycastHit rch, Texture2D texture, float hitRadius)
	{
		float size = hitRadius * m_worldToUVScale;
		DamageToUpdateWith damageToUpdateWith = new DamageToUpdateWith();
		damageToUpdateWith.m_centrePoint = rch.textureCoord;
		damageToUpdateWith.m_size = size;
		damageToUpdateWith.m_stampTexture = texture;
		Add(damageToUpdateWith);
	}

	public void UndoDamage()
	{
		for (int i = 0; i < 4; i++)
		{
			DamageToUpdateWith damageToUpdateWith = new DamageToUpdateWith();
			damageToUpdateWith.m_centrePoint = new Vector2(Random.Range(0f, 1f), Random.Range(0f, 1f));
			damageToUpdateWith.m_size = 2.5f;
			damageToUpdateWith.m_stampTexture = Singleton<DamageShapes>.Instance.GetShape();
			damageToUpdateWith.m_undoDamage = true;
			Add(damageToUpdateWith);
		}
	}

	public void DamageFromUVSphere(Vector3 uvCtr, float hitRadius, float areaRadius, int numHits, Texture2D stampTexture)
	{
		for (int i = 0; i < numHits; i++)
		{
			float size = hitRadius * m_worldToUVScale;
			DamageToUpdateWith damageToUpdateWith = new DamageToUpdateWith();
			damageToUpdateWith.m_centrePoint = uvCtr + (Vector3)Random.insideUnitCircle * (areaRadius * m_worldToUVScale);
			damageToUpdateWith.m_size = size;
			damageToUpdateWith.m_stampTexture = stampTexture;
			Add(damageToUpdateWith);
		}
	}

	public RenderTexture GetTexture()
	{
		return m_texture;
	}

	public void Add(DamageToUpdateWith splatToAdd)
	{
		m_nextFrameSplats.Add(splatToAdd);
	}

	private void Update()
	{
		if (m_nextFrameSplats.Count <= 0)
		{
			return;
		}
		RenderTexture.active = m_texture;
		GL.PushMatrix();
		GL.LoadPixelMatrix(0f, (int)m_textureSize.x, (int)m_textureSize.y, 0f);
		foreach (DamageToUpdateWith nextFrameSplat in m_nextFrameSplats)
		{
			GL.PushMatrix();
			GL.MultMatrix(Matrix4x4.TRS(new Vector3(nextFrameSplat.m_centrePoint.x * m_textureSize.x, m_textureSize.y - nextFrameSplat.m_centrePoint.y * m_textureSize.y, 0f), Quaternion.AngleAxis(0f, Vector3.forward), Vector3.one));
			float num = nextFrameSplat.m_size * m_textureSize.x;
			float num2 = nextFrameSplat.m_size * m_textureSize.y;
			if (nextFrameSplat.m_undoDamage)
			{
				m_drawToBufferMaterial.SetColor("_Tint", new Color(0f, 0f, 0f, Random.Range(0.15f, 0.35f)));
			}
			else
			{
				m_drawToBufferMaterial.SetColor("_Tint", new Color(1f, 1f, 1f, Random.Range(m_damageAlphaMin, m_damageAlphaMax)));
			}
			Graphics.DrawTexture(new Rect((0f - num) / 2f, (0f - num2) / 2f, num, num2), nextFrameSplat.m_stampTexture, m_drawToBufferMaterial);
			GL.PopMatrix();
		}
		GL.PopMatrix();
		RenderTexture.active = null;
		m_nextFrameSplats.Clear();
	}
}
