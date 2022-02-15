using UnityEngine;

public class GameLoader : MonoBehaviour
{
	private void Awake()
	{
		GameObject gameObject = GameObject.Find("GameSystem");
		if (gameObject == null)
		{
			Application.LoadLevel("InitialLoad");
		}
	}

	private void Update()
	{
	}
}
