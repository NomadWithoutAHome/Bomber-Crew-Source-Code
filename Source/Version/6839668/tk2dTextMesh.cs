using System;
using System.Text;
using BomberCrewCommon;
using tk2dRuntime;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[AddComponentMenu("2D Toolkit/Text/tk2dTextMesh")]
public class tk2dTextMesh : TextSetter, ISpriteCollectionForceBuild
{
	[Flags]
	private enum UpdateFlags
	{
		UpdateNone = 0,
		UpdateText = 1,
		UpdateColors = 2,
		UpdateBuffers = 4
	}

	private tk2dFontData _fontInst;

	private string _formattedText = string.Empty;

	[SerializeField]
	private tk2dFontData _font;

	[SerializeField]
	private string _text = string.Empty;

	[SerializeField]
	private Color _color = Color.white;

	[SerializeField]
	private Color _color2 = Color.white;

	[SerializeField]
	private bool _useGradient;

	[SerializeField]
	private int _textureGradient;

	[SerializeField]
	private TextAnchor _anchor = TextAnchor.LowerLeft;

	[SerializeField]
	private Vector3 _scale = new Vector3(1f, 1f, 1f);

	[SerializeField]
	private bool _kerning;

	[SerializeField]
	private int _maxChars = 16;

	[SerializeField]
	private bool _inlineStyling;

	[SerializeField]
	private bool _allCaps;

	[SerializeField]
	private bool _formatting;

	[SerializeField]
	private int _wordWrapWidth;

	[SerializeField]
	private float spacing;

	[SerializeField]
	private float lineSpacing;

	private bool m_forceWesternLineBreaks;

	[SerializeField]
	private tk2dTextMeshData data = new tk2dTextMeshData();

	public Action OnRefresh;

	private Vector3[] vertices;

	private Vector2[] uvs;

	private Vector2[] uv2;

	private Color32[] colors;

	private Color32[] untintedColors;

	private UpdateFlags updateFlags = UpdateFlags.UpdateBuffers;

	private Mesh mesh;

	private MeshFilter meshFilter;

	private Renderer _cachedRenderer;

	private int m_overrideLangFlag;

	public string FormattedText => _formattedText;

	public tk2dFontData font
	{
		get
		{
			UpgradeData();
			return data.font;
		}
		set
		{
			UpgradeData();
			data.font = value;
			_fontInst = data.font.inst;
			SetNeedUpdate(UpdateFlags.UpdateText);
			UpdateMaterial();
		}
	}

	public bool formatting
	{
		get
		{
			UpgradeData();
			return data.formatting;
		}
		set
		{
			UpgradeData();
			if (data.formatting != value)
			{
				data.formatting = value;
				SetNeedUpdate(UpdateFlags.UpdateText);
			}
		}
	}

	public int wordWrapWidth
	{
		get
		{
			UpgradeData();
			return data.wordWrapWidth;
		}
		set
		{
			UpgradeData();
			if (data.wordWrapWidth != value)
			{
				data.wordWrapWidth = value;
				SetNeedUpdate(UpdateFlags.UpdateText);
			}
		}
	}

	public int maxHeight
	{
		get
		{
			UpgradeData();
			return data.areaHeight;
		}
		set
		{
			UpgradeData();
			if (data.areaHeight != value)
			{
				data.areaHeight = value;
				SetNeedUpdate(UpdateFlags.UpdateText);
			}
		}
	}

	public string text
	{
		get
		{
			UpgradeData();
			return data.text;
		}
		set
		{
			if (allCaps)
			{
				string text = value.ToUpper();
				UpgradeData();
				if (data.text != text)
				{
					data.text = text;
					SetNeedUpdate(UpdateFlags.UpdateText);
				}
			}
			else
			{
				UpgradeData();
				if (data.text != value)
				{
					data.text = value;
					SetNeedUpdate(UpdateFlags.UpdateText);
				}
			}
		}
	}

	public Color color
	{
		get
		{
			UpgradeData();
			return data.color;
		}
		set
		{
			UpgradeData();
			data.color = value;
			SetNeedUpdate(UpdateFlags.UpdateColors);
		}
	}

