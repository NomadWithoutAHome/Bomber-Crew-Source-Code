using System.Collections;
using Common;
using UnityEngine;
using WingroveAudio;

public class EndlessModeOverlayGetReady : MonoBehaviour
{
	[SerializeField]
	private GameObject m_text01;

	[SerializeField]
	private GameObject m_text02;

	[SerializeField]
	private GameObject m_background01;

	[SerializeField]
	private GameObject m_background02;

	private void OnEnable()
	{
		StartCoroutine(DoSequence());
	}

	private IEnumerator DoSequence()
	{
		m_background01.CustomActivate(active: true);
		WingroveRoot.Instance.PostEvent("ENDLESS_OVERLAY_APPEAR");
		yield return new WaitForSeconds(0.5f);
		WingroveRoot.Instance.PostEvent("ENDLESS_OVERLAY_APPEAR2");
		m_text01.CustomActivate(active: true);
		yield return new WaitForSeconds(3f);
		WingroveRoot.Instance.PostEvent("ENDLESS_OVERLAY_WAVE");
		m_background01.SetActive(value: false);
		m_text01.SetActive(value: false);
		m_background02.CustomActivate(active: true);
		m_text02.CustomActivate(active: true);
		yield return new WaitForSeconds(3f);
		WingroveRoot.Instance.PostEvent("ENDLESS_OVERLAY_DISAPPEAR");
		m_background02.CustomActivate(active: false);
		m_text02.CustomActivate(active: false);
		yield return new WaitForSeconds(1f);
		Object.Destroy(base.gameObject);
	}
}
