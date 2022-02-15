using System;
using UnityEngine;

namespace BomberCrewCommon;

public static class AimingUtils
{
	public static Vector3 GetAimAheadTarget(Vector3 muzzle, Vector3 target, Vector3 targetVelocity, float bulletSpeed, float inaccuracyFactor, bool isGroundBasedOrFriendly)
	{
		float magnitude = (target - muzzle).magnitude;
		if (magnitude > 2000f && !isGroundBasedOrFriendly)
		{
			return target;
		}
		float num = Vector3.Dot((muzzle - target).normalized, targetVelocity.normalized);
		float magnitude2 = targetVelocity.magnitude;
		float sqrMagnitude = targetVelocity.sqrMagnitude;
		float num2 = bulletSpeed * bulletSpeed;
		float sqrMagnitude2 = (target - muzzle).sqrMagnitude;
		float num3 = num2 - sqrMagnitude;
		float num4 = 2f * magnitude * magnitude2 * num;
		float num5 = 0f - sqrMagnitude2;
		float f = num4 * num4 - 4f * num3 * num5;
		float num6 = (0f - num4 + Mathf.Sqrt(f)) / (2f * num3);
		float num7 = (0f - num4 - Mathf.Sqrt(f)) / (2f * num3);
		float num8 = 0f;
		num8 = ((!(num6 >= 0f) || !(num7 >= 0f)) ? Mathf.Max(num6, num7) : Mathf.Min(num6, num7));
		num8 += inaccuracyFactor;
		if (float.IsNaN(num8))
		{
			return target;
		}
		return target + num8 * targetVelocity;
	}

	public static float GetInaccuracyFactor(float factor, float phase, float fixedFactor)
	{
		return Mathf.Sin(phase * (float)Math.PI * 2f) * factor + fixedFactor;
	}

	public static float GetInaccuracyFighter(float nightFactor, bool corkscrew, bool isLitUp, float phase)
	{
		float num = 0f;
		if (!isLitUp)
		{
			num += 1f + nightFactor * 2f;
		}
		return GetInaccuracyFactor(num, phase, corkscrew ? 3 : 0);
	}

	public static float GetInaccuracyAATurret(float nightFactor, bool corkscrew, bool isLitUp, float lockTime, float phase)
	{
		float value = lockTime / 20f;
		float num = 2f + (1f - Mathf.Clamp01(value)) * 5f;
		num += nightFactor * 2f;
		if (isLitUp)
		{
			num /= 4f;
			if (num < 0.5f)
			{
				num = 0f;
			}
		}
		return GetInaccuracyFactor(num, phase, corkscrew ? 3 : 0);
	}

	public static Vector3 GetAimAheadTarget(Vector3 muzzle, Vector3 target, Vector3 targetVelocity, float bulletSpeed)
	{
		float magnitude = (target - muzzle).magnitude;
		if (magnitude > 2000f)
		{
			return target;
		}
		float num = Vector3.Dot((muzzle - target).normalized, targetVelocity.normalized);
		float magnitude2 = targetVelocity.magnitude;
		float sqrMagnitude = targetVelocity.sqrMagnitude;
		float num2 = bulletSpeed * bulletSpeed;
		float sqrMagnitude2 = (target - muzzle).sqrMagnitude;
		float num3 = num2 - sqrMagnitude;
		float num4 = 2f * magnitude * magnitude2 * num;
		float num5 = 0f - sqrMagnitude2;
		float f = num4 * num4 - 4f * num3 * num5;
		float num6 = (0f - num4 + Mathf.Sqrt(f)) / (2f * num3);
		float num7 = (0f - num4 - Mathf.Sqrt(f)) / (2f * num3);
		float num8 = 0f;
		num8 = ((!(num6 >= 0f) || !(num7 >= 0f)) ? Mathf.Max(num6, num7) : Mathf.Min(num6, num7));
		if (float.IsNaN(num8))
		{
			return target;
		}
		return target + num8 * targetVelocity;
	}
}
