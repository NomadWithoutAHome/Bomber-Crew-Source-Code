using UnityEngine;

public class ButtonStateDisplay : MonoBehaviour
{
	[SerializeField]
	private tk2dUIItem m_uiItem;

	[SerializeField]
	private Transform m_offsetTransform;

	[SerializeField]
	private Vector3 m_downOffset = Vector3.zero;

	[SerializeField]
	private Vector3 m_downScale = Vector3.one;

	[SerializeField]
	private GameObject m_buttonDefault;

	[SerializeField]
	private GameObject m_buttonDown;

	[SerializeField]
	private GameObject m_buttonHover;

	private bool m_lockDown;

	private void Start()
	{
		if (m_buttonHover != null)
		{
			m_uiItem.isHoverEnabled = true;
		}
	}

	private void Awake()
	{
		if (m_offsetTransform != null)
		{
			m_offsetTransform.localPosition = Vector3.zero;
			m_offsetTransform.localScale = Vector3.one;
		}
		if (m_buttonDefault != null && m_buttonDown != null)
		{
			m_buttonDefault.SetActive(value: true);
			m_buttonDown.SetActive(value: false);
		}
		if (m_buttonHover != null)
		{
			m_buttonHover.SetActive(value: false);
		}
		m_uiItem.OnDown += OnDown;
		m_uiItem.OnRelease += OnRelease;
		m_uiItem.OnHoverOver += OnHoverOver;
		m_uiItem.OnHoverOut += OnHoverOut;
	}

	private void OnDown()
	{
		if (m_offsetTransform != null)
		{
			m_offsetTransform.localPosition = m_downOffset;
			m_offsetTransform.localScale = m_downScale;
		}
		if (m_buttonDefault != null && m_buttonDown != null)
		{
			m_buttonDefault.SetActive(value: false);
			m_buttonDown.SetActive(value: true);
		}
		if (m_buttonHover != null)
		{
			m_buttonHover.SetActive(value: false);
		}
	}

	private void OnRelease()
	{
		if (!m_lockDown)
		{
			if (m_offsetTransform != null)
			{
				m_offsetTransform.localPosition = Vector3.zero;
				m_offsetTransform.localScale = Vector3.one;
			}
			if (m_buttonDefault != null && m_buttonDown != null)
			{
				m_buttonDefault.SetActive(value: true);
				m_buttonDown.SetActive(value: false);
			}
		}
	}

	private void OnHoverOver()
	{
		if (m_buttonHover != null)
		{
			m_buttonHover.SetActive(value: true);
		}
	}

	private void OnHoverOut()
	{
		if (m_buttonHover != null)
		{
			m_buttonHover.SetActive(value: false);
		}
	}

	public void SetLockDown(bool lockDown)
	{
		m_lockDown = lockDown;
		if (lockDown)
		{
			OnDown();
		}
		else
		{
			OnRelease();
		}
	}
}
