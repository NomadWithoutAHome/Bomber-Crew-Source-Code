using System.Collections;
using Common;
using UnityEngine;
using WingroveAudio;

public class EndlessModeOverlayAreaExpanding : MonoBehaviour
{
	[SerializeField]
	private GameObject m_text01;

	[SerializeField]
	private GameObject m_background01;

	private void OnEnable()
	{
		StartCoroutine(DoSequence());
	}

	private IEnumerator DoSequence()
	{
		WingroveRoot.Instance.PostEvent("ENDLESS_OVERLAY_APPEAR");
		m_background01.CustomActivate(active: true);
		yield return new WaitForSeconds(0.5f);
		WingroveRoot.Instance.PostEvent("ENDLESS_OVERLAY_APPEAR2");
		m_text01.CustomActivate(active: true);
		yield return new WaitForSeconds(3f);
		WingroveRoot.Instance.PostEvent("ENDLESS_OVERLAY_DISAPPEAR");
		m_background01.CustomActivate(active: false);
		m_text01.CustomActivate(active: false);
		yield return new WaitForSeconds(1f);
		Object.Destroy(base.gameObject);
	}
}
