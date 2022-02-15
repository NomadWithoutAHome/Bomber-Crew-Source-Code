using UnityEngine;

public abstract class ParameterModifierBase : MonoBehaviour
{
	public virtual float GetVolumeMultiplier(int linkedObjectId)
	{
		return 1f;
	}

	public virtual float GetPitchMultiplier(int linkedObjectId)
	{
		return 1f;
	}

	public virtual bool IsGlobalOptimised()
	{
		return false;
	}
}
