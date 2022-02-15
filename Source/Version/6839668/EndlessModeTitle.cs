using BomberCrewCommon;
using UnityEngine;

public class EndlessModeTitle : MonoBehaviour
{
	[SerializeField]
	private TextSetter m_textSetter;

	private void Start()
	{
		Refresh();
	}

	private void Update()
	{
		Refresh();
	}

	private void Refresh()
	{
		m_textSetter.SetTextFromLanguageString(Singleton<EndlessModeGameFlow>.Instance.GetCurrentEndlessMode().GetNamedTextTitle());
	}
}
