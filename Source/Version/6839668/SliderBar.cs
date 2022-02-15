using BomberCrewCommon;
using UnityEngine;

public class SliderBar : MonoBehaviour
{
	[SerializeField]
	private tk2dUIItem m_uiItem;

	[SerializeField]
	private Transform m_left;

	[SerializeField]
	private Transform m_right;

	[SerializeField]
	private tk2dUIProgressBar m_progressBar;

	[SerializeField]
	private UISelectFinder m_uiFinder;

	private bool m_isDragged;

	private UISelectFinder m_previousFinder;

	private bool m_isUIFinderSelected;

	private bool m_hasDown;

	private void OnEnable()
	{
		m_uiItem.OnDown += OnDragStart;
		m_uiItem.OnRelease += OnDragEnd;
		m_uiItem.OnClick += SetSelected;
		Singleton<MainActionButtonMonitor>.Instance.AddListener(BackListener);
	}

	private void OnDisable()
	{
		Singleton<MainActionButtonMonitor>.Instance.RemoveListener(BackListener, invalidateCurrentPress: false);
	}

	private void SetSelected()
	{
		if (Singleton<UISelector>.Instance.IsPrimary() && !m_isUIFinderSelected)
		{
			m_isUIFinderSelected = true;
			m_previousFinder = Singleton<UISelector>.Instance.GetCurrentFinder();
			Singleton<UISelector>.Instance.SetFinder(m_uiFinder);
		}
	}

	private bool BackListener(MainActionButtonMonitor.ButtonPress bp)
	{
		if (m_isUIFinderSelected)
		{
			if (bp == MainActionButtonMonitor.ButtonPress.BackDown || bp == MainActionButtonMonitor.ButtonPress.ConfirmDown)
			{
				m_hasDown = true;
			}
			if (bp == MainActionButtonMonitor.ButtonPress.Back && m_hasDown)
			{
				Singleton<UISelector>.Instance.SetFinder(m_previousFinder);
				Singleton<UISelector>.Instance.ForcePointAt(m_uiItem);
				m_isUIFinderSelected = false;
				m_hasDown = false;
				return true;
			}
			if (bp == MainActionButtonMonitor.ButtonPress.Confirm && m_hasDown)
			{
				Singleton<UISelector>.Instance.SetFinder(m_previousFinder);
				Singleton<UISelector>.Instance.ForcePointAt(m_uiItem);
				m_isUIFinderSelected = false;
				m_hasDown = false;
				return true;
			}
		}
		return false;
	}

	private void OnDragStart()
	{
		if (!Singleton<UISelector>.Instance.IsPrimary())
		{
			m_isDragged = true;
		}
	}

	private void OnDragEnd()
	{
		m_isDragged = false;
	}

	public void SetValue(float value)
	{
		m_progressBar.Value = value;
	}

	public float GetValue()
	{
		return m_progressBar.Value;
	}

	public Vector3 GetScreenPosThumb()
	{
		Vector3 position = Vector3.Lerp(m_left.position, m_right.position, m_progressBar.Value);
		tk2dCamera tk2dCamera2 = tk2dCamera.CameraForLayer(base.gameObject.layer);
		if (tk2dCamera2 != null)
		{
			return tk2dCamera2.ScreenCamera.WorldToScreenPoint(position);
		}
		return Vector3.zero;
	}

	private void Update()
	{
		if (m_isDragged && !Singleton<UISelector>.Instance.IsPrimary())
		{
			tk2dCamera tk2dCamera2 = tk2dCamera.CameraForLayer(base.gameObject.layer);
			if (tk2dCamera2 != null)
			{
				Vector3 vector = tk2dCamera2.ScreenCamera.ScreenToWorldPoint(Input.mousePosition);
				vector.z = 0f;
				float value = Mathf.InverseLerp(m_left.position.x, m_right.position.x, vector.x);
				SetValue(Mathf.Clamp01(value));
			}
		}
	}
}
