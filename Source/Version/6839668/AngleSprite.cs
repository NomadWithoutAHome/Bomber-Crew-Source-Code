using UnityEngine;

public class AngleSprite : MonoBehaviour
{
	[SerializeField]
	private Transform m_rootTransform;

	[SerializeField]
	private bool m_debug;

	[SerializeField]
	private Transform[] m_boneTransforms;

	[SerializeField]
	private tk2dSpriteAnimator[] m_spriteAnimators;

	private void LateUpdate()
	{
		for (int i = 0; i < m_boneTransforms.Length; i++)
		{
			m_spriteAnimators[i].SetFrame(GetFrameFromAngle(GetAngleOfBone(m_boneTransforms[i]), m_spriteAnimators[i]));
			m_boneTransforms[i].localEulerAngles = new Vector3(m_boneTransforms[i].localEulerAngles.x, Mathf.Round(m_boneTransforms[i].localEulerAngles.y / 22.5f) * 22.5f, m_boneTransforms[i].localEulerAngles.z);
		}
		for (int j = 0; j < m_spriteAnimators.Length; j++)
		{
			m_spriteAnimators[j].transform.position = m_boneTransforms[j].position;
		}
	}

	private float GetAngleOfBone(Transform boneTransform)
	{
		Vector3 right = boneTransform.right;
		float num = Mathf.Atan2(right.y, right.x) * 57.29578f;
		if (num < 0f)
		{
			num += 360f;
		}
		return num;
	}

	private int GetFrameFromAngle(float angle, tk2dSpriteAnimator spriteAnimator)
	{
		int num = Mathf.RoundToInt(angle / 22.5f) % 16;
		SetSpriteFlip(num, spriteAnimator);
		return num;
	}

	private void SetSpriteFlip(int frame, tk2dSpriteAnimator spriteAnimator)
	{
		if (frame == 0)
		{
			spriteAnimator.Sprite.FlipY = false;
		}
		else
		{
			spriteAnimator.Sprite.FlipY = true;
		}
		if (frame >= 9 && frame <= 15)
		{
			spriteAnimator.Sprite.FlipX = true;
		}
		else
		{
			spriteAnimator.Sprite.FlipX = false;
		}
	}
}
