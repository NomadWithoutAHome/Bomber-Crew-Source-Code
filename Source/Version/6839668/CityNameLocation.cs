using BomberCrewCommon;
using UnityEngine;

public class CityNameLocation : MonoBehaviour
{
	[SerializeField]
	private string m_geonamesId;

	private string m_translatedTextName;

	private void Awake()
	{
		m_translatedTextName = Singleton<CityNamesHolder>.Instance.GetLocalisedCityName(m_geonamesId);
		Singleton<CityNameProximityDetection>.Instance.RegisterCity(this);
	}

	public string GetTranslatedTextName()
	{
		return m_translatedTextName;
	}
}
