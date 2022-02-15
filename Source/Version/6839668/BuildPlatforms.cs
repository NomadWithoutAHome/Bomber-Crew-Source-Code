using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Runner Duck/Build Batch Settings")]
public class BuildPlatforms : ScriptableObject
{
	[Serializable]
	public class BuildPlatform
	{
		[SerializeField]
		public string m_folderName;

		[SerializeField]
		public string m_flags;

		[SerializeField]
		public bool m_buildISO;

		[SerializeField]
		public string m_steamDepotId;

		[SerializeField]
		public bool m_developmentBuild;

		[SerializeField]
		public string m_dlcFolderName;

		[SerializeField]
		public bool m_buildAssetBundles;

		[SerializeField]
		public bool m_buildAssetBundlesToResources;

		[SerializeField]
		public bool m_splitAssetBundlesAfterBuild;

		[SerializeField]
		public bool m_buildExtraScenesAsAssetBundle;

		[SerializeField]
		public bool m_cleanUpSceneAbs;

		[SerializeField]
		public bool m_deletePrevious;

		[SerializeField]
		public bool m_createFullDirectory;

		[SerializeField]
		public string m_dlcBaseGameTitleId;

		[SerializeField]
		public string m_titleSecret;

		[SerializeField]
		public string m_titleIdPath;

		[SerializeField]
		public string m_publisherName;

		[SerializeField]
		public string m_paramSFOFolder;

		[SerializeField]
		public string m_previousBuildPackagePath;

		[SerializeField]
		public string m_ps4LastPatchPackagePath;
	}

	[Serializable]
	public class DLCBuildDepot
	{
		[SerializeField]
		public string[] m_assetBundleNames;

		[SerializeField]
		public string m_steamDepotId;

		[SerializeField]
		public int m_switchDLCIndex;

		[SerializeField]
		public TextAsset m_xboxOnePackageInfo;

		[SerializeField]
		public string m_ps4PackageInfoFolder;

		[SerializeField]
		public string m_ps4EntitlementKey;

		[SerializeField]
		public string m_ps4ContentId;

		[SerializeField]
		public string m_internalName;
	}

	[SerializeField]
	public string m_buildPathRoot;

	[SerializeField]
	public string m_productName;

	[SerializeField]
	private string m_buildLabelFolder;

	[SerializeField]
	public string[] m_allScenes;

	[SerializeField]
	public string m_flags;

	[SerializeField]
	public BuildPlatform[] m_allPlatforms;

	[SerializeField]
	public DLCBuildDepot[] m_dlcDepots;

	[SerializeField]
	public bool m_deleteBuildDirectoryFirst;

	[SerializeField]
	public UnityEngine.Object m_switchIcon;

	[SerializeField]
	public bool m_xboxNoEncryption;

	[SerializeField]
	public bool m_xboxTestEnc;

	[SerializeField]
	public bool m_xboxProdEnc;

	[SerializeField]
	public string m_xboxContentId;

	[SerializeField]
	public string m_switchTitleId = "0x01007900080b6000";

	[SerializeField]
	public bool m_steamUpload;

	[SerializeField]
	public string m_steamAppId;

	[SerializeField]
	public string m_setLiveOnBranch;

	[SerializeField]
	public string m_steamworksRoot = "D:/Steamworks/";

	[SerializeField]
	public string m_vdfFilePath = "D:/Steamworks/BomberCrewSteamBuildFiles/";

	[SerializeField]
	public string m_versionPrefix;

	[SerializeField]
	public bool m_isPatch;

	[SerializeField]
	public string m_patchVersionSwitchDisplay = "1.0.0";

	[SerializeField]
	public int m_patchVersionSwitchRelease;

	[SerializeField]
	public bool m_ps4DayOnePatch;

	[SerializeField]
	public TextAsset[] m_dlcVersionTexts;

	public string GetBuildLabelFolder(string version)
	{
		return string.Format(m_buildLabelFolder, version);
	}
}
