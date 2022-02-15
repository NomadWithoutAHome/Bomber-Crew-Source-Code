using UnityEngine;

public class RandomAnimationSpeed : MonoBehaviour
{
	[SerializeField]
	private Animation[] m_animations;

	[SerializeField]
	private Vector2 m_speedMinMax = new Vector2(0.9f, 1.1f);

	[SerializeField]
	private bool m_randomStartTime;

	[SerializeField]
	private bool m_syncAllAnimations;

	private void Start()
	{
		if (m_syncAllAnimations)
		{
			float speed = Random.Range(m_speedMinMax.x, m_speedMinMax.y);
			float normalizedTime = Random.Range(0f, 1f);
			Animation[] animations = m_animations;
			foreach (Animation animation in animations)
			{
				string text = animation.clip.name;
				animation[text].speed = speed;
				if (m_randomStartTime)
				{
					animation[text].normalizedTime = normalizedTime;
				}
			}
			return;
		}
		Animation[] animations2 = m_animations;
		foreach (Animation animation2 in animations2)
		{
			string text2 = animation2.clip.name;
			float speed2 = Random.Range(m_speedMinMax.x, m_speedMinMax.y);
			animation2[text2].speed = speed2;
			if (m_randomStartTime)
			{
				animation2[text2].normalizedTime = Random.Range(0f, 1f);
			}
		}
	}
}
