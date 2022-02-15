using System.Collections.Generic;
using BomberCrewCommon;
using UnityEngine;

public class NonBombDroppable : MonoBehaviour
{
	[SerializeField]
	private Bomb m_bomb;

	[SerializeField]
	private NonBombDroppableTargetArea.ExpectedDropType m_dropType;

	private void Awake()
	{
		m_bomb.OnExplode += OnBombExplodes;
	}

	private void OnBombExplodes()
	{
		List<MissionPlaceableObject> objectsByType = Singleton<MissionCoordinator>.Instance.GetObjectsByType("NonBombTarget");
		foreach (MissionPlaceableObject item in objectsByType)
		{
			NonBombDroppableTargetArea component = item.GetComponent<NonBombDroppableTargetArea>();
			if (component.TrySupplyIsSuccessful(this, m_dropType))
			{
				break;
			}
		}
	}
}
