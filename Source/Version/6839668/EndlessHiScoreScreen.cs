using System.Collections;
using System.Collections.Generic;
using BomberCrewCommon;
using Common;
using UnityEngine;

public class EndlessHiScoreScreen : MonoBehaviour
{
	[SerializeField]
	private GameObject m_hiScoreDisplayObject;

	[SerializeField]
	private LayoutGrid m_layoutGrid;

	[SerializeField]
	private SelectableFilterButton m_nearMeButton;

	[SerializeField]
	private SelectableFilterButton m_top10Button;

	[SerializeField]
	private SelectableFilterButton m_friendsButton;

	[SerializeField]
	private SelectableFilterButton m_globalButton;

	[SerializeField]
	private GameObject m_gettingLeaderboardHierarchy;

	[SerializeField]
	private GameObject m_leaderboardInfoPanel;

	[SerializeField]
	private TextSetter m_leaderboardInfoText;

	[SerializeField]
	private GameObject m_connectHierarchy;

	private List<GameObject> m_currentHiScores = new List<GameObject>();

	private HiScoreIdentifier m_currentHiScoreName;

	private EndlessModeVariant m_currentVariant;

	private HiScoreTable.ScoreListType m_currentScoreType;

	private bool m_isDisplaying;

	private int m_versionDisplaying;

	private static bool s_hasDoneFirstLogin;

	private void Start()
	{
		m_currentScoreType = HiScoreTable.ScoreListType.Top10;
		m_friendsButton.OnClick += FriendsClick;
		m_globalButton.OnClick += GlobalClick;
		m_nearMeButton.OnClick += NearClick;
		m_top10Button.OnClick += TopClick;
		SetButtons();
	}

	private void NearClick()
	{
		switch (m_currentScoreType)
		{
		case HiScoreTable.ScoreListType.FriendsTop:
			m_currentScoreType = HiScoreTable.ScoreListType.FriendsNear;
			break;
		case HiScoreTable.ScoreListType.Top10:
			m_currentScoreType = HiScoreTable.ScoreListType.NearMyScore;
			break;
		}
		SetButtons();
		Clear();
	}

	private void TopClick()
	{
		switch (m_currentScoreType)
		{
		case HiScoreTable.ScoreListType.FriendsNear:
			m_currentScoreType = HiScoreTable.ScoreListType.FriendsTop;
			break;
		case HiScoreTable.ScoreListType.NearMyScore:
			m_currentScoreType = HiScoreTable.ScoreListType.Top10;
			break;
		}
		SetButtons();
		Clear();
	}

	private void FriendsClick()
	{
		switch (m_currentScoreType)
		{
		case HiScoreTable.ScoreListType.NearMyScore:
			m_currentScoreType = HiScoreTable.ScoreListType.FriendsNear;
			break;
		case HiScoreTable.ScoreListType.Top10:
			m_currentScoreType = HiScoreTable.ScoreListType.FriendsTop;
			break;
		}
		SetButtons();
		Clear();
	}

	private void GlobalClick()
	{
		switch (m_currentScoreType)
		{
		case HiScoreTable.ScoreListType.FriendsNear:
			m_currentScoreType = HiScoreTable.ScoreListType.NearMyScore;
			break;
		case HiScoreTable.ScoreListType.FriendsTop:
			m_currentScoreType = HiScoreTable.ScoreListType.Top10;
			break;
		}
		SetButtons();
		Clear();
	}

	private void OnEnable()
	{
		Clear();
		m_isDisplaying = false;
		m_versionDisplaying = -1;
		m_currentVariant = Singleton<EndlessModeGameFlow>.Instance.GetCurrentEndlessMode();
		m_currentHiScoreName = Singleton<EndlessModeGameFlow>.Instance.GetCurrentEndlessMode().GetLeaderboardIds();
		SetButtons();
		Singleton<HiScoreInterface>.Instance.RequestLogin(requestedByPlayer: false);
		Singleton<MainActionButtonMonitor>.Instance.AddListener(ButtonListener);
	}

	private void OnDisable()
	{
		Singleton<MainActionButtonMonitor>.Instance.RemoveListener(ButtonListener, invalidateCurrentPress: false);
	}

	private bool ButtonListener(MainActionButtonMonitor.ButtonPress bp)
	{
		if (bp == MainActionButtonMonitor.ButtonPress.TopAction && !Singleton<HiScoreInterface>.Instance.IsOnline() && !Singleton<HiScoreInterface>.Instance.IsWorking() && Singleton<HiScoreInterface>.Instance.ShouldShowConnectPrompt())
		{
			Singleton<HiScoreInterface>.Instance.RequestLogin(requestedByPlayer: true);
			return true;
		}
		return false;
	}

	private void SetButtons()
	{
		bool flag = false;
		bool flag2 = false;
		switch (m_currentScoreType)
		{
		case HiScoreTable.ScoreListType.FriendsNear:
			flag = true;
			flag2 = true;
			break;
		case HiScoreTable.ScoreListType.FriendsTop:
			flag = true;
			break;
		case HiScoreTable.ScoreListType.NearMyScore:
			flag2 = true;
			break;
		}
		m_friendsButton.SetSelected(flag);
		m_globalButton.SetSelected(!flag);
		m_nearMeButton.SetSelected(flag2);
		m_top10Button.SetSelected(!flag2);
	}

	private void Clear()
	{
		StopAllCoroutines();
		foreach (GameObject currentHiScore in m_currentHiScores)
		{
			Object.DestroyImmediate(currentHiScore);
		}
		m_currentHiScores.Clear();
		m_isDisplaying = false;
		m_versionDisplaying = -1;
	}

