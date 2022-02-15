using System;
using System.Collections.Generic;
using BomberCrewCommon;
using UnityEngine;
using WingroveAudio;

public class CrewmanAvatar : Damageable
{
	public class CrewmanNotification
	{
		public string m_iconName;
	}

	public class QueuedAction
	{
		public Func<QueuedAction, bool> m_processFunction;

		public Func<QueuedAction, bool> m_canCancelFunction;

		public InteractiveItem.InteractionContract m_contract;

		public ExitPoint m_bailOutExitPoint;

		public BomberWalkZone m_targetWalkZone;

		public Vector3 m_localPlanarPoint;

		public bool m_cancelRequested;

		public FireOverview.Extinguishable m_currentFireArea;

		public Station m_targetStation;
	}

	[SerializeField]
	private tk2dUIItem m_interactableItem;

	[SerializeField]
	private BoxCollider m_selectCollider;

	[SerializeField]
	private float m_walkSpeed = 1.75f;

	[SerializeField]
	private Transform m_markerTrackingTransform;

	[SerializeField]
	private CrewmanGraphicsInstantiate m_crewmanGraphicsInstantiate;

	[SerializeField]
	private CrewmanPhysicsController m_characterController;

	[SerializeField]
	private Vector3 m_characterControllerGravity;

	[SerializeField]
	private GameObject m_parachutePhysicsModelPrefab;

	[SerializeField]
	private GameObject m_deathPhysicsModelPrefab;

	[SerializeField]
	private float m_damageFactorReductionPerArmourPoint = 0.01f;

	[SerializeField]
	private CrewmanLifeStatus m_lifeStatus;

	[SerializeField]
	private GameObject m_walkingToArrowPreviewPrefab;

	[SerializeField]
	private ShowWalkPath m_pathDisplayCurrent;

	[SerializeField]
	private ShowWalkPath m_pathDisplayPreview;

	[SerializeField]
	private LayerMask m_floorNormalLayerMask;

	[SerializeField]
	private Transform m_transformToFollowGraphics;

	[SerializeField]
	private Transform m_transformToFollowGraphicsMO;

	[SerializeField]
	private float m_stationExitDelayTime = 0.3f;

	[SerializeField]
	private float m_stationEnterDelayTime = 0.3f;

	[SerializeField]
	private Transform m_notDownBumpY;

	[SerializeField]
	private float m_notDownBumpAmount;

	private int m_crewmanIndex;

	private Crewman m_crewman;

	private bool m_isSelected;

	private Station m_currentStation;

	private bool m_isAtAStation;

	private Station m_lastStation;

	private CarryableItem m_currentlyCarrying;

	private GameObject m_controlledByObject;

	private BomberWalkZone.WalkTarget m_finalPoint;

	private FlashManager.ActiveFlash m_coldTemperatureFlash;

	private FlashManager.ActiveFlash m_highlightFlash;

	private FlashManager.ActiveFlash m_selectedFlash;

	private bool m_isOutlined;

	private CrewmanGraphics m_crewmanGraphics;

	private BomberSystems m_bomberSystems;

	private Vector3 m_currentFloorNormal;

	private BomberWalkZone.WalkPath m_previewWalkPath;

	private BomberWalkZone.WalkPath m_cachedWalkPath;

	private int m_defenseFactor;

	private int m_temperatureResist;

	private float m_movementSpeedFactor;

	private int m_oxygenTimeSeconds;

	private BomberWalkZone m_currentWalkZone;

	private BomberWalkZone m_nextWalkZoneFromWalkAnimation;

	private GameObject m_currentWalkArrow;

	private CrewWalkArrowPreview m_previewArrowController;

	private float m_currentTemperature;

	private float m_fireDamage;

	private bool m_onFire;

	private List<CrewmanNotification> m_notifications = new List<CrewmanNotification>();

	private int m_notificationsVersion;

	private CrewmanNotification m_temperatureNotification;

	private CrewmanNotification m_oxygenNotification;

	private Transform m_baseParentTransform;

	private GameObject m_pathLinkedObject;

	private float m_notGroundedTime;

	private float m_grip;

	private float m_smoothGrip;

	private bool m_doingWalkAnimation;

	private string m_walkAnimationTriggerName;

	private bool m_movedThisFrame;

	private bool m_didFireFightThisFrame;

	private bool m_didInteractThisFrame;

	private bool m_didHealThisFrame;

	private bool m_didEngineRepairThisFrame;

	private bool m_didFuelTankRepairThisFrame;

	private bool m_didFallThisFrame;

	private bool m_didBraceThisFrame;

	private float m_interactionSpeed;

	private bool m_isBailedOut;

	private bool m_hasSeenBailOutOrder;

	private int m_ragDollRequests;

	private bool m_ragdolledFromSlip;

	private bool m_ragdolledFromNG;

	private float m_timeSinceBurnt;

	private bool m_blockSelection;

	private bool m_blockOrders;

	private bool m_blockMoveOrders;

	private float m_exitStationDelay;

	private float m_enterStationDelay;

	private bool m_isExitingStationCurrently;

	private float m_currentLean;

	private float m_leanTargetThisFrame;

	private bool m_invincible;

	private float m_damageMultiplier = 1f;

	private List<QueuedAction> m_currentQueuedActionsList = new List<QueuedAction>();

	public void SetUp(Crewman crewman, int index, BomberWalkZone walkZone)
	{
		m_currentWalkZone = walkZone;
		m_crewmanIndex = index;
		m_crewman = crewman;
		m_baseParentTransform = base.transform.parent;
		m_crewmanGraphicsInstantiate.Init(m_crewman);
		m_crewmanGraphics = m_crewmanGraphicsInstantiate.GetCrewmanGraphics();
		m_movementSpeedFactor = crewman.GetMovementFactor();
		m_defenseFactor = crewman.GetArmourTotal();
		m_oxygenTimeSeconds = crewman.GetOxygenTime();
		m_temperatureResist = crewman.GetTemperatureResistance();
		m_crewmanGraphics.SetFromCrewman(m_crewman);
		m_crewmanGraphics.SetEquipmentFromCrewman(m_crewman);
		m_lifeStatus.SetUp(m_temperatureResist, m_defenseFactor, m_oxygenTimeSeconds);
		m_lifeStatus.OnDead += ToDead;
		m_lifeStatus.OnCountdownBegin += ToIncapacitated;
		m_lifeStatus.OnRevive += ToRevived;
		m_currentWalkArrow = UnityEngine.Object.Instantiate(m_walkingToArrowPreviewPrefab);
		m_previewArrowController = m_currentWalkArrow.GetComponent<CrewWalkArrowPreview>();
		m_previewArrowController.SetVisible(visible: false);
		m_bomberSystems = Singleton<BomberSpawn>.Instance.GetBomberSystems();
		m_crewmanGraphics.CombineMeshes();
	}

	public void SetSelectionBlocked(bool blocked)
	{
		m_blockSelection = blocked;
	}

	public void SetOrdersBlocked(bool blocked)
	{
		m_blockOrders = blocked;
	}

	public void SetInvincible(bool invincible)
	{
		m_invincible = invincible;
	}

	public void SetDamageMultiplier(float da)
	{
		m_damageMultiplier = da;
	}

	public void SetMoveOrdersBlocked(bool blocked)
	{
		m_blockMoveOrders = blocked;
	}

	public bool AreOrdersBlocked()
	{
		return m_blockOrders;
	}

	public bool AreMoveOrdersBlocked()
	{
		bool flag = m_isAtAStation && m_currentStation.IsLockedInStation();
		return m_blockOrders || m_blockMoveOrders || flag;
	}

