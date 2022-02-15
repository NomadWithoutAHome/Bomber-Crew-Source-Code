using System;
using BomberCrewCommon;
using UnityEngine;

public class TorpedoBoat : MonoBehaviour
{
	[SerializeField]
	private TaggableItem m_taggableItem;

	[SerializeField]
	private SmoothDamageable m_damageableToMonitor;

	[SerializeField]
	private Animator m_animator;

	[SerializeField]
	private MissionPlaceableObject m_placeable;

	[SerializeField]
	private TorpedoLauncher m_torpedoLauncher;

	[SerializeField]
	private float m_movementSpeed = 25f;

	[SerializeField]
	private string m_isMovingBoolName = "IsMoving";

	private float m_currentCountdown;

	private bool m_isSubmerged;

	private float m_submergeT;

	private bool m_isFullyAtState;

	private Vector3d m_startingPosition;

	private Vector3d m_targetPos;

	private bool m_wantsToLeave;

	private bool m_neverComeBack;

	private void Start()
	{
		m_isFullyAtState = true;
		m_startingPosition = base.gameObject.btransform().position;
		string dontComeBackTrigger = m_placeable.GetParameter("leaveTrigger");
		if (string.IsNullOrEmpty(dontComeBackTrigger))
		{
			return;
		}
		MissionCoordinator instance = Singleton<MissionCoordinator>.Instance;
		instance.OnTrigger = (Action<string>)Delegate.Combine(instance.OnTrigger, (Action<string>)delegate(string st)
		{
			if (st == dontComeBackTrigger)
			{
				m_currentCountdown = Mathf.Min(m_currentCountdown, 1f);
				m_wantsToLeave = true;
				m_neverComeBack = true;
			}
		});
	}

	private void Update()
	{
		if (!(m_damageableToMonitor == null) && !(m_damageableToMonitor.GetHealthNormalised() > 0f))
		{
			return;
		}
		if (m_torpedoLauncher.GetIsChargingAttack())
		{
			Shootable target = m_torpedoLauncher.GetTarget();
			if ((target.GetCentreTransform().position - base.transform.position).magnitude > 800f)
			{
				base.transform.position += base.transform.forward * Time.deltaTime * m_movementSpeed;
			}
			SetAnimatorIsMovingBool(isMoving: true);
		}
		else
		{
			SetAnimatorIsMovingBool(isMoving: false);
		}
	}

	private void SetAnimatorIsMovingBool(bool isMoving)
	{
		m_animator.SetBool(m_isMovingBoolName, isMoving);
	}
}