	public Color color2
	{
		get
		{
			UpgradeData();
			return data.color2;
		}
		set
		{
			UpgradeData();
			data.color2 = value;
			SetNeedUpdate(UpdateFlags.UpdateColors);
		}
	}

	public bool useGradient
	{
		get
		{
			UpgradeData();
			return data.useGradient;
		}
		set
		{
			UpgradeData();
			data.useGradient = value;
			SetNeedUpdate(UpdateFlags.UpdateColors);
		}
	}

	public TextAnchor anchor
	{
		get
		{
			UpgradeData();
			return data.anchor;
		}
		set
		{
			UpgradeData();
			data.anchor = value;
			SetNeedUpdate(UpdateFlags.UpdateText);
		}
	}

	public Vector3 scale
	{
		get
		{
			UpgradeData();
			return data.scale;
		}
		set
		{
			UpgradeData();
			data.scale = value;
			SetNeedUpdate(UpdateFlags.UpdateText);
		}
	}

	public bool kerning
	{
		get
		{
			UpgradeData();
			return data.kerning;
		}
		set
		{
			UpgradeData();
			data.kerning = value;
			SetNeedUpdate(UpdateFlags.UpdateText);
		}
	}

	public int maxChars
	{
		get
		{
			UpgradeData();
			return data.maxChars;
		}
		set
		{
			UpgradeData();
			data.maxChars = value;
			SetNeedUpdate(UpdateFlags.UpdateBuffers);
		}
	}

	public int textureGradient
	{
		get
		{
			UpgradeData();
			return data.textureGradient;
		}
		set
		{
			UpgradeData();
			data.textureGradient = value % font.gradientCount;
			SetNeedUpdate(UpdateFlags.UpdateText);
		}
	}

	public bool inlineStyling
	{
		get
		{
			UpgradeData();
			return data.inlineStyling;
		}
		set
		{
			UpgradeData();
			data.inlineStyling = value;
			SetNeedUpdate(UpdateFlags.UpdateText);
		}
	}

	public bool allCaps
	{
		get
		{
			UpgradeData();
			return data.allCaps;
		}
		set
		{
			UpgradeData();
			data.allCaps = value;
			SetNeedUpdate(UpdateFlags.UpdateText);
		}
	}

	public float Spacing
	{
		get
		{
			UpgradeData();
			return data.spacing;
		}
		set
		{
			UpgradeData();
			if (data.spacing != value)
			{
				data.spacing = value;
				SetNeedUpdate(UpdateFlags.UpdateText);
			}
		}
	}

	public float LineSpacing
	{
		get
		{
			UpgradeData();
			return data.lineSpacing;
		}
		set
		{
			UpgradeData();
			if (data.lineSpacing != value)
			{
				data.lineSpacing = value;
				SetNeedUpdate(UpdateFlags.UpdateText);
			}
		}
	}

	public int SortingOrder
	{
		get
		{
			return CachedRenderer.sortingOrder;
		}
		set
		{
			if (CachedRenderer.sortingOrder != value)
			{
				data.renderLayer = value;
				CachedRenderer.sortingOrder = value;
			}
		}
	}

	private Renderer CachedRenderer
	{
		get
		{
			if (_cachedRenderer == null)
			{
				_cachedRenderer = GetComponent<Renderer>();
			}
			return _cachedRenderer;
		}
	}

	private bool useInlineStyling => inlineStyling && _fontInst.textureGradients;

	public void SetForceWesternLineBreaks()
	{
		m_forceWesternLineBreaks = true;
	}

	public override void SetText(string textIn)
	{
		if (text != textIn)
		{
			if (textIn == null)
			{
				DebugLogWrapper.LogError("ERR: Setting NULL text! That's a bad thing to do...");
				textIn = string.Empty;
			}
			text = textIn;
		}
	}

	public override void SetColor(Color c)
	{
		color = c;
	}

