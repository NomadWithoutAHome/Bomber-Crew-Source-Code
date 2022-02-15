using BomberCrewCommon;
using UnityEngine;

public class PlaceholderMission : MonoBehaviour
{
	private void Start()
	{
		TopBarInfoQueue.TopBarRequest tbr = new TopBarInfoQueue.TopBarRequest("This is a placeholder mission that hasn't been built yet", null, isAlert: true, "Icon_Temperature", 0f, 60f, 30f, 1);
		Singleton<TopBarInfoQueue>.Instance.RegisterRequest(tbr);
	}
}
