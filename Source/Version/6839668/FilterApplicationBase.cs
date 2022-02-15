using UnityEngine;

public abstract class FilterApplicationBase : MonoBehaviour
{
	public abstract void UpdateFor(PooledAudioSource playingSource, int linkedObjectId);
}
