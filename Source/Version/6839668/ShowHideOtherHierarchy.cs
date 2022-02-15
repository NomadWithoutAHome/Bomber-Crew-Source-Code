using Common;
using UnityEngine;

public class ShowHideOtherHierarchy : MonoBehaviour
{
	[SerializeField]
	private GameObject m_otherHierarchy;

	[SerializeField]
	private bool m_enabled;

	[SerializeField]
	private bool m_useCustomActivate = true;

	private void OnEnable()
	{
		if (m_otherHierarchy.activeInHierarchy != m_enabled)
		{
			if (m_useCustomActivate)
			{
				m_otherHierarchy.CustomActivate(m_enabled);
			}
			else
			{
				m_otherHierarchy.SetActive(m_enabled);
			}
		}
	}
}
