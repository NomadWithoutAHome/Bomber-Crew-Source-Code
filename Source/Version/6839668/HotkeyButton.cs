using UnityEngine;

public class HotkeyButton : MonoBehaviour
{
	[SerializeField]
	private KeyCode m_keycode;

	[SerializeField]
	private bool m_andShift;

	private tk2dUIItem m_uiItem;

	private void Awake()
	{
		m_uiItem = GetComponent<tk2dUIItem>();
	}

	private void Update()
	{
		if (Input.GetKeyDown(m_keycode) && (!m_andShift || Input.GetKey(KeyCode.LeftShift)) && m_uiItem != null && m_uiItem.enabled && m_uiItem.gameObject.activeInHierarchy && m_uiItem.GetComponent<Collider>() != null && m_uiItem.GetComponent<Collider>().enabled)
		{
			m_uiItem.SimulateClick();
		}
	}
}