	public void FighterDestroyed(FighterPlane.SpeechCategory speechCategory)
	{
		switch (speechCategory)
		{
		case FighterPlane.SpeechCategory.FighterNormal:
			Singleton<CrewMiscSpeechTriggers>.Instance.DoExternalTrigger(CrewMiscSpeechTriggers.SpeechExternalTrigger.FighterDestroyed, this);
			break;
		case FighterPlane.SpeechCategory.EnemyAce:
			Singleton<CrewMiscSpeechTriggers>.Instance.DoExternalTrigger(CrewMiscSpeechTriggers.SpeechExternalTrigger.FighterDestroyedAce, this);
			break;
		case FighterPlane.SpeechCategory.V1Rocket:
			Singleton<CrewMiscSpeechTriggers>.Instance.DoExternalTrigger(CrewMiscSpeechTriggers.SpeechExternalTrigger.V1Destroyed, this);
			break;
		case FighterPlane.SpeechCategory.V2Rocket:
			Singleton<CrewMiscSpeechTriggers>.Instance.DoExternalTrigger(CrewMiscSpeechTriggers.SpeechExternalTrigger.V2Destroyed, this);
			break;
		}
	}

	public float GetTemperature()
	{
		return m_currentTemperature;
	}

	public FlashManager GetFlashManager()
	{
		return m_crewmanGraphics.GetFlashManager();
	}

	public BomberWalkZone GetWalkZone()
	{
		return m_currentWalkZone;
	}

	public int GetNotificationsVersion()
	{
		return m_notificationsVersion;
	}

	public void RegisterNotification(CrewmanNotification n)
	{
		m_notifications.Add(n);
		m_notificationsVersion++;
	}

	public void RemoveNotification(CrewmanNotification n)
	{
		m_notifications.Remove(n);
		m_notificationsVersion++;
	}

	public List<CrewmanNotification> GetAllNotifications()
	{
		return m_notifications;
	}

	public tk2dUIItem GetInteractionItem()
	{
		return m_interactableItem;
	}

	public void AddRagdollRequest()
	{
		m_ragDollRequests++;
		if (m_ragDollRequests == 1)
		{
			m_crewmanGraphics.GetRagdollController().SetEnabled(enabled: true, base.transform);
		}
	}

	public void RemoveRagdollRequest()
	{
		m_ragDollRequests--;
		if (m_ragDollRequests == 0)
		{
			m_crewmanGraphics.GetRagdollController().SetEnabled(enabled: false, base.transform);
			base.transform.position = m_crewmanGraphics.GetPelvisTransform().position;
		}
	}

	public void HoverPath(GameObject linkedObject, BomberWalkZone connectedZone, Vector3 position)
	{
		if (connectedZone == null)
		{
			m_previewWalkPath = m_currentWalkZone.GetFullWalkPath(position, m_previewWalkPath);
		}
		else
		{
			m_previewWalkPath = m_currentWalkZone.GetFullWalkPath(connectedZone, position, m_previewWalkPath);
		}
		m_pathDisplayPreview.ShowPath(this, m_previewWalkPath, position);
		m_pathLinkedObject = linkedObject;
	}

	public void UnhoverPath(GameObject linkedObject)
	{
		if (m_pathLinkedObject == linkedObject || linkedObject == null)
		{
			m_pathDisplayPreview.HidePath();
		}
	}

	public void FlashPath()
	{
		m_pathDisplayPreview.Flash();
	}

	public Transform GetHandLocator()
	{
		return m_crewmanGraphics.GetHandTransform();
	}

	public CrewmanGraphics GetCrewmanGraphics()
	{
		return m_crewmanGraphics;
	}

	public void PickUp(CarryableItem item)
	{
		if (m_currentlyCarrying != null)
		{
			m_currentlyCarrying.Drop(m_crewmanGraphics.GetPelvisTransform().position);
			m_currentlyCarrying = null;
		}
		m_currentlyCarrying = item;
	}

	public void StoreItem(GearRack rack)
	{
		if (m_currentlyCarrying != null && rack.StoreItem(m_currentlyCarrying))
		{
			m_currentlyCarrying = null;
		}
	}

	public Crewman GetCrewman()
	{
		return m_crewman;
	}

	public bool IsIdle()
	{
		return !m_isAtAStation && m_currentQueuedActionsList.Count == 0 && !m_isBailedOut && !m_lifeStatus.IsDead() && !m_lifeStatus.IsCountingDown();
	}

	private void Awake()
	{
		m_interactableItem.OnHoverOver += CrewmanColliderOnHoverOver;
		m_interactableItem.OnHoverOut += CrewmanColliderOnHoverOut;
		m_interactableItem.OnClick += CrewmanColliderOnClick;
	}

	public int GetIndex()
	{
		return m_crewmanIndex;
	}

	public bool Select()
	{
		if (IsSelectable())
		{
			if (!m_isSelected)
			{
				m_isSelected = true;
				m_selectedFlash = m_crewmanGraphics.GetOutlineFlash().AddOrUpdateFlash(0f, 0.5f, 0f, 255, 0.75f, Color.white, m_selectedFlash);
				return true;
			}
			return true;
		}
		return false;
	}

	public bool IsSelectable()
	{
		return !m_lifeStatus.IsDead() && !m_lifeStatus.IsCountingDown() && !m_isBailedOut && !m_blockSelection;
	}

	public void Deselect()
	{
		if (m_isSelected)
		{
			m_isSelected = false;
			m_crewmanGraphics.GetOutlineFlash().RemoveFlash(m_selectedFlash);
			m_pathDisplayPreview.HidePath();
		}
	}

	private void AbandonQueuedAction(QueuedAction qa)
	{
		if (qa.m_contract != null && qa.m_contract.m_contractState != InteractiveItem.InteractionContract.ContractState.Finished)
		{
			qa.m_contract.AbandonContract();
			qa.m_contract = null;
		}
	}

	public void CancelFromContract(InteractiveItem.InteractionContract contract)
	{
		foreach (QueuedAction currentQueuedActions in m_currentQueuedActionsList)
		{
			if (currentQueuedActions.m_contract == contract)
			{
				currentQueuedActions.m_cancelRequested = true;
			}
		}
	}

	public void DropItem()
	{
		if (m_currentlyCarrying != null)
		{
			m_currentlyCarrying.Drop(m_crewmanGraphics.GetPelvisTransform().position);
			m_currentlyCarrying = null;
		}
	}

	public void SetStationSlow(Station s)
	{
		if (m_currentStation != null && s == null)
		{
			if (!m_isExitingStationCurrently)
			{
				m_crewmanGraphics.SetIsAtStation(atStation: false);
				QueuedAction queuedAction = new QueuedAction();
				queuedAction.m_processFunction = ProcessExitStation;
				queuedAction.m_canCancelFunction = (QueuedAction q) => false;
				m_exitStationDelay = m_stationExitDelayTime;
				m_isExitingStationCurrently = true;
				m_currentQueuedActionsList.Insert(0, queuedAction);
			}
		}
		else if (s != null)
		{
			QueuedAction queuedAction2 = new QueuedAction();
			queuedAction2.m_processFunction = ProcessEnterStation;
			queuedAction2.m_targetStation = s;
			queuedAction2.m_canCancelFunction = (QueuedAction q) => false;
			m_enterStationDelay = m_stationEnterDelayTime;
			if (m_currentlyCarrying != null)
			{
				m_currentlyCarrying.Drop(m_crewmanGraphics.GetPelvisTransform().position);
				m_currentlyCarrying = null;
			}
			m_grip = 1f;
			m_currentStation = s;
			m_isAtAStation = true;
			m_lastStation = s;
			m_currentWalkZone = s.GetContainerWalkZone();
			if (m_currentWalkZone != null)
			{
			}
			base.transform.parent = s.GetSeatedTransform();
			base.transform.localPosition = Vector3.zero;
			base.transform.localRotation = Quaternion.identity;
			m_characterController.SetLocked(locked: true);
			m_currentQueuedActionsList.Insert(0, queuedAction2);
			m_crewmanGraphics.SetIsAtStation(atStation: true);
			if (s.GetStationAnimationAction() == Station.StationAction.Sit)
			{
				m_crewmanGraphics.SetIsProne(prone: false);
				m_crewmanGraphics.SetIsStandingAtStation(standingAtStation: false);
				m_crewmanGraphics.SetIsInFoetalPositionAtStation(inFoetalPosition: false);
			}
			else if (s.GetStationAnimationAction() == Station.StationAction.Prone)
			{
				m_crewmanGraphics.SetIsProne(prone: true);
				m_crewmanGraphics.SetIsStandingAtStation(standingAtStation: false);
				m_crewmanGraphics.SetIsInFoetalPositionAtStation(inFoetalPosition: false);
			}
			else if (s.GetStationAnimationAction() == Station.StationAction.Bed)
			{
				m_crewmanGraphics.SetIsProne(prone: true);
				m_crewmanGraphics.SetIsStandingAtStation(standingAtStation: false);
				m_crewmanGraphics.SetIsInFoetalPositionAtStation(inFoetalPosition: false);
			}
			else if (s.GetStationAnimationAction() == Station.StationAction.StandAtStation)
			{
				m_crewmanGraphics.SetIsProne(prone: false);
				m_crewmanGraphics.SetIsStandingAtStation(standingAtStation: true);
				m_crewmanGraphics.SetIsInFoetalPositionAtStation(inFoetalPosition: false);
			}
			else if (s.GetStationAnimationAction() == Station.StationAction.FoetalPosition)
			{
				m_crewmanGraphics.SetIsProne(prone: false);
				m_crewmanGraphics.SetIsStandingAtStation(standingAtStation: false);
				m_crewmanGraphics.SetIsInFoetalPositionAtStation(inFoetalPosition: true);
			}
		}
	}

