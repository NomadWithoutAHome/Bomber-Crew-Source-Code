using BomberCrewCommon;
using UnityEngine;

public class UISelectorPointingHint : MonoBehaviour
{
	[SerializeField]
	private Transform m_pointAtNode;

	[SerializeField]
	private bool m_noMagnet;

	[SerializeField]
	private bool m_defaultSelection;

	[SerializeField]
	private string m_findTagId;

	[SerializeField]
	private bool m_notSelectable;

	[SerializeField]
	private tk2dUIItem m_upLink;

	[SerializeField]
	private tk2dUIItem m_downLink;

	[SerializeField]
	private tk2dUIItem m_leftLink;

	[SerializeField]
	private tk2dUIItem m_rightLink;

	private void OnEnable()
	{
		if (!string.IsNullOrEmpty(m_findTagId))
		{
			Singleton<UISelector>.Instance.RegisterHint(this, m_findTagId);
		}
	}

	public tk2dUIItem GetUpLink()
	{
		return m_upLink;
	}

	public tk2dUIItem GetDownLink()
	{
		return m_downLink;
	}

	public tk2dUIItem GetLeftLink()
	{
		return m_leftLink;
	}

	public tk2dUIItem GetRightLink()
	{
		return m_rightLink;
	}

	public void SetLeftLink(tk2dUIItem link)
	{
		m_leftLink = link;
	}

	public void SetRightLink(tk2dUIItem link)
	{
		m_rightLink = link;
	}

	public void SetUpLink(tk2dUIItem link)
	{
		m_upLink = link;
	}

	public void SetDownLink(tk2dUIItem link)
	{
		m_downLink = link;
	}

	public bool IsSelectBlocked()
	{
		return m_notSelectable;
	}

	public void OnDisable()
	{
		if (!string.IsNullOrEmpty(m_findTagId) && Singleton<UISelector>.Instance != null)
		{
			Singleton<UISelector>.Instance.DeregisterHint(this, m_findTagId);
		}
	}

	public void SetDefault()
	{
		m_defaultSelection = true;
	}

	public Transform GetPointingHint()
	{
		return m_pointAtNode;
	}

	public bool Magnet()
	{
		return !m_noMagnet;
	}

	public bool IsDefault()
	{
		return m_defaultSelection;
	}
}
