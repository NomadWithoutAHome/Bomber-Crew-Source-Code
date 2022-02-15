using System;
using UnityEngine;

public class tk2dOffsetFromAnimEvent : MonoBehaviour
{
	[SerializeField]
	private tk2dSpriteAnimator m_spriteAnimator;

	[SerializeField]
	private Transform[] m_transformsToOffset;

	private void OnEnable()
	{
		tk2dSpriteAnimator spriteAnimator = m_spriteAnimator;
		spriteAnimator.AnimationEventTriggered = (Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>)Delegate.Combine(spriteAnimator.AnimationEventTriggered, new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>(HandleAnimationEvent));
	}

	private void HandleAnimationEvent(tk2dSpriteAnimator animator, tk2dSpriteAnimationClip clip, int frameNo)
	{
		tk2dSpriteAnimationFrame frame = clip.GetFrame(frameNo);
		Vector3 localPosition = new Vector3(0f, frame.eventInt, 0f);
		Transform[] transformsToOffset = m_transformsToOffset;
		foreach (Transform transform in transformsToOffset)
		{
			transform.localPosition = localPosition;
		}
	}

	private void OnDestroy()
	{
		tk2dSpriteAnimator spriteAnimator = m_spriteAnimator;
		spriteAnimator.AnimationEventTriggered = (Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>)Delegate.Remove(spriteAnimator.AnimationEventTriggered, new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>(HandleAnimationEvent));
	}
}
