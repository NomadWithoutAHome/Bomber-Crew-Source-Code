using System;
using System.Collections.Generic;
using AudioNames;
using BomberCrewCommon;
using Common;
using UnityEngine;
using WingroveAudio;

public class TagManager : Singleton<TagManager>
{
	[Serializable]
	public class TagTypePrefab
	{
		[SerializeField]
		public TaggableItem.TaggableType m_type;

		[SerializeField]
		public GameObject m_prefab;

		[SerializeField]
		public GameObject m_fullyTaggedPrefab;
	}

	public class TaggableItemPairing
	{
		public TaggableItem m_taggableItem;

		public GameObject m_createdTagItem;

		public TagUIProgress m_uiProgress;

		public bool m_isTagComplete;
	}

	[SerializeField]
	private TagTypePrefab[] m_tagTypePrefabs;

	[SerializeField]
	private Transform m_uiHierarchy;

	[SerializeField]
	private tk2dCamera m_uiCamera;

	[SerializeField]
	private float m_circleSpotDistance = 640f;

	[SerializeField]
	private float m_spottingProgressRate = 0.5f;

	[SerializeField]
	private float m_spottingDecreaseRate = 4f;

	private List<TaggableItemPairing> m_taggableItemPairs = new List<TaggableItemPairing>();

	private BomberSystems m_currentBomber;

	private bool m_taggingSoundPlaying;

	private void Start()
	{
		m_currentBomber = Singleton<BomberSpawn>.Instance.GetBomberSystems();
	}

	public tk2dCamera GetUICamera()
	{
		return m_uiCamera;
	}

	public void RegisterTaggable(TaggableItem ti)
	{
		if (GetTagFor(ti) != null)
		{
			return;
		}
		TaggableItemPairing taggableItemPairing = new TaggableItemPairing();
		taggableItemPairing.m_taggableItem = ti;
		taggableItemPairing.m_isTagComplete = ti.IsFullyTagged();
		GameObject prefabFor = GetPrefabFor(ti, taggableItemPairing.m_isTagComplete);
		if (prefabFor != null)
		{
			taggableItemPairing.m_createdTagItem = UnityEngine.Object.Instantiate(prefabFor);
			taggableItemPairing.m_uiProgress = taggableItemPairing.m_createdTagItem.GetComponent<TagUIProgress>();
			if (taggableItemPairing.m_uiProgress != null)
			{
				taggableItemPairing.m_uiProgress.SetUp(taggableItemPairing.m_taggableItem, taggableItemPairing.m_taggableItem.GetTrackingTramsform(), m_uiCamera);
			}
		}
		m_taggableItemPairs.Add(taggableItemPairing);
	}

	public List<TaggableItemPairing> GetAllTaggableItems()
	{
		return m_taggableItemPairs;
	}

	private GameObject GetPrefabFor(TaggableItem ti, bool complete)
	{
		TagTypePrefab[] tagTypePrefabs = m_tagTypePrefabs;
		foreach (TagTypePrefab tagTypePrefab in tagTypePrefabs)
		{
			if (tagTypePrefab.m_type == ti.GetTaggableType())
			{
				if (complete)
				{
					return tagTypePrefab.m_fullyTaggedPrefab;
				}
				return tagTypePrefab.m_prefab;
			}
		}
		return null;
	}

	public void DeRegisterTaggable(TaggableItem ti)
	{
		TaggableItemPairing tagFor = GetTagFor(ti);
		if (tagFor != null)
		{
			if (tagFor.m_createdTagItem != null)
			{
				UnityEngine.Object.Destroy(tagFor.m_createdTagItem);
			}
			m_taggableItemPairs.Remove(tagFor);
		}
	}

	private TaggableItemPairing GetTagFor(TaggableItem ti)
	{
		foreach (TaggableItemPairing taggableItemPair in m_taggableItemPairs)
		{
			if (taggableItemPair.m_taggableItem == ti)
			{
				return taggableItemPair;
			}
		}
		return null;
	}

	public List<TaggableItem> GetAllOfType(TaggableItem.VisibilityType visType)
	{
		List<TaggableItem> list = new List<TaggableItem>();
		foreach (TaggableItemPairing taggableItemPair in m_taggableItemPairs)
		{
			if (taggableItemPair.m_taggableItem.GetVisibilityType() == visType)
			{
				list.Add(taggableItemPair.m_taggableItem);
			}
		}
		return list;
	}

	public List<TaggableItem> GetFunctionalTagsVisible()
	{
		List<TaggableItem> list = new List<TaggableItem>();
		foreach (TaggableItemPairing taggableItemPair in m_taggableItemPairs)
		{
			if (taggableItemPair.m_uiProgress != null && (taggableItemPair.m_uiProgress.IsVisible() || taggableItemPair.m_uiProgress.IsOffscreenMarkerVisible()) && taggableItemPair.m_taggableItem.IsFunctional())
			{
				list.Add(taggableItemPair.m_taggableItem);
			}
		}
		return list;
	}

