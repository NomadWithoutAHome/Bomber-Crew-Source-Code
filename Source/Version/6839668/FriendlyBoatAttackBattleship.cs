using System;
using System.Collections;
using System.Collections.Generic;
using BomberCrewCommon;
using UnityEngine;

public class FriendlyBoatAttackBattleship : MonoBehaviour
{
	[SerializeField]
	private GameObject m_hierarchyToEnableAttacking;

	[SerializeField]
	private string m_triggerToStartAttack;

	[SerializeField]
	private string m_triggerToStopAttack;

	[SerializeField]
	private GameObject m_attackHitEffect;

	[SerializeField]
	private MissionPlaceableObject m_placeable;

	[SerializeField]
	private SmoothDamageable m_damageable;

	[SerializeField]
	private float m_attackEffectSpawnFrequency;

	[SerializeField]
	private Transform m_turretToRotate;

	[SerializeField]
	private float m_rotateSpeed;

	private bool m_isAttacking;

	private float m_timer;

	private int m_effectInstanceId;

	private MissionPlaceableObject m_grafZep;

	private static int s_attackBoatInstance;

	private float m_delay;

	private void Start()
	{
		s_attackBoatInstance++;
		m_delay = s_attackBoatInstance % 4;
		m_effectInstanceId = m_attackHitEffect.GetInstanceID();
		MissionCoordinator instance = Singleton<MissionCoordinator>.Instance;
		instance.OnTrigger = (Action<string>)Delegate.Combine(instance.OnTrigger, new Action<string>(OnTrigger));
		List<MissionPlaceableObject> objectsByType = Singleton<MissionCoordinator>.Instance.GetObjectsByType("BombTarget");
		foreach (MissionPlaceableObject item in objectsByType)
		{
			if (item.GetComponent<GrafZepBoatBehaviour>() != null)
			{
				m_grafZep = item;
			}
		}
	}

	private void SetAttacking(bool attacking)
	{
		if (attacking != m_isAttacking)
		{
			m_isAttacking = attacking;
			m_hierarchyToEnableAttacking.SetActive(m_isAttacking);
		}
	}

	private void OnTrigger(string trig)
	{
		if (trig == m_triggerToStartAttack && m_damageable.GetHealthNormalised() > 0f)
		{
			StartCoroutine(StartAttackInDelay());
		}
		if (trig == m_triggerToStopAttack)
		{
			SetAttacking(attacking: false);
		}
	}

	public IEnumerator StartAttackInDelay()
	{
		yield return new WaitForSeconds(m_delay);
		if (m_damageable.GetHealthNormalised() > 0f)
		{
			SetAttacking(attacking: true);
		}
	}

	private void Update()
	{
		if (m_damageable.GetHealthNormalised() <= 0f || m_grafZep == null)
		{
			SetAttacking(attacking: false);
		}
		if (m_isAttacking)
		{
			Quaternion to = Quaternion.LookRotation((m_grafZep.transform.position - base.transform.position).normalized, Vector3.up);
			Quaternion rotation = Quaternion.RotateTowards(m_turretToRotate.rotation, to, m_rotateSpeed * Time.deltaTime);
			m_turretToRotate.rotation = rotation;
			m_timer += Time.deltaTime;
			if (m_timer > m_attackEffectSpawnFrequency)
			{
				m_timer = 0f;
				GameObject fromPool = Singleton<PoolManager>.Instance.GetFromPool(m_attackHitEffect, m_effectInstanceId);
				fromPool.transform.position = m_grafZep.transform.position + new Vector3(UnityEngine.Random.Range(-35, 35), 0f, UnityEngine.Random.Range(-35, 35));
				fromPool.transform.rotation = Quaternion.AngleAxis(UnityEngine.Random.Range(0f, 360f), Vector3.up);
			}
		}
	}
}
