using System;
using BomberCrewCommon;
using UnityEngine;

public class SelectableFilterButton : MonoBehaviour
{
	[SerializeField]
	private int m_selectLayerDepth;

	[SerializeField]
	private int m_selectLayerGetDepthBump;

	[SerializeField]
	private tk2dUIItem m_uiItem;

	[SerializeField]
	private tk2dBaseSprite m_backgroundSprite;

	[SerializeField]
	private tk2dTextMesh m_textMesh;

	[SerializeField]
	private tk2dBaseSprite m_iconSprite;

	[SerializeField]
	private LayoutGrid m_layoutGrid;

	[SerializeField]
	private ControlPromptDisplay m_controlPromptDisplay;

	[SerializeField]
	private Transform m_offset;

	[SerializeField]
	private UIButtonStateDefinitions.ButtonType m_buttonType;

	private bool m_selected;

	private bool m_filteredIn;

	private bool m_disabled;

	private object m_selectedObject;

	private Vector3 m_textDefaultposition;

	public event Action OnRefresh;

	public event Action OnClick;

	private void Awake()
	{
		Singleton<SelectableFilterTracker>.Instance.RegisterAsActive(this);
		SetSelected(selected: false);
		Singleton<UISelector>.Instance.OnFilterChange += OnFilterChange;
		RefreshGraphicsStates();
	}

	public object GetRelatedObject()
	{
		return m_selectedObject;
	}

	public void SetRelatedObject(object o)
	{
		m_selectedObject = o;
	}

	public int GetLayerDepth()
	{
		return m_selectLayerDepth + m_selectLayerGetDepthBump;
	}

	public void SetDisabled(bool disabled)
	{
		m_disabled = disabled;
		RefreshGraphicsStates();
	}

	private void Start()
	{
		RefreshFilter();
	}

	private void OnEnable()
	{
		if (m_uiItem != null)
		{
			m_uiItem.OnClick += OnButtonClick;
		}
		if (m_selected)
		{
			Singleton<SelectableFilterTracker>.Instance.RegisterAsSelected(this);
		}
		if (m_layoutGrid != null && m_textMesh != null)
		{
			m_layoutGrid.RepositionChildren();
			tk2dTextMesh textMesh = m_textMesh;
			textMesh.OnRefresh = (Action)Delegate.Combine(textMesh.OnRefresh, new Action(m_layoutGrid.RepositionChildren));
		}
	}

	public void FakeClick()
	{
		if (!(m_uiItem != null) || !m_uiItem.enabled || !m_uiItem.gameObject.activeInHierarchy || !(m_uiItem.GetComponent<Collider>() != null) || !m_uiItem.GetComponent<Collider>().enabled)
		{
			return;
		}
		tk2dCamera tk2dCamera2 = tk2dCamera.CameraForLayer(m_uiItem.gameObject.layer);
		if (tk2dCamera2 != null)
		{
			tk2dUICamera component = tk2dCamera2.GetComponent<tk2dUICamera>();
			if (component.GetComponent<tk2dUICamera>() != null && component.enabled)
			{
				m_uiItem.SimulateClick();
			}
		}
	}

	private void OnDestroy()
	{
		if (Singleton<UISelector>.Instance != null)
		{
			Singleton<UISelector>.Instance.OnFilterChange -= OnFilterChange;
		}
		if (Singleton<SelectableFilterTracker>.Instance != null)
		{
			if (m_selected)
			{
				Singleton<SelectableFilterTracker>.Instance.DeRegisterAsSelected(this);
			}
			Singleton<SelectableFilterTracker>.Instance.DeRegisterAsActive(this);
		}
	}

	private void OnDisable()
	{
		if (m_uiItem != null)
		{
			m_uiItem.OnClick -= OnButtonClick;
		}
		if (m_selected && Singleton<SelectableFilterTracker>.Instance != null)
		{
			Singleton<SelectableFilterTracker>.Instance.DeRegisterAsSelected(this);
		}
		if (m_layoutGrid != null && m_textMesh != null)
		{
			tk2dTextMesh textMesh = m_textMesh;
			textMesh.OnRefresh = (Action)Delegate.Remove(textMesh.OnRefresh, new Action(m_layoutGrid.RepositionChildren));
		}
	}

