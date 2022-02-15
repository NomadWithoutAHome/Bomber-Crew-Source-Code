using UnityEngine;

public class RepositionLayoutGridOnEnable : MonoBehaviour
{
	[SerializeField]
	private LayoutGrid m_layoutGrid;

	[SerializeField]
	private bool m_repositionOnEnable = true;

	[SerializeField]
	private bool m_repositionOnDisable = true;

	private void OnEnable()
	{
		if (m_layoutGrid != null)
		{
			m_layoutGrid.RepositionChildren();
		}
	}

	private void OnDisable()
	{
		if (m_layoutGrid != null)
		{
			m_layoutGrid.RepositionChildren();
		}
	}
}
