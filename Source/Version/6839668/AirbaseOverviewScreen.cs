using UnityEngine;

public class AirbaseOverviewScreen : MonoBehaviour
{
	[SerializeField]
	private AirbasePersistentCrew m_persistentCrew;

	private void Awake()
	{
		AirbaseAreaScreen component = GetComponent<AirbaseAreaScreen>();
		component.OnBackButton += OnBackButton;
	}

	private void OnBackButton()
	{
	}

	private void OnEnable()
	{
		m_persistentCrew.DoFreeWalk();
	}
}
