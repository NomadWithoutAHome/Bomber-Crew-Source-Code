using System.Collections.Generic;
using Rewired;
using UnityEngine;

public class UISelectorMovementTypeFree : UISelectorMovementType
{
	private tk2dUIItem m_currentlyPointedAtItem;

	private tk2dUIItem m_currentMagnet;

	private Vector3 m_currentScreenSpacePosition;

	private Vector3 m_currentMagnetOffset;

	[SerializeField]
	private float m_moveSpeedMin = 300f;

	[SerializeField]
	private float m_moveSpeedMinSwitch = 300f;

	[SerializeField]
	private float m_moveSpeedMax = 300f;

	[SerializeField]
	private float m_moveSpeedAcceleration = 150f;

	[SerializeField]
	private float m_largestMagnetDistance = 80f;

	private float m_currentMoveSpeed = 300f;

	private bool m_hasMovedSinceForced;

	private GameObject m_forcePointed;

	private int m_forcePointedLayer;

	private UISelectFinder m_currentFinder;

	private Vector3 m_previousDigital;

	private bool m_hasMovedSinceLastMagnet;

	private bool m_didSnap;

	private float m_scrollerHint;

	private void Start()
	{
		m_currentScreenSpacePosition = new Vector3(300f, 300f, 0f);
	}

	public Vector3 GetCurrentScreenSpacePositionExtended()
	{
		return m_currentScreenSpacePosition + m_currentMagnetOffset;
	}

	public void SetScreenSpacePositionIncludingMagnet(Vector3 magnetEnd)
	{
		m_currentMagnetOffset = Vector3.zero;
		m_currentScreenSpacePosition = magnetEnd;
	}

	public Vector3 GetCurrentScreenSpacePositionForDisplay()
	{
		return m_currentScreenSpacePosition;
	}

	public void SetCameraZoomHappened()
	{
		if (m_currentlyPointedAtItem != null)
		{
			ForcePointAt(m_currentlyPointedAtItem);
		}
		else if (m_currentMagnet != null)
		{
			ForcePointAt(m_currentMagnet);
		}
	}

	public Vector3 GetRoughWorldPosition(Camera usingCamera, float roughDist)
	{
		if (m_forcePointed != null && !m_hasMovedSinceForced)
		{
			return m_forcePointed.transform.position;
		}
		if (m_currentlyPointedAtItem != null)
		{
			return m_currentlyPointedAtItem.transform.position;
		}
		if (m_currentMagnet != null)
		{
			return m_currentMagnet.transform.position;
		}
		Ray ray = usingCamera.ScreenPointToRay(m_currentScreenSpacePosition);
		Vector3 normalized = ray.direction.normalized;
		float num = Vector3.Dot(normalized, usingCamera.transform.forward);
		if (num > 0f)
		{
			Vector3 vector = normalized / num;
			return ray.origin + vector * roughDist;
		}
		return ray.origin + normalized * roughDist;
	}

	public override void SetUp(UISelectFinder finder)
	{
		m_previousDigital = Vector3.zero;
		m_currentFinder = finder;
		if (m_currentlyPointedAtItem != null)
		{
			m_currentlyPointedAtItem.HoverOut(null);
		}
		m_currentlyPointedAtItem = null;
	}

	public override void DeSelect()
	{
		if (m_currentlyPointedAtItem != null)
		{
			m_currentlyPointedAtItem.HoverOut(null);
		}
		m_currentlyPointedAtItem = null;
	}

	public override void ForcePointAt(tk2dUIItem target)
	{
		if (m_currentlyPointedAtItem != null)
		{
			m_currentlyPointedAtItem.HoverOut(null);
		}
		target.HoverOver(m_currentlyPointedAtItem);
		m_currentlyPointedAtItem = target;
		if (m_currentlyPointedAtItem != null)
		{
			tk2dCamera tk2dCamera2 = tk2dCamera.CameraForLayer(m_currentlyPointedAtItem.gameObject.layer);
			if (tk2dCamera2 != null)
			{
				Vector3 currentScreenSpacePosition = tk2dCamera2.ScreenCamera.WorldToScreenPoint(m_currentlyPointedAtItem.transform.position);
				currentScreenSpacePosition.z = 0f;
				m_currentScreenSpacePosition = currentScreenSpacePosition;
				m_forcePointed = m_currentlyPointedAtItem.gameObject;
				m_forcePointedLayer = m_currentlyPointedAtItem.gameObject.layer;
				m_hasMovedSinceForced = false;
				m_hasMovedSinceLastMagnet = true;
				m_currentMagnetOffset = Vector3.zero;
			}
		}
	}

