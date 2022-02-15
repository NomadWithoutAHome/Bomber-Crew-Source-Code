using BomberCrewCommon;
using UnityEngine;

public class AltitudeDisplay : MonoBehaviour
{
	private BomberState m_bomberState;

	[SerializeField]
	private UIGaugeDisplay m_altitudeGauge;

	[SerializeField]
	private bool m_normalisedDisplay;

	private void Start()
	{
		m_bomberState = Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBomberState();
	}

	private void Update()
	{
		if (m_normalisedDisplay)
		{
			m_altitudeGauge.DisplayValue(m_bomberState.GetAltitudeNormalised());
		}
		else
		{
			m_altitudeGauge.DisplayValue(m_bomberState.GetAltitude());
		}
	}
}
