using UnityEngine;

public class TutorialPromptDisplay : MonoBehaviour
{
	[SerializeField]
	private Transform m_controllerTransform;

	[SerializeField]
	private Transform m_pcTransform;

	[SerializeField]
	private GameObject m_buttonPrefab;

	public void SetUp(TutorialPromptSetUp tps)
	{
		tps.GetPCButtons();
	}
}