	public override void ForcePointAt(GameObject go, int cameraLayer)
	{
		tk2dCamera tk2dCamera2 = tk2dCamera.CameraForLayer(cameraLayer);
		if (tk2dCamera2 != null)
		{
			Vector3 currentScreenSpacePosition = tk2dCamera2.ScreenCamera.WorldToScreenPoint(go.transform.position);
			currentScreenSpacePosition.z = 0f;
			m_currentScreenSpacePosition = currentScreenSpacePosition;
			m_hasMovedSinceForced = false;
			m_hasMovedSinceLastMagnet = true;
			m_forcePointed = go;
			m_forcePointedLayer = cameraLayer;
			m_currentMagnetOffset = Vector3.zero;
		}
	}

	public float GetScrollHint()
	{
		return m_scrollerHint;
	}

	public override void DoMovement(Vector2 absMove, Vector2 tickMove)
	{
		float num = (float)Screen.width / 1280f;
		Vector3 vector = ReInput.players.GetPlayer(0).GetAxis2D(43, 41);
		Vector3 vector2 = ReInput.players.GetPlayer(0).GetAxis2D(42, 40);
		float num2 = ((Time.timeScale != 0f) ? (Time.deltaTime / Time.timeScale) : Mathf.Clamp(Time.unscaledDeltaTime, 0f, 0.1f));
		m_currentScreenSpacePosition += vector2 * num2 * m_currentMoveSpeed * num;
		m_didSnap = false;
		m_scrollerHint = 0f;
		bool flag = false;
		if (vector2.magnitude > 0f)
		{
			flag = true;
			if (m_currentMagnetOffset.magnitude > 0f && Vector3.Dot(vector2, m_currentMagnetOffset) > 0f)
			{
				m_currentMagnetOffset -= vector2 * num2 * m_currentMoveSpeed;
				if (Vector3.Dot(vector2, m_currentMagnetOffset) < 0f)
				{
					m_currentMagnetOffset = Vector3.zero;
				}
			}
			else
			{
				m_currentMagnetOffset -= m_currentMagnetOffset * vector2.magnitude * Mathf.Clamp01(num2 * 25f);
				if (m_currentMagnetOffset.magnitude < 0.1f)
				{
					m_currentMagnetOffset = Vector3.zero;
				}
			}
			if (m_currentScreenSpacePosition.x > (float)Screen.width)
			{
				m_currentScreenSpacePosition.x = Screen.width;
			}
			if (m_currentScreenSpacePosition.y > (float)Screen.height)
			{
				m_currentScreenSpacePosition.y = Screen.height;
			}
			if (m_currentScreenSpacePosition.x < 0f)
			{
				m_currentScreenSpacePosition.x = 0f;
			}
			if (m_currentScreenSpacePosition.y < 0f)
			{
				m_currentScreenSpacePosition.y = 0f;
			}
			float num3 = m_currentScreenSpacePosition.x / (float)Screen.width;
			if (vector2.x < 0f && num3 < 0.3f)
			{
				m_scrollerHint = vector2.x;
			}
			if (vector2.x > 0f && num3 > 0.7f)
			{
				m_scrollerHint = vector2.x;
			}
			m_currentMoveSpeed -= m_moveSpeedAcceleration * Time.deltaTime * 4f;
		}
		if (vector.magnitude > 0.2f && (m_previousDigital.magnitude == 0f || Vector3.Dot(vector.normalized, m_previousDigital.normalized) < 0.1f))
		{
			m_currentMagnetOffset = Vector3.zero;
			m_currentMoveSpeed = 0f;
			m_currentMagnet = null;
			flag = SnapToNearest(vector);
		}
		m_previousDigital = vector;
		m_currentMoveSpeed += (vector2.magnitude - 0.5f) * 2f * m_moveSpeedAcceleration * num2;
		m_currentMoveSpeed = Mathf.Clamp(m_currentMoveSpeed, m_moveSpeedMin, m_moveSpeedMax);
		if (flag)
		{
			m_hasMovedSinceLastMagnet = true;
			m_hasMovedSinceForced = true;
		}
		if (!m_hasMovedSinceForced)
		{
			tk2dCamera tk2dCamera2 = tk2dCamera.CameraForLayer(m_forcePointedLayer);
			if (tk2dCamera2 != null && m_forcePointed != null)
			{
				Vector3 screenSpacePositionIncludingMagnet = tk2dCamera2.ScreenCamera.WorldToScreenPoint(m_forcePointed.transform.position);
				screenSpacePositionIncludingMagnet.z = 0f;
				SetScreenSpacePositionIncludingMagnet(screenSpacePositionIncludingMagnet);
			}
		}
	}

