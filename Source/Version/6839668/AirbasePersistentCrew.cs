using System.Collections;
using System.Collections.Generic;
using BomberCrewCommon;
using UnityEngine;

public class AirbasePersistentCrew : MonoBehaviour
{
	[SerializeField]
	private GameObject m_avatarPrefab;

	[SerializeField]
	private Transform[] m_freeWalkTargets;

	private List<Transform> m_freeWalkTransforms = new List<Transform>();

	private List<GameObject> m_crewmenAvatars = new List<GameObject>();

	private AirbasePersistentCrewTarget m_currentTarget;

	private bool m_isImpatient;

	private bool m_isFreeWalking;

	private Dictionary<Crewman, GameObject> m_crewmenAvatarsLinked = new Dictionary<Crewman, GameObject>();

	private bool m_allInitialised;

	public void InitialiseFor(Crewman cm)
	{
		GameObject value = null;
		m_crewmenAvatarsLinked.TryGetValue(cm, out value);
		if (value == null)
		{
			value = Object.Instantiate(m_avatarPrefab);
			value.transform.parent = base.transform;
			value.transform.position = m_freeWalkTargets[Random.Range(0, m_freeWalkTargets.Length)].position;
			value.transform.localRotation = Quaternion.identity;
			value.GetComponent<AirbaseCrewmanAvatarBehaviour>().SetCrewman(cm);
			value.GetComponent<CrewQuartersCrewmanAvatar>().SetUp(cm);
		}
		else
		{
			value.GetComponent<CrewQuartersCrewmanAvatar>().RefreshEquipment();
		}
		m_crewmenAvatars.Add(value);
		m_crewmenAvatarsLinked[cm] = value;
	}

	public void Refresh()
	{
		StopAllCoroutines();
		int currentCrewCount = Singleton<CrewContainer>.Instance.GetCurrentCrewCount();
		List<Crewman> list = new List<Crewman>();
		list.AddRange(m_crewmenAvatarsLinked.Keys);
		m_crewmenAvatars.Clear();
		for (int i = 0; i < currentCrewCount; i++)
		{
			Crewman crewman = Singleton<CrewContainer>.Instance.GetCrewman(i);
			if (!crewman.IsDead())
			{
				InitialiseFor(crewman);
				list.Remove(crewman);
			}
		}
		foreach (Crewman item in list)
		{
			Object.DestroyImmediate(m_crewmenAvatarsLinked[item]);
			m_crewmenAvatarsLinked.Remove(item);
		}
		if (m_isFreeWalking)
		{
			DoFreeWalk();
		}
	}

	private void RefreshGearOnly()
	{
		foreach (GameObject crewmenAvatar in m_crewmenAvatars)
		{
			crewmenAvatar.GetComponent<CrewQuartersCrewmanAvatar>().RefreshEquipment();
		}
	}

	public void SetIsImpatient()
	{
		m_isImpatient = true;
	}

	public void DoTargetedWalk(AirbasePersistentCrewTarget currentTarget, Crewman cm, int index, bool forceWarp)
	{
		m_isFreeWalking = false;
		StopAllCoroutines();
		GameObject gameObject = m_crewmenAvatarsLinked[cm];
		if (gameObject != null)
		{
			Transform[] startTransformsDirect = currentTarget.GetStartTransformsDirect();
			Transform[] endTransforms = currentTarget.GetEndTransforms();
			Transform transform = endTransforms[index];
			if ((transform.transform.position - gameObject.transform.position).magnitude > currentTarget.GetMaxDist() || forceWarp)
			{
				gameObject.GetComponent<AirbaseCrewmanAvatarBehaviour>().WarpTo(startTransformsDirect[index].position, startTransformsDirect[index]);
			}
			Transform[] endTransforms2 = currentTarget.GetEndTransforms();
			gameObject.GetComponent<AirbaseCrewmanAvatarBehaviour>().WalkTo(endTransforms2[index].position, endTransforms2[index], 1f, 25f, null, 1f);
		}
	}

