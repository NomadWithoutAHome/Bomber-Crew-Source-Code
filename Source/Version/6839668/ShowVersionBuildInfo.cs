using BomberCrewCommon;
using UnityEngine;

public class ShowVersionBuildInfo : MonoBehaviour
{
	[SerializeField]
	private string m_versionFormat;

	[SerializeField]
	private TextSetter m_versionInfoText;

	private void Start()
	{
		if (Singleton<VersionInfo>.Instance != null)
		{
			m_versionInfoText.SetText(string.Format(m_versionFormat, Singleton<VersionInfo>.Instance.GetChangeListVersion()));
		}
		else
		{
			m_versionInfoText.SetText("#editor custom#");
		}
	}

	private void Update()
	{
	}
}
