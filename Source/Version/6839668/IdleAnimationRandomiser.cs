using UnityEngine;

public class IdleAnimationRandomiser : StateMachineBehaviour
{
	[SerializeField]
	private int m_idleAnimationCount;

	[SerializeField]
	private string m_idleIndexId = "IdleIndex";

	[SerializeField]
	private string m_idleSpeedId = "IdleSpeed";

	private Vector2 m_randomSpeedRange = new Vector2(0.9f, 1.1f);

	private new void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
	{
		animator.SetFloat(m_idleSpeedId, Random.Range(m_randomSpeedRange.x, m_randomSpeedRange.y));
		animator.SetInteger(m_idleIndexId, Mathf.FloorToInt(Random.Range(0, m_idleAnimationCount)));
	}
}
