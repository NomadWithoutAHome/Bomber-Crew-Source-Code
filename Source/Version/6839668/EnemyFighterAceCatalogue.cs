using UnityEngine;

[CreateAssetMenu(menuName = "Bomber Crew/Enemy Fighter Ace Catalogue")]
public class EnemyFighterAceCatalogue : ScriptableObject
{
	[SerializeField]
	private EnemyFighterAce[] m_allEnemyFighterAces;

	public EnemyFighterAce[] GetEnemyFighterAces()
	{
		return m_allEnemyFighterAces;
	}
}
