using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameAnalyticsSDK.Setup;

public class Settings : ScriptableObject
{
	public enum HelpTypes
	{
		None,
		IncludeSystemSpecsHelp,
		ProvideCustomUserID
	}

	public enum MessageTypes
	{
		None,
		Error,
		Info,
		Warning
	}

	public struct HelpInfo
	{
		public string Message;

		public MessageTypes MsgType;

		public HelpTypes HelpType;
	}

	public enum InspectorStates
	{
		Account,
		Basic,
		Debugging,
		Pref
	}

	[HideInInspector]
	public static string VERSION = "3.10.0";

	[HideInInspector]
	public static bool CheckingForUpdates = false;

	public int TotalMessagesSubmitted;

	public int TotalMessagesFailed;

	public int DesignMessagesSubmitted;

	public int DesignMessagesFailed;

	public int QualityMessagesSubmitted;

	public int QualityMessagesFailed;

	public int ErrorMessagesSubmitted;

	public int ErrorMessagesFailed;

	public int BusinessMessagesSubmitted;

	public int BusinessMessagesFailed;

	public int UserMessagesSubmitted;

	public int UserMessagesFailed;

	public string CustomArea = string.Empty;

	[SerializeField]
	private List<string> gameKey = new List<string>();

	[SerializeField]
	private List<string> secretKey = new List<string>();

	[SerializeField]
	public List<string> Build = new List<string>();

	[SerializeField]
	public List<string> SelectedPlatformStudio = new List<string>();

	[SerializeField]
	public List<string> SelectedPlatformGame = new List<string>();

	[SerializeField]
	public List<int> SelectedPlatformGameID = new List<int>();

	[SerializeField]
	public List<int> SelectedStudio = new List<int>();

	[SerializeField]
	public List<int> SelectedGame = new List<int>();

	public string NewVersion = string.Empty;

	public string Changes = string.Empty;

	public bool SignUpOpen = true;

	public string FirstName = string.Empty;

	public string LastName = string.Empty;

	public string StudioName = string.Empty;

	public string GameName = string.Empty;

	public string PasswordConfirm = string.Empty;

	public bool EmailOptIn = true;

	public string EmailGA = string.Empty;

	[NonSerialized]
	public string PasswordGA = string.Empty;

	[NonSerialized]
	public string TokenGA = string.Empty;

	[NonSerialized]
	public string ExpireTime = string.Empty;

	[NonSerialized]
	public string LoginStatus = "Not logged in.";

	[NonSerialized]
	public bool JustSignedUp;

	[NonSerialized]
	public bool HideSignupWarning;

	public bool IntroScreen = true;

	[NonSerialized]
	public List<Studio> Studios;

	public bool InfoLogEditor = true;

	public bool InfoLogBuild = true;

	public bool VerboseLogBuild;

	public bool UseManualSessionHandling;

	public bool SendExampleGameDataToMyGame;

	public bool InternetConnectivity;

	public List<string> CustomDimensions01 = new List<string>();

	public List<string> CustomDimensions02 = new List<string>();

	public List<string> CustomDimensions03 = new List<string>();

	public List<string> ResourceItemTypes = new List<string>();

	public List<string> ResourceCurrencies = new List<string>();

	public RuntimePlatform LastCreatedGamePlatform;

	public List<RuntimePlatform> Platforms = new List<RuntimePlatform>();

	public InspectorStates CurrentInspectorState;

	public List<HelpTypes> ClosedHints = new List<HelpTypes>();

	public bool DisplayHints;

	public Vector2 DisplayHintsScrollState;

	public Texture2D Logo;

	public Texture2D UpdateIcon;

	public Texture2D InfoIcon;

	public Texture2D DeleteIcon;

	public Texture2D GameIcon;

	public Texture2D HomeIcon;

	public Texture2D InstrumentIcon;

	public Texture2D QuestionIcon;

	public Texture2D UserIcon;

	public Texture2D AmazonIcon;

	public Texture2D GooglePlayIcon;

	public Texture2D iosIcon;

	public Texture2D macIcon;

	public Texture2D windowsPhoneIcon;

	[NonSerialized]
	public GUIStyle SignupButton;

	public bool UseCustomId;

	public bool UsePlayerSettingsBundleVersion;

	public bool SubmitErrors = true;

	public int MaxErrorCount = 10;

	public bool SubmitFpsAverage = true;

	public bool SubmitFpsCritical = true;

	public bool IncludeGooglePlay = true;

	public int FpsCriticalThreshold = 20;

	public int FpsCirticalSubmitInterval = 1;

