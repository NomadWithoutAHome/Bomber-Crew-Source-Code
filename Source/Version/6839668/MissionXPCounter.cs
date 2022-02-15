using BomberCrewCommon;
using UnityEngine;

public class MissionXPCounter : Singleton<MissionXPCounter>
{
	[SerializeField]
	private GameObject m_xpWidgetPrefab;

	[SerializeField]
	private TextSetter m_xpCounter;

	[SerializeField]
	private Animation m_animation;

	[SerializeField]
	private tk2dCamera m_uiCamera;

	private int m_xpTotal;

	private void Start()
	{
		Refresh();
	}

	public void AddXP(int amt, Transform trackingTransform, Vector3 trackPos)
	{
		m_xpTotal += amt;
		GameObject gameObject = Object.Instantiate(m_xpWidgetPrefab);
		gameObject.GetComponent<MissionXPWidget>().SetUp(amt, trackingTransform, trackPos, m_uiCamera);
		m_animation.Play();
		Refresh();
	}

	private void Refresh()
	{
		m_xpCounter.SetText(m_xpTotal.ToString());
	}
}
