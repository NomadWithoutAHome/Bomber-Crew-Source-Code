using System.Collections.Generic;
using UnityEngine;

public class StationPanel : MonoBehaviour
{
	[SerializeField]
	private Renderer m_stationViewRenderer;

	[SerializeField]
	private PanelSkillLockableButton[] m_skillLockableButtons;

	private List<TextureRenderCamera> m_usingCamera = new List<TextureRenderCamera>();

	private Station m_station;

	private Crewman m_forCrewman;

	public void SetUpStationView(Station station, Crewman forCrewman)
	{
		m_station = station;
		m_forCrewman = forCrewman;
		PanelSkillLockableButton[] skillLockableButtons = m_skillLockableButtons;
		foreach (PanelSkillLockableButton panelSkillLockableButton in skillLockableButtons)
		{
			panelSkillLockableButton.SetUp(m_forCrewman);
		}
		Camera[] stationCameras = station.GetStationCameras();
		if (stationCameras != null && m_stationViewRenderer != null)
		{
			Camera[] array = stationCameras;
			foreach (Camera camera in array)
			{
				TextureRenderCamera component = camera.GetComponent<TextureRenderCamera>();
				m_usingCamera.Add(component);
				RenderTexture mainTexture = component.RegisterUse();
				m_stationViewRenderer.material.mainTexture = mainTexture;
			}
		}
		SetUpStation();
	}

	public virtual UISelectFinder GetOverrideFinder()
	{
		return null;
	}

	public virtual void FlashLock()
	{
	}

	protected virtual void SetUpStation()
	{
	}

	private void OnDestroy()
	{
		foreach (TextureRenderCamera item in m_usingCamera)
		{
			item.DeregisterUse();
		}
	}

	protected Station GetStation()
	{
		return m_station;
	}

	protected Crewman GetCrewman()
	{
		return m_forCrewman;
	}
}
