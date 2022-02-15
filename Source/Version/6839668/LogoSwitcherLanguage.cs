using BomberCrewCommon;
using UnityEngine;

public class LogoSwitcherLanguage : MonoBehaviour
{
	[SerializeField]
	private GameObject m_logoEn;

	[SerializeField]
	private GameObject m_logoZh;

	private bool m_isZhShowing;

	private void Update()
	{
		if (Singleton<SystemDataContainer>.Instance.IsLoadComplete())
		{
			string currentLanguage = Singleton<SystemDataContainer>.Instance.GetCurrentLanguage();
			bool flag = currentLanguage == "zh";
			if (flag != m_isZhShowing)
			{
				m_isZhShowing = flag;
				m_logoEn.SetActive(!flag);
				m_logoZh.SetActive(flag);
			}
		}
	}
}
