using BomberCrewCommon;
using UnityEngine;

public class FightersHazardArea : MonoBehaviour
{
	[SerializeField]
	private MissionPlaceableObject m_placeableObject;

	private bool m_spawned;

	private void Start()
	{
		if (m_placeableObject.GetParameter("notIfAce") == "true")
		{
			AceFighterSpawner[] array = Object.FindObjectsOfType<AceFighterSpawner>();
			AceFighterSpawner[] array2 = array;
			foreach (AceFighterSpawner aceFighterSpawner in array2)
			{
				if (aceFighterSpawner.GetWillSpawn())
				{
					return;
				}
			}
			Spawn();
		}
		else
		{
			Spawn();
		}
	}

	private void Spawn()
	{
		FighterWing fighterWing = new FighterWing();
		int result = 3;
		int.TryParse(m_placeableObject.GetParameter("numFighters"), out result);
		string parameter = m_placeableObject.GetParameter("groupType");
		string[] array = ((!string.IsNullOrEmpty(parameter)) ? parameter.Split(",".ToCharArray()) : new string[1] { Singleton<FighterCoordinator>.Instance.GetDefaultFighterId() });
		bool shouldHunt = m_placeableObject.GetParameter("hunt") == "true";
		Vector3d vector3d = base.gameObject.btransform().position;
		if (m_placeableObject.GetParameter("forceSpawnInRange") == "true" && !Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBomberState().IsAboveEngland())
		{
			Vector3d position = Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBigTransform().position;
			vector3d = position + (vector3d - position).normalized * 2500.0;
		}
		for (int i = 0; i < result; i++)
		{
			GameObject fighterForId = Singleton<FighterCoordinator>.Instance.GetFighterForId(array[i % array.Length]);
			if (fighterForId != null)
			{
				GameObject gameObject = Object.Instantiate(fighterForId);
				gameObject.transform.parent = null;
				gameObject.transform.localScale = fighterForId.transform.localScale;
				gameObject.btransform().position = vector3d + new Vector3d(i * 50, 0f, 0f);
				gameObject.GetComponent<FighterPlane>().SetFromArea(fighterWing, shouldHunt);
			}
			else
			{
				DebugLogWrapper.LogError("Couldn't spawn fighters for type: " + array[i % array.Length]);
			}
		}
		Singleton<FighterCoordinator>.Instance.RegisterWing(fighterWing);
	}
}