	private void UpgradeData()
	{
		if (data.version != 1)
		{
			data.font = _font;
			data.text = _text;
			data.color = _color;
			data.color2 = _color2;
			data.useGradient = _useGradient;
			data.textureGradient = _textureGradient;
			data.anchor = _anchor;
			data.scale = _scale;
			data.kerning = _kerning;
			data.maxChars = _maxChars;
			data.inlineStyling = _inlineStyling;
			data.allCaps = _allCaps;
			data.formatting = _formatting;
			data.wordWrapWidth = _wordWrapWidth;
			data.spacing = spacing;
			data.lineSpacing = lineSpacing;
		}
		data.version = 1;
	}

	private static int GetInlineStyleCommandLength(int cmdSymbol)
	{
		int result = 0;
		switch (cmdSymbol)
		{
		case 99:
			result = 5;
			break;
		case 67:
			result = 9;
			break;
		case 103:
			result = 9;
			break;
		case 71:
			result = 17;
			break;
		}
		return result;
	}

	public string FormatText(string unformattedString)
	{
		string _targetString = string.Empty;
		data.scaleModificationFit = Vector3.one;
		while (true)
		{
			Vector2 vector = FormatText(ref _targetString, unformattedString);
			float num = GetTotalScale().y * vector.y * (data.font.lineHeight + data.lineSpacing);
			if (data.areaHeight != 0 && num > (float)data.areaHeight)
			{
				data.scaleModificationFit *= 0.9f;
				continue;
			}
			if (data.wordWrapWidth != 0 && vector.x > (float)data.wordWrapWidth)
			{
				data.scaleModificationFit *= 0.9f;
				continue;
			}
			break;
		}
		return _targetString;
	}

	private void FormatText()
	{
		data.scaleModificationFit = Vector3.one;
		while (true)
		{
			Vector2 vector = FormatText(ref _formattedText, data.text);
			float num = GetTotalScale().y * vector.y * (data.font.lineHeight + data.lineSpacing);
			if (data.areaHeight != 0 && num > (float)data.areaHeight)
			{
				data.scaleModificationFit *= 0.9f;
				continue;
			}
			if (data.wordWrapWidth != 0 && vector.x > (float)data.wordWrapWidth)
			{
				data.scaleModificationFit *= 0.9f;
				continue;
			}
			break;
		}
	}