	private bool SnapToNearest(Vector3 tickMove)
	{
		tk2dUIItem[] allItems = m_currentFinder.GetAllItems();
		float num = ((!(m_currentlyPointedAtItem == null)) ? 0.1f : (-1.1f));
		float num2 = float.MaxValue;
		tk2dUIItem tk2dUIItem2 = null;
		Vector3 currentScreenSpacePosition = m_currentScreenSpacePosition;
		tk2dUIItem[] array = allItems;
		foreach (tk2dUIItem tk2dUIItem3 in array)
		{
			if (!(tk2dUIItem3 != m_currentlyPointedAtItem) || !IsValid(tk2dUIItem3) || !(tk2dUIItem3.GetComponent<WalkableArea>() == null))
			{
				continue;
			}
			tk2dCamera tk2dCamera2 = tk2dCamera.CameraForLayer(tk2dUIItem3.gameObject.layer);
			if (tk2dCamera2 != null)
			{
				Vector3 vector = tk2dCamera2.ScreenCamera.WorldToScreenPoint(tk2dUIItem3.transform.position);
				vector.z = 0f;
				Vector3 vector2 = vector - m_currentScreenSpacePosition;
				if (Vector3.Dot(vector2.normalized, tickMove) > num && vector2.magnitude < num2)
				{
					currentScreenSpacePosition = vector;
					num2 = vector2.magnitude;
					num = Mathf.Clamp(Vector3.Dot(vector2.normalized, tickMove), -1.1f, 0.3f);
					tk2dUIItem2 = tk2dUIItem3;
				}
			}
			else
			{
				Vector3 vector3 = Camera.main.WorldToScreenPoint(tk2dUIItem3.transform.position);
				Vector3 vector4 = vector3 - m_currentScreenSpacePosition;
				if (Vector3.Dot(vector4.normalized, tickMove) > num && vector4.magnitude < num2)
				{
					currentScreenSpacePosition = vector3;
					num2 = vector4.magnitude;
					tk2dUIItem2 = tk2dUIItem3;
				}
			}
		}
		if (tk2dUIItem2 != null)
		{
			if (m_currentlyPointedAtItem != null)
			{
				m_currentlyPointedAtItem.HoverOut(null);
			}
			tk2dUIItem2.HoverOver(m_currentlyPointedAtItem);
			m_currentlyPointedAtItem = tk2dUIItem2;
			m_currentScreenSpacePosition = currentScreenSpacePosition;
			m_currentMagnetOffset = Vector3.zero;
			m_didSnap = true;
			return true;
		}
		return false;
	}

	public bool DidSnap()
	{
		return m_didSnap;
	}