	private void Refresh(HiScoreTable.HiScoreList hsl)
	{
		Clear();
		List<HiScoreTable.HiScore> list = new List<HiScoreTable.HiScore>();
		list.AddRange(hsl.m_scores);
		bool flag = false;
		foreach (HiScoreTable.HiScore item in list)
		{
			if (item.m_isLocalPlayer)
			{
				flag = true;
			}
		}
		int num = 0;
		foreach (HiScoreTable.HiScore item2 in list)
		{
			GameObject gameObject = Object.Instantiate(m_hiScoreDisplayObject);
			gameObject.transform.parent = m_layoutGrid.transform;
			gameObject.transform.localScale = Vector3.one;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localEulerAngles = Vector3.zero;
			m_currentHiScores.Add(gameObject);
			gameObject.GetComponent<HiScoreDisplay>().SetUp(item2.m_rank, item2.m_name, item2.m_score, item2.m_isLocalPlayer);
			num++;
			if (num == 10 && !flag)
			{
				break;
			}
		}
		if (!flag && hsl.m_scoreListType == HiScoreTable.ScoreListType.Top10)
		{
			HiScoreTable.HiScore fetchedHiScore = hsl.m_fromTable.GetFetchedHiScore();
			if (fetchedHiScore != null && fetchedHiScore.m_rank != -1)
			{
				string playerName = fetchedHiScore.m_name;
				if (string.IsNullOrEmpty(playerName))
				{
					playerName = Singleton<HiScoreInterface>.Instance.GetPlayerName();
				}
				if (!string.IsNullOrEmpty(playerName))
				{
					GameObject gameObject2 = Object.Instantiate(m_hiScoreDisplayObject);
					gameObject2.transform.parent = m_layoutGrid.transform;
					gameObject2.transform.localScale = Vector3.one;
					gameObject2.transform.localPosition = Vector3.zero;
					gameObject2.transform.localEulerAngles = Vector3.zero;
					m_currentHiScores.Add(gameObject2);
					gameObject2.GetComponent<HiScoreDisplay>().SetUp(fetchedHiScore.m_rank, playerName, fetchedHiScore.m_score, isPlayer: true);
				}
			}
		}
		m_layoutGrid.RepositionChildren();
		foreach (GameObject currentHiScore in m_currentHiScores)
		{
			currentHiScore.SetActive(value: false);
		}
		StartCoroutine(DoShow());
		m_isDisplaying = true;
		m_versionDisplaying = hsl.m_version;
	}

	private IEnumerator DoShow()
	{
		foreach (GameObject go in m_currentHiScores)
		{
			go.CustomActivate(active: true);
			yield return new WaitForSeconds(0.1f);
		}
	}

	private void Update()
	{
		if (!m_isDisplaying)
		{
			HiScoreTable.HiScoreList nameList = Singleton<HiScoreInterface>.Instance.GetNameList(m_currentHiScoreName, m_currentScoreType);
			Singleton<HiScoreInterface>.Instance.CheckRefresh();
			if (!Singleton<HiScoreInterface>.Instance.IsOnline())
			{
				if (Singleton<HiScoreInterface>.Instance.IsWorking())
				{
					m_leaderboardInfoPanel.SetActive(value: false);
					m_gettingLeaderboardHierarchy.SetActive(value: true);
					m_connectHierarchy.SetActive(value: false);
				}
				else
				{
					m_leaderboardInfoPanel.SetActive(value: true);
					m_gettingLeaderboardHierarchy.SetActive(value: false);
					m_connectHierarchy.SetActive(true && Singleton<HiScoreInterface>.Instance.ShouldShowConnectPrompt());
					m_leaderboardInfoText.SetText(Singleton<HiScoreInterface>.Instance.GetErrorText());
				}
			}
			else if (nameList.m_scores != null && nameList.m_scores.Count != 0)
			{
				m_leaderboardInfoPanel.SetActive(value: false);
				m_gettingLeaderboardHierarchy.SetActive(value: false);
				m_connectHierarchy.SetActive(value: false);
				Refresh(nameList);
			}
			else if (Singleton<HiScoreInterface>.Instance.IsWorking())
			{
				m_leaderboardInfoPanel.SetActive(value: false);
				m_gettingLeaderboardHierarchy.SetActive(value: true);
				m_connectHierarchy.SetActive(value: false);
			}
			else if (Singleton<HiScoreInterface>.Instance.IsError())
			{
				m_leaderboardInfoPanel.SetActive(value: true);
				m_gettingLeaderboardHierarchy.SetActive(value: false);
				m_connectHierarchy.SetActive(value: false);
				m_leaderboardInfoText.SetText(Singleton<HiScoreInterface>.Instance.GetErrorText());
			}
			else
			{
				m_leaderboardInfoPanel.SetActive(value: true);
				m_connectHierarchy.SetActive(value: false);
				m_gettingLeaderboardHierarchy.SetActive(value: false);
				m_leaderboardInfoText.SetTextFromLanguageString("leaderboard_empty");
			}
		}
		else
		{
			HiScoreTable.HiScoreList nameList2 = Singleton<HiScoreInterface>.Instance.GetNameList(m_currentHiScoreName, m_currentScoreType);
			if (!Singleton<HiScoreInterface>.Instance.IsOnline())
			{
				Clear();
			}
			else if (nameList2.m_version != m_versionDisplaying)
			{
				Clear();
			}
		}
	}
}
