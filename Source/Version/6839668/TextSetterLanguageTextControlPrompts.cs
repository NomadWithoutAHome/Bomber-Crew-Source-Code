using System;
using BomberCrewCommon;
using UnityEngine;

public class TextSetterLanguageTextControlPrompts : MonoBehaviour
{
	[SerializeField]
	[NamedText]
	private string m_key;

	[SerializeField]
	private TextSetter m_textMesh;

	[SerializeField]
	private LayoutGrid m_layoutGrid;

	[SerializeField]
	private bool m_refreshLayoutGrid;

	private void Awake()
	{
		if (m_textMesh == null)
		{
			m_textMesh = GetComponent<tk2dTextMesh>();
		}
		Singleton<LanguageProvider>.Instance.GetNamedText(m_key, GotText);
	}

	private void OnEnable()
	{
		if (m_refreshLayoutGrid)
		{
			tk2dTextMesh component = m_textMesh.gameObject.GetComponent<tk2dTextMesh>();
			component.OnRefresh = (Action)Delegate.Combine(component.OnRefresh, new Action(m_layoutGrid.RepositionChildren));
		}
	}

	private void OnDisable()
	{
		if (m_refreshLayoutGrid)
		{
			tk2dTextMesh component = m_textMesh.gameObject.GetComponent<tk2dTextMesh>();
			component.OnRefresh = (Action)Delegate.Remove(component.OnRefresh, new Action(m_layoutGrid.RepositionChildren));
		}
	}

	private void GotText(string result)
	{
		if (this != null && m_textMesh != null)
		{
			m_textMesh.SetText(ControlPromptDisplayHelpers.ConvertString(result));
		}
	}
}
