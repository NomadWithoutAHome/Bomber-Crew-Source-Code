using BomberCrewCommon;
using UnityEngine;

public class EnemyFighterAcesBoardItem : MonoBehaviour
{
	[SerializeField]
	private tk2dTextMesh m_nameTextMesh;

	[SerializeField]
	private Renderer[] m_portraitRenderers;

	[SerializeField]
	private GameObject m_identityKnownHierarchy;

	[SerializeField]
	private GameObject m_identityUnknownHierarchy;

	[SerializeField]
	private GameObject m_downedHierarchy;

	[SerializeField]
	private GameObject m_fundsHierarchy;

	[SerializeField]
	private TextSetter m_fundsText;

	public void SetUp(EnemyFighterAce enemyFighterAce, bool identityKnown)
	{
		string text = enemyFighterAce.GetFirstName() + "\n" + enemyFighterAce.GetSurname();
		m_nameTextMesh.text = text;
		Renderer[] portraitRenderers = m_portraitRenderers;
		foreach (Renderer renderer in portraitRenderers)
		{
			renderer.sharedMaterial = Object.Instantiate(renderer.sharedMaterial);
			renderer.sharedMaterial.SetTexture("_MainTex", enemyFighterAce.GetPortraitTexture());
		}
		m_identityKnownHierarchy.SetActive(identityKnown);
		m_identityUnknownHierarchy.SetActive(!identityKnown);
		m_fundsText.SetText(Singleton<GameFlow>.Instance.GetGameMode().ReplaceCurrencySymbol($"£{enemyFighterAce.GetCashBonus():N0}"));
	}

	public void SetDowned(bool downed)
	{
		m_downedHierarchy.SetActive(downed);
		m_fundsHierarchy.SetActive(!downed);
	}
}
