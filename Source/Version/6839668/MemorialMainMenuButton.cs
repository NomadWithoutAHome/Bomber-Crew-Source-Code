using BomberCrewCommon;
using UnityEngine;

public class MemorialMainMenuButton : MonoBehaviour
{
	[SerializeField]
	private tk2dUIItem m_button;

	private void Awake()
	{
		m_button.OnClick += ClickMainMenu;
	}

	private void ClickMainMenu()
	{
		Singleton<GameFlow>.Instance.ReturnToMainMenu();
	}
}
