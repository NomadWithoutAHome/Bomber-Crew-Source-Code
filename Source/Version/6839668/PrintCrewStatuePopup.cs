using BomberCrewCommon;
using UnityEngine;

public class PrintCrewStatuePopup : MonoBehaviour
{
	[SerializeField]
	private GameObject m_processingHierarchy;

	[SerializeField]
	private GameObject m_notProcessingHierarchy;

	[SerializeField]
	private tk2dTextMesh m_errorText;

	[SerializeField]
	private tk2dUIItem m_printButton;

	[SerializeField]
	private tk2dUIItem m_closeButton;

	private PrintCrewStatue m_statuePrinter;

	public void SetUp(PrintCrewStatue statuePrinter)
	{
		m_statuePrinter = statuePrinter;
	}

	private void Awake()
	{
		m_printButton.OnClick += Print;
		m_closeButton.OnClick += Close;
	}

	private void Print()
	{
		m_statuePrinter.DoPrint();
	}

	private void Close()
	{
		Singleton<UIPopupManager>.Instance.DismissCurrentPopup();
	}

	private void Update()
	{
		bool flag = m_statuePrinter.IsProcessing();
		m_processingHierarchy.SetActive(flag);
		m_notProcessingHierarchy.SetActive(!flag);
		string errorString = m_statuePrinter.GetErrorString();
		if (!string.IsNullOrEmpty(errorString))
		{
			m_errorText.text = "ERR: " + m_statuePrinter.GetErrorString();
		}
	}
}
