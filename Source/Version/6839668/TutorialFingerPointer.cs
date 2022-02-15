using BomberCrewCommon;
using UnityEngine;

public class TutorialFingerPointer : MonoBehaviour
{
	[SerializeField]
	private Transform m_offsetNode;

	[SerializeField]
	private WorldToScreenTracker m_tracker;

	[SerializeField]
	private GameObject m_sidewaysHierarchy;

	[SerializeField]
	private GameObject m_aboveHierarchy;

	private TopBarInfoQueue.PointerHint m_ph;

	public void SetUp(TopBarInfoQueue.PointerHint ph)
	{
		m_offsetNode.transform.localPosition = ph.m_offset2D;
		GameObject toPointAt = ph.m_toPointAt;
		m_ph = ph;
		if (!string.IsNullOrEmpty(ph.m_toPointAtReference))
		{
			UISelectorPointingHint hintForName = Singleton<UISelector>.Instance.GetHintForName(m_ph.m_toPointAtReference);
			if (hintForName != null)
			{
				toPointAt = hintForName.GetPointingHint().gameObject;
			}
		}
		if (toPointAt != null)
		{
			if (ph.m_worldSpace)
			{
				m_tracker.SetTracking(toPointAt.transform, tk2dCamera.CameraForLayer(base.gameObject.layer));
			}
			else
			{
				m_tracker.enabled = false;
				base.transform.position = toPointAt.transform.position;
			}
			m_aboveHierarchy.SetActive(ph.m_downArrow);
			m_sidewaysHierarchy.SetActive(!ph.m_downArrow);
		}
		else
		{
			m_aboveHierarchy.SetActive(value: false);
			m_sidewaysHierarchy.SetActive(value: false);
		}
	}

	private void Update()
	{
		GameObject toPointAt = m_ph.m_toPointAt;
		if (string.IsNullOrEmpty(m_ph.m_toPointAtReference))
		{
			return;
		}
		UISelectorPointingHint hintForName = Singleton<UISelector>.Instance.GetHintForName(m_ph.m_toPointAtReference);
		if (hintForName != null)
		{
			toPointAt = hintForName.GetPointingHint().gameObject;
		}
		if (toPointAt != null)
		{
			if (m_ph.m_worldSpace)
			{
				m_tracker.SetTracking(toPointAt.transform, tk2dCamera.CameraForLayer(base.gameObject.layer));
			}
			else
			{
				m_tracker.enabled = false;
				base.transform.position = toPointAt.transform.position;
			}
			m_aboveHierarchy.SetActive(m_ph.m_downArrow);
			m_sidewaysHierarchy.SetActive(!m_ph.m_downArrow);
		}
		else
		{
			m_aboveHierarchy.SetActive(value: false);
			m_sidewaysHierarchy.SetActive(value: false);
		}
	}
}
