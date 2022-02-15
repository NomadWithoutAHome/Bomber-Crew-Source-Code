using UnityEngine;

namespace Common;

public static class GameObjectExtensions
{
	public static void CustomActivate(this GameObject g, bool active, float? delay = null, ActivatorCallback callback = null)
	{
		CustomActivator component = g.GetComponent<CustomActivator>();
		if (component != null)
		{
			if (active)
			{
				g.SetActive(value: true);
				component.Show(delay, callback);
			}
			else
			{
				component.Hide(callback);
			}
		}
		else if (active)
		{
			g.SetActive(value: true);
			callback?.Invoke();
		}
		else
		{
			g.SetActive(value: false);
			callback?.Invoke();
		}
	}

	public static bool IsActivated(this GameObject g)
	{
		CustomActivator component = g.GetComponent<CustomActivator>();
		if (component != null)
		{
			return component.IsActive();
		}
		return g.activeInHierarchy;
	}
}
