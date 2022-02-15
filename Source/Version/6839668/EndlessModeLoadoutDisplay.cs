using System;
using UnityEngine;

public class EndlessModeLoadoutDisplay : MonoBehaviour
{
	[SerializeField]
	private TextSetter m_titleTextSetter;

	[SerializeField]
	private tk2dUIItem m_loadoutClick;

	[SerializeField]
	private GameObject[] m_emptyHierarchies;

	[SerializeField]
	private GameObject[] m_filledHierarchies;

	[SerializeField]
	private Renderer m_imageRenderer;

	[SerializeField]
	private tk2dUIItem m_deleteButton;

	[SerializeField]
	private GameObject[] m_deleteEnableHierarchies;

	private EndlessModeVariant.Loadout m_loadout;

	public event Action OnClick;

	public event Action OnDelete;

	private void OnEnable()
	{
		m_loadoutClick.OnClick += ClickLoadout;
		m_deleteButton.OnClick += DeleteSlot;
	}

	public void EnableDeleteButton(bool enable)
	{
		GameObject[] deleteEnableHierarchies = m_deleteEnableHierarchies;
		foreach (GameObject gameObject in deleteEnableHierarchies)
		{
			gameObject.SetActive(enable);
		}
	}

	public void SetUp(EndlessModeVariant.Loadout loadout)
	{
		m_loadout = loadout;
		m_titleTextSetter.SetTextFromLanguageString(loadout.m_title);
		GameObject[] emptyHierarchies = m_emptyHierarchies;
		foreach (GameObject gameObject in emptyHierarchies)
		{
			gameObject.SetActive(value: false);
		}
		GameObject[] filledHierarchies = m_filledHierarchies;
		foreach (GameObject gameObject2 in filledHierarchies)
		{
			gameObject2.SetActive(value: true);
		}
		m_imageRenderer.sharedMaterial = UnityEngine.Object.Instantiate(m_imageRenderer.material);
		m_imageRenderer.sharedMaterial.mainTexture = loadout.GetTexture();
	}

	public void SetUp(EndlessModeVariant.LoadoutJS loadout, string mode, int index)
	{
		if (loadout == null)
		{
			GameObject[] emptyHierarchies = m_emptyHierarchies;
			foreach (GameObject gameObject in emptyHierarchies)
			{
				gameObject.SetActive(value: true);
			}
			GameObject[] filledHierarchies = m_filledHierarchies;
			foreach (GameObject gameObject2 in filledHierarchies)
			{
				gameObject2.SetActive(value: false);
			}
			return;
		}
		GameObject[] emptyHierarchies2 = m_emptyHierarchies;
		foreach (GameObject gameObject3 in emptyHierarchies2)
		{
			gameObject3.SetActive(value: false);
		}
		GameObject[] filledHierarchies2 = m_filledHierarchies;
		foreach (GameObject gameObject4 in filledHierarchies2)
		{
			gameObject4.SetActive(value: true);
		}
		m_titleTextSetter.SetText(loadout.m_title);
		m_imageRenderer.sharedMaterial = UnityEngine.Object.Instantiate(m_imageRenderer.material);
		m_imageRenderer.sharedMaterial.mainTexture = loadout.GetTexture(mode, index);
	}

	private void ClickLoadout()
	{
		if (this.OnClick != null)
		{
			this.OnClick();
		}
	}

	private void DeleteSlot()
	{
		if (this.OnDelete != null)
		{
			this.OnDelete();
		}
	}
}
