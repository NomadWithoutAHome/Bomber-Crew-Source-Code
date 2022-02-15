using BomberCrewCommon;
using UnityEngine;

public class ViewControlsPopupPage : MonoBehaviour
{
	[SerializeField]
	private GameObject m_pcControlsHierarchy;

	[SerializeField]
	private GameObject m_consoleControlsHierarchy;

	[SerializeField]
	private SelectableFilterButton m_pcControlsButton;

	[SerializeField]
	private SelectableFilterButton m_consoleControlsButton;

	[SerializeField]
	private GameObject m_tabSelectHierarchy;

	[SerializeField]
	private UISelectFinder m_finder;

	private void OnEnable()
	{
		Singleton<UISelector>.Instance.SetFinder(m_finder);
	}

	private void Start()
	{
		if (true)
		{
			m_tabSelectHierarchy.SetActive(value: false);
			SetControls(Singleton<UISelector>.Instance.IsPrimary());
		}
		else
		{
			m_tabSelectHierarchy.SetActive(value: false);
			m_consoleControlsHierarchy.SetActive(value: true);
			m_pcControlsHierarchy.SetActive(value: false);
		}
		m_pcControlsButton.OnClick += delegate
		{
			SetControls(controller: false);
		};
		m_consoleControlsButton.OnClick += delegate
		{
			SetControls(controller: true);
		};
	}

	private void SetControls(bool controller)
	{
		m_pcControlsButton.SetSelected(!controller);
		m_pcControlsHierarchy.SetActive(!controller);
		m_consoleControlsButton.SetSelected(controller);
		m_consoleControlsHierarchy.SetActive(controller);
	}
}