	public List<bool> PlatformFoldOut = new List<bool>();

	public bool CustomDimensions01FoldOut;

	public bool CustomDimensions02FoldOut;

	public bool CustomDimensions03FoldOut;

	public bool ResourceItemTypesFoldOut;

	public bool ResourceCurrenciesFoldOut;

	public static readonly RuntimePlatform[] AvailablePlatforms = new RuntimePlatform[9]
	{
		RuntimePlatform.Android,
		RuntimePlatform.IPhonePlayer,
		RuntimePlatform.LinuxPlayer,
		RuntimePlatform.OSXPlayer,
		RuntimePlatform.tvOS,
		RuntimePlatform.WebGLPlayer,
		RuntimePlatform.WindowsPlayer,
		RuntimePlatform.MetroPlayerARM,
		RuntimePlatform.TizenPlayer
	};

	public void SetCustomUserID(string customID)
	{
		if (!(customID != string.Empty))
		{
		}
	}

	public void RemovePlatformAtIndex(int index)
	{
		if (index >= 0 && index < Platforms.Count)
		{
			gameKey.RemoveAt(index);
			secretKey.RemoveAt(index);
			Build.RemoveAt(index);
			SelectedPlatformStudio.RemoveAt(index);
			SelectedPlatformGame.RemoveAt(index);
			SelectedPlatformGameID.RemoveAt(index);
			SelectedStudio.RemoveAt(index);
			SelectedGame.RemoveAt(index);
			PlatformFoldOut.RemoveAt(index);
			Platforms.RemoveAt(index);
		}
	}

	public void AddPlatform(RuntimePlatform platform)
	{
		gameKey.Add(string.Empty);
		secretKey.Add(string.Empty);
		Build.Add("0.1");
		SelectedPlatformStudio.Add(string.Empty);
		SelectedPlatformGame.Add(string.Empty);
		SelectedPlatformGameID.Add(-1);
		SelectedStudio.Add(0);
		SelectedGame.Add(0);
		PlatformFoldOut.Add(item: true);
		Platforms.Add(platform);
	}

	public string[] GetAvailablePlatforms()
	{
		List<string> list = new List<string>();
		for (int i = 0; i < AvailablePlatforms.Length; i++)
		{
			RuntimePlatform runtimePlatform = AvailablePlatforms[i];
			switch (runtimePlatform)
			{
			case RuntimePlatform.IPhonePlayer:
				if (!Platforms.Contains(RuntimePlatform.tvOS) && !Platforms.Contains(runtimePlatform))
				{
					list.Add(runtimePlatform.ToString());
				}
				else if (!Platforms.Contains(runtimePlatform))
				{
					list.Add(runtimePlatform.ToString());
				}
				break;
			case RuntimePlatform.tvOS:
				if (!Platforms.Contains(RuntimePlatform.IPhonePlayer) && !Platforms.Contains(runtimePlatform))
				{
					list.Add(runtimePlatform.ToString());
				}
				else if (!Platforms.Contains(runtimePlatform))
				{
					list.Add(runtimePlatform.ToString());
				}
				break;
			case RuntimePlatform.MetroPlayerARM:
				if (!Platforms.Contains(runtimePlatform))
				{
					list.Add("WSA");
				}
				break;
			default:
				if (!Platforms.Contains(runtimePlatform))
				{
					list.Add(runtimePlatform.ToString());
				}
				break;
			}
		}
		return list.ToArray();
	}

	public bool IsGameKeyValid(int index, string value)
	{
		return true;
	}

	public bool IsSecretKeyValid(int index, string value)
	{
		return true;
	}

	public void UpdateGameKey(int index, string value)
	{
		if (!string.IsNullOrEmpty(value))
		{
			if (IsGameKeyValid(index, value))
			{
				gameKey[index] = value;
			}
			else if (gameKey[index].Equals(value))
			{
				gameKey[index] = string.Empty;
			}
		}
		else
		{
			gameKey[index] = value;
		}
	}

	public void UpdateSecretKey(int index, string value)
	{
		if (!string.IsNullOrEmpty(value))
		{
			if (IsSecretKeyValid(index, value))
			{
				secretKey[index] = value;
			}
			else if (secretKey[index].Equals(value))
			{
				secretKey[index] = string.Empty;
			}
		}
		else
		{
			secretKey[index] = value;
		}
	}

	public string GetGameKey(int index)
	{
		return gameKey[index];
	}

	public string GetSecretKey(int index)
	{
		return secretKey[index];
	}

	public void SetCustomArea(string customArea)
	{
	}

	public void SetKeys(string gamekey, string secretkey)
	{
	}
}
