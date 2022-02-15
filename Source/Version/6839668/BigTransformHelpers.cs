using UnityEngine;

public static class BigTransformHelpers
{
	public static BigTransform btransform(this GameObject obj)
	{
		return obj.GetComponent<BigTransform>();
	}
}
