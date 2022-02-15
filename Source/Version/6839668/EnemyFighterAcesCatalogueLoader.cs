using BomberCrewCommon;
using UnityEngine;

public class EnemyFighterAcesCatalogueLoader : Singleton<EnemyFighterAcesCatalogueLoader>
{
	[SerializeField]
	private EnemyFighterAceCatalogue m_catalogue;

	public EnemyFighterAceCatalogue GetCatalogue()
	{
		return m_catalogue;
	}
}
