using System.Collections;
using UnityEngine;

namespace BomberCrewCommon;

public class DismissPopupAfterDelay : MonoBehaviour
{
	[SerializeField]
	private float m_secondsToShow;

	[SerializeField]
	private UIPopUp m_popup;

	public void DismissAfterDelay()
	{
		if (base.enabled && base.gameObject.activeInHierarchy)
		{
			StartCoroutine(DismissAfterDelayEnumerator());
		}
	}

	private void Start()
	{
		m_popup.OnPopupShown += delegate
		{
			DismissAfterDelay();
		};
	}

	private IEnumerator DismissAfterDelayEnumerator()
	{
		yield return new WaitForSeconds(m_secondsToShow);
		m_popup.DismissPopup();
	}
}
