using BomberCrewCommon;
using UnityEngine;

public class WalkableArea : MonoBehaviour
{
	[SerializeField]
	private tk2dUIItem m_uiItem;

	[SerializeField]
	private bool m_ignoreExternalWalkZones;

	private bool m_isHovered;

	private bool m_noHits;

	private RaycastHit[] buffer = new RaycastHit[128];

	private void Awake()
	{
		m_uiItem.OnClick += OnClickToWalk;
		m_uiItem.OnHoverOver += OnHoverWalkArea;
		m_uiItem.OnHoverOut += OnUnhoverWalkArea;
	}

	private void OnEnable()
	{
		Singleton<ContextControl>.Instance.RegisterWalkableArea(this);
	}

	private void OnDisable()
	{
		if (Singleton<ContextControl>.Instance != null)
		{
			Singleton<ContextControl>.Instance.DeRegisterWalkableArea(this);
		}
	}

	private void OnClickToWalk()
	{
		Vector2 screenPosition = Singleton<UISelector>.Instance.GetScreenPosition();
		Ray ray = Camera.main.ScreenPointToRay(screenPosition);
		int num = Physics.RaycastNonAlloc(ray, buffer);
		for (int i = 0; i < num; i++)
		{
			if (buffer[i].collider == m_uiItem.GetComponent<Collider>())
			{
				Singleton<ContextControl>.Instance.ClickOnWalkableArea(buffer[i].point, this, m_ignoreExternalWalkZones);
			}
		}
	}

	private void OnHoverWalkArea()
	{
		m_noHits = false;
		m_isHovered = true;
		Update();
	}

	private void OnUnhoverWalkArea()
	{
		m_noHits = false;
		m_isHovered = false;
		Singleton<ContextControl>.Instance.UnhoverWalkableArea(this);
	}

	public tk2dUIItem GetUIItem()
	{
		return m_uiItem;
	}

	private void Update()
	{
		if (!m_isHovered)
		{
			return;
		}
		Vector2 screenPosition = Singleton<UISelector>.Instance.GetScreenPosition();
		Ray ray = Camera.main.ScreenPointToRay(screenPosition);
		int num = Physics.RaycastNonAlloc(ray, buffer);
		int num2 = 0;
		for (int i = 0; i < num; i++)
		{
			if (buffer[i].collider == m_uiItem.GetComponent<Collider>())
			{
				num2++;
				Singleton<ContextControl>.Instance.HoverWalkableArea(buffer[i].point, this, m_ignoreExternalWalkZones);
			}
		}
		if (num2 == 0 && !m_noHits)
		{
			m_noHits = true;
			Singleton<ContextControl>.Instance.UnhoverWalkableArea(this);
		}
		if (m_noHits && num2 > 0)
		{
			m_noHits = false;
		}
	}
}
