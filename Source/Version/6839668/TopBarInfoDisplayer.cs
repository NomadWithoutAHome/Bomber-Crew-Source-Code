using System.Collections.Generic;
using BomberCrewCommon;
using Common;
using UnityEngine;

public class TopBarInfoDisplayer : MonoBehaviour
{
	[SerializeField]
	private TextSetter m_textMesh;

	[SerializeField]
	private TextSetter m_titleText;

	[SerializeField]
	private Renderer m_quadRenderer;

	[SerializeField]
	private tk2dSprite[] m_showIcons;

	[SerializeField]
	private GameObject m_seeMoreButtonHierachy;

	[SerializeField]
	private tk2dUIItem m_seeMoreButton;

	private TopBarInfoQueue.TopBarRequest m_requestToShow;

	private List<GameObject> m_createdPointers = new List<GameObject>();

	private bool m_isRemoving;

	private bool m_doNoRepool;

	private void Start()
	{
		if (m_seeMoreButton != null)
		{
			m_seeMoreButton.OnClick += ClickSeeMore;
		}
	}

	private void ClickSeeMore()
	{
		if (m_requestToShow != null && !string.IsNullOrEmpty(m_requestToShow.GetSeeMore()))
		{
			Singleton<ContextControl>.Instance.GoToManual(m_requestToShow.GetSeeMore());
		}
	}

	public void SetUp(TopBarInfoQueue.TopBarRequest tbr, GameObject pointerPrefab, Transform pointerParent)
	{
		m_requestToShow = tbr;
		if (!string.IsNullOrEmpty(tbr.GetJabberAudioEvent()))
		{
			Singleton<Jabberer>.Instance.StartJabber(tbr.GetJabberAudioEvent(), "SPEECH_RADIO_STATIC", base.gameObject, tbr.GetJabberAudioSettings());
		}
		foreach (TopBarInfoQueue.PointerHint pointerHint in tbr.GetPointerHints())
		{
			GameObject gameObject = Object.Instantiate(pointerPrefab);
			gameObject.GetComponent<TutorialFingerPointer>().SetUp(pointerHint);
			gameObject.transform.parent = pointerParent;
			m_createdPointers.Add(gameObject);
		}
		Refresh();
	}

	private void OnDestroy()
	{
		m_doNoRepool = true;
	}

	public void Remove()
	{
		foreach (GameObject createdPointer in m_createdPointers)
		{
			Object.Destroy(createdPointer);
		}
		m_createdPointers.Clear();
		base.gameObject.CustomActivate(active: false, null, RePool);
	}

	private void RePool()
	{
		Singleton<PoolManager>.Instance.ReturnToPool(base.gameObject);
	}

	private void OnDisable()
	{
		foreach (GameObject createdPointer in m_createdPointers)
		{
			Object.Destroy(createdPointer);
		}
		m_createdPointers.Clear();
	}

	private void Update()
	{
		Refresh();
	}

	private void Refresh()
	{
		if (m_requestToShow.m_isTexture)
		{
			if (m_quadRenderer != null)
			{
				m_quadRenderer.gameObject.SetActive(value: true);
				m_quadRenderer.material.mainTexture = m_requestToShow.m_hintTexture;
			}
		}
		else
		{
			if (m_quadRenderer != null)
			{
				m_quadRenderer.gameObject.SetActive(value: false);
			}
			if (m_showIcons.Length > 0)
			{
				if (m_requestToShow.m_interactionIcon == null)
				{
					tk2dSprite[] showIcons = m_showIcons;
					foreach (tk2dSprite tk2dSprite2 in showIcons)
					{
						tk2dSprite2.gameObject.SetActive(value: false);
					}
				}
				else
				{
					tk2dSprite[] showIcons2 = m_showIcons;
					foreach (tk2dSprite tk2dSprite3 in showIcons2)
					{
						tk2dSprite3.gameObject.SetActive(value: true);
						tk2dSprite3.SetSprite(m_requestToShow.m_interactionIcon);
					}
				}
			}
		}
		if (!string.IsNullOrEmpty(m_requestToShow.m_titleName))
		{
			m_titleText.SetText(m_requestToShow.m_titleName);
			m_titleText.gameObject.SetActive(value: true);
		}
		else if (m_titleText != null)
		{
			m_titleText.gameObject.SetActive(value: false);
		}
		if (string.IsNullOrEmpty(m_requestToShow.GetSeeMore()))
		{
			if (m_seeMoreButtonHierachy != null)
			{
				m_seeMoreButtonHierachy.SetActive(value: false);
			}
		}
		else if (m_seeMoreButtonHierachy != null)
		{
			m_seeMoreButtonHierachy.SetActive(value: true);
		}
		m_textMesh.SetText(m_requestToShow.GetText());
	}
}