	private void Update()
	{
		List<TaggableItemPairing> list = new List<TaggableItemPairing>();
		TagUIProgress tagUIProgress = null;
		float num = m_circleSpotDistance;
		Vector3 position = m_currentBomber.transform.position;
		foreach (TaggableItemPairing taggableItemPair in m_taggableItemPairs)
		{
			if (taggableItemPair.m_taggableItem == null)
			{
				list.Add(taggableItemPair);
				continue;
			}
			if (taggableItemPair.m_isTagComplete != taggableItemPair.m_taggableItem.IsFullyTagged())
			{
				taggableItemPair.m_isTagComplete = taggableItemPair.m_taggableItem.IsFullyTagged();
				if (taggableItemPair.m_createdTagItem != null)
				{
					taggableItemPair.m_createdTagItem.CustomActivate(active: false);
					taggableItemPair.m_createdTagItem = null;
				}
				GameObject prefabFor = GetPrefabFor(taggableItemPair.m_taggableItem, taggableItemPair.m_taggableItem.IsFullyTagged());
				if (prefabFor != null)
				{
					taggableItemPair.m_createdTagItem = UnityEngine.Object.Instantiate(prefabFor);
					taggableItemPair.m_uiProgress = taggableItemPair.m_createdTagItem.GetComponent<TagUIProgress>();
					if (taggableItemPair.m_uiProgress != null)
					{
						taggableItemPair.m_uiProgress.SetUp(taggableItemPair.m_taggableItem, taggableItemPair.m_taggableItem.GetTrackingTramsform(), m_uiCamera);
					}
				}
				else
				{
					taggableItemPair.m_uiProgress = null;
				}
			}
			if (!taggableItemPair.m_isTagComplete)
			{
				bool visible = Singleton<VisibilityHelpers>.Instance.IsVisibleHumanPlayer(position, taggableItemPair.m_taggableItem.GetTrackingTramsform().position, m_currentBomber, taggableItemPair.m_taggableItem.GetVisibilityType() == TaggableItem.VisibilityType.RadarAssisted, taggableItemPair.m_taggableItem.GetVisibilityType() == TaggableItem.VisibilityType.NavigatorAssisted);
				if (taggableItemPair.m_taggableItem.GetVisibilityType() == TaggableItem.VisibilityType.AlwaysVisibleIfVisible)
				{
					visible = (position - taggableItemPair.m_taggableItem.GetTrackingTramsform().position).magnitude < 4000f;
				}
				if (taggableItemPair.m_uiProgress != null)
				{
					taggableItemPair.m_uiProgress.SetVisible(visible);
				}
				if (!(taggableItemPair.m_uiProgress != null) || !taggableItemPair.m_uiProgress.IsVisible())
				{
					continue;
				}
				float num2 = taggableItemPair.m_uiProgress.GetCentreDistance();
				if (taggableItemPair.m_uiProgress.GetProgress() > 0f && num2 < m_circleSpotDistance)
				{
					num2 /= 4f;
					if (num2 < m_circleSpotDistance * 0.25f)
					{
						num2 = 0f;
					}
				}
				if (num2 < num)
				{
					num = num2;
					tagUIProgress = taggableItemPair.m_uiProgress;
				}
			}
			else if (taggableItemPair.m_uiProgress != null)
			{
				taggableItemPair.m_uiProgress.SetVisible(isVisible: true);
			}
		}
		bool flag = false;
		if (Singleton<ContextControl>.Instance.IsTargetingMode())
		{
			foreach (TaggableItemPairing taggableItemPair2 in m_taggableItemPairs)
			{
				if (taggableItemPair2.m_uiProgress != null)
				{
					if (taggableItemPair2.m_uiProgress == tagUIProgress)
					{
						taggableItemPair2.m_uiProgress.Progress(m_spottingProgressRate * Time.deltaTime);
						flag = true;
					}
					else
					{
						taggableItemPair2.m_uiProgress.Progress((0f - m_spottingDecreaseRate) * Time.deltaTime);
					}
					if (taggableItemPair2.m_uiProgress.ShouldTag())
					{
						taggableItemPair2.m_taggableItem.GetComponent<TaggableItem>().SetTagComplete();
					}
				}
			}
		}
		else
		{
			foreach (TaggableItemPairing taggableItemPair3 in m_taggableItemPairs)
			{
				if (taggableItemPair3.m_uiProgress != null)
				{
					taggableItemPair3.m_uiProgress.Progress(-1f);
				}
			}
		}
		if (m_taggingSoundPlaying != flag)
		{
			if (flag)
			{
				WingroveRoot.Instance.PostEvent("TARGETING_START");
			}
			else
			{
				WingroveRoot.Instance.PostEvent("TARGETING_STOP");
				WingroveRoot.Instance.SetParameterGlobal(GUIEvents.Parameters.CacheVal_TargetingProgress(), 0f);
			}
			m_taggingSoundPlaying = flag;
		}
		if (tagUIProgress != null)
		{
			WingroveRoot.Instance.SetParameterGlobal(GUIEvents.Parameters.CacheVal_TargetingProgress(), tagUIProgress.GetProgress());
		}
		foreach (TaggableItemPairing item in list)
		{
			if (item.m_createdTagItem != null)
			{
				UnityEngine.Object.Destroy(item.m_createdTagItem);
			}
			m_taggableItemPairs.Remove(item);
		}
	}
}