	public override void UpdateLogic()
	{
		float num = ((Time.timeScale != 0f) ? (Time.deltaTime / Time.timeScale) : Mathf.Clamp(Time.unscaledDeltaTime, 0f, 0.1f));
		tk2dUIItem[] allItems = m_currentFinder.GetAllItems();
		float num2 = m_largestMagnetDistance;
		tk2dUIItem currentMagnet = null;
		HashSet<int> hashSet = new HashSet<int>();
		tk2dUIItem[] array = allItems;
		foreach (tk2dUIItem tk2dUIItem2 in array)
		{
			if (IsValid(tk2dUIItem2))
			{
				hashSet.Add(tk2dUIItem2.gameObject.layer);
			}
		}
		if (m_hasMovedSinceLastMagnet)
		{
			m_hasMovedSinceLastMagnet = false;
			tk2dUIItem[] array2 = allItems;
			foreach (tk2dUIItem tk2dUIItem3 in array2)
			{
				if (!IsValid(tk2dUIItem3))
				{
					continue;
				}
				tk2dCamera tk2dCamera2 = tk2dCamera.CameraForLayer(tk2dUIItem3.gameObject.layer);
				if (tk2dCamera2 != null)
				{
					Vector3 vector = tk2dCamera2.ScreenCamera.WorldToScreenPoint(tk2dUIItem3.transform.position);
					vector.z = 0f;
					Vector3 vector2 = vector - m_currentScreenSpacePosition;
					if (vector2.magnitude < num2 && (tk2dUIItem3.GetComponent<UISelectorPointingHint>() == null || tk2dUIItem3.GetComponent<UISelectorPointingHint>().Magnet()))
					{
						num2 = vector2.magnitude;
						currentMagnet = tk2dUIItem3;
					}
				}
			}
		}
		else
		{
			currentMagnet = m_currentMagnet;
		}
		tk2dUIItem tk2dUIItem4 = null;
		float num3 = float.MaxValue;
		foreach (int item in hashSet)
		{
			tk2dCamera tk2dCamera3 = tk2dCamera.CameraForLayer(item);
			if (!(tk2dCamera3 != null))
			{
				continue;
			}
			Ray ray = tk2dCamera3.ScreenCamera.ScreenPointToRay(GetCurrentScreenSpacePositionExtended());
			RaycastHit[] array3 = Physics.RaycastAll(ray, float.MaxValue, 1 << item);
			RaycastHit[] array4 = array3;
			for (int k = 0; k < array4.Length; k++)
			{
				RaycastHit raycastHit = array4[k];
				tk2dUIItem component = raycastHit.collider.GetComponent<tk2dUIItem>();
				if ((!(raycastHit.distance < num3) && !(component == m_currentMagnet)) || !(component != null) || !IsValid(component))
				{
					continue;
				}
				tk2dUIItem[] array5 = allItems;
				foreach (tk2dUIItem tk2dUIItem5 in array5)
				{
					if (tk2dUIItem5 == component)
					{
						tk2dUIItem4 = component;
						num3 = raycastHit.distance;
						if (component == m_currentMagnet)
						{
							num3 = 0f;
						}
					}
				}
			}
		}
		if (tk2dUIItem4 != null)
		{
			if (tk2dUIItem4 != m_currentlyPointedAtItem)
			{
				if (m_currentlyPointedAtItem != null)
				{
					m_currentlyPointedAtItem.HoverOut(null);
				}
				tk2dUIItem4.HoverOver(m_currentlyPointedAtItem);
				m_currentlyPointedAtItem = tk2dUIItem4;
			}
		}
		else
		{
			if (m_currentlyPointedAtItem != null)
			{
				m_currentlyPointedAtItem.HoverOut(null);
			}
			m_currentlyPointedAtItem = null;
		}
		m_currentMagnet = currentMagnet;
		if (m_currentlyPointedAtItem != null && !IsValid(m_currentlyPointedAtItem))
		{
			m_currentlyPointedAtItem.HoverOut(null);
			m_currentlyPointedAtItem = null;
		}
		if (!(m_currentMagnet != null))
		{
			return;
		}
		if (IsValid(m_currentMagnet))
		{
			tk2dCamera tk2dCamera4 = tk2dCamera.CameraForLayer(m_currentMagnet.gameObject.layer);
			Vector3 vector3 = tk2dCamera4.ScreenCamera.WorldToScreenPoint(m_currentMagnet.transform.position);
			Vector3 currentMagnetOffset = vector3 - m_currentScreenSpacePosition;
			if (currentMagnetOffset.magnitude > 0f)
			{
				m_currentMagnetOffset = currentMagnetOffset;
			}
		}
		else
		{
			m_currentMagnetOffset = Vector3.zero;
			m_currentMagnet = null;
		}
	}

	public override tk2dUIItem GetCurrentlyPointedAtItem()
	{
		return m_currentlyPointedAtItem;
	}

	public override Vector2 GetCurrentScreenSpacePointerPosition()
	{
		return m_currentScreenSpacePosition;
	}

	public override bool UseScreenSpacePointerPosition()
	{
		return true;
	}
}