	public void DoTargetedWalk(AirbasePersistentCrewTarget currentTarget)
	{
		m_isFreeWalking = false;
		StopAllCoroutines();
		if (m_isImpatient)
		{
			m_currentTarget = currentTarget;
			Transform[] endTransforms = m_currentTarget.GetEndTransforms();
			int num = 0;
			foreach (GameObject crewmenAvatar in m_crewmenAvatars)
			{
				if (crewmenAvatar != null && endTransforms[num] != null)
				{
					crewmenAvatar.GetComponent<AirbaseCrewmanAvatarBehaviour>().WarpTo(endTransforms[num].position, endTransforms[num]);
				}
				num++;
			}
			m_isImpatient = false;
			return;
		}
		int num2 = 0;
		if (m_currentTarget != currentTarget)
		{
			m_currentTarget = currentTarget;
			List<Transform> startTransforms = m_currentTarget.GetStartTransforms(m_crewmenAvatars.Count);
			Transform[] endTransforms2 = m_currentTarget.GetEndTransforms();
			foreach (GameObject crewmenAvatar2 in m_crewmenAvatars)
			{
				int index = num2 % startTransforms.Count;
				if (crewmenAvatar2 != null && startTransforms[index] != null)
				{
					Transform transform = endTransforms2[num2];
					if ((transform.transform.position - crewmenAvatar2.transform.position).magnitude > m_currentTarget.GetMaxDist())
					{
						crewmenAvatar2.GetComponent<AirbaseCrewmanAvatarBehaviour>().WarpTo(startTransforms[index].position, startTransforms[index]);
					}
				}
				num2++;
			}
		}
		num2 = 0;
		Transform[] endTransforms3 = m_currentTarget.GetEndTransforms();
		foreach (GameObject crewmenAvatar3 in m_crewmenAvatars)
		{
			if (crewmenAvatar3 != null && endTransforms3[num2] != null)
			{
				crewmenAvatar3.GetComponent<AirbaseCrewmanAvatarBehaviour>().WalkTo(endTransforms3[num2].position, endTransforms3[num2], 1f, 25f, null, 1f);
			}
			num2++;
		}
	}

	public void DoFreeWalk()
	{
		m_freeWalkTransforms.Clear();
		m_freeWalkTransforms.AddRange(m_freeWalkTargets);
		m_currentTarget = null;
		m_isFreeWalking = true;
		m_isImpatient = false;
		foreach (GameObject crewmenAvatar in m_crewmenAvatars)
		{
			StartCoroutine(DoFreeWalkCo(crewmenAvatar));
		}
	}

	private IEnumerator DoFreeWalkCo(GameObject go)
	{
		while (!(go == null))
		{
			bool reachedLocation = false;
			int index = Random.Range(0, m_freeWalkTransforms.Count);
			Transform target = m_freeWalkTransforms[index];
			go.GetComponent<AirbaseCrewmanAvatarBehaviour>().WalkTo(target.position, null, 1f, float.MaxValue, delegate
			{
				reachedLocation = true;
			}, Random.Range(0.25f, 1f));
			m_freeWalkTransforms.Remove(target);
			while (!reachedLocation)
			{
				yield return null;
			}
			yield return new WaitForSeconds(Random.Range(2f, 5f));
			m_freeWalkTransforms.Add(target);
		}
	}

	public void ReturnAllToCurrentPositions()
	{
		if (!(m_currentTarget != null))
		{
			return;
		}
		int num = 0;
		Transform[] endTransforms = m_currentTarget.GetEndTransforms();
		foreach (GameObject crewmenAvatar in m_crewmenAvatars)
		{
			if (crewmenAvatar != null && endTransforms[num] != null)
			{
				crewmenAvatar.GetComponent<AirbaseCrewmanAvatarBehaviour>().WalkTo(endTransforms[num].position, endTransforms[num], 1f, 25f, null, 1f);
			}
			num++;
		}
	}

	public GameObject GetCrewmanAvatar(int index)
	{
		InitialisePersistentCrew();
		return m_crewmenAvatars[index];
	}

	public GameObject GetCrewmanAvatar(Crewman forCrewman)
	{
		InitialisePersistentCrew();
		return m_crewmenAvatarsLinked[forCrewman];
	}

	private void Awake()
	{
		m_isFreeWalking = true;
		Singleton<CrewContainer>.Instance.OnNewCrewman += Refresh;
		Singleton<CrewContainer>.Instance.OnCrewmanRemoved += Refresh;
		Singleton<AirbaseNavigation>.Instance.OnChangeArea += RefreshGearOnly;
	}

	private void OnDestroy()
	{
		if (Singleton<CrewContainer>.Instance != null)
		{
			Singleton<CrewContainer>.Instance.OnNewCrewman -= Refresh;
		}
		if (Singleton<CrewContainer>.Instance != null)
		{
			Singleton<CrewContainer>.Instance.OnCrewmanRemoved -= Refresh;
		}
		if (Singleton<AirbaseNavigation>.Instance != null)
		{
			Singleton<AirbaseNavigation>.Instance.OnChangeArea -= RefreshGearOnly;
		}
	}

	public void InitialisePersistentCrew()
	{
		if (!m_allInitialised)
		{
			Refresh();
			m_allInitialised = true;
		}
	}
}
