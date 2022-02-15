using System.Collections;
using System.Collections.Generic;
using BomberCrewCommon;
using UnityEngine;
using WingroveAudio;

public class GameCompleteSequence : MonoBehaviour
{
	[SerializeField]
	private GameObject[] m_objectsToSwitchOn;

	[SerializeField]
	private AirbaseCameraNode[] m_endCreditsCameraNodes;

	[SerializeField]
	private AirbasePersistentCrew m_airbaseCrew;

	[SerializeField]
	private UIScreen m_uiScreenCampaignComplete;

	[SerializeField]
	private UIScreen m_uiScreenEndCredits;

	[SerializeField]
	private GameObject[] m_gameObjectsToHide;

	[SerializeField]
	private AirbaseCameraNode m_creditsCamera;

	[SerializeField]
	private AirbaseCameraNode m_overviewCamera;

	[SerializeField]
	private Transform m_creditsTransform;

	[SerializeField]
	private GameObject m_creditsPC;

	[SerializeField]
	private GameObject m_creditsXbox;

	[SerializeField]
	private GameObject m_creditsSwitch;

	[SerializeField]
	private GameObject m_creditsPS4;

	[SerializeField]
	private float m_delayBetweenMissionSummary = 8f;

	[SerializeField]
	private float m_showEnvironmentTimer = 2f;

	[SerializeField]
	private GameObject m_showMissionHistoryPrefab;

	[SerializeField]
	private AirbasePersistentCrewTarget m_crewStandPositions;

	[SerializeField]
	private MonoBehaviour[] m_postprocessToDisableDuringSequence;

	private int m_cameraIndex;

	private bool m_isRunning;

	private GameObject m_creditsInstance;

	private void Start()
	{
	}

	private void OnDestroy()
	{
		if (m_isRunning)
		{
			WingroveRoot.Instance.PostEvent("MUSIC_STOP");
		}
	}

	private void GCSDemoOptions()
	{
		if (GUILayout.Button("Do Game Complete Sequence"))
		{
			DoGameCompleteSequence(Singleton<GameFlow>.Instance.GetCampaign(), forDLC: false);
		}
		if (GUILayout.Button("Do Game Complete Sequence [DLC]"))
		{
			DoGameCompleteSequence(Singleton<GameFlow>.Instance.GetCampaign(), forDLC: true);
		}
	}

	public void DoGameCompleteSequence(CampaignStructure forCampaignStructure, bool forDLC)
	{
		StartCoroutine(GameCompleteMusic(forDLC));
		StartCoroutine(GameCompleteSequenceCo(forCampaignStructure, forDLC));
	}

	private IEnumerator GameCompleteMusic(bool forDLC)
	{
		if (!Singleton<GameFlow>.Instance.GetGameMode().GetUseUSNaming())
		{
			WingroveRoot.Instance.PostEvent("ENDCREDITS");
			if (!forDLC)
			{
				yield return new WaitForSeconds(92f);
				WingroveRoot.Instance.PostEvent("MUSIC_OVERENGLAND_GOOD");
			}
		}
		else
		{
			WingroveRoot.Instance.PostEvent("ENDCREDITS_US");
			yield return new WaitForSeconds(92f);
			WingroveRoot.Instance.PostEvent("MUSIC_OVERENGLAND_GOOD");
		}
	}

