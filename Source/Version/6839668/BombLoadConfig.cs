using UnityEngine;

[CreateAssetMenu(menuName = "Bomber Crew/BombLoadConfig")]
public class BombLoadConfig : ScriptableObject
{
	[SerializeField]
	[NamedText]
	private string m_namedTextReference;

	[SerializeField]
	private GameObject m_bombLoadPrefab;

	[SerializeField]
	private GameObject m_uiSelectorSwitchesPrefab;

	public GameObject GetBombLoadPrefab()
	{
		return m_bombLoadPrefab;
	}

	public GameObject GetUiSelectorSwitchesPrefab()
	{
		return m_uiSelectorSwitchesPrefab;
	}
}
