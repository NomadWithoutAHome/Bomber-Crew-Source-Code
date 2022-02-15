using System;
using BomberCrewCommon;
using UnityEngine;

public class CloudLayers : MonoBehaviour
{
	[SerializeField]
	private Renderer[] m_cloudLayerRenderer;

	private Material m_cloudLayerMaterialLower;

	private Material m_cloudLayerMaterialUpper;

	[SerializeField]
	private string m_TexturePropertyName = "_MainTex";

	[SerializeField]
	private string m_NoisePropertyName = "_NoiseTex";

	[SerializeField]
	private float m_noiseMovementMainSpeed;

	[SerializeField]
	private float m_noiseMovementAlternatorFrequency;

	[SerializeField]
	private float m_noiseMovementAlternatorAmplitude;

	[SerializeField]
	private float m_alphaMultPreMin = 0.1f;

	[SerializeField]
	private float m_alphaMultPreMax = 7f;

	[SerializeField]
	private Color m_colorNormal = new Color(0.5f, 0.5f, 0.5f, 1f);

	[SerializeField]
	private Color m_colorOverdark = new Color(0.45f, 0.45f, 0.45f, 1f);

	private Vector2 m_materialTextureOffset = Vector2.zero;

	private Vector2 m_noiseTextureOffset = Vector2.zero;

	private Vector2 m_noiseOffset = Vector2.zero;

	private Transform m_bomberTransform;

	private Vector2 m_noiseMovementDirection;

	private float m_noiseT;

	private float m_lowerDensityTarget;

	private float m_lowerDensity;

	private bool m_hasInitialSetting;

	private void Start()
	{
		m_cloudLayerMaterialLower = UnityEngine.Object.Instantiate(m_cloudLayerRenderer[0].sharedMaterial);
		m_cloudLayerRenderer[0].sharedMaterial = m_cloudLayerMaterialLower;
		m_cloudLayerMaterialUpper = UnityEngine.Object.Instantiate(m_cloudLayerRenderer[1].sharedMaterial);
		m_cloudLayerRenderer[1].sharedMaterial = m_cloudLayerMaterialUpper;
		m_noiseMovementDirection = new Vector2(UnityEngine.Random.Range(-1, 1), UnityEngine.Random.Range(-1, 1)).normalized;
		if (m_noiseMovementDirection.magnitude == 0f)
		{
			m_noiseMovementDirection = new Vector2(1f, 1f);
		}
	}

	public float GetLowerY()
	{
		return m_cloudLayerRenderer[0].transform.position.y;
	}

	public float GetUpperY()
	{
		return m_cloudLayerRenderer[1].transform.position.y;
	}

	public float GetLowerDensity()
	{
		return m_lowerDensity;
	}

	public void SetLowerDensityTarget(float target)
	{
		m_lowerDensityTarget = target;
		if (!m_hasInitialSetting)
		{
			m_hasInitialSetting = true;
			m_lowerDensity = target;
		}
	}

	private void LateUpdate()
	{
		m_noiseOffset += m_noiseMovementDirection * Time.deltaTime * m_noiseMovementMainSpeed;
		m_noiseT += Time.deltaTime * m_noiseMovementAlternatorFrequency;
		if (m_noiseT > (float)Math.PI * 2f)
		{
			m_noiseT -= (float)Math.PI * 2f;
		}
		m_noiseOffset += new Vector2(Mathf.Sin(m_noiseT), Mathf.Cos(m_noiseT)) * m_noiseMovementAlternatorAmplitude;
		if (m_bomberTransform == null)
		{
			m_bomberTransform = Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBomberState().transform;
		}
		if (m_bomberTransform != null)
		{
			base.transform.position = new Vector3(m_bomberTransform.position.x, 0f, m_bomberTransform.position.z);
			UpdateMaterial(m_cloudLayerMaterialLower);
			UpdateMaterial(m_cloudLayerMaterialUpper);
		}
		float num = m_lowerDensityTarget - m_lowerDensity;
		m_lowerDensity += Mathf.Clamp01(Time.deltaTime * 0.5f) * num;
		float t = Mathf.Clamp01(m_lowerDensity - 1f);
		Color value = Color.Lerp(m_colorNormal, m_colorOverdark, t);
		m_cloudLayerMaterialLower.SetFloat("_AlphaMultPre", Mathf.LerpUnclamped(m_alphaMultPreMin, m_alphaMultPreMax, Mathf.Pow(m_lowerDensity, 0.5f)));
		m_cloudLayerMaterialLower.SetColor("_TintColor", value);
	}

	private void UpdateMaterial(Material m)
	{
		Vector3d position = m_bomberTransform.gameObject.btransform().position;
		Vector3 vector = (Vector3)position;
		m_materialTextureOffset = new Vector2((0f - vector.x) * m.mainTextureScale.x, (0f - vector.z) * m.mainTextureScale.y) / m_cloudLayerRenderer[0].transform.localScale.x;
		m_noiseTextureOffset = new Vector2((0f - vector.x) * m.GetTextureScale(m_NoisePropertyName).x, (0f - vector.z) * m.GetTextureScale(m_NoisePropertyName).y) / m_cloudLayerRenderer[0].transform.localScale.x;
		m_noiseTextureOffset += m_noiseOffset;
		m_materialTextureOffset = WrapOffsetValue(m_materialTextureOffset);
		m_noiseOffset = WrapOffsetValue(m_noiseOffset);
		m_noiseTextureOffset = WrapOffsetValue(m_noiseTextureOffset);
		m.SetTextureOffset(m_TexturePropertyName, m_materialTextureOffset);
		m.SetTextureOffset(m_NoisePropertyName, m_noiseTextureOffset);
	}

	private Vector2 WrapOffsetValue(Vector2 inV)
	{
		if (inV.x > 1f)
		{
			inV.x -= 1f;
		}
		if (inV.x < 0f)
		{
			inV.x += 1f;
		}
		if (inV.y > 1f)
		{
			inV.y -= 1f;
		}
		if (inV.y < 0f)
		{
			inV.y += 1f;
		}
		return inV;
	}
}