	public void SetStation(Station s)
	{
		if (m_currentStation != null && s == null)
		{
			m_currentStation.LeaveStation(this);
			m_currentStation = null;
			m_isAtAStation = false;
			m_characterController.SetLocked(locked: false);
			base.transform.parent = m_baseParentTransform;
			if (m_currentWalkZone != null)
			{
				base.transform.position = m_currentWalkZone.GetNearestPlanarPoint(base.transform.position);
			}
			m_crewmanGraphics.SetIsAtStation(atStation: false);
			m_crewmanGraphics.SetIsProne(prone: false);
			m_finalPoint = null;
			m_grip = 1f;
		}
		else if (s != null)
		{
			if (m_currentlyCarrying != null)
			{
				m_currentlyCarrying.Drop(m_crewmanGraphics.GetPelvisTransform().position);
				m_currentlyCarrying = null;
			}
			s.ActivateCrewman();
			m_grip = 1f;
			m_currentStation = s;
			m_isAtAStation = true;
			m_lastStation = s;
			m_currentWalkZone = s.GetContainerWalkZone();
			if (m_currentWalkZone != null)
			{
			}
			base.transform.parent = s.GetSeatedTransform();
			base.transform.localPosition = Vector3.zero;
			base.transform.localRotation = Quaternion.identity;
			m_characterController.SetLocked(locked: true);
			m_crewmanGraphics.SetIsAtStation(atStation: true);
			if (s.GetStationAnimationAction() == Station.StationAction.Sit)
			{
				m_crewmanGraphics.SetIsProne(prone: false);
			}
			else if (s.GetStationAnimationAction() == Station.StationAction.Prone)
			{
				m_crewmanGraphics.SetIsProne(prone: true);
			}
			else if (s.GetStationAnimationAction() == Station.StationAction.Bed)
			{
				m_crewmanGraphics.SetIsProne(prone: true);
			}
		}
	}

	private void SetExternalController(GameObject go)
	{
		m_characterController.SetLocked(locked: true);
		m_controlledByObject = go;
		base.transform.parent = m_controlledByObject.transform;
		base.transform.localScale = Vector3.one;
		base.transform.localPosition = Vector3.zero;
		base.transform.parent = m_controlledByObject.transform.parent;
	}

	public Station GetStation()
	{
		return m_currentStation;
	}

	public bool HasStation()
	{
		return m_isAtAStation;
	}

	public Station GetLastStation()
	{
		return m_lastStation;
	}

	private void ToDead()
	{
		m_crewman.SetDead();
		CancelActions();
		QueuedAction queuedAction = new QueuedAction();
		queuedAction.m_processFunction = ProcessDeath;
		queuedAction.m_canCancelFunction = (QueuedAction q) => false;
		m_currentQueuedActionsList.Add(queuedAction);
		Singleton<MusicSelectionRules>.Instance.TriggerTimed(MusicSelectionRules.MusicTriggerEvents.CrewmanDead, 60f);
	}

	private bool ProcessDeath(QueuedAction qa)
	{
		if (m_currentlyCarrying != null)
		{
			m_currentlyCarrying.Drop(m_crewmanGraphics.GetPelvisTransform().position);
			m_currentlyCarrying = null;
		}
		if (m_currentStation != null)
		{
			if (Singleton<ContextControl>.Instance.UseWaypoints() && m_currentWalkZone != null)
			{
				base.transform.position = m_currentWalkZone.GetNearestPlanarPoint(base.transform.position);
			}
			SetStation(null);
		}
		m_crewmanGraphics.SetIsCollapsed(collapsed: false);
		m_crewmanGraphics.SetIsDead(dead: true);
		GameObject gameObject = UnityEngine.Object.Instantiate(m_deathPhysicsModelPrefab);
		if (m_currentWalkZone == null || m_currentWalkZone.IsExternal())
		{
			gameObject.transform.parent = null;
			gameObject.GetComponent<Rigidbody>().velocity = m_bomberSystems.GetBomberState().GetVelocity();
		}
		else
		{
			gameObject.transform.parent = m_baseParentTransform;
		}
		gameObject.transform.SetPositionAndRotation(base.transform.position, base.transform.rotation);
		SetExternalController(gameObject);
		m_crewmanGraphics.FacialAnimController.SetDead();
		return true;
	}

