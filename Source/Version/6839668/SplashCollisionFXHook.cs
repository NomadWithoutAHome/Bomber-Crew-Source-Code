using BomberCrewCommon;
using UnityEngine;

public class SplashCollisionFXHook : MonoBehaviour
{
	[SerializeField]
	private float m_velocityThreshold = 4f;

	private float m_timeSinceSplash;

	private int m_numSplashesRecently;

	private RaycastHit[] buffer = new RaycastHit[128];

	private void OnTriggerEnter(Collider c)
	{
		DoSplashFX(c);
	}

	private void OnTriggerStay(Collider c)
	{
		DoSplashFX(c);
	}

	private void Update()
	{
		m_timeSinceSplash += Time.deltaTime;
		if (m_timeSinceSplash > 0.1f)
		{
			m_numSplashesRecently = 0;
		}
	}

	private void DoSplashFX(Collider c)
	{
		Rigidbody attachedRigidbody = c.attachedRigidbody;
		if (!(attachedRigidbody != null) || !(attachedRigidbody.velocity.magnitude > m_velocityThreshold) || m_numSplashesRecently >= 20)
		{
			return;
		}
		Vector3 center = c.transform.position + new Vector3(30f, 0f, 0f);
		center.y = -0.1f;
		int num = Physics.BoxCastNonAlloc(center, new Vector3(1f, 0.2f, 30f), new Vector3(-1f, 0f, 0f), buffer, Quaternion.identity, 60f);
		for (int i = 0; i < num; i++)
		{
			if (buffer[i].collider == c)
			{
				Vector3 point = buffer[i].point;
				point.y = 0f;
				Singleton<GlobalEffects>.Instance.SpawnImpactEffectsSea(point);
				m_numSplashesRecently++;
				m_timeSinceSplash = 0f;
			}
		}
	}
}
