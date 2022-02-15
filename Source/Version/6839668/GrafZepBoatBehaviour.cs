using System;
using BomberCrewCommon;
using UnityEngine;

public class GrafZepBoatBehaviour : MonoBehaviour
{
	[SerializeField]
	private MissionPlaceableObject m_placeable;

	[SerializeField]
	private float m_movementSpeed;

	[SerializeField]
	private float m_rotateSpeed;

	[SerializeField]
	private GameObject m_toEnableOnRunaway;

	private bool m_shouldRunAway;

	private MissionPlaceableObject m_runAwayTarget;

	private void Start()
	{
		MissionCoordinator instance = Singleton<MissionCoordinator>.Instance;
		instance.OnTrigger = (Action<string>)Delegate.Combine(instance.OnTrigger, new Action<string>(OnTrigger));
		string parameter = m_placeable.GetParameter("runAwayTarget");
		if (!string.IsNullOrEmpty(parameter) && parameter != "REF_-1")
		{
			m_runAwayTarget = Singleton<MissionCoordinator>.Instance.GetPlaceableByRef(parameter);
		}
	}

	private void OnTrigger(string tr)
	{
		if (tr == "GRAFZEP_RUNAWAY")
		{
			m_shouldRunAway = true;
			GetComponent<LauncherSite>().Stop();
			if (m_toEnableOnRunaway != null)
			{
				m_toEnableOnRunaway.SetActive(value: true);
			}
		}
	}

	private void Update()
	{
		if (!m_shouldRunAway)
		{
			return;
		}
		Vector3d vector3d = m_runAwayTarget.gameObject.btransform().position - base.gameObject.btransform().position;
		vector3d.y = 0.0;
		if (vector3d.magnitude > 0.0)
		{
			Quaternion to = Quaternion.LookRotation((Vector3)vector3d.normalized);
			base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, to, m_rotateSpeed * Time.deltaTime);
			if (Vector3.Dot(base.transform.forward, (Vector3)vector3d.normalized) > 0.9f)
			{
				base.transform.position += base.transform.forward * Time.deltaTime * m_movementSpeed;
			}
		}
	}
}
