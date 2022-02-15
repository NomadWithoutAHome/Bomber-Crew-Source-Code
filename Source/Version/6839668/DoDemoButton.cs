using BomberCrewCommon;
using UnityEngine;

public class DoDemoButton : MonoBehaviour
{
	[SerializeField]
	private tk2dUIItem m_demoButton;

	[SerializeField]
	private UISelectFinder m_finder;

	private void Awake()
	{
		m_demoButton.OnClick += DoDemo;
	}

	private void OnEnable()
	{
		Singleton<UISelector>.Instance.SetFinder(m_finder);
	}

	private void DoDemo()
	{
		Singleton<GameFlow>.Instance.DoDemo();
	}
}
