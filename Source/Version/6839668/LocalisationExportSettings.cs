using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Runner Duck/Localisation Settings")]
public class LocalisationExportSettings : ScriptableObject
{
	[Serializable]
	public class ExportSetting
	{
		[SerializeField]
		public string m_exportName;

		[SerializeField]
		public string[] m_toExport;
	}

	[SerializeField]
	public string[] m_languages;

	[SerializeField]
	public ExportSetting[] m_exportSettings;
}
