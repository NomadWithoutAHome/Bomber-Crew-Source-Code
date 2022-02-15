using UnityEngine;

public class CrewAnimationTriggers : StateMachineBehaviour
{
	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		animator.transform.parent.gameObject.GetComponent<CrewmanGraphics>().OnStateExit();
	}
}