	private Vector2 FormatText(ref string _targetString, string _source)
	{
		InitInstance();
		if (!formatting || wordWrapWidth == 0 || _fontInst.texelSize == Vector2.zero)
		{
			_targetString = _source;
			return new Vector2(0f, 0f);
		}
		float num = _fontInst.texelSize.x * (float)wordWrapWidth;
		StringBuilder stringBuilder = new StringBuilder(_source.Length);
		float num2 = 0f;
		float num3 = 0f;
		int num4 = 1;
		float num5 = 0f;
		int num6 = 0;
		int num7 = -1;
		int num8 = -1;
		bool flag = false;
		for (int i = 0; i < _source.Length; i++)
		{
			char c = _source[i];
			bool flag2 = c == '^';
			tk2dFontChar tk2dFontChar2;
			if (_fontInst.useDictionary)
			{
				if (!_fontInst.charDict.ContainsKey(c))
				{
					c = '\0';
				}
				tk2dFontChar2 = _fontInst.charDict[c];
			}
			else
			{
				if (c >= _fontInst.chars.Length)
				{
					c = '\0';
				}
				tk2dFontChar2 = _fontInst.chars[(uint)c];
			}
			if (flag2)
			{
				c = '^';
			}
			if (flag)
			{
				flag = false;
				continue;
			}
			if (data.inlineStyling && c == '^' && i + 1 < _source.Length)
			{
				if (_source[i + 1] != '^')
				{
					int inlineStyleCommandLength = GetInlineStyleCommandLength(_source[i + 1]);
					int num9 = 1 + inlineStyleCommandLength;
					for (int j = 0; j < num9; j++)
					{
						if (i + j < _source.Length)
						{
							stringBuilder.Append(_source[i + j]);
						}
					}
					i += num9 - 1;
					continue;
				}
				flag = true;
				stringBuilder.Append('^');
			}
			switch (c)
			{
			case '\n':
				num2 = 0f;
				num5 = 0f;
				num6 = 0;
				num7 = stringBuilder.Length;
				num8 = i;
				num4++;
				break;
			case ' ':
				num2 += (tk2dFontChar2.advance + data.spacing) * GetTotalScale().x;
				num6++;
				num5 = num2;
				num7 = stringBuilder.Length;
				num8 = i;
				break;
			default:
			{
				int num10 = _source.IndexOf("\n", i);
				if (num10 == -1)
				{
					num10 = _source.Length;
				}
				if (num2 + (tk2dFontChar2.advance + data.spacing) * GetTotalScale().x > num)
				{
					if (Singleton<LanguageLoader>.InstanceDontFetch != null && Singleton<LanguageLoader>.Instance.UseNonWesternLineBreaking() && !m_forceWesternLineBreaks)
					{
						int num11 = _source.IndexOf("。");
						if (num11 != -1)
						{
							num10 = Mathf.Min(num11, num10);
						}
						else
						{
							num11 = int.MaxValue;
						}
						int num12 = _source.IndexOf("!");
						if (num12 != -1)
						{
							num10 = Mathf.Min(num12, num10);
						}
						else
						{
							num12 = int.MaxValue;
						}
						bool flag3 = true;
						if (NonWesternLineBreakInfo.GetDisallowedCharactersStart().Contains(c))
						{
							flag3 = false;
						}
						if (flag3 && stringBuilder.Length > 0)
						{
							char c2 = stringBuilder[stringBuilder.Length - 1];
							if (NonWesternLineBreakInfo.GetDisallowedCharactersEnd().Contains(c))
							{
								flag3 = false;
							}
						}
						if (num6 < 5 || (num10 > i && num10 < i + 3) || (num11 > i && num11 < i + 4) || (num12 > i && num12 < i + 4))
						{
							flag3 = false;
						}
						if (flag3)
						{
							num5 = 0f;
							num2 = 0f;
							num6 = 0;
							num7 = stringBuilder.Length;
							num8 = i;
							stringBuilder.Append('\n');
							num4++;
						}
						else
						{
							num2 += (tk2dFontChar2.advance + data.spacing) * GetTotalScale().x;
							num3 = Mathf.Max(num2, num3);
						}
					}
					else
					{
						if (num5 > 0f)
						{
							num5 = 0f;
							num2 = 0f;
							num6 = 0;
							stringBuilder.Remove(num7 + 1, stringBuilder.Length - num7 - 1);
							stringBuilder.Append('\n');
							num4++;
							i = num8;
							continue;
						}
						if (num6 < 20 || num10 < i + 5)
						{
							num2 += (tk2dFontChar2.advance + data.spacing) * GetTotalScale().x;
							num3 = Mathf.Max(num2, num3);
							break;
						}
						num5 = 0f;
						num2 = 0f;
						num6 = 0;
						num7 = stringBuilder.Length;
						num8 = i;
						stringBuilder.Append('\n');
						num4++;
					}
				}
				else
				{
					num2 += (tk2dFontChar2.advance + data.spacing) * GetTotalScale().x;
					num6++;
					num3 = Mathf.Max(num2, num3);
				}
				break;
			}
			}
			stringBuilder.Append((c != '\u00a0') ? c : ' ');
		}
		_targetString = stringBuilder.ToString();
		return new Vector2(num3, num4);
	}

	public Vector2 GetTotalScale()
	{
		return new Vector2(data.scale.x * data.scaleModificationFit.x * data.scaleModificationRep.x, data.scale.y * data.scaleModificationFit.y * data.scaleModificationRep.y);
	}

	public void SetRescale(Vector3 rescale)
	{
		data.scaleModificationRep = rescale;
	}

	private void SetNeedUpdate(UpdateFlags uf)
	{
		if (updateFlags == UpdateFlags.UpdateNone)
		{
			updateFlags |= uf;
			tk2dUpdateManager.QueueCommit(this);
		}
		else
		{
			updateFlags |= uf;
		}
	}

