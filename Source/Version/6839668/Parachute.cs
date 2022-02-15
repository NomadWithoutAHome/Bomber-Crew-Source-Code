using BomberCrewCommon;
using UnityEngine;

public class Parachute : MonoBehaviour
{
	private void Start()
	{
		if (Singleton<BomberSpawn>.Instance != null)
		{
			Singleton<BomberSpawn>.Instance.GetBomberSystems().RegisterInteractiveSearchable(typeof(Parachute), GetComponent<InteractiveItem>());
		}
	}
}
