using BomberCrewCommon;
using UnityEngine;

public class AirbaseCurrencyDisplay : MonoBehaviour
{
	[SerializeField]
	private UICounterDisplay m_fundsCounter;

	[SerializeField]
	private UICounterDisplay m_intelCounter;

	[SerializeField]
	private TextSetter m_dateText;

	private void Update()
	{
		m_fundsCounter.UpdateValue(Singleton<SaveDataContainer>.Instance.Get().GetBalance());
		m_intelCounter.UpdateValue(Singleton<SaveDataContainer>.Instance.Get().GetIntel());
	}
}