	private void OnButtonClick()
	{
		if (this.OnClick != null)
		{
			this.OnClick();
		}
	}

	private void OnFilterChange()
	{
		RefreshFilter();
	}

	private void RefreshFilter()
	{
		bool flag = Singleton<UISelector>.Instance.MatchesFilter(m_uiItem);
		if (m_filteredIn != flag)
		{
			m_filteredIn = flag;
			RefreshGraphicsStates();
		}
		if (this.OnRefresh != null)
		{
			this.OnRefresh();
		}
	}

	public void RefreshLayers()
	{
		RefreshGraphicsStates();
	}

	public bool IsSelected()
	{
		return m_selected;
	}

	public tk2dUIItem GetUIItem()
	{
		return m_uiItem;
	}

	public void SetSelected(bool selected)
	{
		if (m_selected != selected)
		{
			m_selected = selected;
			if (m_selected)
			{
				Singleton<SelectableFilterTracker>.Instance.RegisterAsSelected(this);
			}
			else
			{
				Singleton<SelectableFilterTracker>.Instance.DeRegisterAsSelected(this);
			}
			RefreshGraphicsStates();
		}
	}

	public void SetText(string text)
	{
		if (m_controlPromptDisplay != null)
		{
			m_controlPromptDisplay.Refresh();
		}
		m_textMesh.SetText(text);
	}

	private void RefreshGraphicsStates()
	{
		bool flag = false;
		if (Singleton<UISelector>.Instance != null && Singleton<UISelector>.Instance.IsPrimary())
		{
			flag = m_filteredIn;
		}
		else
		{
			int highestSelected = Singleton<SelectableFilterTracker>.Instance.GetHighestSelected();
			flag = highestSelected == m_selectLayerDepth - 1;
		}
		if (m_disabled)
		{
			SetUpGraphics(Singleton<UIButtonStateDefinitions>.Instance.GetState(m_buttonType, UIButtonStateDefinitions.ButtonState.Disabled));
		}
		else if (m_selected)
		{
			if (flag)
			{
				SetUpGraphics(Singleton<UIButtonStateDefinitions>.Instance.GetState(m_buttonType, UIButtonStateDefinitions.ButtonState.Selected));
			}
			else
			{
				SetUpGraphics(Singleton<UIButtonStateDefinitions>.Instance.GetState(m_buttonType, UIButtonStateDefinitions.ButtonState.FilteredOutSelected));
			}
		}
		else if (flag)
		{
			SetUpGraphics(Singleton<UIButtonStateDefinitions>.Instance.GetState(m_buttonType, UIButtonStateDefinitions.ButtonState.Default));
		}
		else
		{
			SetUpGraphics(Singleton<UIButtonStateDefinitions>.Instance.GetState(m_buttonType, UIButtonStateDefinitions.ButtonState.FilteredOut));
		}
	}

	private void SetUpGraphics(UIButtonStateDefinitions.ButtonDefinitionState buttonDefinitionState)
	{
		if (m_backgroundSprite != null)
		{
			m_backgroundSprite.SetSprite(buttonDefinitionState.m_backgroundSprite);
		}
		if (m_textMesh != null)
		{
			m_textMesh.color = buttonDefinitionState.m_textColour;
		}
		if (m_iconSprite != null)
		{
			m_iconSprite.color = buttonDefinitionState.m_iconColor;
		}
		if (m_offset != null)
		{
			m_offset.localPosition = buttonDefinitionState.m_offset;
		}
		m_uiItem.enabled = buttonDefinitionState.m_enableUiItem;
	}

	public void Refresh()
	{
		if (this.OnRefresh != null)
		{
			this.OnRefresh();
		}
	}
}
