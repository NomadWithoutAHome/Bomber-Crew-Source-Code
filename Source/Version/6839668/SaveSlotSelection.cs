using System;
using BomberCrewCommon;
using UnityEngine;

public class SaveSlotSelection : MonoBehaviour
{
	[SerializeField]
	private TextSetter m_mainTextMesh;

	[SerializeField]
	private TextSetter m_subTextInfo;

	[SerializeField]
	private tk2dUIItem m_selectButton;

	[SerializeField]
	private GameObject[] m_emptyHierarchies;

	[SerializeField]
	private GameObject[] m_usedHierarchies;

	[SerializeField]
	[NamedText]
	private string m_saveInfoNamedText;

	[SerializeField]
	private GameObject m_completedHierarchy;

	[SerializeField]
	private Renderer m_photoRenderer;

	[SerializeField]
	private TextSetter m_bomberName;

	private DateTime m_dateTime;

	private bool m_isFilled;

	public event Action<int> OnSaveSelect;

	public DateTime GetDateTime()
	{
		return m_dateTime;
	}

	public bool GetIsFilled()
	{
		return m_isFilled;
	}

	public void SetUp(int slotIndex, bool isLoading)
	{
		bool flag = (m_isFilled = Singleton<SaveDataContainer>.Instance.Exists(slotIndex));
		if (flag)
		{
			GameObject[] emptyHierarchies = m_emptyHierarchies;
			foreach (GameObject gameObject in emptyHierarchies)
			{
				gameObject.SetActive(value: false);
			}
			GameObject[] usedHierarchies = m_usedHierarchies;
			foreach (GameObject gameObject2 in usedHierarchies)
			{
				gameObject2.SetActive(value: true);
			}
			m_photoRenderer.sharedMaterial = UnityEngine.Object.Instantiate(m_photoRenderer.sharedMaterial);
			m_photoRenderer.sharedMaterial.mainTexture = Singleton<SaveDataContainer>.Instance.LoadCrewPhoto(slotIndex);
			SaveData saveData = Singleton<SaveDataContainer>.Instance.TempLoadSlot(slotIndex);
			if (saveData != null)
			{
				m_mainTextMesh.SetText($"{saveData.GetSaveTime():dd MMM yyyy HH:mm}");
				m_bomberName.SetText($"\"{saveData.GetCurrentBomber().GetName()}\"");
				m_subTextInfo.SetText(string.Format(Singleton<LanguageProvider>.Instance.GetNamedTextImmediate("ui_savedata_description"), (!saveData.HasStats()) ? saveData.GetNumMissionsPlayed() : saveData.GetNumMissionsFlownActual(), saveData.GetBalance()));
				if (saveData.HasCompletedMission("C08_KEY") || saveData.HasCompletedMission("DLCMP02_C05_KEY"))
				{
					m_completedHierarchy.SetActive(value: true);
				}
				else
				{
					m_completedHierarchy.SetActive(value: false);
				}
			}
			else
			{
				m_mainTextMesh.SetText("[LOADING ERROR]");
				DebugLogWrapper.LogError("Player had loading error- file exists but save does not");
				if (isLoading)
				{
					flag = false;
				}
			}
		}
		else
		{
			m_completedHierarchy.SetActive(value: false);
			GameObject[] emptyHierarchies2 = m_emptyHierarchies;
			foreach (GameObject gameObject3 in emptyHierarchies2)
			{
				gameObject3.SetActive(value: true);
			}
			GameObject[] usedHierarchies2 = m_usedHierarchies;
			foreach (GameObject gameObject4 in usedHierarchies2)
			{
				gameObject4.SetActive(value: false);
			}
		}
		if ((isLoading && flag) || !isLoading)
		{
			m_selectButton.OnClick += delegate
			{
				this.OnSaveSelect(slotIndex);
			};
		}
	}
}
