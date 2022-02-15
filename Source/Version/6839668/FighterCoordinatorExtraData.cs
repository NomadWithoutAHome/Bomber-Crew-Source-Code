using UnityEngine;

[CreateAssetMenu(menuName = "Bomber Crew/Extra Fighter Data")]
public class FighterCoordinatorExtraData : ScriptableObject
{
	[SerializeField]
	private FighterCoordinator.FighterType[] m_extraFighters;

	public FighterCoordinator.FighterType[] GetExtraFighters()
	{
		return m_extraFighters;
	}
}
