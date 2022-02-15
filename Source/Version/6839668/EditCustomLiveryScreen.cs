using System;
using System.Collections.Generic;
using BomberCrewCommon;
using UnityEngine;

public class EditCustomLiveryScreen : MonoBehaviour
{
	[Serializable]
	public class BrushType
	{
		[SerializeField]
		public int m_size;

		[SerializeField]
		public bool m_round;

		[SerializeField]
		public bool m_fill;

		[SerializeField]
		public bool m_stamp;

		[SerializeField]
		public Texture2D m_stampTexture;

		[SerializeField]
		public string m_spriteIconName;

		[SerializeField]
		public string m_spritePreviewName;
	}

	[SerializeField]
	private tk2dUIItem m_canvas;

	[SerializeField]
	private SelectableFilterButton m_saveButton;

	[SerializeField]
	private SelectableFilterButton m_cancelButton;

	[SerializeField]
	private tk2dUIItem m_undoButton;

	[SerializeField]
	private BrushType[] m_brushTypes;

	[SerializeField]
	private GameObject m_colorSelectPrefab;

	[SerializeField]
	private GameObject m_brushTypePrefab;

	[SerializeField]
	private LayoutGrid m_colorSelectLayout;

	[SerializeField]
	private LayoutGrid m_brushSelectLayout;

	[SerializeField]
	private Renderer m_imageRenderer;

	[SerializeField]
	private float m_minStep = 3f;

	[SerializeField]
	private GameObject m_undoHierarchy;

	[SerializeField]
	private AirbaseAreaScreen m_hangarArea;

	[SerializeField]
	private Texture2D m_customLiveryPaletteTexture;

	[SerializeField]
	private int m_colorPalX;

	[SerializeField]
	private int m_colorPalY;

	[SerializeField]
	private int m_skipColorX;

	[SerializeField]
	private int m_skipColorY;

	[SerializeField]
	private tk2dBaseSprite m_brushpreviewSprite;

	[SerializeField]
	private float m_brushPreviewAlpha = 0.25f;

	[SerializeField]
	private UISelectFinder m_menuFinder;

	[SerializeField]
	private UISelectFinder m_canvasFinder;

	[SerializeField]
	private GameObject m_confirmPopup;

	[SerializeField]
	private int m_sizeX = 64;

	[SerializeField]
	private int m_sizeY = 64;

	[SerializeField]
	private float m_worldPixToScreenPix = 10f;

	private List<SelectableFilterButton> m_createdColorButtons = new List<SelectableFilterButton>();

	private List<SelectableFilterButton> m_createdBrushButton = new List<SelectableFilterButton>();

	private Color[] m_cols = new Color[4096];

	private Color[] m_colsUndo = new Color[4096];

	private Texture2D m_texture;

	private BrushType m_currentBrushType;

	private Color m_currentColor = Color.red;

	private bool m_isDown;

	private Vector3 m_lastDownPos;

	private string m_saveRef;

	private int m_saveSlot;

	private LiveryEquippable m_liveryEquippable;

	private bool m_initialised;

	private bool m_isOnCanvas;

	private void Start()
	{
		Initialise();
	}

	private void OnEnable()
	{
		Singleton<MainActionButtonMonitor>.Instance.AddListener(MainButtonListener);
		Singleton<UISelector>.Instance.SetFinder(m_menuFinder);
		m_undoHierarchy.SetActive(value: false);
	}

	private void OnDisable()
	{
		Singleton<MainActionButtonMonitor>.Instance.RemoveListener(MainButtonListener, invalidateCurrentPress: false);
	}

	private bool MainButtonListener(MainActionButtonMonitor.ButtonPress bp)
	{
		switch (bp)
		{
		case MainActionButtonMonitor.ButtonPress.LeftAction:
			m_isOnCanvas = !m_isOnCanvas;
			if (m_isOnCanvas)
			{
				Singleton<UISelector>.Instance.SetFinder(m_canvasFinder);
			}
			else
			{
				Singleton<UISelector>.Instance.SetFinder(m_menuFinder);
			}
			return true;
		case MainActionButtonMonitor.ButtonPress.Back:
			CancelReturn();
			return true;
		default:
			return false;
		}
	}