	private void InitInstance()
	{
		if (data != null && data.font != null)
		{
			_fontInst = data.font.inst;
			_fontInst.InitDictionary();
		}
	}

	public void UpdateFontType()
	{
		if (data.font.m_relatedFontGroup != null)
		{
			data.font = data.font.m_relatedFontGroup.GetReplacementData(m_overrideLangFlag);
		}
		if (data.font != null)
		{
			_fontInst = data.font.inst;
		}
		if (data.font != null)
		{
			Init();
			UpdateMaterial();
		}
		updateFlags = UpdateFlags.UpdateNone;
		ForceBuild();
	}

	private void Awake()
	{
		UpgradeData();
		if (data.font.m_relatedFontGroup != null)
		{
			data.font = data.font.m_relatedFontGroup.GetReplacementData(m_overrideLangFlag);
		}
		if (data.font != null)
		{
			_fontInst = data.font.inst;
		}
		updateFlags = UpdateFlags.UpdateBuffers;
		if (data.font != null)
		{
			Init();
			UpdateMaterial();
		}
		updateFlags = UpdateFlags.UpdateNone;
		FontGroup.OnFontGroupLikelyChanged += UpdateFontType;
	}

	public void SetOverrideLangFlag(int langFlag)
	{
		m_overrideLangFlag = langFlag;
	}

	protected void OnDestroy()
	{
		FontGroup.OnFontGroupLikelyChanged -= UpdateFontType;
		if (meshFilter == null)
		{
			meshFilter = GetComponent<MeshFilter>();
		}
		if (meshFilter != null)
		{
			mesh = meshFilter.sharedMesh;
		}
		if ((bool)mesh)
		{
			UnityEngine.Object.DestroyImmediate(mesh, allowDestroyingAssets: true);
			meshFilter.mesh = null;
		}
	}

	public int NumDrawnCharacters()
	{
		int num = NumTotalCharacters();
		if (num > data.maxChars)
		{
			num = data.maxChars;
		}
		return num;
	}

	public int NumTotalCharacters()
	{
		InitInstance();
		if ((updateFlags & (UpdateFlags.UpdateText | UpdateFlags.UpdateBuffers)) != 0)
		{
			FormatText();
		}
		int num = 0;
		for (int i = 0; i < _formattedText.Length; i++)
		{
			int num2 = _formattedText[i];
			bool flag = num2 == 94;
			if (_fontInst.useDictionary)
			{
				if (!_fontInst.charDict.ContainsKey(num2))
				{
					num2 = 0;
				}
			}
			else if (num2 >= _fontInst.chars.Length)
			{
				num2 = 0;
			}
			if (flag)
			{
				num2 = 94;
			}
			if (num2 == 10)
			{
				continue;
			}
			if (data.inlineStyling && num2 == 94 && i + 1 < _formattedText.Length)
			{
				if (_formattedText[i + 1] != '^')
				{
					i += GetInlineStyleCommandLength(_formattedText[i + 1]);
					continue;
				}
				i++;
			}
			num++;
		}
		return num;
	}

	[Obsolete("Use GetEstimatedMeshBoundsForString().size instead")]
	public Vector2 GetMeshDimensionsForString(string str)
	{
		return tk2dTextGeomGen.GetMeshDimensionsForString(str, tk2dTextGeomGen.Data(data, _fontInst, _formattedText));
	}

	public Bounds GetEstimatedMeshBoundsForString(string str)
	{
		InitInstance();
		tk2dTextGeomGen.GeomData geomData = tk2dTextGeomGen.Data(data, _fontInst, _formattedText);
		Vector2 meshDimensionsForString = tk2dTextGeomGen.GetMeshDimensionsForString(FormatText(str), geomData);
		float yAnchorForHeight = tk2dTextGeomGen.GetYAnchorForHeight(meshDimensionsForString.y, geomData);
		float xAnchorForWidth = tk2dTextGeomGen.GetXAnchorForWidth(meshDimensionsForString.x, geomData);
		float num = (_fontInst.lineHeight + data.lineSpacing) * GetTotalScale().y;
		return new Bounds(new Vector3(xAnchorForWidth + meshDimensionsForString.x * 0.5f, yAnchorForHeight + meshDimensionsForString.y * 0.5f + num, 0f), Vector3.Scale(meshDimensionsForString, new Vector3(1f, -1f, 1f)));
	}

