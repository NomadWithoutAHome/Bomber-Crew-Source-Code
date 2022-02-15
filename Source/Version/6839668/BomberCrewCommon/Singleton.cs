using UnityEngine;
using UnityEngine.SceneManagement;

namespace BomberCrewCommon;

public class Singleton<T> : MonoBehaviour where T : Object
{
	private static T s_instance;

	private static bool s_found;

	private static bool s_setUpHooks;

	public static T Instance
	{
		get
		{
			if (!s_found)
			{
				s_instance = Object.FindObjectOfType<T>();
				if ((Object)s_instance != (Object)null)
				{
					s_found = true;
				}
				if (!s_setUpHooks)
				{
					SceneManager.sceneUnloaded += UnloadScene;
					s_setUpHooks = true;
				}
				return s_instance;
			}
			return s_instance;
		}
	}

	public static T InstanceDontFetch
	{
		get
		{
			if (!s_found)
			{
				return (T)null;
			}
			return s_instance;
		}
	}

	private static void UnloadScene(Scene s)
	{
		s_found = false;
	}
}
