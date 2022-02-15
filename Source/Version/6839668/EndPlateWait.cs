using System.Collections;
using BomberCrewCommon;
using Rewired;
using UnityEngine;

public class EndPlateWait : MonoBehaviour
{
	private IEnumerator Start()
	{
		yield return new WaitForSeconds(3f);
		while (true)
		{
			if (ReInput.players.GetPlayer(0).GetButtonDown(12) || Input.GetMouseButtonDown(0) || Input.GetMouseButton(1) || ReInput.players.GetPlayer(0).GetButtonDown(2) || ReInput.players.GetPlayer(0).GetButtonDown(21))
			{
				Singleton<GameFlow>.Instance.EndSlateEnded();
			}
			yield return null;
		}
	}

	private void Update()
	{
	}
}