	public void Init(bool force)
	{
		if (force)
		{
			SetNeedUpdate(UpdateFlags.UpdateBuffers);
		}
		Init();
	}

	public void Init()
	{
		if (!_fontInst || ((updateFlags & UpdateFlags.UpdateBuffers) == 0 && !(mesh == null)))
		{
			return;
		}
		_fontInst.InitDictionary();
		FormatText();
		tk2dTextGeomGen.GeomData geomData = tk2dTextGeomGen.Data(data, _fontInst, _formattedText);
		tk2dTextGeomGen.GetTextMeshGeomDesc(out var numVertices, out var numIndices, geomData);
		vertices = new Vector3[numVertices];
		uvs = new Vector2[numVertices];
		colors = new Color32[numVertices];
		untintedColors = new Color32[numVertices];
		if (_fontInst.textureGradients)
		{
			uv2 = new Vector2[numVertices];
		}
		int[] array = new int[numIndices];
		int target = tk2dTextGeomGen.SetTextMeshGeom(vertices, uvs, uv2, untintedColors, 0, geomData);
		if (!_fontInst.isPacked)
		{
			Color32 color = data.color;
			Color32 color2 = ((!data.useGradient) ? data.color : data.color2);
			for (int i = 0; i < numVertices; i++)
			{
				Color32 color3 = ((i % 4 >= 2) ? color2 : color);
				byte b = (byte)(untintedColors[i].r * color3.r / 255);
				byte b2 = (byte)(untintedColors[i].g * color3.g / 255);
				byte b3 = (byte)(untintedColors[i].b * color3.b / 255);
				byte b4 = (byte)(untintedColors[i].a * color3.a / 255);
				if (_fontInst.premultipliedAlpha)
				{
					b = (byte)(b * b4 / 255);
					b2 = (byte)(b2 * b4 / 255);
					b3 = (byte)(b3 * b4 / 255);
				}
				ref Color32 reference = ref colors[i];
				reference = new Color32(b, b2, b3, b4);
			}
		}
		else
		{
			colors = untintedColors;
		}
		tk2dTextGeomGen.SetTextMeshIndices(array, 0, 0, geomData, target);
		if (mesh == null)
		{
			if (meshFilter == null)
			{
				meshFilter = GetComponent<MeshFilter>();
			}
			mesh = new Mesh();
			mesh.MarkDynamic();
			mesh.hideFlags = HideFlags.DontSave;
			meshFilter.mesh = mesh;
		}
		else
		{
			mesh.Clear();
		}
		mesh.vertices = vertices;
		mesh.uv = uvs;
		if (font.textureGradients)
		{
			mesh.uv2 = uv2;
		}
		mesh.triangles = array;
		mesh.colors32 = colors;
		mesh.RecalculateBounds();
		mesh.bounds = tk2dBaseSprite.AdjustedMeshBounds(mesh.bounds, data.renderLayer);
		updateFlags = UpdateFlags.UpdateNone;
	}

	public void Commit()
	{
		tk2dUpdateManager.FlushQueues();
	}

