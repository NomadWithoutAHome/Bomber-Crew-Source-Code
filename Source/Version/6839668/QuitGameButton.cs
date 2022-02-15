using UnityEngine;

public class QuitGameButton : MonoBehaviour
{
	[SerializeField]
	private tk2dUIItem m_quitGameButton;

	private void Awake()
	{
		m_quitGameButton.OnClick += Quit;
	}

	private void Quit()
	{
		Application.Quit();
	}
}
