using System.Collections.Generic;
using BomberCrewCommon;
using dbox;
using UnityEngine;
using WingroveAudio;

public class PhotoCamera : MonoBehaviour
{
	[SerializeField]
	private Camera m_camera;

	[SerializeField]
	private CrewmanSkillAbilityBool m_takePhotoSkillMidAltBool;

	private List<PhotoTarget> m_allPhotoTargets = new List<PhotoTarget>();

	public void RegisterPhotoTarget(PhotoTarget pt)
	{
		m_allPhotoTargets.Add(pt);
	}

	public bool WouldBeGoodPhoto()
	{
		Plane[] planes = GeometryUtility.CalculateFrustumPlanes(m_camera);
		foreach (PhotoTarget allPhotoTarget in m_allPhotoTargets)
		{
			if (allPhotoTarget != null && allPhotoTarget.enabled && GeometryUtility.TestPlanesAABB(planes, allPhotoTarget.GetBounds()) && !allPhotoTarget.GetHasBeenPhotographed())
			{
				return true;
			}
		}
		return false;
	}

	public float GetCurrentPhotoVisibility(CrewmanAvatar ca)
	{
		if (ca == null)
		{
			return 0f;
		}
		BomberState bomberState = Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBomberState();
		float targetAltitudeAtIndex = bomberState.GetTargetAltitudeAtIndex(0);
		float targetAltitudeAtIndex2 = bomberState.GetTargetAltitudeAtIndex(1);
		float altitude = bomberState.GetAltitude();
		if (altitude < targetAltitudeAtIndex * 1.1f)
		{
			return 1f;
		}
		if (altitude > targetAltitudeAtIndex2 * 1.1f)
		{
			if (Singleton<CrewmanSkillUpgradeInfo>.Instance.IsUnlocked(m_takePhotoSkillMidAltBool, ca.GetCrewman()))
			{
				Vector3 position = bomberState.transform.position;
				Vector3 endPos = position;
				position.y = targetAltitudeAtIndex2;
				endPos.y = targetAltitudeAtIndex;
				float cloudinessPlayer = Singleton<VisibilityHelpers>.Instance.GetCloudinessPlayer(position, endPos);
				float t = Mathf.InverseLerp(targetAltitudeAtIndex2 * 1.1f, targetAltitudeAtIndex2 * 1.2f, altitude);
				return Mathf.Lerp(1f - cloudinessPlayer, 0f, t);
			}
			return 0f;
		}
		if (Singleton<CrewmanSkillUpgradeInfo>.Instance.IsUnlocked(m_takePhotoSkillMidAltBool, ca.GetCrewman()))
		{
			Vector3 position2 = bomberState.transform.position;
			Vector3 endPos2 = position2;
			position2.y = targetAltitudeAtIndex2;
			endPos2.y = targetAltitudeAtIndex;
			float cloudinessPlayer2 = Singleton<VisibilityHelpers>.Instance.GetCloudinessPlayer(position2, endPos2);
			float t2 = Mathf.InverseLerp(targetAltitudeAtIndex, targetAltitudeAtIndex2, altitude) * 2f;
			return Mathf.Lerp(1f, 1f - cloudinessPlayer2, t2);
		}
		float num = Mathf.InverseLerp(targetAltitudeAtIndex, targetAltitudeAtIndex2, altitude) * 2f;
		return Mathf.Clamp01(1f - num);
	}

	public void TakePhoto(CrewmanAvatar ca)
	{
		if (!(ca != null))
		{
			return;
		}
		DboxInMissionController.DBoxCall(DboxSdkWrapper.PostTakePhoto);
		WingroveRoot.Instance.PostEvent("PHOTO_CAMERA_CLICK");
		bool flag = false;
		float currentPhotoVisibility = GetCurrentPhotoVisibility(ca);
		if (Random.Range(0f, 1f) < currentPhotoVisibility)
		{
			Plane[] planes = GeometryUtility.CalculateFrustumPlanes(m_camera);
			foreach (PhotoTarget allPhotoTarget in m_allPhotoTargets)
			{
				if (allPhotoTarget != null && GeometryUtility.TestPlanesAABB(planes, allPhotoTarget.GetBounds()) && Singleton<VisibilityHelpers>.Instance.IsVisibleGeneric(base.transform.position, allPhotoTarget.transform.position))
				{
					flag = true;
					allPhotoTarget.SetHasBeenPhotographed();
				}
			}
			if (flag)
			{
				WingroveRoot.Instance.PostEvent("PHOTO_GOOD_CONFIRM");
				Singleton<SpeechPrioritiser>.Instance.RequestNewSpeech(StaticSpeechSets.PhotoTaken, ca);
			}
			else
			{
				Singleton<SpeechPrioritiser>.Instance.RequestNewSpeech(StaticSpeechSets.PhotoNotTaken, ca);
			}
		}
		else
		{
			Singleton<SpeechPrioritiser>.Instance.RequestNewSpeech(StaticSpeechSets.PhotoBlurry, ca);
		}
	}
}
