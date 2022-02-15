using UnityEngine;

public class DamageFlash : MonoBehaviour
{
	private enum CurrentState
	{
		OK,
		LowHealth,
		Destroyed
	}

	[SerializeField]
	private FlashManager m_flashManager;

	private FlashManager.ActiveFlash m_activeFlash;

	private FlashManager.ActiveFlash m_lowHealthFlash;

	private FlashManager.ActiveFlash m_destroyedFlash;

	private void Awake()
	{
		if (m_flashManager == null)
		{
			m_flashManager = GetComponent<FlashManager>();
		}
	}

	public void DoFlash()
	{
		if (m_destroyedFlash == null)
		{
			m_activeFlash = m_flashManager.AddOrUpdateFlash(0.3f, 10f, 0f, 1, 1f, Color.white / 2f, m_activeFlash);
		}
		else
		{
			m_activeFlash = m_flashManager.AddOrUpdateFlash(0.3f, 10f, 0f, 1, 1f, new Color(0.2f, 0.2f, 0.2f), m_activeFlash);
		}
	}

	public void DoUpgradeFlash()
	{
		m_flashManager.AddOrUpdateFlash(1f, 0f, 1f, 255, 1f, new Color(0.75f, 0f, 0.95f), null);
	}

	public void DoLowHealth(bool critical)
	{
		m_lowHealthFlash = m_flashManager.AddOrUpdateFlash(0f, (!critical) ? 0.5f : 2f, 0f, 255, 1f, Color.red / 3f, m_lowHealthFlash);
		if (m_destroyedFlash != null)
		{
			m_flashManager.FadeExistingFlash(m_destroyedFlash, 0.5f, 1f);
			m_destroyedFlash = null;
		}
	}

	public void DoDestroyed()
	{
		m_destroyedFlash = m_flashManager.AddOrUpdateFlash(0f, 0f, 0f, 255, 1f, Color.red / 3f, m_destroyedFlash);
		if (m_lowHealthFlash != null)
		{
			m_flashManager.FadeExistingFlash(m_lowHealthFlash, 0.5f, 1f);
			m_lowHealthFlash = null;
		}
	}

	public void ReturnToNormal()
	{
		if (m_destroyedFlash != null)
		{
			m_flashManager.FadeExistingFlash(m_destroyedFlash, 0.5f, 1f);
			m_destroyedFlash = null;
		}
		if (m_lowHealthFlash != null)
		{
			m_flashManager.FadeExistingFlash(m_lowHealthFlash, 0.5f, 1f);
			m_lowHealthFlash = null;
		}
	}
}