	private IEnumerator GameCompleteSequenceCo(CampaignStructure forCampaignStructure, bool forDLC)
	{
		m_isRunning = true;
		GameObject[] objectsToSwitchOn = m_objectsToSwitchOn;
		foreach (GameObject gameObject in objectsToSwitchOn)
		{
			gameObject.SetActive(value: true);
		}
		GameObject[] gameObjectsToHide = m_gameObjectsToHide;
		foreach (GameObject gameObject2 in gameObjectsToHide)
		{
			gameObject2.SetActive(value: false);
		}
		MonoBehaviour[] postprocessToDisableDuringSequence = m_postprocessToDisableDuringSequence;
		foreach (MonoBehaviour monoBehaviour in postprocessToDisableDuringSequence)
		{
			monoBehaviour.enabled = false;
		}
		Singleton<UIScreenManager>.Instance.ShowScreen(m_uiScreenCampaignComplete.name, showNavBarButtons: false);
		Singleton<AirbaseCameraController>.Instance.MoveCameraToLocationInstant(m_creditsCamera);
		m_airbaseCrew.DoTargetedWalk(m_crewStandPositions);
		int numCrew = Singleton<CrewContainer>.Instance.GetCurrentCrewCount();
		for (int l = 0; l < numCrew; l++)
		{
			GameObject crewmanAvatar = m_airbaseCrew.GetCrewmanAvatar(l);
			if (crewmanAvatar != null)
			{
				crewmanAvatar.GetComponent<AirbaseCrewmanAvatarBehaviour>().GetCrewmanGraphics().SwitchToKnackeredController();
			}
		}
		yield return StartCoroutine(ShowMissionCompletionInfo(forCampaignStructure));
		WingroveRoot.Instance.PostEvent("STOP_CLAPPING");
		Singleton<AirbaseCameraController>.Instance.EnableBlur(enable: false, instant: true);
		if (forDLC)
		{
			Singleton<AirbaseCameraController>.Instance.MoveCameraToLocationInstant(m_creditsCamera);
			yield return new WaitForSeconds(15f);
		}
		else
		{
			Singleton<AirbaseCameraController>.Instance.MoveCameraToLocationInstant(m_creditsCamera);
			Singleton<UIScreenManager>.Instance.ShowScreen(m_uiScreenEndCredits.name, showNavBarButtons: false);
			if (m_creditsInstance != null)
			{
				Object.Destroy(m_creditsInstance);
			}
			m_creditsInstance = Object.Instantiate(m_creditsPC);
			m_creditsInstance.transform.parent = m_creditsTransform;
			m_creditsInstance.transform.localPosition = Vector3.zero;
			while (!m_creditsInstance.GetComponent<CreditsPopulator>().IsDone())
			{
				yield return null;
			}
		}
		MonoBehaviour[] postprocessToDisableDuringSequence2 = m_postprocessToDisableDuringSequence;
		foreach (MonoBehaviour monoBehaviour2 in postprocessToDisableDuringSequence2)
		{
			monoBehaviour2.enabled = true;
		}
		if (!forDLC)
		{
			Singleton<GameFlow>.Instance.ReturnToMainMenu();
			yield break;
		}
		GameObject[] objectsToSwitchOn2 = m_objectsToSwitchOn;
		foreach (GameObject gameObject3 in objectsToSwitchOn2)
		{
			gameObject3.SetActive(value: false);
		}
		GameObject[] gameObjectsToHide2 = m_gameObjectsToHide;
		foreach (GameObject gameObject4 in gameObjectsToHide2)
		{
			gameObject4.SetActive(value: true);
		}
		m_airbaseCrew.DoFreeWalk();
		for (int num2 = 0; num2 < numCrew; num2++)
		{
			GameObject crewmanAvatar2 = m_airbaseCrew.GetCrewmanAvatar(num2);
			if (crewmanAvatar2 != null)
			{
				crewmanAvatar2.GetComponent<AirbaseCrewmanAvatarBehaviour>().GetCrewmanGraphics().SwitchToNormalController();
			}
		}
		Singleton<AirbaseCameraController>.Instance.MoveCameraToLocationInstant(m_overviewCamera);
		Singleton<AirbaseNavigation>.Instance.GoToNormal();
		Singleton<UIScreenManager>.Instance.ShowScreen(null, showNavBarButtons: true);
		WingroveRoot.Instance.PostEvent("MUSIC_STOP");
	}

	private IEnumerator ShowMissionCompletionInfo(CampaignStructure forCampaign)
	{
		List<SaveData.CrewCompletedMission> ccmListSource = Singleton<SaveDataContainer>.Instance.Get().GetCrewCompletedMissions();
		CampaignStructure.CampaignMission[] missions = forCampaign.GetAllMissions();
		List<SaveData.CrewCompletedMission> ccmList = new List<SaveData.CrewCompletedMission>();
		List<string> missionNames = new List<string>();
		CampaignStructure.CampaignMission[] array = missions;
		foreach (CampaignStructure.CampaignMission campaignMission in array)
		{
			missionNames.Add(campaignMission.m_missionReferenceName);
		}
		foreach (SaveData.CrewCompletedMission item in ccmListSource)
		{
			if (missionNames.Contains(item.m_missionReference))
			{
				ccmList.Add(item);
			}
		}
		yield return new WaitForSeconds(5f);
		foreach (SaveData.CrewCompletedMission ccm in ccmList)
		{
			m_cameraIndex++;
			if (m_cameraIndex >= m_endCreditsCameraNodes.Length)
			{
				m_cameraIndex = 0;
			}
			Singleton<AirbaseCameraController>.Instance.MoveCameraToLocationInstant(m_endCreditsCameraNodes[m_cameraIndex]);
			Singleton<AirbaseCameraController>.Instance.EnableBlur(enable: false, instant: true);
			yield return new WaitForSeconds(m_showEnvironmentTimer);
			Singleton<AirbaseCameraController>.Instance.EnableBlur(enable: true, instant: false);
			GameObject displayPref = Object.Instantiate(m_showMissionHistoryPrefab);
			displayPref.GetComponent<ShowGameCompleteMissionCompletedInfo>().SetUpFor(forCampaign, ccm);
			yield return new WaitForSeconds(m_delayBetweenMissionSummary);
			Object.Destroy(displayPref);
		}
	}
}
