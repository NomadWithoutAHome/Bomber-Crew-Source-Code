using System;
using BomberCrewCommon;
using UnityEngine;

public class DLCShowDetailsButton : MonoBehaviour
{
	[SerializeField]
	private Renderer m_mainSprite;

	[SerializeField]
	private GameObject m_ownedHierarchy;

	[SerializeField]
	private tk2dUIItem m_button;

	private string m_dlcName;

	private void Awake()
	{
		Singleton<DLCManager>.Instance.OnDLCUpdate += Refresh;
	}

	private void OnDestroy()
	{
		if (Singleton<DLCManager>.Instance != null)
		{
			Singleton<DLCManager>.Instance.OnDLCUpdate -= Refresh;
		}
	}

	private void Refresh(string packName)
	{
		m_ownedHierarchy.SetActive(Singleton<DLCManager>.Instance.GetCurrentInstalledDLCs().Contains(m_dlcName));
	}

	public void SetUp(DLCShowArea.ExpectedDLC forDLC, Action onButtonPress)
	{
		m_dlcName = forDLC.m_dlcPackName;
		m_mainSprite.sharedMaterial = UnityEngine.Object.Instantiate(m_mainSprite.sharedMaterial);
		m_mainSprite.sharedMaterial.mainTexture = forDLC.m_dlcSmallTexture;
		m_button.OnClick += onButtonPress;
		Refresh(null);
	}
}