	public void DoNotUse__CommitInternal()
	{
		InitInstance();
		if (_fontInst == null)
		{
			return;
		}
		_fontInst.InitDictionary();
		bool flag = false;
		if ((updateFlags & UpdateFlags.UpdateBuffers) != 0 || mesh == null)
		{
			Init();
			flag = true;
		}
		else
		{
			if ((updateFlags & UpdateFlags.UpdateText) != 0)
			{
				FormatText();
				tk2dTextGeomGen.GeomData geomData = tk2dTextGeomGen.Data(data, _fontInst, _formattedText);
				int num = tk2dTextGeomGen.SetTextMeshGeom(vertices, uvs, uv2, untintedColors, 0, geomData);
				for (int i = num; i < data.maxChars; i++)
				{
					ref Vector3 reference = ref vertices[i * 4];
					ref Vector3 reference2 = ref vertices[i * 4 + 1];
					ref Vector3 reference3 = ref vertices[i * 4 + 2];
					ref Vector3 reference4 = ref vertices[i * 4 + 3];
					reference = (reference2 = (reference3 = (reference4 = Vector3.zero)));
				}
				mesh.vertices = vertices;
				mesh.uv = uvs;
				if (_fontInst.textureGradients)
				{
					mesh.uv2 = uv2;
				}
				if (_fontInst.isPacked)
				{
					colors = untintedColors;
					mesh.colors32 = colors;
				}
				if (data.inlineStyling)
				{
					SetNeedUpdate(UpdateFlags.UpdateColors);
				}
				mesh.RecalculateBounds();
				mesh.bounds = tk2dBaseSprite.AdjustedMeshBounds(mesh.bounds, data.renderLayer);
				flag = true;
			}
			if (!font.isPacked && (updateFlags & UpdateFlags.UpdateColors) != 0)
			{
				Color32 color = data.color;
				Color32 color2 = ((!data.useGradient) ? data.color : data.color2);
				for (int j = 0; j < colors.Length; j++)
				{
					Color32 color3 = ((j % 4 >= 2) ? color2 : color);
					byte b = (byte)(untintedColors[j].r * color3.r / 255);
					byte b2 = (byte)(untintedColors[j].g * color3.g / 255);
					byte b3 = (byte)(untintedColors[j].b * color3.b / 255);
					byte b4 = (byte)(untintedColors[j].a * color3.a / 255);
					if (_fontInst.premultipliedAlpha)
					{
						b = (byte)(b * b4 / 255);
						b2 = (byte)(b2 * b4 / 255);
						b3 = (byte)(b3 * b4 / 255);
					}
					ref Color32 reference5 = ref colors[j];
					reference5 = new Color32(b, b2, b3, b4);
				}
				mesh.colors32 = colors;
			}
		}
		updateFlags = UpdateFlags.UpdateNone;
		if (flag && OnRefresh != null)
		{
			OnRefresh();
		}
	}

	public void MakePixelPerfect()
	{
		float num = 1f;
		tk2dCamera tk2dCamera2 = tk2dCamera.CameraForLayer(base.gameObject.layer);
		if (tk2dCamera2 != null)
		{
			if (_fontInst.version < 1)
			{
				DebugLogWrapper.LogError("Need to rebuild font.");
			}
			float distance = base.transform.position.z - tk2dCamera2.transform.position.z;
			float num2 = _fontInst.invOrthoSize * _fontInst.halfTargetHeight;
			num = tk2dCamera2.GetSizeAtDistance(distance) * num2;
		}
		else if ((bool)Camera.main)
		{
			if (Camera.main.orthographic)
			{
				num = Camera.main.orthographicSize;
			}
			else
			{
				float zdist = base.transform.position.z - Camera.main.transform.position.z;
				num = tk2dPixelPerfectHelper.CalculateScaleForPerspectiveCamera(Camera.main.fieldOfView, zdist);
			}
			num *= _fontInst.invOrthoSize;
		}
		scale = new Vector3(Mathf.Sign(scale.x) * num, Mathf.Sign(scale.y) * num, Mathf.Sign(scale.z) * num);
	}

	public bool UsesSpriteCollection(tk2dSpriteCollectionData spriteCollection)
	{
		if (data.font != null && data.font.spriteCollection != null)
		{
			return data.font.spriteCollection == spriteCollection;
		}
		return true;
	}

	private void UpdateMaterial()
	{
		if (GetComponent<Renderer>().sharedMaterial != _fontInst.materialInst)
		{
			GetComponent<Renderer>().material = _fontInst.materialInst;
		}
	}

	public void ForceBuild()
	{
		if (data.font != null)
		{
			_fontInst = data.font.inst;
			UpdateMaterial();
		}
		Init(force: true);
	}
}