	private void Initialise()
	{
		if (m_initialised)
		{
			return;
		}
		m_cols = new Color[m_sizeX * m_sizeY];
		m_colsUndo = new Color[m_sizeX * m_sizeY];
		m_initialised = true;
		m_undoHierarchy.SetActive(value: false);
		m_canvas.OnDown += OnDownCanvas;
		m_canvas.OnRelease += OnUpCanvas;
		m_imageRenderer.sharedMaterial = UnityEngine.Object.Instantiate(m_imageRenderer.sharedMaterial);
		Color[] array = new Color[m_colorPalY * m_colorPalX];
		float num = 1f / (float)m_colorPalX;
		float num2 = 1f / (float)m_colorPalY;
		float num3 = 1f - num2 / 2f;
		int num4 = 0;
		for (int i = 0; i < m_colorPalY; i++)
		{
			float num5 = num / 2f;
			for (int j = 0; j < m_colorPalX; j++)
			{
				if (j != m_skipColorX || i != m_skipColorY)
				{
					ref Color reference = ref array[num4];
					reference = m_customLiveryPaletteTexture.GetPixelBilinear(num5, num3);
				}
				num5 += num;
				num4++;
			}
			num3 -= num2;
		}
		int num6 = 0;
		Color[] array2 = array;
		foreach (Color color in array2)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(m_colorSelectPrefab);
			gameObject.name = "COL_" + num6;
			gameObject.transform.parent = m_colorSelectLayout.transform;
			gameObject.GetComponent<CustomLiveryColorSelect>().SetUp(color);
			Color cl = color;
			SelectableFilterButton sfbThis = gameObject.GetComponent<SelectableFilterButton>();
			sfbThis.OnClick += delegate
			{
				m_currentColor = cl;
				m_brushpreviewSprite.color = new Color(cl.r, cl.g, cl.b, m_brushPreviewAlpha);
				foreach (SelectableFilterButton createdColorButton in m_createdColorButtons)
				{
					createdColorButton.SetSelected(sfbThis == createdColorButton);
				}
			};
			m_createdColorButtons.Add(sfbThis);
			num6++;
		}
		m_colorSelectLayout.RepositionChildren();
		m_currentColor = array[1];
		m_brushpreviewSprite.color = new Color(m_currentColor.r, m_currentColor.g, m_currentColor.b, m_brushPreviewAlpha);
		m_createdColorButtons[1].SetSelected(selected: true);
		BrushType[] brushTypes = m_brushTypes;
		foreach (BrushType brushType in brushTypes)
		{
			GameObject gameObject2 = UnityEngine.Object.Instantiate(m_brushTypePrefab);
			gameObject2.transform.parent = m_brushSelectLayout.transform;
			gameObject2.GetComponent<CustomLiveryBrushSelect>().SetUp(brushType.m_spriteIconName);
			BrushType btl = brushType;
			SelectableFilterButton sfbThis2 = gameObject2.GetComponent<SelectableFilterButton>();
			sfbThis2.OnClick += delegate
			{
				m_currentBrushType = btl;
				m_brushpreviewSprite.SetSprite(btl.m_spritePreviewName);
				foreach (SelectableFilterButton item in m_createdBrushButton)
				{
					item.SetSelected(sfbThis2 == item);
				}
			};
			m_createdBrushButton.Add(sfbThis2);
		}
		m_currentBrushType = m_brushTypes[0];
		m_brushpreviewSprite.SetSprite(m_brushTypes[0].m_spritePreviewName);
		m_createdBrushButton[0].SetSelected(selected: true);
		m_brushSelectLayout.RepositionChildren();
		m_undoButton.OnClick += Undo;
		m_saveButton.OnClick += SaveReturn;
		m_cancelButton.OnClick += CancelReturn;
	}

	private void SaveReturn()
	{
		m_hangarArea.GetComponent<BomberUpgradeScreenController>().SetReturnFromLivery(m_liveryEquippable);
		Singleton<CustomLiveryTextures>.Instance.SetCustomLivery(m_texture, m_saveRef, m_saveSlot);
		Singleton<BomberContainer>.Instance.GetLivery().Refresh();
		Singleton<AirbaseNavigation>.Instance.SetCrewPhotoRequiresRefresh();
		Singleton<AirbaseNavigation>.Instance.SetSelectingArea(m_hangarArea);
	}

	private void CancelReturn()
	{
		UIPopupData uIPopupData = new UIPopupData();
		uIPopupData.PopupStartCallback = (Action<UIPopUp>)Delegate.Combine(uIPopupData.PopupStartCallback, (Action<UIPopUp>)delegate(UIPopUp uip)
		{
			uip.GetComponent<GenericYesNoPrompt>().SetUp(Singleton<LanguageProvider>.Instance.GetNamedTextImmediate("ui_livery_edit_cancel_message"), Singleton<LanguageProvider>.Instance.GetNamedTextImmediate("ui_livery_edit_cancel_save"), Singleton<LanguageProvider>.Instance.GetNamedTextImmediate("ui_livery_edit_cancel_discard"), danger: false);
			uip.GetComponent<GenericYesNoPrompt>().OnNo += delegate
			{
				m_hangarArea.GetComponent<BomberUpgradeScreenController>().SetReturnFromLivery(m_liveryEquippable);
				Singleton<AirbaseNavigation>.Instance.SetSelectingArea(m_hangarArea);
			};
			uip.GetComponent<GenericYesNoPrompt>().OnYes += delegate
			{
				m_hangarArea.GetComponent<BomberUpgradeScreenController>().SetReturnFromLivery(m_liveryEquippable);
				Singleton<CustomLiveryTextures>.Instance.SetCustomLivery(m_texture, m_saveRef, m_saveSlot);
				Singleton<BomberContainer>.Instance.GetLivery().Refresh();
				Singleton<AirbaseNavigation>.Instance.SetCrewPhotoRequiresRefresh();
				Singleton<AirbaseNavigation>.Instance.SetSelectingArea(m_hangarArea);
			};
		});
		Singleton<UIPopupManager>.Instance.DisplayPopup(m_confirmPopup, uIPopupData);
	}

	public void SetSelectedIndex(string idS, int index, LiveryEquippable le)
	{
		Initialise();
		Texture2D customLivery = Singleton<CustomLiveryTextures>.Instance.GetCustomLivery(idS, index);
		Vector2 size = Singleton<CustomLiveryTextures>.Instance.GetSize(idS);
		m_liveryEquippable = le;
		m_sizeX = (int)size.x;
		m_sizeY = (int)size.y;
		m_cols = new Color[m_sizeX * m_sizeY];
		m_colsUndo = new Color[m_sizeX * m_sizeY];
		for (int i = 0; i < m_sizeX * m_sizeY; i++)
		{
			ref Color reference = ref m_cols[i];
			reference = new Color(0f, 0f, 0f, 0f);
			ref Color reference2 = ref m_colsUndo[i];
			reference2 = new Color(0f, 0f, 0f, 0f);
		}
		m_texture = new Texture2D(m_sizeX, m_sizeY, TextureFormat.RGBA32, mipmap: false);
		m_texture.wrapMode = TextureWrapMode.Clamp;
		m_texture.filterMode = FilterMode.Point;
		m_imageRenderer.sharedMaterial.mainTexture = m_texture;
		UpdateTexture();
		m_saveSlot = index;
		m_saveRef = idS;
		if (customLivery.width == m_sizeX && customLivery.height == m_sizeY)
		{
			m_cols = customLivery.GetPixels();
		}
		else
		{
			for (int j = 0; j < m_sizeX; j++)
			{
				for (int k = 0; k < m_sizeY; k++)
				{
					SetColor(j, k, customLivery.GetPixelBilinear((float)j / (float)(m_sizeX - 1), (float)k / (float)(m_sizeY - 1)));
				}
			}
		}
		UpdateTexture();
	}

	private void SetColor(int x, int y, Color c)
	{
		if (x >= 0 && x < m_sizeX && y >= 0 && y < m_sizeY)
		{
			m_cols[x + y * m_sizeX] = c;
		}
	}

	private void UpdateTexture()
	{
		m_texture.SetPixels(m_cols);
		m_texture.Apply(updateMipmaps: false, makeNoLongerReadable: false);
	}

	private void OnDownCanvas()
	{
		for (int i = 0; i < m_sizeX * m_sizeY; i++)
		{
			ref Color reference = ref m_colsUndo[i];
			reference = m_cols[i];
		}
		if (!m_currentBrushType.m_fill)
		{
			m_isDown = true;
		}
		m_lastDownPos = GetPosition();
	}

	private void Undo()
	{
		for (int i = 0; i < m_sizeX * m_sizeY; i++)
		{
			ref Color reference = ref m_cols[i];
			reference = m_colsUndo[i];
		}
		UpdateTexture();
		m_undoHierarchy.SetActive(value: false);
	}

	private int CoordToIndex(int x, int y)
	{
		if (x >= 0 && y >= 0 && x < m_sizeX && y < m_sizeY)
		{
			return x + y * m_sizeX;
		}
		return -1;
	}

	private void IndexToCoord(int index, out int x, out int y)
	{
		y = index / m_sizeX;
		x = index % m_sizeX;
	}

	private void FillFrom(int x, int y, Color toMatch, Color toFill)
	{
		if (toMatch == toFill)
		{
			return;
		}
		List<int> list = new List<int>();
		HashSet<int> hashSet = new HashSet<int>();
		list.Add(CoordToIndex(x, y));
		while (list.Count > 0)
		{
			int num = list[0];
			int x2 = 0;
			int y2 = 0;
			IndexToCoord(num, out x2, out y2);
			hashSet.Add(num);
			list.RemoveAt(0);
			if (m_cols[num] == toMatch)
			{
				m_cols[num] = toFill;
				int num2 = CoordToIndex(x2 + 1, y2);
				if (num2 != -1 && !hashSet.Contains(num2))
				{
					list.Add(num2);
				}
				int num3 = CoordToIndex(x2 - 1, y2);
				if (num3 != -1 && !hashSet.Contains(num3))
				{
					list.Add(num3);
				}
				int num4 = CoordToIndex(x2, y2 - 1);
				if (num4 != -1 && !hashSet.Contains(num4))
				{
					list.Add(num4);
				}
				int num5 = CoordToIndex(x2, y2 + 1);
				if (num5 != -1 && !hashSet.Contains(num5))
				{
					list.Add(num5);
				}
			}
		}
	}

	private void StartFill(int x, int y)
	{
		if (x >= 0 && x < m_sizeX && y >= 0 && y < m_sizeY)
		{
			Color toMatch = m_cols[x + y * m_sizeX];
			FillFrom(x, y, toMatch, m_currentColor);
		}
	}

	private void OnUpCanvas()
	{
		m_isDown = false;
		if (m_currentBrushType.m_fill)
		{
			Vector3 position = GetPosition();
			Vector2 pixelRef = GetPixelRef(position);
			StartFill((int)pixelRef.x, (int)pixelRef.y);
			UpdateTexture();
		}
		m_undoHierarchy.SetActive(value: true);
	}

	private Vector3 GetPosition()
	{
		Vector3 position = Singleton<UISelector>.Instance.GetScreenPosition();
		Camera uICameraForControl = tk2dUIManager.Instance.GetUICameraForControl(m_canvas.gameObject);
		return uICameraForControl.ScreenToWorldPoint(position);
	}

	public Vector2 GetPixelRef(Vector3 worldPos)
	{
		Vector3 vector = worldPos - m_canvas.transform.position;
		vector /= m_worldPixToScreenPix;
		int num = Mathf.Clamp(Mathf.FloorToInt(vector.x) + m_sizeX / 2, 0, m_sizeX - 1);
		int num2 = Mathf.Clamp(Mathf.FloorToInt(vector.y) + m_sizeY / 2, 0, m_sizeY - 1);
		return new Vector2(num, num2);
	}

	public Vector3 GetWorldPos(Vector2 pixelPos)
	{
		Vector3 vector = new Vector3(pixelPos.x - (float)(m_sizeX / 2), pixelPos.y - (float)(m_sizeY / 2));
		vector *= m_worldPixToScreenPix;
		return m_canvas.transform.position + vector;
	}

	private void Update()
	{
		if (m_isDown)
		{
			m_brushpreviewSprite.gameObject.SetActive(value: false);
			Vector3 position = GetPosition();
			int num = (int)((position - m_lastDownPos).magnitude / m_minStep) + 2;
			for (int i = 0; i < num; i++)
			{
				float t = (float)i / (float)(num - 1);
				Vector2 pixelRef = GetPixelRef(Vector3.Lerp(m_lastDownPos, position, t));
				int size = m_currentBrushType.m_size;
				for (int j = -size; j <= size; j++)
				{
					for (int k = -size; k <= size; k++)
					{
						if (m_currentBrushType.m_round)
						{
							if (size == 1)
							{
								if (Mathf.Abs(j) + Mathf.Abs(k) < 2)
								{
									SetColor((int)pixelRef.x + j, (int)pixelRef.y + k, m_currentColor);
								}
							}
							else if (Mathf.Round(Mathf.Sqrt(j * j + k * k)) <= (float)size)
							{
								SetColor((int)pixelRef.x + j, (int)pixelRef.y + k, m_currentColor);
							}
						}
						else if (m_currentBrushType.m_stamp)
						{
							float u = (float)j / (float)size * 0.5f + 0.5f;
							float v = (float)k / (float)size * 0.5f + 0.5f;
							float r = m_currentBrushType.m_stampTexture.GetPixelBilinear(u, v).r;
							if (r >= 1f)
							{
								SetColor((int)pixelRef.x + j, (int)pixelRef.y + k, m_currentColor);
							}
						}
						else
						{
							SetColor((int)pixelRef.x + j, (int)pixelRef.y + k, m_currentColor);
						}
					}
				}
			}
			UpdateTexture();
			m_lastDownPos = position;
			return;
		}
		Camera uICameraForControl = tk2dUIManager.Instance.GetUICameraForControl(m_canvas.gameObject);
		if (uICameraForControl != null)
		{
			Ray ray = uICameraForControl.ScreenPointToRay(Singleton<UISelector>.Instance.GetScreenPosition());
			Collider component = m_canvas.GetComponent<Collider>();
			if (component.Raycast(ray, out var hitInfo, 999f))
			{
				Vector3 point = hitInfo.point;
				Vector2 pixelPos = GetPixelRef(point) + new Vector2(0f, 1f);
				m_brushpreviewSprite.transform.position = GetWorldPos(pixelPos);
				m_brushpreviewSprite.gameObject.SetActive(value: true);
			}
			else
			{
				m_brushpreviewSprite.gameObject.SetActive(value: false);
			}
		}
	}
}