	private bool ProcessBailedOut(QueuedAction qa)
	{
		if (!m_isBailedOut)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(m_parachutePhysicsModelPrefab);
			Vector3 velocity = Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBomberState().GetPhysicsModel()
				.GetVelocity();
			gameObject.btransform().SetFromCurrentPage((!(qa.m_bailOutExitPoint == null)) ? (qa.m_bailOutExitPoint.GetLocalPageExternalPosition() - velocity * Time.deltaTime) : (base.transform.position - velocity * Time.deltaTime));
			gameObject.GetComponent<CrewmanBailedOutController>().SetUp(this, velocity);
			SetExternalController(gameObject);
		}
		m_isBailedOut = true;
		m_didFallThisFrame = true;
		m_currentWalkZone = null;
		m_crewmanGraphics.SetIsCrawl(crawl: false);
		if (m_controlledByObject != null && m_controlledByObject.GetComponent<CrewmanBailedOutController>().HasLanded())
		{
			m_didFallThisFrame = false;
		}
		if (m_lifeStatus.IsCountingDown())
		{
			InstantKill();
		}
		return false;
	}

	public void ToIncapacitated()
	{
		CancelActions();
		QueuedAction queuedAction = new QueuedAction();
		queuedAction.m_processFunction = ProcessIncapacitated;
		queuedAction.m_canCancelFunction = (QueuedAction q) => m_lifeStatus.IsDead() ? true : false;
		m_currentQueuedActionsList.Add(queuedAction);
	}

	private void ToRevived()
	{
		m_crewmanGraphics.SetIsCollapsed(collapsed: false);
	}

	private bool ProcessExitStation(QueuedAction qa)
	{
		if (!m_isAtAStation)
		{
			m_characterController.SetLocked(locked: false);
			base.transform.parent = m_baseParentTransform;
			base.transform.position = m_currentWalkZone.GetNearestPlanarPoint(base.transform.position);
			m_crewmanGraphics.SetIsAtStation(atStation: false);
			m_crewmanGraphics.SetIsProne(prone: false);
			m_finalPoint = null;
			m_grip = 1f;
			m_isExitingStationCurrently = false;
			return true;
		}
		m_exitStationDelay -= Time.deltaTime;
		if (m_exitStationDelay < 0f)
		{
			m_currentStation.LeaveStation(this);
			m_currentStation = null;
			m_isAtAStation = false;
			m_characterController.SetLocked(locked: false);
			base.transform.parent = m_baseParentTransform;
			base.transform.position = m_currentWalkZone.GetNearestPlanarPoint(base.transform.position);
			m_crewmanGraphics.SetIsAtStation(atStation: false);
			m_crewmanGraphics.SetIsProne(prone: false);
			m_finalPoint = null;
			m_grip = 1f;
			m_isExitingStationCurrently = false;
			return true;
		}
		return false;
	}

	private bool ProcessEnterStation(QueuedAction qa)
	{
		m_enterStationDelay -= Time.deltaTime;
		if (m_enterStationDelay < 0f)
		{
			Station targetStation = qa.m_targetStation;
			targetStation.ActivateCrewman();
			return true;
		}
		return false;
	}

	private bool ProcessIncapacitated(QueuedAction qa)
	{
		Singleton<CrewMiscSpeechTriggers>.Instance.DoExternalTrigger(CrewMiscSpeechTriggers.SpeechExternalTrigger.CrewmanIncapacitated, this);
		if (m_currentlyCarrying != null)
		{
			m_currentlyCarrying.Drop(m_crewmanGraphics.GetPelvisTransform().position);
			m_currentlyCarrying = null;
		}
		if (m_currentStation != null)
		{
			SetStation(null);
		}
		m_crewmanGraphics.SetIsCollapsed(collapsed: true);
		if (m_currentWalkZone.IsExternal())
		{
			InstantKill();
		}
		Singleton<ControllerRumble>.Instance.GetRumbleMixerShock().Kick();
		return true;
	}

	private void CancelActions()
	{
		foreach (QueuedAction currentQueuedActions in m_currentQueuedActionsList)
		{
			currentQueuedActions.m_cancelRequested = true;
		}
	}

	private bool CanCancelActionStandard(QueuedAction qa)
	{
		if (m_doingWalkAnimation)
		{
			return false;
		}
		return true;
	}

	private bool CanCancelActionRunningFromFire(QueuedAction qa)
	{
		if (m_doingWalkAnimation)
		{
			return false;
		}
		if (!m_lifeStatus.IsDead() && !m_lifeStatus.IsCountingDown())
		{
			return false;
		}
		return true;
	}

	private bool IsFrozen()
	{
		return m_doingWalkAnimation;
	}

	private bool ProcessWalkAction(QueuedAction qa)
	{
		if (qa.m_targetWalkZone == null)
		{
			return false;
		}
		m_cachedWalkPath = m_currentWalkZone.GetFullWalkPath(qa.m_targetWalkZone.GetPointFromLocal(qa.m_localPlanarPoint), m_cachedWalkPath);
		if (WalkToTarget(m_cachedWalkPath, movingAsLastPointStay: false))
		{
			return false;
		}
		return true;
	}

	private bool ProcessDoAutoBailOut(QueuedAction qa)
	{
		if (!m_bomberSystems.GetBomberState().IsBailOutOrderActive())
		{
			m_hasSeenBailOutOrder = false;
			AbandonQueuedAction(qa);
			CancelActions();
			return true;
		}
		if (m_currentlyCarrying == null || m_currentlyCarrying.GetComponent<Parachute>() == null)
		{
			if (ProcessInteractionAction(qa))
			{
				List<InteractiveItem> searchableInteractives = m_bomberSystems.GetSearchableInteractives(typeof(ExitPoint));
				float num = float.MaxValue;
				InteractiveItem interactiveItem = null;
				foreach (InteractiveItem item in searchableInteractives)
				{
					if (item.GetInteractionOptionsPublic(this, skipNullCheck: true) != null)
					{
						float magnitude = (item.transform.position - base.transform.position).magnitude;
						if (magnitude < num)
						{
							num = magnitude;
							interactiveItem = item;
						}
					}
				}
				qa.m_contract = interactiveItem.SetIntentInteraction(interactiveItem.GetInteractionOptionsPublic(this, skipNullCheck: true), this);
			}
			else if (qa.m_contract == null)
			{
				return true;
			}
			return false;
		}
		if (qa.m_contract != null)
		{
			return ProcessInteractionAction(qa);
		}
		return true;
	}

	private bool ProcessRunFromFireAction(QueuedAction qa)
	{
		if (!m_onFire)
		{
			return true;
		}
		if (m_timeSinceBurnt > 0.3f)
		{
			SetFinalPosition();
			return true;
		}
		if (m_currentStation != null)
		{
			SetStationSlow(null);
		}
		FireOverview.Extinguishable nearestSafeArea = m_bomberSystems.GetFireOverview().GetNearestSafeArea(base.transform.position, excludeExternal: true);
		FireOverview.Extinguishable nearestFire = m_bomberSystems.GetFireOverview().GetNearestFire(base.transform.position, forNextFire: true);
		if (nearestSafeArea != null && nearestFire != null)
		{
			Vector3 normalized = (nearestSafeArea.GetTransform().position - nearestFire.GetTransform().position).normalized;
			Vector3 currentPositionOfTarget = nearestSafeArea.GetTransform().position + normalized * 1.5f;
			m_cachedWalkPath = m_currentWalkZone.GetFullWalkPath(currentPositionOfTarget, m_cachedWalkPath);
			SetFinalPosition();
			WalkToTarget(m_cachedWalkPath, movingAsLastPointStay: false);
		}
		return false;
	}

	private bool ProcessInteractionAction(QueuedAction qa)
	{
		if (qa.m_contract == null)
		{
			return true;
		}
		Vector3 interactionPositionFor = qa.m_contract.m_item.GetInteractionPositionFor(base.transform, this);
		Vector3 vector = interactionPositionFor - base.transform.position;
		float num = 0.25f;
		if (qa.m_contract.m_interaction.m_looseDistance)
		{
			num = 1.25f;
		}
		bool flag = (qa.m_contract.m_interaction.CanFinishRemotely() || vector.magnitude < num) && !m_characterController.IsBracing() && !m_characterController.DidSlip() && !m_characterController.IsLocked();
		bool flag2 = false;
		if (!flag)
		{
			if (qa.m_contract.m_item.GetContainerWalkZone() != null && qa.m_contract.m_item.IsStatic())
			{
				m_cachedWalkPath = m_currentWalkZone.GetFullWalkPath(qa.m_contract.m_item.GetContainerWalkZone(), qa.m_contract.m_item.GetInteractionPositionFor(base.transform, this), m_cachedWalkPath);
				flag2 = WalkToTarget(m_cachedWalkPath, movingAsLastPointStay: false);
			}
			else
			{
				m_cachedWalkPath = m_currentWalkZone.GetFullWalkPath(qa.m_contract.m_item.GetInteractionPositionFor(base.transform, this), m_cachedWalkPath);
				flag2 = WalkToTarget(m_cachedWalkPath, movingAsLastPointStay: false);
			}
		}
		if (!flag2)
		{
			SetFinalPosition();
			if (qa.m_contract.m_contractState == InteractiveItem.InteractionContract.ContractState.Started)
			{
				float speedMultiplier = 1f;
				switch (qa.m_contract.m_interaction.GetInteractionType())
				{
				case InteractiveItem.Interaction.InteractionAnimType.Healing:
					m_didHealThisFrame = true;
					break;
				case InteractiveItem.Interaction.InteractionAnimType.RepairEngine:
					m_didEngineRepairThisFrame = true;
					break;
				case InteractiveItem.Interaction.InteractionAnimType.RepairFuelTank:
					m_didFuelTankRepairThisFrame = true;
					break;
				default:
					m_didInteractThisFrame = true;
					break;
				}
				m_interactionSpeed = qa.m_contract.m_interaction.GetAnimationMultiplier();
				if (!qa.m_contract.PumpContract(speedMultiplier))
				{
					return true;
				}
				return false;
			}
			if (qa.m_contract.m_contractState == InteractiveItem.InteractionContract.ContractState.IntentRegistered)
			{
				qa.m_contract = qa.m_contract.m_item.StartInteraction(qa.m_contract);
				if (qa.m_contract == null)
				{
					return true;
				}
				return false;
			}
			return true;
		}
		if (qa.m_contract.m_contractState == InteractiveItem.InteractionContract.ContractState.Started)
		{
			qa.m_contract.DontPumpContract();
		}
		return false;
	}

	private bool ProcessFireFightAction(QueuedAction qa)
	{
		bool result = false;
		FireOverview fireOverview = Singleton<BomberSpawn>.Instance.GetBomberSystems().GetFireOverview();
		if (qa.m_currentFireArea == null || !qa.m_currentFireArea.IsOnFire() || !qa.m_currentFireArea.Exists() || !fireOverview.IsNearestBurningInDirection(qa.m_currentFireArea, base.transform.position))
		{
			bool flag = false;
			if (qa.m_currentFireArea != null && !qa.m_currentFireArea.IsOnFire())
			{
				InteractiveItem interactiveItem = qa.m_currentFireArea.NextInteractive();
				qa.m_currentFireArea = null;
				if (interactiveItem != null)
				{
					InteractiveItem.Interaction interactionOptionsPublic = interactiveItem.GetInteractionOptionsPublic(this, skipNullCheck: true);
					if (interactionOptionsPublic != null)
					{
						result = true;
						QueueAction(andCancelExisting: true, interactiveItem, interactionOptionsPublic);
						flag = true;
					}
				}
			}
			if (!flag)
			{
				qa.m_currentFireArea = fireOverview.GetNearestFire(base.transform.position, forNextFire: true);
			}
		}
		if (qa.m_currentFireArea == null)
		{
			result = true;
		}
		else
		{
			Vector3 vector = base.transform.position - qa.m_currentFireArea.GetTransform().position;
			Vector3 vector2 = qa.m_currentFireArea.GetTransform().position + vector.normalized * 3f;
			Transform putOutPositionOverride = qa.m_currentFireArea.GetPutOutPositionOverride();
			if (putOutPositionOverride != null)
			{
				vector2 = putOutPositionOverride.position;
			}
			vector2 = m_bomberSystems.GetAllWalkZones().FindContainingWalkZone(vector2, ignoreExternal: false).GetNearestPlanarPoint(vector2);
			Vector3 vector3 = base.transform.position - vector2;
			bool flag2 = true;
			bool flag3 = true;
			FireOverview.Extinguishable nearestFire = fireOverview.GetNearestFire(base.transform.position, forNextFire: true);
			if (nearestFire != null && (base.transform.position - nearestFire.GetTransform().position).magnitude < 3f)
			{
				flag3 = false;
			}
			if (flag3 && vector3.magnitude > 0.5f)
			{
				m_cachedWalkPath = m_currentWalkZone.GetFullWalkPath(vector2, m_cachedWalkPath);
				flag2 = !WalkToTarget(m_cachedWalkPath, movingAsLastPointStay: false);
			}
			if (flag2)
			{
				base.transform.rotation = Quaternion.LookRotation(-vector.normalized, Vector3.up);
				m_didFireFightThisFrame = true;
				if (m_currentlyCarrying == null || m_currentlyCarrying.GetComponent<FireExtinguisher>().GetCapacity() < 0f)
				{
					UnityEngine.Object.Destroy(m_currentlyCarrying.gameObject);
					result = true;
					WingroveRoot.Instance.PostEventGO("EXTINGUISHER_EMPTY", base.gameObject);
				}
			}
		}
		return result;
	}

	private bool ProcessCurrentAction()
	{
		bool result = false;
		QueuedAction queuedAction = m_currentQueuedActionsList[0];
		if (m_doingWalkAnimation)
		{
			m_crewmanGraphics.ForceAnimationReTrigger(m_walkAnimationTriggerName);
		}
		else
		{
			result = queuedAction.m_processFunction(queuedAction);
		}
		return result;
	}

	public void QueueAction(bool andCancelExisting, Vector3 position, WalkableArea associatedWalkableArea, bool ignoreExternalZones)
	{
		if (andCancelExisting)
		{
			CancelActions();
		}
		QueuedAction queuedAction = new QueuedAction();
		queuedAction.m_processFunction = ProcessWalkAction;
		queuedAction.m_canCancelFunction = CanCancelActionStandard;
		queuedAction.m_targetWalkZone = m_bomberSystems.GetAllWalkZones().FindContainingWalkZone(position, ignoreExternalZones);
		queuedAction.m_localPlanarPoint = queuedAction.m_targetWalkZone.GetNearestPlanarPointLocalFromWorld(position);
		m_currentQueuedActionsList.Add(queuedAction);
	}

	public InteractiveItem.InteractionContract QueueAction(bool andCancelExisting, InteractiveItem ii, InteractiveItem.Interaction iii)
	{
		if (andCancelExisting)
		{
			CancelActions();
		}
		QueuedAction queuedAction = new QueuedAction();
		InteractiveItem.InteractionContract interactionContract = null;
		interactionContract = ii.SetIntentInteraction(iii, this);
		queuedAction.m_processFunction = ProcessInteractionAction;
		queuedAction.m_canCancelFunction = CanCancelActionStandard;
		queuedAction.m_contract = interactionContract;
		m_currentQueuedActionsList.Add(queuedAction);
		return interactionContract;
	}

	private void UpdateTemperatureOxygen()
	{
		m_currentTemperature = m_bomberSystems.GetTemperatureOxygen().GetCurrentTemperature();
		float currentOxygen = m_bomberSystems.GetTemperatureOxygen().GetCurrentOxygen();
		bool hasOxygen = true;
		if (currentOxygen < 0.35f && (m_bomberSystems.GetOxygenTank().IsBroken() || !m_isAtAStation))
		{
			hasOxygen = false;
		}
		m_lifeStatus.SetStats(m_currentTemperature, hasOxygen);
		if (m_lifeStatus.AffectedByTemperature() && !IsBailedOut())
		{
			if (m_temperatureNotification == null)
			{
				m_temperatureNotification = new CrewmanNotification();
				m_temperatureNotification.m_iconName = "Icon_Temperature";
				RegisterNotification(m_temperatureNotification);
			}
			m_coldTemperatureFlash = m_crewmanGraphics.GetFlashManager().AddOrUpdateFlash(1f, 0.5f, 1f, 255, 1f, new Color(0f, 0.3f, 0.75f), m_coldTemperatureFlash);
		}
		else if (m_temperatureNotification != null)
		{
			RemoveNotification(m_temperatureNotification);
			m_temperatureNotification = null;
		}
		if (m_lifeStatus.AffectedByOxygen() && !IsBailedOut())
		{
			if (m_oxygenNotification == null)
			{
				m_oxygenNotification = new CrewmanNotification();
				m_oxygenNotification.m_iconName = "Icon_Oxygen";
				RegisterNotification(m_oxygenNotification);
			}
		}
		else if (m_oxygenNotification != null)
		{
			RemoveNotification(m_oxygenNotification);
			m_oxygenNotification = null;
		}
	}

	private void DoPhysicsUpdate()
	{
		if (!IsFrozen())
		{
			if (!m_characterController.IsLocked() && !m_isAtAStation)
			{
				if (m_characterController.IsGrounded())
				{
					m_notGroundedTime = 0f;
					float num = Vector3.Dot(base.transform.forward, Vector3.up);
					Vector3 normalized = (base.transform.forward - num * Vector3.up).normalized;
					if (m_currentWalkZone.IsExternal())
					{
						m_characterController.CanBrace(brace: false);
						if (m_currentWalkZone.IsExternal())
						{
							float num2 = 0f - Vector3.Dot(m_currentFloorNormal, normalized);
							m_currentLean = Mathf.Clamp(num2 * 2f, -1f, 1f);
						}
						else
						{
							m_currentLean = 0f;
						}
					}
					else
					{
						if (m_lifeStatus.IsCountingDown())
						{
							m_characterController.TryToCentralise(m_currentWalkZone, m_bomberSystems.GetFloorUp().up, andRotate: true);
						}
						else
						{
							m_characterController.CanBrace(brace: true);
						}
						float num3 = 0f - Vector3.Dot(m_bomberSystems.GetFloorUp().up, normalized);
						m_currentLean = Mathf.Clamp(num3 * 2f, -1f, 1f);
					}
					m_grip += Time.deltaTime;
					m_grip = Mathf.Clamp01(m_grip);
					float num4 = m_grip - m_smoothGrip;
					m_smoothGrip += Mathf.Clamp01(Time.deltaTime * 5f) * num4;
					if (!m_lifeStatus.IsCountingDown())
					{
						m_didFallThisFrame = m_characterController.DidSlip();
					}
				}
				else
				{
					m_characterController.CanBrace(brace: false);
					m_notGroundedTime += Time.deltaTime;
					if (m_notGroundedTime > 0.35f)
					{
						m_didFallThisFrame = true;
						if (!Physics.Raycast(m_crewmanGraphics.GetPelvisTransform().position, Vector3.down, 2f, 1 << LayerMask.NameToLayer("PhysicsInternal")))
						{
							BailOutFromExitPoint(null);
						}
					}
					m_currentLean = 0f;
				}
				m_didBraceThisFrame = m_characterController.IsBracing();
			}
			else if (m_currentStation != null)
			{
				m_characterController.CanBrace(brace: true);
				m_smoothGrip = 1f;
				m_grip = 1f;
				m_currentLean = 0f;
				if (Vector3.Dot(m_bomberSystems.GetFloorUp().up, Vector3.up) < 0.8f)
				{
					m_didBraceThisFrame = true;
				}
			}
			else
			{
				if (!(m_controlledByObject != null))
				{
					return;
				}
				if ((m_controlledByObject.transform.position - base.transform.position).magnitude > 0.01f)
				{
					m_movedThisFrame = true;
				}
				base.transform.SetPositionAndRotation(m_controlledByObject.transform.position, m_controlledByObject.transform.rotation);
				if (!(m_controlledByObject.transform.parent != null) || !m_lifeStatus.IsDead())
				{
					return;
				}
				if (m_currentWalkZone != null && m_bomberSystems.GetBomberState().GetPhysicsModel().IsSimplified())
				{
					BomberWalkZone bomberWalkZone = m_bomberSystems.GetAllWalkZones().FindContainingWalkZone(base.transform.position, ignoreExternal: true);
					if (bomberWalkZone != null && (bomberWalkZone.GetNearestPlanarPoint(base.transform.position) - base.transform.position).magnitude > 1f)
					{
						m_currentWalkZone = null;
					}
				}
				if (m_currentWalkZone == null)
				{
					m_controlledByObject.transform.parent = null;
					m_controlledByObject.GetComponent<Rigidbody>().velocity = m_bomberSystems.GetBomberState().GetVelocity();
				}
			}
		}
		else
		{
			m_currentLean = 0f;
		}
	}

	public bool IsActivityInProgress(out float progress, out string iconType)
	{
		if (m_currentQueuedActionsList.Count > 0 && m_currentQueuedActionsList[0].m_contract != null && m_currentQueuedActionsList[0].m_contract.m_contractState == InteractiveItem.InteractionContract.ContractState.Started)
		{
			progress = m_currentQueuedActionsList[0].m_contract.GetValue();
			iconType = Singleton<EnumToIconMapping>.Instance.GetIconName(m_currentQueuedActionsList[0].m_contract.m_interaction.m_iconType);
			return true;
		}
		progress = 0f;
		iconType = string.Empty;
		return false;
	}

	private void ShowPreviewArrow()
	{
		bool visible = false;
		if (m_isSelected)
		{
			QueuedAction queuedAction = null;
			foreach (QueuedAction currentQueuedActions in m_currentQueuedActionsList)
			{
				if (!currentQueuedActions.m_cancelRequested)
				{
					queuedAction = currentQueuedActions;
				}
			}
			if (queuedAction != null)
			{
				if (queuedAction.m_contract != null)
				{
					if (queuedAction.m_contract.m_item != null)
					{
						m_currentWalkArrow.transform.position = queuedAction.m_contract.m_item.GetMovementArrowTransform().position;
						m_currentWalkArrow.transform.rotation = queuedAction.m_contract.m_item.GetMovementArrowTransform().rotation;
						visible = true;
					}
				}
				else if (queuedAction.m_targetWalkZone != null)
				{
					m_currentWalkArrow.transform.position = queuedAction.m_targetWalkZone.GetPointFromLocal(queuedAction.m_localPlanarPoint);
					m_currentWalkArrow.transform.rotation = Quaternion.identity;
					visible = true;
				}
			}
		}
		m_previewArrowController.SetVisible(visible);
	}

	private void DoFireUpdate()
	{
		if (m_fireDamage > 0f)
		{
			m_fireDamage -= Time.deltaTime;
			if (m_fireDamage < 0f)
			{
				if (m_onFire)
				{
					m_onFire = false;
					m_crewmanGraphics.SetOnFire(onFire: false);
				}
				m_fireDamage = 0f;
			}
			if (m_fireDamage > 5f)
			{
				m_fireDamage = 5f;
				if (!IsBailedOut() && !m_onFire)
				{
					QueuedAction queuedAction = new QueuedAction();
					queuedAction.m_canCancelFunction = CanCancelActionRunningFromFire;
					queuedAction.m_processFunction = ProcessRunFromFireAction;
					m_currentQueuedActionsList.Add(queuedAction);
					m_onFire = true;
					m_crewmanGraphics.SetOnFire(onFire: true);
				}
			}
			if (m_currentlyCarrying != null && m_currentlyCarrying.GetComponent<FireExtinguisher>() != null)
			{
				QueueFireMode(andCancelOthers: true, null);
			}
		}
		m_timeSinceBurnt += Time.deltaTime;
		if (m_onFire && !IsBailedOut())
		{
			DamageSource damageSource = new DamageSource();
			damageSource.m_damageShapeEffect = DamageSource.DamageShape.None;
			damageSource.m_damageType = DamageSource.DamageType.SelfFire;
			damageSource.m_position = base.transform.position;
			damageSource.m_radius = 0f;
			DamageGetPassthrough(m_fireDamage * Time.deltaTime, damageSource);
		}
	}

	private void DoBailOutOrderCheck()
	{
		bool flag = m_bomberSystems.GetBomberState().IsBailOutOrderActive();
		if (flag == m_hasSeenBailOutOrder)
		{
			return;
		}
		if (flag && (!m_isAtAStation || !(m_currentStation is StationPilot)))
		{
			List<InteractiveItem> searchableInteractives = m_bomberSystems.GetSearchableInteractives(typeof(Parachute));
			InteractiveItem interactiveItem = null;
			if (m_currentlyCarrying != null && m_currentlyCarrying.GetComponent<Parachute>() != null)
			{
				interactiveItem = m_currentlyCarrying;
			}
			else
			{
				float num = float.MaxValue;
				foreach (InteractiveItem item in searchableInteractives)
				{
					if (!item.IsAnyInteractionInProgress() && item.GetInteractionOptionsPublic(this, skipNullCheck: true) != null)
					{
						float magnitude = (item.transform.position - base.transform.position).magnitude;
						if (magnitude < num)
						{
							interactiveItem = item;
						}
					}
				}
			}
			if (interactiveItem != null)
			{
				CancelActions();
				QueuedAction queuedAction = new QueuedAction();
				if (interactiveItem != m_currentlyCarrying)
				{
					queuedAction.m_contract = interactiveItem.SetIntentInteraction(interactiveItem.GetInteractionOptionsPublic(this, skipNullCheck: true), this);
				}
				else
				{
					List<InteractiveItem> searchableInteractives2 = m_bomberSystems.GetSearchableInteractives(typeof(ExitPoint));
					float num2 = float.MaxValue;
					InteractiveItem interactiveItem2 = null;
					foreach (InteractiveItem item2 in searchableInteractives2)
					{
						if (item2.GetInteractionOptionsPublic(this, skipNullCheck: true) != null)
						{
							float magnitude2 = (item2.transform.position - base.transform.position).magnitude;
							if (magnitude2 < num2)
							{
								num2 = magnitude2;
								interactiveItem2 = item2;
							}
						}
					}
					queuedAction.m_contract = interactiveItem2.SetIntentInteraction(interactiveItem2.GetInteractionOptionsPublic(this, skipNullCheck: true), this);
				}
				queuedAction.m_canCancelFunction = CanCancelActionStandard;
				queuedAction.m_processFunction = ProcessDoAutoBailOut;
				Singleton<SpeechPrioritiser>.Instance.RequestNewSpeech(StaticSpeechSets.BailingOut, this);
				m_currentQueuedActionsList.Add(queuedAction);
			}
		}
		m_hasSeenBailOutOrder = flag;
	}

	public bool CanDoActions()
	{
		return !m_didBraceThisFrame && !m_didFallThisFrame && !m_lifeStatus.IsDead() && !m_lifeStatus.IsCountingDown() && !m_isBailedOut;
	}

	private void FixedUpdate()
	{
		m_movedThisFrame = false;
		m_didFireFightThisFrame = false;
		m_didInteractThisFrame = false;
		m_didHealThisFrame = false;
		m_didEngineRepairThisFrame = false;
		m_didFuelTankRepairThisFrame = false;
		m_didFallThisFrame = false;
		m_characterController.DoUpdate(m_bomberSystems.GetFloorUp().up, m_currentWalkZone);
		if (m_characterController.DidShift())
		{
			SetFinalPosition();
		}
		if (m_lifeStatus.IsDead() || m_lifeStatus.IsCountingDown())
		{
			m_notDownBumpY.localPosition = Vector3.zero;
			CancelActions();
		}
		else
		{
			m_notDownBumpY.localPosition = new Vector3(0f, m_notDownBumpAmount, 0f);
		}
		while (m_currentQueuedActionsList.Count > 0)
		{
			if (m_currentQueuedActionsList[0].m_cancelRequested && m_currentQueuedActionsList[0].m_canCancelFunction(m_currentQueuedActionsList[0]))
			{
				AbandonQueuedAction(m_currentQueuedActionsList[0]);
				m_currentQueuedActionsList.RemoveAt(0);
				continue;
			}
			QueuedAction item = m_currentQueuedActionsList[0];
			if (ProcessCurrentAction())
			{
				m_currentQueuedActionsList.Remove(item);
			}
			break;
		}
		if (m_currentQueuedActionsList.Count == 0 && m_finalPoint != null && !m_isAtAStation && !m_lifeStatus.IsDead() && !m_lifeStatus.IsCountingDown() && !m_isBailedOut)
		{
			if (m_currentWalkZone != m_finalPoint.m_zone)
			{
				m_cachedWalkPath = m_currentWalkZone.GetFullWalkPath(m_finalPoint.m_zone, m_finalPoint.m_zone.GetPointFromLocal(m_finalPoint.m_zoneLocalPosition), m_cachedWalkPath);
				WalkToTarget(m_cachedWalkPath, movingAsLastPointStay: true);
			}
			else
			{
				m_cachedWalkPath.m_generatedFromWalkZone = null;
				m_cachedWalkPath.m_path.Clear();
				BomberWalkZone.WalkTarget walkTarget = new BomberWalkZone.WalkTarget();
				walkTarget.m_zone = m_finalPoint.m_zone;
				walkTarget.m_zoneLocalPosition = m_finalPoint.m_zoneLocalPosition;
				m_cachedWalkPath.m_path.Add(walkTarget);
				WalkToTarget(m_cachedWalkPath, movingAsLastPointStay: true);
			}
			m_characterController.SetCanShift();
		}
		DoFireUpdate();
		ShowPreviewArrow();
		m_didBraceThisFrame = false;
		DoPhysicsUpdate();
		UpdateTemperatureOxygen();
		DoBailOutOrderCheck();
		UpdateColliderPositions();
		if (!m_lifeStatus.IsDead() && m_lifeStatus.GetTotalHealthN() < 0.5f)
		{
			m_crewmanGraphics.GetComponent<DamageFlash>().DoLowHealth(m_lifeStatus.GetTotalHealthN() < 0.25f);
		}
		else
		{
			m_crewmanGraphics.GetComponent<DamageFlash>().ReturnToNormal();
		}
		if (m_isSelected && !IsSelectable())
		{
			Singleton<ContextControl>.Instance.SelectCrewman(null);
		}
		m_interactableItem.GetComponent<Collider>().enabled = !m_isSelected && IsSelectable();
		if (m_doingWalkAnimation)
		{
			if (m_nextWalkZoneFromWalkAnimation != null)
			{
				m_crewmanGraphics.SetIsCrawl(m_nextWalkZoneFromWalkAnimation.IsExternal());
			}
			else
			{
				m_crewmanGraphics.SetIsCrawl(m_currentWalkZone != null && m_currentWalkZone.IsExternal());
			}
		}
		else
		{
			m_crewmanGraphics.SetIsCrawl(m_currentWalkZone != null && m_currentWalkZone.IsExternal());
		}
		m_crewmanGraphics.SetIsMoving(m_movedThisFrame);
		if (m_movedThisFrame)
		{
			m_crewmanGraphics.SetMoveSpeed(m_movementSpeedFactor * m_walkSpeed);
		}
		m_crewmanGraphics.SetIsFireFighting(m_didFireFightThisFrame);
		m_crewmanGraphics.SetIsInteracting(m_didInteractThisFrame, m_didHealThisFrame, m_didEngineRepairThisFrame, m_didFuelTankRepairThisFrame);
		m_crewmanGraphics.SetInteractionSpeed(m_interactionSpeed);
		m_crewmanGraphics.SetIsFalling(m_didFallThisFrame);
		m_crewmanGraphics.SetIsBrace(m_didBraceThisFrame);
		if (m_currentlyCarrying != null && m_currentlyCarrying.GetComponent<FireExtinguisher>() != null)
		{
			m_currentlyCarrying.GetComponent<FireExtinguisher>().DoExtinguish(m_didFireFightThisFrame, m_crewman);
		}
		if (m_currentStation != null)
		{
			if (m_currentStation is StationGunner)
			{
				m_crewmanGraphics.SetIsFiring(((StationGunner)m_currentStation).IsFiring());
			}
			else
			{
				m_crewmanGraphics.SetIsFiring(firing: false);
			}
		}
		else
		{
			m_crewmanGraphics.SetIsFiring(firing: false);
		}
		m_crewmanGraphics.DoExtinguisherEffects(m_didFireFightThisFrame);
		float num = m_leanTargetThisFrame - m_currentLean;
		m_currentLean += num * Mathf.Clamp01(Time.deltaTime * 10f);
		m_crewmanGraphics.SetCurrentLean(m_currentLean);
	}

	private void LateUpdate()
	{
		if (m_controlledByObject != null)
		{
			if ((m_controlledByObject.transform.position - base.transform.position).magnitude > 0.01f)
			{
				m_movedThisFrame = true;
			}
			base.transform.SetPositionAndRotation(m_controlledByObject.transform.position, m_controlledByObject.transform.rotation);
		}
	}

	private void UpdateColliderPositions()
	{
		if (m_isAtAStation)
		{
			if (m_currentStation.GetStationAnimationAction() == Station.StationAction.FoetalPosition)
			{
				Transform pelvisTransform = m_crewmanGraphics.GetPelvisTransform();
				m_transformToFollowGraphics.SetPositionAndRotation(pelvisTransform.position, pelvisTransform.rotation * Quaternion.AngleAxis(-60f, Vector3.forward));
				m_transformToFollowGraphicsMO.position = pelvisTransform.position;
			}
			else
			{
				Transform pelvisTransform2 = m_crewmanGraphics.GetPelvisTransform();
				m_transformToFollowGraphics.SetPositionAndRotation(pelvisTransform2.position, pelvisTransform2.rotation);
				m_transformToFollowGraphicsMO.position = pelvisTransform2.position;
			}
		}
		else
		{
			Transform pelvisTransform3 = m_crewmanGraphics.GetPelvisTransform();
			m_transformToFollowGraphics.SetPositionAndRotation(pelvisTransform3.position, pelvisTransform3.rotation);
			m_transformToFollowGraphicsMO.position = pelvisTransform3.position;
		}
	}

	private void SetFinalPosition()
	{
		m_finalPoint = new BomberWalkZone.WalkTarget();
		m_finalPoint.m_zone = m_currentWalkZone;
		m_finalPoint.m_zoneLocalPosition = m_currentWalkZone.GetNearestPlanarPointLocalFromWorld(base.transform.position);
	}

	private bool WalkToTarget(BomberWalkZone.WalkPath walkPath, bool movingAsLastPointStay)
	{
		if (!m_doingWalkAnimation && m_currentStation != null)
		{
			if (!m_isExitingStationCurrently)
			{
				SetStationSlow(null);
			}
			return true;
		}
		bool flag = true;
		if (!m_doingWalkAnimation && !m_didBraceThisFrame && !m_characterController.IsLocked())
		{
			if (walkPath != null && walkPath.m_path.Count > 0)
			{
				Vector3 pointFromLocal = walkPath.m_path[0].m_zone.GetPointFromLocal(walkPath.m_path[0].m_zoneLocalPosition);
				Vector3 vector = pointFromLocal - base.transform.position;
				vector.y = 0f;
				if (vector.magnitude < 0.25f)
				{
					if (walkPath.m_path[0].m_connector != null)
					{
						HatchDoor hatchToActivate = walkPath.m_path[0].m_connector.GetHatchToActivate();
						if (hatchToActivate != null)
						{
							hatchToActivate.RequestOpen(4f);
						}
						if (walkPath.m_path[0].m_connector.RequiresWarp())
						{
							string animationTriggerForConnection = walkPath.m_path[0].m_connector.GetAnimationTriggerForConnection();
							if (string.IsNullOrEmpty(animationTriggerForConnection))
							{
								m_currentWalkZone = walkPath.m_path[0].m_connector.GetTargetConnector().GetThisWalkZone();
								if (m_currentWalkZone != null)
								{
								}
								base.transform.position = walkPath.m_path[0].m_connector.GetTargetConnector().transform.position;
							}
							else
							{
								m_doingWalkAnimation = true;
								m_characterController.SetLocked(locked: true);
								base.transform.SetPositionAndRotation(pointFromLocal, walkPath.m_path[0].m_connector.transform.rotation);
								BomberWalkZoneConnector targetConnector = walkPath.m_path[0].m_connector.GetTargetConnector();
								m_nextWalkZoneFromWalkAnimation = targetConnector.GetThisWalkZone();
								m_walkAnimationTriggerName = animationTriggerForConnection;
								m_crewmanGraphics.TriggerAnimation(animationTriggerForConnection, delegate
								{
									if (base.gameObject != null)
									{
										m_characterController.SetLocked(locked: false);
										if (targetConnector != null)
										{
											m_currentWalkZone = targetConnector.GetThisWalkZone();
											if (m_currentWalkZone != null)
											{
											}
											Quaternion quaternion = new Quaternion(targetConnector.transform.rotation.x, targetConnector.transform.rotation.y, targetConnector.transform.rotation.z, targetConnector.transform.rotation.w);
											base.transform.SetPositionAndRotation(targetConnector.transform.position, quaternion *= Quaternion.Euler(0f, 180f, 0f));
										}
										m_doingWalkAnimation = false;
									}
								});
							}
						}
						else
						{
							m_currentWalkZone = walkPath.m_path[0].m_connector.GetTargetConnector().GetThisWalkZone();
							if (!(m_currentWalkZone != null))
							{
							}
						}
					}
					if (!movingAsLastPointStay)
					{
						m_finalPoint = walkPath.m_path[0];
					}
					walkPath.m_path.RemoveAt(0);
					if (walkPath.m_path.Count == 0)
					{
						flag = false;
					}
				}
				else
				{
					float num = m_walkSpeed * m_movementSpeedFactor;
					Vector3 moveDelta = ((!(vector.magnitude < num * Time.deltaTime)) ? (vector.normalized * num * Time.deltaTime) : vector);
					m_characterController.DoMove(moveDelta, isAWalk: true, isDirectionWalk: true, m_bomberSystems.GetFloorUp().up);
				}
			}
			else
			{
				flag = false;
			}
		}
		m_movedThisFrame = flag;
		return flag;
	}

	public bool IsBailedOut()
	{
		return m_isBailedOut;
	}

	public bool IsBailedOutNoParachute()
	{
		if (m_controlledByObject != null)
		{
			return m_isBailedOut && !m_controlledByObject.GetComponent<CrewmanBailedOutController>().HasParachute();
		}
		return false;
	}

	private void OnControllerColliderHit(ControllerColliderHit cch)
	{
		if (Vector3.Dot(cch.normal, Vector3.up) > 0.3f)
		{
			m_currentFloorNormal = cch.normal;
			m_notGroundedTime = 0f;
		}
	}

	public void QueueFireMode(bool andCancelOthers, FireOverview.Extinguishable fireArea)
	{
		if (andCancelOthers)
		{
			CancelActions();
		}
		QueuedAction queuedAction = new QueuedAction();
		queuedAction.m_canCancelFunction = CanCancelActionStandard;
		queuedAction.m_currentFireArea = fireArea;
		queuedAction.m_processFunction = ProcessFireFightAction;
		m_currentQueuedActionsList.Add(queuedAction);
	}

	private void CrewmanColliderOnClick()
	{
		Singleton<ContextControl>.Instance.ClickOnCrewman(this);
	}

	private void CrewmanColliderOnHoverOut()
	{
		if (!m_isSelected)
		{
			m_crewmanGraphics.GetOutlineFlash().RemoveFlash(m_highlightFlash);
			m_isOutlined = false;
		}
	}

	public bool IsCarryingItem()
	{
		return m_currentlyCarrying != null;
	}

	public CarryableItem GetCarriedItem()
	{
		return m_currentlyCarrying;
	}

	private void CrewmanColliderOnHoverOver()
	{
		if (!m_isSelected)
		{
			m_highlightFlash = m_crewmanGraphics.GetOutlineFlash().AddOrUpdateFlash(0f, 0f, 0f, 1, 1f, Color.white, m_highlightFlash);
			m_isOutlined = true;
		}
	}

	public void HoverOnPanel()
	{
		if (IsSelectable())
		{
			CrewmanColliderOnHoverOver();
		}
	}

	public void HoverOffPanel()
	{
		CrewmanColliderOnHoverOut();
	}

	public bool IsOutlined()
	{
		return m_isOutlined;
	}

	public bool RequiresHealing()
	{
		return m_lifeStatus.IsCountingDown();
	}

	public void Heal(Crewman healedBy)
	{
		m_lifeStatus.Heal(healedBy);
		Singleton<SpeechPrioritiser>.Instance.RequestNewSpeech(StaticSpeechSets.InjuriesHealed, this);
	}

	public void HealFractional(Crewman healedBy, float fraction)
	{
		m_lifeStatus.HealFractional(healedBy, fraction);
	}

	public CrewmanLifeStatus GetHealthState()
	{
		return m_lifeStatus;
	}

	public override bool IsDamageBlocker()
	{
		return false;
	}

	public void InstantKill()
	{
		m_lifeStatus.InstantKill();
	}

	public override float DamageGetPassthrough(float amt, DamageSource dt)
	{
		if (!m_invincible)
		{
			m_lifeStatus.DoDamage(amt * m_damageMultiplier);
		}
		m_crewmanGraphics.GetDamageFlash().DoFlash();
		if (!m_lifeStatus.IsCountingDown())
		{
			m_crewmanGraphics.FacialAnimController.Grimace(0.25f);
		}
		if (dt.m_damageType == DamageSource.DamageType.Fire)
		{
			m_fireDamage += amt;
			m_timeSinceBurnt = 0f;
		}
		if (dt.m_damageType == DamageSource.DamageType.Impact)
		{
			Singleton<ControllerRumble>.Instance.GetRumbleMixerHit().Kick();
		}
		return 0f;
	}

	public void SetNullWalkZone()
	{
		m_currentWalkZone = null;
		BailOutFromExitPoint(null);
	}

	public void BailOutFromExitPoint(ExitPoint exitPoint)
	{
		if (m_lifeStatus.IsDead())
		{
			if (!m_isBailedOut && m_controlledByObject != null)
			{
				m_controlledByObject.transform.parent = null;
				m_controlledByObject.GetComponent<Rigidbody>().velocity = m_bomberSystems.GetBomberState().GetVelocity();
			}
		}
		else if (!m_isBailedOut)
		{
			CancelActions();
			QueuedAction queuedAction = new QueuedAction();
			queuedAction.m_bailOutExitPoint = exitPoint;
			queuedAction.m_canCancelFunction = (QueuedAction q) => m_lifeStatus.IsDead() ? true : false;
			queuedAction.m_processFunction = ProcessBailedOut;
			m_currentQueuedActionsList.Add(queuedAction);
		}
	}
}
