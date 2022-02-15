using UnityEngine;

namespace Common;

public abstract class CustomActivator : MonoBehaviour
{
	protected bool m_active;

	public virtual void OnDisable()
	{
		m_active = false;
	}

	public abstract void Show(float? delay, ActivatorCallback callback);

	public abstract void Hide(ActivatorCallback callback);

	public abstract bool IsActive();
}
